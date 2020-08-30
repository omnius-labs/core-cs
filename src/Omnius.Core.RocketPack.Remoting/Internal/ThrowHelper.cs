using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack;

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

    [Serializable]
    public class RocketPackRpcException : Exception
    {
        public RocketPackRpcException() { }
        public RocketPackRpcException(string message) : base(message) { }
        public RocketPackRpcException(string message, System.Exception inner) : base(message, inner) { }
        protected RocketPackRpcException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class RocketPackRpcProtocolException : RocketPackRpcException
    {
        public RocketPackRpcProtocolException() { }
        public RocketPackRpcProtocolException(string message) : base(message) { }
        public RocketPackRpcProtocolException(string message, System.Exception inner) : base(message, inner) { }
        protected RocketPackRpcProtocolException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class RocketPackRpcApplicationException : RocketPackRpcException
    {
        public RocketPackRpcApplicationException() { }
        public RocketPackRpcApplicationException(string message) : base(message) { }
        public RocketPackRpcApplicationException(string message, System.Exception inner) : base(message, inner) { }
        protected RocketPackRpcApplicationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
