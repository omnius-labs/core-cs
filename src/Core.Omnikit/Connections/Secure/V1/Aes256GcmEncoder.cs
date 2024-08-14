
using System.Buffers;
using System.Security.Cryptography;
using Omnius.Core.Base;

namespace Omnius.Core.Omnikit.Connections.Secure.V1;

public class Aes256GcmEncoder
{
    private readonly AesGcm _cipher;
    private readonly IBytesPool _bytesPool;
    private byte[] _nonce;

    private const int TagSize = 16;

    public Aes256GcmEncoder(byte[] key, byte[] nonce, IBytesPool bytesPool)
    {
        _cipher = new AesGcm(key, TagSize);
        _nonce = nonce;
        _bytesPool = bytesPool;
    }

    public IMemoryOwner<byte> Encode(ReadOnlySpan<byte> data)
    {
        var ciphertextWithTag = _bytesPool.Memory.Rent(data.Length + TagSize).Shrink(data.Length + TagSize);

        var ciphertext = ciphertextWithTag.Memory.Span[..^TagSize];
        var tag = ciphertextWithTag.Memory.Span[^TagSize..];

        _cipher.Encrypt(_nonce, data, ciphertext, tag);

        Utils.IncrementBytes(_nonce);

        return ciphertextWithTag;
    }
}
