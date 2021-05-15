using System;

namespace Omnius.Core.Net.Caps
{
    public class SocketCapException : CapException
    {
        public SocketCapException()
            : base()
        {
        }

        public SocketCapException(string message)
            : base(message)
        {
        }

        public SocketCapException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
