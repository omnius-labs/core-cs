namespace Omnius.Core.Network.Connections
{
    public class BaseConnectionDispatcherOptions
    {
        public int MaxSendBytesPerSeconds { get; init; } = 1024 * 64;

        public int MaxReceiveBytesPerSeconds { get; init; } = 1024 * 64;
    }
}
