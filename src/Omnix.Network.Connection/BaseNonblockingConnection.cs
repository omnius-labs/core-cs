using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Serialization;

namespace Omnix.Network.Connection
{
    public sealed class BaseNonblockingConnection : DisposableBase, INonblockingConnection
    {
        private Cap _cap;
        private readonly int _maxReceiveByteCount;
        private readonly BufferPool _bufferPool;

        private Hub _sendHeaderHub;
        private Hub _sendContentHub;

        private Hub _receiveHeaderHub;
        private int _receiveContentRemain = -1;
        private Hub _receiveContentHub;

        private SemaphoreSlim _sendSemaphoreSlim;
        private SemaphoreSlim _receiveSemaphoreSlim;

        private long _receivedByteCount;
        private long _sentByteCount;

        private readonly object _sendLockObject = new object();
        private readonly object _receiveLockObject = new object();

        private volatile bool _disposed;

        public BaseNonblockingConnection(Cap cap, int maxReceiveByteCount, BufferPool bufferPool)
        {
            if (cap == null) throw new ArgumentNullException(nameof(cap));
            if (cap.IsBlocking == true) throw new ArgumentException($"{nameof(cap)} is not nonblocking", nameof(cap));
            if (bufferPool == null) throw new ArgumentNullException(nameof(bufferPool));

            _cap = cap;
            _maxReceiveByteCount = maxReceiveByteCount;
            _bufferPool = bufferPool;

            _sendHeaderHub = new Hub();
            _sendContentHub = new Hub();

            _receiveHeaderHub = new Hub();
            _receiveContentHub = new Hub();

            _receiveSemaphoreSlim = new SemaphoreSlim(0, 1);
            _sendSemaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public Cap Cap => _cap;

        public bool IsConnected => (_cap != null && _cap.IsConnected);

        public long ReceivedByteCount => Interlocked.Read(ref _receivedByteCount);

        public long SentByteCount => Interlocked.Read(ref _sentByteCount);

        private int InternalSend(Hub.HubReader reader, int limit)
        {
            int total = 0;

            var sequence = reader.GetSequence();
            var position = sequence.Start;

            while (total < limit && sequence.TryGet(ref position, out var memory, false))
            {
                if (memory.Length == 0)
                {
                    reader.Complete();
                    break;
                }

                int readLength = Math.Min(limit - total, memory.Length);

                if (!_cap.CanSend()) break;
                int sendLength = _cap.Send(memory.Span.Slice(0, readLength));
                if (sendLength == 0) break;

                position = sequence.GetPosition(sendLength, position);

                total += sendLength;
                Interlocked.Add(ref _sentByteCount, sendLength);

                reader.Advance(sendLength);
            }

            return total;
        }

        public int Send(int limit)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
            if (!this.IsConnected) throw new ConnectionException("Not connected");

            lock (_sendLockObject)
            {
                int total = 0;
                int loopCount = 0;

                while (total < limit)
                {
                    if (!_sendHeaderHub.Writer.IsCompleted && !_sendContentHub.Writer.IsCompleted) break;
                    if (++loopCount > 5) break;

                    if (!_sendHeaderHub.Reader.IsCompleted)
                    {
                        total += this.InternalSend(_sendHeaderHub.Reader, limit - total);
                    }
                    else if (_sendContentHub.Writer.IsCompleted)
                    {
                        total += this.InternalSend(_sendContentHub.Reader, limit - total);

                        if (_sendContentHub.Reader.IsCompleted)
                        {
                            _sendHeaderHub.Reset();
                            _sendContentHub.Reset();

                            _sendSemaphoreSlim.Release();

                            break;
                        }
                    }
                }

                return total;
            }
        }

        public int Receive(int limit)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
            if (!this.IsConnected) throw new ConnectionException("Not connected");

