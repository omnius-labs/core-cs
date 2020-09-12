
#nullable enable

namespace Omnius.Core.Network
{

    public sealed partial class OmniAddress : global::Omnius.Core.RocketPack.IRocketPackObject<global::Omnius.Core.Network.OmniAddress>
    {
        public static global::Omnius.Core.RocketPack.IRocketPackObjectFormatter<global::Omnius.Core.Network.OmniAddress> Formatter => global::Omnius.Core.RocketPack.IRocketPackObject<global::Omnius.Core.Network.OmniAddress>.Formatter;
        public static global::Omnius.Core.Network.OmniAddress Empty => global::Omnius.Core.RocketPack.IRocketPackObject<global::Omnius.Core.Network.OmniAddress>.Empty;

        static OmniAddress()
        {
            global::Omnius.Core.RocketPack.IRocketPackObject<global::Omnius.Core.Network.OmniAddress>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.RocketPack.IRocketPackObject<global::Omnius.Core.Network.OmniAddress>.Empty = new global::Omnius.Core.Network.OmniAddress(string.Empty);
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

        public static global::Omnius.Core.Network.OmniAddress Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.RocketPack.RocketPackObjectReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.RocketPack.RocketPackObjectWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Network.OmniAddress? left, global::Omnius.Core.Network.OmniAddress? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Network.OmniAddress? left, global::Omnius.Core.Network.OmniAddress? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Network.OmniAddress)) return false;
            return this.Equals((global::Omnius.Core.Network.OmniAddress)other);
        }
        public bool Equals(global::Omnius.Core.Network.OmniAddress? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Value != target.Value) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnius.Core.RocketPack.IRocketPackObjectFormatter<global::Omnius.Core.Network.OmniAddress>
        {
            public void Serialize(ref global::Omnius.Core.RocketPack.RocketPackObjectWriter w, in global::Omnius.Core.Network.OmniAddress value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                if (value.Value != string.Empty)
                {
                    w.Write((uint)1);
                    w.Write(value.Value);
                }
                w.Write((uint)0);
            }

            public global::Omnius.Core.Network.OmniAddress Deserialize(ref global::Omnius.Core.RocketPack.RocketPackObjectReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                string p_value = string.Empty;

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

                return new global::Omnius.Core.Network.OmniAddress(p_value);
            }
        }
    }


}
