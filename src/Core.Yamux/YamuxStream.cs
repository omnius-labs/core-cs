using Omnius.Core.Base;

namespace Omnius.Core.Yamux;

internal enum YamuxStreamState
{
    Open,
    SendClosed,
    RecvClosed,
    Closed,
}

internal enum YamuxPendingFlag
{
    None,
    Syn,
    Ack,
}

public sealed class YamuxStream : Stream
{
    private readonly YamuxConnection _connection;
    private readonly YamuxConfig _config;
    private readonly uint _id;
    private readonly bool _isOutbound;
    private readonly IBytesPool _bytesPool;

    private uint _sendWindow;
    private uint _receiveWindow;
    private readonly IncomingBytes _incomingBytes = new IncomingBytes();
    private YamuxStreamState _state = YamuxStreamState.Open;
    private readonly uint _maxReceiveWindow;
    private YamuxPendingFlag _pendingFlag;
    private bool _awaitingRemoteAck;
    private long _bufferedBytes;
    private bool _connectionClosed;
    private ManualResetSignal _sendWindowSignal = new(false);

    private readonly object _lockObject = new();

    private YamuxStream(YamuxConnection connection, YamuxConfig config, uint streamId, bool outbound, uint sendWindow, uint receiveWindow, YamuxPendingFlag pendingFlag, IBytesPool bytesPool)
    {
        _connection = connection;
        _config = config;
        _id = streamId;
        _isOutbound = outbound;
        _sendWindow = sendWindow;
        _receiveWindow = receiveWindow;
        _maxReceiveWindow = YamuxConstants.DefaultCredit;
        _pendingFlag = pendingFlag;
        _awaitingRemoteAck = outbound;
        _bytesPool = bytesPool;
    }

    public uint Id => _id;
    internal bool IsOutbound => _isOutbound;

