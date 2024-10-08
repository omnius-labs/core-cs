using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Cocona;
using Microsoft.Extensions.Logging;
using Omnius.Core.RocketPack.DefinitionCompiler.Configuration;
using Omnius.Core.RocketPack.DefinitionCompiler.Internal;

namespace Omnius.Core.RocketPack.DefinitionCompiler;

public class Program
{
    public static void Main(string[] args)
    {
        CoconaLiteApp.Run<Program>(args);
    }

    public void Compile([Option("config", ['c'])][FilePathExists] string configPath)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .AddDebug();
        });
        var logger = loggerFactory.CreateLogger<Program>();

        var configs = YamlHelper.ReadFile<AppConfig[]>(configPath);

        foreach (var config in configs)
        {
            var includeFiles = GlobFiles(config.Includes);

            foreach (var target in config.CompileTargets ?? throw new NullReferenceException(nameof(config.CompileTargets)))
            {

                try
                {
                    var input = Path.GetFullPath(target.Input ?? throw new NullReferenceException(nameof(target.Input)));
                    var output = Path.GetFullPath(target.Output ?? throw new NullReferenceException(nameof(target.Output)));

                    logger.LogInformation($"Start compile: {input} -> {output}");

                    // 読み込み
                    var (rootDefinition, includedDefinitions) = DefinitionLoader.Load(input, includeFiles);

                    // 出力先フォルダが存在しない場合は作成する
                    CreateParentDirectory(output);

                    // 書き込み
                    using var writer = new StreamWriter(output, false, new UTF8Encoding(false));
                    var codeGenerator = new CodeGenerator(rootDefinition, includedDefinitions);
                    writer.Write(codeGenerator.Generate());
                }
                catch (Exception e)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(CultureInfo.InvariantCulture, $"input: {target.Input}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"output: {target.Output}");

                    logger.LogError(e, sb.ToString());

                    throw;
                }
            }
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
