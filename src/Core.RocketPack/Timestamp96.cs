namespace Omnius.Core.RocketPack;

// https://gitbytesPipe.com/google/protobuf/blob/master/csharp/src/Google.Protobuf/WellKnownTypes/TimestampPartial.cs

public readonly struct Timestamp96 : IRocketPackStruct<Timestamp96>, IEquatable<Timestamp96>, IComparable<Timestamp96>
{
    public Timestamp96(long seconds, uint nanos)
    {
        this.Seconds = seconds;
        this.Nanos = nanos;
    }

    public static void Pack(ref RocketPackBytesEncoder encoder, in Timestamp96 value)
    {
        encoder.WriteMap(2);
        encoder.WriteU64(0);
        encoder.WriteI64(value.Seconds);
        encoder.WriteU64(1);
        encoder.WriteU32(value.Nanos);
    }

    public static Timestamp96 Unpack(ref RocketPackBytesDecoder decoder)
    {
        var count = decoder.ReadMap();
        long seconds = 0;
        uint nanos = 0;
        for (ulong i = 0; i < count; i++)
        {
            switch (decoder.ReadU64())
            {
                case 0:
                    seconds = decoder.ReadI64();
                    break;
                case 1:
                    nanos = decoder.ReadU32();
                    break;
                default:
                    decoder.SkipField();
                    break;
            }
        }

        return new Timestamp96(seconds, nanos);
    }

    public static Timestamp96 Zero { get; } = new Timestamp96(0, 0);

    public long Seconds { get; }

    public uint Nanos { get; }

    public static bool operator ==(Timestamp96 x, Timestamp96 y)
    {
        return x.Equals(y);
    }

    public static bool operator !=(Timestamp96 x, Timestamp96 y)
    {
        return !x.Equals(y);
    }

    public static bool operator <(Timestamp96 x, Timestamp96 y)
    {
        return x.CompareTo(y) < 0;
    }

    public static bool operator >(Timestamp96 x, Timestamp96 y)
    {
        return x.CompareTo(y) > 0;
    }

    public static bool operator <=(Timestamp96 x, Timestamp96 y)
    {
        return x.CompareTo(y) <= 0;
    }

    public static bool operator >=(Timestamp96 x, Timestamp96 y)
    {
        return x.CompareTo(y) >= 0;
    }

    public override int GetHashCode()
    {
        return (int)(this.Seconds ^ this.Nanos);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Timestamp96) return false;

        return this.Equals((Timestamp96)obj);
    }

    public bool Equals(Timestamp96 other)
    {
        return (this.Seconds == other.Seconds && this.Nanos == other.Nanos);
    }

    public int CompareTo(Timestamp96 other)
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

    public static Timestamp96 FromDateTime(DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Utc) throw new ArgumentException("Conversion from DateTime to Timestamp requires the DateTime kind to be Utc", "dateTime");

        long ticks = dateTime.Ticks - DateTime.UnixEpoch.Ticks;
        long seconds = ticks / TimeSpan.TicksPerSecond;
        uint nanos = (uint)(ticks % TimeSpan.TicksPerSecond) * 100;
        return new Timestamp96(seconds, nanos);
    }

    public static Timestamp96 FromSecondsAndNanos(long seconds, uint nanos)
    {
        return new Timestamp96(seconds, nanos);
    }
}
