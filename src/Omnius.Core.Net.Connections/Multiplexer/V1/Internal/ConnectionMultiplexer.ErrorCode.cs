namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal partial class ConnectionMultiplexer
{
    internal enum ErrorCode : byte
    {
        Normal = 0,

        PacketReceiveTimeout = 1,

        PacketTypeInvalid = 2,

        StreamIdInvalid = 3,

        StreamIdNotFound = 4,

        StreamRequestQueueOverflow = 5,

        StreamRequestAcceptedQueueOverflow = 6,

        StreamDataSizeTooLarge = 7,

        StreamDataQueueOverflow = 8,

        StreamDataAcceptedQueueOverflow = 9,
    }
}
