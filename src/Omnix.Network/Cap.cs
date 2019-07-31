using System;
using Omnix.Base;

namespace Omnix.Network
{
    public abstract class Cap : DisposableBase
    {
        public abstract bool IsBlocking { get; }
        public abstract bool IsConnected { get; }

        public abstract bool CanReceive();
        public abstract bool CanSend();

        public abstract int Receive(Span<byte> buffer);
        public abstract int Send(ReadOnlySpan<byte> buffer);
    }

    public class CapException : Exception
    {
        public CapException() : base() { }
        public CapException(string message) : base(message) { }
        public CapException(string message, Exception innerException) : base(message, innerException) { }
    }
}
