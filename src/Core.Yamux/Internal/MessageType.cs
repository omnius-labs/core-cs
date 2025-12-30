namespace Omnius.Yamux.Internal;

internal enum MessageType : byte
{
    Data = 0,
    WindowUpdate = 1,
    Ping = 2,
    GoAway = 3,
}
