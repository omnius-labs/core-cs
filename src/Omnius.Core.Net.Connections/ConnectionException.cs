using System;

namespace Omnius.Core.Net.Connections
{
    public class ConnectionException : Exception
    {
        public ConnectionException()
            : base()
        {
        }

        public ConnectionException(string message)
        : base(message)
        {
        }

        public ConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
