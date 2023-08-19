using System.Security.Cryptography;
using System.Text;

namespace Omnius.Core;

public enum RandomBase16StringCase
{
    Lower,
    Upper,
}

public unsafe class RandomBase16StringProvider : IRandomStringProvider
{
    private readonly int _length;
    private delegate*<in byte, char> _byteToChar;

    public RandomBase16StringProvider(int length, RandomBase16StringCase stringCase)
    {
        _length = length;

        if (stringCase == RandomBase16StringCase.Lower) _byteToChar = &ByteToLowerWord;
        else if (stringCase == RandomBase16StringCase.Upper) _byteToChar = &ByteToUpperWord;
        else throw new NotSupportedException();
    }

    private static char ByteToLowerWord(in byte c)
    {
        if (c < 10) return (char)(c + '0');
        else return (char)(c - 10 + 'a');
    }

    private static char ByteToUpperWord(in byte c)
    {
        if (c < 10) return (char)(c + '0');
        else return (char)(c - 10 + 'A');
    }

    public string Gen()
    {
        Span<byte> bytes = _length <= 1024 ? stackalloc byte[_length] : new byte[_length];
        RandomNumberGenerator.Fill(bytes);

        var sb = new StringBuilder();

        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(_byteToChar((byte)(bytes[i] >> 4)));
            sb.Append(_byteToChar((byte)(bytes[i] & 0x0F)));
        }

        return sb.ToString();
    }
}
