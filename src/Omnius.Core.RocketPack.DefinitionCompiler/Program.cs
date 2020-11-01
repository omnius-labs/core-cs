using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using Cocona;

namespace Omnius.Core.RocketPack.DefinitionCompiler
{
    public class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            CoconaLiteApp.Run<Program>(args);
        }

        public void Compile([Option('s')][FilePathExists] string source, [Option('o')] string output, [Option('i')] string[]? include = null)
        {
            try
            {
                var includeFiles = new List<string>();

                if (include != null)
                {
                    foreach (var path in include)
                    {
                        var result = Ganss.IO.Glob.Expand(path)
                            .Where(n => !n.Attributes.HasFlag(FileAttributes.Directory))
                            .Select(n => n.FullName)
                            .OrderBy(n => n)
                            .ToArray();
                        includeFiles.AddRange(result);
                    }
                }

                // 読み込み
                var (rootDefinition, includedDefinitions) = DefinitionLoader.Load(source, includeFiles);

                // 出力先フォルダが存在しない場合は作成する
                {
                    var destinationParentDirectoryPath = Path.GetDirectoryName(output);

                    if (destinationParentDirectoryPath is not null && !Directory.Exists(destinationParentDirectoryPath))
                    {
                        Directory.CreateDirectory(destinationParentDirectoryPath);
                    }
                }

                // 書き込み
                using var writer = new StreamWriter(output, false, new UTF8Encoding(false));
                var codeGenerator = new CodeGenerator(rootDefinition, includedDefinitions);
                writer.Write(codeGenerator.Generate());
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        private class FilePathExistsAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
            {
                if (value is string path && (File.Exists(path) || File.Exists(path)))
                {
                    return ValidationResult.Success!;
                }

                return new ValidationResult($"The path '{value}' is not found.");
            }
        }
    }
}
