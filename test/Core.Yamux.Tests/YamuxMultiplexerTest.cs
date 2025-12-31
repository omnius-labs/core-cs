using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Time.Testing;
using Omnius.Core.Testkit;
using Omnius.Yamux.Internal;
using Xunit;
using Xunit.Abstractions;

namespace Omnius.Yamux;

public class YamuxMultiplexerTest : TestBase<YamuxMultiplexerTest>
{
    private readonly TestHelper _testHelper;

    public YamuxMultiplexerTest(ITestOutputHelper output) : base(output)
    {
        _testHelper = new TestHelper(output);
    }

    [Fact]
    public async Task RandomTest()
    {
        var (client, server) = await _testHelper.CreateYamuxMultiplexerPair(this.Logger);

        try
        {
            var caseList = new List<int>();
            caseList.AddRange(Enumerable.Range(1, 4));
            caseList.AddRange([100, 1000, 10000]);
            caseList.AddRange([1024 * 1024]);
            caseList.AddRange([1024 * 1024 * 8]);

            var random = new Random();

            foreach (var bufferSize in caseList)
            {
                var (clientStream, serverStream) = await CreateStreamPairAsync(client, server);

                try
                {
                    var buffer1 = new byte[bufferSize];
                    var buffer2 = new byte[bufferSize];

                    random.NextBytes(buffer1);

                    using var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(3000));

                    var sb = Stopwatch.StartNew();

                    var writeTask = clientStream.WriteAsync(buffer1, cancellationTokenSource.Token).AsTask();
                    var readTask = Task.Run(async () =>
                    {
                        int remain = buffer2.Length;
                        while (remain > 0)
                        {
                            int readLength = await serverStream.ReadAsync(buffer2.AsMemory(buffer2.Length - remain, remain), cancellationTokenSource.Token);
                            remain -= readLength;
                        }
                    });
                    await Task.WhenAll(writeTask, readTask);

                    Assert.Equal(buffer1, buffer2);

                    this.Output.WriteLine($"RandomSendAndReceiveTest ({bufferSize}), time: {sb.ElapsedMilliseconds}/ms");
                }
                finally
                {
                    await clientStream.DisposeAsync();
                    await serverStream.DisposeAsync();
                }
            }
        }
        finally
        {
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }

    [Fact]
    public async Task ConnectAndAccept_EstablishesStreamAndCounts()
    {
        var (client, server) = await _testHelper.CreateYamuxMultiplexerPair(this.Logger);

        try
        {
            Assert.Equal(0, client.StreamCount);
            Assert.Equal(0, server.StreamCount);

            var (clientStream, serverStream) = await CreateStreamPairAsync(client, server);

            try
            {
                Assert.Equal((uint)1, clientStream.StreamId);
                Assert.Equal(clientStream.StreamId, serverStream.StreamId);
                Assert.Equal(YamuxStreamState.Established, clientStream.State);
                Assert.Equal(YamuxStreamState.Established, serverStream.State);

                Assert.Equal(1, client.StreamCount);
                Assert.Equal(1, server.StreamCount);
            }
            finally
            {
                await clientStream.DisposeAsync();
                await serverStream.DisposeAsync();
            }
        }
        finally
        {
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }

    [Fact]
    public async Task StreamRoundTrip_ReadsExactPayload()
    {
        var fixtureFactory = new FixtureFactory(0);
        var (client, server) = await _testHelper.CreateYamuxMultiplexerPair(this.Logger);

        try
        {
            var (clientStream, serverStream) = await CreateStreamPairAsync(client, server);

            try
            {
                var payload = fixtureFactory.GenRandomBytes(256);
                await clientStream.WriteAsync(payload, 0, payload.Length);

                var buffer = new byte[payload.Length];
                int read = 0;

                while (read < buffer.Length)
                {
                    int readLength = await serverStream.ReadAsync(buffer, read, buffer.Length - read);
                    Assert.True(readLength > 0, "unexpected EOF");
                    read += readLength;
                }

                Assert.Equal(payload, buffer);
            }
            finally
            {
                await clientStream.DisposeAsync();
                await serverStream.DisposeAsync();
            }
        }
        finally
        {
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }

    [Fact]
    public async Task StreamClose_LocalCloseStopsWriteAndReadReturnsZero()
    {
        var (client, server) = await _testHelper.CreateYamuxMultiplexerPair(this.Logger);

        try
        {
            var (clientStream, serverStream) = await CreateStreamPairAsync(client, server);

            try
            {
                await clientStream.CloseAsync();

                var buffer = new byte[1];
                int localRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                Assert.Equal(0, localRead);

                var exception = await Assert.ThrowsAsync<YamuxException>(async () =>
                {
                    await clientStream.WriteAsync(buffer, 0, buffer.Length);
                });
                Assert.Equal(YamuxErrorCode.StreamClosed, exception.ErrorCode);

                await WaitUntilAsync(() =>
                    serverStream.State == YamuxStreamState.RemoteClose || serverStream.State == YamuxStreamState.Closed,
                    TimeSpan.FromSeconds(5),
                    "server stream did not observe remote close");

                int remoteRead = await serverStream.ReadAsync(buffer, 0, buffer.Length);
                Assert.Equal(0, remoteRead);
            }
            finally
            {
                await clientStream.DisposeAsync();
                await serverStream.DisposeAsync();
            }
        }
        finally
        {
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }

    [Fact]
    public async Task ConnectStreamAsync_CanceledToken_ThrowsOperationCanceledException()
    {
        var (client, server) = await _testHelper.CreateYamuxMultiplexerPair(this.Logger);

        try
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            {
                _ = await client.ConnectStreamAsync(cts.Token);
            });
        }
        finally
        {
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }

    [Fact]
    public async Task AcceptStreamAsync_CanceledToken_ThrowsOperationCanceledException()
    {
        var (client, server) = await _testHelper.CreateYamuxMultiplexerPair(this.Logger);

        try
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            {
                _ = await server.AcceptStreamAsync(cts.Token);
            });
        }
        finally
        {
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }

    [Fact]
    public async Task PingTest()
    {
        var (client, server) = await _testHelper.CreateYamuxMultiplexerPair(this.Logger);

        try
        {
            var clientRTT = await client.PingAsync();
            Assert.True(clientRTT > TimeSpan.Zero);

            var serverRTT = await server.PingAsync();
            Assert.True(serverRTT > TimeSpan.Zero);
        }
        finally
        {
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }

    [Fact]
    public async Task PingTimeoutTest()
    {
        var yamuxOptions = new YamuxOptions() { PingTimeout = TimeSpan.FromSeconds(0) };
        var timeProvider = new FakeTimeProvider(DateTimeOffset.Parse("2020-01-01T00:00:00Z", CultureInfo.InvariantCulture)) { AutoAdvanceAmount = TimeSpan.FromSeconds(10) };

        var (client, server) = await _testHelper.CreateYamuxMultiplexerPair(yamuxOptions, timeProvider, this.Logger);

        try
        {
            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                _ = await client.PingAsync();
            });

            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                _ = await server.PingAsync();
            });
        }
        finally
        {
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }

    private static async Task<(YamuxStream ClientStream, YamuxStream ServerStream)> CreateStreamPairAsync(YamuxMultiplexer client, YamuxMultiplexer server)
    {
        var acceptTask = server.AcceptStreamAsync().AsTask();
        var clientStream = await client.ConnectStreamAsync();
        var serverStream = await acceptTask;
        return (clientStream, serverStream);
    }

    private static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout, string message)
    {
        var stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            if (stopwatch.Elapsed > timeout)
            {
                throw new TimeoutException(message);
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
