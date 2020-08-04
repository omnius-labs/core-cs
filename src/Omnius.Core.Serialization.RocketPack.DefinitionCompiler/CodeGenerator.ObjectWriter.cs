using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnius.Core.Serialization.RocketPack.DefinitionCompiler
{
    internal partial class CodeGenerator
    {
        private sealed class ObjectWriter
        {
            private readonly RocketPackDefinition _rootDefinition;
            private readonly IList<RocketPackDefinition> _externalDefinitions;
            private readonly string _accessLevel;

            private const string CustomFormatterName = "___CustomFormatter";
            private const string HashCodeName = "___hashCode";

            public ObjectWriter(RocketPackDefinition rootDefinition, IEnumerable<RocketPackDefinition> externalDefinitions)
            {
                _rootDefinition = rootDefinition;
                _externalDefinitions = externalDefinitions.ToList();

                var accessLevelOption = _rootDefinition.Options.FirstOrDefault(n => n.Name == "csharp_access_level");
                _accessLevel = accessLevelOption?.Value ?? "public";
            }

            private object? FindDefinition(CustomType customType)
            {
                foreach (var definition in new[] { _rootDefinition }.Union(_externalDefinitions))
                {
                    var enumDefinitiom = definition.Enums.FirstOrDefault(m => m.Name == customType.TypeName);
                    if (enumDefinitiom != null)
                    {
                        return enumDefinitiom;
                    }

                    var objectDefinition = definition.Objects.FirstOrDefault(m => m.Name == customType.TypeName);
                    if (objectDefinition != null)
                    {
                        return objectDefinition;
                    }
                }

                return null;
            }

            /// <summary>
            /// プロパティ名からフィールド変数名を生成します。
            /// </summary>
            private static string GenerateFieldVariableName(string name)
            {
                return name[0].ToString().ToLower() + name.Substring(1);
            }

            private static string GenerateTypeFullName(string name, params string[] types)
            {
                var result = name switch
                {
                    "Lazy<>" => $"System.Lazy<{types[0]}>",
                    "ReadOnlySequence<>" => $"System.Buffers.ReadOnlySequence<{types[0]}>",
                    "IBufferWriter<>" => $"System.Buffers.IBufferWriter<{types[0]}>",
                    "IRocketPackObject<>" => $"Omnius.Core.Serialization.RocketPack.IRocketPackObject<{types[0]}>",
                    "RocketPackReader" => "Omnius.Core.Serialization.RocketPack.RocketPackReader",
                    "RocketPackWriter" => "Omnius.Core.Serialization.RocketPack.RocketPackWriter",
                    "IRocketPackFormatter<>" => $"Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<{types[0]}>",
                    "FormatException" => "System.FormatException",
                    "BytesOperations" => "Omnius.Core.BytesOperations",
                    "CollectionHelper" => "Omnius.Core.Helpers.CollectionHelper",
                    "ObjectHelper" => "Omnius.Core.Helpers.ObjectHelper",
                    "HashCode" => "System.HashCode",
                    "Array" => "System.Array",
                    "Timestamp" => "Omnius.Core.Serialization.RocketPack.Timestamp",
                    "IMemoryOwner<>" => $"System.Buffers.IMemoryOwner<{types[0]}>",
                    "Span<>" => $"System.Span<{types[0]}>",
                    "ReadOnlySpan<>" => $"System.ReadOnlySpan<{types[0]}>",
                    "Memory<>" => $"System.Memory<{types[0]}>",
                    "ReadOnlyMemory<>" => $"System.ReadOnlyMemory<{types[0]}>",
                    "ReadOnlyListSlim<>" => $"Omnius.Core.Collections.ReadOnlyListSlim<{types[0]}>",
                    "ReadOnlyDictionarySlim<,>" => $"Omnius.Core.Collections.ReadOnlyDictionarySlim<{types[0]}, {types[1]}>",
                    "Dictionary<,>" => $"System.Collections.Generic.Dictionary<{types[0]}, {types[1]}>",
                    "RocketPackMessageBase<>" => $"Omnius.Core.Serialization.RocketPack.RocketPackMessageBase<{types[0]}>",
                    "IDisposable" => "System.IDisposable",
                    "IBytesPool" => "Omnius.Core.IBytesPool",
                    "MemoryOwner<>" => $"Omnius.Core.MemoryOwner<{types[0]}>",
                    "ArgumentNullException" => "System.ArgumentNullException",
                    "ArgumentOutOfRangeException" => "System.ArgumentOutOfRangeException",
                    _ => throw new InvalidOperationException(name)
                };

                return "global::" + result;
            }

            private string GenerateParameterTypeFullName(TypeBase typeBase)
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
                    TimestampType type => GenerateTypeFullName("Timestamp") + (type.IsOptional ? "?" : ""),
                    BytesType type when (type.IsUseMemoryPool) => GenerateTypeFullName("IMemoryOwner<>", "byte") + (type.IsOptional ? "?" : ""),
                    BytesType type when (!type.IsUseMemoryPool) => GenerateTypeFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : ""),
                    VectorType type => $"{this.GenerateParameterTypeFullName(type.ElementType)}[]" + (type.IsOptional ? "?" : ""),
                    MapType type => GenerateTypeFullName("Dictionary<,>", this.GenerateParameterTypeFullName(type.KeyType), this.GenerateParameterTypeFullName(type.ValueType)) + (type.IsOptional ? "?" : ""),
                    CustomType type => this.FindDefinition(type) switch
                    {
                        EnumDefinition _ => type.TypeName + (type.IsOptional ? "?" : ""),
                        ObjectDefinition objectDefinition when (objectDefinition.FormatType == MessageFormatType.Table) => type.TypeName + (type.IsOptional ? "?" : ""),
                        ObjectDefinition objectDefinition when (objectDefinition.FormatType == MessageFormatType.Struct) => type.TypeName + (type.IsOptional ? "?" : ""),
                        _ => throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(type)),
                    },
                    _ => throw new ArgumentException($"Type \"{typeBase.GetType().Name}\" was not found", nameof(typeBase)),
                };
            }

            private string GeneratePropertyTypeFullName(TypeBase typeBase)
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
                    TimestampType type => GenerateTypeFullName("Timestamp") + (type.IsOptional ? "?" : ""),
                    BytesType type when (!type.IsUseMemoryPool) => GenerateTypeFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : ""),
                    BytesType type when (type.IsUseMemoryPool) => GenerateTypeFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : ""),
                    VectorType type => GenerateTypeFullName("ReadOnlyListSlim<>", this.GenerateParameterTypeFullName(type.ElementType)) + (type.IsOptional ? "?" : ""),
                    MapType type => GenerateTypeFullName("ReadOnlyDictionarySlim<,>", this.GenerateParameterTypeFullName(type.KeyType), this.GenerateParameterTypeFullName(type.ValueType)) + (type.IsOptional ? "?" : ""),
                    CustomType type => this.FindDefinition(type) switch
                    {
                        EnumDefinition _ => type.TypeName + (type.IsOptional ? "?" : ""),
                        ObjectDefinition objectDefinition when (objectDefinition.FormatType == MessageFormatType.Table) => type.TypeName + (type.IsOptional ? "?" : ""),
                        ObjectDefinition objectDefinition when (objectDefinition.FormatType == MessageFormatType.Struct) => type.TypeName + (type.IsOptional ? "?" : ""),
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
                    TimestampType type => type.IsOptional ? "null" : GenerateTypeFullName("Timestamp") + ".Zero",
                    BytesType type when (!type.IsUseMemoryPool) => type.IsOptional ? "null" : GenerateTypeFullName("ReadOnlyMemory<>", "byte") + ".Empty",
                    BytesType type when (type.IsUseMemoryPool) => type.IsOptional ? "null" : GenerateTypeFullName("MemoryOwner<>", "byte") + ".Empty",
                    VectorType type => type.IsOptional ? "null" : GenerateTypeFullName("Array") + ".Empty<" + this.GenerateParameterTypeFullName(type.ElementType) + ">()",
                    MapType type => type.IsOptional
                            ? "null"
                            : "new " + GenerateTypeFullName("Dictionary<,>", this.GenerateParameterTypeFullName(type.KeyType), this.GenerateParameterTypeFullName(type.ValueType)) + "()",
                    CustomType type => this.FindDefinition(type) switch
                    {
                        EnumDefinition elementEnumDefinition => type.IsOptional ? "null" : $"({elementEnumDefinition.Name})0",
                        ObjectDefinition elementMessageDefinition => type.IsOptional ? "null" : $"{elementMessageDefinition.Name}.Empty",
                        _ => throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(typeBase)),
                    },
                    _ => throw new ArgumentException($"Type \"{typeBase.GetType().Name}\" was not found", nameof(typeBase)),
                };
            }

            public void Write(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                if (objectDefinition.IsStruct)
                {
                    if (objectDefinition.Elements.Select(n => n.Type).OfType<BytesType>().Any(n => n.IsUseMemoryPool))
                    {
                        b.WriteLine($"{_accessLevel} readonly partial struct {objectDefinition.Name} : {GenerateTypeFullName("IRocketPackObject<>", objectDefinition.FullName)}, {GenerateTypeFullName("IDisposable")}");
                    }
                    else
                    {
                        b.WriteLine($"{_accessLevel} readonly partial struct {objectDefinition.Name} : {GenerateTypeFullName("IRocketPackObject<>", objectDefinition.FullName)}");
                    }
                }
                else if (objectDefinition.IsClass)
                {
                    if (objectDefinition.Elements.Select(n => n.Type).OfType<BytesType>().Any(n => n.IsUseMemoryPool))
                    {
                        b.WriteLine($"{_accessLevel} sealed partial class {objectDefinition.Name} : {GenerateTypeFullName("IRocketPackObject<>", objectDefinition.FullName)}, {GenerateTypeFullName("IDisposable")}");
                    }
                    else
                    {
                        b.WriteLine($"{_accessLevel} sealed partial class { objectDefinition.Name} : {GenerateTypeFullName("IRocketPackObject<>", objectDefinition.FullName)}");
                    }
                }

                b.WriteLine("{");

                using (b.Indent())
                {
                    this.Write_StaticConstructor(b, objectDefinition);
                    b.WriteLine();

                    this.Write_Constructor(b, objectDefinition);
                    b.WriteLine();

                    this.Write_Properties(b, objectDefinition);
                    b.WriteLine();

                    this.Write_ImportAndExport(b, objectDefinition);
                    b.WriteLine();

                    this.Write_Equals(b, objectDefinition);
                    b.WriteLine();

                    if (objectDefinition.Elements.Select(n => n.Type).OfType<BytesType>().Any(n => n.IsUseMemoryPool))
                    {
                        this.Write_Dispose(b, objectDefinition);
                        b.WriteLine();
                    }

                    if (objectDefinition.FormatType == MessageFormatType.Table)
                    {
                        this.Write_Medium_Formatter(b, objectDefinition);
                    }
                    else if (objectDefinition.FormatType == MessageFormatType.Struct)
                    {
                        this.Write_Small_Formatter(b, objectDefinition);
                    }
                }

                b.WriteLine("}");
            }

            /// <summary>
            /// 静的コンストラクタの生成。
            /// </summary>
            private void Write_StaticConstructor(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                b.WriteLine($"public static {GenerateTypeFullName("IRocketPackFormatter<>", objectDefinition.FullName)} Formatter => {GenerateTypeFullName("IRocketPackObject<>", objectDefinition.FullName)}.Formatter;");
                b.WriteLine($"public static {objectDefinition.FullName} Empty => {GenerateTypeFullName("IRocketPackObject<>", objectDefinition.FullName)}.Empty;");
                b.WriteLine();

                b.WriteLine($"static {objectDefinition.Name}()");
                b.WriteLine("{");

                using (b.Indent())
                {
                    // CustomFormatterのインスタンスの作成
                    this.Write_StaticConstructor_CustomFormatterProperty(b, objectDefinition);

                    // Emptyのインスタンスの作成
                    this.Write_StaticConstructor_EmptyProperty(b, objectDefinition);
                }

                b.WriteLine("}");
            }

            private void Write_StaticConstructor_CustomFormatterProperty(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                b.WriteLine($"{GenerateTypeFullName("IRocketPackObject<>", objectDefinition.FullName)}.Formatter = new {CustomFormatterName}();");
            }

            private void Write_StaticConstructor_EmptyProperty(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                var parameters = new List<string>();

                foreach (var element in objectDefinition.Elements)
                {
                    parameters.Add(this.GetDefaultValueString(element.Type));
                }

                b.WriteLine($"{GenerateTypeFullName("IRocketPackObject<>", objectDefinition.FullName)}.Empty = new {objectDefinition.FullName}({string.Join(", ", parameters)});");
            }

            /// <summary>
            /// コンストラクタの生成。
            /// </summary>
            private void Write_Constructor(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                if (objectDefinition.IsStruct)
                {
                    b.WriteLine($"private readonly int {HashCodeName};");
                }
                else if (objectDefinition.IsClass)
                {
                    b.WriteLine($"private readonly {GenerateTypeFullName("Lazy<>", "int")} {HashCodeName};");
                }
                b.WriteLine();

                // 最大サイズの宣言。
                this.Write_Constructor_Define_MaxLength(b, objectDefinition);

                // パラメータの生成。
                b.WriteLine($"public {objectDefinition.Name}({string.Join(", ", objectDefinition.Elements.Select(element => this.GenerateParameterTypeFullName(element.Type) + " " + GenerateFieldVariableName(element.Name)))})");
                b.WriteLine("{");

                using (b.Indent())
                {
                    // パラメータのチェック
                    this.Write_Constructor_Parameter_Check(b, objectDefinition);

                    // 初期化
                    this.Write_Constructor_Init(b, objectDefinition);

                    // HashCodeの値の算出
                    this.Write_Constructor_HashCode(b, objectDefinition);
                }

                b.WriteLine("}");
            }

            private void Write_Constructor_Init(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                foreach (var element in objectDefinition.Elements)
                {
                    switch (element.Type)
                    {
                        case BytesType type when (type.IsUseMemoryPool):
                            b.WriteLine($"_{GenerateFieldVariableName(element.Name)} = {GenerateFieldVariableName(element.Name)};");
                            break;
                        case VectorType type:
                            if (type.IsOptional)
                            {
                                b.WriteLine($"if ({GenerateFieldVariableName(element.Name)} != null)");
                                b.WriteLine("{");
                                b.PushIndent();
                            }

                            b.WriteLine($"this.{element.Name} = new {GenerateTypeFullName("ReadOnlyListSlim<>", this.GenerateParameterTypeFullName(type.ElementType))}({GenerateFieldVariableName(element.Name)});");

                            if (type.IsOptional)
                            {
                                b.PopIndent();

                                b.WriteLine("}");
                                b.WriteLine("else");
                                b.WriteLine("{");

                                using (b.Indent())
                                {
                                    b.WriteLine($"this.{element.Name} = null;");
                                }

                                b.WriteLine("}");
                            }
                            break;
                        case MapType type:
                            if (type.IsOptional)
                            {
                                b.WriteLine($"if({GenerateFieldVariableName(element.Name)} != null)");
                                b.WriteLine("{");
                                b.PushIndent();
                            }

                            b.WriteLine($"this.{element.Name} = new {GenerateTypeFullName("ReadOnlyDictionarySlim<,>", this.GenerateParameterTypeFullName(type.KeyType), this.GenerateParameterTypeFullName(type.ValueType))}({GenerateFieldVariableName(element.Name)});");

                            if (type.IsOptional)
                            {
                                b.PopIndent();

                                b.WriteLine("}");
                                b.WriteLine("else");
                                b.WriteLine("{");

                                using (b.Indent())
                                {
                                    b.WriteLine($"this.{element.Name} = null;");
                                }

                                b.WriteLine("}");
                            }
                            break;
                        default:
                            b.WriteLine($"this.{element.Name} = {GenerateFieldVariableName(element.Name)};");
                            break;
                    }
                }

                b.WriteLine();
            }

            private void Write_Constructor_Define_MaxLength(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                bool isDefinedMaxLength = false;

                foreach (var element in objectDefinition.Elements)
                {
                    isDefinedMaxLength |= this.Try_Write_Constructor_Define_MaxLength_Element(b, element.Name, element.Type);
                }

                if (isDefinedMaxLength)
                {
                    b.WriteLine();
                }
            }

            private bool Try_Write_Constructor_Define_MaxLength_Element(CodeBuilder b, string name, TypeBase type)
            {
                switch (type)
                {
                    case StringType stringType:
                        b.WriteLine($"public static readonly int Max{name}Length = {stringType.MaxLength};");
                        return true;
                    case BytesType memoryType:
                        b.WriteLine($"public static readonly int Max{name}Length = {memoryType.MaxLength};");
                        return true;
                    case VectorType listType:
                        b.WriteLine($"public static readonly int Max{name}Count = {listType.MaxLength};");
                        return true;
                    case MapType mapType:
                        b.WriteLine($"public static readonly int Max{name}Count = {mapType.MaxLength};");
                        return true;
                    default:
                        return false;
                }
            }

            private void Write_Constructor_Parameter_Check(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                bool isChecked = false;

                foreach (var element in objectDefinition.Elements)
                {
                    isChecked = this.Try_Write_Constructor_Parameter_Check_Element(b, element);
                }

                if (isChecked)
                {
                    b.WriteLine();
                }
            }

            private bool Try_Write_Constructor_Parameter_Check_Element(CodeBuilder b, ObjectElement element)
            {
                bool isChecked = false;

                isChecked |= this.Try_Write_Constructor_Parameter_Check_Element_Null(b, GenerateFieldVariableName(element.Name), element.Type);
                isChecked |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(b, GenerateFieldVariableName(element.Name), element.Type);

                if (element.Type is VectorType listType)
                {
                    var b2 = new CodeBuilder();
                    bool isChecked2 = false;

                    if (listType.IsOptional)
                    {
                        b2.WriteLine($"if (!({GenerateFieldVariableName(element.Name)} is null))");
                        b2.WriteLine("{");

                        b2.PushIndent();
                    }

                    b2.WriteLine($"foreach (var n in {GenerateFieldVariableName(element.Name)})");
                    b2.WriteLine("{");

                    using (b2.Indent())
                    {
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_Null(b2, "n", listType.ElementType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(b2, "n", listType.ElementType);
                    }

                    b2.WriteLine("}");

                    if (listType.IsOptional)
                    {
                        b2.PopIndent();

                        b2.WriteLine("}");
                    }

                    if (isChecked2)
                    {
                        b.WriteLine(b2.ToString().TrimEnd());
                    }

                    isChecked |= isChecked2;
                }
                else if (element.Type is MapType mapType)
                {
                    var b2 = new CodeBuilder();
                    bool isChecked2 = false;

                    if (mapType.IsOptional)
                    {
                        b2.WriteLine($"if (!({GenerateFieldVariableName(element.Name)} is null))");
                        b2.WriteLine("{");

                        b2.PushIndent();
                    }

                    b2.WriteLine($"foreach (var n in {GenerateFieldVariableName(element.Name)})");
                    b2.WriteLine("{");

                    using (b2.Indent())
                    {
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_Null(b2, "n.Key", mapType.KeyType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_Null(b2, "n.Value", mapType.ValueType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(b2, "n.Key", mapType.KeyType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(b2, "n.Value", mapType.ValueType);
                    }

                    b2.WriteLine("}");

                    if (mapType.IsOptional)
                    {
                        b2.PopIndent();

                        b2.WriteLine("}");
                    }

                    if (isChecked2)
                    {
                        b.WriteLine(b2.ToString().TrimEnd());
                    }

                    isChecked |= isChecked2;
                }

                return isChecked;
            }

            private bool Try_Write_Constructor_Parameter_Check_Element_Null(CodeBuilder b, string name, TypeBase type)
            {
                switch (type)
                {
                    case StringType stringType when (!type.IsOptional):
                        b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case BytesType memoryType when (!type.IsOptional && memoryType.IsUseMemoryPool):
                        b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case VectorType listType when (!type.IsOptional):
                        b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case MapType mapType when (!type.IsOptional):
                        b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case CustomType customType when (!type.IsOptional):
                        switch (this.FindDefinition(customType))
                        {
                            case ObjectDefinition objectDefinition when (objectDefinition.IsClass):
                                b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                                return true;
                            default:
                                return false;
                        }
                    default:
                        return false;
                }
            }

            private bool Try_Write_Constructor_Parameter_Check_Element_MaxLength(CodeBuilder b, string name, TypeBase type)
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
                    b.WriteLine($"if (!({name} is null) && {name}.{property} > {maxLength}) throw new {GenerateTypeFullName("ArgumentOutOfRangeException")}(\"{name}\");");
                }
                else
                {
                    b.WriteLine($"if ({name}.{property} > {maxLength}) throw new {GenerateTypeFullName("ArgumentOutOfRangeException")}(\"{name}\");");
                }

                return true;
            }

            private void Write_Constructor_HashCode(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                const string TempVariableName = "___h";

                if (objectDefinition.IsStruct)
                {
                    b.WriteLine("{");

                    using (b.Indent())
                    {
                        b.WriteLine($"var {TempVariableName} = new {GenerateTypeFullName("HashCode")}();");

                        foreach (var elementInfo in objectDefinition.Elements)
                        {
                            this.Write_Constructor_HashCode_Element(b, TempVariableName, GenerateFieldVariableName(elementInfo.Name), elementInfo.Type);
                        }

                        b.WriteLine($"{HashCodeName} = {TempVariableName}.ToHashCode();");
                    }

                    b.WriteLine("}");
                }
                else
                {
                    b.WriteLine($"{HashCodeName} = new {GenerateTypeFullName("Lazy<>", "int")}(() =>");
                    b.WriteLine("{");

                    using (b.Indent())
                    {
                        b.WriteLine($"var {TempVariableName} = new {GenerateTypeFullName("HashCode")}();");

                        foreach (var elementInfo in objectDefinition.Elements)
                        {
                            this.Write_Constructor_HashCode_Element(b, TempVariableName, GenerateFieldVariableName(elementInfo.Name), elementInfo.Type);
                        }

                        b.WriteLine($"return {TempVariableName}.ToHashCode();");
                    }

                    b.WriteLine("});");
                }
            }

            private void Write_Constructor_HashCode_Element(CodeBuilder b, string hashCodeName, string parameterName, TypeBase type)
            {
                switch (type)
                {
                    case BoolType boolType:
                        b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case IntType intType:
                        b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case FloatType floatType:
                        b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case StringType stringType:
                        b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case TimestampType timestampType:
                        b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                        break;
                    case BytesType memoryType when (!memoryType.IsOptional):
                        if (!memoryType.IsUseMemoryPool)
                        {
                            b.WriteLine($"if (!{parameterName}.IsEmpty) {hashCodeName}.Add({GenerateTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Span));");
                        }
                        else
                        {
                            b.WriteLine($"if (!{parameterName}.Memory.IsEmpty) {hashCodeName}.Add({GenerateTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Memory.Span));");
                        }
                        break;
                    case BytesType memoryType when (memoryType.IsOptional):
                        if (!memoryType.IsUseMemoryPool)
                        {
                            b.WriteLine($"if (!({parameterName} is null) && !{parameterName}.Value.IsEmpty) {hashCodeName}.Add({GenerateTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Value.Span));");
                        }
                        else
                        {
                            b.WriteLine($"if (!({parameterName} is null) && !{parameterName}.Memory.IsEmpty) {hashCodeName}.Add({GenerateTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Memory.Span));");
                        }
                        break;
                    case VectorType listType:
                        {
                            if (type.IsOptional)
                            {
                                b.WriteLine($"if({GenerateFieldVariableName(parameterName)} != null)");
                                b.WriteLine("{");
                                b.PushIndent();
                            }

                            b.WriteLine($"foreach (var n in {GenerateFieldVariableName(parameterName)})");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Constructor_HashCode_Element(b, hashCodeName, "n", listType.ElementType);
                            }

                            b.WriteLine("}");

                            if (type.IsOptional)
                            {
                                b.PopIndent();
                                b.WriteLine("}");
                            }
                        }
                        break;
                    case MapType mapType:
                        {
                            if (type.IsOptional)
                            {
                                b.WriteLine($"if({GenerateFieldVariableName(parameterName)} != null)");
                                b.WriteLine("{");
                                b.PushIndent();
                            }

                            b.WriteLine($"foreach (var n in {GenerateFieldVariableName(parameterName)})");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Constructor_HashCode_Element(b, hashCodeName, "n.Key", mapType.KeyType);
                                this.Write_Constructor_HashCode_Element(b, hashCodeName, "n.Value", mapType.ValueType);
                            }

                            b.WriteLine("}");

                            if (type.IsOptional)
                            {
                                b.PopIndent();
                                b.WriteLine("}");
                            }
                        }
                        break;
                    case CustomType customType:
                        {
                            switch (this.FindDefinition(customType))
                            {
                                case EnumDefinition enumInfo:
                                    b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                                    break;
                                case ObjectDefinition objectDefinition when (objectDefinition.IsStruct):
                                    if (!customType.IsOptional)
                                    {
                                        b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                                    }
                                    else
                                    {
                                        b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.Value.GetHashCode());");
                                    }
                                    break;
                                case ObjectDefinition objectDefinition when (objectDefinition.IsClass):
                                    b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                                    break;
                            }
                        }
                        break;
                }
            }

            private void Write_ImportAndExport(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                b.WriteLine($"public static {objectDefinition.FullName} Import({GenerateTypeFullName("ReadOnlySequence<>", "byte")} sequence, {GenerateTypeFullName("IBytesPool")} bytesPool)");
                b.WriteLine("{");
                using (b.Indent())
                {
                    b.WriteLine($"var reader = new {GenerateTypeFullName("RocketPackReader")}(sequence, bytesPool);");
                    b.WriteLine($"return Formatter.Deserialize(ref reader, 0);");
                }
                b.WriteLine("}");

                b.WriteLine($"public void Export({GenerateTypeFullName("IBufferWriter<>", "byte")} bufferWriter, {GenerateTypeFullName("IBytesPool")} bytesPool)");
                b.WriteLine("{");
                using (b.Indent())
                {
                    b.WriteLine($"var writer = new {GenerateTypeFullName("RocketPackWriter")}(bufferWriter, bytesPool);");
                    b.WriteLine($"Formatter.Serialize(ref writer, this, 0);");
                }
                b.WriteLine("}");
            }

            private void Write_Equals(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                if (objectDefinition.IsStruct)
                {
                    b.WriteLine($"public static bool operator ==({objectDefinition.FullName} left, {objectDefinition.FullName} right)");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("return right.Equals(left);");
                    }
                    b.WriteLine("}");

                    b.WriteLine($"public static bool operator !=({objectDefinition.FullName} left, {objectDefinition.FullName} right)");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("return !(left == right);");
                    }
                    b.WriteLine("}");
                }
                else if (objectDefinition.IsClass)
                {
                    b.WriteLine($"public static bool operator ==({objectDefinition.FullName}? left, {objectDefinition.FullName}? right)");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("return (right is null) ? (left is null) : right.Equals(left);");
                    }
                    b.WriteLine("}");

                    b.WriteLine($"public static bool operator !=({objectDefinition.FullName}? left, {objectDefinition.FullName}? right)");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("return !(left == right);");
                    }
                    b.WriteLine("}");
                }

                b.WriteLine("public override bool Equals(object? other)");
                b.WriteLine("{");
                using (b.Indent())
                {
                    b.WriteLine($"if (!(other is {objectDefinition.FullName})) return false;");
                    b.WriteLine($"return this.Equals(({objectDefinition.FullName})other);");
                }
                b.WriteLine("}");

                if (objectDefinition.IsStruct)
                {
                    b.WriteLine($"public bool Equals({objectDefinition.FullName} target)");
                }
                else if (objectDefinition.IsClass)
                {
                    b.WriteLine($"public bool Equals({objectDefinition.FullName}? target)");
                }
                b.WriteLine("{");

                using (b.Indent())
                {
                    if (objectDefinition.IsClass)
                    {
                        b.WriteLine("if (target is null) return false;");
                        b.WriteLine("if (object.ReferenceEquals(this, target)) return true;");
                    }

                    foreach (var element in objectDefinition.Elements)
                    {
                        switch (element.Type)
                        {
                            case BoolType type:
                                b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case IntType type:
                                b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case FloatType type when (type.Size == 32):
                                b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case StringType type:
                                b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case TimestampType type:
                                b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                break;
                            case BytesType type:
                                if (!type.IsOptional)
                                {
                                    b.WriteLine($"if (!{GenerateTypeFullName("BytesOperations")}.Equals(this.{element.Name}.Span, target.{element.Name}.Span)) return false;");
                                }
                                else
                                {
                                    b.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                    b.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && !{GenerateTypeFullName("BytesOperations")}.Equals(this.{element.Name}.Value.Span, target.{element.Name}.Value.Span)) return false;");
                                }
                                break;
                            case VectorType type:
                                if (!type.IsOptional)
                                {
                                    b.WriteLine($"if (!{GenerateTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                else
                                {
                                    b.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                    b.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && !{GenerateTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                break;
                            case MapType type:
                                if (!type.IsOptional)
                                {
                                    b.WriteLine($"if (!{GenerateTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                else
                                {
                                    b.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                    b.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && !{GenerateTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                break;
                            case CustomType type:
                                {
                                    switch (this.FindDefinition(type))
                                    {
                                        case EnumDefinition enumInfo:
                                            b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                            break;
                                        case ObjectDefinition objectDefinition2 when (objectDefinition2.IsStruct):
                                            if (!type.IsOptional)
                                            {
                                                b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                            }
                                            else
                                            {
                                                b.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                                b.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && this.{element.Name} != target.{element.Name}) return false;");
                                            }
                                            break;
                                        case ObjectDefinition objectDefinition2 when (objectDefinition2.IsClass):
                                            if (!type.IsOptional)
                                            {
                                                b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                            }
                                            else
                                            {
                                                b.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                                b.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && this.{element.Name} != target.{element.Name}) return false;");
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                    b.WriteLine();

                    b.WriteLine("return true;");
                }


                b.WriteLine("}");

                if (objectDefinition.IsStruct)
                {
                    b.WriteLine($"public override int GetHashCode() => {HashCodeName};");
                }
                else if (objectDefinition.IsClass)
                {
                    b.WriteLine($"public override int GetHashCode() => {HashCodeName}.Value;");
                }
            }

            private void Write_Properties(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                foreach (var element in objectDefinition.Elements.OrderBy(n => n.Id))
                {
                    switch (element.Type)
                    {
                        case BytesType type when (type.IsUseMemoryPool):
                            if (!type.IsOptional)
                            {
                                b.WriteLine($"private readonly {this.GenerateParameterTypeFullName(element.Type)} _{GenerateFieldVariableName(element.Name)};");
                                b.WriteLine($"public {this.GeneratePropertyTypeFullName(element.Type)} {element.Name} => _{GenerateFieldVariableName(element.Name)}.Memory;");
                            }
                            else
                            {
                                b.WriteLine($"private readonly {this.GenerateParameterTypeFullName(element.Type)} _{GenerateFieldVariableName(element.Name)};");
                                b.WriteLine($"public {this.GeneratePropertyTypeFullName(element.Type)} {element.Name} => _{GenerateFieldVariableName(element.Name)}?.Memory;");
                            }
                            break;
                        default:
                            b.WriteLine($"public {this.GeneratePropertyTypeFullName(element.Type)} {element.Name} {{ get; }}");
                            break;
                    }
                }
            }

            private void Write_Dispose(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                b.WriteLine("public void Dispose()");
                b.WriteLine("{");

                using (b.Indent())
                {
                    foreach (var element in objectDefinition.Elements)
                    {
                        switch (element.Type)
                        {
                            case BytesType type when (type.IsUseMemoryPool):
                                b.WriteLine($"_{GenerateFieldVariableName(element.Name)}?.Dispose();");
                                break;
                        }
                    }
                }

                b.WriteLine("}");
            }

            private void Write_Medium_Formatter(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                b.WriteLine($"private sealed class ___CustomFormatter : {GenerateTypeFullName("IRocketPackFormatter<>", objectDefinition.FullName)}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    {
                        b.WriteLine($"public void Serialize(ref {GenerateTypeFullName("RocketPackWriter")} w, in {objectDefinition.FullName} value, in int rank)");
                        b.WriteLine("{");

                        using (b.Indent())
                        {
                            b.WriteLine($"if (rank > 256) throw new {GenerateTypeFullName("FormatException")}();");
                            b.WriteLine();

                            {
                                b.WriteLine("{");

                                using (b.Indent())
                                {
                                    b.WriteLine("uint propertyCount = 0;");

                                    foreach (var element in objectDefinition.Elements)
                                    {
                                        this.BlockWhoseValueIsNotDefault(b, element, () =>
                                        {
                                            b.WriteLine("propertyCount++;");
                                        });
                                    }

                                    b.WriteLine($"w.Write(propertyCount);");

                                }

                                b.WriteLine("}");
                            }

                            b.WriteLine();

                            foreach (var element in objectDefinition.Elements)
                            {
                                this.BlockWhoseValueIsNotDefault(b, element, () =>
                                {
                                    b.WriteLine($"w.Write((uint){element.Id});");
                                    this.Write_Medium_Formatter_Serialize_PropertyDef(b, "value." + element.Name, element.Type, 0);
                                });
                            }

                        }

                        b.WriteLine("}");
                    }

                    b.WriteLine();

                    {
                        b.WriteLine($"public {objectDefinition.FullName} Deserialize(ref {GenerateTypeFullName("RocketPackReader")} r, in int rank)");
                        b.WriteLine("{");

                        using (b.Indent())
                        {

                            b.WriteLine($"if (rank > 256) throw new {GenerateTypeFullName("FormatException")}();");
                            b.WriteLine();

                            b.WriteLine("uint propertyCount = r.GetUInt32();");
                            b.WriteLine();

                            foreach (var elementInfo in objectDefinition.Elements)
                            {
                                b.WriteLine($"{this.GenerateParameterTypeFullName(elementInfo.Type)} p_{GenerateFieldVariableName(elementInfo.Name)} = {this.GetDefaultValueString(elementInfo.Type)};");
                            }
                            b.WriteLine();

                            b.WriteLine("for (; propertyCount > 0; propertyCount--)");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                b.WriteLine("uint id = r.GetUInt32();");
                                b.WriteLine("switch (id)");
                                b.WriteLine("{");

                                using (b.Indent())
                                {
                                    foreach (var elementInfo in objectDefinition.Elements)
                                    {
                                        b.WriteLine($"case {elementInfo.Id}:");

                                        using (b.Indent())
                                        {
                                            b.WriteLine("{");

                                            using (b.Indent())
                                            {
                                                this.Write_Medium_Formatter_Deserialize_PropertyDef(b, "p_" + GenerateFieldVariableName(elementInfo.Name), elementInfo.Type, 0);

                                                b.WriteLine("break;");
                                            }

                                            b.WriteLine("}");
                                        }
                                    }
                                }

                                b.WriteLine("}");
                            }

                            b.WriteLine("}");
                            b.WriteLine();

                            b.WriteLine($"return new {objectDefinition.FullName}({ string.Join(", ", objectDefinition.Elements.Select(n => "p_" + GenerateFieldVariableName(n.Name)))});");
                        }

                        b.WriteLine("}");
                    }
                }

                b.WriteLine("}");
            }

            private void Write_Medium_Formatter_Serialize_PropertyDef(CodeBuilder b, string name, TypeBase type, int rank)
            {
                switch (type)
                {
                    case BoolType boolType:
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case IntType inttype:
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case FloatType floatType:
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case StringType stringType:
                        b.WriteLine($"w.Write({name});");
                        break;
                    case TimestampType timestampType:
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case BytesType memoryType when (memoryType.IsUseMemoryPool):
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name}.Span);");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value.Span);");
                        }
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool):
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name}.Span);");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value.Span);");
                        }
                        break;
                    case VectorType listType:
                        {
                            b.WriteLine($"w.Write((uint){name}.Count);");
                            b.WriteLine($"foreach (var n in {name})");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Medium_Formatter_Serialize_PropertyDef(b, "n", listType.ElementType, rank + 1);
                            }

                            b.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            b.WriteLine($"w.Write((uint){name}.Count);");
                            b.WriteLine($"foreach (var n in {name})");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Medium_Formatter_Serialize_PropertyDef(b, "n.Key", mapType.KeyType, rank + 1);
                                this.Write_Medium_Formatter_Serialize_PropertyDef(b, "n.Value", mapType.ValueType, rank + 1);
                            }

                            b.WriteLine("}");
                        }
                        break;
                    case CustomType customType:
                        switch (this.FindDefinition(customType))
                        {
                            case EnumDefinition enumDefinition:
                                switch (enumDefinition.Type)
                                {
                                    case IntType intType when (intType.IsSigned):
                                        if (!intType.IsOptional)
                                        {
                                            b.WriteLine($"w.Write((long){name});");
                                        }
                                        else
                                        {
                                            b.WriteLine($"w.Write((long){name}.Value);");
                                        }

                                        break;
                                    case IntType intType when (!intType.IsSigned):
                                        if (!intType.IsOptional)
                                        {
                                            b.WriteLine($"w.Write((ulong){name});");
                                        }
                                        else
                                        {
                                            b.WriteLine($"w.Write((ulong){name}.Value);");
                                        }

                                        break;
                                }
                                break;
                            case ObjectDefinition objectDefinition when (objectDefinition.IsStruct):
                                if (!type.IsOptional)
                                {
                                    b.WriteLine($"{objectDefinition.FullName}.Formatter.Serialize(ref w, {name}, rank + 1);");
                                }
                                else
                                {
                                    b.WriteLine($"{objectDefinition.FullName}.Formatter.Serialize(ref w, {name}.Value, rank + 1);");
                                }
                                break;
                            case ObjectDefinition objectDefinition when (objectDefinition.IsClass):
                                b.WriteLine($"{objectDefinition.FullName}.Formatter.Serialize(ref w, {name}, rank + 1);");
                                break;
                        }
                        break;
                }
            }

            private void Write_Medium_Formatter_Deserialize_PropertyDef(CodeBuilder b, string name, TypeBase type, int rank)
            {
                switch (type)
                {
                    case BoolType boolType:
                        b.WriteLine($"{name} = r.GetBoolean();");
                        break;
                    case IntType inttype when (!inttype.IsSigned):
                        b.WriteLine($"{name} = r.GetUInt{inttype.Size}();");
                        break;
                    case IntType inttype when (inttype.IsSigned):
                        b.WriteLine($"{name} = r.GetInt{inttype.Size}();");
                        break;
                    case FloatType floatType when (floatType.Size == 32):
                        b.WriteLine($"{name} = r.GetFloat32();");
                        break;
                    case FloatType floatType when (floatType.Size == 64):
                        b.WriteLine($"{name} = r.GetFloat64();");
                        break;
                    case StringType stringType:
                        b.WriteLine($"{name} = r.GetString({stringType.MaxLength});");
                        break;
                    case TimestampType timestampType:
                        b.WriteLine($"{name} = r.GetTimestamp();");
                        break;
                    case BytesType memoryType when (memoryType.IsUseMemoryPool):
                        b.WriteLine($"{name} = r.GetRecyclableMemory({memoryType.MaxLength});");
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool):
                        b.WriteLine($"{name} = r.GetMemory({memoryType.MaxLength});");
                        break;
                    case VectorType listType:
                        {
                            b.WriteLine("var length = r.GetUInt32();");
                            b.WriteLine($"{name} = new {this.GenerateParameterTypeFullName(listType.ElementType)}[length];");

                            b.WriteLine($"for (int i = 0; i < {name}.Length; i++)");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Medium_Formatter_Deserialize_PropertyDef(b, $"{name}[i]", listType.ElementType, rank + 1);
                            }

                            b.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            b.WriteLine("var length = r.GetUInt32();");
                            b.WriteLine($"{name} = new {GenerateTypeFullName("Dictionary<,>", this.GenerateParameterTypeFullName(mapType.KeyType), this.GenerateParameterTypeFullName(mapType.ValueType))}();");
                            b.WriteLine($"{this.GenerateParameterTypeFullName(mapType.KeyType)} t_key = {this.GetDefaultValueString(mapType.KeyType)};");
                            b.WriteLine($"{this.GenerateParameterTypeFullName(mapType.ValueType)} t_value = {this.GetDefaultValueString(mapType.ValueType)};");

                            b.WriteLine("for (int i = 0; i < length; i++)");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Medium_Formatter_Deserialize_PropertyDef(b, "t_key", mapType.KeyType, rank + 1);
                                this.Write_Medium_Formatter_Deserialize_PropertyDef(b, "t_value", mapType.ValueType, rank + 1);
                                b.WriteLine($"{name}[t_key] = t_value;");
                            }

                            b.WriteLine("}");
                        }
                        break;
                    case CustomType customType:
                        switch (this.FindDefinition(customType))
                        {
                            case EnumDefinition enumInfo:
                                switch (enumInfo.Type)
                                {
                                    case IntType inttype when (inttype.IsSigned):
                                        b.WriteLine($"{name} = ({enumInfo.Name})r.GetInt64();");
                                        break;
                                    case IntType inttype when (!inttype.IsSigned):
                                        b.WriteLine($"{name} = ({enumInfo.Name})r.GetUInt64();");
                                        break;
                                }
                                break;
                            case ObjectDefinition objectDefinition when (objectDefinition.IsStruct):
                                b.WriteLine($"{name} = {objectDefinition.FullName}.Formatter.Deserialize(ref r, rank + 1);");
                                break;
                            case ObjectDefinition objectDefinition when (objectDefinition.IsClass):
                                b.WriteLine($"{name} = {objectDefinition.FullName}.Formatter.Deserialize(ref r, rank + 1);");
                                break;
                        }
                        break;
                }
            }

            private void Write_Small_Formatter(CodeBuilder b, ObjectDefinition objectDefinition)
            {
                b.WriteLine($"private sealed class ___CustomFormatter : {GenerateTypeFullName("IRocketPackFormatter<>", objectDefinition.FullName)}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    {
                        b.WriteLine($"public void Serialize(ref {GenerateTypeFullName("RocketPackWriter")} w, in {objectDefinition.FullName} value, in int rank)");
                        b.WriteLine("{");

                        using (b.Indent())
                        {
                            b.WriteLine($"if (rank > 256) throw new {GenerateTypeFullName("FormatException")}();");
                            b.WriteLine();

                            foreach (var element in objectDefinition.Elements)
                            {
                                this.Write_Small_Formatter_Serialize_PropertyDef(b, "value." + element.Name, element.Type, 0);
                            }
                        }

                        b.WriteLine("}");
                    }

                    b.WriteLine();

                    {
                        b.WriteLine($"public {objectDefinition.FullName} Deserialize(ref {GenerateTypeFullName("RocketPackReader")} r, in int rank)");
                        b.WriteLine("{");

                        using (b.Indent())
                        {
                            b.WriteLine($"if (rank > 256) throw new {GenerateTypeFullName("FormatException")}();");
                            b.WriteLine();

                            foreach (var elementInfo in objectDefinition.Elements)
                            {
                                b.WriteLine($"{this.GenerateParameterTypeFullName(elementInfo.Type)} p_{GenerateFieldVariableName(elementInfo.Name)} = {this.GetDefaultValueString(elementInfo.Type)};");
                            }
                            b.WriteLine();

                            foreach (var elementInfo in objectDefinition.Elements)
                            {
                                b.WriteLine("{");
                                using (b.Indent())
                                {
                                    this.Write_Small_Formatter_Deserialize_PropertyDef(b, "p_" + GenerateFieldVariableName(elementInfo.Name), elementInfo.Type, 0);
                                }
                                b.WriteLine("}");
                            }

                            b.WriteLine($"return new {objectDefinition.FullName}({ string.Join(", ", objectDefinition.Elements.Select(n => "p_" + GenerateFieldVariableName(n.Name)))});");

                        }

                        b.WriteLine("}");
                    }
                }

                b.WriteLine("}");
            }

            private void Write_Small_Formatter_Serialize_PropertyDef(CodeBuilder b, string name, TypeBase type, int rank)
            {
                switch (type)
                {
                    case BoolType boolType:
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case IntType inttype:
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case FloatType floatType:
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case StringType stringType:
                        b.WriteLine($"w.Write({name});");
                        break;
                    case TimestampType timestampType:
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name});");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value);");
                        }
                        break;
                    case BytesType memoryType when (memoryType.IsUseMemoryPool):
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name}.Span);");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value.Span);");
                        }
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool):
                        if (!type.IsOptional)
                        {
                            b.WriteLine($"w.Write({name}.Span);");
                        }
                        else
                        {
                            b.WriteLine($"w.Write({name}.Value.Span);");
                        }
                        break;
                    case VectorType listType:
                        {
                            b.WriteLine($"w.Write((uint){name}.Count);");
                            b.WriteLine($"foreach (var n in {name})");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Small_Formatter_Serialize_PropertyDef(b, "n", listType.ElementType, rank + 1);
                            }

                            b.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            b.WriteLine($"w.Write((uint){name}.Count);");
                            b.WriteLine($"foreach (var n in {name})");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Small_Formatter_Serialize_PropertyDef(b, "n.Key", mapType.KeyType, rank + 1);
                                this.Write_Small_Formatter_Serialize_PropertyDef(b, "n.Value", mapType.ValueType, rank + 1);
                            }

                            b.WriteLine("}");
                        }
                        break;
                    case CustomType customType:
                        switch (this.FindDefinition(customType))
                        {
                            case EnumDefinition enumDefinition:
                                switch (enumDefinition.Type)
                                {
                                    case IntType intType when (intType.IsSigned):
                                        if (!intType.IsOptional)
                                        {
                                            b.WriteLine($"w.Write((long){name});");
                                        }
                                        else
                                        {
                                            b.WriteLine($"w.Write((long){name}.Value);");
                                        }

                                        break;
                                    case IntType intType when (!intType.IsSigned):
                                        if (!intType.IsOptional)
                                        {
                                            b.WriteLine($"w.Write((ulong){name});");
                                        }
                                        else
                                        {
                                            b.WriteLine($"w.Write((ulong){name}.Value);");
                                        }

                                        break;
                                }
                                break;
                            case ObjectDefinition objectDefinition when (objectDefinition.IsStruct):
                                if (!customType.IsOptional)
                                {
                                    b.WriteLine($"{objectDefinition.FullName}.Formatter.Serialize(ref w, {name}, rank + 1);");
                                }
                                else
                                {
                                    b.WriteLine($"{objectDefinition.FullName}.Formatter.Serialize(ref w, {name}.Value, rank + 1);");
                                }
                                break;
                            case ObjectDefinition objectDefinition when (objectDefinition.IsClass):
                                b.WriteLine($"{objectDefinition.FullName}.Formatter.Serialize(ref w, {name}, rank + 1);");
                                break;
                        }
                        break;
                }
            }

            private void Write_Small_Formatter_Deserialize_PropertyDef(CodeBuilder b, string name, TypeBase type, int rank)
            {
                switch (type)
                {
                    case BoolType boolType:
                        b.WriteLine($"{name} = r.GetBoolean();");
                        break;
                    case IntType inttype when (!inttype.IsSigned):
                        b.WriteLine($"{name} = r.GetUInt{inttype.Size}();");
                        break;
                    case IntType inttype when (inttype.IsSigned):
                        b.WriteLine($"{name} = r.GetInt{inttype.Size}();");
                        break;
                    case FloatType floatType when (floatType.Size == 32):
                        b.WriteLine($"{name} = r.GetFloat32();");
                        break;
                    case FloatType floatType when (floatType.Size == 64):
                        b.WriteLine($"{name} = r.GetFloat64();");
                        break;
                    case StringType stringType:
                        b.WriteLine($"{name} = r.GetString({stringType.MaxLength});");
                        break;
                    case TimestampType timestampType:
                        b.WriteLine($"{name} = r.GetTimestamp();");
                        break;
                    case BytesType memoryType when (memoryType.IsUseMemoryPool):
                        b.WriteLine($"{name} = r.GetRecyclableMemory({memoryType.MaxLength});");
                        break;
                    case BytesType memoryType when (!memoryType.IsUseMemoryPool):
                        b.WriteLine($"{name} = r.GetMemory({memoryType.MaxLength});");
                        break;
                    case VectorType listType:
                        {
                            b.WriteLine("var length = r.GetUInt32();");
                            b.WriteLine($"{name} = new {this.GenerateParameterTypeFullName(listType.ElementType)}[length];");

                            b.WriteLine($"for (int i = 0; i < {name}.Length; i++)");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Small_Formatter_Deserialize_PropertyDef(b, $"{name}[i]", listType.ElementType, rank + 1);
                            }

                            b.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            b.WriteLine("var length = r.GetUInt32();");
                            b.WriteLine($"{name} = new {GenerateTypeFullName("Dictionary<,>", this.GenerateParameterTypeFullName(mapType.KeyType), this.GenerateParameterTypeFullName(mapType.ValueType))}();");
                            b.WriteLine($"{this.GenerateParameterTypeFullName(mapType.KeyType)} t_key = {this.GetDefaultValueString(mapType.KeyType)};");
                            b.WriteLine($"{this.GenerateParameterTypeFullName(mapType.ValueType)} t_value = {this.GetDefaultValueString(mapType.ValueType)};");

                            b.WriteLine("for (int i = 0; i < length; i++)");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                this.Write_Small_Formatter_Deserialize_PropertyDef(b, "t_key", mapType.KeyType, rank + 1);
                                this.Write_Small_Formatter_Deserialize_PropertyDef(b, "t_value", mapType.ValueType, rank + 1);
                                b.WriteLine($"{name}[t_key] = t_value;");
                            }

                            b.WriteLine("}");
                        }
                        break;
                    case CustomType customType:
                        switch (this.FindDefinition(customType))
                        {
                            case EnumDefinition enumInfo:
                                switch (enumInfo.Type)
                                {
                                    case IntType inttype when (inttype.IsSigned):
                                        b.WriteLine($"{name} = ({enumInfo.Name})r.GetInt64();");
                                        break;
                                    case IntType inttype when (!inttype.IsSigned):
                                        b.WriteLine($"{name} = ({enumInfo.Name})r.GetUInt64();");
                                        break;
                                }
                                break;
                            case ObjectDefinition objectDefinition when (objectDefinition.IsStruct):
                                b.WriteLine($"{name} = {objectDefinition.FullName}.Formatter.Deserialize(ref r, rank + 1);");
                                break;
                            case ObjectDefinition objectDefinition when (objectDefinition.IsClass):
                                b.WriteLine($"{name} = {objectDefinition.FullName}.Formatter.Deserialize(ref r, rank + 1);");
                                break;
                        }
                        break;
                }
            }

            private void BlockWhoseValueIsNotDefault(CodeBuilder b, ObjectElement element, Action callback)
            {
                var sb = new StringBuilder();
                sb.Append($"if (");

                switch (element.Type)
                {
                    case BoolType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{element.Name} != false)");
                        }
                        else
                        {
                            sb.Append($"value.{element.Name} != null)");
                        }
                        break;
                    case IntType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{element.Name} != 0)");
                        }
                        else
                        {
                            sb.Append($"value.{element.Name} != null)");
                        }
                        break;
                    case FloatType type when (type.Size == 32):
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{element.Name} != 0.0F)");
                        }
                        else
                        {
                            sb.Append($"value.{element.Name} != null)");
                        }
                        break;
                    case FloatType type when (type.Size == 64):
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{element.Name} != 0.0D)");
                        }
                        else
                        {
                            sb.Append($"value.{element.Name} != null)");
                        }
                        break;
                    case StringType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{element.Name} != string.Empty)");
                        }
                        else
                        {
                            sb.Append($"value.{element.Name} != null)");
                        }
                        break;
                    case TimestampType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{element.Name} != {GenerateTypeFullName("Timestamp")}.Zero)");
                        }
                        else
                        {
                            sb.Append($"value.{element.Name} != null)");
                        }
                        break;
                    case BytesType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"!value.{element.Name}.IsEmpty)");
                        }
                        else
                        {
                            sb.Append($"value.{element.Name} != null)");
                        }
                        break;
                    case VectorType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{element.Name}.Count != 0)");
                        }
                        else
                        {
                            sb.Append($"value.{element.Name} != null)");
                        }
                        break;
                    case MapType type:
                        if (!type.IsOptional)
                        {
                            sb.Append($"value.{element.Name}.Count != 0)");
                        }
                        else
                        {
                            sb.Append($"value.{element.Name} != null)");
                        }
                        break;
                    case CustomType type:
                        {
                            switch (this.FindDefinition(type))
                            {
                                case EnumDefinition elementEnumDefinition:
                                    if (!type.IsOptional)
                                    {
                                        sb.Append($"value.{element.Name} != ({elementEnumDefinition.Name})0)");
                                    }
                                    else
                                    {
                                        sb.Append($"value.{element.Name} != null)");
                                    }
                                    break;
                                case ObjectDefinition elementMessageDefinition:
                                    if (!type.IsOptional)
                                    {
                                        sb.Append($"value.{element.Name} != {elementMessageDefinition.Name}.Empty)");
                                    }
                                    else
                                    {
                                        sb.Append($"value.{element.Name} != null)");
                                    }
                                    break;
                                default:
                                    throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(element.Type));
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException($"Type \"{element.Type.GetType().Name}\" was not found", nameof(element.Type));
                }

                b.WriteLine(sb.ToString());
                b.WriteLine("{");
                using (b.Indent())
                {
                    callback.Invoke();
                }
                b.WriteLine("}");
            }
        }
    }
}
