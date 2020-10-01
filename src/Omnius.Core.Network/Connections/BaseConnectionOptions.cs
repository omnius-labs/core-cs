namespace Omnius.Core.Network.Connections
{
    public record BaseConnectionOptions
    {
        public int MaxReceiveByteCount = 1024 * 64;
        public IBytesPool? BytesPool;
    }
}
