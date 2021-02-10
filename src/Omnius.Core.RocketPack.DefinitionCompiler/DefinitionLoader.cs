using System.Collections.Generic;
using System.Linq;
using Omnius.Core.RocketPack.DefinitionCompiler.Models;
using Omnius.Core.RocketPack.DefinitionCompiler.Parsers;

namespace Omnius.Core.RocketPack.DefinitionCompiler
{
    internal static class DefinitionLoader
    {
        public static (RocketPackDefinition rootDefinition, IEnumerable<RocketPackDefinition> includedDefinitions) Load(string sourcePath, IEnumerable<string>? includeFiles = null)
        {
            RocketPackDefinition rootDefinition;
            var definitionMap = new Dictionary<string, List<RocketPackDefinition>>();

            // 変換対象の定義ファイル
            {
                var definition = DefinitionParser.Load(sourcePath);
                definitionMap[definition.Namespace.Value] = new List<RocketPackDefinition>() { definition };
                rootDefinition = definition;
            }

            // 変換対象の定義ファイルがUsing可能な定義ファイル群
            if (includeFiles != null)
            {
                var ignoreSet = new HashSet<string>();

                foreach (var path in includeFiles)
                {
                    // 重複排除
                    if (!ignoreSet.Add(path)) continue;

                    var definition = DefinitionParser.Load(path);
                    if (!definitionMap.TryGetValue(definition.Namespace.Value, out var list))
                    {
                        list = new List<RocketPackDefinition>();
                        definitionMap[definition.Namespace.Value] = list;
                    }

                    list.Add(definition);
                }
            }

            var includedDefinitions = new List<RocketPackDefinition>();
            includedDefinitions.Add(rootDefinition);

            // 関連する定義を取得する
            {
                var ignoreSet = new HashSet<string>();

                for (int i = 0; i < includedDefinitions.Count; i++)
                {
                    foreach (var @using in includedDefinitions[i].Usings)
                    {
                        // 既に読み込み済みの場合は読み込まない
                        if (!ignoreSet.Add(@using.Value)) continue;

                        includedDefinitions.AddRange(definitionMap[@using.Value]);
                    }
                }
            }

            return (rootDefinition, includedDefinitions.Skip(1));
        }
    }
}
