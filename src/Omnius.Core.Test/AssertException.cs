using System;

namespace Omnius.Core.Test
{
    public class AssertException : Exception
    {
        public AssertException(string message) : this(message) { }
        public AssertException(string message, Exception innerException) : this(message, innerException) { }
    }
}
