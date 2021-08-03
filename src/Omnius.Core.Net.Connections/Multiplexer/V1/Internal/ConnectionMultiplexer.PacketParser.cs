using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal partial class ConnectionMultiplexer
    {
        internal sealed class PacketParser
        {
            public static uint ParseStreamId(ref ReadOnlySequence<byte> sequence)
            {
                Span<byte> streamIdBytes = stackalloc byte[4];
                sequence.CopyTo(streamIdBytes);
                var streamId = BinaryPrimitives.ReadUInt32BigEndian(streamIdBytes);
                sequence = sequence.Slice(4);
                return streamId;
            }

            public static PacketType ParseMessageType(ref ReadOnlySequence<byte> sequence)
            {
                Span<byte> messageTypeBytes = stackalloc byte[4];
                sequence.CopyTo(messageTypeBytes);
                var messageType = (PacketType)BinaryPrimitives.ReadUInt32BigEndian(messageTypeBytes);
                sequence = sequence.Slice(4);
                return messageType;
            }

            public static ErrorCode ParseErrorCode(ref ReadOnlySequence<byte> sequence)
            {
                Span<byte> messageTypeBytes = stackalloc byte[4];
                sequence.CopyTo(messageTypeBytes);
                var status = (ErrorCode)BinaryPrimitives.ReadUInt32BigEndian(messageTypeBytes);
                sequence = sequence.Slice(4);
                return status;
            }
        }
    }
}
