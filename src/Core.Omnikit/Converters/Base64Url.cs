using System.Buffers;
using System.Text;

namespace Omnius.Core.Omnikit.Converters;

public unsafe class Base64Url : IBytesToUtf8StringConverter
{
    public static Base64Url Instance { get; } = new Base64Url();

    internal Base64Url() { }

    public bool TryEncode(ReadOnlySequence<byte> sequence, out byte[] text, bool includePrefix = false)
    {
        var base64 = Convert.ToBase64String(sequence.ToArray())
            .Replace("+", "-", StringComparison.InvariantCulture)
            .Replace("/", "_", StringComparison.InvariantCulture)
            .TrimEnd('=');

        if (!includePrefix)
        {
            text = Encoding.ASCII.GetBytes(base64);
            return true;
        }

        text = new byte[1 + base64.Length];
        text[0] = (byte)'u';
        Encoding.ASCII.GetBytes(base64, 0, base64.Length, text, 1);

        return true;
    }

    public bool TryDecode(ReadOnlySpan<byte> text, IBufferWriter<byte> bufferWriter)
    {
        var base64Url = Encoding.ASCII.GetString(text);
        var base64 = base64Url
            .Replace("-", "+", StringComparison.InvariantCulture)
            .Replace("_", "/", StringComparison.InvariantCulture);
        var paddingNeeded = 4 - base64.Length % 4;

        if (paddingNeeded < 4)
        {
            base64 += new string('=', paddingNeeded);
        }

        bufferWriter.Write(Convert.FromBase64String(base64));

        return true;
    }
}
