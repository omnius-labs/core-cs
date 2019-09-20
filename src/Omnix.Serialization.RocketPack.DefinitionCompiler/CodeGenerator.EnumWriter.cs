using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnix.Serialization.RocketPack.DefinitionCompiler
{
    internal partial class CodeGenerator
    {
        private sealed class EnumWriter
        {
            private readonly RocketPackDefinition _rootDefinition;
            private readonly string _accessLevel;

            public EnumWriter(RocketPackDefinition rootDefinition)
            {
                _rootDefinition = rootDefinition;

                var accessLevelOption = _rootDefinition.Options.FirstOrDefault(n => n.Name == "csharp_access_level");
                _accessLevel = accessLevelOption?.Value ?? "public";
            }

            /// <summary>
            /// <see cref="TypeBase"/>から型を生成します。
            /// </summary>
            private string GetTypeString(TypeBase type)
            {
                return type switch
                {
                    IntType typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 8) => "byte",
                    IntType typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 16) => "ushort",
                    IntType typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 32) => "uint",
                    IntType typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 64) => "ulong",
                    IntType typeInfo when (typeInfo.IsSigned && typeInfo.Size == 8) => "sbyte",
                    IntType typeInfo when (typeInfo.IsSigned && typeInfo.Size == 16) => "short",
                    IntType typeInfo when (typeInfo.IsSigned && typeInfo.Size == 32) => "int",
                    IntType typeInfo when (typeInfo.IsSigned && typeInfo.Size == 64) => "long",
                    _ => throw new ArgumentException(nameof(type)),
                };
            }

            public void Write(CodeBuilder b, EnumDefinition enumDefinition)
            {
                b.WriteLine($"{_accessLevel} enum {enumDefinition.Name} : {this.GetTypeString(enumDefinition.Type)}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    this.Write_Elements(b, enumDefinition.Elements);
                }

                b.WriteLine("}");
            }

            private void Write_Elements(CodeBuilder b, IEnumerable<EnumElement> enumElements)
            {
                foreach (var element in enumElements)
                {
                    b.WriteLine($"{element.Name} = {element.Id},");
                }
            }
        }
    }
}
