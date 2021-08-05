using Omnius.Core;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Bridge
{
    public record BridgeConnectionOptions(
        int MaxReceiveByteCount,
        IBandwidthLimiter? SenderBandwidthLimiter,
        IBandwidthLimiter? ReceiverBandwidthLimiter,
        IBatchActionDispatcher BatchActionDispatcher,
        IBytesPool BytesPool
    );
}
