using System;
using System.Diagnostics.CodeAnalysis;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Remoting
{
    public sealed partial class RemotingFunction
    {
        [Flags]
        internal enum ReceiveResultFlags : byte
        {
            None = 0,
            Message = 0x1,
            Completed = 0x2,
            Continue = 0x4,
            Error = 0x8,
        }

        internal readonly struct ReceiveResult
        {
            private readonly RocketPackRpcErrorMessage? _errorMessage;
            private readonly ReceiveResultFlags _resultFlags;

            private ReceiveResult(RocketPackRpcErrorMessage? errorMessage, ReceiveResultFlags flags)
            {
                _errorMessage = errorMessage;
                _resultFlags = flags;
            }

            public static ReceiveResult CreateCompleted()
            {
                return new ReceiveResult(null, ReceiveResultFlags.Completed);
            }

            public static ReceiveResult CreateContinue()
            {
                return new ReceiveResult(null, ReceiveResultFlags.Continue);
            }

            public static ReceiveResult CreateError(RocketPackRpcErrorMessage? errorMessage)
            {
                return new ReceiveResult(errorMessage, ReceiveResultFlags.Error);
            }

            public RocketPackRpcErrorMessage? ErrorMessage => _errorMessage;

            public bool IsCompleted => (_resultFlags & ReceiveResultFlags.Completed) != 0;

            public bool IsContinue => (_resultFlags & ReceiveResultFlags.Continue) != 0;

            public bool IsError => (_resultFlags & ReceiveResultFlags.Error) != 0;
        }

        internal readonly struct ReceiveResult<TMessage>
            where TMessage : IRocketPackObject<TMessage>
        {
            [AllowNull]
            private readonly TMessage _message;
            private readonly RocketPackRpcErrorMessage? _errorMessage;
            private readonly ReceiveResultFlags _resultFlags;

            private ReceiveResult([AllowNull] TMessage message, RocketPackRpcErrorMessage? errorMessage, ReceiveResultFlags flags)
            {
                _message = message;
                _errorMessage = errorMessage;
                _resultFlags = flags;
            }

            public static ReceiveResult<TMessage> CreateCompleted([NotNull] TMessage message)
            {
                return new ReceiveResult<TMessage>(message, null, ReceiveResultFlags.Completed);
            }

            public static ReceiveResult<TMessage> CreateCompleted()
            {
                return new ReceiveResult<TMessage>(default, null, ReceiveResultFlags.Completed);
            }

            public static ReceiveResult<TMessage> CreateContinue([NotNull] TMessage message)
            {
                return new ReceiveResult<TMessage>(message, null, ReceiveResultFlags.Continue);
            }

            public static ReceiveResult<TMessage> CreateContinue()
            {
                return new ReceiveResult<TMessage>(default, null, ReceiveResultFlags.Continue);
            }

            public static ReceiveResult<TMessage> CreateError(RocketPackRpcErrorMessage? errorMessage)
            {
                return new ReceiveResult<TMessage>(default, errorMessage, ReceiveResultFlags.Error);
            }

            [MaybeNull]
            public TMessage Message => _message;

            public RocketPackRpcErrorMessage? ErrorMessage => _errorMessage;

            public bool IsCompleted => (_resultFlags & ReceiveResultFlags.Completed) != 0;

            public bool IsContinue => (_resultFlags & ReceiveResultFlags.Continue) != 0;

            public bool IsError => (_resultFlags & ReceiveResultFlags.Error) != 0;
        }
    }
}
