namespace Omnius.Core.Network.Connections
{
    public sealed class BaseConnectionDispatcherOptions
    {
        public int MaxSendBytesPerSeconds { get; set; }
        public int MaxReceiveBytesPerSeconds { get; set; }
    }
}
