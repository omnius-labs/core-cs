using System;
using System.Buffers;
using Omnius.Core.Pipelines;
using Xunit;

namespace Omnius.Core.Serialization;

public class OmniMessageConverterTest
{
    [Fact]
    public void WriteAndReadTest()
    {
        var random = new Random(0);
        using var inPipe = new BytesPipe();
        using var outPipe = new BytesPipe();

        var inVersion = (uint)random.Next();
        var inBody = new byte[random.Next(0, 1024 * 64)];
        random.NextBytes(inBody);

        Assert.True(TryEncode(inVersion, inBody, out var message));
        Assert.True(TryDecode(message!, out var outVersion, out var outBody));

        Assert.Equal(inVersion, outVersion);
        Assert.True(BytesOperations.Equals(inBody.AsSpan(), outBody.AsSpan()));
    }

    private static bool TryEncode(uint version, byte[] body, out byte[]? message)
    {
        message = null;
        using var bytesPipe = new BytesPipe();
        if (!OmniMessageConverter.TryWrite(version, new ReadOnlySequence<byte>(body), bytesPipe.Writer)) return false;

        message = bytesPipe.Reader.GetSequence().ToArray();
        return true;
    }

    private static bool TryDecode(byte[] message, out uint version, out byte[]? body)
    {
        body = null;
        using var bytesPipe = new BytesPipe();
        if (!OmniMessageConverter.TryRead(new ReadOnlySequence<byte>(message), out version, bytesPipe.Writer)) return false;

        body = bytesPipe.Reader.GetSequence().ToArray();
        return true;
    }
}