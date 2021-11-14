using System.Runtime.Serialization;

namespace Omnius.Core.RocketPack.DefinitionCompiler;

[Serializable]
public class RocketPackDefinitionCompilerException : Exception
{
    public RocketPackDefinitionCompilerException()
    {
    }

    public RocketPackDefinitionCompilerException(string message)
        : base(message)
    {
    }

    public RocketPackDefinitionCompilerException(string message, System.Exception inner)
        : base(message, inner)
    {
    }

    protected RocketPackDefinitionCompilerException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
