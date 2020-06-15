using System;
using System.Collections.Generic;
using CommandLine;

namespace Omnius.Core.Serialization.RocketPack.DefinitionCompiler
{
    internal class Options
    {
        [Option('f', "file", Required = true, HelpText = "input file path.")]
        public string Source { get; set; } = string.Empty;

        [Option('o', "output", Required = true, HelpText = "output file path.")]
        public string Output { get; set; } = string.Empty;

        [Option('i', "include", HelpText = "include directory path.")]
        public IEnumerable<string>? Include { get; set; }
    }
}
