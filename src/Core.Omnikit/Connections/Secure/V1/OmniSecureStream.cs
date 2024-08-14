using System.Buffers;
using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;

namespace Omnius.Core.Omnikit.Connections.Secure.V1;

public enum OmniSecureStreamType
{
    Connected,
    Accepted,
}

public partial class OmniSecureStream
{
    private readonly OmniSecureStreamType _type;
    private readonly FramedReceiver _receiver;
    private readonly FramedSender _sender;
    private readonly OmniSigner? _signer;
    private readonly IRandomBytesProvider _randomBytesProvider;
    private readonly IClock _clock;
    private readonly IBytesPool _bytesPool;

    private Aes256GcmEncoder? _encoder;
    private Aes256GcmDecoder? _decoder;

    private readonly AsyncLock _readLock = new();
    private readonly AsyncLock _writeLock = new();

    private const int MaxFrameLength = 1024 * 64;

    public static async ValueTask<OmniSecureStream> CreateAsync(OmniSecureStreamType type, Stream stream, OmniSigner? signer, IRandomBytesProvider randomBytesProvider, IClock clock, IBytesPool bytesPool, CancellationToken cancellationToken = default)
    {
        var secureStream = new OmniSecureStream(type, stream, signer, randomBytesProvider, clock, bytesPool);
        await secureStream.InitAsync(cancellationToken);
        return secureStream;
    }

    internal OmniSecureStream(OmniSecureStreamType type, Stream stream, OmniSigner? signer, IRandomBytesProvider randomBytesProvider, IClock clock, IBytesPool bytesPool)
    {
        _type = type;
        _receiver = new FramedReceiver(stream, MaxFrameLength + 16, bytesPool);
        _sender = new FramedSender(stream, MaxFrameLength + 16, bytesPool);
        _signer = signer;
        _randomBytesProvider = randomBytesProvider;
        _clock = clock;
        _bytesPool = bytesPool;

        _sendMemoryOwner = bytesPool.Memory.Rent(MaxFrameLength).Shrink(MaxFrameLength);
    }

    private async ValueTask InitAsync(CancellationToken cancellationToken = default)
    {
        var authenticator = new Authenticator(_type, _receiver, _sender, _signer, _randomBytesProvider, _clock, _bytesPool);
        var authResult = await authenticator.AuthenticationAsync(cancellationToken);

        _encoder = new Aes256GcmEncoder(authResult.EncKey, authResult.EncNonce, BytesPool.Shared);
        _decoder = new Aes256GcmDecoder(authResult.DecKey, authResult.DecNonce, BytesPool.Shared);
    }

    private async ValueTask InternalSendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (_encoder is null) throw new SecureStreamException("SecureStream is not initialized.");

        using var encryptedData = _encoder.Encode(data.ToArray());
        await _sender.SendAsync(encryptedData.Memory, cancellationToken);
    }

    private async ValueTask<IMemoryOwner<byte>> InternalReceiveAsync(CancellationToken cancellationToken = default)
    {
        if (_decoder is null) throw new SecureStreamException("SecureStream is not initialized.");

        using var encryptedData = await _receiver.ReceiveAsync(cancellationToken);
        return _decoder.Decode(encryptedData.Memory.Span);
    }
}

public class SecureStreamException : Exception
{
    public SecureStreamException() { }
    public SecureStreamException(string message) : base(message) { }
    public SecureStreamException(string message, Exception inner) : base(message, inner) { }
}

public partial class OmniSecureStream : Stream
{
    public override bool CanRead => true;
    public override bool CanWrite => true;
    public override bool CanSeek => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return this.ReadAsync(buffer, offset, count).Result;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        this.WriteAsync(buffer, offset, count).Wait();
    }

    public override void Flush()
    {
        this.FlushAsync().Wait();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    private IMemoryOwner<byte>? _receiveMemoryOwner;
    private int _receiveMemoryOwnerOffset;

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        using (await _readLock.LockAsync(cancellationToken))
        {
            count = Math.Min(count, buffer.Length - offset);
            if (count == 0) return 0;

            for (; ; )
            {
                var memory = buffer.AsMemory(offset, count);

                try
                {
                    var readSize = await this.ReadSubAsync(memory, cancellationToken);
                    if (readSize != 0) return readSize;
                }
                catch (EndOfStreamException)
                {
                    return 0;
                }
            }
        }
    }

    private async Task<int> ReadSubAsync(Memory<byte> memory, CancellationToken cancellationToken)
    {
        _receiveMemoryOwner ??= await this.InternalReceiveAsync(cancellationToken);

        int count = Math.Min(memory.Length, _receiveMemoryOwner.Memory.Length - _receiveMemoryOwnerOffset);

        _receiveMemoryOwner.Memory.Slice(_receiveMemoryOwnerOffset, count).CopyTo(memory.Slice(0, count));
        _receiveMemoryOwnerOffset += count;

        if (_receiveMemoryOwnerOffset == _receiveMemoryOwner.Memory.Length)
        {
            _receiveMemoryOwner.Dispose();
            _receiveMemoryOwner = null;
            _receiveMemoryOwnerOffset = 0;
        }

        return count;
    }

    private readonly IMemoryOwner<byte> _sendMemoryOwner;
    private int _sendMemoryOwnerOffset;

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        using (await _writeLock.LockAsync(cancellationToken))
        {
            count = Math.Min(count, buffer.Length - offset);
            if (count == 0) return;

            while (count > 0)
            {
                var memory = buffer.AsMemory(offset, count);
                var writeSize = await this.WriteSubAsync(memory, cancellationToken);
                offset += writeSize;
                count -= writeSize;
            }
        }
    }

    private async Task<int> WriteSubAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken)
    {
        int count = Math.Min(memory.Length, _sendMemoryOwner.Memory.Length - _sendMemoryOwnerOffset);

        memory.Slice(0, count).CopyTo(_sendMemoryOwner.Memory.Slice(_sendMemoryOwnerOffset, count));
        _sendMemoryOwnerOffset += count;

        if (_sendMemoryOwnerOffset == _sendMemoryOwner.Memory.Length)
        {
            await this.InternalSendAsync(_sendMemoryOwner.Memory, cancellationToken);
            _sendMemoryOwnerOffset = 0;
        }

        return count;
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        using (await _writeLock.LockAsync(cancellationToken))
        {
            await this.InternalSendAsync(_sendMemoryOwner.Memory.Slice(0, _sendMemoryOwnerOffset), cancellationToken);
            _sendMemoryOwnerOffset = 0;
        }
    }
}
