using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnius.Core.Net.Connections.Internal;

internal static class TestHelper
{
    public static async Task RandomSendAndReceive(Random random, IConnection connection1, IConnection connection2)
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

            var valueTask1 = connection1.Sender.SendAsync((bufferWriter) =>
            {
                bufferWriter.Write(buffer1);
            }, cancellationTokenSource.Token);

            var valueTask2 = connection2.Receiver.ReceiveAsync((sequence) =>
            {
                sequence.CopyTo(buffer2);
            }, cancellationTokenSource.Token);

            await Task.WhenAll(valueTask1.AsTask(), valueTask2.AsTask());

            Assert.Equal(buffer1, buffer2);

            Debug.WriteLine($"RandomSendAndReceiveTest ({bufferSize}), time: {sb.ElapsedMilliseconds}/ms");
        }
    }
}