using System;
using System.Buffers;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Cryptography.Internal;

namespace Omnius.Core.Cryptography
{
    public sealed partial class OmniDigitalSignature
    {
        private OmniSignature? _signature;

        public static OmniDigitalSignature Create(string name, OmniDigitalSignatureAlgorithmType algorithmType)
        {
            if (algorithmType == OmniDigitalSignatureAlgorithmType.EcDsa_P521_Sha2_256)
            {
                var (publicKey, privateKey) = EcDsa_P521_Sha2_256.CreateKeys();
                return new OmniDigitalSignature(name, algorithmType, publicKey, privateKey);
            }
            else
            {
                throw new NotSupportedException(nameof(algorithmType));
            }
        }

        public override string ToString()
        {
            return this.GetOmniSignature().ToString();
        }

        public OmniSignature GetOmniSignature()
        {
            return _signature ?? (_signature = SignatureHelper.GetOmniSignature(this));
        }

        public static OmniCertificate CreateOmniCertificate(OmniDigitalSignature digitalSignature, ReadOnlySequence<byte> sequence)
        {
            return OmniCertificate.Create(digitalSignature, sequence);
        }

        public static bool VerifyOmniCertificate(OmniCertificate certificate, ReadOnlySequence<byte> sequence)
        {
            return certificate.Verify(sequence);
        }
    }
}
