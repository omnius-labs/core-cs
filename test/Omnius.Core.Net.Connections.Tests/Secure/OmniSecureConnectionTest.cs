using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Tasks;
using Xunit;

namespace Omnius.Core.Net.Connections.Secure
{
    public class OmniSecureConnectionTest
    {
        [Fact]
        public async Task RandomSendAndReceiveTest()
        {
            var random = new Random();

            var caseList = new List<int>();
            caseList.AddRange(Enumerable.Range(1, 32));
            caseList.AddRange(new int[] { 100, 1000, 10000, 1024 * 1024 });

            var (socket1, socket2) = SocketHelper.GetSocketPair();

            var batchActionDispatcher = new BatchActionDispatcher();
            var options = new BaseConnectionOptions(1024 * 1024 * 256, null, null, batchActionDispatcher, BytesPool.Shared);

            await using var baseConnection1 = new BaseConnection(new SocketCap(socket1), options);
            await using var baseConnection2 = new BaseConnection(new SocketCap(socket2), options);
            await using var connection1 = new OmniSecureConnection(baseConnection1, new OmniSecureConnectionOptions(1024 * 1024 * 256, OmniSecureConnectionType.Connected, null, batchActionDispatcher, BytesPool.Shared));
            await using var connection2 = new OmniSecureConnection(baseConnection2, new OmniSecureConnectionOptions(1024 * 1024 * 256, OmniSecureConnectionType.Accepted, null, batchActionDispatcher, BytesPool.Shared));

            // ハンドシェイクを行う
            {
                var valueTask1 = connection1.HandshakeAsync();
                var valueTask2 = connection2.HandshakeAsync();

                var stopwatch = Stopwatch.StartNew();

                while (!valueTask1.IsCompleted || !valueTask2.IsCompleted)
                {
                    if (stopwatch.Elapsed.TotalSeconds > 60) throw new TimeoutException("Handshake");

                    await Task.Delay(100).ConfigureAwait(false);
                }
            }

            foreach (var bufferSize in caseList)
            {
                var buffer1 = new byte[bufferSize];
                var buffer2 = new byte[bufferSize];

                random.NextBytes(buffer1);

                var valueTask1 = connection1.Sender.SendAsync((bufferWriter) =>
                {
                    bufferWriter.Write(buffer1);
                });

                var valueTask2 = connection2.Receiver.ReceiveAsync((sequence) =>
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
            }
        }
    }
}
