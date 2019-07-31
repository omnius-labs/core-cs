using Omnix.Algorithms.Cryptography;

#nullable enable

namespace Omnix.Network.Connections.Secure.V1.Internal
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

    internal sealed partial class ProfileMessage : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<ProfileMessage>
    {
        static ProfileMessage()
        {
            ProfileMessage.Formatter = new CustomFormatter();
            ProfileMessage.Empty = new ProfileMessage(global::System.ReadOnlyMemory<byte>.Empty, (AuthenticationType)0, global::System.Array.Empty<KeyExchangeAlgorithm>(), global::System.Array.Empty<KeyDerivationAlgorithm>(), global::System.Array.Empty<CryptoAlgorithm>(), global::System.Array.Empty<HashAlgorithm>());
        }

        private readonly int __hashCode;

        public static readonly int MaxSessionIdLength = 32;
        public static readonly int MaxKeyExchangeAlgorithmsCount = 32;
        public static readonly int MaxKeyDerivationAlgorithmsCount = 32;
        public static readonly int MaxCryptoAlgorithmsCount = 32;
        public static readonly int MaxHashAlgorithmsCount = 32;

        public ProfileMessage(global::System.ReadOnlyMemory<byte> sessionId, AuthenticationType authenticationType, KeyExchangeAlgorithm[] keyExchangeAlgorithms, KeyDerivationAlgorithm[] keyDerivationAlgorithms, CryptoAlgorithm[] cryptoAlgorithms, HashAlgorithm[] hashAlgorithms)
        {
            if (sessionId.Length > 32) throw new global::System.ArgumentOutOfRangeException("sessionId");
            if (keyExchangeAlgorithms is null) throw new global::System.ArgumentNullException("keyExchangeAlgorithms");
            if (keyExchangeAlgorithms.Length > 32) throw new global::System.ArgumentOutOfRangeException("keyExchangeAlgorithms");
            if (keyDerivationAlgorithms is null) throw new global::System.ArgumentNullException("keyDerivationAlgorithms");
            if (keyDerivationAlgorithms.Length > 32) throw new global::System.ArgumentOutOfRangeException("keyDerivationAlgorithms");
            if (cryptoAlgorithms is null) throw new global::System.ArgumentNullException("cryptoAlgorithms");
            if (cryptoAlgorithms.Length > 32) throw new global::System.ArgumentOutOfRangeException("cryptoAlgorithms");
            if (hashAlgorithms is null) throw new global::System.ArgumentNullException("hashAlgorithms");
            if (hashAlgorithms.Length > 32) throw new global::System.ArgumentOutOfRangeException("hashAlgorithms");

            this.SessionId = sessionId;
            this.AuthenticationType = authenticationType;
            this.KeyExchangeAlgorithms = new global::Omnix.DataStructures.ReadOnlyListSlim<KeyExchangeAlgorithm>(keyExchangeAlgorithms);
            this.KeyDerivationAlgorithms = new global::Omnix.DataStructures.ReadOnlyListSlim<KeyDerivationAlgorithm>(keyDerivationAlgorithms);
            this.CryptoAlgorithms = new global::Omnix.DataStructures.ReadOnlyListSlim<CryptoAlgorithm>(cryptoAlgorithms);
            this.HashAlgorithms = new global::Omnix.DataStructures.ReadOnlyListSlim<HashAlgorithm>(hashAlgorithms);

            {
                var __h = new global::System.HashCode();
                if (!this.SessionId.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.SessionId.Span));
                if (this.AuthenticationType != default) __h.Add(this.AuthenticationType.GetHashCode());
                foreach (var n in this.KeyExchangeAlgorithms)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                foreach (var n in this.KeyDerivationAlgorithms)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                foreach (var n in this.CryptoAlgorithms)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                foreach (var n in this.HashAlgorithms)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                __hashCode = __h.ToHashCode();
            }
        }

        public global::System.ReadOnlyMemory<byte> SessionId { get; }
        public AuthenticationType AuthenticationType { get; }
        public global::Omnix.DataStructures.ReadOnlyListSlim<KeyExchangeAlgorithm> KeyExchangeAlgorithms { get; }
        public global::Omnix.DataStructures.ReadOnlyListSlim<KeyDerivationAlgorithm> KeyDerivationAlgorithms { get; }
        public global::Omnix.DataStructures.ReadOnlyListSlim<CryptoAlgorithm> CryptoAlgorithms { get; }
        public global::Omnix.DataStructures.ReadOnlyListSlim<HashAlgorithm> HashAlgorithms { get; }

        public override bool Equals(ProfileMessage target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.SessionId.Span, target.SessionId.Span)) return false;
            if (this.AuthenticationType != target.AuthenticationType) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.KeyExchangeAlgorithms, target.KeyExchangeAlgorithms)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.KeyDerivationAlgorithms, target.KeyDerivationAlgorithms)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.CryptoAlgorithms, target.CryptoAlgorithms)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.HashAlgorithms, target.HashAlgorithms)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<ProfileMessage>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, ProfileMessage value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (!value.SessionId.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (value.AuthenticationType != (AuthenticationType)0)
                    {
                        propertyCount++;
                    }
                    if (value.KeyExchangeAlgorithms.Count != 0)
                    {
                        propertyCount++;
                    }
                    if (value.KeyDerivationAlgorithms.Count != 0)
                    {
                        propertyCount++;
                    }
                    if (value.CryptoAlgorithms.Count != 0)
                    {
                        propertyCount++;
                    }
                    if (value.HashAlgorithms.Count != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (!value.SessionId.IsEmpty)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId.Span);
                }
                if (value.AuthenticationType != (AuthenticationType)0)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.AuthenticationType);
                }
                if (value.KeyExchangeAlgorithms.Count != 0)
                {
                    w.Write((uint)2);
                    w.Write((uint)value.KeyExchangeAlgorithms.Count);
                    foreach (var n in value.KeyExchangeAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
                if (value.KeyDerivationAlgorithms.Count != 0)
                {
                    w.Write((uint)3);
                    w.Write((uint)value.KeyDerivationAlgorithms.Count);
                    foreach (var n in value.KeyDerivationAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
                if (value.CryptoAlgorithms.Count != 0)
                {
                    w.Write((uint)4);
                    w.Write((uint)value.CryptoAlgorithms.Count);
                    foreach (var n in value.CryptoAlgorithms)
                    {
                        w.Write((ulong)n);
                    }
                }
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

            public ProfileMessage Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                global::System.ReadOnlyMemory<byte> p_sessionId = global::System.ReadOnlyMemory<byte>.Empty;
                AuthenticationType p_authenticationType = (AuthenticationType)0;
                KeyExchangeAlgorithm[] p_keyExchangeAlgorithms = global::System.Array.Empty<KeyExchangeAlgorithm>();
                KeyDerivationAlgorithm[] p_keyDerivationAlgorithms = global::System.Array.Empty<KeyDerivationAlgorithm>();
                CryptoAlgorithm[] p_cryptoAlgorithms = global::System.Array.Empty<CryptoAlgorithm>();
                HashAlgorithm[] p_hashAlgorithms = global::System.Array.Empty<HashAlgorithm>();

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_sessionId = r.GetMemory(32);
                                break;
                            }
                        case 1:
                            {
                                p_authenticationType = (AuthenticationType)r.GetUInt64();
                                break;
                            }
                        case 2:
                            {
                                var length = r.GetUInt32();
                                p_keyExchangeAlgorithms = new KeyExchangeAlgorithm[length];
                                for (int i = 0; i < p_keyExchangeAlgorithms.Length; i++)
                                {
                                    p_keyExchangeAlgorithms[i] = (KeyExchangeAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                        case 3:
                            {
                                var length = r.GetUInt32();
                                p_keyDerivationAlgorithms = new KeyDerivationAlgorithm[length];
                                for (int i = 0; i < p_keyDerivationAlgorithms.Length; i++)
                                {
                                    p_keyDerivationAlgorithms[i] = (KeyDerivationAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                        case 4:
                            {
                                var length = r.GetUInt32();
                                p_cryptoAlgorithms = new CryptoAlgorithm[length];
                                for (int i = 0; i < p_cryptoAlgorithms.Length; i++)
                                {
                                    p_cryptoAlgorithms[i] = (CryptoAlgorithm)r.GetUInt64();
                                }
                                break;
                            }
                        case 5:
                            {
                                var length = r.GetUInt32();
                                p_hashAlgorithms = new HashAlgorithm[length];
                                for (int i = 0; i < p_hashAlgorithms.Length; i++)
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

    internal sealed partial class VerificationMessage : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<VerificationMessage>
    {
        static VerificationMessage()
        {
            VerificationMessage.Formatter = new CustomFormatter();
            VerificationMessage.Empty = new VerificationMessage(ProfileMessage.Empty, OmniAgreementPublicKey.Empty);
        }

        private readonly int __hashCode;

        public VerificationMessage(ProfileMessage profileMessage, OmniAgreementPublicKey agreementPublicKey)
        {
            if (profileMessage is null) throw new global::System.ArgumentNullException("profileMessage");
            if (agreementPublicKey is null) throw new global::System.ArgumentNullException("agreementPublicKey");

            this.ProfileMessage = profileMessage;
            this.AgreementPublicKey = agreementPublicKey;

            {
                var __h = new global::System.HashCode();
                if (this.ProfileMessage != default) __h.Add(this.ProfileMessage.GetHashCode());
                if (this.AgreementPublicKey != default) __h.Add(this.AgreementPublicKey.GetHashCode());
                __hashCode = __h.ToHashCode();
            }
        }

        public ProfileMessage ProfileMessage { get; }
        public OmniAgreementPublicKey AgreementPublicKey { get; }

        public override bool Equals(VerificationMessage target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.ProfileMessage != target.ProfileMessage) return false;
            if (this.AgreementPublicKey != target.AgreementPublicKey) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<VerificationMessage>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, VerificationMessage value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.ProfileMessage != ProfileMessage.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.AgreementPublicKey != OmniAgreementPublicKey.Empty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.ProfileMessage != ProfileMessage.Empty)
                {
                    w.Write((uint)0);
                    ProfileMessage.Formatter.Serialize(w, value.ProfileMessage, rank + 1);
                }
                if (value.AgreementPublicKey != OmniAgreementPublicKey.Empty)
                {
                    w.Write((uint)1);
                    OmniAgreementPublicKey.Formatter.Serialize(w, value.AgreementPublicKey, rank + 1);
                }
            }

            public VerificationMessage Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                ProfileMessage p_profileMessage = ProfileMessage.Empty;
                OmniAgreementPublicKey p_agreementPublicKey = OmniAgreementPublicKey.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_profileMessage = ProfileMessage.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 1:
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

    internal sealed partial class AuthenticationMessage : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<AuthenticationMessage>
    {
        static AuthenticationMessage()
        {
            AuthenticationMessage.Formatter = new CustomFormatter();
            AuthenticationMessage.Empty = new AuthenticationMessage(global::System.Array.Empty<global::System.ReadOnlyMemory<byte>>());
        }

        private readonly int __hashCode;

        public static readonly int MaxHashesCount = 32;

        public AuthenticationMessage(global::System.ReadOnlyMemory<byte>[] hashes)
        {
            if (hashes is null) throw new global::System.ArgumentNullException("hashes");
            if (hashes.Length > 32) throw new global::System.ArgumentOutOfRangeException("hashes");
            foreach (var n in hashes)
            {
                if (n.Length > 32) throw new global::System.ArgumentOutOfRangeException("n");
            }

            this.Hashes = new global::Omnix.DataStructures.ReadOnlyListSlim<global::System.ReadOnlyMemory<byte>>(hashes);

            {
                var __h = new global::System.HashCode();
                foreach (var n in this.Hashes)
                {
                    if (!n.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(n.Span));
                }
                __hashCode = __h.ToHashCode();
            }
        }

        public global::Omnix.DataStructures.ReadOnlyListSlim<global::System.ReadOnlyMemory<byte>> Hashes { get; }

        public override bool Equals(AuthenticationMessage target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.Hashes, target.Hashes)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<AuthenticationMessage>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, AuthenticationMessage value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.Hashes.Count != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

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

            public AuthenticationMessage Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                global::System.ReadOnlyMemory<byte>[] p_hashes = global::System.Array.Empty<global::System.ReadOnlyMemory<byte>>();

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                var length = r.GetUInt32();
                                p_hashes = new global::System.ReadOnlyMemory<byte>[length];
                                for (int i = 0; i < p_hashes.Length; i++)
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
