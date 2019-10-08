
#nullable enable

namespace Omnix.Configuration.Internal
{
    public readonly struct OmniSettingsVersion : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniSettingsVersion>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniSettingsVersion> Formatter { get; }
        public static OmniSettingsVersion Empty { get; }

        static OmniSettingsVersion()
        {
            OmniSettingsVersion.Formatter = new ___CustomFormatter();
            OmniSettingsVersion.Empty = new OmniSettingsVersion(0);
        }

        private readonly int ___hashCode;

        public OmniSettingsVersion(uint value)
        {
            this.Value = value;

            {
                var ___h = new global::System.HashCode();
                if (value != default) ___h.Add(value.GetHashCode());
                ___hashCode = ___h.ToHashCode();
            }
        }

        public uint Value { get; }

        public static OmniSettingsVersion Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniSettingsVersion left, OmniSettingsVersion right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(OmniSettingsVersion left, OmniSettingsVersion right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniSettingsVersion)) return false;
            return this.Equals((OmniSettingsVersion)other);
        }
        public bool Equals(OmniSettingsVersion target)
        {
            if (this.Value != target.Value) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniSettingsVersion>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniSettingsVersion value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.Value);
            }

            public OmniSettingsVersion Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint p_value = 0;

                {
                    p_value = r.GetUInt32();
                }
                return new OmniSettingsVersion(p_value);
            }
        }
    }

}
