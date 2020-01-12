using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Omnius.Core.Internal;

namespace Omnius.Core.Cryptography.Internal
{
    internal static class EcDsa_P521_Sha2_256
    {
        public static (byte[] publicKey, byte[] privateKey) CreateKeys()
        {
            ECParameters ecParameters;

            using (var ecdsa = ECDsa.Create())
            {
                ecdsa.GenerateKey(ECCurve.NamedCurves.nistP521);
                ecParameters = ecdsa.ExportParameters(true);
            }

            byte[] publicKey;
            {
                var plist = new List<ReadOnlyMemory<byte>>()
                {
                    ecParameters.Q.X.ToArray(),
                    ecParameters.Q.Y.ToArray(),
                };

                publicKey = SerializeHelper.Encode(plist);
            }

            byte[] privateKey;
            {
                var plist = new List<ReadOnlyMemory<byte>>()
                {
                    ecParameters.Q.X.ToArray(),
                    ecParameters.Q.Y.ToArray(),
                    ecParameters.D.ToArray(),
                };

                privateKey = SerializeHelper.Encode(plist);
            }

            return (publicKey, privateKey);
        }

        public static byte[] Sign(ReadOnlyMemory<byte> privateKey, ReadOnlySequence<byte> sequence)
        {
            ECParameters ecParameters;
            {
                var plist = SerializeHelper.Decode(privateKey).ToArray();

                ecParameters = new ECParameters()
                {
                    Q = new ECPoint()
                    {
                        X = plist[0].ToArray(),
                        Y = plist[1].ToArray(),
                    },
                    D = plist[2].ToArray(),
                };
            }

            using (var ecdsa = ECDsa.Create())
            {
                ecdsa.ImportParameters(ecParameters);
                return ecdsa.SignHash(Sha2_256.ComputeHash(sequence));
            }
        }

        public static bool Verify(ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> signature, ReadOnlySequence<byte> sequence)
        {
            ECParameters ecParameters;
            {
                var plist = SerializeHelper.Decode(publicKey).ToArray();

                ecParameters = new ECParameters()
                {
                    Q = new ECPoint()
                    {
                        X = plist[0].ToArray(),
                        Y = plist[1].ToArray(),
                    },
                };
            }

            using (var ecdsa = ECDsa.Create())
            {
                ecdsa.ImportParameters(ecParameters);
                return ecdsa.VerifyHash(Sha2_256.ComputeHash(sequence).AsSpan(), signature.Span);
            }
        }
    }
}
