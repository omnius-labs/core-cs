using System.Buffers;
using System.Text;

namespace Omnius.Core.Omnikit.Converters;

public unsafe class Base64 : IBytesToUtf8StringConverter
{
    public static Base64 Instance { get; } = new Base64();

    internal Base64() { }

    public bool TryEncode(ReadOnlySequence<byte> sequence, out byte[] text, bool includePrefix = false)
    {
        var base64 = Convert.ToBase64String(sequence.ToArray())
            .TrimEnd('=');

        if (!includePrefix)
        {
            text = Encoding.ASCII.GetBytes(base64);
            return true;
        }

        text = new byte[1 + base64.Length];
        text[0] = (byte)'m';
        Encoding.ASCII.GetBytes(base64, 0, base64.Length, text, 1);

        return true;
    }

    public bool TryDecode(ReadOnlySpan<byte> text, IBufferWriter<byte> bufferWriter)
    {
        var base64 = Encoding.ASCII.GetString(text);
        var paddingNeeded = 4 - base64.Length % 4;

        if (paddingNeeded < 4)
        {
            base64 += new string('=', paddingNeeded);
        }

        bufferWriter.Write(Convert.FromBase64String(base64));

        return true;
    }
}
