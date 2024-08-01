using Org.BouncyCastle.Crypto.Digests;

namespace Omnius.Core.Omnikit.Internal;

internal static class Sha3_256
{
    public static byte[] ComputeHash(byte[] input)
    {
        var sha3Digest = new Sha3Digest(256);
        sha3Digest.BlockUpdate(input, 0, input.Length);
        var result = new byte[sha3Digest.GetDigestSize()];
        sha3Digest.DoFinal(result, 0);

        return result;
    }
}
