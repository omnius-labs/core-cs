using System.Buffers;
using System.Text.Json;
using Omnius.Core.Base;
using Omnius.Core.Omnikit.Internal;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit;

// ref. https://github.com/omnius-labs/core-rs/blob/6ac4b87f9ec6c6de4df4d8c2aa9cb9fa7568863a/modules/omnikit/src/model/omni_agreement.rs

[Flags]
public enum OmniAgreementAlgorithmType
{
    None = 0,
    X25519 = 1
}

public class OmniAgreement : IRocketPackStruct<OmniAgreement>, IEquatable<OmniAgreement>
{
    public static OmniAgreement Create(DateTime createdTime, OmniAgreementAlgorithmType algorithmType)
    {
        if (algorithmType == OmniAgreementAlgorithmType.X25519)
        {
            var (publicKey, secretKey) = X25519.CreateKeys();
            return new OmniAgreement
            {
                CreatedTime = createdTime,
                AlgorithmType = algorithmType,
                PublicKey = publicKey,
                SecretKey = secretKey
            };
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public required DateTime CreatedTime { get; init; }
    public required OmniAgreementAlgorithmType AlgorithmType { get; init; }
    public required byte[] PublicKey { get; init; }
    public required byte[] SecretKey { get; init; }

    public OmniAgreementPublicKey GenAgreementPublicKey()
    {
        return new OmniAgreementPublicKey
        {
            CreatedTime = this.CreatedTime,
            AlgorithmType = this.AlgorithmType,
            PublicKey = this.PublicKey
        };
    }

    public OmniAgreementPrivateKey GenAgreementPrivateKey()
    {
        return new OmniAgreementPrivateKey
        {
            CreatedTime = this.CreatedTime,
            AlgorithmType = this.AlgorithmType,
            SecretKey = this.SecretKey
        };
    }

    public static byte[] GenSecret(OmniAgreementPrivateKey privateKey, OmniAgreementPublicKey publicKey)
    {
        if (publicKey.AlgorithmType == OmniAgreementAlgorithmType.X25519)
        {
            return X25519.GetSecret(publicKey.PublicKey, privateKey.SecretKey);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(this.CreatedTime);
            h.Add(this.AlgorithmType);
            h.Add(this.PublicKey);
            h.Add(this.SecretKey);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public bool Equals(OmniAgreement? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.CreatedTime == other.CreatedTime && this.AlgorithmType == other.AlgorithmType && this.PublicKey.SequenceEqual(other.PublicKey) && this.SecretKey.SequenceEqual(other.SecretKey);
    }

    public static void Pack(ref RocketPackBytesEncoder encoder, in OmniAgreement value)
    {
        encoder.WriteMap(4);

        encoder.WriteU64(0);
        encoder.WriteString(value.AlgorithmType.ToString().ToLowerInvariant());

        encoder.WriteU64(1);
        encoder.WriteBytes(value.SecretKey);

        encoder.WriteU64(2);
        encoder.WriteBytes(value.PublicKey);

        encoder.WriteU64(3);
        encoder.WriteStruct(Timestamp64.FromDateTime(value.CreatedTime.ToUniversalTime()));
    }

    public static OmniAgreement Unpack(ref RocketPackBytesDecoder decoder)
    {
        var count = decoder.ReadMap();

        OmniAgreementAlgorithmType? algorithmType = null;
        byte[]? secretKey = null;
        byte[]? publicKey = null;
        DateTime? createdTime = null;

        for (ulong i = 0; i < count; i++)
        {
            switch (decoder.ReadU64())
            {
                case 0:
                    algorithmType = Enum.Parse<OmniAgreementAlgorithmType>(decoder.ReadString(), true);
                    break;
                case 1:
                    secretKey = decoder.ReadBytesToArray();
                    break;
                case 2:
                    publicKey = decoder.ReadBytesToArray();
                    break;
                case 3:
                    createdTime = decoder.ReadStruct<Timestamp64>().ToDateTime();
                    break;
                default:
                    decoder.SkipField();
                    break;
            }
        }

        return new OmniAgreement()
        {
            AlgorithmType = algorithmType ?? throw RocketPackDecoderException.CreateOther("missing field: AlgorithmType"),
            SecretKey = secretKey ?? throw RocketPackDecoderException.CreateOther("missing field: SecretKey"),
            PublicKey = publicKey ?? throw RocketPackDecoderException.CreateOther("missing field: PublicKey"),
            CreatedTime = createdTime ?? throw RocketPackDecoderException.CreateOther("missing field: CreatedTime"),
        };
    }
}

public class OmniAgreementPublicKey : IRocketPackStruct<OmniAgreementPublicKey>, IEquatable<OmniAgreementPublicKey>
{
    public required DateTime CreatedTime { get; init; }
    public required OmniAgreementAlgorithmType AlgorithmType { get; init; }
    public required byte[] PublicKey { get; init; }

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(this.CreatedTime);
            h.Add(this.AlgorithmType);
            h.Add(this.PublicKey);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public bool Equals(OmniAgreementPublicKey? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.CreatedTime == other.CreatedTime && this.AlgorithmType == other.AlgorithmType && this.PublicKey.SequenceEqual(other.PublicKey);
    }

    public static void Pack(ref RocketPackBytesEncoder encoder, in OmniAgreementPublicKey value)
    {
        encoder.WriteMap(3);

        encoder.WriteU64(0);
        encoder.WriteString(value.AlgorithmType.ToString().ToLowerInvariant());

        encoder.WriteU64(1);
        encoder.WriteBytes(value.PublicKey);

        encoder.WriteU64(2);
        encoder.WriteStruct(Timestamp64.FromDateTime(value.CreatedTime.ToUniversalTime()));
    }

    public static OmniAgreementPublicKey Unpack(ref RocketPackBytesDecoder decoder)
    {
        var count = decoder.ReadMap();

        OmniAgreementAlgorithmType? algorithmType = null;
        byte[]? publicKey = null;
        DateTime? createdTime = null;

        for (ulong i = 0; i < count; i++)
        {
            switch (decoder.ReadU64())
            {
                case 0:
                    algorithmType = Enum.Parse<OmniAgreementAlgorithmType>(decoder.ReadString(), true);
                    break;
                case 1:
                    publicKey = decoder.ReadBytesToArray();
                    break;
                case 2:
                    createdTime = decoder.ReadStruct<Timestamp64>().ToDateTime();
                    break;
                default:
                    decoder.SkipField();
                    break;
            }
        }

        return new OmniAgreementPublicKey()
        {
            AlgorithmType = algorithmType ?? throw RocketPackDecoderException.CreateOther("missing field: AlgorithmType"),
            PublicKey = publicKey ?? throw RocketPackDecoderException.CreateOther("missing field: PublicKey"),
            CreatedTime = createdTime ?? throw RocketPackDecoderException.CreateOther("missing field: CreatedTime"),
        };
    }
}

public class OmniAgreementPrivateKey : IRocketPackStruct<OmniAgreementPrivateKey>, IEquatable<OmniAgreementPrivateKey>
{
    public required DateTime CreatedTime { get; init; }
    public required OmniAgreementAlgorithmType AlgorithmType { get; init; }
    public required byte[] SecretKey { get; init; }

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(this.CreatedTime);
            h.Add(this.AlgorithmType);
            h.Add(this.SecretKey);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public bool Equals(OmniAgreementPrivateKey? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.CreatedTime == other.CreatedTime && this.AlgorithmType == other.AlgorithmType && this.SecretKey.SequenceEqual(other.SecretKey);
    }

    public static void Pack(ref RocketPackBytesEncoder encoder, in OmniAgreementPrivateKey value)
    {
        encoder.WriteMap(3);

        encoder.WriteU64(0);
        encoder.WriteString(value.AlgorithmType.ToString().ToLowerInvariant());

        encoder.WriteU64(1);
        encoder.WriteBytes(value.SecretKey);

        encoder.WriteU64(2);
        encoder.WriteStruct(Timestamp64.FromDateTime(value.CreatedTime.ToUniversalTime()));
    }

    public static OmniAgreementPrivateKey Unpack(ref RocketPackBytesDecoder decoder)
    {
        var count = decoder.ReadMap();

        OmniAgreementAlgorithmType? algorithmType = null;
        byte[]? secretKey = null;
        DateTime? createdTime = null;

        for (ulong i = 0; i < count; i++)
        {
            switch (decoder.ReadU64())
            {
                case 0:
                    algorithmType = Enum.Parse<OmniAgreementAlgorithmType>(decoder.ReadString(), true);
                    break;
                case 1:
                    secretKey = decoder.ReadBytesToArray();
                    break;
                case 2:
                    createdTime = decoder.ReadStruct<Timestamp64>().ToDateTime();
                    break;
                default:
                    decoder.SkipField();
                    break;
            }
        }

        return new OmniAgreementPrivateKey()
        {
            AlgorithmType = algorithmType ?? throw RocketPackDecoderException.CreateOther("missing field: AlgorithmType"),
            SecretKey = secretKey ?? throw RocketPackDecoderException.CreateOther("missing field: SecretKey"),
            CreatedTime = createdTime ?? throw RocketPackDecoderException.CreateOther("missing field: CreatedTime"),
        };
    }
}
