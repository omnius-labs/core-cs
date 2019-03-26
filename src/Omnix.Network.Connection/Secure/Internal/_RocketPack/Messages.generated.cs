using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Collections;
using Omnix.Cryptography;
using Omnix.Network.Connection.Secure;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Network.Connection.Secure.Internal
{
    internal sealed partial class HelloMessage : RocketPackMessageBase<HelloMessage>
    {
        static HelloMessage()
        {
            HelloMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxVersionsCount = 32;

        public HelloMessage(OmniSecureConnectionVersion[] versions)
        {
            if (versions is null) throw new ArgumentNullException("versions");
            if (versions.Length > 32) throw new ArgumentOutOfRangeException("versions");

            this.Versions = new ReadOnlyListSlim<OmniSecureConnectionVersion>(versions);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Versions)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ReadOnlyListSlim<OmniSecureConnectionVersion> Versions { get; }

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

                OmniSecureConnectionVersion[] p_versions = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Versions
                            {
                                var length = r.GetUInt32();
                                p_versions = new OmniSecureConnectionVersion[length];
                                for (int i = 0; i < p_versions.Length; i++)
                                {
                                    p_versions[i] = (OmniSecureConnectionVersion)r.GetUInt64();
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
