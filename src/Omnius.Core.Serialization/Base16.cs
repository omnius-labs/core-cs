using System;
using System.Buffers;

namespace Omnius.Core.Serialization
{
    public unsafe class Base16 : IBytesToUtf8StringConverter
    {
        private readonly ConvertStringCase _convertStringCase;

        public Base16()
        {
            _convertStringCase = ConvertStringCase.Lower;
        }

        public Base16(ConvertStringCase convertStringCase)
        {
            _convertStringCase = convertStringCase;
        }

        private byte ByteToWord(in byte c)
        {
            if (_convertStringCase == ConvertStringCase.Lower)
            {
                if (c < 10) return (byte)(c + '0');
                else return (byte)(c - 10 + 'a');
            }
            else if (_convertStringCase == ConvertStringCase.Upper)
            {
                if (c < 10) return (byte)(c + '0');
                else return (byte)(c - 10 + 'A');
            }

            throw new NotSupportedException();
        }

        private byte WordToByte(in byte c)
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
                    *p_result_start++ = (_convertStringCase == ConvertStringCase.Lower) ? (byte)'f' : (byte)'F';
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

                            *p_result_start++ = this.ByteToWord((byte)(b >> 4));
                            *p_result_start++ = this.ByteToWord((byte)(b & 0x0F));
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
                    var tmp = this.WordToByte(*p_text_start++);

                    bufferWriter.GetSpan()[0] = tmp;
                    bufferWriter.Advance(1);
                }

                while (p_text_start != p_text_end)
                {
                    var tmp1 = this.WordToByte(*p_text_start++);
                    var tmp2 = this.WordToByte(*p_text_start++);

                    bufferWriter.GetSpan()[0] = (byte)((tmp1 << 4) | tmp2);
                    bufferWriter.Advance(1);
                }
            }

            return true;
        }
    }
}
