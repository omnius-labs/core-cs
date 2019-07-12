
#nullable enable

namespace Omnix.Cryptography
{
    public enum OmniHashAlgorithmType : byte
    {
        Sha2_256 = 0,
    }

    public enum OmniAgreementAlgorithmType : byte
    {
        EcDh_P521_Sha2_256 = 0,
    }

    public enum OmniDigitalSignatureAlgorithmType : byte
    {
        EcDsa_P521_Sha2_256 = 0,
    }

    public enum OmniHashcashAlgorithmType : byte
    {
        Simple_Sha2_256 = 0,
    }

    public readonly struct OmniHash : System.IEquatable<OmniHash>
    {
        public static Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniHash> Formatter { get; }
        public static OmniHash Empty { get; }

        static OmniHash()
        {
            OmniHash.Formatter = new CustomFormatter();
            OmniHash.Empty = new OmniHash((OmniHashAlgorithmType)0, System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxValueLength = 256;

        public OmniHash(OmniHashAlgorithmType algorithmType, System.ReadOnlyMemory<byte> value)
        {
            if (value.Length > 256) throw new System.ArgumentOutOfRangeException("value");

            this.AlgorithmType = algorithmType;
            this.Value = value;

            {
                var __h = new System.HashCode();
                if (this.AlgorithmType != default) __h.Add(this.AlgorithmType.GetHashCode());
                if (!this.Value.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.Value.Span));
                __hashCode = __h.ToHashCode();
            }
        }

        public OmniHashAlgorithmType AlgorithmType { get; }
        public System.ReadOnlyMemory<byte> Value { get; }

        public static OmniHash Import(System.Buffers.ReadOnlySequence<byte> sequence, Omnix.Base.BufferPool bufferPool)
        {
            return Formatter.Deserialize(new Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool), 0);
        }
        public void Export(System.Buffers.IBufferWriter<byte> bufferWriter, Omnix.Base.BufferPool bufferPool)
        {
            Formatter.Serialize(new Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool), this, 0);
        }
        public static bool operator ==(OmniHash left, OmniHash right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(OmniHash left, OmniHash right)
        {
            return !(left == right);
        }
        public override bool Equals(object other)
        {
            if (!(other is OmniHash)) return false;
            return this.Equals((OmniHash)other);
        }

        public bool Equals(OmniHash target)
        {
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.Value.Span, target.Value.Span)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniHash>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, OmniHash value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                if (value.AlgorithmType != (OmniHashAlgorithmType)0)
                {
                    w.Write((ulong)value.AlgorithmType);
                }
                if (!value.Value.IsEmpty)
                {
                    w.Write(value.Value.Span);
                }
            }

            public OmniHash Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                OmniHashAlgorithmType p_algorithmType = (OmniHashAlgorithmType)0;
                System.ReadOnlyMemory<byte> p_value = System.ReadOnlyMemory<byte>.Empty;

