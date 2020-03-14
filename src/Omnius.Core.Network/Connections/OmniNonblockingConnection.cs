using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Network.Caps;

namespace Omnius.Core.Network.Connections
{
    public sealed class OmniNonblockingConnection : DisposableBase, IConnection
    {
        private readonly ICap _cap;
        private readonly int _maxSendByteCount;
        private readonly int _maxReceiveByteCount;
        private readonly BandwidthController? _bandwidthController;
        private readonly IBytesPool _bytesPool;

        private readonly byte[] _sendHeaderBuffer = new byte[4];
        private int _sendHeaderBufferPosition = -1;
        private readonly BytesHub _sendContentHub;
        private bool _sendContentHubWriterIsCompleted = false;

        private readonly byte[] _receiveHeaderBuffer = new byte[4];
        private int _receiveHeaderBufferPosition = 0;
        private int _receiveContentRemain = -1;
        private readonly BytesHub _receiveContentHub;
        private bool _receiveContentHubWriterIsCompleted = false;

        private readonly SemaphoreSlim _sendSemaphoreSlim;
        private readonly SemaphoreSlim _receiveSemaphoreSlim;

        private long _receivedByteCount;
        private long _sentByteCount;

        private readonly object _sendLockObject = new object();
        private readonly object _receiveLockObject = new object();

