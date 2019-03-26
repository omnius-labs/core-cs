using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Collections;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        Sha2_256 = 0,
    }

    public readonly struct OmniHash
    {
        public static IRocketPackFormatter<OmniHash> Formatter { get; }

        static OmniHash()
        {
            OmniHash.Formatter = new CustomFormatter();
        }

        public static readonly int MaxValueLength = 256;

        public OmniHash(OmniHashAlgorithmType algorithmType, ReadOnlyMemory<byte> value)
        {
            if (value.Length > 256) throw new ArgumentOutOfRangeException("value");

            this.AlgorithmType = algorithmType;
            this.Value = value;

            {
                var hashCode = new HashCode();
                if (this.AlgorithmType != default) hashCode.Add(this.AlgorithmType.GetHashCode());
                if (!this.Value.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.Value.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public static OmniHash Import(ReadOnlySequence<byte> sequence, BufferPool bufferPool)
        {
            return Formatter.Deserialize(new RocketPackReader(sequence, bufferPool), 0);
        }

        public void Export(IBufferWriter<byte> bufferWriter, BufferPool bufferPool)
        {
            Formatter.Serialize(new RocketPackWriter(bufferWriter, bufferPool), (OmniHash)this, 0);
        }

        public OmniHashAlgorithmType AlgorithmType { get; }
        public ReadOnlyMemory<byte> Value { get; }

        public static bool operator ==(OmniHash x, OmniHash y) => x.Equals(y);
        public static bool operator !=(OmniHash x, OmniHash y) => !x.Equals(y);

        public override bool Equals(object other)
        {
            if (!(other is OmniHash)) return false;
            return this.Equals((OmniHash)other);
        }

        public bool Equals(OmniHash target)
        {
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!BytesOperations.SequenceEqual(this.Value.Span, target.Value.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniHash>
        {
            public void Serialize(RocketPackWriter w, OmniHash value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // AlgorithmType
                w.Write((ulong)value.AlgorithmType);
                // Value
                w.Write(value.Value.Span);
            }

            public OmniHash Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                OmniHashAlgorithmType p_algorithmType = default;
                ReadOnlyMemory<byte> p_value = default;

                // AlgorithmType
                {
                    p_algorithmType = (OmniHashAlgorithmType)r.GetUInt64();
                }
                // Value
                {
                    p_value = r.GetMemory(256);
                }

                return new OmniHash(p_algorithmType, p_value);
            }
        }
    }

    public sealed partial class OmniAgreement : RocketPackMessageBase<OmniAgreement>
    {
        static OmniAgreement()
        {
            OmniAgreement.Formatter = new CustomFormatter();
        }

        public static readonly int MaxPublicKeyLength = 8192;
        public static readonly int MaxPrivateKeyLength = 8192;

        public OmniAgreement(Timestamp creationTime, OmniAgreementAlgorithmType algorithmType, ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> privateKey)
        {
            if (publicKey.Length > 8192) throw new ArgumentOutOfRangeException("publicKey");
            if (privateKey.Length > 8192) throw new ArgumentOutOfRangeException("privateKey");

            this.CreationTime = creationTime;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;

            {
                var hashCode = new HashCode();
                if (this.CreationTime != default) hashCode.Add(this.CreationTime.GetHashCode());
                if (this.AlgorithmType != default) hashCode.Add(this.AlgorithmType.GetHashCode());
                if (!this.PublicKey.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.PublicKey.Span));
                if (!this.PrivateKey.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.PrivateKey.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public Timestamp CreationTime { get; }
        public OmniAgreementAlgorithmType AlgorithmType { get; }
        public ReadOnlyMemory<byte> PublicKey { get; }
        public ReadOnlyMemory<byte> PrivateKey { get; }

        public override bool Equals(OmniAgreement target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;
            if (!BytesOperations.SequenceEqual(this.PrivateKey.Span, target.PrivateKey.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniAgreement>
        {
            public void Serialize(RocketPackWriter w, OmniAgreement value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.CreationTime != default) propertyCount++;
                    if (value.AlgorithmType != default) propertyCount++;
                    if (!value.PublicKey.IsEmpty) propertyCount++;
                    if (!value.PrivateKey.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // CreationTime
                if (value.CreationTime != default)
                {
                    w.Write((uint)0);
                    w.Write(value.CreationTime);
                }
                // AlgorithmType
                if (value.AlgorithmType != default)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                // PublicKey
                if (!value.PublicKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PublicKey.Span);
                }
                // PrivateKey
                if (!value.PrivateKey.IsEmpty)
                {
                    w.Write((uint)3);
                    w.Write(value.PrivateKey.Span);
                }
            }

            public OmniAgreement Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                Timestamp p_creationTime = default;
                OmniAgreementAlgorithmType p_algorithmType = default;
                ReadOnlyMemory<byte> p_publicKey = default;
                ReadOnlyMemory<byte> p_privateKey = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // CreationTime
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 1: // AlgorithmType
                            {
                                p_algorithmType = (OmniAgreementAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2: // PublicKey
                            {
                                p_publicKey = r.GetMemory(8192);
                                break;
                            }
                        case 3: // PrivateKey
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

    public sealed partial class OmniAgreementPublicKey : RocketPackMessageBase<OmniAgreementPublicKey>
    {
        static OmniAgreementPublicKey()
        {
            OmniAgreementPublicKey.Formatter = new CustomFormatter();
        }

        public static readonly int MaxPublicKeyLength = 8192;

        public OmniAgreementPublicKey(Timestamp creationTime, OmniAgreementAlgorithmType algorithmType, ReadOnlyMemory<byte> publicKey)
        {
            if (publicKey.Length > 8192) throw new ArgumentOutOfRangeException("publicKey");

            this.CreationTime = creationTime;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;

            {
                var hashCode = new HashCode();
                if (this.CreationTime != default) hashCode.Add(this.CreationTime.GetHashCode());
                if (this.AlgorithmType != default) hashCode.Add(this.AlgorithmType.GetHashCode());
                if (!this.PublicKey.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.PublicKey.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public Timestamp CreationTime { get; }
        public OmniAgreementAlgorithmType AlgorithmType { get; }
        public ReadOnlyMemory<byte> PublicKey { get; }

        public override bool Equals(OmniAgreementPublicKey target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniAgreementPublicKey>
        {
            public void Serialize(RocketPackWriter w, OmniAgreementPublicKey value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.CreationTime != default) propertyCount++;
                    if (value.AlgorithmType != default) propertyCount++;
                    if (!value.PublicKey.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // CreationTime
                if (value.CreationTime != default)
                {
                    w.Write((uint)0);
                    w.Write(value.CreationTime);
                }
                // AlgorithmType
                if (value.AlgorithmType != default)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                // PublicKey
                if (!value.PublicKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PublicKey.Span);
                }
            }

            public OmniAgreementPublicKey Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                Timestamp p_creationTime = default;
                OmniAgreementAlgorithmType p_algorithmType = default;
                ReadOnlyMemory<byte> p_publicKey = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // CreationTime
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 1: // AlgorithmType
                            {
                                p_algorithmType = (OmniAgreementAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2: // PublicKey
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

    public sealed partial class OmniAgreementPrivateKey : RocketPackMessageBase<OmniAgreementPrivateKey>
    {
        static OmniAgreementPrivateKey()
        {
            OmniAgreementPrivateKey.Formatter = new CustomFormatter();
        }

        public static readonly int MaxPrivateKeyLength = 8192;

        public OmniAgreementPrivateKey(Timestamp creationTime, OmniAgreementAlgorithmType algorithmType, ReadOnlyMemory<byte> privateKey)
        {
            if (privateKey.Length > 8192) throw new ArgumentOutOfRangeException("privateKey");

            this.CreationTime = creationTime;
            this.AlgorithmType = algorithmType;
            this.PrivateKey = privateKey;

            {
                var hashCode = new HashCode();
                if (this.CreationTime != default) hashCode.Add(this.CreationTime.GetHashCode());
                if (this.AlgorithmType != default) hashCode.Add(this.AlgorithmType.GetHashCode());
                if (!this.PrivateKey.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.PrivateKey.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public Timestamp CreationTime { get; }
        public OmniAgreementAlgorithmType AlgorithmType { get; }
        public ReadOnlyMemory<byte> PrivateKey { get; }

        public override bool Equals(OmniAgreementPrivateKey target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!BytesOperations.SequenceEqual(this.PrivateKey.Span, target.PrivateKey.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniAgreementPrivateKey>
        {
            public void Serialize(RocketPackWriter w, OmniAgreementPrivateKey value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.CreationTime != default) propertyCount++;
                    if (value.AlgorithmType != default) propertyCount++;
                    if (!value.PrivateKey.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // CreationTime
                if (value.CreationTime != default)
                {
                    w.Write((uint)0);
                    w.Write(value.CreationTime);
                }
                // AlgorithmType
                if (value.AlgorithmType != default)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                // PrivateKey
                if (!value.PrivateKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PrivateKey.Span);
                }
            }

            public OmniAgreementPrivateKey Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                Timestamp p_creationTime = default;
                OmniAgreementAlgorithmType p_algorithmType = default;
                ReadOnlyMemory<byte> p_privateKey = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // CreationTime
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 1: // AlgorithmType
                            {
                                p_algorithmType = (OmniAgreementAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2: // PrivateKey
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

    public sealed partial class OmniDigitalSignature : RocketPackMessageBase<OmniDigitalSignature>
    {
        static OmniDigitalSignature()
        {
            OmniDigitalSignature.Formatter = new CustomFormatter();
        }

        public static readonly int MaxNameLength = 32;
        public static readonly int MaxPublicKeyLength = 8192;
        public static readonly int MaxPrivateKeyLength = 8192;

        public OmniDigitalSignature(string name, OmniDigitalSignatureAlgorithmType algorithmType, ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> privateKey)
        {
            if (name is null) throw new ArgumentNullException("name");
            if (name.Length > 32) throw new ArgumentOutOfRangeException("name");
            if (publicKey.Length > 8192) throw new ArgumentOutOfRangeException("publicKey");
            if (privateKey.Length > 8192) throw new ArgumentOutOfRangeException("privateKey");

            this.Name = name;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;

            {
                var hashCode = new HashCode();
                if (this.Name != default) hashCode.Add(this.Name.GetHashCode());
                if (this.AlgorithmType != default) hashCode.Add(this.AlgorithmType.GetHashCode());
                if (!this.PublicKey.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.PublicKey.Span));
                if (!this.PrivateKey.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.PrivateKey.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public string Name { get; }
        public OmniDigitalSignatureAlgorithmType AlgorithmType { get; }
        public ReadOnlyMemory<byte> PublicKey { get; }
        public ReadOnlyMemory<byte> PrivateKey { get; }

        public override bool Equals(OmniDigitalSignature target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Name != target.Name) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;
            if (!BytesOperations.SequenceEqual(this.PrivateKey.Span, target.PrivateKey.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniDigitalSignature>
        {
            public void Serialize(RocketPackWriter w, OmniDigitalSignature value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Name != default) propertyCount++;
                    if (value.AlgorithmType != default) propertyCount++;
                    if (!value.PublicKey.IsEmpty) propertyCount++;
                    if (!value.PrivateKey.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // Name
                if (value.Name != default)
                {
                    w.Write((uint)0);
                    w.Write(value.Name);
                }
                // AlgorithmType
                if (value.AlgorithmType != default)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                // PublicKey
                if (!value.PublicKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PublicKey.Span);
                }
                // PrivateKey
                if (!value.PrivateKey.IsEmpty)
                {
                    w.Write((uint)3);
                    w.Write(value.PrivateKey.Span);
                }
            }

            public OmniDigitalSignature Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                string p_name = default;
                OmniDigitalSignatureAlgorithmType p_algorithmType = default;
                ReadOnlyMemory<byte> p_publicKey = default;
                ReadOnlyMemory<byte> p_privateKey = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Name
                            {
                                p_name = r.GetString(32);
                                break;
                            }
                        case 1: // AlgorithmType
                            {
                                p_algorithmType = (OmniDigitalSignatureAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2: // PublicKey
                            {
                                p_publicKey = r.GetMemory(8192);
                                break;
                            }
                        case 3: // PrivateKey
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

    public sealed partial class OmniCertificate : RocketPackMessageBase<OmniCertificate>
    {
        static OmniCertificate()
        {
            OmniCertificate.Formatter = new CustomFormatter();
        }

        public static readonly int MaxNameLength = 32;
        public static readonly int MaxPublicKeyLength = 8192;
        public static readonly int MaxValueLength = 8192;

        public OmniCertificate(string name, OmniDigitalSignatureAlgorithmType algorithmType, ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> value)
        {
            if (name is null) throw new ArgumentNullException("name");
            if (name.Length > 32) throw new ArgumentOutOfRangeException("name");
            if (publicKey.Length > 8192) throw new ArgumentOutOfRangeException("publicKey");
            if (value.Length > 8192) throw new ArgumentOutOfRangeException("value");

            this.Name = name;
            this.AlgorithmType = algorithmType;
            this.PublicKey = publicKey;
            this.Value = value;

            {
                var hashCode = new HashCode();
                if (this.Name != default) hashCode.Add(this.Name.GetHashCode());
                if (this.AlgorithmType != default) hashCode.Add(this.AlgorithmType.GetHashCode());
                if (!this.PublicKey.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.PublicKey.Span));
                if (!this.Value.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.Value.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public string Name { get; }
        public OmniDigitalSignatureAlgorithmType AlgorithmType { get; }
        public ReadOnlyMemory<byte> PublicKey { get; }
        public ReadOnlyMemory<byte> Value { get; }

        public override bool Equals(OmniCertificate target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Name != target.Name) return false;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!BytesOperations.SequenceEqual(this.PublicKey.Span, target.PublicKey.Span)) return false;
            if (!BytesOperations.SequenceEqual(this.Value.Span, target.Value.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniCertificate>
        {
            public void Serialize(RocketPackWriter w, OmniCertificate value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Name != default) propertyCount++;
                    if (value.AlgorithmType != default) propertyCount++;
                    if (!value.PublicKey.IsEmpty) propertyCount++;
                    if (!value.Value.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // Name
                if (value.Name != default)
                {
                    w.Write((uint)0);
                    w.Write(value.Name);
                }
                // AlgorithmType
                if (value.AlgorithmType != default)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AlgorithmType);
                }
                // PublicKey
                if (!value.PublicKey.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.PublicKey.Span);
                }
                // Value
                if (!value.Value.IsEmpty)
                {
                    w.Write((uint)3);
                    w.Write(value.Value.Span);
                }
            }

            public OmniCertificate Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                string p_name = default;
                OmniDigitalSignatureAlgorithmType p_algorithmType = default;
                ReadOnlyMemory<byte> p_publicKey = default;
                ReadOnlyMemory<byte> p_value = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Name
                            {
                                p_name = r.GetString(32);
                                break;
                            }
                        case 1: // AlgorithmType
                            {
                                p_algorithmType = (OmniDigitalSignatureAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 2: // PublicKey
                            {
                                p_publicKey = r.GetMemory(8192);
                                break;
                            }
                        case 3: // Value
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

    public sealed partial class OmniSignature : RocketPackMessageBase<OmniSignature>
    {
        static OmniSignature()
        {
            OmniSignature.Formatter = new CustomFormatter();
        }

        public static readonly int MaxNameLength = 32;

        public OmniSignature(string name, OmniHash hash)
        {
            if (name is null) throw new ArgumentNullException("name");
            if (name.Length > 32) throw new ArgumentOutOfRangeException("name");
            this.Name = name;
            this.Hash = hash;

            {
                var hashCode = new HashCode();
                if (this.Name != default) hashCode.Add(this.Name.GetHashCode());
                if (this.Hash != default) hashCode.Add(this.Hash.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public string Name { get; }
        public OmniHash Hash { get; }

        public override bool Equals(OmniSignature target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Name != target.Name) return false;
            if (this.Hash != target.Hash) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniSignature>
        {
            public void Serialize(RocketPackWriter w, OmniSignature value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Name != default) propertyCount++;
                    if (value.Hash != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // Name
                if (value.Name != default)
                {
                    w.Write((uint)0);
                    w.Write(value.Name);
                }
                // Hash
                if (value.Hash != default)
                {
                    w.Write((uint)1);
                    OmniHash.Formatter.Serialize(w, value.Hash, rank + 1);
                }
            }

            public OmniSignature Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                string p_name = default;
                OmniHash p_hash = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Name
                            {
                                p_name = r.GetString(32);
                                break;
                            }
                        case 1: // Hash
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

    public sealed partial class OmniHashcash : RocketPackMessageBase<OmniHashcash>
    {
        static OmniHashcash()
        {
            OmniHashcash.Formatter = new CustomFormatter();
        }

        public static readonly int MaxKeyLength = 32;

        public OmniHashcash(OmniHashcashAlgorithmType algorithmType, ReadOnlyMemory<byte> key)
        {
            if (key.Length > 32) throw new ArgumentOutOfRangeException("key");

            this.AlgorithmType = algorithmType;
            this.Key = key;

            {
                var hashCode = new HashCode();
                if (this.AlgorithmType != default) hashCode.Add(this.AlgorithmType.GetHashCode());
                if (!this.Key.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.Key.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public OmniHashcashAlgorithmType AlgorithmType { get; }
        public ReadOnlyMemory<byte> Key { get; }

        public override bool Equals(OmniHashcash target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.AlgorithmType != target.AlgorithmType) return false;
            if (!BytesOperations.SequenceEqual(this.Key.Span, target.Key.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniHashcash>
        {
            public void Serialize(RocketPackWriter w, OmniHashcash value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.AlgorithmType != default) propertyCount++;
                    if (!value.Key.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // AlgorithmType
                if (value.AlgorithmType != default)
                {
                    w.Write((uint)0);
                    w.Write((ulong)value.AlgorithmType);
                }
                // Key
                if (!value.Key.IsEmpty)
                {
                    w.Write((uint)1);
                    w.Write(value.Key.Span);
                }
            }

            public OmniHashcash Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                OmniHashcashAlgorithmType p_algorithmType = default;
                ReadOnlyMemory<byte> p_key = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // AlgorithmType
                            {
                                p_algorithmType = (OmniHashcashAlgorithmType)r.GetUInt64();
                                break;
                            }
                        case 1: // Key
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
