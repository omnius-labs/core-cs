using System;

namespace Omnius.Core.RocketPack;

public readonly struct FieldType : IEquatable<FieldType>
{
    private FieldType(FieldTypeKind kind, byte major = 0, byte info = 0)
    {
        this.Kind = kind;
        this.Major = major;
        this.Info = info;
    }

    public FieldTypeKind Kind { get; }

    public byte Major { get; }

    public byte Info { get; }

    public static FieldType Bool { get; } = new(FieldTypeKind.Bool);
    public static FieldType U8 { get; } = new(FieldTypeKind.U8);
    public static FieldType U16 { get; } = new(FieldTypeKind.U16);
    public static FieldType U32 { get; } = new(FieldTypeKind.U32);
    public static FieldType U64 { get; } = new(FieldTypeKind.U64);
    public static FieldType I8 { get; } = new(FieldTypeKind.I8);
    public static FieldType I16 { get; } = new(FieldTypeKind.I16);
    public static FieldType I32 { get; } = new(FieldTypeKind.I32);
    public static FieldType I64 { get; } = new(FieldTypeKind.I64);
    public static FieldType F16 { get; } = new(FieldTypeKind.F16);
    public static FieldType F32 { get; } = new(FieldTypeKind.F32);
    public static FieldType F64 { get; } = new(FieldTypeKind.F64);
    public static FieldType Bytes { get; } = new(FieldTypeKind.Bytes);
    public static FieldType String { get; } = new(FieldTypeKind.String);
    public static FieldType Array { get; } = new(FieldTypeKind.Array);
    public static FieldType Map { get; } = new(FieldTypeKind.Map);

    public static FieldType Unknown(byte major, byte info)
    {
        return new FieldType(FieldTypeKind.Unknown, major, info);
    }

    public override string ToString()
    {
        return this.Kind switch
        {
            FieldTypeKind.Bool => "bool",
            FieldTypeKind.U8 => "u8",
            FieldTypeKind.U16 => "u16",
            FieldTypeKind.U32 => "u32",
            FieldTypeKind.U64 => "u64",
            FieldTypeKind.I8 => "i8",
            FieldTypeKind.I16 => "i16",
            FieldTypeKind.I32 => "i32",
            FieldTypeKind.I64 => "i64",
            FieldTypeKind.F16 => "f16",
            FieldTypeKind.F32 => "f32",
            FieldTypeKind.F64 => "f64",
            FieldTypeKind.Bytes => "bytes",
            FieldTypeKind.String => "string",
            FieldTypeKind.Array => "array",
            FieldTypeKind.Map => "map",
            _ => $"unknown(major={this.Major}, info={this.Info})",
        };
    }

    public bool Equals(FieldType other)
    {
        return this.Kind == other.Kind && this.Major == other.Major && this.Info == other.Info;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not FieldType) return false;

        return this.Equals((FieldType)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)this.Kind, this.Major, this.Info);
    }

    public static bool operator ==(FieldType right, FieldType left)
    {
        return right.Equals(left);
    }

    public static bool operator !=(FieldType right, FieldType left)
    {
        return !(right == left);
    }
}

public enum FieldTypeKind : byte
{
    Bool,
    U8,
    U16,
    U32,
    U64,
    I8,
    I16,
    I32,
    I64,
    F16,
    F32,
    F64,
    Bytes,
    String,
    Array,
    Map,
    Unknown,
}
