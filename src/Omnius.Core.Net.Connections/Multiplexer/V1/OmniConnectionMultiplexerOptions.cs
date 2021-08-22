using System;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer.V1
{
    public record OmniConnectionMultiplexerOptions
    {
        public OmniConnectionMultiplexerOptions(OmniConnectionMultiplexerType type, TimeSpan packetReceiveTimeout, uint maxStreamRequestQueueSize, uint maxStreamDataSize, uint maxStreamDataQueueSize)
        {
            this.Type = type;
            this.PacketReceiveTimeout = packetReceiveTimeout;
            this.MaxStreamRequestQueueSize = maxStreamRequestQueueSize;
            this.MaxStreamDataSize = maxStreamDataSize;
            this.MaxStreamDataQueueSize = maxStreamDataQueueSize;
        }

        public OmniConnectionMultiplexerType Type { get; }

        public TimeSpan PacketReceiveTimeout { get; }

        public uint MaxStreamRequestQueueSize { get; }

        public uint MaxStreamDataSize { get; }

        public uint MaxStreamDataQueueSize { get; }
    }
}
