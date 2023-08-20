using System.Security.Cryptography;
using System.Text;

namespace Omnius.Core;

public unsafe class RandomBytesProvider : IRandomBytesProvider
{
    public byte[] GetBytes(int length)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }
}
