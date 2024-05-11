using System.Runtime.Serialization;

namespace Core.RocketPack.Remoting;

[Serializable]
public class RocketPackRpcException : Exception
{
    public RocketPackRpcException() { }
    public RocketPackRpcException(string? message) : base(message) { }
    public RocketPackRpcException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class RocketRemotingProtocolException : RocketPackRpcException
{
    public RocketRemotingProtocolException() { }
    public RocketRemotingProtocolException(string? message) : base(message) { }
    public RocketRemotingProtocolException(string message, Exception inner) : base(message, inner) { }
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
