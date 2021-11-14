using System.Buffers;

namespace Omnius.Core.RocketPack.Remoting.Internal;

internal sealed class PacketParser
{
    public static bool TryParse<TMessage, TErrorMessage>(ref ReadOnlySequence<byte> sequence, out ParsedPacketMessage<TMessage, TErrorMessage> result, IBytesPool bytesPool)
        where TMessage : IRocketMessage<TMessage>
        where TErrorMessage : IRocketMessage<TErrorMessage>
    {
        result = default;

        if (sequence.Length == 0) return false;
        if (!Varint.TryGetUInt8(ref sequence, out var type)) return false;

        result = ((PacketType)type) switch
        {
            PacketType.Completed when (sequence.Length == 0) => ParsedPacketMessage<TMessage, TErrorMessage>.CreateCompleted(),
            PacketType.Completed when (sequence.Length != 0) => ParsedPacketMessage<TMessage, TErrorMessage>.CreateCompleted(IRocketMessage<TMessage>.Import(sequence, bytesPool)),
            PacketType.Continue when (sequence.Length == 0) => ParsedPacketMessage<TMessage, TErrorMessage>.CreateContinue(),
            PacketType.Continue when (sequence.Length != 0) => ParsedPacketMessage<TMessage, TErrorMessage>.CreateContinue(IRocketMessage<TMessage>.Import(sequence, bytesPool)),
            PacketType.Error => ParsedPacketMessage<TMessage, TErrorMessage>.CreateError(IRocketMessage<TErrorMessage>.Import(sequence, bytesPool)),
            _ => ParsedPacketMessage<TMessage, TErrorMessage>.CreateUnknown(),
        };

        return !result.IsUnknown;
    }

    public static bool TryParse<TErrorMessage>(ref ReadOnlySequence<byte> sequence, out ParsedPacketMessage<TErrorMessage> result, IBytesPool bytesPool)
        where TErrorMessage : IRocketMessage<TErrorMessage>
    {
        result = default;

        if (sequence.Length == 0) return false;
        if (!Varint.TryGetUInt8(ref sequence, out var type)) return false;

        result = ((PacketType)type) switch
        {
            PacketType.Completed when (sequence.Length == 0) => ParsedPacketMessage<TErrorMessage>.CreateCompleted(),
            PacketType.Continue when (sequence.Length == 0) => ParsedPacketMessage<TErrorMessage>.CreateContinue(),
            PacketType.Error => ParsedPacketMessage<TErrorMessage>.CreateError(IRocketMessage<TErrorMessage>.Import(sequence, bytesPool)),
            _ => ParsedPacketMessage<TErrorMessage>.CreateUnknown(),
        };

        return !result.IsUnknown;
    }
}
