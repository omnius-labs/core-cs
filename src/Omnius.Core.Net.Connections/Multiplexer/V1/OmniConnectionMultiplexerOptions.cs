using System;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer.V1
{
    public record OmniConnectionMultiplexerOptions
    {
        public OmniConnectionMultiplexerType Type { get; init; }

        public TimeSpan PacketReceiveTimeout { get; init; }

        public uint MaxStreamRequestQueueSize { get; init; }

        public uint MaxStreamDataSize { get; init; }

        public uint MaxStreamDataQueueSize { get; init; }

        public IBatchActionDispatcher? BatchActionDispatcher { get; init; }

        public IBytesPool? BytesPool { get; init; }
    }
}
