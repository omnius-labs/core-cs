using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
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
            var (info, externalInfos) = RocketFormatLoader.Load(definitionFilePath);

            using (var writer = new StreamWriter(destinationFilePath, false, Encoding.UTF8))
            {
                writer.Write(RocketCodeGenerator.Generate(info, externalInfos));
            }
        }
    }
}
