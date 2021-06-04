using System;
using System.Buffers;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Serialization;

namespace Omnius.Core.Cryptography
{
    public partial struct OmniHash
    {
        public static OmniHash Create(OmniHashAlgorithmType algorithmType, ReadOnlySpan<byte> message)
        {
            return algorithmType switch
            {
                OmniHashAlgorithmType.Sha2_256 => new OmniHash(algorithmType, Sha2_256.ComputeHash(message)),
                _ => throw new NotSupportedException(),
            };
        }

        public bool Validate(Span<byte> message)
        {
            switch (this.AlgorithmType)
            {
                case OmniHashAlgorithmType.Sha2_256:
                    Span<byte> v = stackalloc byte[32];
                    Sha2_256.TryComputeHash(message, v);
                    return BytesOperations.Equals(this.Value.Span, v);
                default:
                    return false;
            }
        }

        public string ToString(ConvertStringType convertStringType, ConvertStringCase convertStringCase = ConvertStringCase.Lower)
        {
            var algorithmType = this.AlgorithmType switch
            {
                OmniHashAlgorithmType.Sha2_256 => "sha2-256",
                _ => throw new NotSupportedException()
            };
            var value = OmniBase.Encode(new ReadOnlySequence<byte>(this.Value), convertStringType, convertStringCase);

            return algorithmType + ":" + value;
        }

        public static bool TryParse(string text, out OmniHash value)
        {
            value = default;

            var hub = new BytesHub();
            if (!OmniBase.TryDecode(text, hub.Writer)) return false;

            value = Import(hub.Reader.GetSequence(), BytesPool.Shared);
            return true;
        }
    }
}
