namespace Omnius.Yamux.Internal;

internal enum GoAwayCode
{
    None = -1,
    Normal = 0,
    ProtocolError = 1,
    InternalError = 2,
}
