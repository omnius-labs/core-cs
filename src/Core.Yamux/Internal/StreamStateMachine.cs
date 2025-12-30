using Omnius.Core.Base;

namespace Omnius.Yamux.Internal;

internal sealed class StreamStateMachine
{
    private YamuxStreamState _state;
    private readonly AsyncLock _asyncLock = new();

    public StreamStateMachine(YamuxStreamState initialState)
    {
        _state = initialState;
    }

    public YamuxStreamState State => _state;

    public MessageFlag ComputeSendFlags()
    {
        using (_asyncLock.Lock())
        {
            switch (_state)
            {
                case YamuxStreamState.Init:
                    _state = YamuxStreamState.SYNSent;
                    return MessageFlag.SYN;
                case YamuxStreamState.SYNReceived:
                    _state = YamuxStreamState.Established;
                    return MessageFlag.ACK;
                default:
                    return MessageFlag.None;
            }
        }
    }

    public StateChange ProcessReceivedFlags(MessageFlag flags)
    {
        bool notifyWaiters = false;
        bool notifyEstablished = false;
        bool removeStream = false;
        bool stopCloseTimer = false;

        using (_asyncLock.Lock())
        {
            if (flags.HasFlag(MessageFlag.ACK))
            {
                if (_state == YamuxStreamState.SYNSent) _state = YamuxStreamState.Established;
                notifyEstablished = true;
            }

            if (flags.HasFlag(MessageFlag.FIN))
            {
                switch (_state)
                {
                    case YamuxStreamState.SYNSent:
                    case YamuxStreamState.SYNReceived:
                    case YamuxStreamState.Established:
                        _state = YamuxStreamState.RemoteClose;
                        notifyWaiters = true;
                        break;
                    case YamuxStreamState.LocalClose:
                        _state = YamuxStreamState.Closed;
                        notifyWaiters = true;
                        removeStream = true;
                        stopCloseTimer = true;
                        break;
                    default:
                        throw new YamuxException(YamuxErrorCode.Unexpected);
                }
            }

            if (flags.HasFlag(MessageFlag.RST))
            {
                _state = YamuxStreamState.Reset;
                notifyWaiters = true;
                removeStream = true;
                stopCloseTimer = true;
            }
        }

        return new StateChange()
        {
            NotifyWaiters = notifyWaiters,
            NotifyEstablished = notifyEstablished,
            RemoveStream = removeStream,
            StopCloseTimer = stopCloseTimer,
        };
    }

    public StateChange CloseLocal()
    {
        bool removeStream = false;
        bool startCloseTimer = false;
        bool stopCloseTimer = false;

        using (_asyncLock.Lock())
        {
            switch (_state)
            {
                case YamuxStreamState.SYNSent:
                case YamuxStreamState.SYNReceived:
                case YamuxStreamState.Established:
                    _state = YamuxStreamState.LocalClose;
                    startCloseTimer = true;
                    break;
                case YamuxStreamState.Closed:
                    break;
                case YamuxStreamState.RemoteClose:
                    _state = YamuxStreamState.Closed;
                    removeStream = true;
                    stopCloseTimer = true;
                    break;
                default:
                    throw new YamuxException(YamuxErrorCode.Unexpected);
            }
        }

        return new StateChange()
        {
            NotifyWaiters = true,
            RemoveStream = removeStream,
            StartCloseTimer = startCloseTimer,
            StopCloseTimer = stopCloseTimer,
        };
    }

    public void ForceClose()
    {
        using (_asyncLock.Lock())
        {
            _state = YamuxStreamState.Closed;
        }
    }
}

internal readonly struct StateChange
{
    public bool NotifyWaiters { get; init; }
    public bool NotifyEstablished { get; init; }
    public bool RemoveStream { get; init; }
    public bool StartCloseTimer { get; init; }
    public bool StopCloseTimer { get; init; }
}
