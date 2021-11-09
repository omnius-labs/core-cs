namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal partial class ConnectionMultiplexer
{
    internal enum ErrorCode : byte
    {
        Normal = 0,

        PacketReceiveTimeout = 1,

        InvalidPacketType = 2,

        InvalidStreamId = 3,

        StreamRequestQueueOverflow = 4,

        StreamRequestAcceptedQueueOverflow = 5,

        StreamDataSizeTooLarge = 6,

        StreamDataQueueOverflow = 7,

        StreamDataAcceptedQueueOverflow = 8,
    }
}