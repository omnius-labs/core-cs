using System.Buffers;
using System.Text.Json;
using Omnius.Core.Base;
using Omnius.Core.Omnikit.Converters;
using Omnius.Core.Omnikit.Internal;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;
using Omnius.Core.Serialization;

namespace Omnius.Core.Omnikit;

// ref. https://github.com/omnius-labs/core-rs/blob/6ac4b87f9ec6c6de4df4d8c2aa9cb9fa7568863a/modules/omnikit/src/model/omni_sign.rs

[Flags]
public enum OmniSignType
{
    None = 0,
    Ed25519_Sha3_256_Base64Url = 1
}

public class OmniSigner : RocketMessage<OmniSigner>
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

    public override bool Equals(OmniSigner? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Type == other.Type && this.Name == other.Name && this.Key.SequenceEqual(other.Key);
    }

    static OmniSigner()
    {
        Formatter = new CustomSerializer();
        Empty = new OmniSigner() { Name = string.Empty, Type = OmniSignType.None, Key = Array.Empty<byte>() };
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<OmniSigner>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in OmniSigner value, scoped in int depth)
        {
            w.Put(value.Type.ToString());
            w.Put(value.Name);
            w.Put(value.Key);
        }
        public OmniSigner Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var type = Enum.Parse<OmniSignType>(r.GetString(1024));
            var name = r.GetString(1024);
            var key = r.GetMemory(1024).ToArray();

            return new OmniSigner()
            {
                Type = type,
                Name = name,
                Key = key,
            };
        }
    }
}

public class OmniCert : RocketMessage<OmniCert>
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

    public override bool Equals(OmniCert? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Type == other.Type && this.Name == other.Name && this.PublicKey.SequenceEqual(other.PublicKey) && this.Value.SequenceEqual(other.Value);
    }

    static OmniCert()
    {
        Formatter = new CustomSerializer();
        Empty = new OmniCert() { Name = string.Empty, Type = OmniSignType.None, PublicKey = Array.Empty<byte>(), Value = Array.Empty<byte>() };
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<OmniCert>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in OmniCert value, scoped in int depth)
        {
            w.Put(value.Type.ToString());
            w.Put(value.Name);
            w.Put(value.PublicKey);
            w.Put(value.Value);
        }
        public OmniCert Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var type = Enum.Parse<OmniSignType>(r.GetString(1024));
            var name = r.GetString(1024);
            var publicKey = r.GetMemory(1024).ToArray();
            var value = r.GetMemory(1024).ToArray();

            return new OmniCert()
            {
                Type = type,
                Name = name,
                PublicKey = publicKey,
                Value = value
            };
        }
    }
}
