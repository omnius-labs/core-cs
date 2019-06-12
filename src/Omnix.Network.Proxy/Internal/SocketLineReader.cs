using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Omnix.Base;

namespace Omnix.Network.Proxy.Internal
{
    class SocketLineReader : DisposableBase
    {
        private Socket _socket;
        private Encoding _encoding;

        public SocketLineReader(Socket socket, Encoding encoding)
        {
            _socket = socket;
            _encoding = encoding;
        }

        public string ReadLineAsync(CancellationToken token = default)
        {
            using (var stream = new MemoryStream())
            {
                Span<byte> buffer = stackalloc byte[1];

                for (;;)
                {
                    token.ThrowIfCancellationRequested();

                    int receiveLength = _socket.Receive(buffer);
                    if (receiveLength < 1)
                    {
                        break;
                    }

                    stream.Write(buffer);

                    if (buffer[0] == '\n')
                    {
                        break;
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream, _encoding))
                {
                    return reader.ReadToEnd().TrimEnd('\r', '\n');
                }
            }
        }

        protected override void Dispose(bool isDisposing)
        {

        }
    }
}
