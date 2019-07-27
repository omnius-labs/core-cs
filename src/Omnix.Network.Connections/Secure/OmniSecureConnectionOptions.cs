using Omnix.Base;

namespace Omnix.Network.Connections.Secure
{
    public sealed class OmniSecureConnectionOptions
    {
        public OmniSecureConnectionType Type { get; set; }
        public string[]? Passwords { get; set; }
        public BufferPool? BufferPool { get; set; }
    }
}
