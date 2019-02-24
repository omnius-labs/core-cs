using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Network.Connection;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;

namespace Omnix.Remoting
{
    public partial class OmniRpcStream
    {
        [Flags]
        private enum ReceiveResultFlags : byte
        {
            None = 0x0,
            Canceled = 0x1,
            Completed = 0x2,
            Error = 0x4,
        }

        public readonly struct ReceiveResult<TMessage>
            where TMessage : RocketPackMessageBase<TMessage>
        {
            private readonly TMessage _message;
            private readonly OmniRpcErrorMessage _errorMessage;
            private readonly ReceiveResultFlags _resultFlags;

            public ReceiveResult(TMessage message, OmniRpcErrorMessage errorMessage, bool isCanceled, bool isCompleted)
            {
                _message = message;
                _errorMessage = errorMessage;

                _resultFlags = ReceiveResultFlags.None;

                if (isCanceled)
                {
                    _resultFlags |= ReceiveResultFlags.Canceled;
                }

                if (isCompleted)
                {
                    _resultFlags |= ReceiveResultFlags.Completed;
                }
            }

            public bool IsMessage => (_resultFlags & ReceiveResultFlags.None) != 0;

            public bool IsErrorMessage => (_resultFlags & ReceiveResultFlags.Error) != 0;

            public bool IsCanceled => (_resultFlags & ReceiveResultFlags.Canceled) != 0;

            public bool IsCompleted => (_resultFlags & ReceiveResultFlags.Completed) != 0;
        }
    }
}
