using System;
using System.Collections.Generic;
using System.Linq;
using Omnius.Core.RocketPack.DefinitionCompiler.Models;

namespace Omnius.Core.RocketPack.DefinitionCompiler
{
    internal static partial class CodeGenerator
    {
        public static string Generate(RocketPackDefinition rootDefinition, IEnumerable<RocketPackDefinition> externalDefinitions)
        {
            var b = new CodeBuilder();

            // usingの宣言を行う。
            {
                var hashSet = new HashSet<string>();

                var sortedList = externalDefinitions.Select(n => n.GetCSharpNamespace()).ToList();
                sortedList.Sort();

                foreach (var name in sortedList)
                {
                    if (!hashSet.Add(name)) continue;
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
                b.WriteLine($"namespace {rootDefinition.GetCSharpNamespace()}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    var enumWriter = new EnumWriter(rootDefinition);
                    var objectWriter = new ObjectWriter(rootDefinition, externalDefinitions);
                    var serviceWriter = new ServiceWriter(rootDefinition, externalDefinitions);

                    enumWriter.Write(b);
                    b.WriteLine();

                    objectWriter.Write(b);
                    b.WriteLine();

                    serviceWriter.Write(b);
                    b.WriteLine();
                }

                b.WriteLine("}");
            }

            return b.ToString();
        }
    }
}
