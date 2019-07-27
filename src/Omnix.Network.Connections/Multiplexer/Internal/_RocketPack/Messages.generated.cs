
#nullable enable

namespace Omnix.Network.Connections.Multiplexer.Internal
{
    internal enum CommunicatorVersion : byte
    {
        Version1 = 1,
    }

    internal sealed partial class HelloMessage : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<HelloMessage>
    {
        static HelloMessage()
        {
            HelloMessage.Formatter = new CustomFormatter();
            HelloMessage.Empty = new HelloMessage(global::System.Array.Empty<CommunicatorVersion>());
        }

        private readonly int __hashCode;

        public static readonly int MaxVersionsCount = 32;

        public HelloMessage(CommunicatorVersion[] versions)
        {
            if (versions is null) throw new global::System.ArgumentNullException("versions");
            if (versions.Length > 32) throw new global::System.ArgumentOutOfRangeException("versions");

            this.Versions = new global::Omnix.DataStructures.ReadOnlyListSlim<CommunicatorVersion>(versions);

            {
                var __h = new global::System.HashCode();
                foreach (var n in this.Versions)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                __hashCode = __h.ToHashCode();
            }
        }

        public global::Omnix.DataStructures.ReadOnlyListSlim<CommunicatorVersion> Versions { get; }

        public override bool Equals(HelloMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.Versions, target.Versions)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<HelloMessage>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, HelloMessage value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.Versions.Count != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.Versions.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Versions.Count);
                    foreach (var n in value.Versions)
                    {
                        w.Write((ulong)n);
                    }
                }
            }

            public HelloMessage Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                CommunicatorVersion[] p_versions = global::System.Array.Empty<CommunicatorVersion>();

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                var length = r.GetUInt32();
                                p_versions = new CommunicatorVersion[length];
                                for (int i = 0; i < p_versions.Length; i++)
                                {
                                    p_versions[i] = (CommunicatorVersion)r.GetUInt64();
                                }
                                break;
                            }
                    }
                }

                return new HelloMessage(p_versions);
            }
        }
    }

}
