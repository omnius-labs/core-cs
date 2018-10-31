using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Omnix.Collections
{
    partial class GolombCodedSet
    {
        class GolombDecoder
        {
            private BitReader _bitReader;
            private int _p, _log2p;

            public GolombDecoder(ReadOnlySequence<byte> _sequence, int p)
            {
                _bitReader = new BitReader(_sequence);
                _p = p;
                _log2p = FloorLog2(p);
            }

            public bool TryDecode(out uint result)
            {
                result = 0;

                for (; ; )
                {
                    if (!_bitReader.TryRead(1, out var value)) return false;
                    if (value == 0) break;

                    result += (uint)_p;
                }

                {
                    if (!_bitReader.TryRead(_log2p, out var value)) return false;
                    result += value;
                }

                return true;
            }

            unsafe class BitReader
            {
                private ReadOnlySequence<byte> _sequence;
                private SequencePosition _sequencePosition = new SequencePosition();

                private ReadOnlyMemory<byte> _segment;
                private int _segmentOffset;

                private uint _bitBuffer;
                private int _bitBufferCount;

                public BitReader(ReadOnlySequence<byte> sequence)
                {
                    _sequence = sequence;
                    _sequencePosition = _sequence.GetPosition(0);
                }

                public bool TryRead(int nbits, out uint result)
                {
                    result = 0;

                    while (nbits != 0)
                    {
                        if (_bitBufferCount == 0)
                        {
                            int length;

                            for (; ; )
                            {
                                length = (_segment.Length - _segmentOffset);
                                if (length != 0) break;

                                if (!_sequence.TryGet(ref _sequencePosition, out _segment)) return false;
                                _segmentOffset = 0;
                            }

                            fixed (byte* p_segment = _segment.Span.Slice(_segmentOffset))
                            {
                                if (length >= 4)
                                {
                                    _bitBuffer =
                                        ((uint)p_segment[0] << 24) |
                                        ((uint)p_segment[1] << 16) |
                                        ((uint)p_segment[2] << 8) |
                                        ((uint)p_segment[3]);
                                    _segmentOffset += 4;
                                    _bitBufferCount += 32;
                                }
                                else
                                {
                                    for (int i = 0; i < length; i++)
                                    {
                                        _bitBuffer |= ((uint)p_segment[i] << (((length - 1) - i) * 8));
                                        _segmentOffset++;
                                        _bitBufferCount += 8;
                                    }
                                }
                            }
                        }

                        int readBitCount = Math.Min(_bitBufferCount, nbits);
                        result <<= readBitCount;
                        result |= (_bitBuffer >> (_bitBufferCount - readBitCount));
                        _bitBufferCount -= readBitCount;
                        nbits -= readBitCount;
                        _bitBuffer &= Bitmask(_bitBufferCount);
                    }

                    return true;
                }
            }
        }
    }
}
