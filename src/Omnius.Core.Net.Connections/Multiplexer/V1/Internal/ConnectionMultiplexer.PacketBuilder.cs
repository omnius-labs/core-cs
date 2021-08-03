using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal partial class ConnectionMultiplexer
    {
        internal struct PacketBuilder
        {
            private readonly IBufferWriter<byte> _bufferWriter;

            public PacketBuilder(IBufferWriter<byte> bufferWriter)
            {
                _bufferWriter = bufferWriter;
            }

            public void WriteKeepAlive()
            {
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), (uint)PacketType.KeepAlive);
                _bufferWriter.Advance(4);
            }

            public void WriteStreamRequest()
            {
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), (uint)PacketType.StreamRequest);
                _bufferWriter.Advance(4);
            }

            public void WriteStreamRequestAccepted(uint streamId)
            {
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), (uint)PacketType.StreamRequestAccepted);
                _bufferWriter.Advance(4);
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), streamId);
                _bufferWriter.Advance(4);
            }

            public void WriteStreamData(uint streamId, ArraySegment<byte> payload)
            {
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), (uint)PacketType.StreamData);
                _bufferWriter.Advance(4);
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), streamId);
                _bufferWriter.Advance(4);
                _bufferWriter.Write(payload.AsSpan());
            }

            public void WriteStreamDataAccepted(uint streamId)
            {
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), (uint)PacketType.StreamDataAccepted);
                _bufferWriter.Advance(4);
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), streamId);
                _bufferWriter.Advance(4);
            }

            public void WriteStreamFinish(uint streamId)
            {
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), (uint)PacketType.StreamFinish);
                _bufferWriter.Advance(4);
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), streamId);
                _bufferWriter.Advance(4);
            }

            public void WriteSessionError(ErrorCode status)
            {
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), (uint)PacketType.SessionError);
                _bufferWriter.Advance(4);
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), (uint)status);
                _bufferWriter.Advance(4);
            }

            public void WriteSessionFinish()
            {
                BinaryPrimitives.WriteUInt32BigEndian(_bufferWriter.GetSpan(4), (uint)PacketType.SessionFinish);
                _bufferWriter.Advance(4);
            }
        }
    }
}
