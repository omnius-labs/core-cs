namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal partial class ConnectionMultiplexer
{
    internal enum ErrorCode : byte
    {
        None = 0,
        PacketReceiveTimeout = 1,
        PacketTypeInvalid = 2,
        StreamIdInvalid = 3,
        StreamRequestQueueOverflow = 4,
        StreamRequestAcceptedQueueOverflow = 5,
        StreamDataSizeTooLarge = 6,
        StreamDataQueueOverflow = 7,
        StreamDataAcceptedQueueOverflow = 8,
    }
}
