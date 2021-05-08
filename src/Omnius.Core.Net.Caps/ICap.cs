using System;

namespace Omnius.Core.Net.Caps
{
    public interface ICap : IDisposable
    {
        bool IsConnected { get; }

        bool CanSend();

        bool CanReceive();

        int Send(ReadOnlySpan<byte> buffer);

        int Receive(Span<byte> buffer);
    }
}
