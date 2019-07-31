using System;
using System.IO;
using System.Text;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine(@"Usage: Omnix.Serialization.RocketPack.CodeGenerator.exe [DefinitionFilePath] [DestinationFilePath]");
                    return;
                }

                string definitionFilePath = args[0];
                string destinationFilePath = args[1];

                Run(definitionFilePath, destinationFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Run(string definitionFilePath, string destinationFilePath)
        {
            var result = RocketFormatLoader.Load(definitionFilePath);

            // 出力先フォルダが存在しない場合は作成する
            {
                var destinationParentDirectoryPath = Path.GetDirectoryName(destinationFilePath);

                if (!Directory.Exists(destinationParentDirectoryPath))
                {
                    Directory.CreateDirectory(destinationParentDirectoryPath);
                }
            }

            using (var writer = new StreamWriter(destinationFilePath, false, Encoding.UTF8))
            {
                writer.NewLine = "\n";
                writer.Write(RocketCodeGenerator.Generate(result.definition, result.externalDefinitions));
            }
        }
    }
}
