using System.Buffers;
using Microsoft.Extensions.Logging;
using Omnius.Core.Base;
using Omnius.Yamux.Internal;

namespace Omnius.Yamux;

public enum YamuxStreamState
{
    Init,
    SYNSent,
    SYNReceived,
    Established,
    LocalClose,
    RemoteClose,
    Closed,
    Reset
}

public partial class YamuxStream
{
    private readonly YamuxMultiplexer _multiplexer;
    private readonly uint _streamId;
    private readonly StreamStateMachine _stateMachine;
    private readonly StreamFlowControl _flowControl;
    private readonly ArrayPool<byte> _bytesPool;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger _logger;

    private readonly Header _controlHeader = new();
    private readonly AsyncLock _controlHeaderLock = new();

    private readonly CircularBuffer _receiveBuffer;
    private readonly AsyncLock _receiveLock = new();

    private readonly Header _sendHeader = new();
    private readonly AsyncLock _sendLock = new();
    private readonly AsyncLock _sendWindowLock = new();

    private readonly ManualResetEventSlim _receiveEvent = new ManualResetEventSlim(false);
    private readonly ManualResetEventSlim _sendEvent = new ManualResetEventSlim(false);
    private readonly ManualResetEventSlim _establishEvent = new ManualResetEventSlim(false);
    private ITimer? _closeTimer;

    private readonly CancellationToken _multiplexerCancellationToken;
    private readonly CancellationTokenSource _streamCancellationTokenSource = new();

    private int _closed = 0;

    internal YamuxStream(YamuxMultiplexer multiplexer, uint streamId, YamuxStreamState state, ArrayPool<byte> bytesPool, TimeProvider timeProvider, ILogger logger, CancellationToken multiplexerCancellationToken)
    {
        _multiplexer = multiplexer;
        _streamId = streamId;
        _bytesPool = bytesPool;
        _timeProvider = timeProvider;
        _logger = logger;
        _multiplexerCancellationToken = multiplexerCancellationToken;

        _stateMachine = new StreamStateMachine(state);
        _flowControl = new StreamFlowControl(multiplexer.Options.MaxStreamWindow);

        _receiveBuffer = new CircularBuffer(_bytesPool);
    }

    public YamuxMultiplexer Multiplexer => _multiplexer;
    public uint StreamId => _streamId;
    public YamuxStreamState State => _stateMachine.State;

