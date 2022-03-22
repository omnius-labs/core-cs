using System.Buffers;
using System.IO.Pipelines;
using System.Security.Cryptography;
using System.Text;

namespace Omnius.Core.Cryptography.Functions;

public static class Sha2_256
{
    private static readonly ThreadLocal<Encoding> _utf8Encoding = new ThreadLocal<Encoding>(() => new UTF8Encoding(false));

    public static byte[] ComputeHash(ReadOnlySpan<byte> value)
    {
        byte[] result = new byte[32];
        TryComputeHash(value, result.AsSpan());

        return result;
    }

    public static byte[] ComputeHash(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        byte[] result = new byte[32];
        TryComputeHash(value, result.AsSpan());

        return result;
    }

    public static byte[] ComputeHash(ReadOnlySequence<byte> sequence)
    {
        byte[] result = new byte[32];
        TryComputeHash(sequence, result.AsSpan());

        return result;
    }

    public static bool TryComputeHash(ReadOnlySpan<byte> value, Span<byte> destination)
    {
        using (var sha2_256 = SHA256.Create())
        {
            return sha2_256.TryComputeHash(value, destination, out _);
        }
    }

    public static bool TryComputeHash(string value, Span<byte> destination)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        using (var recyclableMemory = MemoryPool<byte>.Shared.Rent(_utf8Encoding.Value!.GetMaxByteCount(value.Length)))
        {
            var length = _utf8Encoding.Value!.GetBytes(value, recyclableMemory.Memory.Span);

            return TryComputeHash(recyclableMemory.Memory.Span[..length], destination);
        }
    }

    public static bool TryComputeHash(ReadOnlySequence<byte> sequence, Span<byte> destination)
    {
        using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
        {
            foreach (var segment in sequence)
            {
                incrementalHash.AppendData(segment.Span);
            }

            return incrementalHash.TryGetHashAndReset(destination, out _);
        }
    }

    public static async ValueTask<byte[]> ComputeHashAsync(Stream stream)
    {
        var reader = PipeReader.Create(stream);
        return await ComputeHashAsync(reader);
    }

    public static async ValueTask<byte[]> ComputeHashAsync(PipeReader reader)
    {
        using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
        {
            for (; ; )
            {
                var readResult = await reader.ReadAsync();

                if (!readResult.Buffer.IsEmpty)
                {
                    foreach (var segment in readResult.Buffer)
                    {
                        incrementalHash.AppendData(segment.Span);
                    }

                    reader.AdvanceTo(readResult.Buffer.End);
                }
                else if (readResult.IsCompleted)
                {
                    await reader.CompleteAsync();
                    return incrementalHash.GetHashAndReset();
                }
                else if (readResult.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
            }
        }
    }
}
