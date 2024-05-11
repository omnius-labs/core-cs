using Xunit;

namespace Core.Pipelines;

public class BoundedMessagePipeTest
{
    [Fact]
    public void TryWriteAndTryReadTest()
    {
        using var messagePipe = new BoundedMessagePipe(1);
        Assert.True(messagePipe.Writer.TryWrite());
        Assert.False(messagePipe.Writer.TryWrite());
        Assert.True(messagePipe.Reader.TryRead());
        Assert.False(messagePipe.Reader.TryRead());
        Assert.True(messagePipe.Writer.TryWrite());
    }

    [Fact]
    public void TryWriteAndTryRead_T_Test()
    {
        using var messagePipe = new BoundedMessagePipe<int>(1);
        Assert.True(messagePipe.Writer.TryWrite(1));
        Assert.False(messagePipe.Writer.TryWrite(2));
        Assert.True(messagePipe.Reader.TryRead(out var v1));
        Assert.Equal(1, v1);
        Assert.False(messagePipe.Reader.TryRead(out var v2));
        Assert.Equal(0, v2);
        Assert.True(messagePipe.Writer.TryWrite(3));
    }
}
