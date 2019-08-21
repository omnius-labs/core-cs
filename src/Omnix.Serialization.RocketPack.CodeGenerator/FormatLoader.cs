using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static class FormatLoader
    {
        private static RocketPackDefinition LoadFile(string definitionFilePath)
        {
            using var reader = new StreamReader(definitionFilePath);
            return FormatParser.ParseV1_0(reader.ReadToEnd());
        }

        public static (RocketPackDefinition definition, IEnumerable<RocketPackDefinition> includedDefinitions) Load(string sourcePath, IEnumerable<string>? includeDirectoryPathList = null)
        {
            var definition = LoadFile(sourcePath);

            var includedDefinitions = new List<RocketPackDefinition>();

            if (includeDirectoryPathList != null)
            {
                foreach (var directoryPath in includeDirectoryPathList)
                {
                    foreach (var path in Directory.GetFiles(directoryPath))
                    {
                        var externalDefinition = LoadFile(path);

                        // SourceでUsingされていない名前空間の定義は読み込まない。
                        if (!definition.Usings.Any(n => n.Name == externalDefinition.Namespace.Name))
                        {
                            continue;
                        }

                        includedDefinitions.Add(externalDefinition);
                    }
                }
            }

            return (definition, includedDefinitions);
        }
    }
}
