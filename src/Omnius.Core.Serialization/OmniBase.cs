using System;
using System.Buffers;
using Omnius.Core.Serialization.Extensions;

namespace Omnius.Core.Serialization
{
    public static class OmniBase
    {
        private static readonly Lazy<Base16> _base16_Lower = new Lazy<Base16>(() => new Base16(ConvertStringCase.Lower));
        private static readonly Lazy<Base16> _base16_Upper = new Lazy<Base16>(() => new Base16(ConvertStringCase.Upper));
        private static readonly Lazy<Base58Btc> _base58Btc = new Lazy<Base58Btc>(() => new Base58Btc());

        public static string Encode(ReadOnlySpan<byte> span, ConvertStringType convertStringType, ConvertStringCase convertStringCase = ConvertStringCase.Lower)
        {
            if (convertStringType == ConvertStringType.Base16)
            {
                switch (convertStringCase)
                {
                    case ConvertStringCase.Lower:
                        {
                            _base16_Lower.Value.TryEncode(span, out string text, true);
                            return text;
                        }
                    case ConvertStringCase.Upper:
                        {
                            _base16_Upper.Value.TryEncode(span, out string text, true);
                            return text;
                        }
                    default:
                        throw new NotSupportedException(nameof(convertStringCase));
                }
            }
            else if (convertStringType == ConvertStringType.Base58)
            {
                _base58Btc.Value.TryEncode(span, out string text, true);
                return text;
            }
            else
            {
                throw new NotSupportedException(nameof(convertStringType));
            }
        }

        public static string Encode(ReadOnlySequence<byte> sequence, ConvertStringType convertStringType, ConvertStringCase convertStringCase = ConvertStringCase.Lower)
        {
            if (convertStringType == ConvertStringType.Base16)
            {
                switch (convertStringCase)
                {
                    case ConvertStringCase.Lower:
                        {
                            _base16_Lower.Value.TryEncode(sequence, out string text, true);
                            return text;
                        }
                    case ConvertStringCase.Upper:
                        {
                            _base16_Upper.Value.TryEncode(sequence, out string text, true);
                            return text;
                        }
                    default:
                        throw new NotSupportedException(nameof(convertStringCase));
                }
            }
            else if (convertStringType == ConvertStringType.Base58)
            {
                _base58Btc.Value.TryEncode(sequence, out string text, true);
                return text;
            }
            else
            {
                throw new NotSupportedException(nameof(convertStringType));
            }
        }

        // TODO Utf8String版を実装したい
        public static bool TryDecode(string text, IBufferWriter<byte> bufferWriter)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            switch (text[0])
            {
                case 'f':
                    _base16_Lower.Value.TryDecode(text.Substring(1), bufferWriter);
                    return true;
                case 'F':
                    _base16_Upper.Value.TryDecode(text.Substring(1), bufferWriter);
                    return true;
                case 'z':
                    _base58Btc.Value.TryDecode(text.Substring(1), bufferWriter);
                    return true;
                default:
                    return false;
            }
        }
    }
}
