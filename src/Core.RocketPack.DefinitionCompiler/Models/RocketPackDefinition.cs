using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Models;

sealed class RocketPackDefinition
{
    public required IReadOnlyList<UsingDefinition> Usings { get; init; }
    public required NamespaceDefinition Namespace { get; init; }
    public required IReadOnlyList<OptionDefinition> Options { get; init; }
    public required IReadOnlyList<EnumDefinition> Enums { get; init; }
    public required IReadOnlyList<ObjectDefinition> Objects { get; init; }

    public string CSharpNamespace
    {
        get
        {
            if (this.Options.FirstOrDefault(n => n.Name == "csharp_namespace")?.Value is string value)
            {
                return value;
            }

            return this.Namespace.Value;
        }
    }
}

sealed class UsingDefinition
{
    public required string Value { get; init; }
}

sealed class NamespaceDefinition
{
    public required string Value { get; init; }
}

sealed class OptionDefinition
{
    public required string Name { get; init; }
    public required object Value { get; init; }
}

internal sealed class EnumDefinition
{
    public required IReadOnlyList<string> Attributes { get; init; }
    public required string Name { get; init; }
    public required TypeBase Type { get; init; }
    public required IReadOnlyList<EnumElement> Elements { get; init; }

    public string Namespace { get; set; } = string.Empty;
    public string FullName => this.Namespace + "." + this.Name;
    public string CSharpFullName => "global::" + this.FullName;
}

internal sealed class EnumElement
{
    public required IReadOnlyList<string> Attributes { get; init; }
    public required string Name { get; init; }
    public required int Id { get; init; }
}

internal enum ObjectFormatType
{
    Struct,
    Message,
}

internal sealed class ObjectDefinition
{
    public required IReadOnlyList<string> Attributes { get; init; }
    public required string Name { get; init; }
    public required ObjectFormatType FormatType { get; init; }
    public required IReadOnlyList<ObjectElement> Elements { get; init; }

    public string Namespace { get; set; } = string.Empty;
    public string FullName => this.Namespace + "." + this.Name;
    public string CSharpFullName => "global::" + this.FullName;
}

internal sealed class ObjectElement
{
    public required IReadOnlyList<string> Attributes { get; init; }
    public required string Name { get; init; }
    public required TypeBase Type { get; init; }
}

internal abstract class TypeBase
{
    public required bool IsOptional { get; init; }
    public IReadOnlyDictionary<string, object> Parameters { get; init; } = ImmutableDictionary<string, object>.Empty;
}

internal sealed class BoolType : TypeBase
{
}

internal sealed class IntType : TypeBase
{
    public required bool IsSigned { get; init; }
    public required int Size { get; init; }
}

internal sealed class FloatType : TypeBase
{
    public required int Size { get; init; }
}

internal sealed class StringType : TypeBase
{
    public int MaxLength
    {
        get
        {
            if (this.Parameters.GetValueOrDefault("capacity") is long result)
            {
                return (int)result;
            }

            return int.MaxValue;
        }
    }
}

internal sealed class TimestampType : TypeBase
{
    public required int Size { get; init; }
}

internal sealed class BytesType : TypeBase
{
    public int MaxLength
    {
        get
        {
            if (this.Parameters.GetValueOrDefault("capacity") is long result)
            {
                return (int)result;
            }

            return int.MaxValue;
        }
    }

    public bool IsUseMemoryPool
    {
        get
        {
            if (this.Parameters.GetValueOrDefault("recyclable") is bool result)
            {
                return result;
            }

            return false;
        }
    }
}

internal sealed class VectorType : TypeBase
{
    public required TypeBase ElementType { get; init; }

    public int MaxLength
    {
        get
        {
            if (this.Parameters.GetValueOrDefault("capacity") is long result)
            {
                return (int)result;
            }

            return int.MaxValue;
        }
    }
}

internal sealed class MapType : TypeBase
{
    public required TypeBase KeyType { get; init; }
    public required TypeBase ValueType { get; init; }

    public int MaxLength
    {
        get
        {
            if (this.Parameters.GetValueOrDefault("capacity") is long result)
            {
                return (int)result;
            }

            return int.MaxValue;
        }
    }
}

internal sealed class CustomType : TypeBase
{
    public required string Type { get; init; }
}
