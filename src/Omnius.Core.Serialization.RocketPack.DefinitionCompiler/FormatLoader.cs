using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Omnius.Core.Serialization.RocketPack.DefinitionCompiler
{
    internal static class FormatLoader
    {
        private static RocketPackDefinition LoadFile(string definitionFilePath)
        {
            using var reader = new StreamReader(definitionFilePath);
            return FormatParser.ParseV1_0(reader.ReadToEnd());
        }

        public static (RocketPackDefinition rootDefinition, IEnumerable<RocketPackDefinition> includedDefinitions) Load(string sourcePath, IEnumerable<string>? includeDirectoryPathList = null)
        {
            RocketPackDefinition rootDefinition;
            var definitionMap = new Dictionary<string, RocketPackDefinition>();

            // 変換対象の定義ファイル
            {
                var definition = LoadFile(sourcePath);
                definitionMap[definition.Namespace.Value] = definition;
                rootDefinition = definition;
            }

            // 変換対象の定義ファイルがUsing可能な定義ファイル群
            if (includeDirectoryPathList != null)
            {
                foreach (var directoryPath in includeDirectoryPathList)
                {
                    foreach (var path in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
                    {
                        var definition = LoadFile(path);
                        definitionMap[definition.Namespace.Value] = definition;
                    }
                }
            }

            var includedDefinitions = new List<RocketPackDefinition>();
            includedDefinitions.Add(rootDefinition);

            var loadedNamespaceSet = new HashSet<string>();

            for (int i = 0; i < includedDefinitions.Count; i++)
            {
                // 既に読み込み済みの場合は読み込まない
                if (!loadedNamespaceSet.Add(includedDefinitions[i].Namespace.Value))
                {
                    continue;
                }

                foreach(var @using in includedDefinitions[i].Usings)
                {
                    includedDefinitions.Add(definitionMap[@using.TargetNamespace]);
                }
            }

            return (rootDefinition, includedDefinitions.Skip(1));
        }
    }
}
