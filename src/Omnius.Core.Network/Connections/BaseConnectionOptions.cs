namespace Omnius.Core.Network.Connections
{
    public record BaseConnectionOptions
    {
        public int MaxSendByteCount = 1024 * 64;
        public int MaxReceiveByteCount = 1024 * 64;
        public IBytesPool? BytesPool;
    }
}
