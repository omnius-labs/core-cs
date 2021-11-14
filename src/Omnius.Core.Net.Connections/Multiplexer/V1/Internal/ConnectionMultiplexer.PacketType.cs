namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal partial class ConnectionMultiplexer
{
    internal enum PacketType : byte
    {
        KeepAlive = 0x00,

        StreamRequest = 0x01,

        StreamRequestAccepted = 0x02,

        StreamData = 0x03,

        StreamDataAccepted = 0x04,

        StreamFinish = 0xFD,

        SessionError = 0xFE,

        SessionFinish = 0xFF,
    }
}
