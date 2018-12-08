using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Cryptography;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Network.Connection.Secure
{
    public enum SecureConnectionType : byte
    {
        Connect = 0,
        Accept = 1,
    }

    public enum SecureConnectionVersion : byte
    {
        Version1 = 1,
    }

    public sealed partial class SecureConnectionHelloMessage : RocketPackMessageBase<SecureConnectionHelloMessage>
    {
        static SecureConnectionHelloMessage()
        {
            SecureConnectionHelloMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxVersionsCount = 32;

        public SecureConnectionHelloMessage(IList<SecureConnectionVersion> versions)
        {
            if (versions is null) throw new ArgumentNullException("versions");
            if (versions.Count > 32) throw new ArgumentOutOfRangeException("versions");

            this.Versions = new ReadOnlyCollection<SecureConnectionVersion>(versions);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Versions)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<SecureConnectionVersion> Versions { get; }

        public override bool Equals(SecureConnectionHelloMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Versions is null) != (target.Versions is null)) return false;
            if (!(this.Versions is null) && !(target.Versions is null) && !CollectionHelper.Equals(this.Versions, target.Versions)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<SecureConnectionHelloMessage>
        {
            public void Serialize(RocketPackWriter w, SecureConnectionHelloMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Versions
                if (value.Versions.Count != 0)
                {
                    w.Write((ulong)0);
                    w.Write((ulong)value.Versions.Count);
                    foreach (var n in value.Versions)
                    {
                        w.Write((ulong)n);
                    }
                }
            }

            public SecureConnectionHelloMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                IList<SecureConnectionVersion> p_versions = default;

                while (r.Available > 0)
                {
                    int id = (int)r.GetUInt64();
                    switch (id)
                    {
                        case 0: // Versions
                            {
                                var length = (int)r.GetUInt64();
                                p_versions = new SecureConnectionVersion[length];
                                for (int i = 0; i < p_versions.Count; i++)
                                {
                                    p_versions[i] = (SecureConnectionVersion)r.GetUInt64();
                                }
                                break;
                            }
                    }
                }

                return new SecureConnectionHelloMessage(p_versions);
            }
        }
    }

}
