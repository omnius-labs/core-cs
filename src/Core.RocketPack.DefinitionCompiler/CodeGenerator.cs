using Omnius.Core.RocketPack.DefinitionCompiler.Models;

namespace Omnius.Core.RocketPack.DefinitionCompiler;

internal partial class CodeGenerator
{
    private readonly RocketPackDefinition _rootDefinition;
    private readonly List<RocketPackDefinition> _externalDefinitions;

    public CodeGenerator(RocketPackDefinition rootDefinition, IEnumerable<RocketPackDefinition> externalDefinitions)
    {
        _rootDefinition = rootDefinition;
        _externalDefinitions = externalDefinitions.ToList();
    }

    public string Generate()
    {
        var b = new CodeWriter();

        // 自動生成されたと宣言する
        b.WriteLine("// <auto-generated/>");

        // Nullableを有効化する
        b.WriteLine("#nullable enable");
        b.WriteLine();

        // namespaceの宣言を行う。
        {
            b.WriteLine($"namespace {_rootDefinition.CSharpNamespace};");
            b.WriteLine();

            var accessLevel = this.GetAccessLevel();

            var enumWriter = new EnumWriter();
            foreach (var enumDefinition in _rootDefinition.Enums)
            {
                enumWriter.Write(b, enumDefinition, accessLevel);
            }

            var objectWriter = new ObjectWriter(_rootDefinition, _externalDefinitions);
            foreach (var objectDefinition in _rootDefinition.Objects)
            {
                objectWriter.Write(b, objectDefinition, accessLevel);
            }
        }

        return b.ToString();
    }

    private string GetAccessLevel()
    {
        var accessLevelOption = _rootDefinition.Options.FirstOrDefault(n => n.Name == "csharp_access_level");
        return accessLevelOption?.Value as string ?? "public";
    }

    private static string GenerateTypeFullName(string name, params string[] @params)
    {
        var result = name switch
        {
            "ArgumentNullException" => "System.ArgumentNullException",
            "ArgumentOutOfRangeException" => "System.ArgumentOutOfRangeException",
            "Array" => "System.Array",
            "AsyncDisposableBase" => "Omnius.Core.Base.AsyncDisposableBase",
            "BytesOperations" => "Omnius.Core.Base.BytesOperations",
            "CancellationToken" => $"System.Threading.CancellationToken",
            "CollectionHelper" => "Omnius.Core.Base.Helpers.CollectionHelper",
            "Dictionary<,>" => $"System.Collections.Generic.Dictionary<{@params[0]}, {@params[1]}>",
            "FormatException" => "System.FormatException",
            "HashCode" => "System.HashCode",
            "IBufferWriter<>" => $"System.Buffers.IBufferWriter<{@params[0]}>",
            "IBytesPool" => "Omnius.Core.Base.IBytesPool",
            "IConnection" => $"Omnius.Core.Net.Connections.IConnection",
            "IDisposable" => "System.IDisposable",
            "IMemoryOwner<>" => $"System.Buffers.IMemoryOwner<{@params[0]}>",
            "ImmutableDictionary" => $"System.Collections.Immutable.ImmutableDictionary",
            "ImmutableDictionary<,>" => $"System.Collections.Immutable.ImmutableDictionary<{@params[0]}, {@params[1]}>",
            "ImmutableList" => $"System.Collections.Immutable.ImmutableList",
            "ImmutableList<>" => $"System.Collections.Immutable.ImmutableList<{@params[0]}>",
            "RocketMessage<>" => $"Omnius.Core.RocketPack.RocketMessage<{@params[0]}>",
            "IRocketMessageSerializer<>" => $"Omnius.Core.RocketPack.IRocketMessageSerializer<{@params[0]}>",
            "IRocketRemotingCallerFactory" => $"Omnius.Core.RocketPack.Remoting.IRocketRemotingCallerFactory<{@params[0]}>",
            "IRocketRemotingListenerFactory" => $"Omnius.Core.RocketPack.Remoting.IRocketRemotingListenerFactory<{@params[0]}>",
            "Lazy<>" => $"System.Lazy<{@params[0]}>",
            "Memory<>" => $"System.Memory<{@params[0]}>",
            "MemoryOwner<>" => $"Omnius.Core.Base.MemoryOwner<{@params[0]}>",
            "ObjectHelper" => "Omnius.Core.Base.Helpers.ObjectHelper",
            "ReadOnlyMemory<>" => $"System.ReadOnlyMemory<{@params[0]}>",
            "ReadOnlySequence<>" => $"System.Buffers.ReadOnlySequence<{@params[0]}>",
            "ReadOnlySpan<>" => $"System.ReadOnlySpan<{@params[0]}>",
            "RocketMessageReader" => "Omnius.Core.RocketPack.RocketMessageReader",
            "RocketMessageWriter" => "Omnius.Core.RocketPack.RocketMessageWriter",
            "Span<>" => $"System.Span<{@params[0]}>",
            "Task" => $"System.Threading.Tasks.Task",
            "Task<>" => $"System.Threading.Tasks.Task<{@params[0]}>",
            "Timestamp64" => "Omnius.Core.RocketPack.Timestamp64",
            "Timestamp96" => "Omnius.Core.RocketPack.Timestamp96",
            "Utf8String" => "Omnius.Core.RocketPack.Utf8String",
            "ValueTask" => $"System.Threading.Tasks.ValueTask",
            "ValueTask<>" => $"System.Threading.Tasks.ValueTask<{@params[0]}>",
            _ => throw new KeyNotFoundException(name)
        };

        return "global::" + result;
    }
}
