using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static partial class RocketCodeGenerator
    {
        public static string Generate(RocketFormatInfo info, IEnumerable<RocketFormatInfo> externalInfos)
        {
            var w = new CodeWriter();

            // usingの宣言を行う。
            {
                var hashSet = new HashSet<string>();

                // デフォルトで以下の名前空間をusingする。
                hashSet.Add("System");
                hashSet.Add("System.Collections.Generic");
                hashSet.Add("System.Collections.ObjectModel");
                hashSet.Add("System.Buffers");
                hashSet.Add("Omnix.Base");
                hashSet.Add("Omnix.Base.Helpers");
                hashSet.Add("Omnix.Serialization");
                hashSet.Add("Omnix.Serialization.RocketPack");

                // 任意の名前空間をusingする。
                hashSet.UnionWith(info.Options.Where(n => n.Name == "csharp_using").Select(n => n.Value.Trim()));

                // ロードされた*.rpfファイルの名前空間をusingする
                hashSet.UnionWith(externalInfos.SelectMany(n => n.Options.Where(m => m.Name == "csharp_namespace").Select(m => m.Value.Trim())));

                var sortedList = hashSet.ToList();
                sortedList.Sort();

                foreach (var name in sortedList)
                {
                    w.WriteLine($"using {name};");
                }
            }

            w.WriteLine();

            // namespaceの宣言を行う。
            {
                var option = info.Options.First(n => n.Name == "csharp_namespace");

                w.WriteLine($"namespace {option.Value}");
            }

            w.WriteLine("{");
            w.PushIndent();

            object customTypeResolver(CustomTypeInfo n)
            {
                foreach (var targetInfo in new[] { info }.Union(externalInfos))
                {
                    var enumInfo = targetInfo.Enums.FirstOrDefault(m => m.Name == n.TypeName);
                    if (enumInfo != null) return enumInfo;

                    var messageInfo = targetInfo.Messages.FirstOrDefault(m => m.Name == n.TypeName);
                    if (messageInfo != null) return messageInfo;
                }

                return null;
            }

            var enumWriter = new EnumWriter();
            var classWriter = new ClassWriter(customTypeResolver);
            var structWriter = new StructWriter(customTypeResolver);

            foreach (var enumInfo in info.Enums)
            {
                // Enum
                enumWriter.Write(w, enumInfo);

                w.WriteLine();
            }

            foreach (var messageInfo in info.Messages)
            {
                if (messageInfo.FormatType == MessageFormatType.Medium)
                {
                    // Class
                    classWriter.Write(w, messageInfo);
                }
                else if (messageInfo.FormatType == MessageFormatType.Small)
                {
                    // Struct
                    structWriter.Write(w, messageInfo);
                }

                w.WriteLine();
            }

            w.PopIndent();
            w.WriteLine("}");

            return w.ToString();
        }

        /// <summary>
        /// プロパティ名からフィールド変数名を生成します。
        /// </summary>
        private static string GetFieldName(string name)
        {
            return name[0].ToString().ToLower() + name.Substring(1);
        }

        /// <summary>
        /// 最大サイズを示すフィールド変数名を生成します。
        /// </summary>
        private static string GetMaxLengthFieldName(MessageElementInfo element)
        {
            switch (element.Type)
            {
                case StringTypeInfo typeInfo:
                    return $"Max{element.Name}Length";
                case MemoryTypeInfo typeInfo:
                    return $"Max{element.Name}Length";
                case ListTypeInfo typeInfo:
                    return $"Max{element.Name}Count";
                case MapTypeInfo typeInfo:
                    return $"Max{element.Name}Count";
                default:
                    return null;
            }
        }

        private class CodeWriter
        {
            private StringBuilder _sb = new StringBuilder();
            private int _indentDepth = 0;
            private bool _wroteIndent = false;

            public CodeWriter()
            {

            }

            private bool TryWriteIndent()
            {
                if (_wroteIndent) return false;
                _wroteIndent = true;

                for (int i = 0; i < _indentDepth; i++)
                {
                    _sb.Append("    ");
                }

                return true;
            }

            public void WriteLine()
            {
                _sb.AppendLine();
                _wroteIndent = false;
            }

            public void WriteLine(string value)
            {
                foreach (var line in value.Split(new string[] { "\r\n", "\r", "\n" }, options: StringSplitOptions.None))
                {
                    this.TryWriteIndent();
                    _sb.AppendLine(line);
                    _wroteIndent = false;
                }
            }

            public void PushIndent()
            {
                _indentDepth++;
            }

            public void PopIndent()
            {
                _indentDepth--;
            }

            public override string ToString()
            {
                return _sb.ToString();
            }
        }
    }
}
