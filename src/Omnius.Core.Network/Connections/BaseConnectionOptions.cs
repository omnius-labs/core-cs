namespace Omnius.Core.Network.Connections
{
    public sealed class BaseConnectionOptions
    {
        public int MaxSendByteCount { get; set; } = 1024 * 64;
        public int MaxReceiveByteCount { get; set; } = 1024 * 64;
        public IBytesPool? BytesPool { get; set; }
    }
}
