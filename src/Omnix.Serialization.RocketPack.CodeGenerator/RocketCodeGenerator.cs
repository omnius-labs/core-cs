using System;
using System.Collections.Generic;
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
            w.WriteLine($"namespace {definition.Options.First(n => n.Name == "csharp_namespace").Value}");
            w.WriteLine("{");

            using (w.Indent())
            {
                var enumWriter = new EnumWriter(definition);
                var messageWriter = new MessageWriter(definition, externalDefinitions);

                foreach (var enumInfo in definition.Enums)
                {
                    enumWriter.Write(w, enumInfo);
                    w.WriteLine();
                }

                foreach (var messageInfo in definition.Messages)
                {
                    messageWriter.Write(w, messageInfo);
                    w.WriteLine();
                }
            }

            w.WriteLine("}");

            return w.ToString();
        }
    }
}
