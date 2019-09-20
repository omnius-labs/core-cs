using System;

namespace Omnix.Network.Caps
{
    public class CapException : Exception
    {
        public CapException() : base() { }
        public CapException(string message) : base(message) { }
        public CapException(string message, Exception innerException) : base(message, innerException) { }
    }
}
