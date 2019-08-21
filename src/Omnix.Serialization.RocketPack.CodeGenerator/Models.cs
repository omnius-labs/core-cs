using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public sealed class RocketPackDefinition
    {
        public RocketPackDefinition(IEnumerable<UsingDefinition> usings, NamespaceDefinition @namespace, IEnumerable<OptionDefinition> options, IEnumerable<EnumDefinition> enums, IEnumerable<MessageDefinition> messages)
        {
            this.Usings = usings?.ToList() ?? throw new ArgumentNullException(nameof(usings));
            this.Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            this.Options = options?.ToList() ?? throw new ArgumentNullException(nameof(options));
            this.Enums = enums?.ToList() ?? throw new ArgumentNullException(nameof(enums));
            this.Messages = messages?.ToList() ?? throw new ArgumentNullException(nameof(messages));
        }

        public IList<UsingDefinition> Usings { get; }
        public NamespaceDefinition Namespace { get; }
        public IList<OptionDefinition> Options { get; }
        public IList<EnumDefinition> Enums { get; }
        public IList<MessageDefinition> Messages { get; }
    }

    public sealed class UsingDefinition
    {
        public UsingDefinition(string name)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }

    public sealed class NamespaceDefinition
    {
        public NamespaceDefinition(string name)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }

    public sealed class OptionDefinition
    {
        public OptionDefinition(string name, string value)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name { get; }
        public string Value { get; }
    }

    public sealed class EnumDefinition
    {
        public EnumDefinition(IEnumerable<string> attributes, string name, TypeBase type, IEnumerable<EnumElement> elements)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Elements = elements?.ToList() ?? throw new ArgumentNullException(nameof(elements));
        }

        public IList<string> Attributes { get; }
        public string Name { get; }
        public TypeBase Type { get; }
        public IList<EnumElement> Elements { get; }
    }

    public sealed class EnumElement
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

    public enum MessageFormatType
    {
        Small,
        Medium,
    }

    public sealed class MessageDefinition
    {
        public MessageDefinition(IEnumerable<string> attributes, string name, MessageFormatType formatType, IEnumerable<MessageElement> elements)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.FormatType = formatType;
            this.Elements = elements?.ToList() ?? throw new ArgumentNullException(nameof(elements));
        }

        public IList<string> Attributes { get; }
        public string Name { get; }
        public MessageFormatType FormatType { get; }
        public IList<MessageElement> Elements { get; }

        public bool IsClass => this.Attributes.Contains("csharp_class");
        public bool IsStruct => this.Attributes.Contains("csharp_struct");
    }

    public sealed class MessageElement
    {
        public MessageElement(IEnumerable<string> attributes, string name, TypeBase type, int? id)
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

    public abstract class TypeBase
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

    public sealed class BoolType : TypeBase
    {
        public BoolType(bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
        }
    }

    public sealed class IntType : TypeBase
    {
        public IntType(bool isSigned, int size, bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.IsSigned = isSigned;
            this.Size = size;
        }

        public bool IsSigned { get; }
        public int Size { get; }
    }

    public sealed class FloatType : TypeBase
    {
        public FloatType(int size, bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.Size = size;
        }

        public int Size { get; }
    }

    public sealed class StringType : TypeBase
    {
        public StringType(bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
        }

        public int MaxLength => int.Parse(this.Parameters["capacity"]);
    }

    public sealed class TimestampType : TypeBase
    {
        public TimestampType(bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
        }
    }

    public sealed class BytesType : TypeBase
    {
        public BytesType(bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
        }

        public int MaxLength => int.Parse(this.Parameters["capacity"]);
        public bool IsUseMemoryPool => bool.Parse(this.Parameters["recyclable"]);
    }

    public sealed class VectorType : TypeBase
    {
        public VectorType(TypeBase elementType,  bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
        }

        public TypeBase ElementType { get; }
        public int MaxLength => int.Parse(this.Parameters["capacity"]);
    }

    public sealed class MapType : TypeBase
    {
        public MapType(TypeBase keyType, TypeBase valueType, bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.KeyType = keyType ?? throw new ArgumentNullException(nameof(keyType));
            this.ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        }

        public TypeBase KeyType { get; }
        public TypeBase ValueType { get; }
        public int MaxLength => int.Parse(this.Parameters["capacity"]);
    }

    public sealed class CustomType : TypeBase
    {
        public CustomType(string typeName, bool isOptional, IDictionary<string, string> parameters) : base(isOptional, parameters)
        {
            this.TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public string TypeName { get; }
    }
}
