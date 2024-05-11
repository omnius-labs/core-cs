using System.Runtime.Serialization;

namespace Core.RocketPack.DefinitionCompiler;

[Serializable]
public class RocketPackDefinitionCompilerException : Exception
{
    public RocketPackDefinitionCompilerException() { }
    public RocketPackDefinitionCompilerException(string? message) : base(message) { }
    public RocketPackDefinitionCompilerException(string message, System.Exception inner) : base(message, inner) { }
}
