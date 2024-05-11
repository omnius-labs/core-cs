using System.Buffers;
using Core.Base;

namespace Core.RocketPack.Remoting.Internal;

internal struct PacketBuilder
{
    private readonly IBufferWriter<byte> _bufferWriter;
    private readonly IBytesPool _bytesPool;

    public PacketBuilder(IBufferWriter<byte> bufferWriter, IBytesPool bytesPool)
    {
        _bufferWriter = bufferWriter;
        _bytesPool = bytesPool;
    }

    public void WriteCompleted<TMessage>(TMessage message)
        where TMessage : IRocketMessage<TMessage>
    {
        var type = (byte)PacketType.Completed;
        Varint.SetUInt8(in type, in _bufferWriter);
        message.Export(_bufferWriter, _bytesPool);
    }

    public void WriteCompleted()
    {
        var type = (byte)PacketType.Completed;
        Varint.SetUInt8(in type, in _bufferWriter);
    }

    public void WriteContinue<TMessage>(TMessage message)
        where TMessage : IRocketMessage<TMessage>
    {
        var type = (byte)PacketType.Continue;
        Varint.SetUInt8(in type, in _bufferWriter);
        message.Export(_bufferWriter, _bytesPool);
    }

    public void WriteContinueAsync(CancellationToken cancellationToken = default)
    {
        var type = (byte)PacketType.Continue;
        Varint.SetUInt8(in type, in _bufferWriter);
    }

    public void WriteError<TErrorMessage>(TErrorMessage errorMessage)
        where TErrorMessage : IRocketMessage<TErrorMessage>
    {
        var type = (byte)PacketType.Completed;
        Varint.SetUInt8(in type, in _bufferWriter);
        errorMessage.Export(_bufferWriter, _bytesPool);
    }
}
