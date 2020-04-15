namespace Omnius.Core.Network.Connections
{
    public sealed class BaseConnectionOptions
    {
        public int MaxSendByteCount { get; set; }
        public int MaxReceiveByteCount { get; set; }
        public IBytesPool? BytesPool { get; set; }
    }
}
