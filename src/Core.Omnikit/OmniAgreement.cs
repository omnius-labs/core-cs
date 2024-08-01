using System;
using System.Collections.Generic;
using System.Text;
using Omnius.Core.Omnikit.Internal;

namespace Omnius.Core.Omnikit;

// ref. https://github.com/omnius-labs/core-rs/blob/6ac4b87f9ec6c6de4df4d8c2aa9cb9fa7568863a/modules/omnikit/src/model/omni_agreement.rs

[Flags]
public enum OmniAgreementAlgorithmType
{
    None = 0,
    EcDhP256 = 1
}

public class OmniAgreement
{
    public OmniAgreement(DateTime createdTime, OmniAgreementAlgorithmType algorithmType)
    {
        this.CreatedTime = createdTime;
        this.AlgorithmType = algorithmType;

        if (algorithmType == OmniAgreementAlgorithmType.EcDhP256)
        {
            (this.PublicKey, this.SecretKey) = X25519.CreateKeys();
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public DateTime CreatedTime { get; init; }
    public OmniAgreementAlgorithmType AlgorithmType { get; init; }
    public byte[] PublicKey { get; init; }
    public byte[] SecretKey { get; init; }

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
        if (publicKey.AlgorithmType == OmniAgreementAlgorithmType.EcDhP256)
        {
            return X25519.GetSecret(publicKey.PublicKey, privateKey.SecretKey);
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}

public record class OmniAgreementPublicKey
{
    public required DateTime CreatedTime { get; init; }
    public required OmniAgreementAlgorithmType AlgorithmType { get; init; }
    public required byte[] PublicKey { get; init; }
}

public record class OmniAgreementPrivateKey
{
    public required DateTime CreatedTime { get; init; }
    public required OmniAgreementAlgorithmType AlgorithmType { get; init; }
    public required byte[] SecretKey { get; init; }
}
