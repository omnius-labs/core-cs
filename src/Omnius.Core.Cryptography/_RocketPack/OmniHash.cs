using System;
using System.Collections.Generic;
using System.Text;
using Omnius.Core.Serialization;
using Omnius.Core.Serialization.Extensions;

namespace Omnius.Core.Cryptography
{
    /// <summary>
    /// Hashを保管する
    /// </summary>
    partial struct OmniHash
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="convertStringType"></param>
        /// <param name="convertStringCase"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
