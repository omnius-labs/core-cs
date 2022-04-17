using System.Net.Sockets;
using System.Text;

namespace Omnius.Core.Net.I2p.Internal
{
    internal sealed class SocketLineReader
    {
        private Socket _socket;
        private Encoding _encoding;

        public SocketLineReader(Socket socket, Encoding encoding)
        {
            _socket = socket;
            _encoding = encoding;
        }

        public async ValueTask<string?> ReadLineAsync(CancellationToken cancellationToken = default)
        {
            using var stream = new MemoryStream();

            var buffer = new byte[1];

            for (; ; )
            {
                var receivedBytes = await _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
                if (receivedBytes == 0) break;

                stream.Write(buffer, 0, 1);
                if (buffer[0] == '\n') break;
            }

            stream.Seek(0, SeekOrigin.Begin);

            if (stream.Length == 0) return null;

            using (var reader = new StreamReader(stream, _encoding))
            {
                return reader.ReadToEnd().TrimEnd('\r', '\n');
            }
        }
    }
}
