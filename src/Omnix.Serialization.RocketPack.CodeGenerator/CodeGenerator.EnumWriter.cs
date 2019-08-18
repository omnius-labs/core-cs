using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static partial class CodeGenerator
    {
        private sealed class EnumWriter
        {
            private readonly RocketPackDefinition _rocketPackDefinition;
            private readonly string _accessLevel;

            public EnumWriter(RocketPackDefinition rocketPackDefinition)
            {
                _rocketPackDefinition = rocketPackDefinition;

                var accessLevelOption = _rocketPackDefinition.Options.FirstOrDefault(n => n.Name == "csharp_access_level");
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

            public void Write(CodeWriter w, EnumDefinition enumInfo)
            {
                w.WriteLine($"{_accessLevel} enum {enumInfo.Name} : {this.GetTypeString(enumInfo.Type)}");
                w.WriteLine("{");

                using (w.Indent())
                {
                    this.Write_Elements(w, enumInfo.Elements);
                }

                w.WriteLine("}");
            }

            private void Write_Elements(CodeWriter w, IEnumerable<EnumElement> enumElementInfos)
            {
                foreach (var elementInfo in enumElementInfos)
                {
                    w.WriteLine($"{elementInfo.Name} = {elementInfo.Id},");
                }
            }
        }
    }
}
