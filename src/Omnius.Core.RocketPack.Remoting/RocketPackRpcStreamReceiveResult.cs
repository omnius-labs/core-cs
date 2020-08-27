using System;
using System.Diagnostics.CodeAnalysis;

namespace Omnius.Core.RocketPack.Remoting
{
    [Flags]
    internal enum RocketPackRpcStreamReceiveResultFlags : byte
    {
        None = 0,
        Message = 0x1,
        Cancel = 0x2,
        Completed = 0x4,
        Continue = 0x8,
        Error = 0x10,
    }

    public readonly struct RocketPackRpcStreamReceiveResult
    {
        private readonly RocketPackRpcErrorMessage? _errorMessage;
        private readonly RocketPackRpcStreamReceiveResultFlags _resultFlags;

        private RocketPackRpcStreamReceiveResult(RocketPackRpcErrorMessage? errorMessage, RocketPackRpcStreamReceiveResultFlags flags)
        {
            _errorMessage = errorMessage;
            _resultFlags = flags;
        }

        public static RocketPackRpcStreamReceiveResult CreateCancel()
        {
            return new RocketPackRpcStreamReceiveResult(null, RocketPackRpcStreamReceiveResultFlags.Cancel);
        }

        public static RocketPackRpcStreamReceiveResult CreateCompleted()
        {
            return new RocketPackRpcStreamReceiveResult(null, RocketPackRpcStreamReceiveResultFlags.Completed);
        }

        public static RocketPackRpcStreamReceiveResult CreateContinue()
        {
            return new RocketPackRpcStreamReceiveResult(null, RocketPackRpcStreamReceiveResultFlags.Continue);
        }

        public static RocketPackRpcStreamReceiveResult CreateError(RocketPackRpcErrorMessage? errorMessage)
        {
            return new RocketPackRpcStreamReceiveResult(errorMessage, RocketPackRpcStreamReceiveResultFlags.Error);
        }

        public RocketPackRpcErrorMessage? ErrorMessage => _errorMessage;

        public bool IsCancel => (_resultFlags & RocketPackRpcStreamReceiveResultFlags.Cancel) != 0;

        public bool IsCompleted => (_resultFlags & RocketPackRpcStreamReceiveResultFlags.Completed) != 0;

        public bool IsContinue => (_resultFlags & RocketPackRpcStreamReceiveResultFlags.Continue) != 0;

        public bool IsError => (_resultFlags & RocketPackRpcStreamReceiveResultFlags.Error) != 0;
    }

    public readonly struct RocketPackRpcStreamReceiveResult<TMessage>
        where TMessage : IRocketPackObject<TMessage>
    {
        [AllowNull]
        private readonly TMessage _message;
        private readonly RocketPackRpcErrorMessage? _errorMessage;
        private readonly RocketPackRpcStreamReceiveResultFlags _resultFlags;

        private RocketPackRpcStreamReceiveResult([AllowNull] TMessage message, RocketPackRpcErrorMessage? errorMessage, RocketPackRpcStreamReceiveResultFlags flags)
        {
            _message = message;
            _errorMessage = errorMessage;
            _resultFlags = flags;
        }

        public static RocketPackRpcStreamReceiveResult<TMessage> CreateCancel()
        {
            return new RocketPackRpcStreamReceiveResult<TMessage>(default, null, RocketPackRpcStreamReceiveResultFlags.Cancel);
        }

        public static RocketPackRpcStreamReceiveResult<TMessage> CreateCompleted([NotNull] TMessage message)
        {
            return new RocketPackRpcStreamReceiveResult<TMessage>(message, null, RocketPackRpcStreamReceiveResultFlags.Completed);
        }

        public static RocketPackRpcStreamReceiveResult<TMessage> CreateCompleted()
        {
            return new RocketPackRpcStreamReceiveResult<TMessage>(default, null, RocketPackRpcStreamReceiveResultFlags.Completed);
        }

        public static RocketPackRpcStreamReceiveResult<TMessage> CreateContinue([NotNull] TMessage message)
        {
            return new RocketPackRpcStreamReceiveResult<TMessage>(message, null, RocketPackRpcStreamReceiveResultFlags.Continue);
        }

        public static RocketPackRpcStreamReceiveResult<TMessage> CreateContinue()
        {
            return new RocketPackRpcStreamReceiveResult<TMessage>(default, null, RocketPackRpcStreamReceiveResultFlags.Continue);
        }

        public static RocketPackRpcStreamReceiveResult<TMessage> CreateError(RocketPackRpcErrorMessage? errorMessage)
        {
            return new RocketPackRpcStreamReceiveResult<TMessage>(default, errorMessage, RocketPackRpcStreamReceiveResultFlags.Error);
        }

        [MaybeNull]
        public TMessage Message => _message;

        public RocketPackRpcErrorMessage? ErrorMessage => _errorMessage;

        public bool IsCancel => (_resultFlags & RocketPackRpcStreamReceiveResultFlags.Cancel) != 0;

        public bool IsCompleted => (_resultFlags & RocketPackRpcStreamReceiveResultFlags.Completed) != 0;

        public bool IsContinue => (_resultFlags & RocketPackRpcStreamReceiveResultFlags.Continue) != 0;

        public bool IsError => (_resultFlags & RocketPackRpcStreamReceiveResultFlags.Error) != 0;
    }
}
