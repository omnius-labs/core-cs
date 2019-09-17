
#nullable enable

namespace Omnix.Configuration.Internal
{
    public readonly struct SettingsDatabaseVersion : global::Omnix.Serialization.OmniPack.IOmniPackMessage<SettingsDatabaseVersion>
    {
        public static global::Omnix.Serialization.OmniPack.IOmniPackFormatter<SettingsDatabaseVersion> Formatter { get; }
        public static SettingsDatabaseVersion Empty { get; }

        static SettingsDatabaseVersion()
        {
            SettingsDatabaseVersion.Formatter = new ___CustomFormatter();
            SettingsDatabaseVersion.Empty = new SettingsDatabaseVersion(0);
        }

        private readonly int ___hashCode;

        public SettingsDatabaseVersion(uint value)
        {
            this.Value = value;

            {
                var ___h = new global::System.HashCode();
                if (value != default) ___h.Add(value.GetHashCode());
                ___hashCode = ___h.ToHashCode();
            }
        }

        public uint Value { get; }

        public static SettingsDatabaseVersion Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.OmniPack.OmniPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.OmniPack.OmniPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SettingsDatabaseVersion left, SettingsDatabaseVersion right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(SettingsDatabaseVersion left, SettingsDatabaseVersion right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SettingsDatabaseVersion)) return false;
            return this.Equals((SettingsDatabaseVersion)other);
        }
        public bool Equals(SettingsDatabaseVersion target)
        {
            if (this.Value != target.Value) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.OmniPack.IOmniPackFormatter<SettingsDatabaseVersion>
        {
            public void Serialize(ref global::Omnix.Serialization.OmniPack.OmniPackWriter w, in SettingsDatabaseVersion value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.Value);
            }

            public SettingsDatabaseVersion Deserialize(ref global::Omnix.Serialization.OmniPack.OmniPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint p_value = 0;

                {
                    p_value = r.GetUInt32();
                }
                return new SettingsDatabaseVersion(p_value);
            }
        }
    }

}
