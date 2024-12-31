
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;
using Omnius.Core.Serialization;

namespace Omnius.Core.Omnikit.Connections.Secure.V1;

[Flags]
enum AuthType : uint
{
    None = 0,
    Sign = 1
}

[Flags]
enum KeyExchangeAlgorithmType : uint
{
    None = 0,
    X25519 = 1
}

[Flags]
enum KeyDerivationAlgorithmType : uint
{
    None = 0,
    Hkdf = 1
}

[Flags]
enum CipherAlgorithmType : uint
{
    None = 0,
    Aes256Gcm = 1
}

[Flags]
enum HashAlgorithmType : uint
{
    None = 0,
    Sha3_256 = 1
}

class ProfileMessage : RocketMessage<ProfileMessage>
{
    public required byte[] SessionId { get; init; }
    public required AuthType AuthType { get; init; }
    public required KeyExchangeAlgorithmType KeyExchangeAlgorithmType { get; init; }
    public required KeyDerivationAlgorithmType KeyDerivationAlgorithmType { get; init; }
    public required CipherAlgorithmType CipherAlgorithmType { get; init; }
    public required HashAlgorithmType HashAlgorithmType { get; init; }

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(this.SessionId);
            h.Add(this.AuthType);
            h.Add(this.KeyExchangeAlgorithmType);
            h.Add(this.KeyDerivationAlgorithmType);
            h.Add(this.CipherAlgorithmType);
            h.Add(this.HashAlgorithmType);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public override bool Equals(ProfileMessage? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.SessionId.SequenceEqual(other.SessionId) && this.AuthType == other.AuthType
            && this.KeyExchangeAlgorithmType == other.KeyExchangeAlgorithmType && this.KeyDerivationAlgorithmType == other.KeyDerivationAlgorithmType
            && this.CipherAlgorithmType == other.CipherAlgorithmType && this.HashAlgorithmType == other.HashAlgorithmType;
    }

    static ProfileMessage()
    {
        Formatter = new CustomSerializer();
        Empty = new ProfileMessage() { SessionId = Array.Empty<byte>(), AuthType = AuthType.None, KeyExchangeAlgorithmType = KeyExchangeAlgorithmType.None, KeyDerivationAlgorithmType = KeyDerivationAlgorithmType.None, CipherAlgorithmType = CipherAlgorithmType.None, HashAlgorithmType = HashAlgorithmType.None };
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<ProfileMessage>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in ProfileMessage value, scoped in int depth)
        {
            w.Put(value.SessionId);
            w.Put(value.AuthType.ToString());
            w.Put(value.KeyExchangeAlgorithmType.ToString());
            w.Put(value.KeyDerivationAlgorithmType.ToString());
            w.Put(value.CipherAlgorithmType.ToString());
            w.Put(value.HashAlgorithmType.ToString());
        }
        public ProfileMessage Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var sessionId = r.GetBytes(1024);
            var authType = Enum.Parse<AuthType>(r.GetString(1024));
            var keyExchangeAlgorithmType = Enum.Parse<KeyExchangeAlgorithmType>(r.GetString(1024));
            var keyDerivationAlgorithmType = Enum.Parse<KeyDerivationAlgorithmType>(r.GetString(1024));
            var cipherAlgorithmType = Enum.Parse<CipherAlgorithmType>(r.GetString(1024));
            var hashAlgorithmType = Enum.Parse<HashAlgorithmType>(r.GetString(1024));

            return new ProfileMessage()
            {
                SessionId = sessionId,
                AuthType = authType,
                KeyExchangeAlgorithmType = keyExchangeAlgorithmType,
                KeyDerivationAlgorithmType = keyDerivationAlgorithmType,
                CipherAlgorithmType = cipherAlgorithmType,
                HashAlgorithmType = hashAlgorithmType
            };
        }
    }
}
