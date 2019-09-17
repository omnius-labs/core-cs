using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Network.Connections;
using Omnix.Serialization.OmniPack;

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
            where TMessage : IOmniPackMessage<TMessage>
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)PacketType.Message;
                bufferWriter.Advance(1);
                var writer = new OmniPackWriter(bufferWriter, _bufferPool);
                IOmniPackMessage<TMessage>.Formatter.Serialize(ref writer, message, 0);
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
            where TMessage : IOmniPackMessage<TMessage>
        {
            OmniRpcStreamReceiveResult<TMessage> receiveResult = default;

            await _connection.DequeueAsync((sequence) =>
            {
                Span<byte> type = stackalloc byte[1];
                sequence.CopyTo(type);

                switch ((PacketType)type[0])
                {
                    case PacketType.Message:
                        var reader = new OmniPackReader(sequence, _bufferPool);
                        var message = IOmniPackMessage<TMessage>.Formatter.Deserialize(ref reader, 0);
                        receiveResult = new OmniRpcStreamReceiveResult<TMessage>(message, null, false, false);
                        break;
                    case PacketType.ErrorMessage:
                        var errorMessage = OmniRpcErrorMessage.Import(sequence, _bufferPool);
                        receiveResult = new OmniRpcStreamReceiveResult<TMessage>(default, errorMessage, false, false);
                        break;
                    case PacketType.Canceled:
                        receiveResult = new OmniRpcStreamReceiveResult<TMessage>(default, null, true, false);
                        break;
                    case PacketType.Completed:
                        receiveResult = new OmniRpcStreamReceiveResult<TMessage>(default, null, false, true);
                        break;
                }
            }, token);

            return receiveResult;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
        }
    }
}
