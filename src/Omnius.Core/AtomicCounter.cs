using System;
using System.Threading;

namespace Omnius.Core
{
    /// <summary>
    /// <see cref="Interlocked"/>を利用したStatefulなカウンターです。
    /// </summary>
    public sealed class AtomicCounter : IEquatable<AtomicCounter>
    {
        private long _value;

        public AtomicCounter()
        {
            _value = 0;
        }

        public AtomicCounter(long value)
        {
            _value = value;
        }

        private long Value
        {
            get
            {
                return Interlocked.Read(ref _value);
            }
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is AtomicCounter)) return false;

            return this.Equals((AtomicCounter)obj);
        }

        public bool Equals(AtomicCounter? other)
        {
            if (other is null) return false;
            if (object.ReferenceEquals(this, other)) return true;

            return this.Value == other.Value;
        }

        // ==
        public static bool operator ==(in AtomicCounter x, in AtomicCounter y)
        {
            return x.Value == y.Value;
        }

        // !=
        public static bool operator !=(in AtomicCounter x, in AtomicCounter y)
        {
            return x.Value != y.Value;
        }

        // <
        public static bool operator <(in AtomicCounter x, in AtomicCounter y)
        {
            return x.Value < y.Value;
        }

        // >
        public static bool operator >(in AtomicCounter x, in AtomicCounter y)
        {
            return x.Value > y.Value;
        }

        // <=
        public static bool operator <=(in AtomicCounter x, in AtomicCounter y)
        {
            return x.Value <= y.Value;
        }

        // >=
        public static bool operator >=(in AtomicCounter x, in AtomicCounter y)
        {
            return x.Value >= y.Value;
        }

        // implicit
        public static implicit operator long(in AtomicCounter safeInteger)
        {
            return safeInteger.Value;
        }

        public long Increment()
        {
            return Interlocked.Increment(ref _value);
        }

        public long Decrement()
        {
            return Interlocked.Decrement(ref _value);
        }

        public long Add(in long value)
        {
            return Interlocked.Add(ref _value, value);
        }

        public long Subtract(in long value)
        {
            return Interlocked.Add(ref _value, -value);
        }

        public long Exchange(in long value)
        {
            return Interlocked.Exchange(ref _value, value);
        }
    }
}
