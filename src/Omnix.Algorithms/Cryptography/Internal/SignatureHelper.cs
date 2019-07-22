using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Omnix.Base;
using Omnix.Io;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;

namespace Omnix.Algorithms.Cryptography.Internal
{
    static class SignatureHelper
    {
        private static readonly ThreadLocal<Encoding> _encoding = new ThreadLocal<Encoding>(() => new UTF8Encoding(false));

        private static OmniHash CreateOmniHash(string name, ReadOnlySpan<byte> publicKey, OmniHashAlgorithmType hashAlgorithmType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (var hub = new Hub())
            {
                {
                    var writer = new RocketPackWriter(hub.Writer, BufferPool.Shared);

                    writer.Write(name);
                    writer.Write(publicKey);

                    hub.Writer.Complete();
                }

                if (hashAlgorithmType == OmniHashAlgorithmType.Sha2_256)
                {
                    var result = new OmniHash(hashAlgorithmType, Sha2_256.ComputeHash(hub.Reader.GetSequence()));
                    hub.Reader.Complete();

                    return result;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public static OmniSignature GetOmniSignature(OmniDigitalSignature digitalSignature)
        {
            if (digitalSignature is null)
            {
                throw new ArgumentNullException(nameof(digitalSignature));
            }

            if (digitalSignature.Name == null)
            {
                throw new ArgumentNullException(nameof(digitalSignature.Name));
            }

            if (digitalSignature.AlgorithmType == OmniDigitalSignatureAlgorithmType.EcDsa_P521_Sha2_256)
            {
                return new OmniSignature(digitalSignature.Name, CreateOmniHash(digitalSignature.Name, digitalSignature.PublicKey.Span, OmniHashAlgorithmType.Sha2_256));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static OmniSignature GetOmniSignature(OmniCertificate certificate)
        {
            if (certificate is null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            if (certificate.Name == null)
            {
                throw new ArgumentNullException(nameof(certificate.Name));
            }

            if (certificate.AlgorithmType == OmniDigitalSignatureAlgorithmType.EcDsa_P521_Sha2_256)
            {
                return new OmniSignature(certificate.Name, CreateOmniHash(certificate.Name, certificate.PublicKey.Span, OmniHashAlgorithmType.Sha2_256));
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
