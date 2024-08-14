using System.Globalization;
using System.Text;
using Omnius.Core.RocketPack.DefinitionCompiler.Internal;
using Omnius.Core.RocketPack.DefinitionCompiler.Models;

namespace Omnius.Core.RocketPack.DefinitionCompiler;

internal partial class CodeGenerator
{
    private sealed class ObjectWriter
    {
        private const string CustomSerializerName = "___CustomSerializer";
        private const string HashCodeName = "___hashCode";

        private readonly RocketPackDefinition _rootDefinition;
        private readonly IList<RocketPackDefinition> _externalDefinitions;

        public ObjectWriter(RocketPackDefinition rootDefinition, IEnumerable<RocketPackDefinition> externalDefinitions)
        {
            _rootDefinition = rootDefinition;
            _externalDefinitions = externalDefinitions.ToList();
        }

        public void Write(CodeWriter b, ObjectDefinition objectDefinition, string accessLevel = "public")
        {
            var inheritances = new List<string>();
            inheritances.Add(GenerateTypeFullName("RocketMessage<>", objectDefinition.CSharpFullName));
            if (this.ShouldDispose(objectDefinition)) inheritances.Add(GenerateTypeFullName("IDisposable"));

            b.WriteLine($"{accessLevel} sealed partial class {objectDefinition.Name} : {string.Join(", ", inheritances)}");

            b.WriteLine("{");

            using (b.Indent())
            {
                this.Write_StaticConstructor(b, objectDefinition);
                b.WriteLine();

                this.Write_Constructor(b, objectDefinition);
                b.WriteLine();

                if (this.ShouldDispose(objectDefinition))
                {
                    this.Write_Dispose(b, objectDefinition);
                    b.WriteLine();
                }

                this.Write_Properties(b, objectDefinition);
                b.WriteLine();

                this.Write_Equals(b, objectDefinition);
                b.WriteLine();

                if (objectDefinition.FormatType == ObjectFormatType.Message)
                {
                    this.Write_Medium_Formatter(b, objectDefinition);
                }
                else if (objectDefinition.FormatType == ObjectFormatType.Struct)
                {
                    this.Write_Small_Formatter(b, objectDefinition);
                }
            }

            b.WriteLine("}");
        }

        /// <summary>
        /// 静的コンストラクタの生成。
        /// </summary>
        private void Write_StaticConstructor(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"static {objectDefinition.Name}()");
            b.WriteLine("{");

            using (b.Indent())
            {
                // CustomSerializerのインスタンスの作成
                this.Write_StaticConstructor_CustomSerializerProperty(b, objectDefinition);

                // Emptyのインスタンスの作成
                this.Write_StaticConstructor_EmptyProperty(b, objectDefinition);
            }

            b.WriteLine("}");
        }

