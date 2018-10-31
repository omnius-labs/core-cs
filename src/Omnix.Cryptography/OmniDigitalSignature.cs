using System;
using System.Buffers;
using System.IO;
using Omnix.Cryptography.Internal;

namespace Omnix.Cryptography
{
    public sealed partial class OmniDigitalSignature
    {
        private volatile OmniSignature _omniSignature;

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
            if (_omniSignature == null)
                _omniSignature = SignatureHelper.GetSignature(this);

            return _omniSignature;
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
