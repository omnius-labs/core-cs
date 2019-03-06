using System.Collections.Generic;
using Omnix.Base;

namespace Omnix.Network.Connection.Secure
{
    public sealed class OmniSecureConnectionOptions
    {
        public OmniSecureConnectionType Type { get; set; }
        public string[] Passwords { get; set; }
        public BufferPool BufferPool { get; set; }
    }
}
