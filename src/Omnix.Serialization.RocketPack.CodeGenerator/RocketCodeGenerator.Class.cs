using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static partial class RocketCodeGenerator
    {
        /// <summary>
        /// <see cref="MessageInfo"/>からclassを生成します。
        /// </summary>
        class ClassWriter
        {
            private RocketFormatInfo _rocketFormatInfo;
            private IList<RocketFormatInfo> _externalRocketFormatInfos;

            private string _accessLevel;

            public ClassWriter(RocketFormatInfo rocketFormatInfo, IEnumerable<RocketFormatInfo> externalRocketFormatInfos)
            {
                _rocketFormatInfo = rocketFormatInfo;
                _externalRocketFormatInfos = externalRocketFormatInfos.ToList();

                var accessLevelOption = _rocketFormatInfo.Options.FirstOrDefault(n => n.Name == "csharp_access_level");
                _accessLevel = accessLevelOption?.Value ?? "public";
            }

            private object CustomTypeResolver(CustomTypeInfo n)
            {
                foreach (var targetInfo in new[] { _rocketFormatInfo }.Union(_externalRocketFormatInfos))
                {
                    var enumInfo = targetInfo.Enums.FirstOrDefault(m => m.Name == n.TypeName);
                    if (enumInfo != null) return enumInfo;

                    var messageInfo = targetInfo.Messages.FirstOrDefault(m => m.Name == n.TypeName);
                    if (messageInfo != null) return messageInfo;
                }

                return null;
            }

            /// <summary>
            /// <see cref="TypeInfo"/>から型を生成します。
            /// </summary>
            private string GetParameterTypeString(TypeInfo type)
            {
                switch (type)
                {
                    case BoolTypeInfo typeInfo:
                        return "bool" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 8):
                        return "byte" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 16):
                        return "ushort" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 32):
                        return "uint" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 64):
                        return "ulong" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (typeInfo.IsSigned && typeInfo.Size == 8):
                        return "sbyte" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (typeInfo.IsSigned && typeInfo.Size == 16):
                        return "short" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (typeInfo.IsSigned && typeInfo.Size == 32):
                        return "int" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (typeInfo.IsSigned && typeInfo.Size == 64):
                        return "long" + (typeInfo.IsNullable ? "?" : "");
                    case FloatTypeInfo typeInfo when (typeInfo.Size == 32):
                        return "float" + (typeInfo.IsNullable ? "?" : "");
                    case FloatTypeInfo typeInfo when (typeInfo.Size == 64):
                        return "double" + (typeInfo.IsNullable ? "?" : "");
                    case StringTypeInfo typeInfo:
                        return "string";
                    case TimestampTypeInfo typeInfo:
                        return "Timestamp" + (typeInfo.IsNullable ? "?" : "");
                    case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                        return "IMemoryOwner<byte>";
                    case MemoryTypeInfo typeInfo when (!typeInfo.IsUseMemoryPool):
                        return "ReadOnlyMemory<byte>";
                    case ListTypeInfo typeInfo:
                        return $"IList<{this.GetParameterTypeString(typeInfo.ElementType)}>";
                    case MapTypeInfo typeInfo:
                        return $"IDictionary<{this.GetParameterTypeString(typeInfo.KeyType)}, {this.GetParameterTypeString(typeInfo.ValueType)}>";
                    case CustomTypeInfo typeInfo:
                        {
                            switch (this.CustomTypeResolver(typeInfo))
                            {
                                case EnumInfo enumInfo:
                                    return typeInfo.TypeName + (typeInfo.IsNullable ? "?" : "");
                                case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                    return typeInfo.TypeName;
                                case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                    return typeInfo.TypeName + (typeInfo.IsNullable ? "?" : "");
                                default:
                                    throw new ArgumentException($"Type \"{typeInfo.TypeName}\" was not found", nameof(type));
                            }
                        }
                    default:
                        throw new ArgumentException(nameof(type));
                }
            }

            /// <summary>
            /// <see cref="TypeInfo"/>から型を生成します。
            /// </summary>
            private string GetPropertyTypeString(TypeInfo type)
            {
                switch (type)
                {
                    case BoolTypeInfo typeInfo:
                        return "bool" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 8):
                        return "byte" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 16):
                        return "ushort" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 32):
                        return "uint" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 64):
                        return "ulong" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (typeInfo.IsSigned && typeInfo.Size == 8):
                        return "sbyte" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (typeInfo.IsSigned && typeInfo.Size == 16):
                        return "short" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (typeInfo.IsSigned && typeInfo.Size == 32):
                        return "int" + (typeInfo.IsNullable ? "?" : "");
                    case IntTypeInfo typeInfo when (typeInfo.IsSigned && typeInfo.Size == 64):
                        return "long" + (typeInfo.IsNullable ? "?" : "");
                    case FloatTypeInfo typeInfo when (typeInfo.Size == 32):
                        return "float" + (typeInfo.IsNullable ? "?" : "");
                    case FloatTypeInfo typeInfo when (typeInfo.Size == 64):
                        return "double" + (typeInfo.IsNullable ? "?" : "");
                    case StringTypeInfo typeInfo:
                        return "string";
                    case TimestampTypeInfo typeInfo:
                        return "Timestamp" + (typeInfo.IsNullable ? "?" : "");
                    case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                        return "ReadOnlyMemory<byte>";
                    case MemoryTypeInfo typeInfo when (!typeInfo.IsUseMemoryPool):
                        return "ReadOnlyMemory<byte>";
                    case ListTypeInfo typeInfo:
                        return $"IReadOnlyList<{this.GetParameterTypeString(typeInfo.ElementType)}>";
                    case MapTypeInfo typeInfo:
                        return $"IReadOnlyDictionary<{this.GetParameterTypeString(typeInfo.KeyType)}, {this.GetParameterTypeString(typeInfo.ValueType)}>";
                    case CustomTypeInfo typeInfo:
                        {
                            switch (this.CustomTypeResolver(typeInfo))
                            {
                                case EnumInfo enumInfo:
                                    return typeInfo.TypeName + (typeInfo.IsNullable ? "?" : "");
                                case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                    return typeInfo.TypeName;
                                case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                    return typeInfo.TypeName + (typeInfo.IsNullable ? "?" : "");
                                default:
                                    throw new ArgumentException($"Type \"{typeInfo.TypeName}\" was not found", nameof(type));
                            }
                        }
                    default:
                        throw new ArgumentException(nameof(type));
                }
            }

            /// <summary>
            /// クラスの生成。
            /// </summary>
            public void Write(CodeWriter w, MessageInfo messageInfo)
            {
                if (messageInfo.Elements.Select(n => n.Type).OfType<MemoryTypeInfo>().Any(n => n.IsUseMemoryPool))
                {
                    w.WriteLine($"{_accessLevel} sealed partial class {messageInfo.Name} : RocketPackMessageBase<{messageInfo.Name}>, IDisposable");
                }
                else
                {
                    w.WriteLine($"{_accessLevel} sealed partial class {messageInfo.Name} : RocketPackMessageBase<{messageInfo.Name}>");
                }

                w.WriteLine("{");

                w.PushIndent();

                this.Write_StaticConstructor(w, messageInfo);
                w.WriteLine();

                this.Write_Constructor(w, messageInfo);
                w.WriteLine();

                this.Write_Properties(w, messageInfo);
                w.WriteLine();

                this.Write_Equals(w, messageInfo);
                w.WriteLine();

                this.Write_GetHashCode(w, messageInfo);
                w.WriteLine();

                if (messageInfo.Elements.Select(n => n.Type).OfType<MemoryTypeInfo>().Any(n => n.IsUseMemoryPool))
                {
                    this.Write_Dispose(w, messageInfo);
                    w.WriteLine();
                }

                this.Write_Formatter(w, messageInfo);

                w.PopIndent();

                w.WriteLine("}");
            }

            /// <summary>
            /// 静的コンストラクタの生成。
            /// </summary>
            private void Write_StaticConstructor(CodeWriter w, MessageInfo messageInfo)
            {
                w.WriteLine($"static {messageInfo.Name}()");
                w.WriteLine("{");

                w.PushIndent();

                w.WriteLine($"{messageInfo.Name}.Formatter = new CustomFormatter();");

                w.PopIndent();

                w.WriteLine("}");
            }

            /// <summary>
            /// コンストラクタの生成。
            /// </summary>
            private void Write_Constructor(CodeWriter w, MessageInfo messageInfo)
            {
                // 最大サイズの宣言。
                {
                    bool isDefinedMaxLength = false;

                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        isDefinedMaxLength |= this.Try_Write_Constructor_Define_MaxLength(w, elementInfo.Name, elementInfo.Type);
                    }

                    if (isDefinedMaxLength) w.WriteLine();
                }

                // パラメータの生成。
                var parameter = string.Join(", ", messageInfo.Elements.Select(element => this.GetParameterTypeString(element.Type) + " " + GetFieldName(element.Name)));

                w.WriteLine($"public {messageInfo.Name}({parameter})");
                w.WriteLine("{");

                w.PushIndent();

                // パラメータのチェック
                {
                    bool isChecked = false;

                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        isChecked = this.Try_Write_Constructor_Check(w, elementInfo);
                    }

                    if (isChecked) w.WriteLine();
                }

                foreach (var elementInfo in messageInfo.Elements)
                {
                    switch (elementInfo.Type)
                    {
                        case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                            w.WriteLine($"_{GetFieldName(elementInfo.Name)} = {GetFieldName(elementInfo.Name)};");
                            break;
                        case ListTypeInfo typeInfo:
                            w.WriteLine($"this.{elementInfo.Name} = new ReadOnlyCollection<{this.GetParameterTypeString(typeInfo.ElementType)}>({GetFieldName(elementInfo.Name)});");
                            break;
                        case MapTypeInfo typeInfo:
                            w.WriteLine($"this.{elementInfo.Name} = new ReadOnlyDictionary<{this.GetParameterTypeString(typeInfo.KeyType)}, {this.GetParameterTypeString(typeInfo.ValueType)}>({GetFieldName(elementInfo.Name)});");
                            break;
                        default:
                            w.WriteLine($"this.{elementInfo.Name} = {GetFieldName(elementInfo.Name)};");
                            break;
                    }
                }

                w.WriteLine();

                this.Write_Constructor_HashCode(w, messageInfo);

                w.PopIndent();

                w.WriteLine("}");
            }

            private bool Try_Write_Constructor_Check(CodeWriter w, MessageElementInfo elementInfo)
            {
                bool isChecked = false;

                isChecked |= this.Try_Write_Constructor_CheckNull(w, GetFieldName(elementInfo.Name), elementInfo.Type);
                isChecked |= this.Try_Write_Constructor_CheckMaxLength(w, GetFieldName(elementInfo.Name), elementInfo.Type);

                if (elementInfo.Type is ListTypeInfo listTypeInfo)
                {
                    var w2 = new CodeWriter();
                    bool isChecked2 = false;

                    w2.WriteLine($"foreach (var n in {GetFieldName(elementInfo.Name)})");
                    w2.WriteLine("{");

                    w2.PushIndent();

                    {
                        isChecked2 |= this.Try_Write_Constructor_CheckNull(w2, "n", listTypeInfo.ElementType);
                        isChecked2 |= this.Try_Write_Constructor_CheckMaxLength(w2, "n", listTypeInfo.ElementType);
                    }

                    w2.PopIndent();

                    w2.WriteLine("}");

                    if (isChecked2) w.WriteLine(w2.ToString().TrimEnd());

                    isChecked |= isChecked2;
                }
                else if (elementInfo.Type is MapTypeInfo mapTypeInfo)
                {
                    var w2 = new CodeWriter();
                    bool isChecked2 = false;

                    w2.WriteLine($"foreach (var n in {GetFieldName(elementInfo.Name)})");
                    w2.WriteLine("{");

                    w2.PushIndent();

                    {
                        isChecked2 |= this.Try_Write_Constructor_CheckNull(w2, "n.Key", mapTypeInfo.KeyType);
                        isChecked2 |= this.Try_Write_Constructor_CheckNull(w2, "n.Value", mapTypeInfo.ValueType);
                        isChecked2 |= this.Try_Write_Constructor_CheckMaxLength(w2, "n.Key", mapTypeInfo.KeyType);
                        isChecked2 |= this.Try_Write_Constructor_CheckMaxLength(w2, "n.Value", mapTypeInfo.ValueType);
                    }

                    w2.PopIndent();

                    w2.WriteLine("}");

                    if (isChecked2) w.WriteLine(w2.ToString().TrimEnd());

                    isChecked |= isChecked2;
                }

                return isChecked;
            }

            private bool Try_Write_Constructor_CheckNull(CodeWriter w, string name, TypeInfo typeInfo)
            {
                switch (typeInfo)
                {
                    case StringTypeInfo stringTypeInfo when (!typeInfo.IsNullable):
                        w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                        return true;
                    case MemoryTypeInfo memoryTypeInfo when (memoryTypeInfo.IsUseMemoryPool):
                        w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                        return true;
                    case ListTypeInfo listTypeInfo when (!typeInfo.IsNullable):
                        w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                        return true;
                    case MapTypeInfo mapTypeInfo when (!typeInfo.IsNullable):
                        w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                        return true;
                    case CustomTypeInfo customTypeInfo when (!typeInfo.IsNullable):
                        switch (this.CustomTypeResolver(customTypeInfo))
                        {
                            case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                                return true;
                            default:
                                return false;
                        }
                    default:
                        return false;
                }
            }

            private bool Try_Write_Constructor_CheckMaxLength(CodeWriter w, string name, TypeInfo typeInfo)
            {
                string property = null;
                int? maxLength = null;

                switch (typeInfo)
                {
                    case StringTypeInfo stringTypeInfo:
                        property = "Length";
                        maxLength = stringTypeInfo.MaxLength;
                        break;
                    case MemoryTypeInfo memoryTypeInfo when (!memoryTypeInfo.IsUseMemoryPool):
                        property = "Length";
                        maxLength = memoryTypeInfo.MaxLength;
                        break;
                    case MemoryTypeInfo memoryTypeInfo when (memoryTypeInfo.IsUseMemoryPool):
                        property = "Memory.Length";
                        maxLength = memoryTypeInfo.MaxLength;
                        break;
                    case ListTypeInfo listTypeInfo:
                        property = "Count";
                        maxLength = listTypeInfo.MaxLength;
                        break;
                    case MapTypeInfo mapTypeInfo:
                        property = "Count";
                        maxLength = mapTypeInfo.MaxLength;
                        break;
                    default:
                        return false;
                }

                if (typeInfo.IsNullable)
                {
                    w.WriteLine($"if ({name} != null && {name}.{property} > {maxLength}) throw new ArgumentOutOfRangeException(\"{name}\");");
                }
                else
                {
                    w.WriteLine($"if ({name}.{property} > {maxLength}) throw new ArgumentOutOfRangeException(\"{name}\");");
                }

                return true;
            }

            private bool Try_Write_Constructor_Define_MaxLength(CodeWriter w, string name, TypeInfo typeInfo)
            {
                switch (typeInfo)
                {
                    case StringTypeInfo stringTypeInfo:
                        w.WriteLine($"public static readonly int Max{name}Length = {stringTypeInfo.MaxLength};");
                        return true;
                    case MemoryTypeInfo memoryTypeInfo:
                        w.WriteLine($"public static readonly int Max{name}Length = {memoryTypeInfo.MaxLength};");
                        return true;
                    case ListTypeInfo listTypeInfo:
                        w.WriteLine($"public static readonly int Max{name}Count = {listTypeInfo.MaxLength};");
                        return true;
                    case MapTypeInfo mapTypeInfo:
                        w.WriteLine($"public static readonly int Max{name}Count = {mapTypeInfo.MaxLength};");
                        return true;
                    default:
                        return false;
                }
            }

            private void Write_Constructor_HashCode(CodeWriter w, MessageInfo messageInfo)
            {
                w.WriteLine("{");

                w.PushIndent();

                {
                    w.WriteLine("var hashCode = new HashCode();");

                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        this.Write_Constructor_HashCode_Element(w, "this." + elementInfo.Name, elementInfo.Type);
                    }

                    w.WriteLine("_hashCode = hashCode.ToHashCode();");
                }

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_Constructor_HashCode_Element(CodeWriter w, string name, TypeInfo typeInfo)
            {
                switch (typeInfo)
                {
                    case BoolTypeInfo boolTypeInfo:
                        w.WriteLine($"if ({name} != default) hashCode.Add({name}.GetHashCode());");
                        break;
                    case IntTypeInfo intTypeInfo:
                        w.WriteLine($"if ({name} != default) hashCode.Add({name}.GetHashCode());");
                        break;
                    case FloatTypeInfo floatTypeInfo:
                        w.WriteLine($"if ({name} != default) hashCode.Add({name}.GetHashCode());");
                        break;
                    case StringTypeInfo stringTypeInfo:
                        w.WriteLine($"if ({name} != default) hashCode.Add({name}.GetHashCode());");
                        break;
                    case TimestampTypeInfo timestampTypeInfo:
                        w.WriteLine($"if ({name} != default) hashCode.Add({name}.GetHashCode());");
                        break;
                    case MemoryTypeInfo memoryTypeInfo when (memoryTypeInfo.IsUseMemoryPool):
                        w.WriteLine($"if (!{name}.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode({name}.Span));");
                        break;
                    case MemoryTypeInfo memoryTypeInfo when (!memoryTypeInfo.IsUseMemoryPool):
                        w.WriteLine($"if (!{name}.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode({name}.Span));");
                        break;
                    case ListTypeInfo listTypeInfo:
                        {
                            w.WriteLine($"foreach (var n in {GetFieldName(name)})");
                            w.WriteLine("{");

                            w.PushIndent();

                            {
                                this.Write_Constructor_HashCode_Element(w, "n", listTypeInfo.ElementType);
                            }

                            w.PopIndent();

                            w.WriteLine("}");
                        }
                        break;
                    case MapTypeInfo mapTypeInfo:
                        {
                            w.WriteLine($"foreach (var n in {GetFieldName(name)})");
                            w.WriteLine("{");

                            w.PushIndent();

                            {
                                this.Write_Constructor_HashCode_Element(w, "n.Key", mapTypeInfo.KeyType);
                                this.Write_Constructor_HashCode_Element(w, "n.Value", mapTypeInfo.ValueType);
                            }

                            w.PopIndent();

                            w.WriteLine("}");
                        }
                        break;
                    case CustomTypeInfo customTypeInfo:
                        w.WriteLine($"if ({name} != default) hashCode.Add({name}.GetHashCode());");
                        break;
                }
            }

            private void Write_Equals(CodeWriter w, MessageInfo messageInfo)
            {
                w.WriteLine($"public override bool Equals({messageInfo.Name} target)");
                w.WriteLine("{");

                w.PushIndent();

                {
                    w.WriteLine("if ((object)target == null) return false;");
                    w.WriteLine("if (Object.ReferenceEquals(this, target)) return true;");

                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        switch (elementInfo.Type)
                        {
                            case BoolTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != target.{elementInfo.Name}) return false;");
                                break;
                            case IntTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != target.{elementInfo.Name}) return false;");
                                break;
                            case FloatTypeInfo typeInfo when (typeInfo.Size == 32):
                                w.WriteLine($"if (this.{elementInfo.Name} != target.{elementInfo.Name}) return false;");
                                break;
                            case StringTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != target.{elementInfo.Name}) return false;");
                                break;
                            case TimestampTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != target.{elementInfo.Name}) return false;");
                                break;
                            case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                                w.WriteLine($"if (!BytesOperations.SequenceEqual(this.{elementInfo.Name}.Span, target.{elementInfo.Name}.Span)) return false;");
                                break;
                            case MemoryTypeInfo typeInfo when (!typeInfo.IsUseMemoryPool):
                                w.WriteLine($"if (!BytesOperations.SequenceEqual(this.{elementInfo.Name}.Span, target.{elementInfo.Name}.Span)) return false;");
                                break;
                            case ListTypeInfo typeInfo:
                                w.WriteLine($"if ((this.{elementInfo.Name} is null) != (target.{elementInfo.Name} is null)) return false;");
                                w.WriteLine($"if (!(this.{elementInfo.Name} is null) && !(target.{elementInfo.Name} is null) && !CollectionHelper.Equals(this.{elementInfo.Name}, target.{elementInfo.Name})) return false;");
                                break;
                            case MapTypeInfo typeInfo:
                                w.WriteLine($"if ((this.{elementInfo.Name} is null) != (target.{elementInfo.Name} is null)) return false;");
                                w.WriteLine($"if (!(this.{elementInfo.Name} is null) && !(target.{elementInfo.Name} is null) && !CollectionHelper.Equals(this.{elementInfo.Name}, target.{elementInfo.Name})) return false;");
                                break;
                            case CustomTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != target.{elementInfo.Name}) return false;");
                                break;
                        }
                    }
                }

                w.WriteLine();

                w.WriteLine("return true;");

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_GetHashCode(CodeWriter w, MessageInfo messageInfo)
            {
                w.WriteLine("private readonly int _hashCode;");
                w.WriteLine("public override int GetHashCode() => _hashCode;");
            }

            private void Write_Properties(CodeWriter w, MessageInfo messageInfo)
            {
                foreach (var elementInfo in messageInfo.Elements.OrderBy(n => n.Id))
                {
                    switch (elementInfo.Type)
                    {
                        case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                            w.WriteLine($"private readonly {this.GetParameterTypeString(elementInfo.Type)} _{GetFieldName(elementInfo.Name)};");
                            w.WriteLine($"public {this.GetPropertyTypeString(elementInfo.Type)} {elementInfo.Name} => _{GetFieldName(elementInfo.Name)}.Memory;");
                            break;
                        default:
                            w.WriteLine($"public {this.GetPropertyTypeString(elementInfo.Type)} {elementInfo.Name} {{ get; }}");
                            break;
                    }
                }
            }

            private void Write_Dispose(CodeWriter w, MessageInfo messageInfo)
            {
                w.WriteLine("public void Dispose()");
                w.WriteLine("{");

                w.PushIndent();

                foreach (var elementInfo in messageInfo.Elements)
                {
                    switch (elementInfo.Type)
                    {
                        case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                            w.WriteLine($"_{GetFieldName(elementInfo.Name)}?.Dispose();");
                            break;
                    }
                }

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_Formatter(CodeWriter w, MessageInfo messageInfo)
            {
                w.WriteLine($"private sealed class CustomFormatter : IRocketPackFormatter<{messageInfo.Name}>");
                w.WriteLine("{");

                w.PushIndent();

                {
                    w.WriteLine($"public void Serialize(RocketPackWriter w, {messageInfo.Name} value, int rank)");
                    w.WriteLine("{");

                    w.PushIndent();

                    w.WriteLine("if (rank > 256) throw new FormatException();");

                    w.WriteLine();

                    {
                        w.WriteLine("// Write property count");
                        w.WriteLine("{");

                        w.PushIndent();

                        w.WriteLine("uint propertyCount = 0;");

                        foreach (var elementInfo in messageInfo.Elements)
                        {
                            switch (elementInfo.Type)
                            {
                                case BoolTypeInfo typeInfo:
                                    w.WriteLine($"if (value.{elementInfo.Name} != default) propertyCount++;");
                                    break;
                                case IntTypeInfo typeInfo:
                                    w.WriteLine($"if (value.{elementInfo.Name} != default) propertyCount++;");
                                    break;
                                case FloatTypeInfo typeInfo:
                                    w.WriteLine($"if (value.{elementInfo.Name} != default) propertyCount++;");
                                    break;
                                case StringTypeInfo typeInfo:
                                    w.WriteLine($"if (value.{elementInfo.Name} != default) propertyCount++;");
                                    break;
                                case TimestampTypeInfo typeInfo:
                                    w.WriteLine($"if (value.{elementInfo.Name} != default) propertyCount++;");
                                    break;
                                case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                                    w.WriteLine($"if (!value.{elementInfo.Name}.IsEmpty) propertyCount++;");
                                    break;
                                case MemoryTypeInfo typeInfo when (!typeInfo.IsUseMemoryPool):
                                    w.WriteLine($"if (!value.{elementInfo.Name}.IsEmpty) propertyCount++;");
                                    break;
                                case ListTypeInfo typeInfo:
                                    w.WriteLine($"if (value.{elementInfo.Name}.Count != 0) propertyCount++;");
                                    break;
                                case MapTypeInfo typeInfo:
                                    w.WriteLine($"if (value.{elementInfo.Name}.Count != 0) propertyCount++;");
                                    break;
                                case CustomTypeInfo typeInfo:
                                    w.WriteLine($"if (value.{elementInfo.Name} != default) propertyCount++;");
                                    break;
                            }
                        }

                        w.WriteLine($"w.Write(propertyCount);");

                        w.PopIndent();

                        w.WriteLine("}");
                    }

                    w.WriteLine();

                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        w.WriteLine($"// {elementInfo.Name}");

                        switch (elementInfo.Type)
                        {
                            case BoolTypeInfo typeInfo:
                                w.WriteLine($"if (value.{elementInfo.Name} != default)");
                                break;
                            case IntTypeInfo typeInfo:
                                w.WriteLine($"if (value.{elementInfo.Name} != default)");
                                break;
                            case FloatTypeInfo typeInfo:
                                w.WriteLine($"if (value.{elementInfo.Name} != default)");
                                break;
                            case StringTypeInfo typeInfo:
                                w.WriteLine($"if (value.{elementInfo.Name} != default)");
                                break;
                            case TimestampTypeInfo typeInfo:
                                w.WriteLine($"if (value.{elementInfo.Name} != default)");
                                break;
                            case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                                w.WriteLine($"if (!value.{elementInfo.Name}.IsEmpty)");
                                break;
                            case MemoryTypeInfo typeInfo when (!typeInfo.IsUseMemoryPool):
                                w.WriteLine($"if (!value.{elementInfo.Name}.IsEmpty)");
                                break;
                            case ListTypeInfo typeInfo:
                                w.WriteLine($"if (value.{elementInfo.Name}.Count != 0)");
                                break;
                            case MapTypeInfo typeInfo:
                                w.WriteLine($"if (value.{elementInfo.Name}.Count != 0)");
                                break;
                            case CustomTypeInfo typeInfo:
                                w.WriteLine($"if (value.{elementInfo.Name} != default)");
                                break;
                        }

                        w.WriteLine("{");

                        w.PushIndent();

                        w.WriteLine($"w.Write((uint){elementInfo.Id});");
                        this.Write_Formatter_Serialize_PropertyDef(w, "value." + elementInfo.Name, elementInfo.Type, 0);

                        w.PopIndent();

                        w.WriteLine("}");
                    }

                    w.PopIndent();

                    w.WriteLine("}");
                }

                w.WriteLine();

                {
                    w.WriteLine($"public {messageInfo.Name} Deserialize(RocketPackReader r, int rank)");
                    w.WriteLine("{");

                    w.PushIndent();

                    w.WriteLine("if (rank > 256) throw new FormatException();");

                    w.WriteLine();

                    w.WriteLine("// Read property count");
                    w.WriteLine("uint propertyCount = r.GetUInt32();");

                    w.WriteLine();

                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        switch (elementInfo.Type)
                        {
                            case BoolTypeInfo typeInfo:
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                            case IntTypeInfo typeInfo:
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                            case FloatTypeInfo typeInfo:
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                            case StringTypeInfo typeInfo:
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                            case TimestampTypeInfo typeInfo:
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                            case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                            case MemoryTypeInfo typeInfo when (!typeInfo.IsUseMemoryPool):
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                            case ListTypeInfo typeInfo:
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                            case MapTypeInfo typeInfo:
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                            case CustomTypeInfo typeInfo:
                                w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = default;");
                                break;
                        }
                    }

                    w.WriteLine();

                    w.WriteLine("for (; propertyCount > 0; propertyCount--)");
                    w.WriteLine("{");

                    w.PushIndent();

                    w.WriteLine("uint id = r.GetUInt32();");
                    w.WriteLine("switch (id)");
                    w.WriteLine("{");

                    w.PushIndent();

                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        w.WriteLine($"case {elementInfo.Id}: // {elementInfo.Name}");

                        w.PushIndent();

                        {
                            w.WriteLine("{");

                            w.PushIndent();

                            this.Write_Formatter_Deserialize_PropertyDef(w, "p_" + GetFieldName(elementInfo.Name), elementInfo.Type, 0);

                            w.WriteLine("break;");

                            w.PopIndent();

                            w.WriteLine("}");
                        }

                        w.PopIndent();
                    }

                    w.PopIndent();
                    w.WriteLine("}");

                    w.PopIndent();

                    w.WriteLine("}");

                    w.WriteLine();

                    w.WriteLine($"return new {messageInfo.Name}({ string.Join(", ", messageInfo.Elements.Select(n => "p_" + GetFieldName(n.Name)))});");

                    w.PopIndent();

                    w.WriteLine("}");
                }

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_Formatter_Serialize_PropertyDef(CodeWriter w, string name, TypeInfo typeInfo, int rank)
            {
                switch (typeInfo)
                {
                    case BoolTypeInfo boolTypeInfo:
                        w.WriteLine($"w.Write({name});");
                        break;
                    case IntTypeInfo intTypeInfo:
                        w.WriteLine($"w.Write({name});");
                        break;
                    case FloatTypeInfo floatTypeInfo:
                        w.WriteLine($"w.Write({name});");
                        break;
                    case StringTypeInfo stringTypeInfo:
                        w.WriteLine($"w.Write({name});");
                        break;
                    case TimestampTypeInfo timestampTypeInfo:
                        w.WriteLine($"w.Write({name});");
                        break;
                    case MemoryTypeInfo memoryTypeInfo when (memoryTypeInfo.IsUseMemoryPool):
                        w.WriteLine($"w.Write({name}.Span);");
                        break;
                    case MemoryTypeInfo memoryTypeInfo when (!memoryTypeInfo.IsUseMemoryPool):
                        w.WriteLine($"w.Write({name}.Span);");
                        break;
                    case ListTypeInfo listTypeInfo:
                        {
                            w.WriteLine($"w.Write((uint){name}.Count);");
                            w.WriteLine($"foreach (var n in {name})");
                            w.WriteLine("{");

                            w.PushIndent();

                            {
                                this.Write_Formatter_Serialize_PropertyDef(w, "n", listTypeInfo.ElementType, rank + 1);
                            }

                            w.PopIndent();

                            w.WriteLine("}");
                        }
                        break;
                    case MapTypeInfo mapTypeInfo:
                        {
                            w.WriteLine($"w.Write((uint){name}.Count);");
                            w.WriteLine($"foreach (var n in {name})");
                            w.WriteLine("{");

                            w.PushIndent();

                            {
                                this.Write_Formatter_Serialize_PropertyDef(w, "n.Key", mapTypeInfo.KeyType, rank + 1);
                                this.Write_Formatter_Serialize_PropertyDef(w, "n.Value", mapTypeInfo.ValueType, rank + 1);
                            }

                            w.PopIndent();

                            w.WriteLine("}");
                        }
                        break;
                    case CustomTypeInfo customTypeInfo:
                        switch (this.CustomTypeResolver(customTypeInfo))
                        {
                            case EnumInfo enumInfo:
                                switch (enumInfo.Type)
                                {
                                    case IntTypeInfo intTypeInfo when (intTypeInfo.IsSigned):
                                        w.WriteLine($"w.Write((long){name});");
                                        break;
                                    case IntTypeInfo intTypeInfo when (!intTypeInfo.IsSigned):
                                        w.WriteLine($"w.Write((ulong){name});");
                                        break;
                                }
                                break;
                            case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                w.WriteLine($"{messageInfo.Name}.Formatter.Serialize(w, {name}, rank + 1);");
                                break;
                            case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                w.WriteLine($"{messageInfo.Name}.Formatter.Serialize(w, {name}, rank + 1);");
                                break;
                        }
                        break;
                }
            }

            private void Write_Formatter_Deserialize_PropertyDef(CodeWriter w, string name, TypeInfo typeInfo, int rank)
            {
                switch (typeInfo)
                {
                    case BoolTypeInfo boolTypeInfo:
                        w.WriteLine($"{name} = r.GetBoolean();");
                        break;
                    case IntTypeInfo intTypeInfo when (!intTypeInfo.IsSigned):
                        w.WriteLine($"{name} = r.GetUInt{intTypeInfo.Size}();");
                        break;
                    case IntTypeInfo intTypeInfo when (intTypeInfo.IsSigned):
                        w.WriteLine($"{name} = r.GetInt{intTypeInfo.Size}();");
                        break;
                    case FloatTypeInfo floatTypeInfo when (floatTypeInfo.Size == 32):
                        w.WriteLine($"{name} = r.GetFloat32();");
                        break;
                    case FloatTypeInfo floatTypeInfo when (floatTypeInfo.Size == 64):
                        w.WriteLine($"{name} = r.GetFloat64();");
                        break;
                    case StringTypeInfo stringTypeInfo:
                        w.WriteLine($"{name} = r.GetString({stringTypeInfo.MaxLength});");
                        break;
                    case TimestampTypeInfo timestampTypeInfo:
                        w.WriteLine($"{name} = r.GetTimestamp();");
                        break;
                    case MemoryTypeInfo memoryTypeInfo when (memoryTypeInfo.IsUseMemoryPool):
                        w.WriteLine($"{name} = r.GetRecyclableMemory({memoryTypeInfo.MaxLength});");
                        break;
                    case MemoryTypeInfo memoryTypeInfo when (!memoryTypeInfo.IsUseMemoryPool):
                        w.WriteLine($"{name} = r.GetMemory({memoryTypeInfo.MaxLength});");
                        break;
                    case ListTypeInfo listTypeInfo:
                        {
                            w.WriteLine("var length = r.GetUInt32();");
                            w.WriteLine($"{name} = new {this.GetParameterTypeString(listTypeInfo.ElementType)}[length];");

                            w.WriteLine($"for (int i = 0; i < {name}.Count; i++)");
                            w.WriteLine("{");

                            w.PushIndent();

                            this.Write_Formatter_Deserialize_PropertyDef(w, $"{name}[i]", listTypeInfo.ElementType, rank + 1);

                            w.PopIndent();

                            w.WriteLine("}");
                        }
                        break;
                    case MapTypeInfo mapTypeInfo:
                        {
                            w.WriteLine("var length = r.GetUInt32();");
                            w.WriteLine($"{name} = new Dictionary<{this.GetParameterTypeString(mapTypeInfo.KeyType)}, {this.GetParameterTypeString(mapTypeInfo.ValueType)}>();");
                            w.WriteLine($"{this.GetParameterTypeString(mapTypeInfo.KeyType)} t_key = default;");
                            w.WriteLine($"{this.GetParameterTypeString(mapTypeInfo.ValueType)} t_value = default;");

                            w.WriteLine("for (int i = 0; i < length; i++)");
                            w.WriteLine("{");

                            w.PushIndent();

                            this.Write_Formatter_Deserialize_PropertyDef(w, "t_key", mapTypeInfo.KeyType, rank + 1);
                            this.Write_Formatter_Deserialize_PropertyDef(w, "t_value", mapTypeInfo.ValueType, rank + 1);
                            w.WriteLine($"{name}[t_key] = t_value;");

                            w.PopIndent();

                            w.WriteLine("}");
                        }
                        break;
                    case CustomTypeInfo customTypeInfo:
                        switch (this.CustomTypeResolver(customTypeInfo))
                        {
                            case EnumInfo enumInfo:
                                switch (enumInfo.Type)
                                {
                                    case IntTypeInfo intTypeInfo when (intTypeInfo.IsSigned):
                                        w.WriteLine($"{name} = ({enumInfo.Name})r.GetInt64();");
                                        break;
                                    case IntTypeInfo intTypeInfo when (!intTypeInfo.IsSigned):
                                        w.WriteLine($"{name} = ({enumInfo.Name})r.GetUInt64();");
                                        break;
                                }
                                break;
                            case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                w.WriteLine($"{name} = {messageInfo.Name}.Formatter.Deserialize(r, rank + 1);");
                                break;
                            case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                w.WriteLine($"{name} = {messageInfo.Name}.Formatter.Deserialize(r, rank + 1);");
                                break;
                        }
                        break;
                }
            }
        }
    }
}
