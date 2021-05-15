using System;
using System.Net.Sockets;

namespace Omnius.Core.Net.Caps
{
    public sealed class SocketCap : DisposableBase, ICap
    {
        private readonly Socket _socket;

        private volatile bool _isConnected;

        public SocketCap(Socket socket)
        {
            if (socket == null) throw new ArgumentNullException(nameof(socket));
            if (!socket.Connected) throw new ArgumentException("Socket is not connected.");

            _socket = socket;
            _socket.Blocking = false;
            _socket.ReceiveBufferSize = 1024 * 32;
            _socket.SendBufferSize = 1024 * 32;
            _socket.NoDelay = true;

            _isConnected = true;
        }

        public Socket Socket => _socket;

        public bool IsBlocking => _socket.Blocking;

        public bool IsConnected => _isConnected && !(_socket.Poll(0, SelectMode.SelectRead) && (_socket.Available == 0));

        public bool CanSend()
        {
            return _socket.Poll(1, SelectMode.SelectWrite);
        }

        public bool CanReceive()
        {
            return _socket.Poll(1, SelectMode.SelectRead);
        }

        public int Send(ReadOnlySpan<byte> buffer)
        {
            this.ThrowIfDisposingRequested();

            if (!_isConnected) throw new CapException("Closed");

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

        public int Receive(Span<byte> buffer)
        {
            this.ThrowIfDisposingRequested();

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

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _socket.Dispose();
                _isConnected = false;
            }
        }
    }
}
