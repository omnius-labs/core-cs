using System;

namespace Omnius.Core.Net.Connections.Multiplexer.V1
{
    public record ConnectionMultiplexerOptions
    (
        OmniConnectionMultiplexerType Type,
        TimeSpan PacketReceiveTimeout,
        uint MaxStreamRequestQueueSize,
        uint MaxStreamDataSize,
        uint MaxStreamDataQueueSize
    );
}
