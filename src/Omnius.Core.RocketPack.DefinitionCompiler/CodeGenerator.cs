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

            var objectWriter = new MessageWriter(_rootDefinition, _externalDefinitions);
            foreach (var objectDefinition in _rootDefinition.Objects)
            {
                objectWriter.Write(b, objectDefinition, accessLevel);
            }

            var serviceWriter = new ServiceWriter(_rootDefinition, _externalDefinitions);
            foreach (var serviceDefinition in _rootDefinition.Services)
            {
                serviceWriter.Write(b, serviceDefinition, accessLevel);
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
            "Utf8Array" => "Omnius.Core.RocketPack.Utf8Array",
            "ArgumentNullException" => "System.ArgumentNullException",
            "ArgumentOutOfRangeException" => "System.ArgumentOutOfRangeException",
            "Array" => "System.Array",
            "AsyncDisposableBase" => "Omnius.Core.AsyncDisposableBase",
            "BytesOperations" => "Omnius.Core.BytesOperations",
            "CancellationToken" => $"System.Threading.CancellationToken",
            "CollectionHelper" => "Omnius.Core.Helpers.CollectionHelper",
            "Dictionary<,>" => $"System.Collections.Generic.Dictionary<{@params[0]}, {@params[1]}>",
            "FormatException" => "System.FormatException",
            "HashCode" => "System.HashCode",
            "IBufferWriter<>" => $"System.Buffers.IBufferWriter<{@params[0]}>",
            "IBytesPool" => "Omnius.Core.IBytesPool",
            "IConnection" => $"Omnius.Core.Net.Connections.IConnection",
            "IDisposable" => "System.IDisposable",
            "IMemoryOwner<>" => $"System.Buffers.IMemoryOwner<{@params[0]}>",
            "IRocketMessage" => $"Omnius.Core.RocketPack.IRocketMessage<{@params[0]}>",
            "IRocketMessage<>" => $"Omnius.Core.RocketPack.IRocketMessage<{@params[0]}>",
            "IRocketMessageFormatter<>" => $"Omnius.Core.RocketPack.IRocketMessageFormatter<{@params[0]}>",
            "IRocketRemotingCallerFactory" => $"Omnius.Core.RocketPack.Remoting.IRocketRemotingCallerFactory<{@params[0]}>",
            "IRocketRemotingListenerFactory" => $"Omnius.Core.RocketPack.Remoting.IRocketRemotingListenerFactory<{@params[0]}>",
            "Lazy<>" => $"System.Lazy<{@params[0]}>",
            "Memory<>" => $"System.Memory<{@params[0]}>",
            "MemoryOwner<>" => $"Omnius.Core.MemoryOwner<{@params[0]}>",
            "ObjectHelper" => "Omnius.Core.Helpers.ObjectHelper",
            "ReadOnlyDictionarySlim<,>" => $"Omnius.Core.Collections.ReadOnlyDictionarySlim<{@params[0]}, {@params[1]}>",
            "ReadOnlyListSlim<>" => $"Omnius.Core.Collections.ReadOnlyListSlim<{@params[0]}>",
            "ReadOnlyMemory<>" => $"System.ReadOnlyMemory<{@params[0]}>",
            "ReadOnlySequence<>" => $"System.Buffers.ReadOnlySequence<{@params[0]}>",
            "ReadOnlySpan<>" => $"System.ReadOnlySpan<{@params[0]}>",
            "RocketMessageReader" => "Omnius.Core.RocketPack.RocketMessageReader",
            "RocketMessageWriter" => "Omnius.Core.RocketPack.RocketMessageWriter",
            "Span<>" => $"System.Span<{@params[0]}>",
            "Task" => $"System.Threading.Tasks.Task",
            "Task<>" => $"System.Threading.Tasks.Task<{@params[0]}>",
            "Timestamp" => "Omnius.Core.RocketPack.Timestamp",
            "ValueTask" => $"System.Threading.Tasks.ValueTask",
            "ValueTask<>" => $"System.Threading.Tasks.ValueTask<{@params[0]}>",
            _ => throw new KeyNotFoundException(name)
        };

        return "global::" + result;
    }
}