                {
                    p_algorithmType = (OmniHashAlgorithmType)r.GetUInt64();
                }
                {
                    p_value = r.GetMemory(256);
                }
                return new OmniHash(p_algorithmType, p_value);
            }
        }
    }

    public sealed partial class OmniAgreement : Omnix.Serialization.RocketPack.RocketPackMessageBase<OmniAgreement>
    {
        static OmniAgreement()
        {
            OmniAgreement.Formatter = new CustomFormatter();
            OmniAgreement.Empty = new OmniAgreement(Omnix.Serialization.RocketPack.Timestamp.Zero, (OmniAgreementAlgorithmType)0, System.ReadOnlyMemory<byte>.Empty, System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxPublicKeyLength = 8192;
        public static readonly int MaxPrivateKeyLength = 8192;

        public OmniAgreement(Omnix.Serialization.RocketPack.Timestamp creationTime, OmniAgreementAlgorithmType algorithmType, System.ReadOnlyMemory<byte> publicKey, System.ReadOnlyMemory<byte> privateKey)
        {
            if (publicKey.Length > 8192) throw new System.ArgumentOutOfRangeException("publicKey");
            if (privateKey.Length > 8192) throw new System.ArgumentOutOfRangeException("privateKey");

            this.CreationTime = creationTime;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;

            {
                var __h = new System.HashCode();
                if (this.CreationTime != default) __h.Add(this.CreationTime.GetHashCode());
                if (this.AlgorithmType != default) __h.Add(this.AlgorithmType.GetHashCode());
                if (!this.PublicKey.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.PublicKey.Span));
                if (!this.PrivateKey.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.PrivateKey.Span));
                __hashCode = __h.ToHashCode();
            }
        }

        public Omnix.Serialization.RocketPack.Timestamp CreationTime { get; }
        public OmniAgreementAlgorithmType AlgorithmType { get; }
        public System.ReadOnlyMemory<byte> PublicKey { get; }
        public System.ReadOnlyMemory<byte> PrivateKey { get; }

        public override bool Equals(OmniAgreement? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.PrivateKey.Span, target.PrivateKey.Span)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAgreement>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, OmniAgreement value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.CreationTime != Omnix.Serialization.RocketPack.Timestamp.Zero)
                    {
                        propertyCount++;
                    }
                    if (value.AlgorithmType != (OmniAgreementAlgorithmType)0)
                    {
                        propertyCount++;
                    }
                    if (!value.PublicKey.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.PrivateKey.IsEmpty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.CreationTime != Omnix.Serialization.RocketPack.Timestamp.Zero)
                {
                    w.Write((uint)0);
                    w.Write(value.CreationTime);
                }
                if (value.AlgorithmType != (OmniAgreementAlgorithmType)0)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                if (!value.PublicKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PublicKey.Span);
                }
                if (!value.PrivateKey.IsEmpty)
                {
                    w.Write((uint)3);
                    w.Write(value.PrivateKey.Span);
                }
            }

            public OmniAgreement Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                Omnix.Serialization.RocketPack.Timestamp p_creationTime = Omnix.Serialization.RocketPack.Timestamp.Zero;
                OmniAgreementAlgorithmType p_algorithmType = (OmniAgreementAlgorithmType)0;
                System.ReadOnlyMemory<byte> p_publicKey = System.ReadOnlyMemory<byte>.Empty;
                System.ReadOnlyMemory<byte> p_privateKey = System.ReadOnlyMemory<byte>.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 1:
                            {
                                p_algorithmType = (OmniAgreementAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2:
                            {
                                p_publicKey = r.GetMemory(8192);
                                break;
                            }
                        case 3:
                            {
                                p_privateKey = r.GetMemory(8192);
                                break;
                            }
                    }
                }

                return new OmniAgreement(p_creationTime, p_algorithmType, p_publicKey, p_privateKey);
            }
        }
    }

    public sealed partial class OmniAgreementPublicKey : Omnix.Serialization.RocketPack.RocketPackMessageBase<OmniAgreementPublicKey>
    {
        static OmniAgreementPublicKey()
        {
            OmniAgreementPublicKey.Formatter = new CustomFormatter();
            OmniAgreementPublicKey.Empty = new OmniAgreementPublicKey(Omnix.Serialization.RocketPack.Timestamp.Zero, (OmniAgreementAlgorithmType)0, System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxPublicKeyLength = 8192;

        public OmniAgreementPublicKey(Omnix.Serialization.RocketPack.Timestamp creationTime, OmniAgreementAlgorithmType algorithmType, System.ReadOnlyMemory<byte> publicKey)
        {
            if (publicKey.Length > 8192) throw new System.ArgumentOutOfRangeException("publicKey");

            this.CreationTime = creationTime;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;

            {
                var __h = new System.HashCode();
                if (this.CreationTime != default) __h.Add(this.CreationTime.GetHashCode());
                if (this.AlgorithmType != default) __h.Add(this.AlgorithmType.GetHashCode());
                if (!this.PublicKey.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.PublicKey.Span));
                __hashCode = __h.ToHashCode();
            }
        }

        public Omnix.Serialization.RocketPack.Timestamp CreationTime { get; }
        public OmniAgreementAlgorithmType AlgorithmType { get; }
        public System.ReadOnlyMemory<byte> PublicKey { get; }

        public override bool Equals(OmniAgreementPublicKey? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAgreementPublicKey>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, OmniAgreementPublicKey value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.CreationTime != Omnix.Serialization.RocketPack.Timestamp.Zero)
                    {
                        propertyCount++;
                    }
                    if (value.AlgorithmType != (OmniAgreementAlgorithmType)0)
                    {
                        propertyCount++;
                    }
                    if (!value.PublicKey.IsEmpty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.CreationTime != Omnix.Serialization.RocketPack.Timestamp.Zero)
                {
                    w.Write((uint)0);
                    w.Write(value.CreationTime);
                }
                if (value.AlgorithmType != (OmniAgreementAlgorithmType)0)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                if (!value.PublicKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PublicKey.Span);
                }
            }

            public OmniAgreementPublicKey Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                Omnix.Serialization.RocketPack.Timestamp p_creationTime = Omnix.Serialization.RocketPack.Timestamp.Zero;
                OmniAgreementAlgorithmType p_algorithmType = (OmniAgreementAlgorithmType)0;
                System.ReadOnlyMemory<byte> p_publicKey = System.ReadOnlyMemory<byte>.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 1:
                            {
                                p_algorithmType = (OmniAgreementAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2:
                            {
                                p_publicKey = r.GetMemory(8192);
                                break;
                            }
                    }
                }

                return new OmniAgreementPublicKey(p_creationTime, p_algorithmType, p_publicKey);
            }
        }
    }

    public sealed partial class OmniAgreementPrivateKey : Omnix.Serialization.RocketPack.RocketPackMessageBase<OmniAgreementPrivateKey>
    {
        static OmniAgreementPrivateKey()
        {
            OmniAgreementPrivateKey.Formatter = new CustomFormatter();
            OmniAgreementPrivateKey.Empty = new OmniAgreementPrivateKey(Omnix.Serialization.RocketPack.Timestamp.Zero, (OmniAgreementAlgorithmType)0, System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxPrivateKeyLength = 8192;

        public OmniAgreementPrivateKey(Omnix.Serialization.RocketPack.Timestamp creationTime, OmniAgreementAlgorithmType algorithmType, System.ReadOnlyMemory<byte> privateKey)
        {
            if (privateKey.Length > 8192) throw new System.ArgumentOutOfRangeException("privateKey");

            this.CreationTime = creationTime;
            this.AlgorithmType = algorithmType;
            this.PrivateKey = privateKey;

            {
                var __h = new System.HashCode();
                if (this.CreationTime != default) __h.Add(this.CreationTime.GetHashCode());
                if (this.AlgorithmType != default) __h.Add(this.AlgorithmType.GetHashCode());
                if (!this.PrivateKey.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.PrivateKey.Span));
                __hashCode = __h.ToHashCode();
            }
        }

        public Omnix.Serialization.RocketPack.Timestamp CreationTime { get; }
        public OmniAgreementAlgorithmType AlgorithmType { get; }
        public System.ReadOnlyMemory<byte> PrivateKey { get; }

        public override bool Equals(OmniAgreementPrivateKey? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.PrivateKey.Span, target.PrivateKey.Span)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAgreementPrivateKey>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, OmniAgreementPrivateKey value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.CreationTime != Omnix.Serialization.RocketPack.Timestamp.Zero)
                    {
                        propertyCount++;
                    }
                    if (value.AlgorithmType != (OmniAgreementAlgorithmType)0)
                    {
                        propertyCount++;
                    }
                    if (!value.PrivateKey.IsEmpty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.CreationTime != Omnix.Serialization.RocketPack.Timestamp.Zero)
                {
                    w.Write((uint)0);
                    w.Write(value.CreationTime);
                }
                if (value.AlgorithmType != (OmniAgreementAlgorithmType)0)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                if (!value.PrivateKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PrivateKey.Span);
                }
            }

            public OmniAgreementPrivateKey Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                Omnix.Serialization.RocketPack.Timestamp p_creationTime = Omnix.Serialization.RocketPack.Timestamp.Zero;
                OmniAgreementAlgorithmType p_algorithmType = (OmniAgreementAlgorithmType)0;
                System.ReadOnlyMemory<byte> p_privateKey = System.ReadOnlyMemory<byte>.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 1:
                            {
                                p_algorithmType = (OmniAgreementAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2:
                            {
                                p_privateKey = r.GetMemory(8192);
                                break;
                            }
                    }
                }

                return new OmniAgreementPrivateKey(p_creationTime, p_algorithmType, p_privateKey);
            }
        }
    }

    public sealed partial class OmniDigitalSignature : Omnix.Serialization.RocketPack.RocketPackMessageBase<OmniDigitalSignature>
    {
        static OmniDigitalSignature()
        {
            OmniDigitalSignature.Formatter = new CustomFormatter();
            OmniDigitalSignature.Empty = new OmniDigitalSignature(string.Empty, (OmniDigitalSignatureAlgorithmType)0, System.ReadOnlyMemory<byte>.Empty, System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxNameLength = 32;
        public static readonly int MaxPublicKeyLength = 8192;
        public static readonly int MaxPrivateKeyLength = 8192;

        public OmniDigitalSignature(string name, OmniDigitalSignatureAlgorithmType algorithmType, System.ReadOnlyMemory<byte> publicKey, System.ReadOnlyMemory<byte> privateKey)
        {
            if (name is null) throw new System.ArgumentNullException("name");
            if (name.Length > 32) throw new System.ArgumentOutOfRangeException("name");
            if (publicKey.Length > 8192) throw new System.ArgumentOutOfRangeException("publicKey");
            if (privateKey.Length > 8192) throw new System.ArgumentOutOfRangeException("privateKey");

            this.Name = name;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;

            {
                var __h = new System.HashCode();
                if (this.Name != default) __h.Add(this.Name.GetHashCode());
                if (this.AlgorithmType != default) __h.Add(this.AlgorithmType.GetHashCode());
                if (!this.PublicKey.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.PublicKey.Span));
                if (!this.PrivateKey.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.PrivateKey.Span));
                __hashCode = __h.ToHashCode();
            }
        }

        public string Name { get; }
        public OmniDigitalSignatureAlgorithmType AlgorithmType { get; }
        public System.ReadOnlyMemory<byte> PublicKey { get; }
        public System.ReadOnlyMemory<byte> PrivateKey { get; }

        public override bool Equals(OmniDigitalSignature? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Name != target.Name) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.PrivateKey.Span, target.PrivateKey.Span)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniDigitalSignature>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, OmniDigitalSignature value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.Name != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.AlgorithmType != (OmniDigitalSignatureAlgorithmType)0)
                    {
                        propertyCount++;
                    }
                    if (!value.PublicKey.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.PrivateKey.IsEmpty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.Name != string.Empty)
                {
                    w.Write((uint)0);
                    w.Write(value.Name);
                }
                if (value.AlgorithmType != (OmniDigitalSignatureAlgorithmType)0)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                if (!value.PublicKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PublicKey.Span);
                }
                if (!value.PrivateKey.IsEmpty)
                {
                    w.Write((uint)3);
                    w.Write(value.PrivateKey.Span);
                }
            }

            public OmniDigitalSignature Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                string p_name = string.Empty;
                OmniDigitalSignatureAlgorithmType p_algorithmType = (OmniDigitalSignatureAlgorithmType)0;
                System.ReadOnlyMemory<byte> p_publicKey = System.ReadOnlyMemory<byte>.Empty;
                System.ReadOnlyMemory<byte> p_privateKey = System.ReadOnlyMemory<byte>.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_name = r.GetString(32);
                                break;
                            }
                        case 1:
                            {
                                p_algorithmType = (OmniDigitalSignatureAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2:
                            {
                                p_publicKey = r.GetMemory(8192);
                                break;
                            }
                        case 3:
                            {
                                p_privateKey = r.GetMemory(8192);
                                break;
                            }
                    }
                }

                return new OmniDigitalSignature(p_name, p_algorithmType, p_publicKey, p_privateKey);
            }
        }
    }

    public sealed partial class OmniCertificate : Omnix.Serialization.RocketPack.RocketPackMessageBase<OmniCertificate>
    {
        static OmniCertificate()
        {
            OmniCertificate.Formatter = new CustomFormatter();
            OmniCertificate.Empty = new OmniCertificate(string.Empty, (OmniDigitalSignatureAlgorithmType)0, System.ReadOnlyMemory<byte>.Empty, System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxNameLength = 32;
        public static readonly int MaxPublicKeyLength = 8192;
        public static readonly int MaxValueLength = 8192;

        public OmniCertificate(string name, OmniDigitalSignatureAlgorithmType algorithmType, System.ReadOnlyMemory<byte> publicKey, System.ReadOnlyMemory<byte> value)
        {
            if (name is null) throw new System.ArgumentNullException("name");
            if (name.Length > 32) throw new System.ArgumentOutOfRangeException("name");
            if (publicKey.Length > 8192) throw new System.ArgumentOutOfRangeException("publicKey");
            if (value.Length > 8192) throw new System.ArgumentOutOfRangeException("value");

            this.Name = name;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;
            this.Value = value;

            {
                var __h = new System.HashCode();
                if (this.Name != default) __h.Add(this.Name.GetHashCode());
                if (this.AlgorithmType != default) __h.Add(this.AlgorithmType.GetHashCode());
                if (!this.PublicKey.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.PublicKey.Span));
                if (!this.Value.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.Value.Span));
                __hashCode = __h.ToHashCode();
            }
        }

        public string Name { get; }
        public OmniDigitalSignatureAlgorithmType AlgorithmType { get; }
        public System.ReadOnlyMemory<byte> PublicKey { get; }
        public System.ReadOnlyMemory<byte> Value { get; }

        public override bool Equals(OmniCertificate? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Name != target.Name) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.Value.Span, target.Value.Span)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniCertificate>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, OmniCertificate value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.Name != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.AlgorithmType != (OmniDigitalSignatureAlgorithmType)0)
                    {
                        propertyCount++;
                    }
                    if (!value.PublicKey.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.Value.IsEmpty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.Name != string.Empty)
                {
                    w.Write((uint)0);
                    w.Write(value.Name);
                }
                if (value.AlgorithmType != (OmniDigitalSignatureAlgorithmType)0)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                if (!value.PublicKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PublicKey.Span);
                }
                if (!value.Value.IsEmpty)
                {
                    w.Write((uint)3);
                    w.Write(value.Value.Span);
                }
            }

            public OmniCertificate Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                string p_name = string.Empty;
                OmniDigitalSignatureAlgorithmType p_algorithmType = (OmniDigitalSignatureAlgorithmType)0;
                System.ReadOnlyMemory<byte> p_publicKey = System.ReadOnlyMemory<byte>.Empty;
                System.ReadOnlyMemory<byte> p_value = System.ReadOnlyMemory<byte>.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_name = r.GetString(32);
                                break;
                            }
                        case 1:
                            {
                                p_algorithmType = (OmniDigitalSignatureAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2:
                            {
                                p_publicKey = r.GetMemory(8192);
                                break;
                            }
                        case 3:
                            {
                                p_value = r.GetMemory(8192);
                                break;
                            }
                    }
                }

                return new OmniCertificate(p_name, p_algorithmType, p_publicKey, p_value);
            }
        }
    }

    public sealed partial class OmniSignature : Omnix.Serialization.RocketPack.RocketPackMessageBase<OmniSignature>
    {
        static OmniSignature()
        {
            OmniSignature.Formatter = new CustomFormatter();
            OmniSignature.Empty = new OmniSignature(string.Empty, OmniHash.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxNameLength = 32;

        public OmniSignature(string name, OmniHash hash)
        {
            if (name is null) throw new System.ArgumentNullException("name");
            if (name.Length > 32) throw new System.ArgumentOutOfRangeException("name");
            this.Name = name;
            this.Hash = hash;

            {
                var __h = new System.HashCode();
                if (this.Name != default) __h.Add(this.Name.GetHashCode());
                if (this.Hash != default) __h.Add(this.Hash.GetHashCode());
                __hashCode = __h.ToHashCode();
            }
        }

        public string Name { get; }
        public OmniHash Hash { get; }

        public override bool Equals(OmniSignature? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Name != target.Name) return false;
            if (this.Hash != target.Hash) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniSignature>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, OmniSignature value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.Name != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.Hash != OmniHash.Empty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.Name != string.Empty)
                {
                    w.Write((uint)0);
                    w.Write(value.Name);
                }
                if (value.Hash != OmniHash.Empty)
                {
                    w.Write((uint)1);
                    OmniHash.Formatter.Serialize(w, value.Hash, rank + 1);
                }
            }

            public OmniSignature Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                string p_name = string.Empty;
                OmniHash p_hash = OmniHash.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_name = r.GetString(32);
                                break;
                            }
                        case 1:
                            {
                                p_hash = OmniHash.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new OmniSignature(p_name, p_hash);
            }
        }
    }

    public sealed partial class OmniHashcash : Omnix.Serialization.RocketPack.RocketPackMessageBase<OmniHashcash>
    {
        static OmniHashcash()
        {
            OmniHashcash.Formatter = new CustomFormatter();
            OmniHashcash.Empty = new OmniHashcash((OmniHashcashAlgorithmType)0, System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxKeyLength = 32;

        public OmniHashcash(OmniHashcashAlgorithmType algorithmType, System.ReadOnlyMemory<byte> key)
        {
            if (key.Length > 32) throw new System.ArgumentOutOfRangeException("key");

            this.AlgorithmType = algorithmType;
            this.Key = key;

            {
                var __h = new System.HashCode();
                if (this.AlgorithmType != default) __h.Add(this.AlgorithmType.GetHashCode());
                if (!this.Key.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.Key.Span));
                __hashCode = __h.ToHashCode();
            }
        }

        public OmniHashcashAlgorithmType AlgorithmType { get; }
        public System.ReadOnlyMemory<byte> Key { get; }

        public override bool Equals(OmniHashcash? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.Key.Span, target.Key.Span)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniHashcash>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, OmniHashcash value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.AlgorithmType != (OmniHashcashAlgorithmType)0)
                    {
                        propertyCount++;
                    }
                    if (!value.Key.IsEmpty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.AlgorithmType != (OmniHashcashAlgorithmType)0)
                {
                    w.Write((uint)0);
                    w.Write((ulong)value.AlgorithmType);
                }
                if (!value.Key.IsEmpty)
                {
                    w.Write((uint)1);
                    w.Write(value.Key.Span);
                }
            }

            public OmniHashcash Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                OmniHashcashAlgorithmType p_algorithmType = (OmniHashcashAlgorithmType)0;
                System.ReadOnlyMemory<byte> p_key = System.ReadOnlyMemory<byte>.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_algorithmType = (OmniHashcashAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 1:
                            {
                                p_key = r.GetMemory(32);
                                break;
                            }
                    }
                }

                return new OmniHashcash(p_algorithmType, p_key);
            }
        }
    }

}
