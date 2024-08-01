using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;

namespace Omnius.Core.Omnikit.Internal;

internal static class X25519
{
    public static (byte[] publicKey, byte[] privateKey) CreateKeys()
    {
        var keyPairGenerator = new X25519KeyPairGenerator();
        keyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 256));

        var keyPair = keyPairGenerator.GenerateKeyPair();

        var privateKeyParams = (X25519PrivateKeyParameters)keyPair.Private;
        var publicKeyParams = (X25519PublicKeyParameters)keyPair.Public;

        var privateKey = privateKeyParams.GetEncoded();
        var publicKey = publicKeyParams.GetEncoded();

        return (publicKey, privateKey);
    }

    public static byte[] GetSecret(ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> privateKey)
    {
        var privateKeyParams = new X25519PrivateKeyParameters(privateKey.ToArray(), 0);
        var publicKeyParams = new X25519PublicKeyParameters(publicKey.ToArray(), 0);

        var agreement = new X25519Agreement();
        agreement.Init(privateKeyParams);
        var secret = new byte[agreement.AgreementSize];
        agreement.CalculateAgreement(publicKeyParams, secret, 0);
        return secret;
    }
}
