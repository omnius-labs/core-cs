using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static partial class RocketCodeGenerator
    {
        /// <summary>
        /// <see cref="EnumDefinition"/>からenumを生成します。
        /// </summary>
        private class EnumWriter
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
                switch (type)
                {
                    case IntType typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 8):
                        return "byte";
                    case IntType typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 16):
                        return "ushort";
                    case IntType typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 32):
                        return "uint";
                    case IntType typeInfo when (!typeInfo.IsSigned && typeInfo.Size == 64):
                        return "ulong";
                    case IntType typeInfo when (typeInfo.IsSigned && typeInfo.Size == 8):
                        return "sbyte";
                    case IntType typeInfo when (typeInfo.IsSigned && typeInfo.Size == 16):
                        return "short";
                    case IntType typeInfo when (typeInfo.IsSigned && typeInfo.Size == 32):
                        return "int";
                    case IntType typeInfo when (typeInfo.IsSigned && typeInfo.Size == 64):
                        return "long";
                    default:
                        throw new ArgumentException(nameof(type));
                }
            }

            /// <summary>
            /// Enumの生成。
            /// </summary>
            public void Write(CodeWriter w, EnumDefinition enumInfo)
            {
                w.WriteLine($"{_accessLevel} enum {enumInfo.Name} : {this.GetTypeString(enumInfo.Type)}");
                w.WriteLine("{");

                w.PushIndent();

                this.Write_Elements(w, enumInfo.Elements);

                w.PopIndent();

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
