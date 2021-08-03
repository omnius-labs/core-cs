namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal partial class ConnectionMultiplexer
    {
        internal enum ErrorCode : byte
        {
            Normal = 0,

            UnknownMessageType = 1,

            StreamDataSizeTooLarge = 1,

            StreamDataQueueOverflow = 2,

            MessageReceiveTimeout = 2,

            StreamRequestQueueOverflow = 3,

            StreamRequestAcceptedQueueOverflow = 4,
        }
    }
}
