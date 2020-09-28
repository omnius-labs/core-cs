namespace Omnius.Core.Network.Connections
{
    public record BaseConnectionDispatcherOptions
    {
        public int MaxSendBytesPerSeconds = 1024 * 64;
        public int MaxReceiveBytesPerSeconds = 1024 * 64;
    }
}
