using System;
using System.Security.Cryptography;

namespace Omnius.Core.Cryptography.Functions
{
    public static class EcDh_P521_Sha2_256
    {
        public static (byte[] publicKey, byte[] privateKey) CreateKeys()
        {
            using var ecdh = ECDiffieHellman.Create();
            ecdh.GenerateKey(ECCurve.NamedCurves.nistP521);
            var publicKey = ecdh.ExportSubjectPublicKeyInfo();
            var privateKey = ecdh.ExportPkcs8PrivateKey();
            return (publicKey, privateKey);
        }

        public static byte[] GetSecret(ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> privateKey)
        {
            using var ecdhPublicKey = ECDiffieHellman.Create();
            ecdhPublicKey.ImportSubjectPublicKeyInfo(publicKey.Span, out var _);
            using var ecdhPrivateKey = ECDiffieHellman.Create();
            ecdhPrivateKey.ImportPkcs8PrivateKey(privateKey.Span, out var _);
            return ecdhPrivateKey.DeriveKeyFromHash(ecdhPublicKey.PublicKey, HashAlgorithmName.SHA256);
        }
    }
}
