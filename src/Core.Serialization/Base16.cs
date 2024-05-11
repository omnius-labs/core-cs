using System.Buffers;

namespace Core.Serialization;

public enum Base16Case
{
    Upper,
    Lower,
}

public unsafe class Base16 : IBytesToUtf8StringConverter
{
    private readonly byte _prefix;
    private readonly delegate*<in byte, byte> _byteToWord;

    public Base16() : this(Base16Case.Lower) { }

    public Base16(Base16Case base16Case)
    {
        if (base16Case == Base16Case.Lower)
        {
            _prefix = (byte)'f';
            _byteToWord = &ByteToLowerWord;
        }
        else if (base16Case == Base16Case.Upper)
        {
            _prefix = (byte)'F';
            _byteToWord = &ByteToUpperWord;
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    private static byte ByteToLowerWord(in byte c)
    {
        if (c < 10) return (byte)(c + '0');
        else return (byte)(c - 10 + 'a');
    }

    private static byte ByteToUpperWord(in byte c)
    {
        if (c < 10) return (byte)(c + '0');
        else return (byte)(c - 10 + 'A');
    }

    private static byte WordToByte(in byte c)
    {
        if (c >= '0' && c <= '9') return (byte)(c - '0');
        else if (c >= 'a' && c <= 'f') return (byte)((c - 'a') + 10);
        else if (c >= 'A' && c <= 'F') return (byte)((c - 'A') + 10);

        throw new FormatException();
    }

    public bool TryEncode(ReadOnlySequence<byte> sequence, out byte[] text, bool includePrefix = false)
    {
        var result = new byte[(includePrefix ? 1 : 0) + (sequence.Length * 2)];

        fixed (byte* p_result_fixed = result)
        {
            var p_result_start = p_result_fixed;

            if (includePrefix)
            {
                *p_result_start++ = _prefix;
            }

            foreach (var segment in sequence)
            {
                fixed (byte* p_value_fixed = segment.Span)
                {
                    var p_value_start = p_value_fixed;
                    var p_value_end = p_value_fixed + segment.Length;

                    while (p_value_start != p_value_end)
                    {
                        byte b = *p_value_start++;

                        *p_result_start++ = _byteToWord((byte)(b >> 4));
                        *p_result_start++ = _byteToWord((byte)(b & 0x0F));
                    }
                }
            }
        }

        text = result;

        return true;
    }

    public bool TryDecode(ReadOnlySpan<byte> text, IBufferWriter<byte> bufferWriter)
    {
        if (bufferWriter == null) throw new ArgumentNullException(nameof(bufferWriter));

        if (text.IsEmpty) return true;

        fixed (byte* p_text_fixed = text)
        {
            var p_text_start = p_text_fixed;
            var p_text_end = p_text_fixed + text.Length;

            if (text.Length % 2 != 0)
            {
                var tmp = WordToByte(*p_text_start++);

                bufferWriter.GetSpan()[0] = tmp;
                bufferWriter.Advance(1);
            }

            while (p_text_start != p_text_end)
            {
                var tmp1 = WordToByte(*p_text_start++);
                var tmp2 = WordToByte(*p_text_start++);

                bufferWriter.GetSpan()[0] = (byte)((tmp1 << 4) | tmp2);
                bufferWriter.Advance(1);
            }
        }

        return true;
    }
}
