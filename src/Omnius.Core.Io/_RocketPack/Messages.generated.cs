
#nullable enable

namespace Omnius.Core.Io
{
    public sealed partial class OmniPath : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Io.OmniPath>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Io.OmniPath> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Io.OmniPath>.Formatter;
        public static global::Omnius.Core.Io.OmniPath Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Io.OmniPath>.Empty;

        static OmniPath()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Io.OmniPath>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Io.OmniPath>.Empty = new global::Omnius.Core.Io.OmniPath(string.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxValueLength = 8192;

        public OmniPath(string value)
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

        public static global::Omnius.Core.Io.OmniPath Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Io.OmniPath? left, global::Omnius.Core.Io.OmniPath? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Io.OmniPath? left, global::Omnius.Core.Io.OmniPath? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Io.OmniPath)) return false;
            return this.Equals((global::Omnius.Core.Io.OmniPath)other);
        }
        public bool Equals(global::Omnius.Core.Io.OmniPath? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Value != target.Value) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Io.OmniPath>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Io.OmniPath value, in int rank)
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

            public global::Omnius.Core.Io.OmniPath Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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

                return new global::Omnius.Core.Io.OmniPath(p_value);
            }
        }
    }

}
