using System.Collections.Generic;
using Omnius.Core.Cryptography;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Secure.V1
{
    public record OmniSecureConnectionOptions
    {
        public int MaxReceiveByteCount { get; init; }

        public OmniSecureConnectionType Type { get; init; }

        public OmniDigitalSignature? DigitalSignature { get; init; }

        public IBatchActionDispatcher? BatchActionDispatcher { get; init; }

        public IBytesPool? BytesPool { get; init; }
    }
}
