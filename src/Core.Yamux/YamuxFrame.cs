using System.Buffers.Binary;
using System.Buffers;
using Omnius.Core.Base;

namespace Omnius.Yamux;

internal enum FrameTag : byte
{
    Data = 0,
    WindowUpdate = 1,
    Ping = 2,
    GoAway = 3,
}

[Flags]
internal enum FrameFlags : ushort
{
    None = 0,
    Syn = 1,
    Ack = 2,
    Fin = 4,
    Rst = 8,
}

internal enum GoAwayCode : uint
{
    Normal = 0,
    ProtocolError = 1,
    InternalError = 2,
}

internal readonly struct FrameHeader
{
    public FrameHeader(FrameTag tag, FrameFlags flags, uint streamId, uint length)
    {
        this.Version = 0;
        this.Tag = tag;
        this.Flags = flags;
        this.StreamId = streamId;
        this.Length = length;
    }

    public byte Version { get; }
    public FrameTag Tag { get; }
    public FrameFlags Flags { get; }
    public uint StreamId { get; }
    public uint Length { get; }

    public void Encode(Span<byte> buffer)
    {
        if (buffer.Length < YamuxConstants.HeaderSize) throw new ArgumentException("Header buffer too small.");

        buffer[0] = this.Version;
        buffer[1] = (byte)this.Tag;
        BinaryPrimitives.WriteUInt16BigEndian(buffer.Slice(2, 2), (ushort)this.Flags);
        BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(4, 4), this.StreamId);
        BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(8, 4), this.Length);
    }

    public static FrameHeader Decode(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < YamuxConstants.HeaderSize) throw new YamuxProtocolException("Invalid yamux header length.");

        var version = buffer[0];
        if (version != 0) throw new YamuxProtocolException($"Unsupported yamux version: {version}.");

        var tag = buffer[1] switch
        {
            0 => FrameTag.Data,
            1 => FrameTag.WindowUpdate,
            2 => FrameTag.Ping,
            3 => FrameTag.GoAway,
            _ => throw new YamuxProtocolException($"Unknown yamux tag: {buffer[1]}.")
        };

        var flags = (FrameFlags)BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(2, 2));
        var streamId = BinaryPrimitives.ReadUInt32BigEndian(buffer.Slice(4, 4));
        var length = BinaryPrimitives.ReadUInt32BigEndian(buffer.Slice(8, 4));
        return new FrameHeader(tag, flags, streamId, length);
    }
}

internal sealed class Frame : IDisposable
{
    private IMemoryOwner<byte>? _bodyOwner;

    internal Frame(FrameHeader header, ReadOnlyMemory<byte> body, IMemoryOwner<byte>? bodyOwner)
    {
        this.Header = header;
        this.Body = body;
        _bodyOwner = bodyOwner;
    }

    public FrameHeader Header { get; }
    public ReadOnlyMemory<byte> Body { get; }

    public OwnedBytes TakeBody()
    {
        if (this.Body.Length == 0) return OwnedBytes.Empty;

        var owner = _bodyOwner;
        _bodyOwner = null;
        return new OwnedBytes(this.Body, owner);
    }

    public void Dispose()
    {
        _bodyOwner?.Dispose();
        _bodyOwner = null;
    }

    public static Frame Data(uint streamId, ReadOnlyMemory<byte> body, FrameFlags flags, IBytesPool bytesPool)
    {
        if (body.Length > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(body));

        var header = new FrameHeader(FrameTag.Data, flags, streamId, (uint)body.Length);
        if (body.Length == 0) return new Frame(header, ReadOnlyMemory<byte>.Empty, null);

        if (bytesPool is null) throw new ArgumentNullException(nameof(bytesPool));

        var owner = bytesPool.Memory.Rent(body.Length).Shrink(body.Length);
        body.CopyTo(owner.Memory);
        return new Frame(header, owner.Memory, owner);
    }

    public static Frame WindowUpdate(uint streamId, uint credit, FrameFlags flags)
    {
        var header = new FrameHeader(FrameTag.WindowUpdate, flags, streamId, credit);
        return new Frame(header, ReadOnlyMemory<byte>.Empty, null);
    }

    public static Frame Ping(uint nonce, FrameFlags flags)
    {
        var header = new FrameHeader(FrameTag.Ping, flags, 0, nonce);
        return new Frame(header, ReadOnlyMemory<byte>.Empty, null);
    }

    public static Frame GoAway(GoAwayCode code)
    {
        var header = new FrameHeader(FrameTag.GoAway, FrameFlags.None, 0, (uint)code);
        return new Frame(header, ReadOnlyMemory<byte>.Empty, null);
    }
}

internal static class FrameCodec
{
    public static async ValueTask<Frame?> ReadAsync(Stream stream, IBytesPool bytesPool, CancellationToken cancellationToken)
    {
        if (bytesPool is null) throw new ArgumentNullException(nameof(bytesPool));

        var headerBuffer = bytesPool.Array.Rent(YamuxConstants.HeaderSize);
        FrameHeader header;
        try
        {
            var headerMemory = headerBuffer.AsMemory(0, YamuxConstants.HeaderSize);
            if (!await ReadExactlyAsync(stream, headerMemory, cancellationToken).ConfigureAwait(false))
            {
                return null;
            }

            header = FrameHeader.Decode(headerBuffer.AsSpan(0, YamuxConstants.HeaderSize));
        }
        finally
        {
            bytesPool.Array.Return(headerBuffer);
        }

        if (header.Tag != FrameTag.Data)
        {
            return new Frame(header, ReadOnlyMemory<byte>.Empty, null);
        }

        if (header.Length > YamuxConstants.MaxFrameBodyLength)
        {
            throw new YamuxProtocolException(
                $"Frame body too large: {header.Length} bytes.");
        }

        if (header.Length == 0)
        {
            return new Frame(header, ReadOnlyMemory<byte>.Empty, null);
        }

        IMemoryOwner<byte>? owner = null;
        try
        {
            owner = bytesPool.Memory.Rent((int)header.Length).Shrink((int)header.Length);
            await ReadExactlyAsync(stream, owner.Memory, cancellationToken).ConfigureAwait(false);
            return new Frame(header, owner.Memory, owner);
        }
        catch
        {
            owner?.Dispose();
            throw;
        }
    }

    public static async ValueTask WriteAsync(
        Stream stream,
        Frame frame,
        IBytesPool bytesPool,
        CancellationToken cancellationToken)
    {
        if (bytesPool is null)
        {
            throw new ArgumentNullException(nameof(bytesPool));
        }

        var headerBuffer = bytesPool.Array.Rent(YamuxConstants.HeaderSize);
        try
        {
            frame.Header.Encode(headerBuffer.AsSpan(0, YamuxConstants.HeaderSize));
            await stream.WriteAsync(
                    headerBuffer.AsMemory(0, YamuxConstants.HeaderSize),
                    cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            bytesPool.Array.Return(headerBuffer);
        }

        if (frame.Body.Length > 0)
        {
            await stream.WriteAsync(frame.Body, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async ValueTask<bool> ReadExactlyAsync(
        Stream stream,
        Memory<byte> buffer,
        CancellationToken cancellationToken)
    {
        var offset = 0;
        while (offset < buffer.Length)
        {
            var read = await stream.ReadAsync(
                buffer.Slice(offset),
                cancellationToken).ConfigureAwait(false);
            if (read == 0)
            {
                if (offset == 0)
                {
                    return false;
                }

                throw new IOException("Unexpected EOF while reading yamux frame.");
            }

            offset += read;
        }

        return true;
    }
}
