using System.IO;
using System.Text;
using CommandLine;

namespace Omnix.Serialization.RocketPack.DefinitionCompiler
{
    public class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);

            if (result.Tag == ParserResultType.Parsed)
            {
                // パース成功時
                var parsed = (Parsed<Options>)result;

                // 読み込み
                var (rootDefinition, includedDefinitions) = FormatLoader.Load(parsed.Value.Source, parsed.Value.Include);

                // 出力先フォルダが存在しない場合は作成する
                {
                    var destinationParentDirectoryPath = Path.GetDirectoryName(parsed.Value.Output);

                    if (!Directory.Exists(destinationParentDirectoryPath))
                    {
                        Directory.CreateDirectory(destinationParentDirectoryPath);
                    }
                }

                using (var writer = new StreamWriter(parsed.Value.Output, false, new UTF8Encoding(false)))
                {
                    writer.Write(CodeGenerator.Generate(rootDefinition, includedDefinitions));
                }
            }
            else
            {
                // パース失敗時
                return;
            }
        }
    }
}
