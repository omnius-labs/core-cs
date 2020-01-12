using Omnius.Core;

namespace Omnius.Core.Network.Connections
{
    public sealed class OmniNonblockingConnectionOptions
    {
        public int MaxSendByteCount { get; set; }
        public int MaxReceiveByteCount { get; set; }
        public BandwidthController? BandwidthController { get; set; }
        public BufferPool<byte>? BufferPool { get; set; }
    }
}
