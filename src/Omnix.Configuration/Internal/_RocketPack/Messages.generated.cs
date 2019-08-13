
#nullable enable

namespace Omnix.Configuration.Internal
{
    public readonly struct SettingsDatabaseVersion : global::System.IEquatable<SettingsDatabaseVersion>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SettingsDatabaseVersion> Formatter { get; }
        public static SettingsDatabaseVersion Empty { get; }

        static SettingsDatabaseVersion()
        {
            SettingsDatabaseVersion.Formatter = new CustomFormatter();
            SettingsDatabaseVersion.Empty = new SettingsDatabaseVersion(0);
        }

        private readonly int __hashCode;

        public SettingsDatabaseVersion(uint value)
        {
            this.Value = value;

            {
                var __h = new global::System.HashCode();
                if (this.Value != default) __h.Add(this.Value.GetHashCode());
                __hashCode = __h.ToHashCode();
            }
        }

        public uint Value { get; }

        public static SettingsDatabaseVersion Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
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

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SettingsDatabaseVersion>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, SettingsDatabaseVersion value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                if (value.Value != 0)
                {
                    w.Write(value.Value);
                }
            }

            public SettingsDatabaseVersion Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
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
