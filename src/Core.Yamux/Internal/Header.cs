using System.Buffers.Binary;

namespace Omnius.Yamux.Internal;

internal class Header
{
    private byte[] _value;

    public Header()
    {
        _value = new byte[Constants.HeaderSize.TOTAL];
    }

    public Header(byte[] value)
    {
        _value = value;
    }

    public Header(MessageType messageType, MessageFlag messageFlag, uint streamId, uint length)
    {
        _value = new byte[Constants.HeaderSize.TOTAL];
        this.encode(messageType, messageFlag, streamId, length);
    }

    public byte Version => _value[0];
    public MessageType Type => (MessageType)_value[1];
    public MessageFlag Flags => (MessageFlag)BinaryPrimitives.ReadUInt16BigEndian(_value.AsSpan(2, 2));
    public uint StreamId => BinaryPrimitives.ReadUInt32BigEndian(_value.AsSpan(4, 4));
    public uint Length => BinaryPrimitives.ReadUInt32BigEndian(_value.AsSpan(8, 4));

    public void encode(MessageType messageType, MessageFlag messageFlag, uint streamId, uint length)
    {
        _value[0] = Constants.PROTO_VERSION;
        _value[1] = (byte)messageType;
        BinaryPrimitives.WriteUInt16BigEndian(_value.AsSpan(2, 2), (ushort)messageFlag);
        BinaryPrimitives.WriteUInt32BigEndian(_value.AsSpan(4, 4), streamId);
        BinaryPrimitives.WriteUInt32BigEndian(_value.AsSpan(8, 4), length);
    }

    public byte[] GetBytes()
    {
        return _value;
    }
}
