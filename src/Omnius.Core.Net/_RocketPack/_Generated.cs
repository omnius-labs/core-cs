// <auto-generated/>
#nullable enable

namespace Omnius.Core.Net;

public sealed partial class OmniAddress : global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.OmniAddress>
{
    public static global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.Net.OmniAddress> Formatter => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.OmniAddress>.Formatter;
    public static global::Omnius.Core.Net.OmniAddress Empty => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.OmniAddress>.Empty;

    static OmniAddress()
    {
        global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.OmniAddress>.Formatter = new ___CustomFormatter();
        global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.OmniAddress>.Empty = new global::Omnius.Core.Net.OmniAddress(global::Omnius.Core.RocketPack.Utf8String.Empty);
    }

    private readonly global::System.Lazy<int> ___hashCode;

    public static readonly int MaxValueLength = 8192;

    public OmniAddress(global::Omnius.Core.RocketPack.Utf8String value)
    {
        if (value is null) throw new global::System.ArgumentNullException("value");
        if (value.Length > 8192) throw new global::System.ArgumentOutOfRangeException("value");

        this.Value = value;

        ___hashCode = new global::System.Lazy<int>(() =>
        {
            var ___h = new global::System.HashCode();
            if (!value.IsEmpty) ___h.Add(value.GetHashCode());
            return ___h.ToHashCode();
        });
    }

    public global::Omnius.Core.RocketPack.Utf8String Value { get; }

    public static global::Omnius.Core.Net.OmniAddress Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
    {
        var reader = new global::Omnius.Core.RocketPack.RocketMessageReader(sequence, bytesPool);
        return Formatter.Deserialize(ref reader, 0);
    }
    public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
    {
        var writer = new global::Omnius.Core.RocketPack.RocketMessageWriter(bufferWriter, bytesPool);
        Formatter.Serialize(ref writer, this, 0);
    }

    public static bool operator ==(global::Omnius.Core.Net.OmniAddress? left, global::Omnius.Core.Net.OmniAddress? right)
    {
        return (right is null) ? (left is null) : right.Equals(left);
    }
    public static bool operator !=(global::Omnius.Core.Net.OmniAddress? left, global::Omnius.Core.Net.OmniAddress? right)
    {
        return !(left == right);
    }
    public override bool Equals(object? other)
    {
        if (other is not global::Omnius.Core.Net.OmniAddress) return false;
        return this.Equals((global::Omnius.Core.Net.OmniAddress)other);
    }
    public bool Equals(global::Omnius.Core.Net.OmniAddress? target)
    {
        if (target is null) return false;
        if (object.ReferenceEquals(this, target)) return true;
        if (this.Value != target.Value) return false;

        return true;
    }
    public override int GetHashCode() => ___hashCode.Value;

    private sealed class ___CustomFormatter : global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.Net.OmniAddress>
    {
        public void Serialize(ref global::Omnius.Core.RocketPack.RocketMessageWriter w, scoped in global::Omnius.Core.Net.OmniAddress value, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            if (value.Value != global::Omnius.Core.RocketPack.Utf8String.Empty)
            {
                w.Write((uint)1);
                w.Write(value.Value);
            }
            w.Write((uint)0);
        }
        public global::Omnius.Core.Net.OmniAddress Deserialize(ref global::Omnius.Core.RocketPack.RocketMessageReader r, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            global::Omnius.Core.RocketPack.Utf8String p_value = global::Omnius.Core.RocketPack.Utf8String.Empty;

            for (; ; )
            {
                uint id = r.GetUInt32();
                if (id == 0) break;
                switch (id)
                {
                    case 1:
                        {
                            p_value = r.GetString(8192);
                            break;
                        }
                }
            }

            return new global::Omnius.Core.Net.OmniAddress(p_value);
        }
    }
}