    internal bool IsPendingAck
    {
        get
        {
            lock (_lockObject)
            {
                return _awaitingRemoteAck;
            }
        }
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    internal static YamuxStream CreateInbound(YamuxConnection connection, YamuxConfig config, uint streamId, uint initialSendWindow, IBytesPool bytesPool)
    {
        return new YamuxStream(connection, config, streamId, outbound: false, sendWindow: initialSendWindow, receiveWindow: YamuxConstants.DefaultCredit, pendingFlag: YamuxPendingFlag.Ack, bytesPool);
    }

    internal static YamuxStream CreateOutbound(YamuxConnection connection, YamuxConfig config, uint streamId, IBytesPool bytesPool)
    {
        return new YamuxStream(connection, config, streamId, outbound: true, sendWindow: YamuxConstants.DefaultCredit, receiveWindow: YamuxConstants.DefaultCredit, pendingFlag: YamuxPendingFlag.Syn, bytesPool);
    }

    internal bool MarkAcknowledgedByRemote()
    {
        lock (_lockObject)
        {
            if (!_awaitingRemoteAck) return false;
            _awaitingRemoteAck = false;
            return true;
        }
    }

    internal void ReceiveData(OwnedBytes data, bool fin)
    {
        bool closeRead = false;
        bool notify = false;
        var dataLength = data.Memory.Length;

        lock (_lockObject)
        {
            if (_state == YamuxStreamState.Closed)
            {
                data.Dispose();
                return;
            }

            if (dataLength > _receiveWindow)
            {
                data.Dispose();
                throw new YamuxProtocolException($"Stream {this.Id}: frame exceeds receive window.");
            }

            _receiveWindow -= (uint)dataLength;
            _bufferedBytes += dataLength;

            if (fin)
            {
                closeRead = true;
                notify = this.TransitionToRecvClosedLocked();
            }
        }

        if (dataLength > 0)
        {
            if (!_incomingBytes.TryWrite(data))
            {
                lock (_lockObject)
                {
                    _receiveWindow += (uint)dataLength;
                    _bufferedBytes -= dataLength;
                    if (_bufferedBytes < 0) _bufferedBytes = 0;
                }
            }
        }
        else
        {
            data.Dispose();
        }

        if (closeRead)
        {
            _incomingBytes.Complete();
        }

        if (notify)
        {
            _connection.NotifyStreamClosed(this.Id, this);
        }
    }

    internal void ReceiveWindowUpdate(uint credit, bool fin)
    {
        bool notify = false;

        lock (_lockObject)
        {
            _sendWindow = checked(_sendWindow + credit);

            if (fin)
            {
                notify = this.TransitionToRecvClosedLocked();
            }

            if (_sendWindow > 0)
            {
                _sendWindowSignal.Set();
            }
        }

        if (notify)
        {
            _connection.NotifyStreamClosed(this.Id, this);
        }
    }

    internal void ReceiveReset()
    {
        bool notify = false;

        lock (_lockObject)
        {
            if (_state == YamuxStreamState.Closed) return;
            _state = YamuxStreamState.Closed;
            notify = true;
        }

        _incomingBytes.CompleteAndDrain();
        _sendWindowSignal.Set();

        if (notify)
        {
            _connection.NotifyStreamClosed(this.Id, this);
        }
    }

    internal void MarkConnectionClosed()
    {
        lock (_lockObject)
        {
            _connectionClosed = true;
            _state = YamuxStreamState.Closed;
        }

        if (_config.ReadAfterClose)
        {
            _incomingBytes.Complete();
        }
        else
        {
            _incomingBytes.CompleteAndDrain();
        }

        _sendWindowSignal.Set();
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (buffer.Length == 0) return 0;

        if (!_config.ReadAfterClose)
        {
            lock (_lockObject)
            {
                if (_connectionClosed)
                {
                    return 0;
                }
            }
        }

        var read = await _incomingBytes.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        if (read > 0) this.OnBytesConsumed(read);
        return read;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return this.ReadAsync(buffer.AsMemory(offset, count)).GetAwaiter().GetResult();
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (buffer.Length == 0) return;

        var offset = 0;

        while (offset < buffer.Length)
        {
            uint allowed;
            FrameFlags flags;

            lock (_lockObject)
            {
                if (!this.CanWriteLocked()) throw new IOException("Stream is closed for writing.");
                if (_connectionClosed) throw new YamuxConnectionClosedException("Connection is closed for writing.");
            }

            await this.WaitForSendWindowAsync(cancellationToken).ConfigureAwait(false);

            lock (_lockObject)
            {
                if (!this.CanWriteLocked()) throw new IOException("Stream is closed for writing.");
                if (_connectionClosed) throw new YamuxConnectionClosedException("Connection is closed for writing.");

                if (_sendWindow == 0) continue;

                var remaining = buffer.Length - offset;
                var windowAllowed = Math.Min(_sendWindow, (uint)remaining);
                var splitAllowed = Math.Min(windowAllowed, (uint)_config.SplitSendSize);
                allowed = splitAllowed;
                _sendWindow -= allowed;

                if (_sendWindow == 0)
                {
                    _sendWindowSignal.Reset();
                }

                flags = this.ApplyPendingFlagLocked(FrameFlags.None);
            }

            var slice = buffer.Slice(offset, (int)allowed);
            var frame = Frame.Data(this.Id, slice, flags, _bytesPool);
            _connection.EnqueueFrame(frame);
            offset += (int)allowed;
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        this.WriteAsync(buffer.AsMemory(offset, count)).GetAwaiter().GetResult();
    }

    public override void Flush()
    {
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        Frame? frame = null;
        bool notify = false;

        lock (_lockObject)
        {
            if (_state == YamuxStreamState.Closed || _state == YamuxStreamState.SendClosed) return;

            var flags = this.ApplyPendingFlagLocked(FrameFlags.Fin);
            frame = Frame.Data(this.Id, ReadOnlyMemory<byte>.Empty, flags, _bytesPool);

            _state = _state == YamuxStreamState.RecvClosed
                ? YamuxStreamState.Closed
                : YamuxStreamState.SendClosed;
            notify = _state == YamuxStreamState.Closed;
        }

        if (frame is not null)
        {
            _connection.EnqueueFrame(frame);
        }

        if (notify)
        {
            _connection.NotifyStreamClosed(this.Id, this);
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.CloseAsync().GetAwaiter().GetResult();
            _incomingBytes.CompleteAndDrain();
        }

        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        await this.CloseAsync().ConfigureAwait(false);
        _incomingBytes.CompleteAndDrain();
        await base.DisposeAsync().ConfigureAwait(false);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    private void OnBytesConsumed(int bytes)
    {
        Frame? update = null;

        lock (_lockObject)
        {
            _bufferedBytes -= bytes;
            if (_bufferedBytes < 0) _bufferedBytes = 0;

            if (!this.CanReadLocked()) return;

            var bytesReceived = (long)_maxReceiveWindow - _receiveWindow;
            var pending = bytesReceived - _bufferedBytes;
            if (pending < _maxReceiveWindow / 2) return;

            var credit = (uint)Math.Min(pending, uint.MaxValue);
            _receiveWindow += credit;

            var flags = this.ApplyPendingFlagLocked(FrameFlags.None);
            update = Frame.WindowUpdate(this.Id, credit, flags);
        }

        if (update is not null)
        {
            _connection.EnqueueFrame(update);
        }
    }

    private bool CanReadLocked()
    {
        return _state != YamuxStreamState.RecvClosed && _state != YamuxStreamState.Closed;
    }

    private bool CanWriteLocked()
    {
        return _state != YamuxStreamState.SendClosed && _state != YamuxStreamState.Closed;
    }

    private FrameFlags ApplyPendingFlagLocked(FrameFlags flags)
    {
        if (_pendingFlag == YamuxPendingFlag.Syn)
        {
            _pendingFlag = YamuxPendingFlag.None;
            return flags | FrameFlags.Syn;
        }

        if (_pendingFlag == YamuxPendingFlag.Ack)
        {
            _pendingFlag = YamuxPendingFlag.None;
            return flags | FrameFlags.Ack;
        }

        return flags;
    }

    private bool TransitionToRecvClosedLocked()
    {
        if (_state == YamuxStreamState.Closed) return false;

        _state = _state == YamuxStreamState.SendClosed
            ? YamuxStreamState.Closed
            : YamuxStreamState.RecvClosed;

        return _state == YamuxStreamState.Closed;
    }

    private async Task WaitForSendWindowAsync(CancellationToken cancellationToken)
    {
        lock (_lockObject)
        {
            if (_sendWindow > 0) return;
        }

        await _sendWindowSignal.WaitAsync(cancellationToken).ConfigureAwait(false);
    }
}