    private CancellationTokenSource GetMixedCancellationToken(CancellationToken cancellationToken = default)
    {
        return CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _streamCancellationTokenSource.Token, _multiplexerCancellationToken);
    }

    private void NotifyWaiting()
    {
        _receiveEvent.Set();
        _sendEvent.Set();
        _establishEvent.Set();
    }

    internal async ValueTask WaitForEstablishedAsync(CancellationToken cancellationToken = default)
    {
        await _establishEvent.WaitHandle.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<int> InternalReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (buffer.Length == 0) return 0;

        for (; ; )
        {
            YamuxStreamState state = _stateMachine.State;
            if (state == YamuxStreamState.LocalClose || state == YamuxStreamState.RemoteClose || state == YamuxStreamState.Closed) return 0;
            if (state == YamuxStreamState.Reset) throw new YamuxException(YamuxErrorCode.StreamReset);

            bool available;

            using (_receiveLock.Lock(cancellationToken))
            {
                available = _receiveBuffer.Reader.Available();
                if (!available) _receiveEvent.Reset();
            }

            if (!available)
            {
                await _receiveEvent.WaitHandle.WaitAsync(cancellationToken).ConfigureAwait(false);
                continue;
            }

            int readLength;

            using (_receiveLock.Lock(cancellationToken))
            {
                readLength = _receiveBuffer.Reader.Read(buffer);
            }

            await this.SendWindowUpdateAsync(cancellationToken).ConfigureAwait(false);

            return readLength;
        }
    }

    private async ValueTask InternalWriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (buffer.Length == 0) throw new ArgumentOutOfRangeException(nameof(buffer));

        using (await _sendLock.LockAsync(cancellationToken).ConfigureAwait(false))
        {
            int remain = buffer.Length;
            while (remain > 0)
            {
                var writeLength = await this.InternalWriteSubAsync(buffer.Slice(buffer.Length - remain), cancellationToken).ConfigureAwait(false);
                remain -= writeLength;
            }
        }
    }

    private async ValueTask<int> InternalWriteSubAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        for (; ; )
        {
            YamuxStreamState state = _stateMachine.State;
            if (state == YamuxStreamState.LocalClose || state == YamuxStreamState.RemoteClose || state == YamuxStreamState.Closed) throw new YamuxException(YamuxErrorCode.StreamClosed);
            if (state == YamuxStreamState.Reset) throw new YamuxException(YamuxErrorCode.StreamReset);

            uint sendWindow;

            using (_sendWindowLock.Lock())
            {
                sendWindow = _flowControl.SendWindow;
                if (sendWindow == 0) _sendEvent.Reset();
            }

            if (sendWindow == 0)
            {
                await _sendEvent.WaitHandle.WaitAsync(cancellationToken).ConfigureAwait(false);
                continue;
            }

            var length = Math.Min((int)sendWindow, buffer.Length);

            var flags = this.ComputeSendFlags();
            _sendHeader.encode(MessageType.Data, flags, _streamId, (uint)length);

            await _multiplexer.SendFrameAsync(_sendHeader, buffer.Slice(0, length), cancellationToken).ConfigureAwait(false);

            using (_sendWindowLock.Lock())
            {
                _flowControl.ConsumeSendWindow((uint)length);
            }

            return length;
        }
    }

    public async ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref _closed, 1, 0) != 0) return;

        try
        {
            var change = _stateMachine.CloseLocal();

            using (await _controlHeaderLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                var flags = this.ComputeSendFlags();
                flags |= MessageFlag.FIN;
                _controlHeader.encode(MessageType.WindowUpdate, flags, _streamId, 0);
                await _multiplexer.SendFrameAsync(_controlHeader, default, cancellationToken).ConfigureAwait(false);
            }

            this.ApplyStateChange(change);
        }
        catch (YamuxException)
        {
            _logger.LogWarning("yamux: invalid state for close: {0}", _stateMachine.State);
        }
    }

    private async Task OnCloseTimeoutAsync()
    {
        try
        {
            this.ForceClose();

            _multiplexer.RemoveStream(_streamId);

            using (await _sendLock.LockAsync().ConfigureAwait(false))
            {
                var header = new Header(MessageType.WindowUpdate, MessageFlag.RST, _streamId, 0);
                await _multiplexer.SendFrameFireAndForgetAsync(header, ReadOnlyMemory<byte>.Empty).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "yamux: stream {0} close timeout handling failed", _streamId);
        }
    }

    internal void ForceClose()
    {
        _stateMachine.ForceClose();

        this.NotifyWaiting();
    }

    internal async ValueTask SendWindowUpdateAsync(CancellationToken cancellationToken = default)
    {
        using (await _controlHeaderLock.LockAsync(cancellationToken).ConfigureAwait(false))
        {
            uint? delta;
            MessageFlag flags;

            using (await _receiveLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                flags = this.ComputeSendFlags();
                uint bufferedBytes = (uint)_receiveBuffer.Writer.WrittenBytes;
                delta = _flowControl.TryUpdateReceiveWindowAndGetDelta(bufferedBytes, flags != MessageFlag.None);
                if (delta == null) return;
            }

            _controlHeader.encode(MessageType.WindowUpdate, flags, _streamId, delta.Value);
            await _multiplexer.SendFrameAsync(_controlHeader, default, cancellationToken).ConfigureAwait(false);
        }
    }

    internal MessageFlag ComputeSendFlags()
    {
        return _stateMachine.ComputeSendFlags();
    }

    internal void AddSendWindow(Header header)
    {
        this.ProcessReceivedFlags(header.Flags);

        using (_sendWindowLock.Lock())
        {
            _flowControl.AddSendWindow(header.Length);
            _sendEvent.Set();
        }
    }

    internal async ValueTask EnqueueReadBytesAsync(Header header, Stream reader, CancellationToken cancellationToken = default)
    {
        this.ProcessReceivedFlags(header.Flags);

        if (header.Length == 0) return;

        using (await _receiveLock.LockAsync(cancellationToken).ConfigureAwait(false))
        {
            if (!_flowControl.CanConsumeReceiveWindow(header.Length))
            {
                _logger.LogWarning("yamux: stream {0} receive window exceeded", _streamId);
                throw new YamuxException(YamuxErrorCode.StreamReceiveWindowExceeded);
            }

            var buffer = _receiveBuffer.Writer.GetMemory((int)header.Length);
            int remain = (int)header.Length;

            try
            {
                while (remain > 0)
                {
                    var readLength = await reader.ReadAsync(buffer.Slice((int)header.Length - remain, remain), cancellationToken).ConfigureAwait(false);
                    if (readLength == 0)
                    {
                        _logger.LogError("yamux: stream {0} unexpected EOF", _streamId);
                        throw new YamuxException(YamuxErrorCode.ConnectionReceiveError);
                    }

                    _receiveBuffer.Writer.Advance(readLength);
                    _flowControl.ConsumeReceiveWindow((uint)readLength);
                    remain -= readLength;
                }
            }
            catch (YamuxException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "yamux: stream {0} read error", _streamId);
                throw new YamuxException(YamuxErrorCode.ConnectionReceiveError);
            }

            _receiveEvent.Set();
        }
    }

    internal void ProcessReceivedFlags(MessageFlag flags)
    {
        StateChange change;

        try
        {
            change = _stateMachine.ProcessReceivedFlags(flags);
        }
        catch (YamuxException)
        {
            if (flags.HasFlag(MessageFlag.FIN))
            {
                _logger.LogWarning("yamux: invalid state for FIN: {0}", _stateMachine.State);
            }

            throw;
        }

        this.ApplyStateChange(change);
    }

    private void ApplyStateChange(StateChange change)
    {
        if (change.NotifyEstablished)
        {
            _establishEvent.Set();
        }

        if (change.NotifyWaiters)
        {
            this.NotifyWaiting();
        }

        if (change.StopCloseTimer)
        {
            _closeTimer?.Dispose();
            _closeTimer = null;
        }

        if (change.StartCloseTimer && _multiplexer.Options.StreamCloseTimeout != Timeout.InfiniteTimeSpan)
        {
            _closeTimer?.Dispose();
            _closeTimer = _timeProvider.CreateTimer((_) => _ = this.OnCloseTimeoutAsync(), null, _multiplexer.Options.StreamCloseTimeout, Timeout.InfiniteTimeSpan);
        }

        if (change.RemoveStream)
        {
            _multiplexer.RemoveStream(_streamId);
        }
    }
}

public partial class YamuxStream : Stream
{
    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        this.DisposeAsync().AsTask().Wait();
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            await this.CloseAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // ignore
        }

        _receiveBuffer.Dispose();
        _sendEvent.Dispose();
        _establishEvent.Dispose();

        _streamCancellationTokenSource.Cancel();
        _streamCancellationTokenSource.Dispose();
    }

    public override bool CanRead => true;
    public override bool CanWrite => true;
    public override bool CanSeek => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return this.ReadAsync(buffer, offset, count).Result;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        this.WriteAsync(buffer, offset, count).Wait();
    }

    public override void Flush()
    {
        this.FlushAsync().Wait();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        return Task.CompletedTask;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        using var cancellationTokenSource = this.GetMixedCancellationToken(cancellationToken);
        return await this.InternalReadAsync(buffer.AsMemory(offset, count), cancellationTokenSource.Token).ConfigureAwait(false);
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        using var cancellationTokenSource = this.GetMixedCancellationToken(cancellationToken);
        await this.InternalWriteAsync(buffer.AsMemory(offset, count), cancellationTokenSource.Token).ConfigureAwait(false);
    }
}
