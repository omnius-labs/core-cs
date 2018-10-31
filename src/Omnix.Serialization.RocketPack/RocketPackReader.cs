using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading;
using Omnix.Base;

namespace Omnix.Serialization.RocketPack
{
    /// <summary>
    /// RocketPackフォーマットのデシリアライズ機能を提供します。
    /// </summary>
    public class RocketPackReader
    {
        private ReadOnlySequence<byte> _sequence;
        private BufferPool _bufferPool;

        private static readonly ThreadLocal<Encoding> _encoding = new ThreadLocal<Encoding>(() => new UTF8Encoding(false));

        public RocketPackReader(ReadOnlySequence<byte> sequence, BufferPool bufferPool)
        {
            _sequence = sequence;
            _bufferPool = bufferPool;
        }

        public long Available => _sequence.Length;

        public IMemoryOwner<byte> GetRecyclableMemory(int limit)
        {
            if (!Varint.TryGetUInt64(_sequence, out ulong length, out var consumed)) throw new FormatException();

            _sequence = _sequence.Slice(consumed);

            if (length > (ulong)limit) throw new FormatException();

            var memoryOwner = _bufferPool.Rent((int)length);

            _sequence.Slice(0, (long)length).CopyTo(memoryOwner.Memory.Span);
            _sequence = _sequence.Slice((long)length);

            return memoryOwner;
        }

        public ReadOnlyMemory<byte> GetMemory(int limit)
        {
            if (!Varint.TryGetUInt64(_sequence, out ulong length, out var consumed)) throw new FormatException();

            _sequence = _sequence.Slice(consumed);

            if (length > (ulong)limit) throw new FormatException();

            var result = new byte[(int)length];

            _sequence.Slice(0, (long)length).CopyTo(result.AsSpan());
            _sequence = _sequence.Slice((long)length);

            return new ReadOnlyMemory<byte>(result);
        }

        public string GetString(int limit)
        {
            if (!Varint.TryGetUInt64(_sequence, out ulong length, out var consumed)) throw new FormatException();

            _sequence = _sequence.Slice(consumed);

            if (length > (ulong)limit) throw new FormatException();

            using (var memoryOwner = _bufferPool.Rent((int)length))
            {
                _sequence.Slice(0, (long)length).CopyTo(memoryOwner.Memory.Span);
                _sequence = _sequence.Slice((long)length);

                return _encoding.Value.GetString(memoryOwner.Memory.Span);
            }
        }

        public Timestamp GetTimestamp()
        {
            long seconds = this.GetInt64();
            int nanos = (int)this.GetUInt64();

            return new Timestamp(seconds, nanos);
        }

        public bool GetBoolean()
        {
            if (!Varint.TryGetUInt64(_sequence, out ulong result, out var consumed)) throw new FormatException();

            _sequence = _sequence.Slice(consumed);

            return (result != 0);
        }

        public ulong GetUInt64()
        {
            if (!Varint.TryGetUInt64(_sequence, out ulong result, out var consumed)) throw new FormatException();

            _sequence = _sequence.Slice(consumed);

            return result;
        }

        public long GetInt64()
        {
            if (!Varint.TryGetInt64(_sequence, out long result, out var consumed)) throw new FormatException();

            _sequence = _sequence.Slice(consumed);

            return result;
        }
    }
}
