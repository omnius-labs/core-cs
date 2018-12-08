using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Omnix.Network.Connection.Tests.Internal
{
    internal static class SocketHelpers
    {
        private static object _lockObject = new object();

        public static (Socket, Socket) GetSockets()
        {
            lock (_lockObject)
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

                listener.Stop();

                return (socket1, socket2);
            }
        }
    }
}

