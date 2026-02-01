using Omnius.Core.Omnikit.Converters;
using Omnius.Core.Omnikit.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit;

// ref. https://github.com/omnius-labs/core-rs/blob/6ac4b87f9ec6c6de4df4d8c2aa9cb9fa7568863a/modules/omnikit/src/model/omni_sign.rs

[Flags]
public enum OmniSignType
{
    None = 0,
    Ed25519_Sha3_256_Base64Url = 1
}

public class OmniSigner : IRocketPackStruct<OmniSigner>, IEquatable<OmniSigner>
{
    public required OmniSignType Type { get; init; }
    public required string Name { get; init; }
    public required byte[] Key { get; init; }

    public static OmniSigner Create(OmniSignType type, string name)
    {
        if (type == OmniSignType.Ed25519_Sha3_256_Base64Url)
        {
            var signingKey = Ed25519.CreateSigningKey();
            var key = Ed25519.GetPrivateKeyPkcs8Der(signingKey);

            return new OmniSigner
            {
                Type = type,
                Name = name,
                Key = key
            };
        }
        else
        {
            throw new NotSupportedException("Unsupported sign type");
        }
    }

    public OmniCert Sign(byte[] value)
    {
        if (this.Type == OmniSignType.Ed25519_Sha3_256_Base64Url)
        {
            var signingKey = Ed25519.ParsePrivateKeyPkcs8Der(this.Key);

            return new OmniCert
            {
                Type = this.Type,
                Name = this.Name,
                PublicKey = Ed25519.GetPublicKeyDer(signingKey.GeneratePublicKey()),
                Value = Ed25519.Sign(this.Key, value)
            };
        }
        else
        {
            throw new NotSupportedException("Unsupported sign type");
        }
    }

    public override string ToString()
    {
        if (this.Type == OmniSignType.Ed25519_Sha3_256_Base64Url)
        {
            var signingKey = Ed25519.ParsePrivateKeyPkcs8Der(this.Key);
            var publicKey = Ed25519.GetPublicKeyDer(signingKey.GeneratePublicKey());
            var hash = Sha3_256.ComputeHash(publicKey);

            return $"{this.Name}@{Base64Url.Instance.BytesToString(hash)}";
        }
        else
        {
            return string.Empty;
        }
    }

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(this.Type);
            h.Add(this.Name);
            h.Add(this.Key);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public bool Equals(OmniSigner? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Type == other.Type && this.Name == other.Name && this.Key.SequenceEqual(other.Key);
    }

    public static void Pack(ref RocketPackBytesEncoder encoder, in OmniSigner value)
    {
        encoder.WriteMap(3);

        encoder.WriteU64(0);
        encoder.WriteString(value.Type.ToString().ToLowerInvariant());

        encoder.WriteU64(1);
        encoder.WriteString(value.Name);

        encoder.WriteU64(2);
        encoder.WriteBytes(value.Key);
    }

    public static OmniSigner Unpack(ref RocketPackBytesDecoder decoder)
    {
        var count = decoder.ReadMap();

        OmniSignType? type = null;
        string? name = null;
        byte[]? key = null;

        for (ulong i = 0; i < count; i++)
        {
            switch (decoder.ReadU64())
            {
                case 0:
                    type = Enum.Parse<OmniSignType>(decoder.ReadString(), true);
                    break;
                case 1:
                    name = decoder.ReadString();
                    break;
                case 2:
                    key = decoder.ReadBytesToArray();
                    break;
                default:
                    decoder.SkipField();
                    break;
            }
        }

        return new OmniSigner()
        {
            Type = type ?? throw RocketPackDecoderException.CreateOther("missing field: Type"),
            Name = name ?? throw RocketPackDecoderException.CreateOther("missing field: Name"),
            Key = key ?? throw RocketPackDecoderException.CreateOther("missing field: Key"),
        };
    }
}

public class OmniCert : IRocketPackStruct<OmniCert>, IEquatable<OmniCert>
{
    public required OmniSignType Type { get; init; }
    public required string Name { get; init; }
    public required byte[] PublicKey { get; init; }
    public required byte[] Value { get; init; }

    public bool Verify(byte[] msg)
    {
        if (this.Type == OmniSignType.Ed25519_Sha3_256_Base64Url)
        {
            return Ed25519.Verify(this.PublicKey, this.Value, msg);
        }

        throw new NotSupportedException("Unsupported sign type");
    }

    public override string ToString()
    {
        if (this.Type == OmniSignType.Ed25519_Sha3_256_Base64Url)
        {
            var hash = Sha3_256.ComputeHash(this.PublicKey);
            return $"{this.Name}@{Base64Url.Instance.BytesToString(hash)}";
        }

        return string.Empty;
    }

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(this.Type);
            h.Add(this.Name);
            h.Add(this.PublicKey);
            h.Add(this.Value);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public bool Equals(OmniCert? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Type == other.Type && this.Name == other.Name && this.PublicKey.SequenceEqual(other.PublicKey) && this.Value.SequenceEqual(other.Value);
    }

    public static void Pack(ref RocketPackBytesEncoder encoder, in OmniCert value)
    {
        encoder.WriteMap(4);

        encoder.WriteU64(0);
        encoder.WriteString(value.Type.ToString().ToLowerInvariant());

        encoder.WriteU64(1);
        encoder.WriteString(value.Name);

        encoder.WriteU64(2);
        encoder.WriteBytes(value.PublicKey);

        encoder.WriteU64(3);
        encoder.WriteBytes(value.Value);
    }

    public static OmniCert Unpack(ref RocketPackBytesDecoder decoder)
    {
        var count = decoder.ReadMap();

        OmniSignType? type = null;
        string? name = null;
        byte[]? publicKey = null;
        byte[]? value = null;

        for (ulong i = 0; i < count; i++)
        {
            switch (decoder.ReadU64())
            {
                case 0:
                    type = Enum.Parse<OmniSignType>(decoder.ReadString(), true);
                    break;
                case 1:
                    name = decoder.ReadString();
                    break;
                case 2:
                    publicKey = decoder.ReadBytesToArray();
                    break;
                case 3:
                    value = decoder.ReadBytesToArray();
                    break;
                default:
                    decoder.SkipField();
                    break;
            }
        }

        return new OmniCert()
        {
            Type = type ?? throw RocketPackDecoderException.CreateOther("missing field: Type"),
            Name = name ?? throw RocketPackDecoderException.CreateOther("missing field: Name"),
            PublicKey = publicKey ?? throw RocketPackDecoderException.CreateOther("missing field: PublicKey"),
            Value = value ?? throw RocketPackDecoderException.CreateOther("missing field: Value"),
        };
    }
}
