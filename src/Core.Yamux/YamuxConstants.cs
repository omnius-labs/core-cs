namespace Omnius.Yamux;

public static class YamuxConstants
{
    public const uint DefaultCredit = 256u * 1024;
    public const int DefaultSplitSendSize = 16 * 1024;
    public const int MaxAckBacklog = 256;

    public const int HeaderSize = 12;
    public const int MaxFrameBodyLength = 1024 * 1024;
}
