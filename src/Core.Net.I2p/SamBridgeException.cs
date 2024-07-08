namespace Omnius.Core.Net.I2p;

public sealed class SamBridgeException : Exception
{
    public SamBridgeException() : base() { }
    public SamBridgeException(string? message) : base(message) { }
    public SamBridgeException(string? message, Exception? innerException) : base(message, innerException) { }
}
