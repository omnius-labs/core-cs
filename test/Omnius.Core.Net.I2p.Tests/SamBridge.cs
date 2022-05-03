using FluentAssertions;
using Omnius.Core.Net.I2p.Internal;
using Xunit;

namespace Omnius.Core.Net.I2p;

public class SamBridgeTest
{
    [Fact]
    public async Task SamCommandTest()
    {
        new SamCommand(new[] { "a", "b" }, new[] { ("c", "d") }).ToString()
            .Should().Be("a b c=d");

        SamCommand.Parse("a b c=d")
            .Should().Be(new SamCommand(new[] { "a", "b" }, new[] { ("c", "d") }));

        new SamCommand(new[] { "e" }).ToString()
            .Should().Be("e");

        SamCommand.Parse("e")
            .Should().Be(new SamCommand(new[] { "e" }));

        SamCommand.Parse("PING aaaaaaaaaaa")
            .Should().Be(new SamCommand(new[] { "PING", "aaaaaaaaaaa" }));

        new SamCommand(new[] { "PONG", "bbbbbbbbb" }).ToString()
            .Should().Be("PONG bbbbbbbbb");
    }
}
