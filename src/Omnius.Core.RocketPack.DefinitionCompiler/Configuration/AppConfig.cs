using System.IO;
using System.Net;
using Omnius.Core.RocketPack.DefinitionCompiler.Internal;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Configuration
{
    public class AppConfig
    {
        public string[]? Includes { get; init; }

        public CompileTargetConfig[]? CompileTargets { get; init; }
    }

    public class CompileTargetConfig
    {
        public string? Input { get; init; }

        public string? Output { get; init; }
    }
}
