using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static partial class RocketCodeGenerator
    {
        public static string Generate(RocketPackDefinition definition, IEnumerable<RocketPackDefinition> externalDefinitions)
        {
            var w = new CodeWriter();

            // usingの宣言を行う。
            {
                var hashSet = new HashSet<string>();

                // ロードされた*.rpfファイルの名前空間をusingする
                hashSet.UnionWith(externalDefinitions.SelectMany(n => n.Options.Where(m => m.Name == "csharp_namespace").Select(m => m.Value.Trim())));

                var sortedList = hashSet.ToList();
                sortedList.Sort();

                foreach (var name in sortedList)
                {
                    w.WriteLine($"using {name};");
                }
            }

            w.WriteLine();

            w.WriteLine("#nullable enable");

            w.WriteLine();

            // namespaceの宣言を行う。
            {
                var option = definition.Options.First(n => n.Name == "csharp_namespace");

                w.WriteLine($"namespace {option.Value}");
            }

            w.WriteLine("{");
            w.PushIndent();

            var enumWriter = new EnumWriter(definition);
            var classWriter = new ClassWriter(definition, externalDefinitions);
            var structWriter = new StructWriter(definition, externalDefinitions);

            foreach (var enumInfo in definition.Enums)
            {
                // Enum
                enumWriter.Write(w, enumInfo);

                w.WriteLine();
            }

            foreach (var messageInfo in definition.Messages)
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

        private static string GetFullName(string name, params string[] types)
        {
            return name switch
            {
                "ReadOnlySequence<>" => $"System.Buffers.ReadOnlySequence<{types[0]}>",
                "IBufferWriter<>" => $"System.Buffers.IBufferWriter<{types[0]}>",
                "IEquatable<>" => $"System.IEquatable<{types[0]}>",
                "RocketPackReader" => "Omnix.Serialization.RocketPack.RocketPackReader",
                "RocketPackWriter" => "Omnix.Serialization.RocketPack.RocketPackWriter",
                "IRocketPackFormatter<>" => $"Omnix.Serialization.RocketPack.IRocketPackFormatter<{types[0]}>",
                "FormatException" => "System.FormatException",
                "BytesOperations" => "Omnix.Base.BytesOperations",
                "CollectionHelper" => "Omnix.Base.Helpers.CollectionHelper",
                "ObjectHelper" => "Omnix.Base.Helpers.ObjectHelper",
                "HashCode" => "System.HashCode",
                "Array" => "System.Array",
                "Timestamp" => "Omnix.Serialization.RocketPack.Timestamp",
                "IMemoryOwner<>" => $"System.Buffers.IMemoryOwner<{types[0]}>",
                "Span<>" => $"System.Span<{types[0]}>",
                "ReadOnlySpan<>" => $"System.ReadOnlySpan<{types[0]}>",
                "Memory<>" => $"System.Memory<{types[0]}>",
                "ReadOnlyMemory<>" => $"System.ReadOnlyMemory<{types[0]}>",
                "ReadOnlyListSlim<>" => $"Omnix.Collections.ReadOnlyListSlim<{types[0]}>",
                "ReadOnlyDictionarySlim<,>" => $"Omnix.Collections.ReadOnlyDictionarySlim<{types[0]}, {types[1]}>",
                "Dictionary<,>" => $"System.Collections.Generic.Dictionary<{types[0]}, {types[1]}>",
                "RocketPackMessageBase<>" => $"Omnix.Serialization.RocketPack.RocketPackMessageBase<{types[0]}>",
                "IDisposable" => "System.IDisposable",
                "BufferPool" => "Omnix.Base.BufferPool",
                "MemoryOwner<>" => $"Omnix.Base.SimpleMemoryOwner<{types[0]}>",
                "ArgumentNullException" => "System.ArgumentNullException",
                "ArgumentOutOfRangeException" => "System.ArgumentOutOfRangeException",
                _ => throw new InvalidOperationException(name)
            };
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
