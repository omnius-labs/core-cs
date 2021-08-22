using Omnius.Core;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Bridge
{
    public record BridgeConnectionOptions
    {
        public BridgeConnectionOptions(int maxReceiveByteCount)
        {
            this.MaxReceiveByteCount = maxReceiveByteCount;
        }

        public int MaxReceiveByteCount { get; }
    }
}
