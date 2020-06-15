using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;

namespace Omnius.Core.Serialization.RocketPack.DefinitionCompiler
{
    public class Program
    {
        static void Main(string[] args)
        {
            var commandLineParser = new CommandLine.Parser(x =>
            {
                x.HelpWriter = Console.Error;
            });

            Options? options = null;

            commandLineParser.ParseArguments<Options>(args)
                .WithParsed(x => options = x);

            if (options == null)
            {
                return;
            }

            // 読み込み
            var (rootDefinition, includedDefinitions) = FormatLoader.Load(options.Source, options.Include);

            // 出力先フォルダが存在しない場合は作成する
            {
                var destinationParentDirectoryPath = Path.GetDirectoryName(options.Output);

                if (!Directory.Exists(destinationParentDirectoryPath))
                {
                    Directory.CreateDirectory(destinationParentDirectoryPath);
                }
            }

            using (var writer = new StreamWriter(options.Output, false, new UTF8Encoding(false)))
            {
                writer.Write(CodeGenerator.Generate(rootDefinition, includedDefinitions));
            }
        }
    }
}
