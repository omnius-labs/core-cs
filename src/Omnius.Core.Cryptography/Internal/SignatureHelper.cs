using System.Text;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Cryptography.Internal;

internal static class SignatureHelper
{
    private static readonly ThreadLocal<Encoding> _encoding = new ThreadLocal<Encoding>(() => new UTF8Encoding(false));

    private static OmniHash CreateOmniHash(Utf8String name, ReadOnlySpan<byte> publicKey, OmniHashAlgorithmType hashAlgorithmType)
    {
        using var bytesPipe = new BytesPipe();
        {
            var writer = new RocketMessageWriter(bytesPipe.Writer, BytesPool.Shared);

            writer.Write(name);
            writer.Write(publicKey);
        }

        if (hashAlgorithmType == OmniHashAlgorithmType.Sha2_256)
        {
            var result = new OmniHash(hashAlgorithmType, Sha2_256.ComputeHash(bytesPipe.Reader.GetSequence()));

            return result;
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public static OmniSignature GetOmniSignature(OmniDigitalSignature digitalSignature)
    {
        if (digitalSignature is null) throw new ArgumentNullException(nameof(digitalSignature));

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
        if (certificate is null) throw new ArgumentNullException(nameof(certificate));

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
