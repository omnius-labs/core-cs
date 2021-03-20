using System;

namespace Omnius.Core.RocketPack
{
    // https://github.com/google/protobuf/blob/master/csharp/src/Google.Protobuf/WellKnownTypes/TimestampPartial.cs

    public readonly struct Timestamp : IEquatable<Timestamp>, IComparable<Timestamp>
    {
        public Timestamp(long seconds, int nanos)
        {
            this.Seconds = seconds;
            this.Nanos = nanos;
        }

        public static Timestamp Zero { get; } = new Timestamp(0, 0);

        public long Seconds { get; }

        public int Nanos { get; }

        public static bool operator ==(Timestamp x, Timestamp y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Timestamp x, Timestamp y)
        {
            return !x.Equals(y);
        }

        public static bool operator <(Timestamp x, Timestamp y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(Timestamp x, Timestamp y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator <=(Timestamp x, Timestamp y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >=(Timestamp x, Timestamp y)
        {
            return x.CompareTo(y) >= 0;
        }

        public override int GetHashCode()
        {
            return (int)(this.Seconds ^ this.Nanos);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Timestamp) return false;

            return this.Equals((Timestamp)obj);
        }

        public bool Equals(Timestamp other)
        {
            return (this.Seconds == other.Seconds && this.Nanos == other.Nanos);
        }

        public int CompareTo(Timestamp other)
        {
            int result;

            result = this.Seconds.CompareTo(other.Seconds);
            if (result != 0) return result;

            result = this.Nanos.CompareTo(other.Nanos);
            if (result != 0) return result;

            return 0;
        }

        public DateTime ToDateTime()
        {
            return DateTime.UnixEpoch.AddSeconds(this.Seconds).AddTicks(this.Nanos / 100);
        }

        public static Timestamp FromDateTime(DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc) throw new ArgumentException("Conversion from DateTime to Timestamp requires the DateTime kind to be Utc", "dateTime");

            long ticks = dateTime.Ticks - DateTime.UnixEpoch.Ticks;
            long seconds = ticks / TimeSpan.TicksPerSecond;
            int nanos = (int)(ticks % TimeSpan.TicksPerSecond) * 100;
            return new Timestamp(seconds, nanos);
        }
    }
}
