namespace Omnius.Core.Omnikit.Remoting;

public class OmniRemotingDefaultErrorMessageFactory : IOmniRemotingErrorMessageFactory<OmniRemotingDefaultErrorMessage>
{
    public static OmniRemotingDefaultErrorMessageFactory Default { get; } = new();

    public OmniRemotingDefaultErrorMessage Create(Exception e)
    {
        return new OmniRemotingDefaultErrorMessage
        {
            Type = e.GetType().FullName ?? string.Empty,
            Message = e.Message ?? string.Empty,
            StackTrace = e.StackTrace ?? string.Empty,
        };
    }
}
