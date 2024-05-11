using System.Buffers;

namespace Core.Base;

public static class StreamExtensions
{
    public static async ValueTask<byte[]> ToBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        var result = new byte[stream.Length];
        var remain = result.Length;

        while (remain > 0)
        {
            var readLength = await stream.ReadAsync(result.AsMemory(^remain));
            remain -= readLength;
        }

        return result;
    }

    public static async ValueTask<IMemoryOwner<byte>> ToBytesAsync(this Stream stream, IBytesPool bytesPool, CancellationToken cancellationToken = default)
    {
        var memoryOwner = bytesPool.Memory.Rent((int)stream.Length).Shrink((int)stream.Length);
        var remain = memoryOwner.Memory.Length;

        while (remain > 0)
        {
            var readLength = await stream.ReadAsync(memoryOwner.Memory.Slice(memoryOwner.Memory.Length - remain));
            remain -= readLength;
        }

        return memoryOwner;
    }

    public static async ValueTask WriteAsync(this Stream stream, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
    {
        foreach (var m in sequence)
        {
            await stream.WriteAsync(m, cancellationToken);
        }
    }
}
