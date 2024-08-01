using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Omnius.Core.Omnikit.Converters;
using Omnius.Core.Omnikit.Internal;

namespace Omnius.Core.Omnikit;

[Flags]
public enum OmniSignType
{
    None = 0,
    Ed25519_Sha3_256_Base64Url = 1
}

public class OmniSigner
{
    public OmniSignType Type { get; init; }
    public string Name { get; init; }
    public byte[] Key { get; init; }

    public OmniSigner(OmniSignType type, string name)
    {
        this.Type = type;
        this.Name = name;

        if (type == OmniSignType.Ed25519_Sha3_256_Base64Url)
        {
            var signingKey = Ed25519.CreateSigningKey();
            this.Key = Ed25519.GetPrivateKeyPkcs8Der(signingKey);
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
}

public class OmniCert
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
        else
        {
            throw new NotSupportedException("Unsupported sign type");
        }
    }

    public override string ToString()
    {
        if (this.Type == OmniSignType.Ed25519_Sha3_256_Base64Url)
        {
            var hash = Sha3_256.ComputeHash(this.PublicKey);
            return $"{this.Name}@{Base64Url.Instance.BytesToString(hash)}";
        }
        else
        {
            return string.Empty;
        }
    }
}
