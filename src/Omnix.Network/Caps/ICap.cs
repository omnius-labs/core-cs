using System;

namespace Omnix.Network.Caps
{
    public interface ICap : IDisposable, IAsyncDisposable
    {
        bool IsBlocking { get; }
        bool IsConnected { get; }

        bool CanSend();
        bool CanReceive();

        int Send(ReadOnlySpan<byte> buffer);
        int Receive(Span<byte> buffer);
    }
}
