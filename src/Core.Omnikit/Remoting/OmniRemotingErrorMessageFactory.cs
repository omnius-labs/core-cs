namespace Omnius.Core.Omnikit.Remoting;

public class OmniRemotingErrorMessageFactory : IOmniRemotingErrorMessageFactory<OmniRemotingErrorMessage>
{
    public static OmniRemotingErrorMessageFactory Default { get; } = new();

    public OmniRemotingErrorMessage Create(Exception e)
    {
        return new OmniRemotingErrorMessage
        {
            Type = e.GetType().FullName ?? string.Empty,
            Message = e.Message ?? string.Empty,
            StackTrace = e.StackTrace ?? string.Empty,
        };
    }
}
