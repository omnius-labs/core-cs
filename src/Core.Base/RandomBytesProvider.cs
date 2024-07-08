using System.Security.Cryptography;
using System.Text;

namespace Omnius.Core.Base;

public unsafe class RandomBytesProvider : IRandomBytesProvider
{
    public static readonly RandomBytesProvider Shared = new();

    public byte[] GetBytes(int length)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }
}
