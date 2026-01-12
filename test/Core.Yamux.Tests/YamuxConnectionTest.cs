using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Time.Testing;
using Omnius.Core.Testkit;
using Omnius.Yamux.Internal;
using Xunit;
using Xunit.Abstractions;

namespace Omnius.Yamux;

public class YamuxConnectionTest : TestBase<YamuxConnectionTest>
{
    private readonly TestHelper _testHelper;

    public YamuxConnectionTest(ITestOutputHelper output) : base(output)
    {
        _testHelper = new TestHelper(output);
    }

    [Fact]
    public async Task RandomTest()
    {
        var (client, server) = await _testHelper.CreateYamuxConnectionPair(this.Logger);

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
                var acceptTask = server.AcceptStreamAsync().AsTask();
                var clientStream = await client.ConnectStreamAsync();
                YamuxStream? serverStream = null;

                try
                {
                    var buffer1 = new byte[bufferSize];
                    var buffer2 = new byte[bufferSize];

                    random.NextBytes(buffer1);

                    using var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(3000));

                    var sb = Stopwatch.StartNew();

                    var writeTask = clientStream.WriteAsync(buffer1, cancellationTokenSource.Token).AsTask();
                    serverStream = await acceptTask;
                    Assert.NotNull(serverStream);

                    var readTask = Task.Run(async () =>
                    {
                        int remain = buffer2.Length;
                        while (remain > 0)
                        {
                            int readLength = await serverStream.ReadAsync(
                                buffer2.AsMemory(buffer2.Length - remain, remain),
                                cancellationTokenSource.Token);
                            if (readLength == 0) throw new EndOfStreamException("Unexpected EOF during RandomTest");

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
                    if (serverStream is not null)
                    {
                        await serverStream.DisposeAsync();
                    }
                }
            }
        }
        finally
        {
            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }
}
