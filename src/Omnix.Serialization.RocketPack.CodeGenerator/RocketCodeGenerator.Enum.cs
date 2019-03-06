using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static partial class RocketCodeGenerator
    {
        /// <summary>
        /// <see cref="EnumInfo"/>からenumを生成します。
        /// </summary>
        class EnumWriter
        {
            private RocketFormatInfo _rocketFormatInfo;
            private string _accessLevel;

            public EnumWriter(RocketFormatInfo rocketFormatInfo)
            {
                _rocketFormatInfo = rocketFormatInfo;

                var accessLevelOption = _rocketFormatInfo.Options.FirstOrDefault(n => n.Name == "csharp_access_level");
                _accessLevel = accessLevelOption?.Value ?? "public";
            }

            /// <summary>
            /// <see cref="TypeInfo"/>から型を生成します。
            /// </summary>
            private string GetTypeString(TypeInfo type)
            {
                switch (type)
                {
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
                    default:
                        throw new ArgumentException(nameof(type));
                }
            }

            /// <summary>
            /// Enumの生成。
            /// </summary>
            public void Write(CodeWriter w, EnumInfo enumInfo)
            {
                w.WriteLine($"{_accessLevel} enum {enumInfo.Name} : {this.GetTypeString(enumInfo.Type)}");
                w.WriteLine("{");

                w.PushIndent();

                this.Write_Elements(w, enumInfo.Elements);

                w.PopIndent();

                w.WriteLine("}");
            }

            private void Write_Elements(CodeWriter w, IEnumerable<EnumElementInfo> enumElementInfos)
            {
                foreach (var elementInfo in enumElementInfos)
                {
                    w.WriteLine($"{elementInfo.Name} = {elementInfo.Id},");
                }
            }
        }
    }
}