        private void Write_StaticConstructor_CustomSerializerProperty(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"Formatter = new {CustomSerializerName}();");
        }

        private void Write_StaticConstructor_EmptyProperty(CodeWriter b, ObjectDefinition objectDefinition)
        {
            var parameters = new List<string>();

            foreach (var element in objectDefinition.Elements)
            {
                parameters.Add(this.GetDefaultValueString(element.Type));
            }

            b.WriteLine($"Empty = new {objectDefinition.CSharpFullName}({string.Join(", ", parameters)});");
        }

        /// <summary>
        /// コンストラクタの生成。
        /// </summary>
        private void Write_Constructor(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"private readonly {GenerateTypeFullName("Lazy<>", "int")} {HashCodeName};");

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

        private void Write_Constructor_Init(CodeWriter b, ObjectDefinition objectDefinition)
        {
            foreach (var element in objectDefinition.Elements)
            {
                switch (element.Type)
                {
                    case VectorType type:
                        if (type.IsOptional)
                        {
                            b.WriteLine($"if ({GenerateFieldVariableName(element.Name)} != null)");
                            b.WriteLine("{");
                            b.PushIndent();
                        }

                        b.WriteLine($"this.{element.Name} = {GenerateTypeFullName("ImmutableList")}.CreateRange<{this.GenerateParameterTypeFullName(type.ElementType)}>({GenerateFieldVariableName(element.Name)});");

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

                        b.WriteLine($"this.{element.Name} = {GenerateTypeFullName("ImmutableDictionary")}.CreateRange<{this.GenerateParameterTypeFullName(type.KeyType)}, {this.GenerateParameterTypeFullName(type.ValueType)}>({GenerateFieldVariableName(element.Name)});");

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

        private void Write_Constructor_Define_MaxLength(CodeWriter b, ObjectDefinition objectDefinition)
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

        private bool Try_Write_Constructor_Define_MaxLength_Element(CodeWriter b, string name, TypeBase type)
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

        private void Write_Constructor_Parameter_Check(CodeWriter b, ObjectDefinition objectDefinition)
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

        private bool Try_Write_Constructor_Parameter_Check_Element(CodeWriter b, ObjectElement element)
        {
            bool isChecked = false;

            isChecked |= this.Try_Write_Constructor_Parameter_Check_Element_Null(b, GenerateFieldVariableName(element.Name), element.Type);
            isChecked |= this.Try_Write_Constructor_Parameter_Check_Element_MaxLength(b, GenerateFieldVariableName(element.Name), element.Type);

            if (element.Type is VectorType listType)
            {
                var b2 = new CodeWriter();
                bool isChecked2 = false;

                if (listType.IsOptional)
                {
                    b2.WriteLine($"if ({GenerateFieldVariableName(element.Name)} is not null)");
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
                var b2 = new CodeWriter();
                bool isChecked2 = false;

                if (mapType.IsOptional)
                {
                    b2.WriteLine($"if ({GenerateFieldVariableName(element.Name)} is not null)");
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

        private bool Try_Write_Constructor_Parameter_Check_Element_Null(CodeWriter b, string name, TypeBase type)
        {
            switch (type)
            {
                case StringType when !type.IsOptional:
                    b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                    return true;
                case BytesType memoryType when !type.IsOptional && memoryType.IsUseMemoryPool:
                    b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                    return true;
                case VectorType when !type.IsOptional:
                    b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                    return true;
                case MapType when !type.IsOptional:
                    b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                    return true;
                case CustomType customType when !type.IsOptional:
                    switch (this.FindDefinition(customType))
                    {
                        case ObjectDefinition objectDefinition:
                            b.WriteLine($"if ({name} is null) throw new {GenerateTypeFullName("ArgumentNullException")}(\"{name}\");");
                            return true;
                    }

                    return false;
                default:
                    return false;
            }
        }

        private bool Try_Write_Constructor_Parameter_Check_Element_MaxLength(CodeWriter b, string name, TypeBase type)
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
                b.WriteLine($"if ({name} is not null && {name}.{property} > {maxLength}) throw new {GenerateTypeFullName("ArgumentOutOfRangeException")}(\"{name}\");");
            }
            else
            {
                b.WriteLine($"if ({name}.{property} > {maxLength}) throw new {GenerateTypeFullName("ArgumentOutOfRangeException")}(\"{name}\");");
            }

            return true;
        }

        private void Write_Constructor_HashCode(CodeWriter b, ObjectDefinition objectDefinition)
        {
            const string TempVariableName = "___h";

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

        private void Write_Constructor_HashCode_Element(CodeWriter b, string hashCodeName, string parameterName, TypeBase type)
        {
            switch (type)
            {
                case BoolType:
                    b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                    break;
                case IntType:
                    b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                    break;
                case FloatType:
                    b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                    break;
                case StringType stringType:
                    if (!stringType.IsOptional)
                    {
                        b.WriteLine($"if (!{parameterName}.IsEmpty) {hashCodeName}.Add({parameterName}.GetHashCode());");
                    }
                    else
                    {
                        b.WriteLine($"if ({parameterName} is not null && !{parameterName}.IsEmpty) {hashCodeName}.Add({parameterName}.GetHashCode());");
                    }

                    break;
                case TimestampType:
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
                        b.WriteLine($"if ({parameterName} is not null && !{parameterName}.Value.IsEmpty) {hashCodeName}.Add({GenerateTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Value.Span));");
                    }
                    else
                    {
                        b.WriteLine($"if ({parameterName} is not null && !{parameterName}.Memory.IsEmpty) {hashCodeName}.Add({GenerateTypeFullName("ObjectHelper")}.GetHashCode({parameterName}.Memory.Span));");
                    }

                    break;
                case VectorType listType:
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

                    break;
                case MapType mapType:
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

                    break;
                case CustomType customType:
                    switch (this.FindDefinition(customType))
                    {
                        case EnumDefinition:
                            b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                            break;
                        case ObjectDefinition objectDefinition:
                            b.WriteLine($"if ({parameterName} != default) {hashCodeName}.Add({parameterName}.GetHashCode());");
                            break;
                    }

                    break;
            }
        }

        private void Write_Dispose(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"public void Dispose()");
            b.WriteLine("{");

            using (b.Indent())
            {
                foreach (var element in objectDefinition.Elements)
                {
                    if (this.ShouldDispose(element.Type))
                    {
                        if (!element.Type.IsOptional)
                        {
                            b.WriteLine($"this.{element.Name}.Dispose();");
                        }
                        else
                        {
                            b.WriteLine($"if (this.{element.Name} is not null) this.{element.Name}.Dispose();");
                        }
                    }
                }
            }

            b.WriteLine("}");
        }
        private void Write_Equals(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"public static bool operator ==({objectDefinition.CSharpFullName}? left, {objectDefinition.CSharpFullName}? right)");
            b.WriteLine("{");

            using (b.Indent())
            {
                b.WriteLine("return (right is null) ? (left is null) : right.Equals(left);");
            }

            b.WriteLine("}");

            b.WriteLine($"public static bool operator !=({objectDefinition.CSharpFullName}? left, {objectDefinition.CSharpFullName}? right)");
            b.WriteLine("{");

            using (b.Indent())
            {
                b.WriteLine("return !(left == right);");
            }

            b.WriteLine("}");

            b.WriteLine("public override bool Equals(object? other)");
            b.WriteLine("{");

            using (b.Indent())
            {
                b.WriteLine($"if (other is not {objectDefinition.CSharpFullName}) return false;");
                b.WriteLine($"return this.Equals(({objectDefinition.CSharpFullName})other);");
            }

            b.WriteLine("}");

            b.WriteLine($"public override bool Equals({objectDefinition.CSharpFullName}? target)");
            b.WriteLine("{");

            using (b.Indent())
            {
                b.WriteLine("if (target is null) return false;");
                b.WriteLine("if (object.ReferenceEquals(this, target)) return true;");

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
                        case FloatType type:
                            b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                            break;
                        case StringType type:
                            b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                            break;
                        case TimestampType type:
                            b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                            break;
                        case BytesType type when !type.IsUseMemoryPool:
                            if (!type.IsOptional)
                            {
                                b.WriteLine($"if (!{GenerateTypeFullName("BytesOperations")}.Equals(this.{element.Name}.Span, target.{element.Name}.Span)) return false;");
                            }
                            else
                            {
                                b.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                b.WriteLine($"if ((this.{element.Name} is not null) && (target.{element.Name} is not null) && !{GenerateTypeFullName("BytesOperations")}.Equals(this.{element.Name}.Value.Span, target.{element.Name}.Value.Span)) return false;");
                            }

                            break;
                        case BytesType type when type.IsUseMemoryPool:
                            if (!type.IsOptional)
                            {
                                b.WriteLine($"if (!{GenerateTypeFullName("BytesOperations")}.Equals(this.{element.Name}.Memory.Span, target.{element.Name}.Memory.Span)) return false;");
                            }
                            else
                            {
                                b.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                b.WriteLine($"if ((this.{element.Name} is not null) && (target.{element.Name} is not null) && !{GenerateTypeFullName("BytesOperations")}.Equals(this.{element.Name}.Memory.Span, target.{element.Name}.Memory.Span)) return false;");
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
                                b.WriteLine($"if ((this.{element.Name} is not null) && (target.{element.Name} is not null) && !{GenerateTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
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
                                b.WriteLine($"if ((this.{element.Name} is not null) && (target.{element.Name} is not null) && !{GenerateTypeFullName("CollectionHelper")}.Equals(this.{element.Name}, target.{element.Name})) return false;");
                            }

                            break;
                        case CustomType type:
                            switch (this.FindDefinition(type))
                            {
                                case EnumDefinition enumInfo:
                                    b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                    break;
                                case ObjectDefinition objectDefinition2:
                                    if (!type.IsOptional)
                                    {
                                        b.WriteLine($"if (this.{element.Name} != target.{element.Name}) return false;");
                                    }
                                    else
                                    {
                                        b.WriteLine($"if ((this.{element.Name} is null) != (target.{element.Name} is null)) return false;");
                                        b.WriteLine($"if ((this.{element.Name} is not null) && (target.{element.Name} is not null) && this.{element.Name} != target.{element.Name}) return false;");
                                    }

                                    break;
                            }

                            break;
                    }
                }

                b.WriteLine();

                b.WriteLine("return true;");
            }

            b.WriteLine("}");

            b.WriteLine($"public override int GetHashCode() => {HashCodeName}.Value;");
        }

        private void Write_Properties(CodeWriter b, ObjectDefinition objectDefinition)
        {
            foreach (var element in objectDefinition.Elements)
            {
                b.WriteLine($"public {this.GeneratePropertyTypeFullName(element.Type)} {element.Name} {{ get; }}");
            }
        }

        private void Write_Medium_Formatter(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"private sealed class {CustomSerializerName} : {GenerateTypeFullName("IRocketMessageSerializer<>", objectDefinition.CSharpFullName)}");
            b.WriteLine("{");

            using (b.Indent())
            {
                this.Write_Medium_Formatter_Serialize(b, objectDefinition);
                this.Write_Medium_Formatter_Deserialize(b, objectDefinition);
            }

            b.WriteLine("}");
        }

        private void Write_Medium_Formatter_Serialize(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"public void Serialize(ref {GenerateTypeFullName("RocketMessageWriter")} w, scoped in {objectDefinition.CSharpFullName} value, scoped in int depth)");
            b.WriteLine("{");

            using (b.Indent())
            {
                b.WriteLine($"if (depth > 256) throw new {GenerateTypeFullName("FormatException")}();");
                b.WriteLine();

                foreach (var (index, element) in objectDefinition.Elements.Select((n, i) => (i + 1, n)))
                {
                    this.BlockWhoseValueIsNotDefault(b, element, () =>
                    {
                        b.WriteLine($"w.Write((uint){index});");
                        this.Write_Medium_Formatter_Serialize_PropertyDef(b, "value." + element.Name, element.Type, 0);
                    });
                }

                b.WriteLine($"w.Write((uint){0});");
            }

            b.WriteLine("}");
        }

        private void Write_Medium_Formatter_Serialize_PropertyDef(CodeWriter b, string name, TypeBase type, int depth)
        {
            switch (type)
            {
                case BoolType:
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name});");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Value);");
                    }

                    break;
                case IntType:
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name});");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Value);");
                    }

                    break;
                case FloatType:
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name});");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Value);");
                    }

                    break;
                case StringType:
                    b.WriteLine($"w.Write({name});");
                    break;
                case TimestampType:
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name});");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Value);");
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
                case BytesType memoryType when (memoryType.IsUseMemoryPool):
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name}.Memory.Span);");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Memory.Span);");
                    }

                    break;
                case VectorType listType:
                    b.WriteLine($"w.Write((uint){name}.Count);");
                    b.WriteLine($"foreach (var n in {name})");
                    b.WriteLine("{");

                    using (b.Indent())
                    {
                        this.Write_Medium_Formatter_Serialize_PropertyDef(b, "n", listType.ElementType, depth + 1);
                    }

                    b.WriteLine("}");

                    break;
                case MapType mapType:
                    b.WriteLine($"w.Write((uint){name}.Count);");
                    b.WriteLine($"foreach (var n in {name})");
                    b.WriteLine("{");

                    using (b.Indent())
                    {
                        this.Write_Medium_Formatter_Serialize_PropertyDef(b, "n.Key", mapType.KeyType, depth + 1);
                        this.Write_Medium_Formatter_Serialize_PropertyDef(b, "n.Value", mapType.ValueType, depth + 1);
                    }

                    b.WriteLine("}");

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
                        case ObjectDefinition objectDefinition:
                            b.WriteLine($"{objectDefinition.CSharpFullName}.Formatter.Serialize(ref w, {name}, depth + 1);");
                            break;
                    }

                    break;
            }
        }

        private void Write_Medium_Formatter_Deserialize(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"public {objectDefinition.CSharpFullName} Deserialize(ref {GenerateTypeFullName("RocketMessageReader")} r, scoped in int depth)");
            b.WriteLine("{");

            using (b.Indent())
            {
                b.WriteLine($"if (depth > 256) throw new {GenerateTypeFullName("FormatException")}();");
                b.WriteLine();

                foreach (var element in objectDefinition.Elements)
                {
                    b.WriteLine($"{this.GenerateParameterTypeFullName(element.Type)} p_{GenerateFieldVariableName(element.Name)} = {this.GetDefaultValueString(element.Type)};");
                }

                b.WriteLine();

                b.WriteLine("for (; ; )");
                b.WriteLine("{");

                using (b.Indent())
                {
                    b.WriteLine("uint id = r.GetUInt32();");
                    b.WriteLine("if (id == 0) break;");
                    b.WriteLine("switch (id)");
                    b.WriteLine("{");

                    using (b.Indent())
                    {
                        foreach (var (index, element) in objectDefinition.Elements.Select((n, i) => (i + 1, n)))
                        {
                            b.WriteLine($"case {index}:");

                            using (b.Indent())
                            {
                                b.WriteLine("{");

                                using (b.Indent())
                                {
                                    this.Write_Medium_Formatter_Deserialize_PropertyDef(b, "p_" + GenerateFieldVariableName(element.Name), element.Type, 0);

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

                b.WriteLine($"return new {objectDefinition.CSharpFullName}({string.Join(", ", objectDefinition.Elements.Select(n => "p_" + GenerateFieldVariableName(n.Name)))});");
            }

            b.WriteLine("}");
        }

        private void Write_Medium_Formatter_Deserialize_PropertyDef(CodeWriter b, string name, TypeBase type, int depth)
        {
            switch (type)
            {
                case BoolType:
                    b.WriteLine($"{name} = r.GetBoolean();");
                    break;
                case IntType intType when (!intType.IsSigned):
                    b.WriteLine($"{name} = r.GetUInt{intType.Size}();");
                    break;
                case IntType intType when (intType.IsSigned):
                    b.WriteLine($"{name} = r.GetInt{intType.Size}();");
                    break;
                case FloatType floatType:
                    b.WriteLine($"{name} = r.GetFloat{floatType.Size}();");
                    break;
                case StringType stringType:
                    b.WriteLine($"{name} = r.GetString({stringType.MaxLength});");
                    break;
                case TimestampType timestampType:
                    b.WriteLine($"{name} = r.GetTimestamp{timestampType.Size}();");
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
                            this.Write_Medium_Formatter_Deserialize_PropertyDef(b, $"{name}[i]", listType.ElementType, depth + 1);
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
                            this.Write_Medium_Formatter_Deserialize_PropertyDef(b, "t_key", mapType.KeyType, depth + 1);
                            this.Write_Medium_Formatter_Deserialize_PropertyDef(b, "t_value", mapType.ValueType, depth + 1);
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
                                case IntType intType when (intType.IsSigned):
                                    b.WriteLine($"{name} = ({enumInfo.CSharpFullName})r.GetInt64();");
                                    break;
                                case IntType intType when (!intType.IsSigned):
                                    b.WriteLine($"{name} = ({enumInfo.CSharpFullName})r.GetUInt64();");
                                    break;
                            }

                            break;
                        case ObjectDefinition objectDefinition:
                            b.WriteLine($"{name} = {objectDefinition.CSharpFullName}.Formatter.Deserialize(ref r, depth + 1);");
                            break;
                    }

                    break;
            }
        }

        private void Write_Small_Formatter(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"private sealed class ___CustomSerializer : {GenerateTypeFullName("IRocketMessageSerializer<>", objectDefinition.CSharpFullName)}");
            b.WriteLine("{");

            using (b.Indent())
            {
                this.Write_Small_Formatter_Serialize(b, objectDefinition);
                this.Write_Small_Formatter_Deserialize(b, objectDefinition);
            }

            b.WriteLine("}");
        }

        private void Write_Small_Formatter_Serialize(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"public void Serialize(ref {GenerateTypeFullName("RocketMessageWriter")} w, scoped in {objectDefinition.CSharpFullName} value, scoped in int depth)");
            b.WriteLine("{");

            using (b.Indent())
            {
                b.WriteLine($"if (depth > 256) throw new {GenerateTypeFullName("FormatException")}();");
                b.WriteLine();

                foreach (var element in objectDefinition.Elements)
                {
                    this.Write_Small_Formatter_Serialize_PropertyDef(b, "value." + element.Name, element.Type, 0);
                }
            }

            b.WriteLine("}");
        }

        private void Write_Small_Formatter_Serialize_PropertyDef(CodeWriter b, string name, TypeBase type, int depth)
        {
            switch (type)
            {
                case BoolType:
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name});");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Value);");
                    }

                    break;
                case IntType:
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name});");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Value);");
                    }

                    break;
                case FloatType:
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name});");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Value);");
                    }

                    break;
                case StringType:
                    b.WriteLine($"w.Write({name});");
                    break;
                case TimestampType:
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name});");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Value);");
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
                case BytesType memoryType when (memoryType.IsUseMemoryPool):
                    if (!type.IsOptional)
                    {
                        b.WriteLine($"w.Write({name}.Memory.Span);");
                    }
                    else
                    {
                        b.WriteLine($"w.Write({name}.Memory.Span);");
                    }

                    break;
                case VectorType listType:
                    {
                        b.WriteLine($"w.Write((uint){name}.Count);");
                        b.WriteLine($"foreach (var n in {name})");
                        b.WriteLine("{");

                        using (b.Indent())
                        {
                            this.Write_Small_Formatter_Serialize_PropertyDef(b, "n", listType.ElementType, depth + 1);
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
                            this.Write_Small_Formatter_Serialize_PropertyDef(b, "n.Key", mapType.KeyType, depth + 1);
                            this.Write_Small_Formatter_Serialize_PropertyDef(b, "n.Value", mapType.ValueType, depth + 1);
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
                        case ObjectDefinition objectDefinition:
                            b.WriteLine($"{objectDefinition.CSharpFullName}.Formatter.Serialize(ref w, {name}, depth + 1);");
                            break;
                    }

                    break;
            }
        }

        private void Write_Small_Formatter_Deserialize(CodeWriter b, ObjectDefinition objectDefinition)
        {
            b.WriteLine($"public {objectDefinition.CSharpFullName} Deserialize(ref {GenerateTypeFullName("RocketMessageReader")} r, scoped in int depth)");
            b.WriteLine("{");

            using (b.Indent())
            {
                b.WriteLine($"if (depth > 256) throw new {GenerateTypeFullName("FormatException")}();");
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

                b.WriteLine($"return new {objectDefinition.CSharpFullName}({string.Join(", ", objectDefinition.Elements.Select(n => "p_" + GenerateFieldVariableName(n.Name)))});");
            }

            b.WriteLine("}");
        }

        private void Write_Small_Formatter_Deserialize_PropertyDef(CodeWriter b, string name, TypeBase type, int depth)
        {
            switch (type)
            {
                case BoolType:
                    b.WriteLine($"{name} = r.GetBoolean();");
                    break;
                case IntType intType when (!intType.IsSigned):
                    b.WriteLine($"{name} = r.GetUInt{intType.Size}();");
                    break;
                case IntType intType when (intType.IsSigned):
                    b.WriteLine($"{name} = r.GetInt{intType.Size}();");
                    break;
                case FloatType floatType:
                    b.WriteLine($"{name} = r.GetFloat{floatType.Size}();");
                    break;
                case StringType stringType:
                    b.WriteLine($"{name} = r.GetString({stringType.MaxLength});");
                    break;
                case TimestampType timestampType:
                    b.WriteLine($"{name} = r.GetTimestamp{timestampType.Size}();");
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
                            this.Write_Small_Formatter_Deserialize_PropertyDef(b, $"{name}[i]", listType.ElementType, depth + 1);
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
                            this.Write_Small_Formatter_Deserialize_PropertyDef(b, "t_key", mapType.KeyType, depth + 1);
                            this.Write_Small_Formatter_Deserialize_PropertyDef(b, "t_value", mapType.ValueType, depth + 1);
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
                                case IntType intType when (intType.IsSigned):
                                    b.WriteLine($"{name} = ({enumInfo.CSharpFullName})r.GetInt64();");
                                    break;
                                case IntType intType when (!intType.IsSigned):
                                    b.WriteLine($"{name} = ({enumInfo.CSharpFullName})r.GetUInt64();");
                                    break;
                            }

                            break;
                        case ObjectDefinition objectDefinition:
                            b.WriteLine($"{name} = {objectDefinition.CSharpFullName}.Formatter.Deserialize(ref r, depth + 1);");
                            break;
                    }

                    break;
            }
        }

        private void BlockWhoseValueIsNotDefault(CodeWriter b, ObjectElement element, Action callback)
        {
            var sb = new StringBuilder();
            sb.Append($"if (");

            switch (element.Type)
            {
                case BoolType type:
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != false)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case IntType type:
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != 0)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case FloatType type when (type.Size == 32):
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != 0.0F)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case FloatType type when (type.Size == 64):
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != 0.0D)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case StringType type:
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != {GenerateTypeFullName("Utf8String")}.Empty)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case TimestampType type:
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != {GenerateTypeFullName($"Timestamp{type.Size}")}.Zero)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case BytesType type when !type.IsUseMemoryPool:
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"!value.{element.Name}.IsEmpty)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case BytesType type when type.IsUseMemoryPool:
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"!value.{element.Name}.Memory.IsEmpty)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case VectorType type:
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name}.Count != 0)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case MapType type:
                    if (!type.IsOptional)
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name}.Count != 0)");
                    }
                    else
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                    }

                    break;
                case CustomType type:
                    switch (this.FindDefinition(type))
                    {
                        case EnumDefinition elementEnumDefinition:
                            if (!type.IsOptional)
                            {
                                sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != ({elementEnumDefinition.CSharpFullName})0)");
                            }
                            else
                            {
                                sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                            }

                            break;
                        case ObjectDefinition elementMessageDefinition:
                            if (!type.IsOptional)
                            {
                                sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != {elementMessageDefinition.CSharpFullName}.Empty)");
                            }
                            else
                            {
                                sb.Append(CultureInfo.InvariantCulture, $"value.{element.Name} != null)");
                            }

                            break;
                        default:
                            throw new ArgumentException($"Type \"{type.Type}\" was not found", nameof(element));
                    }

                    break;
                default:
                    throw new ArgumentException($"Type \"{element.Type.GetType().Name}\" was not found", nameof(element));
            }

            b.WriteLine(sb.ToString());
            b.WriteLine("{");

            using (b.Indent())
            {
                callback.Invoke();
            }

            b.WriteLine("}");
        }

        /// <summary>
        /// プロパティ名からフィールド変数名を生成します。
        /// </summary>
        private static string GenerateFieldVariableName(string name)
        {
            return name[0].ToString().ToLower(CultureInfo.InvariantCulture) + name[1..];
        }

        private object? FindDefinition(CustomType customType)
        {
            bool isFullName = customType.Type.Contains(".", StringComparison.InvariantCulture);

            if (isFullName)
            {
                var result = FindDefsForRootAndExternalByFullName(customType);
                if (result is not null) return result;
            }
            else
            {
                var result = FindDefsForRootByName(customType);
                if (result is not null) return result;

                result = FindDefsForExternalByName(customType);
                if (result is not null) return result;
            }

            throw ThrowHelper.CreateRocketPackDefinitionCompilerException_DefinitionNotFound(customType.Type);

            object? FindDefsForRootByName(CustomType customType)
            {
                var enumDefinitions = _rootDefinition.Enums.Where(m => m.Name == customType.Type).ToArray();
                var objectDefinitions = _rootDefinition.Objects.Where(m => m.Name == customType.Type).ToArray();
                return Validate(enumDefinitions.Union<object>(objectDefinitions));
            }

            object? FindDefsForExternalByName(CustomType customType)
            {
                var results = new List<object>();

                foreach (var definition in _externalDefinitions)
                {
                    var enumDefinitions = definition.Enums.Where(m => m.Name == customType.Type).ToArray();
                    var objectDefinitions = definition.Objects.Where(m => m.Name == customType.Type).ToArray();
                    results.AddRange(enumDefinitions.Union<object>(objectDefinitions));
                }

                return Validate(results);
            }

            object? FindDefsForRootAndExternalByFullName(CustomType customType)
            {
                var results = new List<object>();

                foreach (var definition in new[] { _rootDefinition }.Union(_externalDefinitions))
                {
                    var enumDefinitions = definition.Enums.Where(m => m.FullName == customType.Type).ToArray();
                    var objectDefinitions = definition.Objects.Where(m => m.FullName == customType.Type).ToArray();
                    results.AddRange(enumDefinitions.Union<object>(objectDefinitions));
                }

                return Validate(results);
            }

            object? Validate(IEnumerable<object> results)
            {
                int count = results.Count();

                if (count > 1) throw ThrowHelper.CreateRocketPackDefinitionCompilerException_NotOneDefinitionFound(customType.Type);

                return results.FirstOrDefault();
            }
        }

        private string GeneratePropertyTypeFullName(TypeBase typeBase)
        {
            return typeBase switch
            {
                VectorType type => GenerateTypeFullName("ImmutableList<>", this.GenerateParameterTypeFullName(type.ElementType)) + (type.IsOptional ? "?" : ""),
                MapType type => GenerateTypeFullName("ImmutableDictionary<,>", this.GenerateParameterTypeFullName(type.KeyType), this.GenerateParameterTypeFullName(type.ValueType)) + (type.IsOptional ? "?" : ""),
                _ => this.GenerateParameterTypeFullName(typeBase),
            };
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
                StringType type => GenerateTypeFullName("Utf8String") + (type.IsOptional ? "?" : ""),
                TimestampType type when (type.Size == 64) => GenerateTypeFullName("Timestamp64") + (type.IsOptional ? "?" : ""),
                TimestampType type when (type.Size == 96) => GenerateTypeFullName("Timestamp96") + (type.IsOptional ? "?" : ""),
                BytesType type when (type.IsUseMemoryPool) => GenerateTypeFullName("IMemoryOwner<>", "byte") + (type.IsOptional ? "?" : ""),
                BytesType type when (!type.IsUseMemoryPool) => GenerateTypeFullName("ReadOnlyMemory<>", "byte") + (type.IsOptional ? "?" : ""),
                VectorType type => $"{this.GenerateParameterTypeFullName(type.ElementType)}[]" + (type.IsOptional ? "?" : ""),
                MapType type => GenerateTypeFullName("Dictionary<,>", this.GenerateParameterTypeFullName(type.KeyType), this.GenerateParameterTypeFullName(type.ValueType)) + (type.IsOptional ? "?" : ""),
                CustomType type => this.FindDefinition(type) switch
                {
                    EnumDefinition enumDefinition => enumDefinition.CSharpFullName + (type.IsOptional ? "?" : ""),
                    ObjectDefinition objectDefinition when (objectDefinition.FormatType == ObjectFormatType.Message) => objectDefinition.CSharpFullName + (type.IsOptional ? "?" : ""),
                    ObjectDefinition objectDefinition when (objectDefinition.FormatType == ObjectFormatType.Struct) => objectDefinition.CSharpFullName + (type.IsOptional ? "?" : ""),
                    _ => throw new ArgumentException($"Type \"{type.Type}\" was not found", nameof(typeBase)),
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
                StringType type => type.IsOptional ? "null" : GenerateTypeFullName("Utf8String") + ".Empty",
                TimestampType type => type.IsOptional ? "null" : GenerateTypeFullName($"Timestamp{type.Size}") + ".Zero",
                BytesType type when (!type.IsUseMemoryPool) => type.IsOptional ? "null" : GenerateTypeFullName("ReadOnlyMemory<>", "byte") + ".Empty",
                BytesType type when (type.IsUseMemoryPool) => type.IsOptional ? "null" : GenerateTypeFullName("MemoryOwner<>", "byte") + ".Empty",
                VectorType type => type.IsOptional ? "null" : GenerateTypeFullName("Array") + ".Empty<" + this.GenerateParameterTypeFullName(type.ElementType) + ">()",
                MapType type => type.IsOptional ? "null" : "new " + GenerateTypeFullName("Dictionary<,>", this.GenerateParameterTypeFullName(type.KeyType), this.GenerateParameterTypeFullName(type.ValueType)) + "()",
                CustomType type => this.FindDefinition(type) switch
                {
                    EnumDefinition elementEnumDefinition => type.IsOptional ? "null" : $"({elementEnumDefinition.CSharpFullName})0",
                    ObjectDefinition elementMessageDefinition => type.IsOptional ? "null" : $"{elementMessageDefinition.CSharpFullName}.Empty",
                    _ => throw new ArgumentException($"Type \"{type.Type}\" was not found", nameof(typeBase)),
                },
                _ => throw new ArgumentException($"Type \"{typeBase.GetType().Name}\" was not found", nameof(typeBase)),
            };
        }

        private bool ShouldDispose(ObjectDefinition objectDefinition)
        {
            return objectDefinition.Elements.Any(n => this.ShouldDispose(n.Type));
        }

        private bool ShouldDispose(TypeBase typeBase)
        {
            return typeBase switch
            {
                BytesType type when (type.IsUseMemoryPool) => true,
                CustomType type => this.FindDefinition(type) switch
                {
                    ObjectDefinition objectDefinition => this.ShouldDispose(objectDefinition),
                    _ => false,
                },
                _ => false,
            };
        }
    }
}
