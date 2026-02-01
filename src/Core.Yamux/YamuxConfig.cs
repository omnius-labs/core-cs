namespace Omnius.Core.Yamux;

public sealed class YamuxConfig
{
    public int? MaxConnectionReceiveWindow { get; private set; } = 1024 * 1024 * 1024;
    public int MaxNumStreams { get; private set; } = 512;
    public bool ReadAfterClose { get; private set; } = true;
    public int SplitSendSize { get; private set; } = YamuxConstants.DefaultSplitSendSize;

    public YamuxConfig SetMaxConnectionReceiveWindow(int? bytes)
    {
        if (bytes is <= 0) throw new ArgumentOutOfRangeException(nameof(bytes));

        this.MaxConnectionReceiveWindow = bytes;
        this.EnsureWindowLimits();
        return this;
    }

    public YamuxConfig SetMaxNumStreams(int value)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

        this.MaxNumStreams = value;
        this.EnsureWindowLimits();
        return this;
    }

    public YamuxConfig SetReadAfterClose(bool value)
    {
        this.ReadAfterClose = value;
        return this;
    }

    public YamuxConfig SetSplitSendSize(int value)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

        this.SplitSendSize = value;
        return this;
    }

    internal void EnsureWindowLimits()
    {
        if (this.MaxConnectionReceiveWindow is int maxConnectionReceiveWindow)
        {
            var required = this.MaxNumStreams * YamuxConstants.DefaultCredit;
            if (maxConnectionReceiveWindow < required)
            {
                throw new ArgumentException("MaxConnectionReceiveWindow must be >= 256KiB * MaxNumStreams.");
            }
        }
    }
}
