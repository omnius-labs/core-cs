using System.Net;
using System.Net.Sockets;
using System.Text;
using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;
using Omnius.Core.RocketPack;
using Omnius.Core.Testkit;
using Xunit;
using Xunit.Abstractions;

namespace Omnius.Core.Omnikit.Connections.Secure.V1;

public class OmniSecureStreamTest : TestBase<OmniSecureStreamTest>
{
    public OmniSecureStreamTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task ExportTest()
    {
        var p = new ProfileMessage()
        {
            SessionId = [1, 2, 3, 4],
            AuthType = AuthType.Sign,
            KeyExchangeAlgorithmType = KeyExchangeAlgorithmType.X25519,
            KeyDerivationAlgorithmType = KeyDerivationAlgorithmType.Hkdf,
            CipherAlgorithmType = CipherAlgorithmType.Aes256Gcm,
            HashAlgorithmType = HashAlgorithmType.Sha3_256,
        };
        using var b = RocketPackStruct.Export(p, BytesPool.Shared);
        var p2 = RocketPackStruct.Import<ProfileMessage>(b.Memory, BytesPool.Shared);

        Assert.Equal(p, p2);
    }

    [Fact]
    public async Task SimpleTest()
    {
        var fixtureFactory = new FixtureFactory(0);
        var randomBytesProvider = new RandomBytesProvider();
        var clock = new FakeClock(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var bytesPool = BytesPool.Shared;

        var (client, server) = DuplexStream.CreatePair();

        var clientTask = Task.Run(async () =>
        {
            var secureClient = await OmniSecureStream.CreateAsync(
                OmniSecureStreamType.Connected,
                client,
                null,
                randomBytesProvider,
                clock,
                bytesPool
            );

            return secureClient;
        });

        var serverTask = Task.Run(async () =>
        {
            var secureServer = await OmniSecureStream.CreateAsync(
                OmniSecureStreamType.Accepted,
                server,
                null,
                randomBytesProvider,
                clock,
                bytesPool
            );

            return secureServer;
        });

        var (secureClient, secureServer) = (await clientTask, await serverTask);

        var secureClientSender = new FramedSender(secureClient, 1024 * 1024 * 32, BytesPool.Shared);
        var secureServerReceiver = new FramedReceiver(secureServer, 1024 * 1024 * 32, BytesPool.Shared);

        var cases = new int[] { 1, 2, 3, 10, 100, 1000, 1024 * 1024 };
        foreach (var c in cases)
        {
            var data = randomBytesProvider.GetBytes(c);
            var sendTask = secureClientSender.SendAsync(data).AsTask();
            var receiveTask = secureServerReceiver.ReceiveAsync().AsTask();

            await Task.WhenAll(sendTask, receiveTask);

            var receivedData = await receiveTask;

            Assert.Equal(data, receivedData.Memory.ToArray());
        }
    }

    [Fact(Skip = "Echo server is required.")]
    public async Task ConnectEchoTest()
    {
        var randomBytesProvider = new RandomBytesProvider();
        var clock = new FakeClock(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var bytesPool = BytesPool.Shared;

        var ipEndPoint = new IPEndPoint(IPAddress.Loopback, 50000);
        using var client = new TcpClient();
        await client.ConnectAsync(ipEndPoint.Address, ipEndPoint.Port);
        var clientStream = client.GetStream();

        var secureClient = await OmniSecureStream.CreateAsync(
            OmniSecureStreamType.Connected,
            clientStream,
            null,
            randomBytesProvider,
            clock,
            bytesPool
        );

        var secureReceiver = new FramedReceiver(secureClient, 1024, BytesPool.Shared);
        var secureSender = new FramedSender(secureClient, 1024, BytesPool.Shared);

        await secureSender.SendAsync(new UTF8Encoding(false).GetBytes("Hello, World!"));
        var v = await secureReceiver.ReceiveAsync();

        this.Output.WriteLine(new UTF8Encoding(false).GetString(v.Memory.Span));
    }
}
