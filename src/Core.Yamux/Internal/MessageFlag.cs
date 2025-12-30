namespace Omnius.Yamux.Internal;

[Flags]
internal enum MessageFlag : ushort
{
    None = 0,
    SYN = 1 << 0,
    ACK = 1 << 1,
    FIN = 1 << 2,
    RST = 1 << 3,
}
