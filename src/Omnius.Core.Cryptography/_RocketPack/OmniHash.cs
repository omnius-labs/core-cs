using System;
using Omnius.Core.Serialization;

namespace Omnius.Core.Cryptography
{
    partial struct OmniHash
    {
        public string ToString(ConvertStringType convertStringType, ConvertStringCase convertStringCase = ConvertStringCase.Lower)
        {
            var algorithmType = this.AlgorithmType switch
            {
                OmniHashAlgorithmType.Sha2_256 => "sha2-256",
                _ => throw new NotSupportedException()
            };
            var value = OmniBase.Encode(this.Value.Span, convertStringType, convertStringCase);

            return algorithmType + ":" + value;
        }

        public static bool TryParse(string text, out OmniHash value)
        {
            value = default;

            var hub = new BytesHub();
            if (!OmniBase.TryDecode(text, hub.Writer)) return false;

            value = OmniHash.Import(hub.Reader.GetSequence(), BytesPool.Shared);
            return true;
        }
    }
}
