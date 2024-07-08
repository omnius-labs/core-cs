using System.Diagnostics.CodeAnalysis;
using System.Text;
using Omnius.Core.Base;
using Omnius.Core.Base.Helpers;

namespace Omnius.Core.RocketPack;

public class Utf8String : IEquatable<Utf8String>
{
    private static Encoding _utf8Encoding = new UTF8Encoding(false);

    private readonly byte[] _buffer;
    private readonly int _hashCode;

    public static Utf8String Empty { get; } = new Utf8String(Array.Empty<byte>());

    public Utf8String(string text) : this(_utf8Encoding.GetBytes(text))
    {
    }

    public Utf8String(byte[] encodedBytes)
    {
        _buffer = encodedBytes;
        _hashCode = ObjectHelper.GetHashCode(_buffer);
    }

    public static bool operator ==(Utf8String? right, Utf8String? left)
    {
        return (right is null) ? (left is null) : right.Equals(left);
    }

    public static bool operator !=(Utf8String? right, Utf8String? left)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Utf8String) return false;

        return this.Equals((Utf8String)obj);
    }

    public bool Equals(Utf8String? other)
    {
        if (other is null) return false;
        return BytesOperations.Equals(_buffer, other.Span);
    }

    [return: NotNullIfNotNull("utf8String")]
    public static implicit operator string?(Utf8String? utf8String)
    {
        return utf8String?.ToString() ?? null;
    }

    [return: NotNullIfNotNull("value")]
    public static implicit operator Utf8String?(string? value)
    {
        return value is null ? null : new Utf8String(value);
    }

    public ReadOnlySpan<byte> Span => _buffer;

    public bool IsEmpty => _buffer.Length == 0;

    public int Length => _buffer.Length;

    public override string ToString() => _utf8Encoding.GetString(_buffer);
}
