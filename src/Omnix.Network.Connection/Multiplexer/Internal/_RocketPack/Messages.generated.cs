using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Network.Connection.Multiplexer.Internal
{
    internal enum CommunicatorVersion : byte
    {
        Version1 = 1,
    }

    internal sealed partial class HelloMessage : RocketPackMessageBase<HelloMessage>
    {
        static HelloMessage()
        {
            HelloMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxVersionsCount = 32;

        public HelloMessage(IList<CommunicatorVersion> versions)
        {
            if (versions is null) throw new ArgumentNullException("versions");
            if (versions.Count > 32) throw new ArgumentOutOfRangeException("versions");

            this.Versions = new ReadOnlyCollection<CommunicatorVersion>(versions);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Versions)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<CommunicatorVersion> Versions { get; }

        public override bool Equals(HelloMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Versions is null) != (target.Versions is null)) return false;
            if (!(this.Versions is null) && !(target.Versions is null) && !CollectionHelper.Equals(this.Versions, target.Versions)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<HelloMessage>
        {
            public void Serialize(RocketPackWriter w, HelloMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Versions.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Versions
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

            public HelloMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IList<CommunicatorVersion> p_versions = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Versions
                            {
                                var length = r.GetUInt32();
                                p_versions = new CommunicatorVersion[length];
                                for (int i = 0; i < p_versions.Count; i++)
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
