using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Cryptography;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Network.Connection.Secure.V1
{
    public enum KeyExchangeAlgorithm : byte
    {
        EcDh_P521_Sha2_256 = 0,
    }

    public enum KeyDerivationAlgorithm : byte
    {
        Pbkdf2 = 0,
    }

    public enum HashAlgorithm : byte
    {
        Sha2_256 = 0,
    }

    public enum CryptoAlgorithm : byte
    {
        Aes_256 = 0,
    }

    public enum AuthenticationType : byte
    {
        None = 0,
        Password = 1,
    }

    public sealed partial class SecureConnectionProfileMessage : RocketPackMessageBase<SecureConnectionProfileMessage>
    {
        static SecureConnectionProfileMessage()
        {
            SecureConnectionProfileMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxSessionIdLength = 32;
        public static readonly int MaxKeyExchangeAlgorithmsCount = 32;
        public static readonly int MaxKeyDerivationAlgorithmsCount = 32;
        public static readonly int MaxCryptoAlgorithmsCount = 32;
        public static readonly int MaxHashAlgorithmsCount = 32;

        public SecureConnectionProfileMessage(ReadOnlyMemory<byte> sessionId, AuthenticationType authenticationType, IList<KeyExchangeAlgorithm> keyExchangeAlgorithms, IList<KeyDerivationAlgorithm> keyDerivationAlgorithms, IList<CryptoAlgorithm> cryptoAlgorithms, IList<HashAlgorithm> hashAlgorithms)
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

        public override bool Equals(SecureConnectionProfileMessage target)
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

        private sealed class CustomFormatter : IRocketPackFormatter<SecureConnectionProfileMessage>
        {
            public void Serialize(RocketPackWriter w, SecureConnectionProfileMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // SessionId
                if (!value.SessionId.IsEmpty)
                {
                    w.Write((ulong)0);
                    w.Write(value.SessionId.Span);
                }
                // AuthenticationType
                if (value.AuthenticationType != default)
                {
                    w.Write((ulong)1);
                    w.Write((ulong)value.AuthenticationType);
                }
                // KeyExchangeAlgorithms
                if (value.KeyExchangeAlgorithms.Count != 0)
                {
                    w.Write((ulong)2);
                    w.Write((ulong)value.KeyExchangeAlgorithms.Count);
                    foreach (var n in value.KeyExchangeAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
                // KeyDerivationAlgorithms
                if (value.KeyDerivationAlgorithms.Count != 0)
                {
                    w.Write((ulong)3);
                    w.Write((ulong)value.KeyDerivationAlgorithms.Count);
                    foreach (var n in value.KeyDerivationAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
                // CryptoAlgorithms
                if (value.CryptoAlgorithms.Count != 0)
                {
                    w.Write((ulong)4);
                    w.Write((ulong)value.CryptoAlgorithms.Count);
                    foreach (var n in value.CryptoAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
                // HashAlgorithms
                if (value.HashAlgorithms.Count != 0)
                {
                    w.Write((ulong)5);
                    w.Write((ulong)value.HashAlgorithms.Count);
                    foreach (var n in value.HashAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
            }

            public SecureConnectionProfileMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                ReadOnlyMemory<byte> p_sessionId = default;
                AuthenticationType p_authenticationType = default;
                IList<KeyExchangeAlgorithm> p_keyExchangeAlgorithms = default;
                IList<KeyDerivationAlgorithm> p_keyDerivationAlgorithms = default;
                IList<CryptoAlgorithm> p_cryptoAlgorithms = default;
                IList<HashAlgorithm> p_hashAlgorithms = default;

                while (r.Available > 0)
                {
                    int id = (int)r.GetUInt64();
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
                                var length = (int)r.GetUInt64();
                                p_keyExchangeAlgorithms = new KeyExchangeAlgorithm[length];
                                for (int i = 0; i < p_keyExchangeAlgorithms.Count; i++)
                                {
                                    p_keyExchangeAlgorithms[i] = (KeyExchangeAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                        case 3: // KeyDerivationAlgorithms
                            {
                                var length = (int)r.GetUInt64();
                                p_keyDerivationAlgorithms = new KeyDerivationAlgorithm[length];
                                for (int i = 0; i < p_keyDerivationAlgorithms.Count; i++)
                                {
                                    p_keyDerivationAlgorithms[i] = (KeyDerivationAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                        case 4: // CryptoAlgorithms
                            {
                                var length = (int)r.GetUInt64();
                                p_cryptoAlgorithms = new CryptoAlgorithm[length];
                                for (int i = 0; i < p_cryptoAlgorithms.Count; i++)
                                {
                                    p_cryptoAlgorithms[i] = (CryptoAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                        case 5: // HashAlgorithms
                            {
                                var length = (int)r.GetUInt64();
                                p_hashAlgorithms = new HashAlgorithm[length];
                                for (int i = 0; i < p_hashAlgorithms.Count; i++)
                                {
                                    p_hashAlgorithms[i] = (HashAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                    }
                }

                return new SecureConnectionProfileMessage(p_sessionId, p_authenticationType, p_keyExchangeAlgorithms, p_keyDerivationAlgorithms, p_cryptoAlgorithms, p_hashAlgorithms);
            }
        }
    }

    public sealed partial class SecureConnectionVerificationMessage : RocketPackMessageBase<SecureConnectionVerificationMessage>
    {
        static SecureConnectionVerificationMessage()
        {
            SecureConnectionVerificationMessage.Formatter = new CustomFormatter();
        }

        public SecureConnectionVerificationMessage(SecureConnectionProfileMessage profileMessage, OmniAgreementPublicKey agreementPublicKey)
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

        public SecureConnectionProfileMessage ProfileMessage { get; }
        public OmniAgreementPublicKey AgreementPublicKey { get; }

        public override bool Equals(SecureConnectionVerificationMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.ProfileMessage != target.ProfileMessage) return false;
            if (this.AgreementPublicKey != target.AgreementPublicKey) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<SecureConnectionVerificationMessage>
        {
            public void Serialize(RocketPackWriter w, SecureConnectionVerificationMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // ProfileMessage
                if (value.ProfileMessage != default)
                {
                    w.Write((ulong)0);
                    SecureConnectionProfileMessage.Formatter.Serialize(w, value.ProfileMessage, rank + 1);
                }
                // AgreementPublicKey
                if (value.AgreementPublicKey != default)
                {
                    w.Write((ulong)1);
                    OmniAgreementPublicKey.Formatter.Serialize(w, value.AgreementPublicKey, rank + 1);
                }
            }

            public SecureConnectionVerificationMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                SecureConnectionProfileMessage p_profileMessage = default;
                OmniAgreementPublicKey p_agreementPublicKey = default;

                while (r.Available > 0)
                {
                    int id = (int)r.GetUInt64();
                    switch (id)
                    {
                        case 0: // ProfileMessage
                            {
                                p_profileMessage = SecureConnectionProfileMessage.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 1: // AgreementPublicKey
                            {
                                p_agreementPublicKey = OmniAgreementPublicKey.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new SecureConnectionVerificationMessage(p_profileMessage, p_agreementPublicKey);
            }
        }
    }

    public sealed partial class SecureConnectionAuthenticationMessage : RocketPackMessageBase<SecureConnectionAuthenticationMessage>
    {
        static SecureConnectionAuthenticationMessage()
        {
            SecureConnectionAuthenticationMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxHashesCount = 32;

        public SecureConnectionAuthenticationMessage(IList<ReadOnlyMemory<byte>> hashes)
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

        public override bool Equals(SecureConnectionAuthenticationMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Hashes is null) != (target.Hashes is null)) return false;
            if (!(this.Hashes is null) && !(target.Hashes is null) && !CollectionHelper.Equals(this.Hashes, target.Hashes)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<SecureConnectionAuthenticationMessage>
        {
            public void Serialize(RocketPackWriter w, SecureConnectionAuthenticationMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Hashes
                if (value.Hashes.Count != 0)
                {
                    w.Write((ulong)0);
                    w.Write((ulong)value.Hashes.Count);
                    foreach (var n in value.Hashes)
                    {
                        w.Write(n.Span);
                    }
                }
            }

            public SecureConnectionAuthenticationMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                IList<ReadOnlyMemory<byte>> p_hashes = default;

                while (r.Available > 0)
                {
                    int id = (int)r.GetUInt64();
                    switch (id)
                    {
                        case 0: // Hashes
                            {
                                var length = (int)r.GetUInt64();
                                p_hashes = new ReadOnlyMemory<byte>[length];
                                for (int i = 0; i < p_hashes.Count; i++)
                                {
                                    p_hashes[i] = r.GetMemory(32);
                                }
                                break;
                            }
                    }
                }

                return new SecureConnectionAuthenticationMessage(p_hashes);
            }
        }
    }

}
