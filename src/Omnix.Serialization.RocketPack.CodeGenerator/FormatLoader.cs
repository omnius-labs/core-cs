using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static class FormatLoader
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
                var filePath = usingPathList[i];
                var basePath = Path.GetDirectoryName(filePath);
                if (basePath is null) continue;

                RocketPackDefinition tempDefinition;

                using (var reader = new StreamReader(filePath))
                {
                    tempDefinition = FormatParser.ParseV1_0(reader.ReadToEnd());
                    results.Add(tempDefinition);
                }

                foreach (var usingInfo in tempDefinition.Usings)
                {
                    var targetPath = Path.Combine(basePath, usingInfo.Path);

                    if (!loadedPathSet.Add(targetPath))
                    {
                        continue;
                    }

                    usingPathList.Add(targetPath);
                }
            }

            return (results[0], results.Skip(1));
        }
    }
}
