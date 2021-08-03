using System;
using System.Runtime.Serialization;

namespace Omnius.Core.RocketPack.Remoting
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
    public class RocketPackRpcApplicationException<T> : RocketPackRpcException
    {
        public RocketPackRpcApplicationException(T errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        public T ErrorMessage { get; }
    }
}
