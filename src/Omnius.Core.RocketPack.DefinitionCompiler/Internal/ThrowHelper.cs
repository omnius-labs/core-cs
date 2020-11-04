using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Internal
{
    internal sealed partial class ThrowHelper
    {
        public static Exception CreateRocketPackDefinitionCompilerException_DefinitionNotFound(string name)
        {
            return new RocketPackDefinitionCompilerException($"Definition not found ({name})");
        }

        public static Exception CreateRocketPackDefinitionCompilerException_NotOneDefinitionFound(string name)
        {
            return new RocketPackDefinitionCompilerException($"Not one definition found ({name})");
        }
    }
}
