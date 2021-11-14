namespace Omnius.Core.Net.Connections.Multiplexer;

public sealed class OmniConnectionMultiplexerException : Exception
{
    public OmniConnectionMultiplexerException()
        : base()
    {
    }

    public OmniConnectionMultiplexerException(string message)
        : base(message)
    {
    }

    public OmniConnectionMultiplexerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
