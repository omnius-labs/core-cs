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
            private Func<CustomTypeInfo, object> _customTypeResolver;

            public ClassWriter(Func<CustomTypeInfo, object> customTypeResolver)
            {
                _customTypeResolver = customTypeResolver;
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
                            switch (_customTypeResolver.Invoke(typeInfo))
                            {
                                case EnumInfo enumInfo:
                                    return typeInfo.TypeName + (typeInfo.IsNullable ? "?" : "");
                                case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                    return typeInfo.TypeName;
                                case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                    return typeInfo.TypeName + (typeInfo.IsNullable ? "?" : "");
                                default:
                                    throw new ArgumentException(nameof(type));
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
                            switch (_customTypeResolver.Invoke(typeInfo))
                            {
                                case EnumInfo enumInfo:
                                    return typeInfo.TypeName + (typeInfo.IsNullable ? "?" : "");
                                case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                    return typeInfo.TypeName;
                                case MessageInfo messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                    return typeInfo.TypeName + (typeInfo.IsNullable ? "?" : "");
                                default:
                                    throw new ArgumentException(nameof(type));
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
                    w.WriteLine($"public sealed partial class {messageInfo.Name} : RocketPackMessageBase<{messageInfo.Name}>, IDisposable");
                }
                else
                {
                    w.WriteLine($"public sealed partial class {messageInfo.Name} : RocketPackMessageBase<{messageInfo.Name}>");
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
                bool isDefinedMaxLength = false;

                // 最大サイズの宣言。
                foreach (var elementInfo in messageInfo.Elements)
                {
                    isDefinedMaxLength |= this.Try_Write_Constructor_Define_MaxLength(w, elementInfo.Name, elementInfo.Type);
                }

                if (isDefinedMaxLength) w.WriteLine();

                // パラメータの生成。
                var parameter = string.Join(", ", messageInfo.Elements.Select(element => this.GetParameterTypeString(element.Type) + " " + GetFieldName(element.Name)));

                w.WriteLine($"public {messageInfo.Name}({parameter})");
                w.WriteLine("{");

                w.PushIndent();

                bool isChecked = false;

                // パラメータのnullチェック
                foreach (var elementInfo in messageInfo.Elements)
                {
                    isChecked |= this.Try_Write_Constructor_CheckNull(w, GetFieldName(elementInfo.Name), elementInfo.Type);
                }

                // パラメータの最大長チェック
                foreach (var elementInfo in messageInfo.Elements)
                {
                    isChecked |= this.Try_Write_Constructor_CheckMaxLength(w, GetFieldName(elementInfo.Name), elementInfo.Type);
                }

                // listとmapのパラメータの要素のnullチェック
                foreach (var elementInfo in messageInfo.Elements)
                {
                    if (elementInfo.Type is ListTypeInfo listTypeInfo)
                    {
                        w.WriteLine($"foreach (var n in {GetFieldName(elementInfo.Name)})");
                        w.WriteLine("{");

                        w.PushIndent();

                        {
                            isChecked |= this.Try_Write_Constructor_CheckNull(w, "n", listTypeInfo.ElementType);
                            isChecked |= this.Try_Write_Constructor_CheckMaxLength(w, "n", listTypeInfo.ElementType);
                        }

                        w.PopIndent();

                        w.WriteLine("}");
                    }
                    else if (elementInfo.Type is MapTypeInfo mapTypeInfo)
                    {
                        w.WriteLine($"foreach (var n in {GetFieldName(elementInfo.Name)})");
                        w.WriteLine("{");

                        w.PushIndent();

                        {
                            isChecked |= this.Try_Write_Constructor_CheckNull(w, "n.Key", mapTypeInfo.KeyType);
                            isChecked |= this.Try_Write_Constructor_CheckNull(w, "n.Value", mapTypeInfo.ValueType);
                            isChecked |= this.Try_Write_Constructor_CheckMaxLength(w, "n.Key", mapTypeInfo.KeyType);
                            isChecked |= this.Try_Write_Constructor_CheckMaxLength(w, "n.Value", mapTypeInfo.ValueType);
                        }

                        w.PopIndent();

                        w.WriteLine("}");
                    }
                }

                if (isChecked) w.WriteLine();

                foreach (var elementInfo in messageInfo.Elements)
                {
                    switch (elementInfo.Type)
                    {
                        case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                            w.WriteLine($"_{GetFieldName(elementInfo.Name)} = {GetFieldName(elementInfo.Name)};");
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

            private void Write_Constructor_HashCode(CodeWriter w, MessageInfo messageInfo)
            {
                w.WriteLine("{");

                w.PushIndent();

                {
                    w.WriteLine("var hashCode = new HashCode();");

                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        switch (elementInfo.Type)
                        {
                            case BoolTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != default) hashCode.Add(this.{elementInfo.Name}.GetHashCode());");
                                break;
                            case IntTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != default) hashCode.Add(this.{elementInfo.Name}.GetHashCode());");
                                break;
                            case FloatTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != default) hashCode.Add(this.{elementInfo.Name}.GetHashCode());");
                                break;
                            case StringTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != default) hashCode.Add(this.{elementInfo.Name}.GetHashCode());");
                                break;
                            case TimestampTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != default) hashCode.Add(this.{elementInfo.Name}.GetHashCode());");
                                break;
                            case MemoryTypeInfo typeInfo when (typeInfo.IsUseMemoryPool):
                                w.WriteLine($"if (!this.{elementInfo.Name}.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.{elementInfo.Name}.Span));");
                                break;
                            case MemoryTypeInfo typeInfo when (!typeInfo.IsUseMemoryPool):
                                w.WriteLine($"if (!this.{elementInfo.Name}.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.{elementInfo.Name}.Span));");
                                break;
                            case ListTypeInfo typeInfo:
                                {
                                    w.WriteLine($"foreach (var n in {GetFieldName(elementInfo.Name)})");
                                    w.WriteLine("{");

                                    w.PushIndent();

                                    {
                                        w.WriteLine($"if (n != default) hashCode.Add(ObjectHelper.GetHashCode(n));");
                                    }

                                    w.PopIndent();

                                    w.WriteLine("}");
                                }
                                break;
                            case MapTypeInfo typeInfo:
                                {
                                    w.WriteLine($"foreach (var n in {GetFieldName(elementInfo.Name)})");
                                    w.WriteLine("{");

                                    w.PushIndent();

                                    {
                                        w.WriteLine($"if (n.Key != default) h ^= hashCode.Add(ObjectHelper.GetHashCode(n.Key));");
                                        w.WriteLine($"if (n.Value != default) hashCode.Add(ObjectHelper.GetHashCode(n.Value));");
                                    }

                                    w.PopIndent();

                                    w.WriteLine("}");
                                }
                                break;
                            case CustomTypeInfo typeInfo:
                                w.WriteLine($"if (this.{elementInfo.Name} != default) hashCode.Add(this.{elementInfo.Name}.GetHashCode());");
                                break;
                        }
                    }

                    w.WriteLine("_hashCode = hashCode.ToHashCode();");
                }

                w.PopIndent();

                w.WriteLine("}");
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

            private bool Try_Write_Constructor_CheckNull(CodeWriter w, string name, TypeInfo typeInfo)
            {
                switch (typeInfo)
                {
                    case StringTypeInfo stringTypeInfo when (typeInfo.IsNullable):
                        w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                        return true;
                    case MemoryTypeInfo memoryTypeInfo when (memoryTypeInfo.IsUseMemoryPool):
                        w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                        return true;
                    case ListTypeInfo listTypeInfo when (typeInfo.IsNullable):
                        w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                        return true;
                    case MapTypeInfo mapTypeInfo when (typeInfo.IsNullable):
                        w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                        return true;
                    case CustomTypeInfo customTypeInfo when (typeInfo.IsNullable):
                        w.WriteLine($"if ({name} is null) throw new ArgumentNullException(\"{name}\");");
                        return true;
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
                                w.WriteLine($"if (!(this.{elementInfo.Name} is null) && !(target.{elementInfo.Name} is null) && !CollectionHelper.Equals(this.{elementInfo.Name}.Span, target.{elementInfo.Name}.Span)) return false;");
                                break;
                            case MapTypeInfo typeInfo:
                                w.WriteLine($"if ((this.{elementInfo.Name} is null) != (target.{elementInfo.Name} is null)) return false;");
                                w.WriteLine($"if (!(this.{elementInfo.Name} is null) && !(target.{elementInfo.Name} is null) && !CollectionHelper.Equals(this.{elementInfo.Name}.Span, target.{elementInfo.Name}.Span)) return false;");
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

                        w.WriteLine($"w.Write((ulong){elementInfo.Id});");
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

                    w.WriteLine("while (r.Available > 0)");
                    w.WriteLine("{");

                    w.PushIndent();

                    w.WriteLine("int id = (int)r.GetUInt64();");
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
                    case IntTypeInfo intTypeInfo when (intTypeInfo.IsSigned):
                        w.WriteLine($"w.Write((long){name});");
                        break;
                    case IntTypeInfo intTypeInfo when (!intTypeInfo.IsSigned):
                        w.WriteLine($"w.Write((ulong){name});");
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
                        switch (_customTypeResolver.Invoke(customTypeInfo))
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
                    case IntTypeInfo intTypeInfo when (intTypeInfo.IsSigned):
                        w.WriteLine($"{name} = ({this.GetParameterTypeString(intTypeInfo)})r.GetInt64();");
                        break;
                    case IntTypeInfo intTypeInfo when (!intTypeInfo.IsSigned):
                        w.WriteLine($"{name} = ({this.GetParameterTypeString(intTypeInfo)})r.GetUInt64();");
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
                            w.WriteLine("var length = (int)r.GetUInt64();");
                            w.WriteLine($"var t_array = new {listTypeInfo.ElementType}[length];");

                            w.WriteLine("for (int i = 0; i < t_array.Length; i++)");
                            w.WriteLine("{");

                            w.PushIndent();

                            this.Write_Formatter_Deserialize_PropertyDef(w, "t_array[i]", listTypeInfo.ElementType, rank + 1);

                            w.PopIndent();

                            w.WriteLine("}");

                            w.WriteLine($"{name} = new List<{listTypeInfo.ElementType}>(t_array);");
                        }
                        break;
                    case MapTypeInfo mapTypeInfo:
                        {
                            w.WriteLine("var length = (int)r.GetUInt64();");
                            w.WriteLine($"var t_map = new Dictionary<{mapTypeInfo.KeyType}, {mapTypeInfo.ValueType}>();");

                            w.WriteLine("for (int i = 0; i < t_array.Length; i++)");
                            w.WriteLine("{");

                            w.PushIndent();

                            this.Write_Formatter_Deserialize_PropertyDef(w, "t_array[i]", mapTypeInfo.KeyType, rank + 1);

                            w.PopIndent();

                            w.WriteLine("}");

                            w.WriteLine($"{name} = new List<{mapTypeInfo.KeyType}>(t_array);");
                        }
                        break;
                    case CustomTypeInfo customTypeInfo:
                        switch (_customTypeResolver.Invoke(customTypeInfo))
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
