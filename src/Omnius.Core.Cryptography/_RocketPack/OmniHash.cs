using System;
using System.Collections.Generic;
using System.Text;
using Omnius.Core.Serialization;
using Omnius.Core.Serialization.Extensions;

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
    }
}
