using System;
using System.Buffers;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal enum ConnectionMultiplexerMessageType : byte
    {
        Unknown = 0x00,

        ConnectionRequest = 0x01,

        ConnectionRequestAccepted = 0x02,

        Data = 0x03,

        DataAccepted = 0x04,

        Error = 0xFE,

        Finish = 0xFF,
    }

    internal readonly struct ConnectionMultiplexerMessage : IDisposable
    {
        private readonly ConnectionMultiplexerMessageType _type;
        private readonly IMemoryOwner<byte>? _payload;

        public ConnectionMultiplexerMessage(ConnectionMultiplexerMessageType type, IMemoryOwner<byte>? payload)
        {
            _type = type;
            _payload = payload;
        }

        public void Dispose()
        {
            _payload?.Dispose();
        }

        public ConnectionMultiplexerMessageType Type => _type;

        public ReadOnlyMemory<byte>? Payload => _payload?.Memory ?? default;
    }
}
