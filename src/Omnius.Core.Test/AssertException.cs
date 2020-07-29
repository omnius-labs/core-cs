using System;

namespace Omnius.Core.Test
{
    public class AssertException : Exception
    {
        public AssertException(string message) : base(message) { }
        public AssertException(string message, Exception innerException) : base(message, innerException) { }
    }
}
