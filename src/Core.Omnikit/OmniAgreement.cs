using System.Buffers;
using System.Text.Json;
using Omnius.Core.Base;
using Omnius.Core.Omnikit.Internal;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;
using Omnius.Core.Serialization;

namespace Omnius.Core.Omnikit;

// ref. https://github.com/omnius-labs/core-rs/blob/6ac4b87f9ec6c6de4df4d8c2aa9cb9fa7568863a/modules/omnikit/src/model/omni_agreement.rs

[Flags]
public enum OmniAgreementAlgorithmType
{
    None = 0,
    X25519 = 1
}

public class OmniAgreement : RocketMessage<OmniAgreement>
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

    public override bool Equals(OmniAgreement? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.CreatedTime == other.CreatedTime && this.AlgorithmType == other.AlgorithmType && this.PublicKey.SequenceEqual(other.PublicKey) && this.SecretKey.SequenceEqual(other.SecretKey);
    }

    static OmniAgreement()
    {
        Formatter = new CustomSerializer();
        Empty = new OmniAgreement() { CreatedTime = DateTime.MinValue, AlgorithmType = OmniAgreementAlgorithmType.None, PublicKey = Array.Empty<byte>(), SecretKey = Array.Empty<byte>() };
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<OmniAgreement>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in OmniAgreement value, scoped in int depth)
        {
            w.Put(Timestamp64.FromDateTime(value.CreatedTime));
            w.Put(value.AlgorithmType.ToString());
            w.Put(value.PublicKey);
            w.Put(value.SecretKey);
        }
        public OmniAgreement Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var createdTime = r.GetTimestamp64().ToDateTime();
            var algorithmType = Enum.Parse<OmniAgreementAlgorithmType>(r.GetString(1024));
            var publicKey = r.GetBytes(1024);
            var secretKey = r.GetBytes(1024);

            return new OmniAgreement()
            {
                CreatedTime = createdTime,
                AlgorithmType = algorithmType,
                PublicKey = publicKey,
                SecretKey = secretKey
            };
        }
    }
}

public class OmniAgreementPublicKey : RocketMessage<OmniAgreementPublicKey>
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

    public override bool Equals(OmniAgreementPublicKey? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.CreatedTime == other.CreatedTime && this.AlgorithmType == other.AlgorithmType && this.PublicKey.SequenceEqual(other.PublicKey);
    }

    static OmniAgreementPublicKey()
    {
        Formatter = new CustomSerializer();
        Empty = new OmniAgreementPublicKey() { CreatedTime = DateTime.MinValue, AlgorithmType = OmniAgreementAlgorithmType.None, PublicKey = Array.Empty<byte>() };
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<OmniAgreementPublicKey>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in OmniAgreementPublicKey value, scoped in int depth)
        {
            w.Put(Timestamp64.FromDateTime(value.CreatedTime));
            w.Put(value.AlgorithmType.ToString());
            w.Put(value.PublicKey);
        }
        public OmniAgreementPublicKey Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var createdTime = r.GetTimestamp64().ToDateTime();
            var algorithmType = Enum.Parse<OmniAgreementAlgorithmType>(r.GetString(1024));
            var publicKey = r.GetBytes(1024);

            return new OmniAgreementPublicKey()
            {
                CreatedTime = createdTime,
                AlgorithmType = algorithmType,
                PublicKey = publicKey,
            };
        }
    }
}

public class OmniAgreementPrivateKey : RocketMessage<OmniAgreementPrivateKey>
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

    public override bool Equals(OmniAgreementPrivateKey? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.CreatedTime == other.CreatedTime && this.AlgorithmType == other.AlgorithmType && this.SecretKey.SequenceEqual(other.SecretKey);
    }

    static OmniAgreementPrivateKey()
    {
        Formatter = new CustomSerializer();
        Empty = new OmniAgreementPrivateKey() { CreatedTime = DateTime.MinValue, AlgorithmType = OmniAgreementAlgorithmType.None, SecretKey = Array.Empty<byte>() };
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<OmniAgreementPrivateKey>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in OmniAgreementPrivateKey value, scoped in int depth)
        {
            w.Put(Timestamp64.FromDateTime(value.CreatedTime));
            w.Put(value.AlgorithmType.ToString());
            w.Put(value.SecretKey);
        }
        public OmniAgreementPrivateKey Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var createdTime = r.GetTimestamp64().ToDateTime();
            var algorithmType = Enum.Parse<OmniAgreementAlgorithmType>(r.GetString(1024));
            var secretKey = r.GetBytes(1024);

            return new OmniAgreementPrivateKey()
            {
                CreatedTime = createdTime,
                AlgorithmType = algorithmType,
                SecretKey = secretKey
            };
        }
    }
}
