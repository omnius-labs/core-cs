using System;
using System.Collections.Generic;
using System.Text;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public sealed class RocketFormatInfo
    {
        public IList<UsingInfo> Usings { get; set; }
        public IList<OptionInfo> Options { get; set; }
        public IList<EnumInfo> Enums { get; set; }
        public IList<MessageInfo> Messages { get; set; }
    }

    public sealed class UsingInfo
    {
        public string Path { get; set; }
    }
    public sealed class OptionInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public sealed class EnumInfo
    {
        public IList<string> Attributes { get; set; }
        public string Name { get; set; }
        public TypeInfo Type { get; set; }
        public IList<EnumElementInfo> Elements { get; set; }
    }

    public sealed class EnumElementInfo
    {
        public IList<string> Attributes { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public enum MessageFormatType
    {
        Small,
        Medium,
    }

    public sealed class MessageInfo
    {
        public IList<string> Attributes { get; set; }
        public MessageFormatType FormatType { get; set; }
        public string Name { get; set; }
        public IList<MessageElementInfo> Elements { get; set; }
    }

    public class MessageElementInfo
    {
        public IList<string> Attributes { get; set; }
        public string Name { get; set; }
        public TypeInfo Type { get; set; }
        public int? Id { get; set; }
    }

    public abstract class TypeInfo
    {
        public bool IsNullable { get; set; }
    }

    public sealed class BoolTypeInfo : TypeInfo
    {
    }

    public sealed class IntTypeInfo : TypeInfo
    {
        public bool IsSigned { get; set; }
        public int Size { get; set; }
    }

    public sealed class FloatTypeInfo : TypeInfo
    {
        public int Size { get; set; }
    }

    public sealed class StringTypeInfo : TypeInfo
    {
        public int MaxLength { get; set; }
    }

    public sealed class TimestampTypeInfo : TypeInfo
    {
    }

    public sealed class MemoryTypeInfo : TypeInfo
    {
        public int MaxLength { get; set; }
        public bool IsUseMemoryPool { get; set; }
    }

    public sealed class ListTypeInfo : TypeInfo
    {
        public int MaxLength { get; set; }
        public TypeInfo ElementType { get; set; }
    }

    public sealed class MapTypeInfo : TypeInfo
    {
        public int MaxLength { get; set; }
        public TypeInfo KeyType { get; set; }
        public TypeInfo ValueType { get; set; }
    }

    public sealed class CustomTypeInfo : TypeInfo
    {
        public string TypeName { get; set; }
    }
}
