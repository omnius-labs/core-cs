using System.Diagnostics.CodeAnalysis;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting.Internal;

[Flags]
internal enum PacketType : byte
{
    Unknown = 0,
    Completed = 1,
    Continue = 2,
    Error = 3,
}

internal class PacketMessage<TMessage, TErrorMessage> : RocketMessage<PacketMessage<TMessage, TErrorMessage>>
    where TMessage : RocketMessage<TMessage>
    where TErrorMessage : RocketMessage<TErrorMessage>
{
    private readonly PacketType _type;
    private readonly TMessage _message;
    private readonly TErrorMessage _errorMessage;

    private PacketMessage(PacketType type, TMessage message, TErrorMessage errorMessage)
    {
        _type = type;
        _message = message;
        _errorMessage = errorMessage;
    }

    public static PacketMessage<TMessage, TErrorMessage> CreateUnknown()
    {
        return new PacketMessage<TMessage, TErrorMessage>(PacketType.Unknown, RocketMessage<TMessage>.Empty, RocketMessage<TErrorMessage>.Empty);
    }

    public static PacketMessage<TMessage, TErrorMessage> CreateCompleted(TMessage message)
    {
        return new PacketMessage<TMessage, TErrorMessage>(PacketType.Completed, message, RocketMessage<TErrorMessage>.Empty);
    }

    public static PacketMessage<TMessage, TErrorMessage> CreateContinue(TMessage message)
    {
        return new PacketMessage<TMessage, TErrorMessage>(PacketType.Continue, message, RocketMessage<TErrorMessage>.Empty);
    }

    public static PacketMessage<TMessage, TErrorMessage> CreateError(TErrorMessage errorMessage)
    {
        return new PacketMessage<TMessage, TErrorMessage>(PacketType.Error, RocketMessage<TMessage>.Empty, errorMessage);
    }

    public TMessage Message => _message;
    public TErrorMessage ErrorMessage => _errorMessage;

    public bool IsUnknown => (_type & PacketType.Unknown) != 0;
    public bool IsCompleted => (_type & PacketType.Completed) != 0;
    public bool IsContinue => (_type & PacketType.Continue) != 0;
    public bool IsError => (_type & PacketType.Error) != 0;

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(_type);
            h.Add(_errorMessage);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public override bool Equals(PacketMessage<TMessage, TErrorMessage>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _type == other._type && _message == other._message && _errorMessage == other._errorMessage;
    }

    static PacketMessage()
    {
        Formatter = new CustomSerializer();
        Empty = PacketMessage<TMessage, TErrorMessage>.CreateUnknown();
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<PacketMessage<TMessage, TErrorMessage>>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in PacketMessage<TMessage, TErrorMessage> value, scoped in int depth)
        {
            w.Write((byte)value._type);

            if (value._type == PacketType.Continue || value._type == PacketType.Completed)
            {
                RocketMessage<TMessage>.Formatter.Serialize(ref w, value._message, depth + 1);
            }
            else if (value._type == PacketType.Error)
            {
                RocketMessage<TErrorMessage>.Formatter.Serialize(ref w, value._errorMessage, depth + 1);
            }
        }
        public PacketMessage<TMessage, TErrorMessage> Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var type = (PacketType)r.GetUInt8();
            var message = RocketMessage<TMessage>.Empty;
            var errorMessage = RocketMessage<TErrorMessage>.Empty;

            if (type == PacketType.Continue || type == PacketType.Completed)
            {
                message = RocketMessage<TMessage>.Formatter.Deserialize(ref r, depth + 1);
            }
            else if (type == PacketType.Error)
            {
                errorMessage = RocketMessage<TErrorMessage>.Formatter.Deserialize(ref r, depth + 1);
            }

            return new PacketMessage<TMessage, TErrorMessage>(type, message, errorMessage);
        }
    }
}
