using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Network;
using Omnix.Network.Connection;
using Omnix.Network.Connection.Tests.Internal;
using Xunit;

namespace Omnix.Network.Connection.Tests
{
    public class BaseNonblockingConnectionTests
    {
        [Fact]
        public void RandomSendAndReceiveTest()
        {
            var random = new Random();

            var caseList = new List<int>();
            caseList.AddRange(Enumerable.Range(1, 32));
            caseList.AddRange(new int[] { 100, 1000, 10000, 1024 * 1024, 1024 * 1024 * 32 });

            var (socket1, socket2) = SocketHelpers.GetSockets();

            var options = new OmniNonblockingConnectionOptions()
            {
                MaxReceiveByteCount = 1024 * 1024 * 256,
                MaxSendByteCount = 1024 * 1024 * 256,
                BufferPool = BufferPool.Shared,
            };

            using (var connection1 = new OmniNonblockingConnection(new SocketCap(socket1, false), options))
            using (var connection2 = new OmniNonblockingConnection(new SocketCap(socket2, false), options))
            {
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

                        Thread.Sleep(100);

                        connection1.DoEvents();
                        connection2.DoEvents();
                    }

                    Assert.True(BytesOperations.SequenceEqual(buffer1, buffer2));
                }
            }
        }
    }
}
