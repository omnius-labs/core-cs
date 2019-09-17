using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Serialization.OmniPack;

namespace Omnix.Network.Connections.Multiplexer.V1.Internal
{
    internal sealed class MultiplexCommunicator
    {
        private readonly IConnection _connection;
        private readonly BufferPool _bufferPool;

        private enum PacketType : byte
        {
            Connect,
            Accept,
            UpdateWindowSize,
            Data,
            Close,
            Error,
        }

        public MultiplexCommunicator(IConnection connection, BufferPool bufferPool)
        {
            _connection = connection;
            _bufferPool = bufferPool;
        }

        public void DoEvents()
        {
            _connection.DoEvents();
        }

        public async ValueTask SendConnectMessageAsync(SessionConnectMessage connectMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                connectMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendAcceptMessageAsync(SessionAcceptMessage acceptMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                acceptMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendUpdateWindowSizeMessageAsync(SessionUpdateWindowSizeMessage updateWindowSizeMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                updateWindowSizeMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendDataMessageAsync(SessionDataMessage dataMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                dataMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendCloseMessageAsync(SessionCloseMessage closeMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                closeMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendErrorMessageAsync(SessionErrorMessage errorMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                errorMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask<MultiplexReceiveMessageResult?> ReceiveMessageAsync(CancellationToken token = default)
        {
            MultiplexReceiveMessageResult? result = default;

            await _connection.DequeueAsync((sequence) =>
            {
                var reader = new SequenceReader<byte>(sequence);

                if (!Varint.TryGetUInt64(ref reader, out var type))
                {
                    throw new FormatException();
                }

                sequence = sequence.Slice(reader.Position);

                switch ((PacketType)type)
                {
                    case PacketType.Connect:
                        result = new MultiplexReceiveMessageResult(connectMessage: SessionConnectMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.Accept:
                        result = new MultiplexReceiveMessageResult(acceptMessage: SessionAcceptMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.UpdateWindowSize:
                        result = new MultiplexReceiveMessageResult(updateWindowSizeMessage: SessionUpdateWindowSizeMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.Data:
                        result = new MultiplexReceiveMessageResult(dataMessage: SessionDataMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.Close:
                        result = new MultiplexReceiveMessageResult(closeMessage: SessionCloseMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.Error:
                        result = new MultiplexReceiveMessageResult(errorMessage: SessionErrorMessage.Import(sequence, _bufferPool));
                        break;
                }
            }, token);

            return result;
        }
    }
}
