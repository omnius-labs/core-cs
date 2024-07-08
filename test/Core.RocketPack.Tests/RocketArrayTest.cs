
using System.Buffers;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack.Tests.Internal;
using Xunit;

namespace Omnius.Core.RocketPack;

public class RocketArrayTest
{
    [Fact]
    public void Test()
    {
        var m1 = new MessageElement_Class(true);
        var m2 = new MessageElement_Class(true);
        var a = new RocketArray<MessageElement_Class>([m1, m2]);

        using var b = RocketMessageConverter.ToBytes(a);
        var c = RocketMessageConverter.FromBytes<RocketArray<MessageElement_Class>>(b.Memory);

        Assert.Equal(a, c);
    }
}
