// <auto-generated/>
#nullable enable

namespace Omnius.Core.RocketPack.Remoting;

public sealed partial class DefaultErrorMessage : global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage>
{
    public static global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage> Formatter => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage>.Formatter;
    public static global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage Empty => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage>.Empty;

    static DefaultErrorMessage()
    {
        global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage>.Formatter = new ___CustomFormatter();
        global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage>.Empty = new global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage(global::Omnius.Core.RocketPack.Utf8String.Empty, global::Omnius.Core.RocketPack.Utf8String.Empty, global::Omnius.Core.RocketPack.Utf8String.Empty);
    }

    private readonly global::System.Lazy<int> ___hashCode;

    public static readonly int MaxTypeLength = 8192;
    public static readonly int MaxMessageLength = 8192;
    public static readonly int MaxStackTraceLength = 8192;

    public DefaultErrorMessage(global::Omnius.Core.RocketPack.Utf8String type, global::Omnius.Core.RocketPack.Utf8String message, global::Omnius.Core.RocketPack.Utf8String stackTrace)
    {
        if (type is null) throw new global::System.ArgumentNullException("type");
        if (type.Length > 8192) throw new global::System.ArgumentOutOfRangeException("type");
        if (message is null) throw new global::System.ArgumentNullException("message");
        if (message.Length > 8192) throw new global::System.ArgumentOutOfRangeException("message");
        if (stackTrace is null) throw new global::System.ArgumentNullException("stackTrace");
        if (stackTrace.Length > 8192) throw new global::System.ArgumentOutOfRangeException("stackTrace");

        this.Type = type;
        this.Message = message;
        this.StackTrace = stackTrace;

        ___hashCode = new global::System.Lazy<int>(() =>
        {
            var ___h = new global::System.HashCode();
            if (!type.IsEmpty) ___h.Add(type.GetHashCode());
            if (!message.IsEmpty) ___h.Add(message.GetHashCode());
            if (!stackTrace.IsEmpty) ___h.Add(stackTrace.GetHashCode());
            return ___h.ToHashCode();
        });
    }

    public global::Omnius.Core.RocketPack.Utf8String Type { get; }
    public global::Omnius.Core.RocketPack.Utf8String Message { get; }
    public global::Omnius.Core.RocketPack.Utf8String StackTrace { get; }

    public static global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
    {
        var reader = new global::Omnius.Core.RocketPack.RocketMessageReader(sequence, bytesPool);
        return Formatter.Deserialize(ref reader, 0);
    }
    public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
    {
        var writer = new global::Omnius.Core.RocketPack.RocketMessageWriter(bufferWriter, bytesPool);
        Formatter.Serialize(ref writer, this, 0);
    }

    public static bool operator ==(global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage? left, global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage? right)
    {
        return (right is null) ? (left is null) : right.Equals(left);
    }
    public static bool operator !=(global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage? left, global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage? right)
    {
        return !(left == right);
    }
    public override bool Equals(object? other)
    {
        if (other is not global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage) return false;
        return this.Equals((global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage)other);
    }
    public bool Equals(global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage? target)
    {
        if (target is null) return false;
        if (object.ReferenceEquals(this, target)) return true;
        if (this.Type != target.Type) return false;
        if (this.Message != target.Message) return false;
        if (this.StackTrace != target.StackTrace) return false;

        return true;
    }
    public override int GetHashCode() => ___hashCode.Value;

    private sealed class ___CustomFormatter : global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage>
    {
        public void Serialize(ref global::Omnius.Core.RocketPack.RocketMessageWriter w, scoped in global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage value, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            if (value.Type != global::Omnius.Core.RocketPack.Utf8String.Empty)
            {
                w.Write((uint)1);
                w.Write(value.Type);
            }
            if (value.Message != global::Omnius.Core.RocketPack.Utf8String.Empty)
            {
                w.Write((uint)2);
                w.Write(value.Message);
            }
            if (value.StackTrace != global::Omnius.Core.RocketPack.Utf8String.Empty)
            {
                w.Write((uint)3);
                w.Write(value.StackTrace);
            }
            w.Write((uint)0);
        }
        public global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage Deserialize(ref global::Omnius.Core.RocketPack.RocketMessageReader r, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            global::Omnius.Core.RocketPack.Utf8String p_type = global::Omnius.Core.RocketPack.Utf8String.Empty;
            global::Omnius.Core.RocketPack.Utf8String p_message = global::Omnius.Core.RocketPack.Utf8String.Empty;
            global::Omnius.Core.RocketPack.Utf8String p_stackTrace = global::Omnius.Core.RocketPack.Utf8String.Empty;

            for (; ; )
            {
                uint id = r.GetUInt32();
                if (id == 0) break;
                switch (id)
                {
                    case 1:
                        {
                            p_type = r.GetString(8192);
                            break;
                        }
                    case 2:
                        {
                            p_message = r.GetString(8192);
                            break;
                        }
                    case 3:
                        {
                            p_stackTrace = r.GetString(8192);
                            break;
                        }
                }
            }

            return new global::Omnius.Core.RocketPack.Remoting.DefaultErrorMessage(p_type, p_message, p_stackTrace);
        }
    }
}
