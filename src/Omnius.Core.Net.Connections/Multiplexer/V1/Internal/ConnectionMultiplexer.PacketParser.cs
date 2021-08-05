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
            public static bool TryParseStreamId(ref ReadOnlySequence<byte> sequence, out uint streamId)
            {
                streamId = 0;
                if (sequence.Length < 4) return false;

                Span<byte> streamIdBytes = stackalloc byte[4];
                sequence.Slice(0, 4).CopyTo(streamIdBytes);
                streamId = BinaryPrimitives.ReadUInt32BigEndian(streamIdBytes);

                sequence = sequence.Slice(4);
                return true;
            }

            public static bool TryParsePacketType(ref ReadOnlySequence<byte> sequence, out PacketType type)
            {
                type = PacketType.KeepAlive;
                if (sequence.Length < 4) return false;

                Span<byte> packetTypeBytes = stackalloc byte[4];
                sequence.Slice(0, 4).CopyTo(packetTypeBytes);
                type = (PacketType)BinaryPrimitives.ReadUInt32BigEndian(packetTypeBytes);

                sequence = sequence.Slice(4);
                return true;
            }

            public static bool TryParseErrorCode(ref ReadOnlySequence<byte> sequence, out ErrorCode errorCode)
            {
                errorCode = ErrorCode.Normal;
                if (sequence.Length < 4) return false;

                Span<byte> messageTypeBytes = stackalloc byte[4];
                sequence.Slice(0, 4).CopyTo(messageTypeBytes);
                errorCode = (ErrorCode)BinaryPrimitives.ReadUInt32BigEndian(messageTypeBytes);

                sequence = sequence.Slice(4);
                return true;
            }
        }
    }
}
