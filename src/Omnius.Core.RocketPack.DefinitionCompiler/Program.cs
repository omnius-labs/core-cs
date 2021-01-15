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
                var includeFiles = GlobFiles(include);

                // 読み込み
                var (rootDefinition, includedDefinitions) = DefinitionLoader.Load(source, includeFiles);

                // 出力先フォルダが存在しない場合は作成する
                CreateParentDirectory(output);

                // 書き込み
                using var writer = new StreamWriter(output, false, new UTF8Encoding(false));
                var codeGenerator = new CodeGenerator(rootDefinition, includedDefinitions);
                writer.Write(codeGenerator.Generate());
            }
            catch (Exception e)
            {
                _logger.Error(e);

                var sb = new StringBuilder();
                sb.AppendLine($"source: {source}");
                sb.AppendLine($"output: {output}");

                if (include is not null)
                {
                    sb.AppendLine($"include: {string.Join(", ", include)}");
                }

                _logger.Error(sb.ToString());

                throw;
            }
        }

        private static void CreateParentDirectory(string output)
        {
            var dirPath = Path.GetDirectoryName(output);

            if (dirPath is not null && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        private static IEnumerable<string> GlobFiles(IEnumerable<string>? include)
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

            return includeFiles;
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
