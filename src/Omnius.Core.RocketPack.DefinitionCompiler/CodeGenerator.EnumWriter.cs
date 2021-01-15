using System;
using System.Linq;
using Omnius.Core.RocketPack.DefinitionCompiler.Models;

namespace Omnius.Core.RocketPack.DefinitionCompiler
{
    internal partial class CodeGenerator
    {
        private sealed class EnumWriter
        {
            public EnumWriter()
            {
            }

            public void Write(CodeWriter b, EnumDefinition enumDefinition, string accessLevel = "public")
            {
                b.WriteLine($"{accessLevel} enum {enumDefinition.Name} : {this.GetTypeString(enumDefinition.Type)}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    foreach (var element in enumDefinition.Elements)
                    {
                        b.WriteLine($"{element.Name} = {element.Id},");
                    }
                }

                b.WriteLine("}");
            }

            /// <summary>
            /// <see cref="TypeBase"/>から型を生成します。
            /// </summary>
            private string GetTypeString(TypeBase typeBase)
            {
                return typeBase switch
                {
                    IntType type when (!type.IsSigned && type.Size == 8) => "byte",
                    IntType type when (!type.IsSigned && type.Size == 16) => "ushort",
                    IntType type when (!type.IsSigned && type.Size == 32) => "uint",
                    IntType type when (!type.IsSigned && type.Size == 64) => "ulong",
                    IntType type when (type.IsSigned && type.Size == 8) => "sbyte",
                    IntType type when (type.IsSigned && type.Size == 16) => "short",
                    IntType type when (type.IsSigned && type.Size == 32) => "int",
                    IntType type when (type.IsSigned && type.Size == 64) => "long",
                    _ => throw new ArgumentException("unknown type", nameof(typeBase)),
                };
            }
        }
    }
}
