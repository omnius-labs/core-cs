using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public sealed class RocketPackDefinition
    {
        public RocketPackDefinition(IEnumerable<UsingDefinition> usings, IEnumerable<OptionDefinition> options, IEnumerable<EnumDefinition> enums, IEnumerable<MessageDefinition> messages)
        {
            this.Usings = usings?.ToList() ?? throw new ArgumentNullException(nameof(usings));
            this.Options = options?.ToList() ?? throw new ArgumentNullException(nameof(options));
            this.Enums = enums?.ToList() ?? throw new ArgumentNullException(nameof(enums));
            this.Messages = messages?.ToList() ?? throw new ArgumentNullException(nameof(messages));
        }

        public IList<UsingDefinition> Usings { get; }
        public IList<OptionDefinition> Options { get; }
        public IList<EnumDefinition> Enums { get; }
        public IList<MessageDefinition> Messages { get; }
    }

    public sealed class UsingDefinition
    {
        public UsingDefinition(string path)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public string Path { get; }
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
        public MessageDefinition(IEnumerable<string> attributes, MessageFormatType formatType, string name, IEnumerable<MessageElement> elements)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.FormatType = formatType;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Elements = elements?.ToList() ?? throw new ArgumentNullException(nameof(elements));
        }

        public IList<string> Attributes { get; }
        public MessageFormatType FormatType { get; }
        public string Name { get; }
        public IList<MessageElement> Elements { get; }
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
        public TypeBase(bool isOptional)
        {
            this.IsOptional = isOptional;
        }

        public bool IsOptional { get; }
    }

    public sealed class BoolType : TypeBase
    {
        public BoolType(bool isOptional) : base(isOptional)
        {
        }
    }

    public sealed class IntType : TypeBase
    {
        public IntType(bool isSigned, int size, bool isOptional) : base(isOptional)
        {
            this.IsSigned = isSigned;
            this.Size = size;
        }

        public bool IsSigned { get; }
        public int Size { get; }
    }

    public sealed class FloatType : TypeBase
    {
        public FloatType(int size, bool isOptional) : base(isOptional)
        {
            this.Size = size;
        }

        public int Size { get; }
    }

    public sealed class StringType : TypeBase
    {
        public StringType(int maxLength, bool isOptional) : base(isOptional)
        {
            this.MaxLength = maxLength;
        }

        public int MaxLength { get; }
    }

    public sealed class TimestampType : TypeBase
    {
        public TimestampType(bool isOptional) : base(isOptional)
        {
        }
    }

    public sealed class MemoryType : TypeBase
    {
        public MemoryType(int maxLength, bool isOptional) : base(isOptional)
        {
            this.MaxLength = maxLength;
        }

        public int MaxLength { get; }
        public bool IsUseMemoryPool { get; set; }
    }

    public sealed class ListType : TypeBase
    {
        public ListType(TypeBase elementType, int maxLength, bool isOptional) : base(isOptional)
        {
            this.ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
            this.MaxLength = maxLength;
        }

        public TypeBase ElementType { get; }
        public int MaxLength { get; }
    }

    public sealed class MapType : TypeBase
    {
        public MapType(TypeBase keyType, TypeBase valueType, int maxLength, bool isOptional) : base(isOptional)
        {
            this.KeyType = keyType ?? throw new ArgumentNullException(nameof(keyType));
            this.ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            this.MaxLength = maxLength;
        }

        public TypeBase KeyType { get; }
        public TypeBase ValueType { get; }
        public int MaxLength { get; }
    }

    public sealed class CustomType : TypeBase
    {
        public CustomType(string typeName, bool isOptional) : base(isOptional)
        {
            this.TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public string TypeName { get; }
    }
}
