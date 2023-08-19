using System.Buffers;

namespace Omnius.Core.Serialization;

// example: https://github.com/multiformats/multibase#multibase-table

public static class OmniBase
{
    private static readonly Lazy<Base16> _base16_Lower = new(() => new Base16(Base16Case.Lower));
    private static readonly Lazy<Base16> _base16_Upper = new(() => new Base16(Base16Case.Upper));
    private static readonly Lazy<Base58Btc> _base58_Btc = new(() => new Base58Btc());

    public static string? Encode(ReadOnlyMemory<byte> memory, ConvertStringType convertStringType)
    {
        return Encode(new ReadOnlySequence<byte>(memory), convertStringType);
    }

    public static string? Encode(ReadOnlySequence<byte> sequence, ConvertStringType convertStringType)
    {
        if (convertStringType == ConvertStringType.Base16Lower)
        {
            _base16_Lower.Value.TryEncode(sequence, out string? text, true);
            return text;
        }
        else if (convertStringType == ConvertStringType.Base16Upper)
        {
            _base16_Upper.Value.TryEncode(sequence, out string? text, true);
            return text;
        }
        else if (convertStringType == ConvertStringType.Base58Btc)
        {
            _base58_Btc.Value.TryEncode(sequence, out string? text, true);
            return text;
        }
        else if (convertStringType == ConvertStringType.Base64)
        {
            var bytes = sequence.ToArray();
            return "m" + Convert.ToBase64String(bytes);
        }
        else
        {
            throw new NotSupportedException(nameof(convertStringType));
        }
    }

    // TODO Utf8String版を実装したい
    public static bool TryDecode(string text, IBufferWriter<byte> bufferWriter)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));

        if (string.IsNullOrEmpty(text)) return true;

        switch (text[0])
        {
            case 'f':
                _base16_Lower.Value.TryDecode(text[1..], bufferWriter);
                return true;
            case 'F':
                _base16_Upper.Value.TryDecode(text[1..], bufferWriter);
                return true;
            case 'z':
                _base58_Btc.Value.TryDecode(text[1..], bufferWriter);
                return true;
            case 'm':
                bufferWriter.Write(Convert.FromBase64String(text[1..]));
                return true;
            default:
                return false;
        }
    }
}
