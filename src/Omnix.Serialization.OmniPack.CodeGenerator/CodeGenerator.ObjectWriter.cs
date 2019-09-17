using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnix.Serialization.OmniPack.CodeGenerator
{
    public static partial class CodeGenerator
    {
        private sealed class ObjectWriter
        {
            private readonly OmniPackDefinition _rocketFormatDefinition;
            private readonly IList<OmniPackDefinition> _externalOmniPackDefinitions;
            private readonly string _accessLevel;

            private const string CustomFormatterName = "___CustomFormatter";
            private const string HashCodeName = "___hashCode";

            public ObjectWriter(OmniPackDefinition rocketPackDefinition, IEnumerable<OmniPackDefinition> externalOmniPackDefinitions)
            {
                _rocketFormatDefinition = rocketPackDefinition;
                _externalOmniPackDefinitions = externalOmniPackDefinitions.ToList();

                var accessLevelOption = _rocketFormatDefinition.Options.FirstOrDefault(n => n.Name == "csharp_access_level");
                _accessLevel = accessLevelOption?.Value ?? "public";
            }

            /// <summary>
            /// プロパティ名からフィールド変数名を生成します。
            /// </summary>
            private static string GenerateFieldName(string name)
            {
                return name[0].ToString().ToLower() + name.Substring(1);
            }

            private static string GetTypeFullName(string name, params string[] types)
            {
                var result = name switch
                {
                    "Lazy<>" => $"System.Lazy<{types[0]}>",
                    "ReadOnlySequence<>" => $"System.Buffers.ReadOnlySequence<{types[0]}>",
                    "IBufferWriter<>" => $"System.Buffers.IBufferWriter<{types[0]}>",
                    "IOmniPackMessage<>" => $"Omnix.Serialization.OmniPack.IOmniPackMessage<{types[0]}>",
                    "OmniPackReader" => "Omnix.Serialization.OmniPack.OmniPackReader",
                    "OmniPackWriter" => "Omnix.Serialization.OmniPack.OmniPackWriter",
                    "IOmniPackFormatter<>" => $"Omnix.Serialization.OmniPack.IOmniPackFormatter<{types[0]}>",
                    "FormatException" => "System.FormatException",
                    "BytesOperations" => "Omnix.Base.BytesOperations",
                    "CollectionHelper" => "Omnix.Base.Helpers.CollectionHelper",
                    "ObjectHelper" => "Omnix.Base.Helpers.ObjectHelper",
                    "HashCode" => "System.HashCode",
                    "Array" => "System.Array",
                    "Timestamp" => "Omnix.Serialization.OmniPack.Timestamp",
                    "IMemoryOwner<>" => $"System.Buffers.IMemoryOwner<{types[0]}>",
                    "Span<>" => $"System.Span<{types[0]}>",
                    "ReadOnlySpan<>" => $"System.ReadOnlySpan<{types[0]}>",
                    "Memory<>" => $"System.Memory<{types[0]}>",
                    "ReadOnlyMemory<>" => $"System.ReadOnlyMemory<{types[0]}>",
                    "ReadOnlyListSlim<>" => $"Omnix.DataStructures.ReadOnlyListSlim<{types[0]}>",
                    "ReadOnlyDictionarySlim<,>" => $"Omnix.DataStructures.ReadOnlyDictionarySlim<{types[0]}, {types[1]}>",
                    "Dictionary<,>" => $"System.Collections.Generic.Dictionary<{types[0]}, {types[1]}>",
                    "OmniPackMessageBase<>" => $"Omnix.Serialization.OmniPack.OmniPackMessageBase<{types[0]}>",
                    "IDisposable" => "System.IDisposable",
                    "BufferPool" => "Omnix.Base.BufferPool",
                    "SimpleMemoryOwner<>" => $"Omnix.Base.SimpleMemoryOwner<{types[0]}>",
                    "ArgumentNullException" => "System.ArgumentNullException",
                    "ArgumentOutOfRangeException" => "System.ArgumentOutOfRangeException",
                    _ => throw new InvalidOperationException(name)
                };

                return "global::" + result;
            }

            private object? CustomTypeResolver(CustomType n)
            {
                foreach (var targetInfo in new[] { _rocketFormatDefinition }.Union(_externalOmniPackDefinitions))
                {
                    var enumInfo = targetInfo.Enums.FirstOrDefault(m => m.Name == n.TypeName);
                    if (enumInfo != null)
                    {
                        return enumInfo;
                    }

                    var messageInfo = targetInfo.Objects.FirstOrDefault(m => m.Name == n.TypeName);
                    if (messageInfo != null)
                    {
                        return messageInfo;
                    }
                }

                return null;
            }

            private string GetParameterTypeString(TypeBase typeBase)
            {
                return typeBase switch
                {
                    BoolType type => "bool" + (type.IsOptional ? "?" : ""),
                    IntType type when (!type.IsSigned && type.Size == 8) => "byte" + (type.IsOptional ? "?" : ""),
                    IntType type when (!type.IsSigned && type.Size == 16) => "ushort" + (type.IsOptional ? "?" : ""),
                    IntType type when (!type.IsSigned && type.Size == 32) => "uint" + (type.IsOptional ? "?" : ""),
                    IntType type when (!type.IsSigned && type.Size == 64) => "ulong" + (type.IsOptional ? "?" : ""),
                    IntType type when (type.IsSigned && type.Size == 8) => "sbyte" + (type.IsOptional ? "?" : ""),
                    IntType type when (type.IsSigned && type.Size == 16) => "short" + (type.IsOptional ? "?" : ""),
                    IntType type when (type.IsSigned && type.Size == 32) => "int" + (type.IsOptional ? "?" : ""),
                    IntType type when (type.IsSigned && type.Size == 64) => "long" + (type.IsOptional ? "?" : ""),
                    FloatType type when (type.Size == 32) => "float" + (type.IsOptional ? "?" : ""),
                    FloatType type when (type.Size == 64) => "double" + (type.IsOptional ? "?" : ""),
                    StringType type => "string" + (type.IsOptional ? "?" : ""),
                    TimestampType type => GetTypeFullName("Timestamp") + (type.IsOptional ? "?" : ""),
                    BytesType type when (type.IsUseMemoryPool) => GetTypeFullName("IMemoryOwner<>", "byte") + (type.IsOptional ? "?" : ""),
                    BytesType type when (!type.IsUseMemoryPool) => GetTypeFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : ""),
                    VectorType type => $"{this.GetParameterTypeString(type.ElementType)}[]" + (type.IsOptional ? "?" : ""),
                    MapType type => GetTypeFullName("Dictionary<,>", this.GetParameterTypeString(type.KeyType), this.GetParameterTypeString(type.ValueType)) + (type.IsOptional ? "?" : ""),
                    CustomType type => this.CustomTypeResolver(type) switch
                    {
                        EnumDefinition _ => type.TypeName + (type.IsOptional ? "?" : ""),
                        ObjectDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Table) => type.TypeName + (type.IsOptional ? "?" : ""),
                        ObjectDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Struct) => type.TypeName + (type.IsOptional ? "?" : ""),
                        _ => throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(type)),
                    },
                    _ => throw new ArgumentException($"Type \"{typeBase.GetType().Name}\" was not found", nameof(typeBase)),
                };
            }

            private string GetPropertyTypeString(TypeBase typeBase)
            {
                return typeBase switch
                {
                    BoolType type => "bool" + (type.IsOptional ? "?" : ""),
                    IntType type when (!type.IsSigned && type.Size == 8) => "byte" + (type.IsOptional ? "?" : ""),
                    IntType type when (!type.IsSigned && type.Size == 16) => "ushort" + (type.IsOptional ? "?" : ""),
                    IntType type when (!type.IsSigned && type.Size == 32) => "uint" + (type.IsOptional ? "?" : ""),
                    IntType type when (!type.IsSigned && type.Size == 64) => "ulong" + (type.IsOptional ? "?" : ""),
                    IntType type when (type.IsSigned && type.Size == 8) => "sbyte" + (type.IsOptional ? "?" : ""),
                    IntType type when (type.IsSigned && type.Size == 16) => "short" + (type.IsOptional ? "?" : ""),
                    IntType type when (type.IsSigned && type.Size == 32) => "int" + (type.IsOptional ? "?" : ""),
                    IntType type when (type.IsSigned && type.Size == 64) => "long" + (type.IsOptional ? "?" : ""),
                    FloatType type when (type.Size == 32) => "float" + (type.IsOptional ? "?" : ""),
                    FloatType type when (type.Size == 64) => "double" + (type.IsOptional ? "?" : ""),
                    StringType type => "string" + (type.IsOptional ? "?" : ""),
                    TimestampType type => GetTypeFullName("Timestamp") + (type.IsOptional ? "?" : ""),
                    BytesType type when (!type.IsUseMemoryPool) => GetTypeFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : ""),
                    BytesType type when (type.IsUseMemoryPool) => GetTypeFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : ""),
                    VectorType type => GetTypeFullName("ReadOnlyListSlim<>", this.GetParameterTypeString(type.ElementType)) + (type.IsOptional ? "?" : ""),
                    MapType type => GetTypeFullName("ReadOnlyDictionarySlim<,>", this.GetParameterTypeString(type.KeyType), this.GetParameterTypeString(type.ValueType)) + (type.IsOptional ? "?" : ""),
                    CustomType type => this.CustomTypeResolver(type) switch
                    {
                        EnumDefinition _ => type.TypeName + (type.IsOptional ? "?" : ""),
                        ObjectDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Table) => type.TypeName + (type.IsOptional ? "?" : ""),
                        ObjectDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Struct) => type.TypeName + (type.IsOptional ? "?" : ""),
                        _ => throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(type)),
                    },
                    _ => throw new ArgumentException($"Type \"{typeBase.GetType().Name}\" was not found", nameof(typeBase)),
                };
            }

            private string GetDefaultValueString(TypeBase typeBase)
            {
                return typeBase switch
                {
                    BoolType type => type.IsOptional ? "null" : "false",
                    IntType type => type.IsOptional ? "null" : "0",
                    FloatType type when (type.Size == 32) => type.IsOptional ? "null" : "0.0F",
                    FloatType type when (type.Size == 64) => type.IsOptional ? "null" : "0.0D",
                    StringType type => type.IsOptional ? "null" : "string.Empty",
                    TimestampType type => type.IsOptional ? "null" : GetTypeFullName("Timestamp") + ".Zero",
                    BytesType type when (!type.IsUseMemoryPool) => type.IsOptional ? "null" : GetTypeFullName("ReadOnlyMemory<>", "byte") + ".Empty",
                    BytesType type when (type.IsUseMemoryPool) => type.IsOptional ? "null" : GetTypeFullName("SimpleMemoryOwner<>", "byte") + ".Empty",
                    VectorType type => type.IsOptional ? "null" : GetTypeFullName("Array") + ".Empty<" + this.GetParameterTypeString(type.ElementType) + ">()",
                    MapType type => type.IsOptional
                            ? "null"
                            : "new " + GetTypeFullName("Dictionary<,>", this.GetParameterTypeString(type.KeyType), this.GetParameterTypeString(type.ValueType)) + "()",
                    CustomType type => this.CustomTypeResolver(type) switch
                    {
                        EnumDefinition elementEnumDefinition => type.IsOptional ? "null" : $"({elementEnumDefinition.Name})0",
                        ObjectDefinition elementMessageDefinition => type.IsOptional ? "null" : $"{elementMessageDefinition.Name}.Empty",
                        _ => throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(typeBase)),
                    },
                    _ => throw new ArgumentException($"Type \"{typeBase.GetType().Name}\" was not found", nameof(typeBase)),
                };
            }

            public void Write(CodeWriter w, ObjectDefinition messageDefinition)
            {
                if (messageDefinition.IsStruct)
                {
                    if (messageDefinition.Elements.Select(n => n.Type).OfType<BytesType>().Any(n => n.IsUseMemoryPool))
                    {
                        w.WriteLine($"{_accessLevel} readonly struct {messageDefinition.Name} : {GetTypeFullName("IOmniPackMessage<>", messageDefinition.Name)}, {GetTypeFullName("IDisposable")}");
                    }
                    else
                    {
                        w.WriteLine($"{_accessLevel} readonly struct {messageDefinition.Name} : {GetTypeFullName("IOmniPackMessage<>", messageDefinition.Name)}");
                    }
                }
                else if (messageDefinition.IsClass)
                {
                    if (messageDefinition.Elements.Select(n => n.Type).OfType<BytesType>().Any(n => n.IsUseMemoryPool))
                    {
                        w.WriteLine($"{_accessLevel} sealed partial class {messageDefinition.Name} : {GetTypeFullName("IOmniPackMessage<>", messageDefinition.Name)}, {GetTypeFullName("IDisposable")}");
                    }
                    else
                    {
                        w.WriteLine($"{_accessLevel} sealed partial class {messageDefinition.Name} : {GetTypeFullName("IOmniPackMessage<>", messageDefinition.Name)}");
                    }
                }

                w.WriteLine("{");

                using (w.Indent())
                {
                    this.Write_StaticConstructor(w, messageDefinition);
                    w.WriteLine();

                    this.Write_Constructor(w, messageDefinition);
                    w.WriteLine();

                    this.Write_Properties(w, messageDefinition);
                    w.WriteLine();

                    this.Write_ImportAndExport(w, messageDefinition);
                    w.WriteLine();

                    this.Write_Equals(w, messageDefinition);
                    w.WriteLine();

                    if (messageDefinition.Elements.Select(n => n.Type).OfType<BytesType>().Any(n => n.IsUseMemoryPool))
                    {
                        this.Write_Dispose(w, messageDefinition);
                        w.WriteLine();
                    }

                    if (messageDefinition.FormatType == MessageFormatType.Table)
                    {
                        this.Write_Medium_Formatter(w, messageDefinition);
                    }
                    else if (messageDefinition.FormatType == MessageFormatType.Struct)
                    {
                        this.Write_Small_Formatter(w, messageDefinition);
                    }
                }

                w.WriteLine("}");
            }

            /// <summary>
            /// 静的コンストラクタの生成。
            /// </summary>
            private void Write_StaticConstructor(CodeWriter w, ObjectDefinition messageDefinition)
            {
                w.WriteLine($"public static {GetTypeFullName("IOmniPackFormatter<>", messageDefinition.Name)} Formatter" + " { get; }");
                w.WriteLine($"public static {messageDefinition.Name} Empty" + " { get; }");
                w.WriteLine();

                w.WriteLine($"static {messageDefinition.Name}()");
                w.WriteLine("{");

                using (w.Indent())
                {
                    // CustomFormatterのインスタンスの作成
                    this.Write_StaticConstructor_CustomFormatterProperty(w, messageDefinition);

                    // Emptyのインスタンスの作成
                    this.Write_StaticConstructor_EmptyProperty(w, messageDefinition);
                }

                w.WriteLine("}");
            }

            private void Write_StaticConstructor_CustomFormatterProperty(CodeWriter w, ObjectDefinition messageDefinition)
            {
                w.WriteLine($"{messageDefinition.Name}.Formatter = new {CustomFormatterName}();");
            }

            private void Write_StaticConstructor_EmptyProperty(CodeWriter w, ObjectDefinition messageDefinition)
            {
                var parameters = new List<string>();

                foreach (var element in messageDefinition.Elements)
                {
                    parameters.Add(this.GetDefaultValueString(element.Type));
                }

                w.WriteLine($"{messageDefinition.Name}.Empty = new {messageDefinition.Name}({string.Join(", ", parameters)});");
            }

            /// <summary>
            /// コンストラクタの生成。
            /// </summary>
            private void Write_Constructor(CodeWriter w, ObjectDefinition messageDefinition)
            {
                if (messageDefinition.IsStruct)
                {
                    w.WriteLine($"private readonly int {HashCodeName};");
                }
                else if (messageDefinition.IsClass)
                {
                    w.WriteLine($"private readonly {GetTypeFullName("Lazy<>", "int")} {HashCodeName};");
                }
                w.WriteLine();

                // 最大サイズの宣言。
                this.Write_Constructor_Define_MaxLength(w, messageDefinition);

                // パラメータの生成。
                w.WriteLine($"public {messageDefinition.Name}({string.Join(", ", messageDefinition.Elements.Select(element => this.GetParameterTypeString(element.Type) + " " + GenerateFieldName(element.Name)))})");
                w.WriteLine("{");

                using (w.Indent())
                {
                    // パラメータのチェック
                    this.Write_Constructor_Parameter_Check(w, messageDefinition);

                    // 初期化
                    this.Write_Constructor_Init(w, messageDefinition);

                    // HashCodeの値の算出
                    this.Write_Constructor_HashCode(w, messageDefinition);
                }

                w.WriteLine("}");
            }

            private void Write_Constructor_Init(CodeWriter w, ObjectDefinition messageDefinition)
            {
                foreach (var elementInfo in messageDefinition.Elements)
                {
                    switch (elementInfo.Type)
                    {
                        case BytesType type when (type.IsUseMemoryPool):
                            w.WriteLine($"_{GenerateFieldName(elementInfo.Name)} = {GenerateFieldName(elementInfo.Name)};");
                            break;
                        case VectorType type:
                            if (type.IsOptional)
                            {
                                w.WriteLine($"if({GenerateFieldName(elementInfo.Name)} != null)");
                                w.WriteLine("{");
                                w.PushIndent();
                            }

                            w.WriteLine($"this.{elementInfo.Name} = new {GetTypeFullName("ReadOnlyListSlim<>", this.GetParameterTypeString(type.ElementType))}({GenerateFieldName(elementInfo.Name)});");

                            if (type.IsOptional)
                            {
                                w.PopIndent();

                                w.WriteLine("}");
                                w.WriteLine("else");
                                w.WriteLine("{");

                                using (w.Indent())
                                {
                                    w.WriteLine($"this.{elementInfo.Name} = null;");
                                }

                                w.WriteLine("}");
                            }
                            break;
                        case MapType type:
                            if (type.IsOptional)
                            {
                                w.WriteLine($"if({GenerateFieldName(elementInfo.Name)} != null)");
                                w.WriteLine("{");
                                w.PushIndent();
                            }

                            w.WriteLine($"this.{elementInfo.Name} = new {GetTypeFullName("ReadOnlyDictionarySlim<,>", this.GetParameterTypeString(type.KeyType), this.GetParameterTypeString(type.ValueType))}({GenerateFieldName(elementInfo.Name)});");

                            if (type.IsOptional)
                            {
                                w.PopIndent();

                                w.WriteLine("}");
                                w.WriteLine("else");
                                w.WriteLine("{");

                                using (w.Indent())
                                {
                                    w.WriteLine($"this.{elementInfo.Name} = null;");
                                }

                                w.WriteLine("}");
                            }
                            break;
                        default:
                            w.WriteLine($"this.{elementInfo.Name} = {GenerateFieldName(elementInfo.Name)};");
                            break;
                    }
                }

                w.WriteLine();
            }

            private void Write_Constructor_Define_MaxLength(CodeWriter w, ObjectDefinition messageDefinition)
            {
                bool isDefinedMaxLength = false;

                foreach (var elementInfo in messageDefinition.Elements)
                {
                    isDefinedMaxLength |= this.Try_Write_Constructor_Define_MaxLength_Element(w, elementInfo.Name, elementInfo.Type);
                }

                if (isDefinedMaxLength)
                {
                    w.WriteLine();
                }
            }

            private bool Try_Write_Constructor_Define_MaxLength_Element(CodeWriter w, string name, TypeBase type)
            {
                switch (type)
                {
                    case StringType stringType:
                        w.WriteLine($"public static readonly int Max{name}Length = {stringType.MaxLength};");
                        return true;
                    case BytesType memoryType:
                        w.WriteLine($"public static readonly int Max{name}Length = {memoryType.MaxLength};");
                        return true;
                    case VectorType listType:
                        w.WriteLine($"public static readonly int Max{name}Count = {listType.MaxLength};");
                        return true;
                    case MapType mapType:
                        w.WriteLine($"public static readonly int Max{name}Count = {mapType.MaxLength};");
                        return true;
                    default:
                        return false;
                }
            }

            private void Write_Constructor_Parameter_Check(CodeWriter w, ObjectDefinition messageInfo)
            {
                bool isChecked = false;

                foreach (var elementInfo in messageInfo.Elements)
                {
                    isChecked = this.Try_Write_Constructor_Parameter_Check_Element(w, elementInfo);
                }

                if (isChecked)
                {
                    w.WriteLine();
                }
            }

            private bool Try_Write_Constructor_Parameter_Check_Element(CodeWriter w, ObjectElement elementInfo)
            {
                bool isChecked = false;

                isChecked |= this.Try_Write_Constructor_Parameter_Check_Element_Null(w, GenerateFieldName(elementInfo.Name), elementInfo.Type);
                isChecked |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(w, GenerateFieldName(elementInfo.Name), elementInfo.Type);

                if (elementInfo.Type is VectorType listType)
                {
                    var w2 = new CodeWriter();
                    bool isChecked2 = false;

                    if (listType.IsOptional)
                    {
                        w2.WriteLine($"if (!({GenerateFieldName(elementInfo.Name)} is null))");
                        w2.WriteLine("{");

                        w2.PushIndent();
                    }

                    w2.WriteLine($"foreach (var n in {GenerateFieldName(elementInfo.Name)})");
                    w2.WriteLine("{");

                    using (w2.Indent())
                    {
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_Null(w2, "n", listType.ElementType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(w2, "n", listType.ElementType);
                    }

                    w2.WriteLine("}");

                    if (listType.IsOptional)
                    {
                        w2.PopIndent();

                        w2.WriteLine("}");
                    }

                    if (isChecked2)
                    {
                        w.WriteLine(w2.ToString().TrimEnd());
                    }

                    isChecked |= isChecked2;
                }
                else if (elementInfo.Type is MapType mapType)
                {
                    var w2 = new CodeWriter();
                    bool isChecked2 = false;

                    if (mapType.IsOptional)
                    {
                        w2.WriteLine($"if (!({GenerateFieldName(elementInfo.Name)} is null))");
                        w2.WriteLine("{");

                        w2.PushIndent();
                    }

                    w2.WriteLine($"foreach (var n in {GenerateFieldName(elementInfo.Name)})");
                    w2.WriteLine("{");

                    using (w2.Indent())
                    {
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_Null(w2, "n.Key", mapType.KeyType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_Null(w2, "n.Value", mapType.ValueType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(w2, "n.Key", mapType.KeyType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(w2, "n.Value", mapType.ValueType);
                    }

                    w2.WriteLine("}");

                    if (mapType.IsOptional)
                    {
                        w2.PopIndent();

                        w2.WriteLine("}");
                    }

                    if (isChecked2)
                    {
                        w.WriteLine(w2.ToString().TrimEnd());
                    }

                    isChecked |= isChecked2;
                }

                return isChecked;
            }

            private bool Try_Write_Constructor_Parameter_Check_Element_Null(CodeWriter w, string name, TypeBase type)
            {
                switch (type)
                {
                    case StringType stringType when (!type.IsOptional):
                        w.WriteLine($"if ({name} is null) throw new {GetTypeFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case BytesType memoryType when (!type.IsOptional && memoryType.IsUseMemoryPool):
                        w.WriteLine($"if ({name} is null) throw new {GetTypeFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case VectorType listType when (!type.IsOptional):
                        w.WriteLine($"if ({name} is null) throw new {GetTypeFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case MapType mapType when (!type.IsOptional):
                        w.WriteLine($"if ({name} is null) throw new {GetTypeFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case CustomType customType when (!type.IsOptional):
                        switch (this.CustomTypeResolver(customType))
                        {
                            case ObjectDefinition messageInfo when (messageInfo.IsClass):
                                w.WriteLine($"if ({name} is null) throw new {GetTypeFullName("ArgumentNullException")}(\"{name}\");");
                                return true;
                            default:
                                return false;
                        }
                    default:
                        return false;
                }
            }

            private bool Try_Write_Constructor_Parameter_Check_Element_MaxLength(CodeWriter w, string name, TypeBase type)
            {
                string property;
                int maxLength;

                switch (type)
                {
                    case StringType stringType:
                        property = "Length";
                        maxLength = stringType.MaxLength;
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool && !memoryType.IsOptional):
                        property = "Length";
                        maxLength = memoryType.MaxLength;
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool && memoryType.IsOptional):
                        property = "Value.Length";
                        maxLength = memoryType.MaxLength;
                        break;
                    case BytesType memoryType when (memoryType.IsUseMemoryPool):
                        property = "Memory.Length";
                        maxLength = memoryType.MaxLength;
                        break;
                    case VectorType listType:
                        property = "Length";
                        maxLength = listType.MaxLength;
                        break;
                    case MapType mapType:
                        property = "Count";
                        maxLength = mapType.MaxLength;
                        break;
                    default:
                        return false;
                }

                if (type.IsOptional)
                {
                    w.WriteLine($"if (!({name} is null) && {name}.{property} > {maxLength}) throw new {GetTypeFullName("ArgumentOutOfRangeException")}(\"{name}\");");
                }
                else
                {
                    w.WriteLine($"if ({name}.{property} > {maxLength}) throw new {GetTypeFullName("ArgumentOutOfRangeException")}(\"{name}\");");
                }

                return true;
            }

            private void Write_Constructor_HashCode(CodeWriter w, ObjectDefinition messageInfo)
            {
                const string TempVariableName = "___h";

                if (messageInfo.IsStruct)
                {
                    w.WriteLine("{");

                    using (w.Indent())
                    {
                        w.WriteLine($"var {TempVariableName} = new {GetTypeFullName("HashCode")}();");

                        foreach (var elementInfo in messageInfo.Elements)
                        {
                            this.Write_Constructor_HashCode_Element(w, TempVariableName, GenerateFieldName(elementInfo.Name), elementInfo.Type);
                        }

                        w.WriteLine($"{HashCodeName} = {TempVariableName}.ToHashCode();");
                    }

                    w.WriteLine("}");
                }
                else
                {
                    w.WriteLine($"{HashCodeName} = new {GetTypeFullName("Lazy<>", "int")}(() =>");
                    w.WriteLine("{");

                    using (w.Indent())
                    {
                        w.WriteLine($"var {TempVariableName} = new {GetTypeFullName("HashCode")}();");

                        foreach (var elementInfo in messageInfo.Elements)
                        {
                            this.Write_Constructor_HashCode_Element(w, TempVariableName, GenerateFieldName(elementInfo.Name), elementInfo.Type);
                        }

                        w.WriteLine($"return {TempVariableName}.ToHashCode();");
                    }

                    w.WriteLine("});");
                }
            }

            private void Write_Constructor_HashCode_Element(CodeWriter w, string hashCodeName, string parameterName, TypeBase type)
            {
                switch (type)
                {
                    case BoolType boolType:
                        w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case IntType intType:
                        w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case FloatType floatType:
                        w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case StringType stringType:
                        w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case TimestampType timestampType:
                        w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case BytesType memoryType when (!memoryType.IsOptional):
                        if (!memoryType.IsUseMemoryPool)
                        {
                            w.WriteLine($"if (!{parameterName}.IsEmpty) {hashCodeName}.Add({GetTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Span));");
                        }
                        else
                        {
                            w.WriteLine($"if (!{parameterName}.Memory.IsEmpty) {hashCodeName}.Add({GetTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Memory.Span));");
                        }
                        break;
                    case BytesType memoryType when (memoryType.IsOptional):
                        if (!memoryType.IsUseMemoryPool)
                        {
                            w.WriteLine($"if (!({parameterName} is null) && !{parameterName}.Value.IsEmpty) {hashCodeName}.Add({GetTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Value.Span));");
                        }
                        else
                        {
                            w.WriteLine($"if (!({parameterName} is null) && !{parameterName}.Memory.IsEmpty) {hashCodeName}.Add({GetTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Memory.Span));");
                        }
                        break;
                    case VectorType listType:
                        {
                            if (type.IsOptional)
                            {
                                w.WriteLine($"if({GenerateFieldName(parameterName)} != null)");
                                w.WriteLine("{");
                                w.PushIndent();
                            }

                            w.WriteLine($"foreach (var n in {GenerateFieldName(parameterName)})");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Constructor_HashCode_Element(w, hashCodeName, "n", listType.ElementType);
                            }

                            w.WriteLine("}");

                            if (type.IsOptional)
                            {
                                w.PopIndent();
                                w.WriteLine("}");
                            }
                        }
                        break;
                    case MapType mapType:
                        {
                            if (type.IsOptional)
                            {
                                w.WriteLine($"if({GenerateFieldName(parameterName)} != null)");
                                w.WriteLine("{");
                                w.PushIndent();
                            }

                            w.WriteLine($"foreach (var n in {GenerateFieldName(parameterName)})");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Constructor_HashCode_Element(w, hashCodeName, "n.Key", mapType.KeyType);
                                this.Write_Constructor_HashCode_Element(w, hashCodeName, "n.Value", mapType.ValueType);
                            }

                            w.WriteLine("}");

                            if (type.IsOptional)
                            {
                                w.PopIndent();
                                w.WriteLine("}");
                            }
                        }
                        break;
                    case CustomType customType:
                        {
                            switch (this.CustomTypeResolver(customType))
                            {
                                case EnumDefinition enumInfo:
                                    w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                                    break;
                                case ObjectDefinition messageInfo when (messageInfo.IsStruct):
                                    if (!customType.IsOptional)
                                    {
                                        w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                                    }
                                    else
                                    {
                                        w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.Value.GetHashCode());");
                                    }
                                    break;
                                case ObjectDefinition messageInfo when (messageInfo.IsClass):
                                    w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                                    break;
                            }
                        }
                        break;
                }
            }

            private void Write_ImportAndExport(CodeWriter w, ObjectDefinition messageDefinition)
            {
                w.WriteLine($"public static {messageDefinition.Name} Import({GetTypeFullName("ReadOnlySequence<>", "byte")} sequence, {GetTypeFullName("BufferPool")} bufferPool)");
                w.WriteLine("{");
                using (w.Indent())
                {
                    w.WriteLine($"var reader = new {GetTypeFullName("OmniPackReader")}(sequence, bufferPool);");
                    w.WriteLine($"return Formatter.Deserialize(ref reader, 0);");
                }
                w.WriteLine("}");

                w.WriteLine($"public void Export({GetTypeFullName("IBufferWriter<>", "byte")} bufferWriter, {GetTypeFullName("BufferPool")} bufferPool)");
                w.WriteLine("{");
                using (w.Indent())
                {
                    w.WriteLine($"var writer = new {GetTypeFullName("OmniPackWriter")}(bufferWriter, bufferPool);");
                    w.WriteLine($"Formatter.Serialize(ref writer, this, 0);");
                }
                w.WriteLine("}");
            }

            private void Write_Equals(CodeWriter w, ObjectDefinition messageDefinition)
            {
                if (messageDefinition.IsStruct)
                {
                    w.WriteLine($"public static bool operator ==({messageDefinition.Name} left, {messageDefinition.Name} right)");
                    w.WriteLine("{");
                    using (w.Indent())
                    {
                        w.WriteLine("return right.Equals(left);");
                    }
                    w.WriteLine("}");

                    w.WriteLine($"public static bool operator !=({messageDefinition.Name} left, {messageDefinition.Name} right)");
                    w.WriteLine("{");
                    using (w.Indent())
                    {
                        w.WriteLine("return !(left == right);");
                    }
                    w.WriteLine("}");
                }
                else if (messageDefinition.IsClass)
                {
                    w.WriteLine($"public static bool operator ==({messageDefinition.Name}? left, {messageDefinition.Name}? right)");
                    w.WriteLine("{");
                    using (w.Indent())
                    {
                        w.WriteLine("return (right is null) ? (left is null) : right.Equals(left);");
                    }
                    w.WriteLine("}");

                    w.WriteLine($"public static bool operator !=({messageDefinition.Name}? left, {messageDefinition.Name}? right)");
                    w.WriteLine("{");
                    using (w.Indent())
                    {
                        w.WriteLine("return !(left == right);");
                    }
                    w.WriteLine("}");
                }

                w.WriteLine("public override bool Equals(object? other)");
                w.WriteLine("{");
                using (w.Indent())
                {
                    w.WriteLine($"if (!(other is {messageDefinition.Name})) return false;");
                    w.WriteLine($"return this.Equals(({messageDefinition.Name})other);");
                }
                w.WriteLine("}");

                if (messageDefinition.IsStruct)
                {
                    w.WriteLine($"public bool Equals({messageDefinition.Name} target)");
                }
                else if (messageDefinition.IsClass)
                {
                    w.WriteLine($"public bool Equals({messageDefinition.Name}? target)");
                }
                w.WriteLine("{");

                using (w.Indent())
                {
                    if (messageDefinition.IsClass)
                    {
                        w.WriteLine("if (target is null) return false;");
                        w.WriteLine("if (object.ReferenceEquals(this, target)) return true;");
                    }

                    foreach (var element in messageDefinition.Elements)
                    {
                        switch (element.Type)
                        {
                            case BoolType type:
                                w.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case IntType type:
                                w.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case FloatType type when (type.Size == 32):
                                w.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case StringType type:
                                w.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case TimestampType type:
                                w.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case BytesType type:
                                if (!type.IsOptional)
                                {
                                    w.WriteLine($"if (!{GetTypeFullName("BytesOperations")}.SequenceEqual(this.{element.Name}.Span, target.{element.Name}.Span)) return false;");
                                }
                                else
                                {
                                    w.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                    w.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && !{GetTypeFullName("BytesOperations")}.SequenceEqual(this.{element.Name}.Value.Span, target.{element.Name}.Value.Span)) return false;");
                                }
                                break;
                            case VectorType type:
                                if (!type.IsOptional)
                                {
                                    w.WriteLine($"if (!{GetTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                else
                                {
                                    w.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                    w.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && !{GetTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                break;
                            case MapType type:
                                if (!type.IsOptional)
                                {
                                    w.WriteLine($"if (!{GetTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                else
                                {
                                    w.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                    w.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && !{GetTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                break;
                            case CustomType type:
                                {
                                    switch (this.CustomTypeResolver(type))
                                    {
                                        case EnumDefinition enumInfo:
                                            w.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                            break;
                                        case ObjectDefinition messageInfo when (messageInfo.IsStruct):
                                            if (!type.IsOptional)
                                            {
                                                w.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                            }
                                            else
                                            {
                                                w.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                                w.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && this.{element.Name} != target.{element.Name}) return false;");
                                            }
                                            break;
                                        case ObjectDefinition messageInfo when (messageInfo.IsClass):
                                            if (!type.IsOptional)
                                            {
                                                w.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                            }
                                            else
                                            {
                                                w.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                                w.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && this.{element.Name} != target.{element.Name}) return false;");
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                    w.WriteLine();

                    w.WriteLine("return true;");
                }


                w.WriteLine("}");

                if (messageDefinition.IsStruct)
                {
                    w.WriteLine($"public override int GetHashCode() => {HashCodeName};");
                }
                else if (messageDefinition.IsClass)
                {
                    w.WriteLine($"public override int GetHashCode() => {HashCodeName}.Value;");
                }
            }

            private void Write_Properties(CodeWriter w, ObjectDefinition messageInfo)
            {
                foreach (var element in messageInfo.Elements.OrderBy(n => n.Id))
                {
                    switch (element.Type)
                    {
                        case BytesType type when (type.IsUseMemoryPool):
                            if (!type.IsOptional)
                            {
                                w.WriteLine($"private readonly {this.GetParameterTypeString(element.Type)} _{GenerateFieldName(element.Name)};");
                                w.WriteLine($"public {this.GetPropertyTypeString(element.Type)} {element.Name} => _{GenerateFieldName(element.Name)}.Memory;");
                            }
                            else
                            {
                                w.WriteLine($"private readonly {this.GetParameterTypeString(element.Type)} _{GenerateFieldName(element.Name)};");
                                w.WriteLine($"public {this.GetPropertyTypeString(element.Type)} {element.Name} => _{GenerateFieldName(element.Name)}?.Memory;");
                            }
                            break;
                        default:
                            w.WriteLine($"public {this.GetPropertyTypeString(element.Type)} {element.Name} {{ get; }}");
                            break;
                    }
                }
            }

            private void Write_Dispose(CodeWriter w, ObjectDefinition messageInfo)
            {
                w.WriteLine("public void Dispose()");
                w.WriteLine("{");

                using (w.Indent())
                {
                    foreach (var element in messageInfo.Elements)
                    {
                        switch (element.Type)
                        {
                            case BytesType type when (type.IsUseMemoryPool):
                                w.WriteLine($"_{GenerateFieldName(element.Name)}?.Dispose();");
                                break;
                        }
                    }
                }

                w.WriteLine("}");
            }

            private void Write_Medium_Formatter(CodeWriter w, ObjectDefinition messageFormat)
            {
                w.WriteLine($"private sealed class ___CustomFormatter : {GetTypeFullName("IOmniPackFormatter<>", messageFormat.Name)}");
                w.WriteLine("{");

                using (w.Indent())
                {
                    {
                        w.WriteLine($"public void Serialize(ref {GetTypeFullName("OmniPackWriter")} w, in {messageFormat.Name} value, in int rank)");
                        w.WriteLine("{");

                        using (w.Indent())
                        {
                            w.WriteLine($"if (rank > 256) throw new {GetTypeFullName("FormatException")}();");
                            w.WriteLine();

                            {
                                w.WriteLine("{");

                                using (w.Indent())
                                {
                                    w.WriteLine("uint propertyCount = 0;");

                                    foreach (var element in messageFormat.Elements)
                                    {
                                        this.BlockWhoseValueIsNotDefault(w, element, () =>
                                        {
                                            w.WriteLine("propertyCount++;");
                                        });
                                    }

                                    w.WriteLine($"w.Write(propertyCount);");

                                }

                                w.WriteLine("}");
                            }

                            w.WriteLine();

                            foreach (var element in messageFormat.Elements)
                            {
                                this.BlockWhoseValueIsNotDefault(w, element, () =>
                                {
                                    w.WriteLine($"w.Write((uint){element.Id});");
                                    this.Write_Medium_Formatter_Serialize_PropertyDef(w, "value." + element.Name, element.Type, 0);
                                });
                            }

                        }

                        w.WriteLine("}");
                    }

                    w.WriteLine();

                    {
                        w.WriteLine($"public {messageFormat.Name} Deserialize(ref {GetTypeFullName("OmniPackReader")} r, in int rank)");
                        w.WriteLine("{");

                        using (w.Indent())
                        {

                            w.WriteLine($"if (rank > 256) throw new {GetTypeFullName("FormatException")}();");
                            w.WriteLine();

                            w.WriteLine("uint propertyCount = r.GetUInt32();");
                            w.WriteLine();

                            foreach (var elementInfo in messageFormat.Elements)
                            {
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GenerateFieldName(elementInfo.Name)} = {this.GetDefaultValueString(elementInfo.Type)};");
                            }
                            w.WriteLine();

                            w.WriteLine("for (; propertyCount > 0; propertyCount--)");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                w.WriteLine("uint id = r.GetUInt32();");
                                w.WriteLine("switch (id)");
                                w.WriteLine("{");

                                using (w.Indent())
                                {
                                    foreach (var elementInfo in messageFormat.Elements)
                                    {
                                        w.WriteLine($"case {elementInfo.Id}:");

                                        using (w.Indent())
                                        {
                                            w.WriteLine("{");

                                            using (w.Indent())
                                            {
                                                this.Write_Medium_Formatter_Deserialize_PropertyDef(w, "p_" + GenerateFieldName(elementInfo.Name), elementInfo.Type, 0);

                                                w.WriteLine("break;");
                                            }

                                            w.WriteLine("}");
                                        }
                                    }
                                }

                                w.WriteLine("}");
                            }

                            w.WriteLine("}");
                            w.WriteLine();

                            w.WriteLine($"return new {messageFormat.Name}({ string.Join(", ", messageFormat.Elements.Select(n => "p_" + GenerateFieldName(n.Name)))});");
                        }

                        w.WriteLine("}");
                    }
                }

                w.WriteLine("}");
            }

            private void Write_Medium_Formatter_Serialize_PropertyDef(CodeWriter w, string name, TypeBase type, int rank)
            {
                switch (type)
                {
                    case BoolType boolType:
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case IntType inttype:
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case FloatType floatType:
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case StringType stringType:
                        w.WriteLine($"w.Write({name});");
                        break;
                    case TimestampType timestampType:
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case BytesType memoryType when (memoryType.IsUseMemoryPool):
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name}.Span);");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value.Span);");
                        }
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool):
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name}.Span);");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value.Span);");
                        }
                        break;
                    case VectorType listType:
                        {
                            w.WriteLine($"w.Write((uint){name}.Count);");
                            w.WriteLine($"foreach (var n in {name})");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Medium_Formatter_Serialize_PropertyDef(w, "n", listType.ElementType, rank + 1);
                            }

                            w.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            w.WriteLine($"w.Write((uint){name}.Count);");
                            w.WriteLine($"foreach (var n in {name})");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Medium_Formatter_Serialize_PropertyDef(w, "n.Key", mapType.KeyType, rank + 1);
                                this.Write_Medium_Formatter_Serialize_PropertyDef(w, "n.Value", mapType.ValueType, rank + 1);
                            }

                            w.WriteLine("}");
                        }
                        break;
                    case CustomType customType:
                        switch (this.CustomTypeResolver(customType))
                        {
                            case EnumDefinition enumDefinition:
                                switch (enumDefinition.Type)
                                {
                                    case IntType intType when (intType.IsSigned):
                                        if (!intType.IsOptional)
                                        {
                                            w.WriteLine($"w.Write((long){name});");
                                        }
                                        else
                                        {
                                            w.WriteLine($"w.Write((long){name}.Value);");
                                        }

                                        break;
                                    case IntType intType when (!intType.IsSigned):
                                        if (!intType.IsOptional)
                                        {
                                            w.WriteLine($"w.Write((ulong){name});");
                                        }
                                        else
                                        {
                                            w.WriteLine($"w.Write((ulong){name}.Value);");
                                        }

                                        break;
                                }
                                break;
                            case ObjectDefinition messageDefinition when (messageDefinition.IsStruct):
                                if (!type.IsOptional)
                                {
                                    w.WriteLine($"{messageDefinition.Name}.Formatter.Serialize(ref w, {name}, rank + 1);");
                                }
                                else
                                {
                                    w.WriteLine($"{messageDefinition.Name}.Formatter.Serialize(ref w, {name}.Value, rank + 1);");
                                }
                                break;
                            case ObjectDefinition messageDefinition when (messageDefinition.IsClass):
                                w.WriteLine($"{messageDefinition.Name}.Formatter.Serialize(ref w, {name}, rank + 1);");
                                break;
                        }
                        break;
                }
            }

            private void Write_Medium_Formatter_Deserialize_PropertyDef(CodeWriter w, string name, TypeBase type, int rank)
            {
                switch (type)
                {
                    case BoolType boolType:
                        w.WriteLine($"{name} = r.GetBoolean();");
                        break;
                    case IntType inttype when (!inttype.IsSigned):
                        w.WriteLine($"{name} = r.GetUInt{inttype.Size}();");
                        break;
                    case IntType inttype when (inttype.IsSigned):
                        w.WriteLine($"{name} = r.GetInt{inttype.Size}();");
                        break;
                    case FloatType floatType when (floatType.Size == 32):
                        w.WriteLine($"{name} = r.GetFloat32();");
                        break;
                    case FloatType floatType when (floatType.Size == 64):
                        w.WriteLine($"{name} = r.GetFloat64();");
                        break;
                    case StringType stringType:
                        w.WriteLine($"{name} = r.GetString({stringType.MaxLength});");
                        break;
                    case TimestampType timestampType:
                        w.WriteLine($"{name} = r.GetTimestamp();");
                        break;
                    case BytesType memoryType when (memoryType.IsUseMemoryPool):
                        w.WriteLine($"{name} = r.GetRecyclableMemory({memoryType.MaxLength});");
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool):
                        w.WriteLine($"{name} = r.GetMemory({memoryType.MaxLength});");
                        break;
                    case VectorType listType:
                        {
                            w.WriteLine("var length = r.GetUInt32();");
                            w.WriteLine($"{name} = new {this.GetParameterTypeString(listType.ElementType)}[length];");

                            w.WriteLine($"for (int i = 0; i < {name}.Length; i++)");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Medium_Formatter_Deserialize_PropertyDef(w, $"{name}[i]", listType.ElementType, rank + 1);
                            }

                            w.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            w.WriteLine("var length = r.GetUInt32();");
                            w.WriteLine($"{name} = new {GetTypeFullName("Dictionary<,>", this.GetParameterTypeString(mapType.KeyType), this.GetParameterTypeString(mapType.ValueType))}();");
                            w.WriteLine($"{this.GetParameterTypeString(mapType.KeyType)} t_key = {this.GetDefaultValueString(mapType.KeyType)};");
                            w.WriteLine($"{this.GetParameterTypeString(mapType.ValueType)} t_value = {this.GetDefaultValueString(mapType.ValueType)};");

                            w.WriteLine("for (int i = 0; i < length; i++)");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Medium_Formatter_Deserialize_PropertyDef(w, "t_key", mapType.KeyType, rank + 1);
                                this.Write_Medium_Formatter_Deserialize_PropertyDef(w, "t_value", mapType.ValueType, rank + 1);
                                w.WriteLine($"{name}[t_key] = t_value;");
                            }

                            w.WriteLine("}");
                        }
                        break;
                    case CustomType customType:
                        switch (this.CustomTypeResolver(customType))
                        {
                            case EnumDefinition enumInfo:
                                switch (enumInfo.Type)
                                {
                                    case IntType inttype when (inttype.IsSigned):
                                        w.WriteLine($"{name} = ({enumInfo.Name})r.GetInt64();");
                                        break;
                                    case IntType inttype when (!inttype.IsSigned):
                                        w.WriteLine($"{name} = ({enumInfo.Name})r.GetUInt64();");
                                        break;
                                }
                                break;
                            case ObjectDefinition messageInfo when (messageInfo.IsStruct):
                                w.WriteLine($"{name} = {messageInfo.Name}.Formatter.Deserialize(ref r, rank + 1);");
                                break;
                            case ObjectDefinition messageInfo when (messageInfo.IsClass):
                                w.WriteLine($"{name} = {messageInfo.Name}.Formatter.Deserialize(ref r, rank + 1);");
                                break;
                        }
                        break;
                }
            }

            private void Write_Small_Formatter(CodeWriter w, ObjectDefinition messageFormat)
            {
                w.WriteLine($"private sealed class ___CustomFormatter : {GetTypeFullName("IOmniPackFormatter<>", messageFormat.Name)}");
                w.WriteLine("{");

                using (w.Indent())
                {
                    {
                        w.WriteLine($"public void Serialize(ref {GetTypeFullName("OmniPackWriter")} w, in {messageFormat.Name} value, in int rank)");
                        w.WriteLine("{");

                        using (w.Indent())
                        {
                            w.WriteLine($"if (rank > 256) throw new {GetTypeFullName("FormatException")}();");
                            w.WriteLine();

                            foreach (var element in messageFormat.Elements)
                            {
                                this.Write_Small_Formatter_Serialize_PropertyDef(w, "value." + element.Name, element.Type, 0);
                            }
                        }

                        w.WriteLine("}");
                    }

                    w.WriteLine();

                    {
                        w.WriteLine($"public {messageFormat.Name} Deserialize(ref {GetTypeFullName("OmniPackReader")} r, in int rank)");
                        w.WriteLine("{");

                        using (w.Indent())
                        {
                            w.WriteLine($"if (rank > 256) throw new {GetTypeFullName("FormatException")}();");
                            w.WriteLine();

                            foreach (var elementInfo in messageFormat.Elements)
                            {
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GenerateFieldName(elementInfo.Name)} = {this.GetDefaultValueString(elementInfo.Type)};");
                            }
                            w.WriteLine();

                            foreach (var elementInfo in messageFormat.Elements)
                            {
                                w.WriteLine("{");
                                using (w.Indent())
                                {
                                    this.Write_Small_Formatter_Deserialize_PropertyDef(w, "p_" + GenerateFieldName(elementInfo.Name), elementInfo.Type, 0);
                                }
                                w.WriteLine("}");
                            }

                            w.WriteLine($"return new {messageFormat.Name}({ string.Join(", ", messageFormat.Elements.Select(n => "p_" + GenerateFieldName(n.Name)))});");

                        }

                        w.WriteLine("}");
                    }
                }

                w.WriteLine("}");
            }

            private void Write_Small_Formatter_Serialize_PropertyDef(CodeWriter w, string name, TypeBase type, int rank)
            {
                switch (type)
                {
                    case BoolType boolType:
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case IntType inttype:
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case FloatType floatType:
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case StringType stringType:
                        w.WriteLine($"w.Write({name});");
                        break;
                    case TimestampType timestampType:
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case BytesType memoryType when (memoryType.IsUseMemoryPool):
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name}.Span);");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value.Span);");
                        }
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool):
                        if (!type.IsOptional)
                        {
                            w.WriteLine($"w.Write({name}.Span);");
                        }
                        else
                        {
                            w.WriteLine($"w.Write({name}.Value.Span);");
                        }
                        break;
                    case VectorType listType:
                        {
                            w.WriteLine($"w.Write((uint){name}.Count);");
                            w.WriteLine($"foreach (var n in {name})");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Small_Formatter_Serialize_PropertyDef(w, "n", listType.ElementType, rank + 1);
                            }

                            w.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            w.WriteLine($"w.Write((uint){name}.Count);");
                            w.WriteLine($"foreach (var n in {name})");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Small_Formatter_Serialize_PropertyDef(w, "n.Key", mapType.KeyType, rank + 1);
                                this.Write_Small_Formatter_Serialize_PropertyDef(w, "n.Value", mapType.ValueType, rank + 1);
                            }

                            w.WriteLine("}");
                        }
                        break;
                    case CustomType customType:
                        switch (this.CustomTypeResolver(customType))
                        {
                            case EnumDefinition enumDefinition:
                                switch (enumDefinition.Type)
                                {
                                    case IntType intType when (intType.IsSigned):
                                        if (!intType.IsOptional)
                                        {
                                            w.WriteLine($"w.Write((long){name});");
                                        }
                                        else
                                        {
                                            w.WriteLine($"w.Write((long){name}.Value);");
                                        }

                                        break;
                                    case IntType intType when (!intType.IsSigned):
                                        if (!intType.IsOptional)
                                        {
                                            w.WriteLine($"w.Write((ulong){name});");
                                        }
                                        else
                                        {
                                            w.WriteLine($"w.Write((ulong){name}.Value);");
                                        }

                                        break;
                                }
                                break;
                            case ObjectDefinition messageDefinition when (messageDefinition.IsStruct):
                                if (!customType.IsOptional)
                                {
                                    w.WriteLine($"{messageDefinition.Name}.Formatter.Serialize(ref w, {name}, rank + 1);");
                                }
                                else
                                {
                                    w.WriteLine($"{messageDefinition.Name}.Formatter.Serialize(ref w, {name}.Value, rank + 1);");
                                }
                                break;
                            case ObjectDefinition messageDefinition when (messageDefinition.IsClass):
                                w.WriteLine($"{messageDefinition.Name}.Formatter.Serialize(ref w, {name}, rank + 1);");
                                break;
                        }
                        break;
                }
            }

            private void Write_Small_Formatter_Deserialize_PropertyDef(CodeWriter w, string name, TypeBase type, int rank)
            {
                switch (type)
                {
                    case BoolType boolType:
                        w.WriteLine($"{name} = r.GetBoolean();");
                        break;
                    case IntType inttype when (!inttype.IsSigned):
                        w.WriteLine($"{name} = r.GetUInt{inttype.Size}();");
                        break;
                    case IntType inttype when (inttype.IsSigned):
                        w.WriteLine($"{name} = r.GetInt{inttype.Size}();");
                        break;
                    case FloatType floatType when (floatType.Size == 32):
                        w.WriteLine($"{name} = r.GetFloat32();");
                        break;
                    case FloatType floatType when (floatType.Size == 64):
                        w.WriteLine($"{name} = r.GetFloat64();");
                        break;
                    case StringType stringType:
                        w.WriteLine($"{name} = r.GetString({stringType.MaxLength});");
                        break;
                    case TimestampType timestampType:
                        w.WriteLine($"{name} = r.GetTimestamp();");
                        break;
                    case BytesType memoryType when (memoryType.IsUseMemoryPool):
                        w.WriteLine($"{name} = r.GetRecyclableMemory({memoryType.MaxLength});");
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool):
                        w.WriteLine($"{name} = r.GetMemory({memoryType.MaxLength});");
                        break;
                    case VectorType listType:
                        {
                            w.WriteLine("var length = r.GetUInt32();");
                            w.WriteLine($"{name} = new {this.GetParameterTypeString(listType.ElementType)}[length];");

                            w.WriteLine($"for (int i = 0; i < {name}.Length; i++)");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Small_Formatter_Deserialize_PropertyDef(w, $"{name}[i]", listType.ElementType, rank + 1);
                            }

                            w.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            w.WriteLine("var length = r.GetUInt32();");
                            w.WriteLine($"{name} = new {GetTypeFullName("Dictionary<,>", this.GetParameterTypeString(mapType.KeyType), this.GetParameterTypeString(mapType.ValueType))}();");
                            w.WriteLine($"{this.GetParameterTypeString(mapType.KeyType)} t_key = {this.GetDefaultValueString(mapType.KeyType)};");
                            w.WriteLine($"{this.GetParameterTypeString(mapType.ValueType)} t_value = {this.GetDefaultValueString(mapType.ValueType)};");

                            w.WriteLine("for (int i = 0; i < length; i++)");
                            w.WriteLine("{");

                            using (w.Indent())
                            {
                                this.Write_Small_Formatter_Deserialize_PropertyDef(w, "t_key", mapType.KeyType, rank + 1);
                                this.Write_Small_Formatter_Deserialize_PropertyDef(w, "t_value", mapType.ValueType, rank + 1);
                                w.WriteLine($"{name}[t_key] = t_value;");
                            }

                            w.WriteLine("}");
                        }
                        break;
                    case CustomType customType:
                        switch (this.CustomTypeResolver(customType))
                        {
                            case EnumDefinition enumInfo:
                                switch (enumInfo.Type)
                                {
                                    case IntType inttype when (inttype.IsSigned):
                                        w.WriteLine($"{name} = ({enumInfo.Name})r.GetInt64();");
                                        break;
                                    case IntType inttype when (!inttype.IsSigned):
                                        w.WriteLine($"{name} = ({enumInfo.Name})r.GetUInt64();");
                                        break;
                                }
                                break;
                            case ObjectDefinition messageInfo when (messageInfo.IsStruct):
                                w.WriteLine($"{name} = {messageInfo.Name}.Formatter.Deserialize(ref r, rank + 1);");
                                break;
                            case ObjectDefinition messageInfo when (messageInfo.IsClass):
                                w.WriteLine($"{name} = {messageInfo.Name}.Formatter.Deserialize(ref r, rank + 1);");
                                break;
                        }
                        break;
                }
            }

            private void BlockWhoseValueIsNotDefault(CodeWriter w, ObjectElement messageElement, Action callback)
            {
                var sb = new StringBuilder();
                sb.Append($"if (");

                switch (messageElement.Type)
                {
                    case BoolType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{messageElement.Name} != false)");
                        }
                        else
                        {
                            sb.Append($"value.{messageElement.Name} != null)");
                        }
                        break;
                    case IntType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{messageElement.Name} != 0)");
                        }
                        else
                        {
                            sb.Append($"value.{messageElement.Name} != null)");
                        }
                        break;
                    case FloatType type when (type.Size == 32):
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{messageElement.Name} != 0.0F)");
                        }
                        else
                        {
                            sb.Append($"value.{messageElement.Name} != null)");
                        }
                        break;
                    case FloatType type when (type.Size == 64):
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{messageElement.Name} != 0.0D)");
                        }
                        else
                        {
                            sb.Append($"value.{messageElement.Name} != null)");
                        }
                        break;
                    case StringType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{messageElement.Name} != string.Empty)");
                        }
                        else
                        {
                            sb.Append($"value.{messageElement.Name} != null)");
                        }
                        break;
                    case TimestampType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{messageElement.Name} != {GetTypeFullName("Timestamp")}.Zero)");
                        }
                        else
                        {
                            sb.Append($"value.{messageElement.Name} != null)");
                        }
                        break;
                    case BytesType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"!value.{messageElement.Name}.IsEmpty)");
                        }
                        else
                        {
                            sb.Append($"value.{messageElement.Name} != null)");
                        }
                        break;
                    case VectorType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{messageElement.Name}.Count != 0)");
                        }
                        else
                        {
                            sb.Append($"value.{messageElement.Name} != null)");
                        }
                        break;
                    case MapType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{messageElement.Name}.Count != 0)");
                        }
                        else
                        {
                            sb.Append($"value.{messageElement.Name} != null)");
                        }
                        break;
                    case CustomType type:
                        {
                            switch (this.CustomTypeResolver(type))
                            {
                                case EnumDefinition elementEnumDefinition:
                                    if (!type.IsOptional)
                                    {
                                        sb.Append($"value.{messageElement.Name} != ({elementEnumDefinition.Name})0)");
                                    }
                                    else
                                    {
                                        sb.Append($"value.{messageElement.Name} != null)");
                                    }
                                    break;
                                case ObjectDefinition elementMessageDefinition:
                                    if (!type.IsOptional)
                                    {
                                        sb.Append($"value.{messageElement.Name} != {elementMessageDefinition.Name}.Empty)");
                                    }
                                    else
                                    {
                                        sb.Append($"value.{messageElement.Name} != null)");
                                    }
                                    break;
                                default:
                                    throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(messageElement.Type));
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException($"Type \"{messageElement.Type.GetType().Name}\" was not found", nameof(messageElement.Type));
                }

                w.WriteLine(sb.ToString());
                w.WriteLine("{");
                using (w.Indent())
                {
                    callback.Invoke();
                }
                w.WriteLine("}");
            }
        }
    }
}
