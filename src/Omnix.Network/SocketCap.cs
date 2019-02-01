using System;
using System.Net.Sockets;

namespace Omnix.Network
{
    public class SocketCap : Cap
    {
        private Socket _socket;

        private volatile bool _isConnected;

        private volatile bool _disposed;

        public SocketCap(Socket socket, bool blocking)
        {
            if (socket == null) throw new ArgumentNullException(nameof(socket));
            if (!socket.Connected) throw new ArgumentException("Socket is not connected.");

            _socket = socket;
            _socket.Blocking = blocking;
            _socket.ReceiveBufferSize = 1024 * 32;
            _socket.SendBufferSize = 1024 * 32;
            _socket.NoDelay = true;

            _isConnected = true;
        }

        public Socket Socket => _socket;

        public override bool IsBlocking => _socket.Blocking;
        public override bool IsConnected => !(_socket.Poll(0, SelectMode.SelectRead) && (_socket.Available == 0));

        public override bool CanReceive()
        {
            return _socket.Poll(1, SelectMode.SelectRead);
        }

        public override bool CanSend()
        {
            return _socket.Poll(1, SelectMode.SelectWrite);
        }

        public override int Receive(Span<byte> buffer)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
            if (!_isConnected) throw new CapException("Closed");

            try
            {
                return _socket.Receive(buffer, SocketFlags.None);
            }
            catch (Exception e)
            {
                _isConnected = false;

                throw new CapException("Receive", e);
            }
        }

        public override int Send(ReadOnlySpan<byte> buffer)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
            if (!_isConnected) throw new CapException();

            try
            {
                return _socket.Send(buffer, SocketFlags.None);
            }
            catch (Exception e)
            {
                _isConnected = false;

                throw new CapException("Send", e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                _socket?.Dispose();
                _socket = null;

                _isConnected = false;
            }
        }
    }
}
