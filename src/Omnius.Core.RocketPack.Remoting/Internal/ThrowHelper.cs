using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace Omnius.Core.RocketPack.Remoting.Internal
{
    internal sealed partial class ThrowHelper
    {
        [DoesNotReturn]
        public static void ThrowRocketPackRpcProtocolException_UnexpectedProtocol() => throw CreateRocketPackRpcProtocolException_UnexpectedProtocol();

        public static Exception CreateRocketPackRpcProtocolException_UnexpectedProtocol()
        {
            return new RocketPackRpcProtocolException("Unexpected protocol");
        }

        [DoesNotReturn]
        public static void ThrowRocketPackRpcApplicationException<T>(T errorMessage) => throw CreateRocketPackRpcApplicationException(errorMessage);

        public static Exception CreateRocketPackRpcApplicationException<T>(T errorMessage)
        {
            return new RocketPackRpcApplicationException<T>(errorMessage);
        }
    }
}
