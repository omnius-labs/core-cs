using System.Buffers;
using Omnius.Core.Collections;

namespace Omnius.Core.RocketPack;

public struct RocketArray<T> : IRocketMessage<RocketArray<T>>
{
    public static IRocketMessageFormatter<RocketArray<T>> Formatter => IRocketMessage<RocketArray<T>>.Formatter;
    public static RocketArray<T> Empty => IRocketMessage<RocketArray<T>>.Empty;

    static RocketArray()
    {
        IRocketMessage<RocketArray<T>>.Formatter = new CustomFormatter();
        IRocketMessage<RocketArray<T>>.Empty = new RocketArray<T>(Array.Empty<T>());
    }

    private readonly Lazy<int> _hashCode;

    public RocketArray(T[] values)
    {
        if (values is null) throw new ArgumentNullException("values");
        foreach (var n in values)
        {
            if (n is null) throw new ArgumentNullException("n");
        }

        this.Values = new ReadOnlyListSlim<T>(values);

        _hashCode = new Lazy<int>(() =>
        {
            var h = new HashCode();
            foreach (var n in values)
            {
                h.Add(n!.GetHashCode());
            }

            return h.ToHashCode();
        });
    }

    public ReadOnlyListSlim<T> Values { get; }

    public static RocketArray<T> Import(ReadOnlySequence<byte> sequence, IBytesPool bytesPool)
    {
        var reader = new RocketMessageReader(sequence, bytesPool);
        return Formatter.Deserialize(ref reader, 0);
    }

    public void Export(IBufferWriter<byte> bufferWriter, IBytesPool bytesPool)
    {
        var writer = new RocketMessageWriter(bufferWriter, bytesPool);
        Formatter.Serialize(ref writer, this, 0);
    }

    public static bool operator ==(RocketArray<T>? left, RocketArray<T>? right)
    {
        return (right is null) ? (left is null) : right.Equals(left);
    }

    public static bool operator !=(RocketArray<T>? left, RocketArray<T>? right)
    {
        return !(left == right);
    }

    public override bool Equals(object? other)
    {
        if (other is not RocketArray<T>) return false;
        return this.Equals((RocketArray<T>)other);
    }

    public bool Equals(RocketArray<T> target)
    {
        if (object.ReferenceEquals(this, target)) return true;
        if (!Helpers.CollectionHelper.Equals(this.Values, target.Values)) return false;

        return true;
    }

    public override int GetHashCode() => _hashCode.Value;

    private sealed class CustomFormatter : IRocketMessageFormatter<RocketArray<T>>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in RocketArray<T> value, scoped in int rank)
        {
            w.Write((uint)value.Values.Count);
            foreach (var n in value.Values)
            {
                IRocketMessage<T>.Formatter.Serialize(ref w, n, rank + 1);
            }
        }

        public RocketArray<T> Deserialize(ref RocketMessageReader r, scoped in int rank)
        {
            var length = r.GetUInt32();
            var values = new T[length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = IRocketMessage<T>.Formatter.Deserialize(ref r, rank + 1);
            }

            return new RocketArray<T>(values);
        }
    }
}
