using Omnix.Remoting.Internal;
using Omnix.Serialization.RocketPack;

namespace Omnix.Remoting
{
    public readonly struct OmniRpcStreamReceiveResult<TMessage>
        where TMessage : RocketPackMessageBase<TMessage>
    {
        private readonly TMessage _message;
        private readonly OmniRpcErrorMessage _errorMessage;
        private readonly OmniRpcStreamReceiveResultFlags _resultFlags;

        public OmniRpcStreamReceiveResult(TMessage message, OmniRpcErrorMessage errorMessage, bool isCanceled, bool isCompleted)
        {
            _message = message;
            _errorMessage = errorMessage;

            _resultFlags = OmniRpcStreamReceiveResultFlags.None;

            if (isCanceled)
            {
                _resultFlags |= OmniRpcStreamReceiveResultFlags.Canceled;
            }

            if (isCompleted)
            {
                _resultFlags |= OmniRpcStreamReceiveResultFlags.Completed;
            }
        }

        public bool IsMessage => (_resultFlags & OmniRpcStreamReceiveResultFlags.None) != 0;

        public bool IsErrorMessage => (_resultFlags & OmniRpcStreamReceiveResultFlags.Error) != 0;

        public bool IsCanceled => (_resultFlags & OmniRpcStreamReceiveResultFlags.Canceled) != 0;

        public bool IsCompleted => (_resultFlags & OmniRpcStreamReceiveResultFlags.Completed) != 0;
    }
}
