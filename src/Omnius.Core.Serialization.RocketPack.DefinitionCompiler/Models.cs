using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Omnius.Core.Serialization.RocketPack.DefinitionCompiler
{
    internal sealed class RocketPackDefinition
    {
        public RocketPackDefinition(IEnumerable<UsingDefinition> usings, NamespaceDefinition @namespace, IEnumerable<OptionDefinition> options, IEnumerable<EnumDefinition> enums, IEnumerable<ObjectDefinition> objects)
        {
            this.Usings = usings?.ToList() ?? throw new ArgumentNullException(nameof(usings));
            this.Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            this.Options = options?.ToList() ?? throw new ArgumentNullException(nameof(options));
            this.Enums = enums?.ToList() ?? throw new ArgumentNullException(nameof(enums));
            this.Objects = objects?.ToList() ?? throw new ArgumentNullException(nameof(objects));
        }

        public IList<UsingDefinition> Usings { get; }
        public NamespaceDefinition Namespace { get; }
        public IList<OptionDefinition> Options { get; }
        public IList<EnumDefinition> Enums { get; }
        public IList<ObjectDefinition> Objects { get; }

        public string GetCSharpNamespace()
        {
            var value = this.Options.FirstOrDefault(n => n.Name == "csharp_namespace")?.Value;
            if (value != null) return value;

            return this.Namespace.Value;
        }
    }

    internal sealed class UsingDefinition
    {
        public UsingDefinition(string targetNamespace)
        {
            this.TargetNamespace = targetNamespace ?? throw new ArgumentNullException(nameof(targetNamespace));
        }

        public string TargetNamespace { get; }
    }

    internal sealed class NamespaceDefinition
    {
        public NamespaceDefinition(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; }
    }

    internal sealed class OptionDefinition
    {
        public OptionDefinition(string name, string value)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name { get; }
        public string Value { get; }
    }

    internal sealed class EnumDefinition
    {
        public EnumDefinition(IEnumerable<string> attributes, string name, TypeBase type, IEnumerable<EnumElement> elements)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Elements = elements?.ToList() ?? throw new ArgumentNullException(nameof(elements));
        }

        public string Namespace { get; set; }
        public IList<string> Attributes { get; }
        public string Name { get; }
        public TypeBase Type { get; }
        public IList<EnumElement> Elements { get; }
    }

    internal sealed class EnumElement
    {
        public EnumElement(IEnumerable<string> attributes, string name, int id)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Id = id;
        }

        public IList<string> Attributes { get; }
        public string Name { get; }
        public int Id { get; }
    }

    internal enum MessageFormatType
    {
        Struct,
        Table,
    }

    internal sealed class ObjectDefinition
    {
        public ObjectDefinition(IEnumerable<string> attributes, string name, MessageFormatType formatType, IEnumerable<ObjectElement> elements)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.FormatType = formatType;
            this.Elements = elements?.ToList() ?? throw new ArgumentNullException(nameof(elements));
        }

        public string Namespace { get; set; }
        public IList<string> Attributes { get; }
        public string Name { get; }
        public MessageFormatType FormatType { get; }
        public IList<ObjectElement> Elements { get; }

        public string FullName => "global::" + this.Namespace + "." + this.Name;

        public bool IsClass => !this.IsStruct;
        public bool IsStruct => this.Attributes.Contains("csharp_struct");
    }

    internal sealed class ObjectElement
    {
        public ObjectElement(IEnumerable<string> attributes, string name, TypeBase type, int? id)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Id = id;
        }

        public IList<string> Attributes { get; }
        public string Name { get; }
        public TypeBase Type { get; }
        public int? Id { get; }
    }

    internal abstract class TypeBase
    {
        public TypeBase(bool isOptional, IDictionary<string, string> parameters)
        {
            this.IsOptional = isOptional;
            if (parameters is null) throw new ArgumentNullException(nameof(parameters));
            this.Parameters = new ReadOnlyDictionary<string, string>(parameters);
        }

        public bool IsOptional { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }
    }

    internal sealed class BoolType : TypeBase
    {
        public BoolType(bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
        }
    }

    internal sealed class IntType : TypeBase
    {
        public IntType(bool isSigned, int size, bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.IsSigned = isSigned;
            this.Size = size;
        }

        public bool IsSigned { get; }
        public int Size { get; }
    }

    internal sealed class FloatType : TypeBase
    {
        public FloatType(int size, bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.Size = size;
        }

        public int Size { get; }
    }

    internal sealed class StringType : TypeBase
    {
        public StringType(bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
        }

        public int MaxLength
        {
            get
            {
                var text = this.Parameters.GetValueOrDefault("capacity");
                if (text is null) return int.MaxValue;
                return int.Parse(text);
            }
        }
    }

    internal sealed class TimestampType : TypeBase
    {
        public TimestampType(bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
        }
    }

    internal sealed class BytesType : TypeBase
    {
        public BytesType(bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
        }

        public int MaxLength
        {
            get
            {
                var text = this.Parameters.GetValueOrDefault("capacity");
                if (text is null) return int.MaxValue;
                return int.Parse(text);
            }
        }

        public bool IsUseMemoryPool
        {
            get
            {
                var text = this.Parameters.GetValueOrDefault("recyclable");
                if (text is null) return false;
                return bool.Parse(text);
            }
        }
    }

    internal sealed class VectorType : TypeBase
    {
        public VectorType(TypeBase elementType, bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
        }

        public TypeBase ElementType { get; }

        public int MaxLength
        {
            get
            {
                var text = this.Parameters.GetValueOrDefault("capacity");
                if (text is null) return int.MaxValue;
                return int.Parse(text);
            }
        }
    }

    internal sealed class MapType : TypeBase
    {
        public MapType(TypeBase keyType, TypeBase valueType, bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.KeyType = keyType ?? throw new ArgumentNullException(nameof(keyType));
            this.ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        }

        public TypeBase KeyType { get; }
        public TypeBase ValueType { get; }

        public int MaxLength
        {
            get
            {
                var text = this.Parameters.GetValueOrDefault("capacity");
                if (text is null) return int.MaxValue;
                return int.Parse(text);
            }
        }
    }

    internal sealed class CustomType : TypeBase
    {
        public CustomType(string typeName, bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public string TypeName { get; }
    }
}
