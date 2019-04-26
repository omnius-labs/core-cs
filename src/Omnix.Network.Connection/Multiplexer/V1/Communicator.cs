using System;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Network.Connection.Multiplexer.V1.Internal;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;

namespace Omnix.Network.Connection.Multiplexer.V1
{
    internal sealed class Communicator
    {
        private readonly IConnection _connection;
        private readonly BufferPool _bufferPool;

        public event Action<SessionConnectMessage> ReceiveSessionConnectMessageEvent;
        public event Action<SessionAcceptMessage> ReceiveSessionAcceptMessageEvent;
        public event Action<SessionUpdateWindowSizeMessage> ReceiveSessionUpdateWindowSizeMessageEvent;
        public event Action<SessionDataMessage> ReceiveSessionDataMessageEvent;
        public event Action<SessionCloseMessage> ReceiveSessionCloseMessageEvent;
        public event Action<SessionErrorMessage> ReceiveSessionErrorMessageEvent;

        private enum PacketType : byte
        {
            Connect,
            Accept,
            UpdateWindowSize,
            Data,
            Close,
            Error,
        }

        public Communicator(IConnection connection)
        {
            _connection = connection;
        }

        public void DoEvents()
        {
            _connection.DoEvents();

            _connection.TryDequeue((sequence) =>
            {
                if (!Varint.TryGetUInt64(sequence, out var type, out var comsumed))
                {
                    throw new FormatException();
                }

                sequence = sequence.Slice(comsumed);

                switch ((PacketType)type)
                {
                    case PacketType.Connect:
                        this.ReceiveSessionConnectMessageEvent.Invoke(SessionConnectMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.Accept:
                        this.ReceiveSessionAcceptMessageEvent.Invoke(SessionAcceptMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.UpdateWindowSize:
                        this.ReceiveSessionUpdateWindowSizeMessageEvent.Invoke(SessionUpdateWindowSizeMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.Data:
                        this.ReceiveSessionDataMessageEvent.Invoke(SessionDataMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.Close:
                        this.ReceiveSessionCloseMessageEvent.Invoke(SessionCloseMessage.Import(sequence, _bufferPool));
                        break;
                    case PacketType.Error:
                        this.ReceiveSessionErrorMessageEvent.Invoke(SessionErrorMessage.Import(sequence, _bufferPool));
                        break;
                }
            });
        }

        public async ValueTask SendSessionConnectMessageAsync(SessionConnectMessage sessionConnectMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                sessionConnectMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendSessionAcceptMessageAsync(SessionAcceptMessage sessionAcceptMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                sessionAcceptMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendSessionUpdateWindowSizeMessageAsync(SessionUpdateWindowSizeMessage sessionUpdateWindowSizeMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                sessionUpdateWindowSizeMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendSessionDataMessageAsync(SessionDataMessage sessionDataMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                sessionDataMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendSessionCloseMessageAsync(SessionCloseMessage sessionCloseMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                sessionCloseMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }

        public async ValueTask SendSessionErrorMessageAsync(SessionErrorMessage sessionErrorMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64((byte)PacketType.Connect, bufferWriter);
                sessionErrorMessage.Export(bufferWriter, _bufferPool);
            }, token);
        }
    }
}
