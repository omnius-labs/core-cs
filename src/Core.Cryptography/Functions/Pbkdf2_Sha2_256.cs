using Core.Base;

namespace Core.Cryptography.Functions;

public unsafe class Pbkdf2_Sha2_256
{
    public static byte[] ComputeHash(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, int iterationCount, int length)
    {
        byte[] result = new byte[length];
        TryComputeHash(password, salt, iterationCount, result);

        return result;
    }

    public static bool TryComputeHash(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, int iterationCount, Span<byte> destination)
    {
        const int hashLength = 32;

        int keyLength = destination.Length / hashLength;
        if (destination.Length % hashLength != 0)
        {
            keyLength++;
        }

        var extendedKeyLength = (salt.Length + 4);
        Span<byte> extendedKey = extendedKeyLength <= 128 ? stackalloc byte[extendedKeyLength] : new byte[extendedKeyLength];

        BytesOperations.Copy(salt, extendedKey, salt.Length);

        Span<byte> f = stackalloc byte[hashLength];
        Span<byte> u = stackalloc byte[hashLength];

        for (int i = 0; i < keyLength; i++)
        {
            extendedKey[salt.Length] = (byte)(((i + 1) >> 24) & 0xFF);
            extendedKey[salt.Length + 1] = (byte)(((i + 1) >> 16) & 0xFF);
            extendedKey[salt.Length + 2] = (byte)(((i + 1) >> 8) & 0xFF);
            extendedKey[salt.Length + 3] = (byte)(((i + 1)) & 0xFF);

            Hmac_Sha2_256.TryComputeHash(extendedKey, password, u);
            BytesOperations.Zero(extendedKey.Slice(salt.Length, 4));

            BytesOperations.Copy(u, f, hashLength);

            for (int j = 1; j < iterationCount; j++)
            {
                Hmac_Sha2_256.TryComputeHash(u, password, u);
                BytesOperations.Xor(f, u, f);
            }

            int position = i * hashLength;
            int remain = Math.Min(hashLength, destination.Length - position);

            BytesOperations.Copy(f, destination[position..], remain);
        }

        return true;
    }
}
