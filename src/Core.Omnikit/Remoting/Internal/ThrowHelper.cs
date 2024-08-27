using System.Diagnostics.CodeAnalysis;

namespace Omnius.Core.Omnikit.Remoting.Internal;

internal sealed partial class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowRocketRemotingProtocolException_UnexpectedProtocol() => throw CreateRocketRemotingProtocolException_UnexpectedProtocol();

    public static Exception CreateRocketRemotingProtocolException_UnexpectedProtocol()
    {
        return new OmniRemotingProtocolException("Unexpected protocol");
    }

    [DoesNotReturn]
    public static void ThrowRocketRemotingApplicationException<T>(T errorMessage) => throw CreateRocketRemotingApplicationException(errorMessage);

    public static Exception CreateRocketRemotingApplicationException<T>(T errorMessage)
    {
        return new OmniRemotingApplicationException<T>(errorMessage);
    }
}
