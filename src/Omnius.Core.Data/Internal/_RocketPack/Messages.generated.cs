
#nullable enable

namespace Omnius.Core.Data.Internal
{
    public readonly struct OmniDatabaseConfig : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<OmniDatabaseConfig>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<OmniDatabaseConfig> Formatter { get; }
        public static OmniDatabaseConfig Empty { get; }

        static OmniDatabaseConfig()
        {
            OmniDatabaseConfig.Formatter = new ___CustomFormatter();
            OmniDatabaseConfig.Empty = new OmniDatabaseConfig(0);
        }

        private readonly int ___hashCode;

        public OmniDatabaseConfig(uint value)
        {
            this.Value = value;

            {
                var ___h = new global::System.HashCode();
                if (value != default) ___h.Add(value.GetHashCode());
                ___hashCode = ___h.ToHashCode();
            }
        }

        public uint Value { get; }

        public static OmniDatabaseConfig Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniDatabaseConfig left, OmniDatabaseConfig right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(OmniDatabaseConfig left, OmniDatabaseConfig right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniDatabaseConfig)) return false;
            return this.Equals((OmniDatabaseConfig)other);
        }
        public bool Equals(OmniDatabaseConfig target)
        {
            if (this.Value != target.Value) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<OmniDatabaseConfig>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in OmniDatabaseConfig value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.Value);
            }

            public OmniDatabaseConfig Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint p_value = 0;

                {
                    p_value = r.GetUInt32();
                }
                return new OmniDatabaseConfig(p_value);
            }
        }
    }

}
