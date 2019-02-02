using System;
using System.Buffers;
using System.Collections.Generic;
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

            using (var connection1 = new NonblockingConnection(new SocketCap(socket1, false), 1024 * 1024 * 256, BufferPool.Shared))
            using (var connection2 = new NonblockingConnection(new SocketCap(socket2, false), 1024 * 1024 * 256, BufferPool.Shared))
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

                    while (!valueTask1.IsCompleted || !valueTask2.IsCompleted)
                    {
                        Thread.Sleep(100);

                        connection1.Send(1024 * 1024);
                        connection1.Receive(1024 * 1024);
                        connection2.Send(1024 * 1024);
                        connection2.Receive(1024 * 1024);
                    }

                    Assert.True(BytesOperations.SequenceEqual(buffer1, buffer2));
                }
            }
        }
    }
}
