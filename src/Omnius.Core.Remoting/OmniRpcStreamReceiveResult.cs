using System.Diagnostics.CodeAnalysis;
using Omnius.Core.Remoting.Internal;
using Omnius.Core.Serialization.RocketPack;

namespace Omnius.Core.Remoting
{
    public readonly struct OmniRpcStreamReceiveResult<TMessage>
        where TMessage : IRocketPackMessage<TMessage>
    {
        private readonly TMessage _message;
        private readonly OmniRpcErrorMessage? _errorMessage;
        private readonly OmniRpcStreamReceiveResultFlags _resultFlags;

        public OmniRpcStreamReceiveResult([AllowNull] TMessage message, OmniRpcErrorMessage? errorMessage, bool isCanceled, bool isCompleted)
        {
            _message = message;
            _errorMessage = errorMessage;

            _resultFlags = OmniRpcStreamReceiveResultFlags.None;

            if (!(errorMessage is null))
            {
                _resultFlags |= OmniRpcStreamReceiveResultFlags.Error;
            }

            if (isCanceled)
            {
                _resultFlags |= OmniRpcStreamReceiveResultFlags.Canceled;
            }

            if (isCompleted)
            {
                _resultFlags |= OmniRpcStreamReceiveResultFlags.Completed;
            }
        }

        [MaybeNull]
        public TMessage Message => _message;

        public OmniRpcErrorMessage? ErrorMessage => _errorMessage;

        public bool IsMessage => (_resultFlags & OmniRpcStreamReceiveResultFlags.None) != 0;

        public bool IsErrorMessage => (_resultFlags & OmniRpcStreamReceiveResultFlags.Error) != 0;

        public bool IsCanceled => (_resultFlags & OmniRpcStreamReceiveResultFlags.Canceled) != 0;

        public bool IsCompleted => (_resultFlags & OmniRpcStreamReceiveResultFlags.Completed) != 0;
    }
}
