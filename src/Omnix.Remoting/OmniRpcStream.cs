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

        internal OmniRpcStream(IConnection connection, BufferPool bufferPool)
        {
            _connection = connection;
            _bufferPool = bufferPool;
        }

        private enum PacketType : byte
        {
            Message = 0,
            ErrorMessage = 1,
            Canceled = 2,
            Completed = 3,
        }

        public async ValueTask SendMessageAsync<TMessage>(TMessage message, CancellationToken token = default)
            where TMessage : RocketPackMessageBase<TMessage>
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)PacketType.Message;
                bufferWriter.Advance(1);
                RocketPackMessageBase<TMessage>.Formatter.Serialize(new RocketPackWriter(bufferWriter, _bufferPool), message, 0);
            }, token);
        }

        public async ValueTask SendErrorMessageAsync(OmniRpcErrorMessage errorMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)PacketType.ErrorMessage;
                bufferWriter.Advance(1);
                errorMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendCanceledAsync(CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)PacketType.Canceled;
                bufferWriter.Advance(1);
            }, token);
        }

        public async ValueTask SendCompletedAsync(CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)PacketType.Completed;
                bufferWriter.Advance(1);
            }, token);
        }

        public async ValueTask<OmniRpcStreamReceiveResult<TMessage>> ReceiveAsync<TMessage>(CancellationToken token = default)
            where TMessage : RocketPackMessageBase<TMessage>
        {
            OmniRpcStreamReceiveResult<TMessage> receiveResult = default;

            await _connection.DequeueAsync((sequence) =>
            {
                Span<byte> type = stackalloc byte[1];
                sequence.CopyTo(type);

                switch ((PacketType)type[0])
                {
                    case PacketType.Message:
                        var message = RocketPackMessageBase<TMessage>.Formatter.Deserialize(new RocketPackReader(sequence, _bufferPool), 0);
                        receiveResult = new OmniRpcStreamReceiveResult<TMessage>(message, null, false, false);
                        break;
                    case PacketType.ErrorMessage:
                        var errorMessage = OmniRpcErrorMessage.Import(sequence, _bufferPool);
                        receiveResult = new OmniRpcStreamReceiveResult<TMessage>(null, errorMessage, false, false);
                        break;
                    case PacketType.Canceled:
                        receiveResult = new OmniRpcStreamReceiveResult<TMessage>(null, null, true, false);
                        break;
                    case PacketType.Completed:
                        receiveResult = new OmniRpcStreamReceiveResult<TMessage>(null, null, false, true);
                        break;
                }
            }, token);

            return receiveResult;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
        }
    }
}
