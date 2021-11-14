using System.Buffers;
using System.Security.Cryptography;
using Omnius.Core.Common;

namespace Omnius.Core.Helpers;

public static partial class ObjectHelper
{
    private static readonly byte[] _key = new byte[16];

    static ObjectHelper()
    {
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(_key);
    }

    public static int GetHashCode(ReadOnlySpan<byte> value)
    {
        ulong v = SipHasher.ComputeHash(_key, value);
        return (int)(v & 0xFFFFFFFF) | (int)(v >> 32);
    }

    public static int GetHashCode(ReadOnlySequence<byte> sequence)
    {
        ulong v = SipHasher.ComputeHash(_key, sequence);
        return (int)(v & 0xFFFFFFFF) | (int)(v >> 32);
    }
}
