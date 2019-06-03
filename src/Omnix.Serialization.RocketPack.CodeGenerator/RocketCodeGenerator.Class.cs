using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static partial class RocketCodeGenerator
    {
        /// <summary>
        /// <see cref="MessageDefinition"/>からclassを生成します。
        /// </summary>
        class ClassWriter
        {
            private readonly RocketPackDefinition _rocketFormatDefinition;
            private readonly IList<RocketPackDefinition> _externalRocketPackDefinitions;
            private readonly string _accessLevel;

            public ClassWriter(RocketPackDefinition rocketPackDefinition, IEnumerable<RocketPackDefinition> externalRocketPackDefinitions)
            {
                _rocketFormatDefinition = rocketPackDefinition;
                _externalRocketPackDefinitions = externalRocketPackDefinitions.ToList();

                var accessLevelOption = _rocketFormatDefinition.Options.FirstOrDefault(n => n.Name == "csharp_access_level");
                _accessLevel = accessLevelOption?.Value ?? "public";
            }

            private object? CustomTypeResolver(CustomType n)
            {
                foreach (var targetInfo in new[] { _rocketFormatDefinition }.Union(_externalRocketPackDefinitions))
                {
                    var enumInfo = targetInfo.Enums.FirstOrDefault(m => m.Name == n.TypeName);
                    if (enumInfo != null) return enumInfo;

                    var messageInfo = targetInfo.Messages.FirstOrDefault(m => m.Name == n.TypeName);
                    if (messageInfo != null) return messageInfo;
                }

                return null;
            }

            /// <summary>
            /// <see cref="TypeBase"/>から型を生成します。
            /// </summary>
            private string GetParameterTypeString(TypeBase typeBase)
            {
                switch (typeBase)
                {
                    case BoolType type:
                        return "bool" + (type.IsOptional ? "?" : "");
                    case IntType type when (!type.IsSigned && type.Size == 8):
                        return "byte" + (type.IsOptional ? "?" : "");
                    case IntType type when (!type.IsSigned && type.Size == 16):
                        return "ushort" + (type.IsOptional ? "?" : "");
                    case IntType type when (!type.IsSigned && type.Size == 32):
                        return "uint" + (type.IsOptional ? "?" : "");
                    case IntType type when (!type.IsSigned && type.Size == 64):
                        return "ulong" + (type.IsOptional ? "?" : "");
                    case IntType type when (type.IsSigned && type.Size == 8):
                        return "sbyte" + (type.IsOptional ? "?" : "");
                    case IntType type when (type.IsSigned && type.Size == 16):
                        return "short" + (type.IsOptional ? "?" : "");
                    case IntType type when (type.IsSigned && type.Size == 32):
                        return "int" + (type.IsOptional ? "?" : "");
                    case IntType type when (type.IsSigned && type.Size == 64):
                        return "long" + (type.IsOptional ? "?" : "");
                    case FloatType type when (type.Size == 32):
                        return "float" + (type.IsOptional ? "?" : "");
                    case FloatType type when (type.Size == 64):
                        return "double" + (type.IsOptional ? "?" : "");
                    case StringType type:
                        return "string" + (type.IsOptional ? "?" : "");
                    case TimestampType type:
                        return GetFullName("Timestamp") + (type.IsOptional ? "?" : "");
                    case MemoryType type when (type.IsUseMemoryPool):
                        return GetFullName("IMemoryOwner<>", "byte") + (type.IsOptional ? "?" : "");
                    case MemoryType type when (!type.IsUseMemoryPool):
                        return GetFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : "");
                    case ListType type:
                        return $"{this.GetParameterTypeString(type.ElementType)}[]" + (type.IsOptional ? "?" : ""); 
                    case MapType type:
                        return GetFullName("Dictionary<,>", this.GetParameterTypeString(type.KeyType), this.GetParameterTypeString(type.ValueType)) + (type.IsOptional ? "?" : ""); 
                    case CustomType type:
                        {
                            switch (this.CustomTypeResolver(type))
                            {
                                case EnumDefinition _:
                                    return type.TypeName + (type.IsOptional ? "?" : "");
                                case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                    return type.TypeName + (type.IsOptional ? "?" : "");
                                case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                    return type.TypeName + (type.IsOptional ? "?" : "");
                                default:
                                    throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(type));
                            }
                        }
                    default:
                        throw new ArgumentException($"Type \"{typeBase.GetType().Name}\" was not found", nameof(typeBase));
                }
            }

            /// <summary>
            /// <see cref="TypeBase"/>から型を生成します。
            /// </summary>
            private string GetPropertyTypeString(TypeBase typeBase)
            {
                switch (typeBase)
                {
                    case BoolType type:
                        return "bool" + (type.IsOptional ? "?" : "");
                    case IntType type when (!type.IsSigned && type.Size == 8):
                        return "byte" + (type.IsOptional ? "?" : "");
                    case IntType type when (!type.IsSigned && type.Size == 16):
                        return "ushort" + (type.IsOptional ? "?" : "");
                    case IntType type when (!type.IsSigned && type.Size == 32):
                        return "uint" + (type.IsOptional ? "?" : "");
                    case IntType type when (!type.IsSigned && type.Size == 64):
                        return "ulong" + (type.IsOptional ? "?" : "");
                    case IntType type when (type.IsSigned && type.Size == 8):
                        return "sbyte" + (type.IsOptional ? "?" : "");
                    case IntType type when (type.IsSigned && type.Size == 16):
                        return "short" + (type.IsOptional ? "?" : "");
                    case IntType type when (type.IsSigned && type.Size == 32):
                        return "int" + (type.IsOptional ? "?" : "");
                    case IntType type when (type.IsSigned && type.Size == 64):
                        return "long" + (type.IsOptional ? "?" : "");
                    case FloatType type when (type.Size == 32):
                        return "float" + (type.IsOptional ? "?" : "");
                    case FloatType type when (type.Size == 64):
                        return "double" + (type.IsOptional ? "?" : "");
                    case StringType type:
                        return "string" + (type.IsOptional ? "?" : "");
                    case TimestampType type:
                        return GetFullName("Timestamp") + (type.IsOptional ? "?" : "");
                    case MemoryType type when (!type.IsUseMemoryPool):
                        return GetFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : "");
                    case MemoryType type when (type.IsUseMemoryPool):
                        return GetFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : "");
                    case ListType type:
                        return GetFullName("ReadOnlyListSlim<>", this.GetParameterTypeString(type.ElementType)) + (type.IsOptional ? "?" : "");
                    case MapType type:
                        return GetFullName("ReadOnlyDictionarySlim<,>", this.GetParameterTypeString(type.KeyType), this.GetParameterTypeString(type.ValueType)) + (type.IsOptional ? "?" : "");
                    case CustomType type:
                        {
                            switch (this.CustomTypeResolver(type))
                            {
                                case EnumDefinition _:
                                    return type.TypeName + (type.IsOptional ? "?" : "");
                                case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                    return type.TypeName + (type.IsOptional ? "?" : "");
                                case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                    return type.TypeName + (type.IsOptional ? "?" : "");
                                default:
                                    throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(type));
                            }
                        }
                    default:
                        throw new ArgumentException($"Type \"{typeBase.GetType().Name}\" was not found", nameof(typeBase));
                }
            }

            private string GetDefaultValueString(TypeBase typeBase)
            {
                switch (typeBase)
                {
                    case BoolType type:
                        if (!type.IsOptional) return "false";
                        else return "null";
                    case IntType type:
                        if (!type.IsOptional) return "0";
                        else return "null";
                    case FloatType type when (type.Size == 32):
                        if (!type.IsOptional) return "0.0F";
                        else return "null";
                    case FloatType type when (type.Size == 64):
                        if (!type.IsOptional) return "0.0D";
                        else return "null";
                    case StringType type:
                        if (!type.IsOptional) return "string.Empty";
                        else return "null";
                    case TimestampType type:
                        if (!type.IsOptional) return GetFullName("Timestamp") + ".Zero";
                        else return "null";
                    case MemoryType type when (!type.IsUseMemoryPool):
                        if (!type.IsOptional) return GetFullName("ReadOnlyMemory<>", "byte") + ".Empty";
                        else return "null";
                    case MemoryType type when (type.IsUseMemoryPool):
                        if (!type.IsOptional) return GetFullName("MemoryOwner<>", "byte") + ".Empty";
                        else return "null";
                    case ListType type:
                        if (!type.IsOptional) return GetFullName("Array") + ".Empty<" + this.GetParameterTypeString(type.ElementType) + ">()";
                        else return "null";
                    case MapType type:
                        if (!type.IsOptional) return "new " + GetFullName("Dictionary<,>", this.GetParameterTypeString(type.KeyType), this.GetParameterTypeString(type.ValueType)) + "()";
                        else return "null";
                    case CustomType type:
                        {
                            switch (this.CustomTypeResolver(type))
                            {
                                case EnumDefinition elementEnumDefinition:
                                    if (!type.IsOptional) return $"({elementEnumDefinition.Name})0";
                                    else return "null";
                                case MessageDefinition elementMessageDefinition:
                                    if (!type.IsOptional) return $"{elementMessageDefinition.Name}.Empty";
                                    else return "null";
                                default:
                                    throw new ArgumentException($"Type \"{type.TypeName}\" was not found", nameof(typeBase));
                            }
                        }
                    default:
                        throw new ArgumentException($"Type \"{typeBase.GetType().Name}\" was not found", nameof(typeBase));
                }
            }

            /// <summary>
            /// クラスの生成。
            /// </summary>
            public void Write(CodeWriter w, MessageDefinition messageDefinition)
            {
                if (messageDefinition.Elements.Select(n => n.Type).OfType<MemoryType>().Any(n => n.IsUseMemoryPool))
                {
                    w.WriteLine($"{_accessLevel} sealed partial class {messageDefinition.Name} : {GetFullName("RocketPackMessageBase<>", messageDefinition.Name)}, {GetFullName("IDisposable")}");
                }
                else
                {
                    w.WriteLine($"{_accessLevel} sealed partial class {messageDefinition.Name} : {GetFullName("RocketPackMessageBase<>", messageDefinition.Name)}");
                }

                w.WriteLine("{");

                w.PushIndent();

                this.Write_StaticConstructor(w, messageDefinition);
                w.WriteLine();

                this.Write_Constructor(w, messageDefinition);
                w.WriteLine();

                this.Write_Properties(w, messageDefinition);
                w.WriteLine();

                this.Write_Equals(w, messageDefinition);
                w.WriteLine();

                this.Write_GetHashCode(w, messageDefinition);
                w.WriteLine();

                if (messageDefinition.Elements.Select(n => n.Type).OfType<MemoryType>().Any(n => n.IsUseMemoryPool))
                {
                    this.Write_Dispose(w, messageDefinition);
                    w.WriteLine();
                }

                this.Write_Formatter(w, messageDefinition);

                w.PopIndent();

                w.WriteLine("}");
            }

            /// <summary>
            /// 静的コンストラクタの生成。
            /// </summary>
            private void Write_StaticConstructor(CodeWriter w, MessageDefinition messageDefinition)
            {
                w.WriteLine($"static {messageDefinition.Name}()");
                w.WriteLine("{");

                w.PushIndent();

                // CustomFormatterのインスタンスの作成
                this.Write_StaticConstructor_CustomFormatterProperty(w, messageDefinition);

                // Defaultのインスタンスの作成
                this.Write_StaticConstructor_DefaultProperty(w, messageDefinition);

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_StaticConstructor_CustomFormatterProperty(CodeWriter w, MessageDefinition messageDefinition)
            {
                w.WriteLine($"{messageDefinition.Name}.Formatter = new CustomFormatter();");
            }

            private void Write_StaticConstructor_DefaultProperty(CodeWriter w, MessageDefinition messageDefinition)
            {
                var parameters = new List<string>();

                foreach (var element in messageDefinition.Elements)
                {
                    parameters.Add(GetDefaultValueString(element.Type));
                }

                w.WriteLine($"{messageDefinition.Name}.Empty = new {messageDefinition.Name}({string.Join(", ", parameters)});");
            }

            private const string HashCodeName = "__hashCode";

            /// <summary>
            /// コンストラクタの生成。
            /// </summary>
            private void Write_Constructor(CodeWriter w, MessageDefinition messageDefinition)
            {
                w.WriteLine($"private readonly int {HashCodeName};");
                w.WriteLine();

                // 最大サイズの宣言。
                this.Write_Constructor_Define_MaxLength(w, messageDefinition);

                // パラメータの生成。
                w.WriteLine($"public {messageDefinition.Name}({string.Join(", ", messageDefinition.Elements.Select(element => this.GetParameterTypeString(element.Type) + " " + GetFieldName(element.Name)))})");
                w.WriteLine("{");

                w.PushIndent();

                // パラメータのチェック
                this.Write_Constructor_Parameter_Check(w, messageDefinition);

                // 初期化
                this.Write_Constructor_Init(w, messageDefinition);

                w.WriteLine();

                // HashCodeの値の算出
                this.Write_Constructor_HashCode(w, messageDefinition);

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_Constructor_Init(CodeWriter w, MessageDefinition messageDefinition)
            {
                foreach (var elementInfo in messageDefinition.Elements)
                {
                    switch (elementInfo.Type)
                    {
                        case MemoryType type when (type.IsUseMemoryPool):
                            w.WriteLine($"_{GetFieldName(elementInfo.Name)} = {GetFieldName(elementInfo.Name)};");
                            break;
                        case ListType type:
                            if (type.IsOptional)
                            {
                                w.WriteLine($"if({GetFieldName(elementInfo.Name)} != null)");
                                w.WriteLine("{");
                                w.PushIndent();
                            }

                            w.WriteLine($"this.{elementInfo.Name} = new {GetFullName("ReadOnlyListSlim<>", this.GetParameterTypeString(type.ElementType))}({GetFieldName(elementInfo.Name)});");

                            if (type.IsOptional)
                            {
                                w.PopIndent();
                                w.WriteLine("}");
                            }
                            break;
                        case MapType type:
                            if (type.IsOptional)
                            {
                                w.WriteLine($"if({GetFieldName(elementInfo.Name)} != null)");
                                w.WriteLine("{");
                                w.PushIndent();
                            }

                            w.WriteLine($"this.{elementInfo.Name} = new {GetFullName("ReadOnlyDictionarySlim<,>", this.GetParameterTypeString(type.KeyType), this.GetParameterTypeString(type.ValueType))}({GetFieldName(elementInfo.Name)});");

                            if (type.IsOptional)
                            {
                                w.PopIndent();
                                w.WriteLine("}");
                            }
                            break;
                        default:
                            w.WriteLine($"this.{elementInfo.Name} = {GetFieldName(elementInfo.Name)};");
                            break;
                    }
                }
            }

            private void Write_Constructor_Define_MaxLength(CodeWriter w, MessageDefinition messageDefinition)
            {
                bool isDefinedMaxLength = false;

                foreach (var elementInfo in messageDefinition.Elements)
                {
                    isDefinedMaxLength |= this.Try_Write_Constructor_Define_MaxLength_Element(w, elementInfo.Name, elementInfo.Type);
                }

                if (isDefinedMaxLength) w.WriteLine();
            }

            private bool Try_Write_Constructor_Define_MaxLength_Element(CodeWriter w, string name, TypeBase type)
            {
                switch (type)
                {
                    case StringType stringType:
                        w.WriteLine($"public static readonly int Max{name}Length = {stringType.MaxLength};");
                        return true;
                    case MemoryType memoryType:
                        w.WriteLine($"public static readonly int Max{name}Length = {memoryType.MaxLength};");
                        return true;
                    case ListType listType:
                        w.WriteLine($"public static readonly int Max{name}Count = {listType.MaxLength};");
                        return true;
                    case MapType mapType:
                        w.WriteLine($"public static readonly int Max{name}Count = {mapType.MaxLength};");
                        return true;
                    default:
                        return false;
                }
            }

            private void Write_Constructor_Parameter_Check(CodeWriter w, MessageDefinition messageInfo)
            {
                bool isChecked = false;

                foreach (var elementInfo in messageInfo.Elements)
                {
                    isChecked = this.Try_Write_Constructor_Parameter_Check_Element(w, elementInfo);
                }

                if (isChecked) w.WriteLine();
            }

            private bool Try_Write_Constructor_Parameter_Check_Element(CodeWriter w, MessageElement elementInfo)
            {
                bool isChecked = false;

                isChecked |= this.Try_Write_Constructor_Parameter_Check_Element_Null(w, GetFieldName(elementInfo.Name), elementInfo.Type);
                isChecked |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(w, GetFieldName(elementInfo.Name), elementInfo.Type);

                if (elementInfo.Type is ListType listType)
                {
                    var w2 = new CodeWriter();
                    bool isChecked2 = false;

                    if (listType.IsOptional)
                    {
                        w2.WriteLine($"if (!({GetFieldName(elementInfo.Name)} is null))");
                        w2.WriteLine("{");

                        w2.PushIndent();
                    }

                    w2.WriteLine($"foreach (var n in {GetFieldName(elementInfo.Name)})");
                    w2.WriteLine("{");

                    w2.PushIndent();

                    {
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_Null(w2, "n", listType.ElementType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(w2, "n", listType.ElementType);
                    }

                    w2.PopIndent();

                    w2.WriteLine("}");

                    if (listType.IsOptional)
                    {
                        w2.PopIndent();

                        w2.WriteLine("}");
                    }

                    if (isChecked2) w.WriteLine(w2.ToString().TrimEnd());

                    isChecked |= isChecked2;
                }
                else if (elementInfo.Type is MapType mapType)
                {
                    var w2 = new CodeWriter();
                    bool isChecked2 = false;

                    if (mapType.IsOptional)
                    {
                        w2.WriteLine($"if (!({GetFieldName(elementInfo.Name)} is null))");
                        w2.WriteLine("{");

                        w2.PushIndent();
                    }

                    w2.WriteLine($"foreach (var n in {GetFieldName(elementInfo.Name)})");
                    w2.WriteLine("{");

                    w2.PushIndent();

                    {
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_Null(w2, "n.Key", mapType.KeyType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_Null(w2, "n.Value", mapType.ValueType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(w2, "n.Key", mapType.KeyType);
                        isChecked2 |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(w2, "n.Value", mapType.ValueType);
                    }

                    w2.PopIndent();

                    w2.WriteLine("}");

                    if (mapType.IsOptional)
                    {
                        w2.PopIndent();

                        w2.WriteLine("}");
                    }

                    if (isChecked2) w.WriteLine(w2.ToString().TrimEnd());

                    isChecked |= isChecked2;
                }

                return isChecked;
            }

            private bool Try_Write_Constructor_Parameter_Check_Element_Null(CodeWriter w, string name, TypeBase type)
            {
                switch (type)
                {
                    case StringType stringType when (!type.IsOptional):
                        w.WriteLine($"if ({name} is null) throw new {GetFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case MemoryType memoryType when (!type.IsOptional && memoryType.IsUseMemoryPool):
                        w.WriteLine($"if ({name} is null) throw new {GetFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case ListType listType when (!type.IsOptional):
                        w.WriteLine($"if ({name} is null) throw new {GetFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case MapType mapType when (!type.IsOptional):
                        w.WriteLine($"if ({name} is null) throw new {GetFullName("ArgumentNullException")}(\"{name}\");");
                        return true;
                    case CustomType customType when (!type.IsOptional):
                        switch (this.CustomTypeResolver(customType))
                        {
                            case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                w.WriteLine($"if ({name} is null) throw new {GetFullName("ArgumentNullException")}(\"{name}\");");
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
                    case MemoryType memoryType when (!memoryType.IsUseMemoryPool && !memoryType.IsOptional):
                        property = "Length";
                        maxLength = memoryType.MaxLength;
                        break;
                    case MemoryType memoryType when (!memoryType.IsUseMemoryPool && memoryType.IsOptional):
                        property = "Value.Length";
                        maxLength = memoryType.MaxLength;
                        break;
                    case MemoryType memoryType when (memoryType.IsUseMemoryPool):
                        property = "Memory.Length";
                        maxLength = memoryType.MaxLength;
                        break;
                    case ListType listType:
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
                    w.WriteLine($"if (!({name} is null) && {name}.{property} > {maxLength}) throw new {GetFullName("ArgumentOutOfRangeException")}(\"{name}\");");
                }
                else
                {
                    w.WriteLine($"if ({name}.{property} > {maxLength}) throw new {GetFullName("ArgumentOutOfRangeException")}(\"{name}\");");
                }

                return true;
            }

            private void Write_Constructor_HashCode(CodeWriter w, MessageDefinition messageInfo)
            {
                w.WriteLine("{");

                w.PushIndent();

                var variableName = "__h";

                {
                    w.WriteLine($"var {variableName} = new {GetFullName("HashCode")}();");

                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        this.Write_Constructor_HashCode_Element(w, variableName, "this." + elementInfo.Name, elementInfo.Type);
                    }

                    w.WriteLine($"{HashCodeName} = {variableName}.ToHashCode();");
                }

                w.PopIndent();

                w.WriteLine("}");
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
                    case MemoryType memoryType when (!memoryType.IsOptional):
                        w.WriteLine($"if (!{parameterName}.IsEmpty) {hashCodeName}.Add({GetFullName("ObjectHelper")}.GetHashCode({parameterName}.Span));");
                        break;
                    case MemoryType memoryType when (memoryType.IsOptional):
                        w.WriteLine($"if (!({parameterName} is null) && !{parameterName}.Value.IsEmpty) {hashCodeName}.Add({GetFullName("ObjectHelper")}.GetHashCode({parameterName}.Value.Span));");
                        break;
                    case ListType listType:
                        {
                            if (type.IsOptional)
                            {
                                w.WriteLine($"if({GetFieldName(parameterName)} != null)");
                                w.WriteLine("{");
                                w.PushIndent();
                            }

                            w.WriteLine($"foreach (var n in {GetFieldName(parameterName)})");
                            w.WriteLine("{");

                            w.PushIndent();

                            {
                                this.Write_Constructor_HashCode_Element(w, hashCodeName, "n", listType.ElementType);
                            }

                            w.PopIndent();

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
                                w.WriteLine($"if({GetFieldName(parameterName)} != null)");
                                w.WriteLine("{");
                                w.PushIndent();
                            }

                            w.WriteLine($"foreach (var n in {GetFieldName(parameterName)})");
                            w.WriteLine("{");

                            w.PushIndent();

                            {
                                this.Write_Constructor_HashCode_Element(w, hashCodeName, "n.Key", mapType.KeyType);
                                this.Write_Constructor_HashCode_Element(w, hashCodeName, "n.Value", mapType.ValueType);
                            }

                            w.PopIndent();

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
                                case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                    w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                                    break;
                                case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                    w.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                                    break;
                            }
                        }
                        break;
                }
            }

            private void Write_Equals(CodeWriter w, MessageDefinition messageDefinition)
            {
                w.WriteLine($"public override bool Equals({messageDefinition.Name}? target)");
                w.WriteLine("{");

                w.PushIndent();

                {
                    w.WriteLine("if (target is null) return false;");
                    w.WriteLine("if (object.ReferenceEquals(this, target)) return true;");

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
                            case MemoryType type:
                                if (!type.IsOptional)
                                {
                                    w.WriteLine($"if (!{GetFullName("BytesOperations")}.SequenceEqual(this.{element.Name}.Span, target.{element.Name}.Span)) return false;");
                                }
                                else
                                {
                                    w.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                    w.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && !{GetFullName("BytesOperations")}.SequenceEqual(this.{element.Name}.Value.Span, target.{element.Name}.Value.Span)) return false;");
                                }
                                break;
                            case ListType type:
                                if (!type.IsOptional)
                                {
                                    w.WriteLine($"if (!{GetFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                else
                                {
                                    w.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                    w.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && !{GetFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                break;
                            case MapType type:
                                if (!type.IsOptional)
                                {
                                    w.WriteLine($"if (!{GetFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                else
                                {
                                    w.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                    w.WriteLine($"if (!(this.{element.Name} is null) && !(target.{element.Name} is null) && !{GetFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                                }
                                break;
                            case CustomType type:
                                {
                                    switch (this.CustomTypeResolver(type))
                                    {
                                        case EnumDefinition enumInfo:
                                            w.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                            break;
                                        case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
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
                                        case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
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
                }

                w.WriteLine();

                w.WriteLine("return true;");

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_GetHashCode(CodeWriter w, MessageDefinition messageInfo)
            {
                w.WriteLine("public override int GetHashCode() => __hashCode;");
            }

            private void Write_Properties(CodeWriter w, MessageDefinition messageInfo)
            {
                foreach (var element in messageInfo.Elements.OrderBy(n => n.Id))
                {
                    switch (element.Type)
                    {
                        case MemoryType type when (type.IsUseMemoryPool):
                            if (!type.IsOptional)
                            {
                                w.WriteLine($"private readonly {this.GetParameterTypeString(element.Type)} _{GetFieldName(element.Name)};");
                                w.WriteLine($"public {this.GetPropertyTypeString(element.Type)} {element.Name} => _{GetFieldName(element.Name)}.Memory;");
                            }
                            else
                            {
                                w.WriteLine($"private readonly {this.GetParameterTypeString(element.Type)} _{GetFieldName(element.Name)};");
                                w.WriteLine($"public {this.GetPropertyTypeString(element.Type)} {element.Name} => _{GetFieldName(element.Name)}?.Memory;");
                            }
                            break;
                        default:
                            w.WriteLine($"public {this.GetPropertyTypeString(element.Type)} {element.Name} {{ get; }}");
                            break;
                    }
                }
            }

            private void Write_Dispose(CodeWriter w, MessageDefinition messageInfo)
            {
                w.WriteLine("public void Dispose()");
                w.WriteLine("{");

                w.PushIndent();

                foreach (var element in messageInfo.Elements)
                {
                    switch (element.Type)
                    {
                        case MemoryType type when (type.IsUseMemoryPool):
                            w.WriteLine($"_{GetFieldName(element.Name)}?.Dispose();");
                            break;
                    }
                }

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_Formatter(CodeWriter w, MessageDefinition messageFormat)
            {
                w.WriteLine($"private sealed class CustomFormatter : {GetFullName("IRocketPackFormatter<>", messageFormat.Name)}");
                w.WriteLine("{");

                w.PushIndent();

                {
                    w.WriteLine($"public void Serialize({GetFullName("RocketPackWriter")} w, {messageFormat.Name} value, int rank)");
                    w.WriteLine("{");

                    w.PushIndent();

                    w.WriteLine($"if (rank > 256) throw new {GetFullName("FormatException")}();");

                    w.WriteLine();

                    {
                        w.WriteLine("{");

                        w.PushIndent();

                        w.WriteLine("uint propertyCount = 0;");

                        foreach (var element in messageFormat.Elements)
                        {
                            this.BlockWhoseValueIsNotDefault(w, element, () =>
                            {
                                w.WriteLine("propertyCount++;");
                            });
                        }

                        w.WriteLine($"w.Write(propertyCount);");

                        w.PopIndent();

                        w.WriteLine("}");
                    }

                    w.WriteLine();

                    foreach (var element in messageFormat.Elements)
                    {
                        this.BlockWhoseValueIsNotDefault(w, element, () =>
                        {
                            w.WriteLine($"w.Write((uint){element.Id});");
                            this.Write_Formatter_Serialize_PropertyDef(w, "value." + element.Name, element.Type, 0);
                        });
                    }

                    w.PopIndent();

                    w.WriteLine("}");
                }

                w.WriteLine();

                {
                    w.WriteLine($"public {messageFormat.Name} Deserialize({GetFullName("RocketPackReader")} r, int rank)");
                    w.WriteLine("{");

                    w.PushIndent();

                    w.WriteLine($"if (rank > 256) throw new {GetFullName("FormatException")}();");

                    w.WriteLine();

                    w.WriteLine("uint propertyCount = r.GetUInt32();");

                    w.WriteLine();

                    foreach (var elementInfo in messageFormat.Elements)
                    {
                        w.WriteLine($"{this.GetParameterTypeString(elementInfo.Type)} p_{GetFieldName(elementInfo.Name)} = {GetDefaultValueString(elementInfo.Type)};");
                    }

                    w.WriteLine();

                    w.WriteLine("for (; propertyCount > 0; propertyCount--)");
                    w.WriteLine("{");

                    w.PushIndent();

                    w.WriteLine("uint id = r.GetUInt32();");
                    w.WriteLine("switch (id)");
                    w.WriteLine("{");

                    w.PushIndent();

                    foreach (var elementInfo in messageFormat.Elements)
                    {
                        w.WriteLine($"case {elementInfo.Id}:");

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

                    w.WriteLine($"return new {messageFormat.Name}({ string.Join(", ", messageFormat.Elements.Select(n => "p_" + GetFieldName(n.Name)))});");

                    w.PopIndent();

                    w.WriteLine("}");
                }

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_Formatter_Serialize_PropertyDef(CodeWriter w, string name, TypeBase type, int rank)
            {
                switch (type)
                {
                    case BoolType boolType:
                        if (!type.IsOptional) w.WriteLine($"w.Write({name});");
                        else w.WriteLine($"w.Write({name}.Value);");
                        break;
                    case IntType inttype:
                        if (!type.IsOptional) w.WriteLine($"w.Write({name});");
                        else w.WriteLine($"w.Write({name}.Value);");
                        break;
                    case FloatType floatType:
                        if (!type.IsOptional) w.WriteLine($"w.Write({name});");
                        else w.WriteLine($"w.Write({name}.Value);");
                        break;
                    case StringType stringType:
                        w.WriteLine($"w.Write({name});");
                        break;
                    case TimestampType timestampType:
                        if (!type.IsOptional) w.WriteLine($"w.Write({name});");
                        else w.WriteLine($"w.Write({name}.Value);");
                        break;
                    case MemoryType memoryType when (memoryType.IsUseMemoryPool):
                        if (!type.IsOptional) w.WriteLine($"w.Write({name}.Span);");
                        else w.WriteLine($"w.Write({name}.Value.Span);");
                        break;
                    case MemoryType memoryType when (!memoryType.IsUseMemoryPool):
                        if (!type.IsOptional) w.WriteLine($"w.Write({name}.Span);");
                        else w.WriteLine($"w.Write({name}.Value.Span);");
                        break;
                    case ListType listType:
                        {
                            w.WriteLine($"w.Write((uint){name}.Count);");
                            w.WriteLine($"foreach (var n in {name})");
                            w.WriteLine("{");

                            w.PushIndent();

                            {
                                this.Write_Formatter_Serialize_PropertyDef(w, "n", listType.ElementType, rank + 1);
                            }

                            w.PopIndent();

                            w.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            w.WriteLine($"w.Write((uint){name}.Count);");
                            w.WriteLine($"foreach (var n in {name})");
                            w.WriteLine("{");

                            w.PushIndent();

                            {
                                this.Write_Formatter_Serialize_PropertyDef(w, "n.Key", mapType.KeyType, rank + 1);
                                this.Write_Formatter_Serialize_PropertyDef(w, "n.Value", mapType.ValueType, rank + 1);
                            }

                            w.PopIndent();

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
                                        if (!intType.IsOptional) w.WriteLine($"w.Write((long){name});");
                                        else w.WriteLine($"w.Write((long){name}.Value);");
                                        break;
                                    case IntType intType when (!intType.IsSigned):
                                        if (!intType.IsOptional) w.WriteLine($"w.Write((ulong){name});");
                                        else w.WriteLine($"w.Write((ulong){name}.Value);");
                                        break;
                                }
                                break;
                            case MessageDefinition messageDefinition when (messageDefinition.FormatType == MessageFormatType.Medium):
                                w.WriteLine($"{messageDefinition.Name}.Formatter.Serialize(w, {name}, rank + 1);");
                                break;
                            case MessageDefinition messageDefinition when (messageDefinition.FormatType == MessageFormatType.Small):
                                if (!type.IsOptional)
                                {
                                    w.WriteLine($"{messageDefinition.Name}.Formatter.Serialize(w, {name}, rank + 1);");
                                }
                                else
                                {
                                    w.WriteLine($"{messageDefinition.Name}.Formatter.Serialize(w, {name}.Value, rank + 1);");
                                }
                                break;
                        }
                        break;
                }
            }

            private void BlockWhoseValueIsNotDefault(CodeWriter w, MessageElement messageElement, Action callback)
            {
                var sb = new StringBuilder();
                sb.Append($"if (");

                switch (messageElement.Type)
                {
                    case BoolType type:
                        if (!type.IsOptional) sb.Append($"value.{messageElement.Name} != false)");
                        else sb.Append($"value.{messageElement.Name} != null)");
                        break;
                    case IntType type:
                        if (!type.IsOptional) sb.Append($"value.{messageElement.Name} != 0)");
                        else sb.Append($"value.{messageElement.Name} != null)");
                        break;
                    case FloatType type when (type.Size == 32):
                        if (!type.IsOptional) sb.Append($"value.{messageElement.Name} != 0.0F)");
                        else sb.Append($"value.{messageElement.Name} != null)");
                        break;
                    case FloatType type when (type.Size == 64):
                        if (!type.IsOptional) sb.Append($"value.{messageElement.Name} != 0.0D)");
                        else sb.Append($"value.{messageElement.Name} != null)");
                        break;
                    case StringType type:
                        if (!type.IsOptional) sb.Append($"value.{messageElement.Name} != string.Empty)");
                        else sb.Append($"value.{messageElement.Name} != null)");
                        break;
                    case TimestampType type:
                        if (!type.IsOptional) sb.Append($"value.{messageElement.Name} != {GetFullName("Timestamp")}.Zero)");
                        else sb.Append($"value.{messageElement.Name} != null)");
                        break;
                    case MemoryType type:
                        if (!type.IsOptional) sb.Append($"!value.{messageElement.Name}.IsEmpty)");
                        else sb.Append($"value.{messageElement.Name} != null)");
                        break;
                    case ListType type:
                        if (!type.IsOptional) sb.Append($"value.{messageElement.Name}.Count != 0)");
                        else sb.Append($"value.{messageElement.Name} != null)");
                        break;
                    case MapType type:
                        if (!type.IsOptional) sb.Append($"value.{messageElement.Name}.Count != 0)");
                        else sb.Append($"value.{messageElement.Name} != null)");
                        break;
                    case CustomType type:
                        {
                            switch (this.CustomTypeResolver(type))
                            {
                                case EnumDefinition elementEnumDefinition:
                                    if (!type.IsOptional) sb.Append($"value.{messageElement.Name} != ({elementEnumDefinition.Name})0)");
                                    else sb.Append($"value.{messageElement.Name} != null)");
                                    break;
                                case MessageDefinition elementMessageDefinition:
                                    if (!type.IsOptional) sb.Append($"value.{messageElement.Name} != {elementMessageDefinition.Name}.Empty)");
                                    else sb.Append($"value.{messageElement.Name} != null)");
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
                w.PushIndent();

                callback.Invoke();

                w.PopIndent();
                w.WriteLine("}");
            }

            private void Write_Formatter_Deserialize_PropertyDef(CodeWriter w, string name, TypeBase type, int rank)
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
                    case MemoryType memoryType when (memoryType.IsUseMemoryPool):
                        w.WriteLine($"{name} = r.GetRecyclableMemory({memoryType.MaxLength});");
                        break;
                    case MemoryType memoryType when (!memoryType.IsUseMemoryPool):
                        w.WriteLine($"{name} = r.GetMemory({memoryType.MaxLength});");
                        break;
                    case ListType listType:
                        {
                            w.WriteLine("var length = r.GetUInt32();");
                            w.WriteLine($"{name} = new {this.GetParameterTypeString(listType.ElementType)}[length];");

                            w.WriteLine($"for (int i = 0; i < {name}.Length; i++)");
                            w.WriteLine("{");

                            w.PushIndent();

                            this.Write_Formatter_Deserialize_PropertyDef(w, $"{name}[i]", listType.ElementType, rank + 1);

                            w.PopIndent();

                            w.WriteLine("}");
                        }
                        break;
                    case MapType mapType:
                        {
                            w.WriteLine("var length = r.GetUInt32();");
                            w.WriteLine($"{name} = new {GetFullName("Dictionary<,>", this.GetParameterTypeString(mapType.KeyType), this.GetParameterTypeString(mapType.ValueType))}();");
                            w.WriteLine($"{this.GetParameterTypeString(mapType.KeyType)} t_key = {GetDefaultValueString(mapType.KeyType)};");
                            w.WriteLine($"{this.GetParameterTypeString(mapType.ValueType)} t_value = {GetDefaultValueString(mapType.ValueType)};");

                            w.WriteLine("for (int i = 0; i < length; i++)");
                            w.WriteLine("{");

                            w.PushIndent();

                            this.Write_Formatter_Deserialize_PropertyDef(w, "t_key", mapType.KeyType, rank + 1);
                            this.Write_Formatter_Deserialize_PropertyDef(w, "t_value", mapType.ValueType, rank + 1);
                            w.WriteLine($"{name}[t_key] = t_value;");

                            w.PopIndent();

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
                            case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Medium):
                                w.WriteLine($"{name} = {messageInfo.Name}.Formatter.Deserialize(r, rank + 1);");
                                break;
                            case MessageDefinition messageInfo when (messageInfo.FormatType == MessageFormatType.Small):
                                w.WriteLine($"{name} = {messageInfo.Name}.Formatter.Deserialize(r, rank + 1);");
                                break;
                        }
                        break;
                }
            }
        }
    }
}
