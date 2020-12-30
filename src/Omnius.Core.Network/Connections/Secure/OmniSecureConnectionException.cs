using System;

namespace Omnius.Core.Network.Connections.Secure
{
    public sealed class OmniSecureConnectionException : Exception
    {
        public OmniSecureConnectionException()
            : base()
        {
        }

        public OmniSecureConnectionException(string message)
            : base(message)
        {
        }

        public OmniSecureConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
