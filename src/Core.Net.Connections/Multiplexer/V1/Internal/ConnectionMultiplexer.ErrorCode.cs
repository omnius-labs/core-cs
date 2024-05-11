namespace Core.Net.Connections.Multiplexer.V1.Internal;

internal partial class ConnectionMultiplexer
{
    internal enum ErrorCode : byte
    {
        None = 0,
        PacketReceiveTimeout = 1,
        PacketTypeInvalid = 2,
        PacketFormatInvalid = 3,
        StreamIdInvalid = 4,
        StreamRequestQueueOverflow = 5,
        StreamRequestAcceptedQueueOverflow = 6,
        StreamDataSizeTooLarge = 7,
        StreamDataQueueOverflow = 8,
        StreamDataAcceptedQueueOverflow = 9,
    }
}
