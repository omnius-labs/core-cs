using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Omnix.Cryptography.Internal
{
    static class EcDh_P521_Sha2_256
    {
        public static (ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> privateKey) CreateKeys()
        {
            ECParameters ecParameters;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var ckcp = new CngKeyCreationParameters();
                ckcp.ExportPolicy = CngExportPolicies.AllowPlaintextExport;
                ckcp.KeyUsage = CngKeyUsages.Signing;

                using (var ck = CngKey.Create(CngAlgorithm.ECDsaP521, null, ckcp))
                using (var ecdh = new ECDiffieHellmanCng(ck))
                {
                    ecParameters = ecdh.ExportParameters(true);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using (var ecdh = new ECDiffieHellmanOpenSsl())
                {
                    ecdh.GenerateKey(ECCurve.NamedCurves.nistP521);
                    ecParameters = ecdh.ExportParameters(true);
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            ReadOnlyMemory<byte> publicKey;
            {
                var plist = new List<ReadOnlyMemory<byte>>()
                {
                    ecParameters.Q.X.ToArray(),
                    ecParameters.Q.Y.ToArray(),
                };

                publicKey = SerializeHelper.Encode(plist);
            }

            ReadOnlyMemory<byte> privateKey;
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

        public static ReadOnlyMemory<byte> GetSecret(ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> privateKey)
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (var ecdh = new ECDiffieHellmanCng())
                {
                    ecdh.ImportParameters(ecParameters);
                    return ecdh.DeriveKeyFromHash(ecDiffieHellmanPublicKey, HashAlgorithmName.SHA256);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using (var ecdh = new ECDiffieHellmanOpenSsl())
                {
                    ecdh.ImportParameters(ecParameters);
                    return ecdh.DeriveKeyFromHash(ecDiffieHellmanPublicKey, HashAlgorithmName.SHA256);
                }
            }
            else
            {
                throw new NotSupportedException();
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
