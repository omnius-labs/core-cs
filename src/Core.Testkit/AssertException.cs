namespace Omnius.Core.Testkit;

public class AssertException : Exception
{
    public AssertException(string? message)
        : base(message)
    {
    }

    public AssertException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
