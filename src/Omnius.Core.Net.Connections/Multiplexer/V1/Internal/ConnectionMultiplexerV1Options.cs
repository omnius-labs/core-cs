using System.Collections.Generic;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal record ConnectionMultiplexerV1Options
    {
        public OmniConnectionMultiplexerType Type { get; init; }

        public uint MaxMessageSize { get; init; }

        public uint MaxQueueSize { get; init; }

        public IBytesPool? BufferPool { get; init; }
    }
}
