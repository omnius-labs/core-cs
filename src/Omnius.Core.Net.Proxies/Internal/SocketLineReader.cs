using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Omnius.Core.Net.Proxies.Internal
{
    internal class SocketLineReader : DisposableBase
    {
        private readonly Socket _socket;
        private readonly Encoding _encoding;

        public SocketLineReader(Socket socket, Encoding encoding)
        {
            _socket = socket;
            _encoding = encoding;
        }

        public string ReadLineAsync(CancellationToken cancellationToken = default)
        {
            using (var stream = new MemoryStream())
            {
                Span<byte> buffer = stackalloc byte[1];

                for (; ; )
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int receiveLength = _socket.Receive(buffer);
                    if (receiveLength < 1) break;

                    stream.Write(buffer);

                    if (buffer[0] == '\n') break;
                }

                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream, _encoding))
                {
                    return reader.ReadToEnd().TrimEnd('\r', '\n');
                }
            }
        }

        protected override void OnDispose(bool isDisposing)
        {
        }
    }
}