        public OmniNonblockingConnection(ICap cap, OmniNonblockingConnectionOptions option)
        {
            if (cap == null)
            {
                throw new ArgumentNullException(nameof(cap));
            }

            if (cap.IsBlocking == true)
            {
                throw new ArgumentException($"{nameof(cap)} is not nonblocking", nameof(cap));
            }

            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            _cap = cap;
            _maxSendByteCount = Math.Max(256, option.MaxSendByteCount);
            _maxReceiveByteCount = Math.Max(256, option.MaxReceiveByteCount);
            _bandwidthController = option.BandwidthController;
            _bytesPool = option.BufferPool ?? BytesPool.Shared;

            _sendContentHub = new BytesHub(_bytesPool);
            _receiveContentHub = new BytesHub(_bytesPool);

            _receiveSemaphoreSlim = new SemaphoreSlim(0, 1);
            _sendSemaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public ICap Cap => _cap;
        public bool IsConnected => (_cap != null && _cap.IsConnected);
        public long ReceivedByteCount => Interlocked.Read(ref _receivedByteCount);
        public long SentByteCount => Interlocked.Read(ref _sentByteCount);

        public long TotalBytesSent { get; }
        public long TotalBytesReceived { get; }

        public void RunJobs()
        {
            if (_bandwidthController == null)
            {
                this.Send(_maxSendByteCount);
                this.Receive(_maxReceiveByteCount);
            }
            else
            {
                lock (_bandwidthController.SendBytesLimiter.LockObject)
                {
                    var availableSize = _bandwidthController.SendBytesLimiter.ComputeFreeBytes();
                    var sentSize = this.Send(availableSize);
                    _bandwidthController.SendBytesLimiter.AddConsumedBytes(sentSize);
                }

                lock (_bandwidthController.ReceiveBytesLimiter.LockObject)
                {
                    var availableSize = _bandwidthController.ReceiveBytesLimiter.ComputeFreeBytes();
                    var receivedSize = this.Receive(availableSize);
                    _bandwidthController.ReceiveBytesLimiter.AddConsumedBytes(receivedSize);
                }
            }
        }

        private int Send(int limit)
        {
            this.ThrowIfDisposingRequested();
            if (!this.IsConnected) throw new ConnectionException("Not connected");

            lock (_sendLockObject)
            {
                int total = 0;
                int loopCount = 0;

                while (total < limit)
                {
                    if (_sendHeaderBufferPosition == -1 && !_sendContentHubWriterIsCompleted) break;
                    if (++loopCount > 5) break;

                    if (_sendHeaderBufferPosition < 4)
                    {
                        if (!_cap.CanSend()) break;

                        int sendLength = _cap.Send(_sendHeaderBuffer.AsSpan().Slice(_sendHeaderBufferPosition));
                        if (sendLength <= 0) break;

                        _sendHeaderBufferPosition += sendLength;
                    }
                    else if (_sendContentHub.Reader.RemainBytes > 0)
                    {
                        var sequence = _sendContentHub.Reader.GetSequence();
                        var position = sequence.Start;

                        while (total < limit && sequence.Length > 0 && sequence.TryGet(ref position, out var memory, false))
                        {
                            if (!_cap.CanSend())                                break;

                            int readLength = Math.Min(limit - total, memory.Length);
                            int sendLength = _cap.Send(memory.Span.Slice(0, readLength));
                            if (sendLength <= 0) break;

                            position = sequence.GetPosition(sendLength, position);

                            total += sendLength;
                            Interlocked.Add(ref _sentByteCount, sendLength);

                            _sendContentHub.Reader.Advance(sendLength);
                        }

                        if (_sendContentHub.Reader.RemainBytes == 0)
                        {
                            _sendHeaderBufferPosition = -1;
                            _sendContentHub.Reset();
                            _sendContentHubWriterIsCompleted = false;

                            _sendSemaphoreSlim.Release();

                            break;
                        }
                    }
                }

                return total;
            }
        }

        private int Receive(int limit)
        {
            this.ThrowIfDisposingRequested();

            if (!this.IsConnected) throw new ConnectionException("Not connected");

            lock (_receiveLockObject)
            {
                int total = 0;
                int loopCount = 0;

                while (total < limit)
                {
                    if (_receiveContentHubWriterIsCompleted) break;
                    if (++loopCount > 5) break;

                    if (_receiveContentRemain == -1)
                    {
                        for (; ; )
                        {
                            if (!_cap.CanReceive()) break;

                            int receiveLength = _cap.Receive(_receiveHeaderBuffer.AsSpan().Slice(_receiveHeaderBufferPosition));
                            if (receiveLength <= 0) break;

                            _receiveHeaderBufferPosition += receiveLength;

                            if (_receiveHeaderBufferPosition == 4)
                            {
                                var contentLength = BinaryPrimitives.ReadInt32BigEndian(_receiveHeaderBuffer);
                                if (contentLength > _maxReceiveByteCount) throw new ConnectionException("This message is too long");

                                _receiveContentRemain = contentLength;

                                break;
                            }
                        }
                    }

                    while (_receiveContentRemain > 0)
                    {
                        if (!_cap.CanReceive()) break;

                        int receiveLength = _cap.Receive(_receiveContentHub.Writer.GetSpan(_receiveContentRemain).Slice(0, _receiveContentRemain));
                        if (receiveLength <= 0) break;

                        _receiveContentHub.Writer.Advance(receiveLength);
                        _receiveContentRemain -= receiveLength;
                    }

                    if (_receiveContentRemain == 0)
                    {
                        _receiveHeaderBufferPosition = 0;
                        _receiveContentRemain = -1;
                        _receiveContentHubWriterIsCompleted = true;

                        _receiveSemaphoreSlim.Release();

                        break;
                    }
                }

                return total;
            }
        }

        private void InternalSend(Action<IBufferWriter<byte>> action)
        {
            action.Invoke(_sendContentHub.Writer);
            _sendContentHubWriterIsCompleted = true;

            BinaryPrimitives.WriteInt32BigEndian(_sendHeaderBuffer, (int)_sendContentHub.Writer.WrittenBytes);
            _sendHeaderBufferPosition = 0;
        }

        public bool TrySend(Action<IBufferWriter<byte>> action)
        {
            if (!_sendSemaphoreSlim.Wait(0)) return false;

            this.InternalSend(action);
            return true;
        }

        public async ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
        {
            await _sendSemaphoreSlim.WaitAsync(cancellationToken);
            this.InternalSend(action);
        }

        private void InternalReceive(Action<ReadOnlySequence<byte>> action)
        {
            var sequence = _receiveContentHub.Reader.GetSequence();
            action.Invoke(sequence);

            _receiveContentHub.Reset();
            _receiveContentHubWriterIsCompleted = false;
        }

        public bool TryReceive(Action<ReadOnlySequence<byte>> action)
        {
            if (!_receiveSemaphoreSlim.Wait(0)) return false;

            this.InternalReceive(action);
            return true;
        }

        public async ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default)
        {
            await _receiveSemaphoreSlim.WaitAsync(cancellationToken);
            this.InternalReceive(action);
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _cap?.Dispose();
                _sendContentHub?.Dispose();
                _receiveContentHub?.Dispose();
                _sendSemaphoreSlim?.Dispose();
                _receiveSemaphoreSlim?.Dispose();
            }
        }
    }
}
