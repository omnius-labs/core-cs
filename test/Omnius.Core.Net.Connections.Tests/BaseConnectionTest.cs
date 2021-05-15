using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Internal;
using Xunit;

namespace Omnius.Core.Net.Connections
{
    public class BaseConnectionTest
    {
        [Fact]
        public async Task RandomSendAndReceiveTest()
        {
            var random = new Random();

            var caseList = new List<int>();
            caseList.AddRange(Enumerable.Range(1, 32));
            caseList.AddRange(new int[] { 100, 1000, 10000, 1024 * 1024 });

            var (socket1, socket2) = SocketHelper.GetSocketPair();

            var dispatcherOptions = new BaseConnectionDispatcherOptions()
            {
                MaxSendBytesPerSeconds = 1024 * 1024 * 32,
                MaxReceiveBytesPerSeconds = 1024 * 1024 * 32,
            };

            var options = new BaseConnectionOptions()
            {
                MaxReceiveByteCount = 1024 * 1024 * 256,
                BytesPool = BytesPool.Shared,
            };

            await using var dispatcher = new BaseConnectionDispatcher(dispatcherOptions);
            using var connection1 = new BaseConnection(new SocketCap(socket1), dispatcher, options);
            using var connection2 = new BaseConnection(new SocketCap(socket2), dispatcher, options);

            foreach (var bufferSize in caseList)
            {
                var buffer1 = new byte[bufferSize];
                var buffer2 = new byte[bufferSize];

                random.NextBytes(buffer1);

                var valueTask1 = connection1.EnqueueAsync((bufferWriter) =>
                {
                    bufferWriter.Write(buffer1);
                });

                var valueTask2 = connection2.DequeueAsync((sequence) =>
                {
                    sequence.CopyTo(buffer2);
                });

                var stopwatch = Stopwatch.StartNew();

                while (!valueTask1.IsCompleted || !valueTask2.IsCompleted)
                {
                    if (stopwatch.Elapsed.TotalSeconds > 60) throw new TimeoutException("SendAndReceive");

                    await Task.Delay(100).ConfigureAwait(false);
                }

                Assert.Equal(buffer1, buffer2);
                Debug.WriteLine($"BaseConnectionTests RandomSendAndReceiveTest ({bufferSize})");
            }
        }
    }
}
