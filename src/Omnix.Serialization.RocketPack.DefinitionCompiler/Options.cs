using System;
using System.Collections.Generic;
using CommandLine;

namespace Omnix.Serialization.RocketPack.DefinitionCompiler
{
    internal class Options
    {
        [Option('i', "include", HelpText = "include directory path.")]
        public IEnumerable<string>? Include { get; set; }

        [Option('o', "output", Required = true, HelpText = "output file path.")]
        public string Output { get; set; } = string.Empty;

        [Value(0)]
        public string Source { get; set; } = string.Empty;
    }
}
