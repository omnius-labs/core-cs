using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Omnix.Cryptography.Internal;

namespace Omnix.Cryptography.Internal
{
    static class EcDsa_P521_Sha2_256
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
                using (var ecdsa = new ECDsaCng(ck))
                {
                    ecParameters = ecdsa.ExportParameters(true);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using (var ecdsa = new ECDsaOpenSsl())
                {
                    ecdsa.GenerateKey(ECCurve.NamedCurves.nistP521);
                    ecParameters = ecdsa.ExportParameters(true);
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (var ecdsa = new ECDsaCng())
                {
                    ecdsa.ImportParameters(ecParameters);
                    return ecdsa.SignHash(Sha2_256.ComputeHash(sequence).ToArray());
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using (var ecdsa = new ECDsaOpenSsl())
                {
                    ecdsa.ImportParameters(ecParameters);
                    return ecdsa.SignHash(Sha2_256.ComputeHash(sequence).ToArray());
                }
            }
            else
            {
                throw new NotSupportedException();
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    using (var ecdsa = new ECDsaCng())
                    {
                        ecdsa.ImportParameters(ecParameters);
                        return ecdsa.VerifyHash(Sha2_256.ComputeHash(sequence).Span, signature.Span);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    using (var ecdsa = new ECDsaOpenSsl())
                    {
                        ecdsa.ImportParameters(ecParameters);
                        return ecdsa.VerifyHash(Sha2_256.ComputeHash(sequence).Span, signature.Span);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
