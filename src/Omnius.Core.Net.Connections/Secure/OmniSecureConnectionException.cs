using System;

namespace Omnius.Core.Net.Connections.Secure
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