            lock (_receiveLockObject)
            {
                int total = 0;
                int loopCount = 0;

                while (total < limit)
                {
                    if (_receiveContentHub.Writer.IsCompleted) break;
                    if (++loopCount > 5) break;

                    if (_receiveContentRemain == -1)
                    {
                        Span<byte> buffer = stackalloc byte[1];

                        for (; ; )
                        {
                            if (_receiveHeaderHub.Writer.BytesWritten > 10) throw new ConnectionException("too large varint.");

                            if (!_cap.CanReceive()) break;
                            int receiveLength = _cap.Receive(buffer);
                            if (receiveLength == 0) break;

                            _receiveHeaderHub.Writer.Write(buffer);

                            if (Varint.IsEnd(buffer[0]))
                            {
                                _receiveHeaderHub.Writer.Complete();
                                var sequence = _receiveHeaderHub.Reader.GetSequence();

                                Varint.TryGetUInt64(sequence, out var contentLength, out var _);
                                _receiveContentRemain = (int)contentLength;

                                _receiveHeaderHub.Reader.Complete();
                                _receiveHeaderHub.Reset();

                                break;
                            }
                        }
                    }

                    while (_receiveContentRemain > 0)
                    {
                        if (!_cap.CanReceive()) break;
                        int receiveLength = _cap.Receive(_receiveContentHub.Writer.GetSpan(_receiveContentRemain).Slice(0, _receiveContentRemain));
                        if (receiveLength == 0) break;

                        _receiveContentHub.Writer.Advance(receiveLength);
                        _receiveContentRemain -= receiveLength;
                    }

                    if (_receiveContentRemain == 0)
                    {
                        _receiveContentHub.Writer.Complete();
                        _receiveContentRemain = -1;

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

            Varint.SetUInt64((ulong)_sendContentHub.Writer.BytesWritten , _sendHeaderHub.Writer);
            _sendHeaderHub.Writer.Complete();
        }

        public async ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            await _sendSemaphoreSlim.WaitAsync(token);
            this.InternalEnqueue(action);
        }

        public bool TryEnqueue(Action<IBufferWriter<byte>> action)
        {
            if (!_sendSemaphoreSlim.Wait(0)) return false;

            this.InternalEnqueue(action);

            return true;
        }

        public void InternalDequeue(Action<ReadOnlySequence<byte>> action)
        {
            var sequence = _receiveContentHub.Reader.GetSequence();
            action.Invoke(sequence);

            _receiveContentHub.Reader.Complete();
            _receiveContentHub.Reset();
        }

        public async ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            await _receiveSemaphoreSlim.WaitAsync(token);
            this.InternalDequeue(action);
        }

        public bool TryDequeue(Action<ReadOnlySequence<byte>> action)
        {
            if (!_receiveSemaphoreSlim.Wait(0)) return false;

            this.InternalDequeue(action);

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                var cap = _cap;
                var sendHeaderHub = _sendHeaderHub;
                var sendContentHub = _sendContentHub;
                var receiveHeaderHub = _receiveContentHub;
                var receiveContentHub = _receiveContentHub;
                var sendSemaphoreSlim = _sendSemaphoreSlim;
                var receiveSemaphoreSlim = _receiveSemaphoreSlim;

                if (cap != null)
                {
                    cap.Dispose();
                    _cap = null;
                }

                if (sendHeaderHub != null)
                {
                    sendHeaderHub.Reset();
                    _sendHeaderHub = null;
                }

                if (sendContentHub != null)
                {
                    sendContentHub.Reset();
                    _sendContentHub = null;
                }

                if (receiveHeaderHub != null)
                {
                    receiveHeaderHub.Reset();
                    _receiveHeaderHub = null;
                }

                if (sendSemaphoreSlim != null)
                {
                    sendSemaphoreSlim.Dispose();
                    _sendSemaphoreSlim = null;
                }

                if (receiveSemaphoreSlim != null)
                {
                    receiveSemaphoreSlim.Dispose();
                    _receiveSemaphoreSlim = null;
                }
            }
        }
    }
}
