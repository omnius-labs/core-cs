namespace Omnius.Yamux.Internal;

internal sealed class StreamFlowControl
{
    private readonly uint _maxWindow;
    private uint _receiveWindow;
    private uint _sendWindow;

    public StreamFlowControl(uint maxWindow)
    {
        _maxWindow = maxWindow;
    }

    public uint SendWindow => _sendWindow;

    public void AddSendWindow(uint delta)
    {
        _sendWindow += delta;
    }

    public void ConsumeSendWindow(uint length)
    {
        _sendWindow -= length;
    }

    public bool CanConsumeReceiveWindow(uint length)
    {
        return length <= _receiveWindow;
    }

    public void ConsumeReceiveWindow(uint length)
    {
        _receiveWindow -= length;
    }

    public uint? TryUpdateReceiveWindowAndGetDelta(uint bufferedBytes, bool force)
    {
        uint delta = (_maxWindow - bufferedBytes) - _receiveWindow;

        if (!force && delta < (_maxWindow / 2))
        {
            return null;
        }

        _receiveWindow += delta;

        return delta;
    }
}
