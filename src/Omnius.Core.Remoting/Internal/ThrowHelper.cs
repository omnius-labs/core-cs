using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace Omnius.Core.Remoting.Internal
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
        public static void ThrowRocketPackRpcApplicationException(RocketPackRpcErrorMessage errorMessage) => throw CreateRocketPackRpcApplicationException(errorMessage);

        public static Exception CreateRocketPackRpcApplicationException(RocketPackRpcErrorMessage errorMessage)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Type: {errorMessage.Type}");
            sb.AppendLine($"Message: {errorMessage.Message}");
            sb.AppendLine($"StackTrace: {errorMessage.StackTrace}");
            return new RocketPackRpcApplicationException(sb.ToString());
        }
    }
}
