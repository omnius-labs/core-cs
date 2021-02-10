using System;
using System.Buffers;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Cryptography.Internal;

namespace Omnius.Core.Cryptography
{
    public sealed partial class OmniCertificate
    {
        public static OmniCertificate Create(OmniDigitalSignature digitalSignature, ReadOnlySequence<byte> sequence)
        {
            if (digitalSignature is null) throw new ArgumentNullException(nameof(digitalSignature));

            ReadOnlyMemory<byte> value;

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

        private OmniSignature? _signature;

        public OmniSignature GetOmniSignature()
        {
            return _signature ?? (_signature = SignatureHelper.GetOmniSignature(this));
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
