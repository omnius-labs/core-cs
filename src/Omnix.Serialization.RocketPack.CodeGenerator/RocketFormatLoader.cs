using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static class RocketFormatLoader
    {
        public static (RocketPackDefinition definition, IEnumerable<RocketPackDefinition> externalDefinitions) Load(string definitionFilePath)
        {
            var usingPathList = new List<string>();
            var loadedPathSet = new HashSet<string>();
            usingPathList.Add(definitionFilePath);
            loadedPathSet.Add(definitionFilePath);

            var results = new List<RocketPackDefinition>();

            for (int i = 0; i < usingPathList.Count; i++)
            {
                RocketPackDefinition tempDefinition;

                using (var reader = new StreamReader(usingPathList[i]))
                {
                    tempDefinition = RocketFormatParser.ParseV1(reader.ReadToEnd());
                    results.Add(tempDefinition);
                }

                {
                    var basePath = Path.GetDirectoryName(usingPathList[i]);

                    foreach (var usingInfo in tempDefinition.Usings)
                    {
                        var targetPath = Path.Combine(basePath, usingInfo.Path);

                        if (!loadedPathSet.Add(targetPath)) continue;
                        usingPathList.Add(targetPath);
                    }
                }
            }

            return (results[0], results.Skip(1));
        }
    }
}
