using System.Collections.Generic;
using Omnix.Base;

namespace Omnix.Network.Connections.Secure
{
    public sealed class OmniSecureConnectionOptions
    {
        public OmniSecureConnectionType Type { get; set; }
        public IReadOnlyList<string>? Passwords { get; set; }
        public IBufferPool<byte>? BufferPool { get; set; }
    }
}
