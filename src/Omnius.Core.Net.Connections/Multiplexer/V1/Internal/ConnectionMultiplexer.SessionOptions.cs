using System;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal sealed partial class ConnectionMultiplexer
{
    private record SessionOptions(
        TimeSpan PacketReceiveTimeout,
        uint MaxStreamRequestQueueSize,
        uint MaxDataSize,
        uint MaxDataQueueSize);
}