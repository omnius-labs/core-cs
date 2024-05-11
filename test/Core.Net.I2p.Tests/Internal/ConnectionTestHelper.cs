using System.Diagnostics;
using System.Net.Sockets;
using FluentAssertions;

namespace Core.Net.I2p.Internal;

internal static class ConnectionTestHelper
{
    public static async Task RandomSendAndReceiveAsync(Random random, Socket socket1, Socket socket2)
    {
        var caseList = new List<int>();
        caseList.AddRange(Enumerable.Range(1, 4));
        caseList.AddRange(new int[] { 100, 1000, 10000, 1024 * 1024, 1024 * 1024 * 8 });

        foreach (var bufferSize in caseList)
        {
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            random.NextBytes(buffer1);

            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(60));

            var sb = Stopwatch.StartNew();

            var task1 = Task.Run(async () =>
            {
                int offset = 0;

                for (; ; )
                {
                    int sentBytes = await socket1.SendAsync(buffer1.AsMemory(offset), SocketFlags.None);
                    offset += sentBytes;
                    if (offset == bufferSize) break;
                }
            });

            var task2 = Task.Run(async () =>
            {
                int offset = 0;

                for (; ; )
                {
                    int receivedBytes = await socket2.ReceiveAsync(buffer2.AsMemory(offset), SocketFlags.None);
                    offset += receivedBytes;
                    if (offset == bufferSize) break;
                }
            });

            await Task.WhenAll(task1, task2);

            buffer1.Should().Equal(buffer2);

            Debug.WriteLine($"RandomSendAndReceiveTest ({bufferSize}), time: {sb.ElapsedMilliseconds}/ms");
        }
    }
}
