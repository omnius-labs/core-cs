using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Omnix.Base;
using System.Collections;
using System.Runtime.CompilerServices;
using Omnix.Base.Extensions;

namespace Omnix.Collections
{
    public class VolatileBloomFilter<T> : ISynchronized
    {
        private readonly Dictionary<DateTime, BitArray> _map;
        private readonly int _capacity;
        private readonly double _errorRate;
        private readonly Func<T, long> _hashFunction;
        private readonly TimeSpan _unitTime;
        private readonly TimeSpan _survivalTime;

        private readonly int _hashFunctionCount;
        private readonly int _bitCount;

        private int[] _hashes;

        public VolatileBloomFilter(int capacity, double errorRate, int hashFunctionCount, Func<T, long> hashFunction, TimeSpan unitTime, TimeSpan survivalTime)
        {
            _map = new Dictionary<DateTime, BitArray>();
            _capacity = capacity;
            _errorRate = errorRate;
            _hashFunctionCount = hashFunctionCount;
            _hashFunction = hashFunction;
            _unitTime = unitTime;
            _survivalTime = survivalTime;

            _bitCount = ComputeM(_capacity, _errorRate, _hashFunctionCount);

            _hashes = new int[_hashFunctionCount];
        }

        private static int ComputeM(int n, double p, int k)
        {
            return (int)Math.Ceiling(n * ((-k) / Math.Log(1 - Math.Exp(Math.Log(p) / k))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeHashes(long seed, in int[] hashes, int range)
        {
            for (uint i = 0; i < hashes.Length; i++)
            {
                hashes[i] = (int)(((ulong)seed * (i + 1)) % (ulong)range);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Contains(BitArray bits, in int[] hashes)
        {
            for (int i = 0; i < hashes.Length; i++)
            {
                if (!bits.Get(hashes[i])) return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetFlags(BitArray bits, in int[] hashes)
        {
            for (int i = 0; i < hashes.Length; i++)
            {
                bits.Set(hashes[i], true);
            }
        }

        public object LockObject { get; } = new object();

        public TimeSpan SurvivalTime => _survivalTime;

        public TimeSpan GetElapsedTime(T item)
        {
            lock (this.LockObject)
            {
                var now = DateTime.UtcNow;
                ComputeHashes(_hashFunction(item), _hashes, _bitCount);

                foreach (var (updateTime, bits) in _map)
                {
                    if (Contains(bits, _hashes)) return (now - updateTime);
                }

                return _survivalTime;
            }
        }

        public void Update()
        {
            lock (this.LockObject)
            {
                var now = DateTime.UtcNow;

                foreach (var updateTime in _map.Keys.ToArray())
                {
                    if ((now - updateTime) > _survivalTime)
                    {
                        _map.Remove(updateTime);
                    }
                }
            }
        }

        public void Add(T item)
        {
            lock (this.LockObject)
            {
                var now = DateTime.UtcNow;
                now = now.AddTicks(-(now.Ticks % _unitTime.Ticks));

                var bits = _map.GetOrAdd(now, (_) => new BitArray(_bitCount));

                ComputeHashes(_hashFunction(item), _hashes, _bitCount);
                SetFlags(bits, _hashes);
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            lock (this.LockObject)
            {
                var now = DateTime.UtcNow;
                now = now.AddTicks(-(now.Ticks % _unitTime.Ticks));

                var bits = _map.GetOrAdd(now, (_) => new BitArray(_bitCount));

                foreach (var item in collection)
                {
                    ComputeHashes(_hashFunction(item), _hashes, _bitCount);
                    SetFlags(bits, _hashes);
                }
            }
        }

        public bool Contains(T item)
        {
            lock (this.LockObject)
            {
                ComputeHashes(_hashFunction(item), _hashes, _bitCount);

                foreach (var bits in _map.Values)
                {
                    if (Contains(bits, _hashes)) return true;
                }

                return false;
            }
        }
    }
}
