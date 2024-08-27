using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public interface IOmniRemotingErrorMessageFactory<TErrorMessage>
    where TErrorMessage : RocketMessage<TErrorMessage>
{
    TErrorMessage Create(Exception e);
}
