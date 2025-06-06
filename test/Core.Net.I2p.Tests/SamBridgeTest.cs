using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Omnius.Core.Net.I2p.Internal;
using Shouldly;
using Xunit;

namespace Omnius.Core.Net.I2p;

public class SamBridgeTest
{
    // [Fact]
    [Fact(Skip = "I2p is required")]
    public async Task ConnectTest()
    {
        var random = new Random(0);
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .AddDebug();
        });
        var logger = loggerFactory.CreateLogger<SamBridge>();

        var samBridge1 = await SamBridge.CreateAsync(IPAddress.Parse("127.0.0.1"), 7656, "Test_1", logger);
        var samBridge2 = await SamBridge.CreateAsync(IPAddress.Parse("127.0.0.1"), 7656, "Test_2", logger);

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
            .ShouldBe("a b c=d");

        SamCommand.Parse("a b c=d")
            .ShouldBe(new SamCommand(new[] { "a", "b" }, new[] { ("c", "d") }));

        new SamCommand(new[] { "e" }).ToString()
            .ShouldBe("e");

        SamCommand.Parse("e")
            .ShouldBe(new SamCommand(new[] { "e" }));

        SamCommand.Parse("PING aaaaaaaaaaa")
            .ShouldBe(new SamCommand(new[] { "PING", "aaaaaaaaaaa" }));

        new SamCommand(new[] { "PONG", "bbbbbbbbb" }).ToString()
            .ShouldBe("PONG bbbbbbbbb");
    }
}
