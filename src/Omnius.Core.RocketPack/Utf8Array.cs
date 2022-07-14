using System.Diagnostics.CodeAnalysis;
using System.Text;
using Omnius.Core.Helpers;

namespace Omnius.Core.RocketPack;

public class Utf8Array : IEquatable<Utf8Array>
{
    private static Encoding _utf8Encoding = new UTF8Encoding(false);

    private readonly byte[] _buffer;
    private readonly int _hashCode;

    public Utf8Array(string text) : this(_utf8Encoding.GetBytes(text))
    {
    }

    public Utf8Array(byte[] encodedBytes)
    {
        _buffer = encodedBytes;
        _hashCode = ObjectHelper.GetHashCode(_buffer);
    }

    public static Utf8Array Empty { get; } = new Utf8Array(Array.Empty<byte>());

    public static bool operator ==(Utf8Array? right, Utf8Array? left)
    {
        return (right is null) ? (left is null) : right.Equals(left);
    }

    public static bool operator !=(Utf8Array? right, Utf8Array? left)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Utf8Array) return false;

        return this.Equals((Utf8Array)obj);
    }

    public bool Equals(Utf8Array? other)
    {
        if (other is null) return false;
        return BytesOperations.Equals(_buffer, other.Span);
    }

    [return: NotNullIfNotNull("utf8Array")]
    public static implicit operator string?(Utf8Array? utf8Array)
    {
        return utf8Array?.ToString() ?? null;
    }

    [return: NotNullIfNotNull("value")]
    public static implicit operator Utf8Array?(string? value)
    {
        return value is null ? null : new Utf8Array(value);
    }

    public ReadOnlySpan<byte> Span => _buffer;

    public bool IsEmpty => _buffer.Length == 0;

    public int Length => _buffer.Length;

    public override string ToString() => _utf8Encoding.GetString(_buffer);
}
