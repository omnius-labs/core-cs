using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Omnius.Core.Omnikit.Internal;

internal static class Ed25519
{
    public static Ed25519PrivateKeyParameters CreateSigningKey()
    {
        var keyPairGenerator = new Ed25519KeyPairGenerator();
        keyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 256));
        var keyPair = keyPairGenerator.GenerateKeyPair();

        return (Ed25519PrivateKeyParameters)keyPair.Private;
    }

    public static byte[] GetPrivateKeyPkcs8Der(Ed25519PrivateKeyParameters privateKey)
    {
        var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
        return privateKeyInfo.GetDerEncoded();
    }

    public static Ed25519PrivateKeyParameters ParsePrivateKeyPkcs8Der(ReadOnlyMemory<byte> privateKeyPkcs8)
    {
        var privateKeyInfo = PrivateKeyInfo.GetInstance(privateKeyPkcs8.ToArray());
        var privateKeyAsn1Object = (Asn1OctetString)privateKeyInfo.ParsePrivateKey();
        return new Ed25519PrivateKeyParameters(privateKeyAsn1Object.GetOctets(), 0);
    }

    public static byte[] GetPublicKeyDer(Ed25519PublicKeyParameters publicKey)
    {
        return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey).GetDerEncoded();
    }

    public static Ed25519PublicKeyParameters ParsePublicKeyDer(ReadOnlyMemory<byte> publicKey)
    {
        var subjectPublicKeyInfo = SubjectPublicKeyInfo.GetInstance(publicKey.ToArray());
        var publicKeyBytes = subjectPublicKeyInfo.PublicKeyData.GetBytes();
        return new Ed25519PublicKeyParameters(publicKeyBytes, 0);
    }

    public static byte[] Sign(ReadOnlyMemory<byte> privateKey, ReadOnlyMemory<byte> value)
    {
        var ed25519PrivateKeyParameters = ParsePrivateKeyPkcs8Der(privateKey.ToArray());

        var signer = new Ed25519Signer();
        signer.Init(true, ed25519PrivateKeyParameters);
        signer.BlockUpdate(value.ToArray(), 0, value.Length);
        return signer.GenerateSignature();
    }

    public static bool Verify(ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> signature, ReadOnlyMemory<byte> value)
    {
        var ed25519PublicKeyParameters = ParsePublicKeyDer(publicKey);

        var verifier = new Ed25519Signer();
        verifier.Init(false, ed25519PublicKeyParameters);
        verifier.BlockUpdate(value.ToArray(), 0, value.Length);
        return verifier.VerifySignature(signature.ToArray());
    }
}
