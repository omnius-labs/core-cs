using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Models
{
    internal sealed class RocketPackDefinition
    {
        public RocketPackDefinition(IEnumerable<UsingDefinition> usings, NamespaceDefinition @namespace, IEnumerable<OptionDefinition> options, IEnumerable<EnumDefinition> enums, IEnumerable<ObjectDefinition> objects, IEnumerable<ServiceDefinition> services)
        {
            this.Usings = usings?.ToList() ?? throw new ArgumentNullException(nameof(usings));
            this.Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            this.Options = options?.ToList() ?? throw new ArgumentNullException(nameof(options));
            this.Enums = enums?.ToList() ?? throw new ArgumentNullException(nameof(enums));
            this.Objects = objects?.ToList() ?? throw new ArgumentNullException(nameof(objects));
            this.Services = services?.ToList() ?? throw new ArgumentNullException(nameof(services));
        }

        public IList<UsingDefinition> Usings { get; }

        public NamespaceDefinition Namespace { get; }

        public IList<OptionDefinition> Options { get; }

        public IList<EnumDefinition> Enums { get; }

        public IList<ObjectDefinition> Objects { get; }

        public IList<ServiceDefinition> Services { get; }

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

    internal sealed class UsingDefinition
    {
        public UsingDefinition(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; }
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
        public OptionDefinition(string name, object value)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name { get; }

        public object Value { get; }
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

        public string Namespace { get; set; } = string.Empty;

        public IList<string> Attributes { get; }

        public string Name { get; }

        public string FullName => this.Namespace + "." + this.Name;

        public TypeBase Type { get; }

        public IList<EnumElement> Elements { get; }

        public string CSharpFullName => "global::" + this.FullName;
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
        Message,
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

        public string Namespace { get; set; } = string.Empty;

        public IList<string> Attributes { get; }

        public string Name { get; }

        public string FullName => this.Namespace + "." + this.Name;

        public MessageFormatType FormatType { get; }

        public IList<ObjectElement> Elements { get; }

        public string CSharpFullName => "global::" + this.FullName;

        public bool IsCSharpStruct => this.Attributes.Contains("csharp_struct");

        public bool IsCSharpClass => !this.IsCSharpStruct;
    }

    internal sealed class ObjectElement
    {
        public ObjectElement(IEnumerable<string> attributes, string name, TypeBase type)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IList<string> Attributes { get; }

        public string Name { get; }

        public TypeBase Type { get; }
    }

    internal abstract class TypeBase
    {
        public TypeBase(bool isOptional, IDictionary<string, object>? parameters = null)
        {
            this.IsOptional = isOptional;

            if (parameters != null)
            {
                this.Parameters = new ReadOnlyDictionary<string, object>(parameters);
            }
            else
            {
                this.Parameters = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
            }
        }

        public bool IsOptional { get; }

        public IReadOnlyDictionary<string, object> Parameters { get; }
    }

    internal sealed class BoolType : TypeBase
    {
        public BoolType(bool isOptional)
            : base(isOptional)
        {
        }
    }

    internal sealed class IntType : TypeBase
    {
        public IntType(bool isSigned, int size, bool isOptional)
            : base(isOptional)
        {
            this.IsSigned = isSigned;
            this.Size = size;
        }

        public bool IsSigned { get; }

        public int Size { get; }
    }

    internal sealed class FloatType : TypeBase
    {
        public FloatType(int size, bool isOptional)
            : base(isOptional)
        {
            this.Size = size;
        }

        public int Size { get; }
    }

    internal sealed class StringType : TypeBase
    {
        public StringType(bool isOptional, IDictionary<string, object> parameters)
            : base(isOptional, parameters)
        {
        }

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
        public TimestampType(bool isOptional)
            : base(isOptional)
        {
        }
    }

    internal sealed class BytesType : TypeBase
    {
        public BytesType(bool isOptional, IDictionary<string, object> parameters)
            : base(isOptional, parameters)
        {
        }

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
        public VectorType(TypeBase elementType, bool isOptional, IDictionary<string, object> parameters)
            : base(isOptional, parameters)
        {
            this.ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
        }

        public TypeBase ElementType { get; }

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
        public MapType(TypeBase keyType, TypeBase valueType, bool isOptional, IDictionary<string, object> parameters)
            : base(isOptional, parameters)
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
        public CustomType(string typeName, bool isOptional)
            : base(isOptional)
        {
            this.TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public string TypeName { get; }
    }

    internal sealed class ServiceDefinition
    {
        public ServiceDefinition(IEnumerable<string> attributes, string name, IEnumerable<FuncElement> functions)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Functions = functions?.ToList() ?? throw new ArgumentNullException(nameof(functions));
        }

        public string Namespace { get; set; } = string.Empty;

        public IList<string> Attributes { get; }

        public string Name { get; }

        public IList<FuncElement> Functions { get; }

        public string CSharpInterfaceName => "I" + this.Name;

        public string CSharpInterfaceFullName => "global::" + this.Namespace + "." + this.CSharpInterfaceName;
    }

    internal sealed class FuncElement
    {
        public FuncElement(IEnumerable<string> attributes, string name, CustomType? inType, CustomType? outType)
        {
            this.Attributes = attributes?.ToList() ?? throw new ArgumentNullException(nameof(attributes));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.InType = inType;
            this.OutType = outType;
        }

        public IList<string> Attributes { get; }

        public string Name { get; }

        public CustomType? InType { get; }

        public CustomType? OutType { get; }

        public string CSharpFunctionName => this.Name + "Async";
    }
}
