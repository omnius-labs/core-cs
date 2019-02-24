using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Network.Connection;
using Omnix.Serialization.RocketPack;

namespace Omnix.Remoting
{
    public sealed partial class OmniRpcStream : DisposableBase
    {
        private readonly IConnection _connection;
        private readonly BufferPool _bufferPool;

        private volatile bool _disposed;

        internal OmniRpcStream(IConnection connection, BufferPool bufferPool)
        {
            _connection = connection;
            _bufferPool = bufferPool;
        }

        public async ValueTask SendMessageAsync<TMessage>(TMessage message, CancellationToken token = default)
            where TMessage : RocketPackMessageBase<TMessage>
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)OmniRpcStreamPacketType.Message;
                bufferWriter.Advance(1);
                RocketPackMessageBase<TMessage>.Formatter.Serialize(new RocketPackWriter(bufferWriter, _bufferPool), message, 0);
            }, token);
        }

        public async ValueTask SendErrorMessageAsync(OmniRpcErrorMessage errorMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)OmniRpcStreamPacketType.ErrorMessage;
                bufferWriter.Advance(1);
                errorMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendCanceledAsync(CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)OmniRpcStreamPacketType.Canceled;
                bufferWriter.Advance(1);
            }, token);
        }

        public async ValueTask SendCompletedAsync(CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)OmniRpcStreamPacketType.Completed;
                bufferWriter.Advance(1);
            }, token);
        }

        public async ValueTask<ReceiveResult<TMessage>> ReceiveAsync<TMessage>(CancellationToken token = default)
            where TMessage : RocketPackMessageBase<TMessage>
        {
            ReceiveResult<TMessage> receiveResult = default;

            await _connection.DequeueAsync((sequence) =>
            {
                Span<byte> type = stackalloc byte[1];
                sequence.CopyTo(type);

                switch ((OmniRpcStreamPacketType)type[0])
                {
                    case OmniRpcStreamPacketType.Message:
                        var message = RocketPackMessageBase<TMessage>.Formatter.Deserialize(new RocketPackReader(sequence, _bufferPool), 0);
                        receiveResult = new ReceiveResult<TMessage>(message, null, false, false);
                        break;
                    case OmniRpcStreamPacketType.ErrorMessage:
                        var errorMessage = OmniRpcErrorMessage.Import(sequence, _bufferPool);
                        receiveResult = new ReceiveResult<TMessage>(null, errorMessage, false, false);
                        break;
                    case OmniRpcStreamPacketType.Canceled:
                        receiveResult = new ReceiveResult<TMessage>(null, null, true, false);
                        break;
                    case OmniRpcStreamPacketType.Completed:
                        receiveResult = new ReceiveResult<TMessage>(null, null, false, true);
                        break;
                }
            }, token);

            return receiveResult;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                _connection?.Dispose();
            }
        }
    }
}
