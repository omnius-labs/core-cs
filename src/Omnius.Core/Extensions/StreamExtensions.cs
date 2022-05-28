namespace Omnius.Core;

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
}
