using System.Runtime.Serialization;

namespace Omnius.Core.RocketPack.Remoting;

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
public class RocketRemotingProtocolException : RocketPackRpcException
{
    public RocketRemotingProtocolException()
    {
    }

    public RocketRemotingProtocolException(string message)
        : base(message)
    {
    }

    public RocketRemotingProtocolException(string message, System.Exception inner)
        : base(message, inner)
    {
    }

    protected RocketRemotingProtocolException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

[Serializable]
public class RocketRemotingApplicationException<T> : RocketPackRpcException
{
    public RocketRemotingApplicationException(T errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public T ErrorMessage { get; }
}
