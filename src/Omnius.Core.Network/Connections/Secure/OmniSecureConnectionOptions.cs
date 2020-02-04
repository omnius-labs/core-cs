using System.Collections.Generic;
using Omnius.Core;

namespace Omnius.Core.Network.Connections.Secure
{
    public sealed class OmniSecureConnectionOptions
    {
        public OmniSecureConnectionType Type { get; set; }
        public IReadOnlyList<string>? Passwords { get; set; }
        public IBytesPool? BufferPool { get; set; }
    }
}
