
using System.Text.Json;
using System.Text.Json.Serialization;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

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

class ProfileMessage : IRocketPackStruct<ProfileMessage>, IEquatable<ProfileMessage>
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

    public bool Equals(ProfileMessage? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.SessionId.SequenceEqual(other.SessionId) && this.AuthType == other.AuthType
            && this.KeyExchangeAlgorithmType == other.KeyExchangeAlgorithmType && this.KeyDerivationAlgorithmType == other.KeyDerivationAlgorithmType
            && this.CipherAlgorithmType == other.CipherAlgorithmType && this.HashAlgorithmType == other.HashAlgorithmType;
    }

    public static void Pack(ref RocketPackBytesEncoder encoder, in ProfileMessage value)
    {
        encoder.WriteMap(6);

        encoder.WriteU64(0);
        encoder.WriteBytes(value.SessionId);

        encoder.WriteU64(1);
        encoder.WriteString(value.AuthType.ToString());

        encoder.WriteU64(2);
        encoder.WriteString(value.KeyExchangeAlgorithmType.ToString());

        encoder.WriteU64(3);
        encoder.WriteString(value.KeyDerivationAlgorithmType.ToString());

        encoder.WriteU64(4);
        encoder.WriteString(value.CipherAlgorithmType.ToString());

        encoder.WriteU64(5);
        encoder.WriteString(value.HashAlgorithmType.ToString());
    }

    public static ProfileMessage Unpack(ref RocketPackBytesDecoder decoder)
    {
        var count = decoder.ReadMap();

        byte[]? sessionId = null;
        AuthType? authType = null;
        KeyExchangeAlgorithmType? keyExchangeAlgorithmType = null;
        KeyDerivationAlgorithmType? keyDerivationAlgorithmType = null;
        CipherAlgorithmType? cipherAlgorithmType = null;
        HashAlgorithmType? hashAlgorithmType = null;

        for (ulong i = 0; i < count; i++)
        {
            switch (decoder.ReadU64())
            {
                case 0:
                    sessionId = decoder.ReadBytesToArray();
                    break;
                case 1:
                    authType = Enum.Parse<AuthType>(decoder.ReadString());
                    break;
                case 2:
                    keyExchangeAlgorithmType = Enum.Parse<KeyExchangeAlgorithmType>(decoder.ReadString());
                    break;
                case 3:
                    keyDerivationAlgorithmType = Enum.Parse<KeyDerivationAlgorithmType>(decoder.ReadString());
                    break;
                case 4:
                    cipherAlgorithmType = Enum.Parse<CipherAlgorithmType>(decoder.ReadString());
                    break;
                case 5:
                    hashAlgorithmType = Enum.Parse<HashAlgorithmType>(decoder.ReadString());
                    break;
                default:
                    decoder.SkipField();
                    break;
            }
        }

        return new ProfileMessage()
        {
            SessionId = sessionId ?? throw RocketPackDecoderException.CreateOther("missing SessionId"),
            AuthType = authType ?? throw RocketPackDecoderException.CreateOther("missing AuthType"),
            KeyExchangeAlgorithmType = keyExchangeAlgorithmType ?? throw RocketPackDecoderException.CreateOther("missing KeyExchangeAlgorithmType"),
            KeyDerivationAlgorithmType = keyDerivationAlgorithmType ?? throw RocketPackDecoderException.CreateOther("missing KeyDerivationAlgorithmType"),
            CipherAlgorithmType = cipherAlgorithmType ?? throw RocketPackDecoderException.CreateOther("missing CipherAlgorithmType"),
            HashAlgorithmType = hashAlgorithmType ?? throw RocketPackDecoderException.CreateOther("missing HashAlgorithmType"),
        };
    }
}
