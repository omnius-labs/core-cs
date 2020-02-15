
#nullable enable

namespace Omnius.Core.Data.Internal
{
    public sealed partial class OmniFileDatabaseConfig : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<OmniFileDatabaseConfig>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<OmniFileDatabaseConfig> Formatter { get; }
        public static OmniFileDatabaseConfig Empty { get; }

        static OmniFileDatabaseConfig()
        {
            OmniFileDatabaseConfig.Formatter = new ___CustomFormatter();
            OmniFileDatabaseConfig.Empty = new OmniFileDatabaseConfig(0, new global::System.Collections.Generic.Dictionary<string, string>());
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxMapCount = 1048576;

        public OmniFileDatabaseConfig(uint version, global::System.Collections.Generic.Dictionary<string, string> map)
        {
            if (map is null) throw new global::System.ArgumentNullException("map");
            if (map.Count > 1048576) throw new global::System.ArgumentOutOfRangeException("map");
            foreach (var n in map)
            {
                if (n.Key is null) throw new global::System.ArgumentNullException("n.Key");
                if (n.Value is null) throw new global::System.ArgumentNullException("n.Value");
                if (n.Key.Length > 8192) throw new global::System.ArgumentOutOfRangeException("n.Key");
                if (n.Value.Length > 256) throw new global::System.ArgumentOutOfRangeException("n.Value");
            }

            this.Version = version;
            this.Map = new global::Omnius.Core.Collections.ReadOnlyDictionarySlim<string, string>(map);

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (version != default) ___h.Add(version.GetHashCode());
                foreach (var n in map)
                {
                    if (n.Key != default) ___h.Add(n.Key.GetHashCode());
                    if (n.Value != default) ___h.Add(n.Value.GetHashCode());
                }
                return ___h.ToHashCode();
            });
        }

        public uint Version { get; }
        public global::Omnius.Core.Collections.ReadOnlyDictionarySlim<string, string> Map { get; }

        public static OmniFileDatabaseConfig Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniFileDatabaseConfig? left, OmniFileDatabaseConfig? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniFileDatabaseConfig? left, OmniFileDatabaseConfig? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniFileDatabaseConfig)) return false;
            return this.Equals((OmniFileDatabaseConfig)other);
        }
        public bool Equals(OmniFileDatabaseConfig? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Version != target.Version) return false;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.Map, target.Map)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<OmniFileDatabaseConfig>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in OmniFileDatabaseConfig value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.Version != 0)
                    {
                        propertyCount++;
                    }
                    if (value.Map.Count != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.Version != 0)
                {
                    w.Write((uint)0);
                    w.Write(value.Version);
                }
                if (value.Map.Count != 0)
                {
                    w.Write((uint)1);
                    w.Write((uint)value.Map.Count);
                    foreach (var n in value.Map)
                    {
                        w.Write(n.Key);
                        w.Write(n.Value);
                    }
                }
            }

            public OmniFileDatabaseConfig Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                uint p_version = 0;
                global::System.Collections.Generic.Dictionary<string, string> p_map = new global::System.Collections.Generic.Dictionary<string, string>();

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_version = r.GetUInt32();
                                break;
                            }
                        case 1:
                            {
                                var length = r.GetUInt32();
                                p_map = new global::System.Collections.Generic.Dictionary<string, string>();
                                string t_key = string.Empty;
                                string t_value = string.Empty;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = r.GetString(8192);
                                    t_value = r.GetString(256);
                                    p_map[t_key] = t_value;
                                }
                                break;
                            }
                    }
                }

                return new OmniFileDatabaseConfig(p_version, p_map);
            }
        }
    }

}
