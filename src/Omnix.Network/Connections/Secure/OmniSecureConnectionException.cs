using System;

namespace Omnix.Network.Connections.Secure
{
    public class OmniSecureConnectionException : Exception
    {
        public OmniSecureConnectionException() : base() { }
        public OmniSecureConnectionException(string message) : base(message) { }
        public OmniSecureConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
