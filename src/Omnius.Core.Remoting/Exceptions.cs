using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace Omnius.Core.Remoting
{
    [Serializable]
    public class RocketPackRpcException : Exception
    {
        public RocketPackRpcException()
        {
        }

        public RocketPackRpcException(string message)
            : base(message)
        {
        }

        public RocketPackRpcException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        protected RocketPackRpcException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class RocketPackRpcProtocolException : RocketPackRpcException
    {
        public RocketPackRpcProtocolException()
        {
        }

        public RocketPackRpcProtocolException(string message)
            : base(message)
        {
        }

        public RocketPackRpcProtocolException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        protected RocketPackRpcProtocolException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class RocketPackRpcApplicationException : RocketPackRpcException
    {
        public RocketPackRpcApplicationException()
        {
        }

        public RocketPackRpcApplicationException(string message)
            : base(message)
        {
        }

        public RocketPackRpcApplicationException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        protected RocketPackRpcApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
