using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Omnix.Algorithms.Internal;

namespace Omnix.Algorithms.Cryptography.Internal
{
    static class EcDh_P521_Sha2_256
    {
        public static (byte[] publicKey, byte[] privateKey) CreateKeys()
        {
            ECParameters ecParameters;

            using (var ecdh = ECDiffieHellman.Create())
            {
                ecdh.GenerateKey(ECCurve.NamedCurves.nistP521);
                ecParameters = ecdh.ExportParameters(true);
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

        public static byte[] GetSecret(ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> privateKey)
        {
            ECParameters ecParameters;
            {
                var plist = SerializeHelper.Decode(privateKey).ToArray();

                ecParameters = new ECParameters()
                {
                    Curve = ECCurve.NamedCurves.nistP521,
                    Q = new ECPoint()
                    {
                        X = plist[0].ToArray(),
                        Y = plist[1].ToArray(),
                    },
                    D = plist[2].ToArray(),
                };
            }

            ECDiffieHellmanPublicKey ecDiffieHellmanPublicKey;
            {
                var plist = SerializeHelper.Decode(publicKey).ToArray();

                ecDiffieHellmanPublicKey = new CustomECDiffieHellmanPublicKey(
                    ECCurve.NamedCurves.nistP521,
                    new ECPoint()
                    {
                        X = plist[0].ToArray(),
                        Y = plist[1].ToArray(),
                    });
            }

            using (var ecdh = ECDiffieHellman.Create())
            {
                ecdh.ImportParameters(ecParameters);
                return ecdh.DeriveKeyFromHash(ecDiffieHellmanPublicKey, HashAlgorithmName.SHA256);
            }
        }

        public sealed partial class CustomECDiffieHellmanPublicKey : ECDiffieHellmanPublicKey
        {
            private readonly ECCurve _ecCurve;
            private readonly ECPoint _ecPoint;

            public CustomECDiffieHellmanPublicKey(ECCurve ecCurve, ECPoint ecPoint)
            {
                _ecCurve = ecCurve;
                _ecPoint = ecPoint;
            }

            public override ECParameters ExportExplicitParameters()
            {
                var ecParams = new ECParameters();
                ecParams.Q = _ecPoint;
                return ecParams;
            }

            public override ECParameters ExportParameters()
            {
                var ecParams = new ECParameters();
                ecParams.Curve = _ecCurve;
                ecParams.Q = _ecPoint;
                return ecParams;
            }
        }
    }
}
