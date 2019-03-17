using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Cryptography;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Network.Connection.Secure.V1.Internal
{
    internal enum KeyExchangeAlgorithm : byte
    {
        EcDh_P521_Sha2_256 = 0,
    }

    internal enum KeyDerivationAlgorithm : byte
    {
        Pbkdf2 = 0,
    }

    internal enum HashAlgorithm : byte
    {
        Sha2_256 = 0,
    }

    internal enum CryptoAlgorithm : byte
    {
        Aes_256 = 0,
    }

    internal enum AuthenticationType : byte
    {
        None = 0,
        Password = 1,
    }

    internal sealed partial class ProfileMessage : RocketPackMessageBase<ProfileMessage>
    {
        static ProfileMessage()
        {
            ProfileMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxSessionIdLength = 32;
        public static readonly int MaxKeyExchangeAlgorithmsCount = 32;
        public static readonly int MaxKeyDerivationAlgorithmsCount = 32;
        public static readonly int MaxCryptoAlgorithmsCount = 32;
        public static readonly int MaxHashAlgorithmsCount = 32;

        public ProfileMessage(ReadOnlyMemory<byte> sessionId, AuthenticationType authenticationType, IList<KeyExchangeAlgorithm> keyExchangeAlgorithms, IList<KeyDerivationAlgorithm> keyDerivationAlgorithms, IList<CryptoAlgorithm> cryptoAlgorithms, IList<HashAlgorithm> hashAlgorithms)
        {
            if (sessionId.Length > 32) throw new ArgumentOutOfRangeException("sessionId");
            if (keyExchangeAlgorithms is null) throw new ArgumentNullException("keyExchangeAlgorithms");
            if (keyExchangeAlgorithms.Count > 32) throw new ArgumentOutOfRangeException("keyExchangeAlgorithms");
            if (keyDerivationAlgorithms is null) throw new ArgumentNullException("keyDerivationAlgorithms");
            if (keyDerivationAlgorithms.Count > 32) throw new ArgumentOutOfRangeException("keyDerivationAlgorithms");
            if (cryptoAlgorithms is null) throw new ArgumentNullException("cryptoAlgorithms");
            if (cryptoAlgorithms.Count > 32) throw new ArgumentOutOfRangeException("cryptoAlgorithms");
            if (hashAlgorithms is null) throw new ArgumentNullException("hashAlgorithms");
            if (hashAlgorithms.Count > 32) throw new ArgumentOutOfRangeException("hashAlgorithms");

            this.SessionId = sessionId;
            this.AuthenticationType = authenticationType;
            this.KeyExchangeAlgorithms = new ReadOnlyCollection<KeyExchangeAlgorithm>(keyExchangeAlgorithms);
            this.KeyDerivationAlgorithms = new ReadOnlyCollection<KeyDerivationAlgorithm>(keyDerivationAlgorithms);
            this.CryptoAlgorithms = new ReadOnlyCollection<CryptoAlgorithm>(cryptoAlgorithms);
            this.HashAlgorithms = new ReadOnlyCollection<HashAlgorithm>(hashAlgorithms);

            {
                var hashCode = new HashCode();
                if (!this.SessionId.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.SessionId.Span));
                if (this.AuthenticationType != default) hashCode.Add(this.AuthenticationType.GetHashCode());
                foreach (var n in this.KeyExchangeAlgorithms)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                foreach (var n in this.KeyDerivationAlgorithms)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                foreach (var n in this.CryptoAlgorithms)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                foreach (var n in this.HashAlgorithms)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ReadOnlyMemory<byte> SessionId { get; }
        public AuthenticationType AuthenticationType { get; }
        public IReadOnlyList<KeyExchangeAlgorithm> KeyExchangeAlgorithms { get; }
        public IReadOnlyList<KeyDerivationAlgorithm> KeyDerivationAlgorithms { get; }
        public IReadOnlyList<CryptoAlgorithm> CryptoAlgorithms { get; }
        public IReadOnlyList<HashAlgorithm> HashAlgorithms { get; }

        public override bool Equals(ProfileMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (!BytesOperations.SequenceEqual(this.SessionId.Span, target.SessionId.Span)) return false;
            if (this.AuthenticationType != target.AuthenticationType) return false;
            if ((this.KeyExchangeAlgorithms is null) != (target.KeyExchangeAlgorithms is null)) return false;
            if (!(this.KeyExchangeAlgorithms is null) && !(target.KeyExchangeAlgorithms is null) && !CollectionHelper.Equals(this.KeyExchangeAlgorithms, target.KeyExchangeAlgorithms)) return false;
            if ((this.KeyDerivationAlgorithms is null) != (target.KeyDerivationAlgorithms is null)) return false;
            if (!(this.KeyDerivationAlgorithms is null) && !(target.KeyDerivationAlgorithms is null) && !CollectionHelper.Equals(this.KeyDerivationAlgorithms, target.KeyDerivationAlgorithms)) return false;
            if ((this.CryptoAlgorithms is null) != (target.CryptoAlgorithms is null)) return false;
            if (!(this.CryptoAlgorithms is null) && !(target.CryptoAlgorithms is null) && !CollectionHelper.Equals(this.CryptoAlgorithms, target.CryptoAlgorithms)) return false;
            if ((this.HashAlgorithms is null) != (target.HashAlgorithms is null)) return false;
            if (!(this.HashAlgorithms is null) && !(target.HashAlgorithms is null) && !CollectionHelper.Equals(this.HashAlgorithms, target.HashAlgorithms)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<ProfileMessage>
        {
            public void Serialize(RocketPackWriter w, ProfileMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (!value.SessionId.IsEmpty) propertyCount++;
                    if (value.AuthenticationType != default) propertyCount++;
                    if (value.KeyExchangeAlgorithms.Count != 0) propertyCount++;
                    if (value.KeyDerivationAlgorithms.Count != 0) propertyCount++;
                    if (value.CryptoAlgorithms.Count != 0) propertyCount++;
                    if (value.HashAlgorithms.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // SessionId
                if (!value.SessionId.IsEmpty)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId.Span);
                }
                // AuthenticationType
                if (value.AuthenticationType != default)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AuthenticationType);
                }
                // KeyExchangeAlgorithms
                if (value.KeyExchangeAlgorithms.Count != 0)
                {
                    w.Write((uint)2);
                    w.Write((uint)value.KeyExchangeAlgorithms.Count);
                    foreach (var n in value.KeyExchangeAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
                // KeyDerivationAlgorithms
                if (value.KeyDerivationAlgorithms.Count != 0)
                {
                    w.Write((uint)3);
                    w.Write((uint)value.KeyDerivationAlgorithms.Count);
                    foreach (var n in value.KeyDerivationAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
                // CryptoAlgorithms
                if (value.CryptoAlgorithms.Count != 0)
                {
                    w.Write((uint)4);
                    w.Write((uint)value.CryptoAlgorithms.Count);
                    foreach (var n in value.CryptoAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
                // HashAlgorithms
                if (value.HashAlgorithms.Count != 0)
                {
                    w.Write((uint)5);
                    w.Write((uint)value.HashAlgorithms.Count);
                    foreach (var n in value.HashAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
            }

            public ProfileMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ReadOnlyMemory<byte> p_sessionId = default;
                AuthenticationType p_authenticationType = default;
                IList<KeyExchangeAlgorithm> p_keyExchangeAlgorithms = default;
                IList<KeyDerivationAlgorithm> p_keyDerivationAlgorithms = default;
                IList<CryptoAlgorithm> p_cryptoAlgorithms = default;
                IList<HashAlgorithm> p_hashAlgorithms = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // SessionId
                            {
                                p_sessionId = r.GetMemory(32);
                                break;
                            }
                        case 1: // AuthenticationType
                            {
                                p_authenticationType = (AuthenticationType)r.GetUInt64();
                                break;
                            }
                        case 2: // KeyExchangeAlgorithms
                            {
                                var length = r.GetUInt32();
                                p_keyExchangeAlgorithms = new KeyExchangeAlgorithm[length];
                                for (int i = 0; i < p_keyExchangeAlgorithms.Count; i++)
                                {
                                    p_keyExchangeAlgorithms[i] = (KeyExchangeAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                        case 3: // KeyDerivationAlgorithms
                            {
                                var length = r.GetUInt32();
                                p_keyDerivationAlgorithms = new KeyDerivationAlgorithm[length];
                                for (int i = 0; i < p_keyDerivationAlgorithms.Count; i++)
                                {
                                    p_keyDerivationAlgorithms[i] = (KeyDerivationAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                        case 4: // CryptoAlgorithms
                            {
                                var length = r.GetUInt32();
                                p_cryptoAlgorithms = new CryptoAlgorithm[length];
                                for (int i = 0; i < p_cryptoAlgorithms.Count; i++)
                                {
                                    p_cryptoAlgorithms[i] = (CryptoAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                        case 5: // HashAlgorithms
                            {
                                var length = r.GetUInt32();
                                p_hashAlgorithms = new HashAlgorithm[length];
                                for (int i = 0; i < p_hashAlgorithms.Count; i++)
                                {
                                    p_hashAlgorithms[i] = (HashAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                    }
                }

                return new ProfileMessage(p_sessionId, p_authenticationType, p_keyExchangeAlgorithms, p_keyDerivationAlgorithms, p_cryptoAlgorithms, p_hashAlgorithms);
            }
        }
    }

    internal sealed partial class VerificationMessage : RocketPackMessageBase<VerificationMessage>
    {
        static VerificationMessage()
        {
            VerificationMessage.Formatter = new CustomFormatter();
        }

        public VerificationMessage(ProfileMessage profileMessage, OmniAgreementPublicKey agreementPublicKey)
        {
            if (profileMessage is null) throw new ArgumentNullException("profileMessage");
            if (agreementPublicKey is null) throw new ArgumentNullException("agreementPublicKey");

            this.ProfileMessage = profileMessage;
            this.AgreementPublicKey = agreementPublicKey;

            {
                var hashCode = new HashCode();
                if (this.ProfileMessage != default) hashCode.Add(this.ProfileMessage.GetHashCode());
                if (this.AgreementPublicKey != default) hashCode.Add(this.AgreementPublicKey.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ProfileMessage ProfileMessage { get; }
        public OmniAgreementPublicKey AgreementPublicKey { get; }

        public override bool Equals(VerificationMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.ProfileMessage != target.ProfileMessage) return false;
            if (this.AgreementPublicKey != target.AgreementPublicKey) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<VerificationMessage>
        {
            public void Serialize(RocketPackWriter w, VerificationMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.ProfileMessage != default) propertyCount++;
                    if (value.AgreementPublicKey != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // ProfileMessage
                if (value.ProfileMessage != default)
                {
                    w.Write((uint)0);
                    ProfileMessage.Formatter.Serialize(w, value.ProfileMessage, rank + 1);
                }
                // AgreementPublicKey
                if (value.AgreementPublicKey != default)
                {
                    w.Write((uint)1);
                    OmniAgreementPublicKey.Formatter.Serialize(w, value.AgreementPublicKey, rank + 1);
                }
            }

            public VerificationMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ProfileMessage p_profileMessage = default;
                OmniAgreementPublicKey p_agreementPublicKey = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // ProfileMessage
                            {
                                p_profileMessage = ProfileMessage.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 1: // AgreementPublicKey
                            {
                                p_agreementPublicKey = OmniAgreementPublicKey.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new VerificationMessage(p_profileMessage, p_agreementPublicKey);
            }
        }
    }

    internal sealed partial class AuthenticationMessage : RocketPackMessageBase<AuthenticationMessage>
    {
        static AuthenticationMessage()
        {
            AuthenticationMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxHashesCount = 32;

        public AuthenticationMessage(IList<ReadOnlyMemory<byte>> hashes)
        {
            if (hashes is null) throw new ArgumentNullException("hashes");
            if (hashes.Count > 32) throw new ArgumentOutOfRangeException("hashes");
            foreach (var n in hashes)
            {
                if (n.Length > 32) throw new ArgumentOutOfRangeException("n");
            }

            this.Hashes = new ReadOnlyCollection<ReadOnlyMemory<byte>>(hashes);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Hashes)
                {
                    if (!n.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(n.Span));
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<ReadOnlyMemory<byte>> Hashes { get; }

        public override bool Equals(AuthenticationMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Hashes is null) != (target.Hashes is null)) return false;
            if (!(this.Hashes is null) && !(target.Hashes is null) && !CollectionHelper.Equals(this.Hashes, target.Hashes)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<AuthenticationMessage>
        {
            public void Serialize(RocketPackWriter w, AuthenticationMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Hashes.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Hashes
                if (value.Hashes.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Hashes.Count);
                    foreach (var n in value.Hashes)
                    {
                        w.Write(n.Span);
                    }
                }
            }

            public AuthenticationMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IList<ReadOnlyMemory<byte>> p_hashes = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Hashes
                            {
                                var length = r.GetUInt32();
                                p_hashes = new ReadOnlyMemory<byte>[length];
                                for (int i = 0; i < p_hashes.Count; i++)
                                {
                                    p_hashes[i] = r.GetMemory(32);
                                }
                                break;
                            }
                    }
                }

                return new AuthenticationMessage(p_hashes);
            }
        }
    }

}
