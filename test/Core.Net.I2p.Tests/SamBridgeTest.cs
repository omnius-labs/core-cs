using System.Net;
using System.Net.Sockets;
using FluentAssertions;
using Omnius.Core.Net.I2p.Internal;
using Xunit;

namespace Omnius.Core.Net.I2p;

public class SamBridgeTest
{
    // [Fact]
    [Fact(Skip = "I2p is required")]
    public async Task ConnectTest()
    {
        var random = new Random(0);

        var samBridge1 = await SamBridge.CreateAsync(IPAddress.Parse("127.0.0.1"), 7656, "Test_1");
        var samBridge2 = await SamBridge.CreateAsync(IPAddress.Parse("127.0.0.1"), 7656, "Test_2");

        await Task.Delay(10 * 1000);

        Socket socket1, socket2;

        socket1 = await samBridge1.ConnectAsync(samBridge2.Base32Address!);
        var acceptResult = await samBridge2.AcceptAsync();
        socket2 = acceptResult?.Socket ?? throw new NullReferenceException();

        await ConnectionTestHelper.RandomSendAndReceiveAsync(random, socket1, socket2);
    }

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
