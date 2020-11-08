using System;
using System.Buffers;
using System.Security.Cryptography;

namespace Omnius.Core.Cryptography.Functions
{
    public static class EcDsa_P521_Sha2_256
    {
        public static (byte[] publicKey, byte[] privateKey) CreateKeys()
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.GenerateKey(ECCurve.NamedCurves.nistP521);
            var publicKey = ecdsa.ExportSubjectPublicKeyInfo();
            var privateKey = ecdsa.ExportPkcs8PrivateKey();
            return (publicKey, privateKey);
        }

        public static byte[] Sign(ReadOnlyMemory<byte> privateKey, ReadOnlySequence<byte> sequence)
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportPkcs8PrivateKey(privateKey.Span, out var _);
            return ecdsa.SignHash(Sha2_256.ComputeHash(sequence));
        }

        public static bool Verify(ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> signature, ReadOnlySequence<byte> sequence)
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportSubjectPublicKeyInfo(publicKey.Span, out var _);
            return ecdsa.VerifyHash(Sha2_256.ComputeHash(sequence).AsSpan(), signature.Span);
        }
    }
}
