using System.Buffers;

namespace Omnius.Core.Omnikit.Converters;

// example: https://github.com/multiformats/multibase#multibase-table

public static class OmniBase
{
    public static string? EncodeByBase16(ReadOnlyMemory<byte> memory)
    {
        return EncodeByBase16(new ReadOnlySequence<byte>(memory));
    }

    public static string? EncodeByBase16(ReadOnlySequence<byte> sequence)
    {
        Base16.Lower.TryEncode(sequence, out string? text, true);
        return text;
    }

    public static string? EncodeByBase16Upper(ReadOnlyMemory<byte> memory)
    {
        return EncodeByBase16Upper(new ReadOnlySequence<byte>(memory));
    }

    public static string? EncodeByBase16Upper(ReadOnlySequence<byte> sequence)
    {
        Base16.Upper.TryEncode(sequence, out string? text, true);
        return text;
    }

    public static string? EncodeByBase58Bit(ReadOnlyMemory<byte> memory)
    {
        return EncodeByBase58Bit(new ReadOnlySequence<byte>(memory));
    }

    public static string? EncodeByBase58Bit(ReadOnlySequence<byte> sequence)
    {
        Base58Btc.Instance.TryEncode(sequence, out string? text, true);
        return text;
    }

    public static string? EncodeByBase64(ReadOnlyMemory<byte> memory)
    {
        return EncodeByBase64(new ReadOnlySequence<byte>(memory));
    }

    public static string? EncodeByBase64(ReadOnlySequence<byte> sequence)
    {
        Base64.Instance.TryEncode(sequence, out string? text, true);
        return text;
    }

    public static string? EncodeByBase64Url(ReadOnlyMemory<byte> memory)
    {
        return EncodeByBase64Url(new ReadOnlySequence<byte>(memory));
    }

    public static string? EncodeByBase64Url(ReadOnlySequence<byte> sequence)
    {
        Base64Url.Instance.TryEncode(sequence, out string? text, true);
        return text;
    }

    public static bool TryDecode(string text, IBufferWriter<byte> bufferWriter)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));

        if (string.IsNullOrEmpty(text)) return true;

        switch (text[0])
        {
            case 'f':
                return Base16.Lower.TryDecode(text[1..], bufferWriter);
            case 'F':
                return Base16.Upper.TryDecode(text[1..], bufferWriter);
            case 'z':
                return Base58Btc.Instance.TryDecode(text[1..], bufferWriter);
            case 'm':
                return Base64.Instance.TryDecode(text[1..], bufferWriter);
            case 'u':
                return Base64Url.Instance.TryDecode(text[1..], bufferWriter);
            default:
                return false;
        }
    }
}
