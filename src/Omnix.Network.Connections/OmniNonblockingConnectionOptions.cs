using Omnix.Base;

namespace Omnix.Network.Connections
{
    public sealed class OmniNonblockingConnectionOptions
    {
        public int MaxSendByteCount { get; set; }
        public int MaxReceiveByteCount { get; set; }
        public BandwidthController? BandwidthController { get; set; }
        public BufferPool? BufferPool { get; set; }
    }
}
