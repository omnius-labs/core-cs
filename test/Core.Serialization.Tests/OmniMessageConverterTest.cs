using System.Buffers;
using Core.Base;
using Core.Pipelines;
using Xunit;

namespace Core.Serialization;

public class OmniMessageConverterTest
{
    [Fact]
    public void WriteAndReadTest()
    {
        var random = new Random(0);

        var cases = new int[] {
            0,
            1,
            10,
            100,
            random.Next(0, 1024 * 64),
            random.Next(0, 1024 * 64),
            random.Next(0, 1024 * 64),
            random.Next(0, 1024 * 64),
        };

        foreach (var length in cases)
        {
            var inBody = new byte[length];
            random.NextBytes(inBody);

            Assert.True(TryEncode(inBody, out var message));
            Assert.True(TryDecode(message!, out var outBody));

            Assert.True(BytesOperations.Equals(inBody.AsSpan(), outBody.AsSpan()));
        }
    }

    private static bool TryEncode(byte[] body, out byte[]? message)
    {
        message = null;
        using var bytesPipe = new BytesPipe();
        if (!OmniMessageConverter.TryWrite(new ReadOnlySequence<byte>(body), bytesPipe.Writer)) return false;

        message = bytesPipe.Reader.GetSequence().ToArray();
        return true;
    }

    private static bool TryDecode(byte[] message, out byte[]? body)
    {
        body = null;
        using var bytesPipe = new BytesPipe();
        if (!OmniMessageConverter.TryRead(new ReadOnlySequence<byte>(message), bytesPipe.Writer)) return false;

        body = bytesPipe.Reader.GetSequence().ToArray();
        return true;
    }
}
