
#nullable enable

namespace Omnix.Network
{
    public sealed partial class OmniAddress : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniAddress>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAddress> Formatter { get; }
        public static OmniAddress Empty { get; }

        static OmniAddress()
        {
            OmniAddress.Formatter = new ___CustomFormatter();
            OmniAddress.Empty = new OmniAddress(string.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxValueLength = 8192;

        public OmniAddress(string value)
        {
            if (value is null) throw new global::System.ArgumentNullException("value");
            if (value.Length > 8192) throw new global::System.ArgumentOutOfRangeException("value");

            this.Value = value;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (value != default) ___h.Add(value.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public string Value { get; }

        public static OmniAddress Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniAddress? left, OmniAddress? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniAddress? left, OmniAddress? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniAddress)) return false;
            return this.Equals((OmniAddress)other);
        }
        public bool Equals(OmniAddress? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Value != target.Value) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value!;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAddress>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniAddress value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.Value != string.Empty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.Value != string.Empty)
                {
                    w.Write((uint)0);
                    w.Write(value.Value);
                }
            }

            public OmniAddress Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                string p_value = string.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_value = r.GetString(8192);
                                break;
                            }
                    }
                }

                return new OmniAddress(p_value);
            }
        }
    }

}
