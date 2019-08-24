using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static partial class CodeGenerator
    {
        public static string Generate(RocketPackDefinition definition, IEnumerable<RocketPackDefinition> externalDefinitions)
        {
            var w = new CodeWriter();

            // usingの宣言を行う。
            {
                var hashSet = new HashSet<string>();

                var sortedList = definition.Usings.Select(n => n.TargetNamespace).ToList();
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
            w.WriteLine($"namespace {definition.Namespace.Value}");
            w.WriteLine("{");

            using (w.Indent())
            {
                var enumWriter = new EnumWriter(definition);
                var objectWriter = new ObjectWriter(definition, externalDefinitions);

                foreach (var enumInfo in definition.Enums)
                {
                    enumWriter.Write(w, enumInfo);
                    w.WriteLine();
                }

                foreach (var messageInfo in definition.Objects)
                {
                    objectWriter.Write(w, messageInfo);
                    w.WriteLine();
                }
            }

            w.WriteLine("}");

            return w.ToString();
        }
    }
}
