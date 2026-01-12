namespace Omnius.Yamux;

public class YamuxException : Exception
{
    public YamuxException(string message) : base(message) { }
    public YamuxException(string message, Exception inner) : base(message, inner) { }
}

public sealed class YamuxConnectionClosedException : YamuxException
{
    public YamuxConnectionClosedException(string message) : base(message) { }
}

public sealed class YamuxProtocolException : YamuxException
{
    public YamuxProtocolException(string message) : base(message) { }
}
