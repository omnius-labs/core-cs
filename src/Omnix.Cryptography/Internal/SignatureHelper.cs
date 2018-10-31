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

namespace Omnix.Cryptography.Internal
{
    static class SignatureHelper
    {
        private static readonly ThreadLocal<Encoding> _encoding = new ThreadLocal<Encoding>(() => new UTF8Encoding(false));

        private static OmniHash CreateHash(string name, ReadOnlySpan<byte> publicKey, OmniHashAlgorithmType hashAlgorithmType)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var pipe = new Pipe();

            {
                var writer = new RocketPackWriter(pipe.Writer, BufferPool.Shared);

                writer.Write(name);
                writer.Write(publicKey);

                pipe.Writer.Complete();
            }

            OmniHash omniHash;
            {
                pipe.Reader.TryRead(out var readResult);

                using (var recyclableMemory = MemoryPool<byte>.Shared.Rent((int)readResult.Buffer.Length))
                {
                    readResult.Buffer.CopyTo(recyclableMemory.Memory.Span);

                    if (hashAlgorithmType == OmniHashAlgorithmType.Sha2_256)
                    {
                        using (var sha2_256 = SHA256.Create())
                        {
                            var result = new byte[32];
                            if (!sha2_256.TryComputeHash(recyclableMemory.Memory.Span.Slice((int)readResult.Buffer.Length), result, out _)) throw new Exception();

                            omniHash = new OmniHash(hashAlgorithmType, result);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                pipe.Reader.Complete();
            }

            return omniHash;
        }

        public static OmniSignature GetSignature(OmniDigitalSignature digitalSignature)
        {
            if (digitalSignature == null) throw new ArgumentNullException(nameof(digitalSignature));
            if (digitalSignature.Name == null) throw new ArgumentNullException(nameof(digitalSignature.Name));

            if (digitalSignature.AlgorithmType == OmniDigitalSignatureAlgorithmType.EcDsa_P521_Sha2_256)
            {
                return new OmniSignature(digitalSignature.Name, CreateHash(digitalSignature.Name, digitalSignature.PublicKey.Span, OmniHashAlgorithmType.Sha2_256));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static OmniSignature GetSignature(OmniCertificate certificate)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            if (certificate.Name == null) throw new ArgumentNullException(nameof(certificate.Name));

            if (certificate.AlgorithmType == OmniDigitalSignatureAlgorithmType.EcDsa_P521_Sha2_256)
            {
                return new OmniSignature(certificate.Name, CreateHash(certificate.Name, certificate.PublicKey.Span, OmniHashAlgorithmType.Sha2_256));
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
