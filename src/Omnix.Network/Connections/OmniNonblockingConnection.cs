using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Network.Caps;

namespace Omnix.Network.Connections
{
    public sealed class OmniNonblockingConnection : DisposableBase, IConnection
    {
        private readonly ICap _cap;
        private readonly int _maxSendByteCount;
        private readonly int _maxReceiveByteCount;
        private readonly BandwidthController? _bandwidthController;
        private readonly IBufferPool<byte> _bufferPool;

        private readonly byte[] _sendHeaderBuffer = new byte[4];
        private int _sendHeaderBufferPosition = -1;
        private readonly Hub _sendContentHub;

        private readonly byte[] _receiveHeaderBuffer = new byte[4];
        private int _receiveHeaderBufferPosition = 0;
        private int _receiveContentRemain = -1;
        private readonly Hub _receiveContentHub;

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
            _bufferPool = option.BufferPool ?? BufferPool<byte>.Shared;

            _sendContentHub = new Hub();
            _receiveContentHub = new Hub();

            _receiveSemaphoreSlim = new SemaphoreSlim(0, 1);
            _sendSemaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public ICap Cap => _cap;
        public bool IsConnected => (_cap != null && _cap.IsConnected);
        public long ReceivedByteCount => Interlocked.Read(ref _receivedByteCount);
        public long SentByteCount => Interlocked.Read(ref _sentByteCount);

        public long TotalBytesSent { get; }
        public long TotalBytesReceived { get; }

        public void DoEvents()
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

            if (!this.IsConnected)
            {
                throw new ConnectionException("Not connected");
            }

            lock (_sendLockObject)
            {
                int total = 0;
                int loopCount = 0;

                while (total < limit)
                {
                    if (_sendHeaderBufferPosition == -1 && !_sendContentHub.Writer.IsCompleted)
                    {
                        break;
                    }

                    if (++loopCount > 5)
                    {
                        break;
                    }

                    if (_sendHeaderBufferPosition < 4)
                    {
                        if (!_cap.CanSend())
                        {
                            break;
                        }

                        int sendLength = _cap.Send(_sendHeaderBuffer.AsSpan().Slice(_sendHeaderBufferPosition));
                        if (sendLength <= 0)
                        {
                            break;
                        }

                        _sendHeaderBufferPosition += sendLength;
                    }
                    else if (!_sendContentHub.Reader.IsCompleted)
                    {
                        var sequence = _sendContentHub.Reader.GetSequence();
                        var position = sequence.Start;

                        while (total < limit && sequence.TryGet(ref position, out var memory, false))
                        {
                            if (memory.Length == 0)
                            {
                                _sendContentHub.Reader.Complete();
                                break;
                            }

                            int readLength = Math.Min(limit - total, memory.Length);

                            if (!_cap.CanSend())
                            {
                                break;
                            }

                            int sendLength = _cap.Send(memory.Span.Slice(0, readLength));
                            if (sendLength <= 0)
                            {
                                break;
                            }

                            position = sequence.GetPosition(sendLength, position);

                            total += sendLength;
                            Interlocked.Add(ref _sentByteCount, sendLength);

                            _sendContentHub.Reader.Advance(sendLength);
                        }

                        if (_sendContentHub.Reader.IsCompleted)
                        {
                            _sendHeaderBufferPosition = -1;
                            _sendContentHub.Reset();

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

            if (!this.IsConnected)
            {
                throw new ConnectionException("Not connected");
            }

            lock (_receiveLockObject)
            {
                int total = 0;
                int loopCount = 0;

                while (total < limit)
                {
                    if (_receiveContentHub.Writer.IsCompleted)
                    {
                        break;
                    }

                    if (++loopCount > 5)
                    {
                        break;
                    }

                    if (_receiveContentRemain == -1)
                    {
                        for (; ; )
                        {
                            if (!_cap.CanReceive())
                            {
                                break;
                            }

                            int receiveLength = _cap.Receive(_receiveHeaderBuffer.AsSpan().Slice(_receiveHeaderBufferPosition));
                            if (receiveLength <= 0)
                            {
                                break;
                            }

                            _receiveHeaderBufferPosition += receiveLength;

                            if (_receiveHeaderBufferPosition == 4)
                            {
                                var contentLength = BinaryPrimitives.ReadInt32BigEndian(_receiveHeaderBuffer);
                                if (contentLength > _maxReceiveByteCount)
                                {
                                    throw new ConnectionException("This message is too long");
                                }

                                _receiveContentRemain = contentLength;

                                break;
                            }
                        }
                    }

                    while (_receiveContentRemain > 0)
                    {
                        if (!_cap.CanReceive())
                        {
                            break;
                        }

                        int receiveLength = _cap.Receive(_receiveContentHub.Writer.GetSpan(_receiveContentRemain).Slice(0, _receiveContentRemain));
                        if (receiveLength <= 0)
                        {
                            break;
                        }

                        _receiveContentHub.Writer.Advance(receiveLength);
                        _receiveContentRemain -= receiveLength;
                    }

                    if (_receiveContentRemain == 0)
                    {
                        _receiveHeaderBufferPosition = 0;
                        _receiveContentRemain = -1;
                        _receiveContentHub.Writer.Complete();

                        _receiveSemaphoreSlim.Release();

                        break;
                    }
                }

                return total;
            }
        }

        private void InternalEnqueue(Action<IBufferWriter<byte>> action)
        {
            action.Invoke(_sendContentHub.Writer);
            _sendContentHub.Writer.Complete();

            BinaryPrimitives.WriteInt32BigEndian(_sendHeaderBuffer, (int)_sendContentHub.Writer.BytesWritten);
            _sendHeaderBufferPosition = 0;
        }


        public async ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            await _sendSemaphoreSlim.WaitAsync(token);
            this.InternalEnqueue(action);
        }

        public void InternalDequeue(Action<ReadOnlySequence<byte>> action)
        {
            var sequence = _receiveContentHub.Reader.GetSequence();
            action.Invoke(sequence);

            _receiveContentHub.Reader.Complete();
            _receiveContentHub.Reset();
        }

        public async ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            await _receiveSemaphoreSlim.WaitAsync(token);
            this.InternalDequeue(action);
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _cap?.Dispose();
                _sendContentHub?.Dispose();
                _sendSemaphoreSlim?.Dispose();
                _receiveSemaphoreSlim?.Dispose();
            }
        }
    }
}
