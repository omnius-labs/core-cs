namespace Core.RocketPack;

// https://gitbytesPipe.com/google/protobuf/blob/master/csharp/src/Google.Protobuf/WellKnownTypes/TimestampPartial.cs

public readonly struct Timestamp64 : IEquatable<Timestamp64>, IComparable<Timestamp64>
{
    public Timestamp64(long seconds)
    {
        this.Seconds = seconds;
    }

    public static Timestamp64 Zero { get; } = new Timestamp64(0);

    public long Seconds { get; }

    public static bool operator ==(Timestamp64 x, Timestamp64 y)
    {
        return x.Equals(y);
    }

    public static bool operator !=(Timestamp64 x, Timestamp64 y)
    {
        return !x.Equals(y);
    }

    public static bool operator <(Timestamp64 x, Timestamp64 y)
    {
        return x.CompareTo(y) < 0;
    }

    public static bool operator >(Timestamp64 x, Timestamp64 y)
    {
        return x.CompareTo(y) > 0;
    }

    public static bool operator <=(Timestamp64 x, Timestamp64 y)
    {
        return x.CompareTo(y) <= 0;
    }

    public static bool operator >=(Timestamp64 x, Timestamp64 y)
    {
        return x.CompareTo(y) >= 0;
    }

    public override int GetHashCode()
    {
        return (int)(this.Seconds);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Timestamp64) return false;

        return this.Equals((Timestamp64)obj);
    }

    public bool Equals(Timestamp64 other)
    {
        return (this.Seconds == other.Seconds);
    }

    public int CompareTo(Timestamp64 other)
    {
        int result;

        result = this.Seconds.CompareTo(other.Seconds);
        if (result != 0) return result;

        return 0;
    }

    public DateTime ToDateTime()
    {
        return DateTime.UnixEpoch.AddSeconds(this.Seconds);
    }

    public static Timestamp64 FromDateTime(DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Utc) throw new ArgumentException("Conversion from DateTime to Timestamp requires the DateTime kind to be Utc", "dateTime");

        long ticks = dateTime.Ticks - DateTime.UnixEpoch.Ticks;
        long seconds = ticks / TimeSpan.TicksPerSecond;
        return new Timestamp64(seconds);
    }

    public static Timestamp64 FromSeconds(long seconds)
    {
        return new Timestamp64(seconds);
    }
}
