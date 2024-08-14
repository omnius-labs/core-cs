using System.Buffers;
using System.Security.Cryptography;
using Omnius.Core.Base;

namespace Omnius.Core.Omnikit.Connections.Secure.V1;

public class Aes256GcmDecoder
{
    private readonly AesGcm _cipher;
    private readonly IBytesPool _bytesPool;
    private byte[] _nonce;

    private const int TagSize = 16;

    public Aes256GcmDecoder(byte[] key, byte[] nonce, IBytesPool bytesPool)
    {
        _cipher = new AesGcm(key, TagSize);
        _nonce = nonce;
        _bytesPool = bytesPool;
    }

    public IMemoryOwner<byte> Decode(ReadOnlySpan<byte> data)
    {
        var plaintext = _bytesPool.Memory.Rent(data.Length - TagSize).Shrink(data.Length - TagSize);

        var ciphertext = data[..^TagSize];
        var tag = data[^TagSize..];

        _cipher.Decrypt(_nonce, ciphertext, tag, plaintext.Memory.Span);

        Utils.IncrementBytes(_nonce);

        return plaintext;
    }
}
