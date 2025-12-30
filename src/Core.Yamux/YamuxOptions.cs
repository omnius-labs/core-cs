using System;
using Omnius.Yamux.Internal;

namespace Omnius.Yamux;

public record YamuxOptions
{
    public int AcceptBacklog { get; init; } = 100;
    public bool EnableKeepAlive { get; init; } = false;
    public TimeSpan KeepAliveInterval { get; init; } = TimeSpan.FromSeconds(10);
    public uint MaxStreamWindow { get; init; } = 1024 * 1024;
    public TimeSpan PingTimeout { get; init; } = TimeSpan.FromSeconds(5);
    public TimeSpan StreamCloseTimeout { get; init; } = TimeSpan.FromSeconds(5);
    public TimeSpan StreamWriteTimeout { get; init; } = TimeSpan.FromSeconds(10);

    public void Verify()
    {
        if (this.AcceptBacklog < 0) throw new ArgumentOutOfRangeException(nameof(this.AcceptBacklog));
        if (this.KeepAliveInterval < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(this.KeepAliveInterval));
        if (this.MaxStreamWindow < Constants.INITIAL_STREAM_WINDOW) throw new ArgumentOutOfRangeException(nameof(this.MaxStreamWindow));
    }
}
