namespace Omnius.Core.Network.Connections
{
    public sealed class BaseConnectionDispatcherOptions
    {
        public int MaxSendBytesPerSeconds { get; set; } = 1024 * 64;
        public int MaxReceiveBytesPerSeconds { get; set; } = 1024 * 64;
    }
}
