using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnius.Core.Serialization.RocketPack.DefinitionCompiler
{
    internal static partial class CodeGenerator
    {
        public static string Generate(RocketPackDefinition rootDefinition, IEnumerable<RocketPackDefinition> externalDefinitions)
        {
            var b = new CodeBuilder();

            // usingの宣言を行う。
            {
                var hashSet = new HashSet<string>();

                var sortedList = rootDefinition.Usings.Select(n => n.TargetNamespace).ToList();
                sortedList.Sort();

                foreach (var name in sortedList)
                {
                    b.WriteLine($"using {name};");
                }
                b.WriteLine();
            }

            // Nullableを有効化する
            {
                b.WriteLine("#nullable enable");
                b.WriteLine();
            }

            // namespaceの宣言を行う。
            {
                b.WriteLine($"namespace {rootDefinition.Namespace.Value}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    var enumWriter = new EnumWriter(rootDefinition);
                    var objectWriter = new ObjectWriter(rootDefinition, externalDefinitions);

                    foreach (var enumInfo in rootDefinition.Enums)
                    {
                        enumWriter.Write(b, enumInfo);
                        b.WriteLine();
                    }

                    foreach (var messageInfo in rootDefinition.Objects)
                    {
                        objectWriter.Write(b, messageInfo);
                        b.WriteLine();
                    }
                }

                b.WriteLine("}");
            }

            return b.ToString();
        }
    }
}
