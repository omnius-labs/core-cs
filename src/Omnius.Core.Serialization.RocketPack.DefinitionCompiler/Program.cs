using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Cocona;

namespace Omnius.Core.Serialization.RocketPack.DefinitionCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaLiteApp.Run<Program>(args);
        }

        public void Compile([Option('s')][FilePathExists] string source, [Option('o')] string output, [Option('i')] string[] include)
        {
            // 読み込み
            var (rootDefinition, includedDefinitions) = FormatLoader.Load(source, include);

            // 出力先フォルダが存在しない場合は作成する
            {
                var destinationParentDirectoryPath = Path.GetDirectoryName(output);

                if (!Directory.Exists(destinationParentDirectoryPath))
                {
                    Directory.CreateDirectory(destinationParentDirectoryPath);
                }
            }

            using (var writer = new StreamWriter(output, false, new UTF8Encoding(false)))
            {
                writer.Write(CodeGenerator.Generate(rootDefinition, includedDefinitions));
            }
        }

        private class FilePathExistsAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is string path && (File.Exists(path) || File.Exists(path)))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"The path '{value}' is not found.");
            }
        }
    }
}
