using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static class RocketFormatLoader
    {
        public static (RocketFormatInfo info, IEnumerable<RocketFormatInfo> externalInfos) Load(string definitionFilePath)
        {
            var usingPathList = new List<string>();
            var loadedPathSet = new HashSet<string>();
            usingPathList.Add(definitionFilePath);
            loadedPathSet.Add(definitionFilePath);

            var infos = new List<RocketFormatInfo>();

            for (int i = 0; i < usingPathList.Count; i++)
            {
                RocketFormatInfo tempInfo;

                using (var reader = new StreamReader(usingPathList[i]))
                {
                    tempInfo = RocketFormatParser.ParseV1(reader.ReadToEnd());
                    infos.Add(tempInfo);
                }

                {
                    var basePath = Path.GetDirectoryName(usingPathList[i]);

                    foreach (var usingInfo in tempInfo.Usings)
                    {
                        var targetPath = Path.Combine(basePath, usingInfo.Path);

                        if (!loadedPathSet.Add(targetPath)) continue;
                        usingPathList.Add(targetPath);
                    }
                }
            }

            return (infos[0], infos.Skip(1));
        }
    }
}
