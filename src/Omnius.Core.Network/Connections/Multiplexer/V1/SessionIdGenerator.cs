using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Omnius.Core.Network.Connections.Multiplexer.V1
{
    public enum SessionIdGenerateType
    {
        EvenNumber,
        OddNumber,
    }

    internal sealed class SessionIdGenerator
    {
        private readonly SessionIdGenerateType _type;
        private readonly HashSet<ulong> _hashSet = new HashSet<ulong>();
        private readonly Random _random = new Random();

        private readonly object _lockObject = new object();

        public SessionIdGenerator(SessionIdGenerateType type)
        {
            _type = type;
        }

        public ulong Create()
        {
            lock (_lockObject)
            {
                Span<byte> buffer = stackalloc byte[8];

                for (; ; )
                {
                    _random.NextBytes(buffer);
                    var id = BinaryPrimitives.ReadUInt64BigEndian(buffer);

                    if (_type == SessionIdGenerateType.EvenNumber)
                    {
                        id &= ~(ulong)0x01;
                    }
                    else
                    {
                        id |= 0x01;
                    }

                    if (_hashSet.Add(id))
                    {
                        continue;
                    }

                    return id;
                }
            }
        }

        public void Release(ulong id)
        {
            lock (_lockObject)
            {
                _hashSet.Remove(id);
            }
        }
    }
}
