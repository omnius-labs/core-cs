
#nullable enable

namespace Omnix.Algorithms.Cryptography
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

    public readonly struct OmniHash : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniHash>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniHash> Formatter { get; }
        public static OmniHash Empty { get; }

        static OmniHash()
        {
            OmniHash.Formatter = new ___CustomFormatter();
            OmniHash.Empty = new OmniHash((OmniHashAlgorithmType)0, global::System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly int ___hashCode;

        public static readonly int MaxValueLength = 256;

        public OmniHash(OmniHashAlgorithmType algorithmType, global::System.ReadOnlyMemory<byte> value)
        {
            if (value.Length > 256) throw new global::System.ArgumentOutOfRangeException("value");

            this.AlgorithmType = algorithmType;
            this.Value = value;

            {
                var ___h = new global::System.HashCode();
                if (algorithmType != default) ___h.Add(algorithmType.GetHashCode());
                if (!value.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(value.Span));
                ___hashCode = ___h.ToHashCode();
            }
        }

        public OmniHashAlgorithmType AlgorithmType { get; }
        public global::System.ReadOnlyMemory<byte> Value { get; }

        public static OmniHash Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniHash left, OmniHash right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(OmniHash left, OmniHash right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniHash)) return false;
            return this.Equals((OmniHash)other);
        }
        public bool Equals(OmniHash target)
        {
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.Value.Span, target.Value.Span)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniHash>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniHash value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write((ulong)value.AlgorithmType);
                w.Write(value.Value.Span);
            }

            public OmniHash Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                OmniHashAlgorithmType p_algorithmType = (OmniHashAlgorithmType)0;
                global::System.ReadOnlyMemory<byte> p_value = global::System.ReadOnlyMemory<byte>.Empty;

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

    public sealed partial class OmniAgreement : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniAgreement>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAgreement> Formatter { get; }
        public static OmniAgreement Empty { get; }

        static OmniAgreement()
        {
            OmniAgreement.Formatter = new ___CustomFormatter();
            OmniAgreement.Empty = new OmniAgreement(global::Omnix.Serialization.RocketPack.Timestamp.Zero, (OmniAgreementAlgorithmType)0, global::System.ReadOnlyMemory<byte>.Empty, global::System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxPublicKeyLength = 8192;
        public static readonly int MaxPrivateKeyLength = 8192;

        public OmniAgreement(global::Omnix.Serialization.RocketPack.Timestamp creationTime, OmniAgreementAlgorithmType algorithmType, global::System.ReadOnlyMemory<byte> publicKey, global::System.ReadOnlyMemory<byte> privateKey)
        {
            if (publicKey.Length > 8192) throw new global::System.ArgumentOutOfRangeException("publicKey");
            if (privateKey.Length > 8192) throw new global::System.ArgumentOutOfRangeException("privateKey");

            this.CreationTime = creationTime;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (creationTime != default) ___h.Add(creationTime.GetHashCode());
                if (algorithmType != default) ___h.Add(algorithmType.GetHashCode());
                if (!publicKey.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(publicKey.Span));
                if (!privateKey.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(privateKey.Span));
                return ___h.ToHashCode();
            });
        }

        public global::Omnix.Serialization.RocketPack.Timestamp CreationTime { get; }
        public OmniAgreementAlgorithmType AlgorithmType { get; }
        public global::System.ReadOnlyMemory<byte> PublicKey { get; }
        public global::System.ReadOnlyMemory<byte> PrivateKey { get; }

        public static OmniAgreement Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniAgreement? left, OmniAgreement? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniAgreement? left, OmniAgreement? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniAgreement)) return false;
            return this.Equals((OmniAgreement)other);
        }
        public bool Equals(OmniAgreement? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.PrivateKey.Span, target.PrivateKey.Span)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAgreement>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniAgreement value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.CreationTime != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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

                if (value.CreationTime != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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

            public OmniAgreement Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                global::Omnix.Serialization.RocketPack.Timestamp p_creationTime = global::Omnix.Serialization.RocketPack.Timestamp.Zero;
                OmniAgreementAlgorithmType p_algorithmType = (OmniAgreementAlgorithmType)0;
                global::System.ReadOnlyMemory<byte> p_publicKey = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.ReadOnlyMemory<byte> p_privateKey = global::System.ReadOnlyMemory<byte>.Empty;

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

    public sealed partial class OmniAgreementPublicKey : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniAgreementPublicKey>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAgreementPublicKey> Formatter { get; }
        public static OmniAgreementPublicKey Empty { get; }

        static OmniAgreementPublicKey()
        {
            OmniAgreementPublicKey.Formatter = new ___CustomFormatter();
            OmniAgreementPublicKey.Empty = new OmniAgreementPublicKey(global::Omnix.Serialization.RocketPack.Timestamp.Zero, (OmniAgreementAlgorithmType)0, global::System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxPublicKeyLength = 8192;

        public OmniAgreementPublicKey(global::Omnix.Serialization.RocketPack.Timestamp creationTime, OmniAgreementAlgorithmType algorithmType, global::System.ReadOnlyMemory<byte> publicKey)
        {
            if (publicKey.Length > 8192) throw new global::System.ArgumentOutOfRangeException("publicKey");

            this.CreationTime = creationTime;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (creationTime != default) ___h.Add(creationTime.GetHashCode());
                if (algorithmType != default) ___h.Add(algorithmType.GetHashCode());
                if (!publicKey.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(publicKey.Span));
                return ___h.ToHashCode();
            });
        }

        public global::Omnix.Serialization.RocketPack.Timestamp CreationTime { get; }
        public OmniAgreementAlgorithmType AlgorithmType { get; }
        public global::System.ReadOnlyMemory<byte> PublicKey { get; }

        public static OmniAgreementPublicKey Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniAgreementPublicKey? left, OmniAgreementPublicKey? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniAgreementPublicKey? left, OmniAgreementPublicKey? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniAgreementPublicKey)) return false;
            return this.Equals((OmniAgreementPublicKey)other);
        }
        public bool Equals(OmniAgreementPublicKey? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAgreementPublicKey>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniAgreementPublicKey value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.CreationTime != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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

                if (value.CreationTime != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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

            public OmniAgreementPublicKey Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                global::Omnix.Serialization.RocketPack.Timestamp p_creationTime = global::Omnix.Serialization.RocketPack.Timestamp.Zero;
                OmniAgreementAlgorithmType p_algorithmType = (OmniAgreementAlgorithmType)0;
                global::System.ReadOnlyMemory<byte> p_publicKey = global::System.ReadOnlyMemory<byte>.Empty;

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

    public sealed partial class OmniAgreementPrivateKey : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniAgreementPrivateKey>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAgreementPrivateKey> Formatter { get; }
        public static OmniAgreementPrivateKey Empty { get; }

        static OmniAgreementPrivateKey()
        {
            OmniAgreementPrivateKey.Formatter = new ___CustomFormatter();
            OmniAgreementPrivateKey.Empty = new OmniAgreementPrivateKey(global::Omnix.Serialization.RocketPack.Timestamp.Zero, (OmniAgreementAlgorithmType)0, global::System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxPrivateKeyLength = 8192;

        public OmniAgreementPrivateKey(global::Omnix.Serialization.RocketPack.Timestamp creationTime, OmniAgreementAlgorithmType algorithmType, global::System.ReadOnlyMemory<byte> privateKey)
        {
            if (privateKey.Length > 8192) throw new global::System.ArgumentOutOfRangeException("privateKey");

            this.CreationTime = creationTime;
            this.AlgorithmType = algorithmType;
            this.PrivateKey = privateKey;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (creationTime != default) ___h.Add(creationTime.GetHashCode());
                if (algorithmType != default) ___h.Add(algorithmType.GetHashCode());
                if (!privateKey.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(privateKey.Span));
                return ___h.ToHashCode();
            });
        }

        public global::Omnix.Serialization.RocketPack.Timestamp CreationTime { get; }
        public OmniAgreementAlgorithmType AlgorithmType { get; }
        public global::System.ReadOnlyMemory<byte> PrivateKey { get; }

        public static OmniAgreementPrivateKey Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniAgreementPrivateKey? left, OmniAgreementPrivateKey? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniAgreementPrivateKey? left, OmniAgreementPrivateKey? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniAgreementPrivateKey)) return false;
            return this.Equals((OmniAgreementPrivateKey)other);
        }
        public bool Equals(OmniAgreementPrivateKey? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.PrivateKey.Span, target.PrivateKey.Span)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniAgreementPrivateKey>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniAgreementPrivateKey value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.CreationTime != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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

                if (value.CreationTime != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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

            public OmniAgreementPrivateKey Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                global::Omnix.Serialization.RocketPack.Timestamp p_creationTime = global::Omnix.Serialization.RocketPack.Timestamp.Zero;
                OmniAgreementAlgorithmType p_algorithmType = (OmniAgreementAlgorithmType)0;
                global::System.ReadOnlyMemory<byte> p_privateKey = global::System.ReadOnlyMemory<byte>.Empty;

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

    public sealed partial class OmniDigitalSignature : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniDigitalSignature>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniDigitalSignature> Formatter { get; }
        public static OmniDigitalSignature Empty { get; }

        static OmniDigitalSignature()
        {
            OmniDigitalSignature.Formatter = new ___CustomFormatter();
            OmniDigitalSignature.Empty = new OmniDigitalSignature(string.Empty, (OmniDigitalSignatureAlgorithmType)0, global::System.ReadOnlyMemory<byte>.Empty, global::System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxNameLength = 32;
        public static readonly int MaxPublicKeyLength = 8192;
        public static readonly int MaxPrivateKeyLength = 8192;

        public OmniDigitalSignature(string name, OmniDigitalSignatureAlgorithmType algorithmType, global::System.ReadOnlyMemory<byte> publicKey, global::System.ReadOnlyMemory<byte> privateKey)
        {
            if (name is null) throw new global::System.ArgumentNullException("name");
            if (name.Length > 32) throw new global::System.ArgumentOutOfRangeException("name");
            if (publicKey.Length > 8192) throw new global::System.ArgumentOutOfRangeException("publicKey");
            if (privateKey.Length > 8192) throw new global::System.ArgumentOutOfRangeException("privateKey");

            this.Name = name;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (name != default) ___h.Add(name.GetHashCode());
                if (algorithmType != default) ___h.Add(algorithmType.GetHashCode());
                if (!publicKey.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(publicKey.Span));
                if (!privateKey.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(privateKey.Span));
                return ___h.ToHashCode();
            });
        }

        public string Name { get; }
        public OmniDigitalSignatureAlgorithmType AlgorithmType { get; }
        public global::System.ReadOnlyMemory<byte> PublicKey { get; }
        public global::System.ReadOnlyMemory<byte> PrivateKey { get; }

        public static OmniDigitalSignature Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniDigitalSignature? left, OmniDigitalSignature? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniDigitalSignature? left, OmniDigitalSignature? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniDigitalSignature)) return false;
            return this.Equals((OmniDigitalSignature)other);
        }
        public bool Equals(OmniDigitalSignature? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Name != target.Name) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.PrivateKey.Span, target.PrivateKey.Span)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniDigitalSignature>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniDigitalSignature value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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

            public OmniDigitalSignature Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                string p_name = string.Empty;
                OmniDigitalSignatureAlgorithmType p_algorithmType = (OmniDigitalSignatureAlgorithmType)0;
                global::System.ReadOnlyMemory<byte> p_publicKey = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.ReadOnlyMemory<byte> p_privateKey = global::System.ReadOnlyMemory<byte>.Empty;

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

    public sealed partial class OmniCertificate : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniCertificate>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniCertificate> Formatter { get; }
        public static OmniCertificate Empty { get; }

        static OmniCertificate()
        {
            OmniCertificate.Formatter = new ___CustomFormatter();
            OmniCertificate.Empty = new OmniCertificate(string.Empty, (OmniDigitalSignatureAlgorithmType)0, global::System.ReadOnlyMemory<byte>.Empty, global::System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxNameLength = 32;
        public static readonly int MaxPublicKeyLength = 8192;
        public static readonly int MaxValueLength = 8192;

        public OmniCertificate(string name, OmniDigitalSignatureAlgorithmType algorithmType, global::System.ReadOnlyMemory<byte> publicKey, global::System.ReadOnlyMemory<byte> value)
        {
            if (name is null) throw new global::System.ArgumentNullException("name");
            if (name.Length > 32) throw new global::System.ArgumentOutOfRangeException("name");
            if (publicKey.Length > 8192) throw new global::System.ArgumentOutOfRangeException("publicKey");
            if (value.Length > 8192) throw new global::System.ArgumentOutOfRangeException("value");

            this.Name = name;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;
            this.Value = value;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (name != default) ___h.Add(name.GetHashCode());
                if (algorithmType != default) ___h.Add(algorithmType.GetHashCode());
                if (!publicKey.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(publicKey.Span));
                if (!value.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(value.Span));
                return ___h.ToHashCode();
            });
        }

        public string Name { get; }
        public OmniDigitalSignatureAlgorithmType AlgorithmType { get; }
        public global::System.ReadOnlyMemory<byte> PublicKey { get; }
        public global::System.ReadOnlyMemory<byte> Value { get; }

        public static OmniCertificate Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniCertificate? left, OmniCertificate? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniCertificate? left, OmniCertificate? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniCertificate)) return false;
            return this.Equals((OmniCertificate)other);
        }
        public bool Equals(OmniCertificate? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Name != target.Name) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.Value.Span, target.Value.Span)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniCertificate>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniCertificate value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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

            public OmniCertificate Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                string p_name = string.Empty;
                OmniDigitalSignatureAlgorithmType p_algorithmType = (OmniDigitalSignatureAlgorithmType)0;
                global::System.ReadOnlyMemory<byte> p_publicKey = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.ReadOnlyMemory<byte> p_value = global::System.ReadOnlyMemory<byte>.Empty;

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

    public sealed partial class OmniSignature : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniSignature>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniSignature> Formatter { get; }
        public static OmniSignature Empty { get; }

        static OmniSignature()
        {
            OmniSignature.Formatter = new ___CustomFormatter();
            OmniSignature.Empty = new OmniSignature(string.Empty, OmniHash.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxNameLength = 32;

        public OmniSignature(string name, OmniHash hash)
        {
            if (name is null) throw new global::System.ArgumentNullException("name");
            if (name.Length > 32) throw new global::System.ArgumentOutOfRangeException("name");
            this.Name = name;
            this.Hash = hash;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (name != default) ___h.Add(name.GetHashCode());
                if (hash != default) ___h.Add(hash.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public string Name { get; }
        public OmniHash Hash { get; }

        public static OmniSignature Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniSignature? left, OmniSignature? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniSignature? left, OmniSignature? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniSignature)) return false;
            return this.Equals((OmniSignature)other);
        }
        public bool Equals(OmniSignature? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Name != target.Name) return false;
            if (this.Hash != target.Hash) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniSignature>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniSignature value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                    OmniHash.Formatter.Serialize(ref w, value.Hash, rank + 1);
                }
            }

            public OmniSignature Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                                p_hash = OmniHash.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                    }
                }

                return new OmniSignature(p_name, p_hash);
            }
        }
    }

    public sealed partial class OmniHashcash : global::Omnix.Serialization.RocketPack.IRocketPackMessage<OmniHashcash>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniHashcash> Formatter { get; }
        public static OmniHashcash Empty { get; }

        static OmniHashcash()
        {
            OmniHashcash.Formatter = new ___CustomFormatter();
            OmniHashcash.Empty = new OmniHashcash((OmniHashcashAlgorithmType)0, global::System.ReadOnlyMemory<byte>.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxKeyLength = 32;

        public OmniHashcash(OmniHashcashAlgorithmType algorithmType, global::System.ReadOnlyMemory<byte> key)
        {
            if (key.Length > 32) throw new global::System.ArgumentOutOfRangeException("key");

            this.AlgorithmType = algorithmType;
            this.Key = key;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (algorithmType != default) ___h.Add(algorithmType.GetHashCode());
                if (!key.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(key.Span));
                return ___h.ToHashCode();
            });
        }

        public OmniHashcashAlgorithmType AlgorithmType { get; }
        public global::System.ReadOnlyMemory<byte> Key { get; }

        public static OmniHashcash Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniHashcash? left, OmniHashcash? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniHashcash? left, OmniHashcash? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniHashcash)) return false;
            return this.Equals((OmniHashcash)other);
        }
        public bool Equals(OmniHashcash? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.Key.Span, target.Key.Span)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniHashcash>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in OmniHashcash value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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

            public OmniHashcash Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                OmniHashcashAlgorithmType p_algorithmType = (OmniHashcashAlgorithmType)0;
                global::System.ReadOnlyMemory<byte> p_key = global::System.ReadOnlyMemory<byte>.Empty;

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
