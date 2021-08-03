using System.Diagnostics.CodeAnalysis;
using Omnius.Core.RocketPack;

namespace Omnius.Core.RocketPack.Remoting
{
    public partial class RocketRemoting
    {
        internal readonly struct ParsedPacketMessage<TErrorMessage>
            where TErrorMessage : IRocketMessage<TErrorMessage>
        {
            private readonly TErrorMessage _errorMessage;
            private readonly PacketType _resultFlags;

            private ParsedPacketMessage(TErrorMessage errorMessage, PacketType flags)
            {
                _errorMessage = errorMessage;
                _resultFlags = flags;
            }

            public static ParsedPacketMessage<TErrorMessage> CreateUnknown()
            {
                return new ParsedPacketMessage<TErrorMessage>(IRocketMessage<TErrorMessage>.Empty, PacketType.Unknown);
            }

            public static ParsedPacketMessage<TErrorMessage> CreateCompleted()
            {
                return new ParsedPacketMessage<TErrorMessage>(IRocketMessage<TErrorMessage>.Empty, PacketType.Completed);
            }

            public static ParsedPacketMessage<TErrorMessage> CreateContinue()
            {
                return new ParsedPacketMessage<TErrorMessage>(IRocketMessage<TErrorMessage>.Empty, PacketType.Continue);
            }

            public static ParsedPacketMessage<TErrorMessage> CreateError(TErrorMessage errorMessage)
            {
                return new ParsedPacketMessage<TErrorMessage>(errorMessage, PacketType.Error);
            }

            public TErrorMessage ErrorMessage => _errorMessage;

            public bool IsUnknown => (_resultFlags & PacketType.Unknown) != 0;

            public bool IsCompleted => (_resultFlags & PacketType.Completed) != 0;

            public bool IsContinue => (_resultFlags & PacketType.Continue) != 0;

            public bool IsError => (_resultFlags & PacketType.Error) != 0;
        }

        internal readonly struct ParsedPacketMessage<TMessage, TErrorMessage>
            where TMessage : IRocketMessage<TMessage>
            where TErrorMessage : IRocketMessage<TErrorMessage>
        {
            [AllowNull]
            private readonly TMessage _message;
            private readonly TErrorMessage _errorMessage;
            private readonly PacketType _resultFlags;

            private ParsedPacketMessage(TMessage message, TErrorMessage errorMessage, PacketType flags)
            {
                _message = message;
                _errorMessage = errorMessage;
                _resultFlags = flags;
            }

            public static ParsedPacketMessage<TMessage, TErrorMessage> CreateUnknown()
            {
                return new ParsedPacketMessage<TMessage, TErrorMessage>(IRocketMessage<TMessage>.Empty, IRocketMessage<TErrorMessage>.Empty, PacketType.Unknown);
            }

            public static ParsedPacketMessage<TMessage, TErrorMessage> CreateCompleted(TMessage message)
            {
                return new ParsedPacketMessage<TMessage, TErrorMessage>(message, IRocketMessage<TErrorMessage>.Empty, PacketType.Completed);
            }

            public static ParsedPacketMessage<TMessage, TErrorMessage> CreateCompleted()
            {
                return new ParsedPacketMessage<TMessage, TErrorMessage>(IRocketMessage<TMessage>.Empty, IRocketMessage<TErrorMessage>.Empty, PacketType.Completed);
            }

            public static ParsedPacketMessage<TMessage, TErrorMessage> CreateContinue(TMessage message)
            {
                return new ParsedPacketMessage<TMessage, TErrorMessage>(message, IRocketMessage<TErrorMessage>.Empty, PacketType.Continue);
            }

            public static ParsedPacketMessage<TMessage, TErrorMessage> CreateContinue()
            {
                return new ParsedPacketMessage<TMessage, TErrorMessage>(IRocketMessage<TMessage>.Empty, IRocketMessage<TErrorMessage>.Empty, PacketType.Continue);
            }

            public static ParsedPacketMessage<TMessage, TErrorMessage> CreateError(TErrorMessage errorMessage)
            {
                return new ParsedPacketMessage<TMessage, TErrorMessage>(IRocketMessage<TMessage>.Empty, errorMessage, PacketType.Error);
            }

            public TMessage Message => _message;

            public TErrorMessage ErrorMessage => _errorMessage;

            public bool IsUnknown => (_resultFlags & PacketType.Unknown) != 0;

            public bool IsCompleted => (_resultFlags & PacketType.Completed) != 0;

            public bool IsContinue => (_resultFlags & PacketType.Continue) != 0;

            public bool IsError => (_resultFlags & PacketType.Error) != 0;
        }
    }
}
