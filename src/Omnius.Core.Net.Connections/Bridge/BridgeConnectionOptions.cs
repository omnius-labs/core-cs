using Omnius.Core;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Bridge
{
    public record BridgeConnectionOptions
    {
        public int MaxReceiveByteCount { get; init; }

        public IBandwidthLimiter? SenderBandwidthLimiter { get; init; }

        public IBandwidthLimiter? ReceiverBandwidthLimiter { get; init; }

        public IBatchActionDispatcher? BatchActionDispatcher { get; init; }

        public IBytesPool? BytesPool { get; init; }
    }
}
