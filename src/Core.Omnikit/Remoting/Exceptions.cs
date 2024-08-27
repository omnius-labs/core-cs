namespace Omnius.Core.Omnikit.Remoting;

public class OmniRemotingException : Exception
{
    public OmniRemotingException() { }
    public OmniRemotingException(string? message) : base(message) { }
    public OmniRemotingException(string message, Exception inner) : base(message, inner) { }
}

public class OmniRemotingProtocolException : OmniRemotingException
{
    public OmniRemotingProtocolException() { }
    public OmniRemotingProtocolException(string? message) : base(message) { }
    public OmniRemotingProtocolException(string message, Exception inner) : base(message, inner) { }
}

public class OmniRemotingApplicationException<T> : OmniRemotingException
{
    public OmniRemotingApplicationException(T errorMessage) { this.ErrorMessage = errorMessage; }
    public T ErrorMessage { get; }
}
