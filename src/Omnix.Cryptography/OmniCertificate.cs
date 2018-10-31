using System;
using System.Buffers;
using Omnix.Cryptography.Internal;

namespace Omnix.Cryptography
{
    public sealed partial class OmniCertificate
    {
        public static OmniCertificate Create(OmniDigitalSignature digitalSignature, ReadOnlySequence<byte> sequence)
        {
            if (digitalSignature == null) throw new ArgumentNullException(nameof(digitalSignature));

            byte[] value = null;

            if (digitalSignature.AlgorithmType == OmniDigitalSignatureAlgorithmType.EcDsa_P521_Sha2_256)
            {
                value = EcDsa_P521_Sha2_256.Sign(digitalSignature.PrivateKey, sequence);
            }
            else
            {
                throw new NotSupportedException(nameof(digitalSignature.AlgorithmType));
            }

            return new OmniCertificate(digitalSignature.Name, digitalSignature.AlgorithmType, digitalSignature.PublicKey, value);
        }

        public override string ToString()
        {
            return this.GetOmniSignature().ToString();
        }

        private volatile OmniSignature _omniSignature;

        public OmniSignature GetOmniSignature()
        {
            if (_omniSignature == null)
                _omniSignature = SignatureHelper.GetSignature(this);

            return _omniSignature;
        }

        public bool Verify(ReadOnlySequence<byte> sequence)
        {
            if (this.AlgorithmType == OmniDigitalSignatureAlgorithmType.EcDsa_P521_Sha2_256)
            {
                return EcDsa_P521_Sha2_256.Verify(this.PublicKey, this.Value, sequence);
            }
            else
            {
                return false;
            }
        }
    }
}
