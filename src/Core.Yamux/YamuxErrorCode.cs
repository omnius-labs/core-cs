namespace Omnius.Yamux;

public enum YamuxErrorCode
{
    None,

    InvalidFrameType,
    ConnectionReceiveError,
    ConnectionSendError,
    ConnectionShutdown,

    StreamNotFound,
    DuplicateStreamId,
    StreamsExhausted,
    StreamReceiveWindowExceeded,
    StreamReset,
    StreamClosed,

    RemoteGoAway,
    Timeout,
    InvalidVersion,
    ProtocolError,
    InternalError,
    Unexpected,
}
