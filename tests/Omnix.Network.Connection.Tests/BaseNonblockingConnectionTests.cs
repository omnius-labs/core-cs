using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Network;
using Omnix.Network.Connection;
using Xunit;

namespace Omnix.Network.Connection.Tests
{
    public class BaseNonblockingConnectionTests
    {
        private (Socket, Socket) GetSockets()
        {
            Socket socket1, socket2;

            var listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30000));
            listener.Start();
            var acceptSocketTask = listener.AcceptSocketAsync();

            var client = new TcpClient();
            client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 30000).Wait();

            var server = acceptSocketTask.Result;
            listener.Stop();

            socket1 = client.Client;
            socket2 = server;

            return (socket1, socket2);
        }

        [Fact]
        public void RandomSendAndReceiveeTest()
        {
            var random = new Random();
            var caseList = new int[] { 1, 5, 10, 100, 1000, 10000, 1024 * 1024 };

            foreach (var bufferSize in caseList)
            {
                var (socket1, socket2) = this.GetSockets();

                using (var connection1 = new BaseNonblockingConnection(new SocketCap(socket1, false), 1024 * 1024 * 256, BufferPool.Shared))
                using (var connection2 = new BaseNonblockingConnection(new SocketCap(socket2, false), 1024 * 1024 * 256, BufferPool.Shared))
                {
                    var buffer1 = new byte[bufferSize];
                    var buffer2 = new byte[bufferSize];

                    random.NextBytes(buffer1);

                    using (var semaphore = new SemaphoreSlim(0, 1))
                    {
                        var valueTask1 = connection1.EnqueueAsync((bufferWriter) =>
                        {
                            bufferWriter.Write(buffer1);
                        });

                        var valueTask2 = connection2.DequeueAsync((sequence) =>
                        {
                            sequence.CopyTo(buffer2);
                            semaphore.Release();
                        });

                        var task3 = Task.Run(() =>
                        {
                            while (!semaphore.Wait(300))
                            {
                                connection1.Send(1024 * 1024);
                                connection2.Receive(1024 * 1024);
                            }
                        });

                        Task.WaitAll(valueTask1.AsTask(), valueTask2.AsTask(), task3);

                        Assert.True(BytesOperations.SequenceEqual(buffer1, buffer2));
                    }
                }
            }
        }
    }
}
