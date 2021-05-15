using System.Collections.Generic;

namespace Omnius.Core.Net.Connections.Secure
{
    public sealed class OmniSecureConnectionOptions
    {
        public OmniSecureConnectionType Type { get; set; }

        public IReadOnlyList<string>? Passwords { get; set; }

        public IBytesPool? BufferPool { get; set; }
    }
}
