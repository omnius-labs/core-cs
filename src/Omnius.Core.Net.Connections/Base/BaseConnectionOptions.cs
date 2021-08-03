using Omnius.Core;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections
{
    public record BaseConnectionOptions(
        int MaxReceiveByteCount,
        IBandwidthLimiter? SenderBandwidthLimiter,
        IBandwidthLimiter? ReceiverBandwidthLimiter,
        IBatchActionDispatcher BatchActionDispatcher,
        IBytesPool BytesPool
    );
}
