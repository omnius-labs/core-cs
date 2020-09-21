using System;
using System.Collections.Generic;
using System.Linq;
using Omnius.Core.RocketPack.DefinitionCompiler.Models;


namespace Omnius.Core.RocketPack.DefinitionCompiler
{
    internal partial class CodeGenerator
    {
        private sealed class ServiceWriter
        {
            private readonly RocketPackDefinition _rootDefinition;
            private readonly IList<RocketPackDefinition> _externalDefinitions;
            private readonly string _accessLevel;

            public ServiceWriter(RocketPackDefinition rootDefinition, IEnumerable<RocketPackDefinition> externalDefinitions)
            {
                _rootDefinition = rootDefinition;
                _externalDefinitions = externalDefinitions.ToList();

                var accessLevelOption = _rootDefinition.Options.FirstOrDefault(n => n.Name == "csharp_access_level");
                _accessLevel = accessLevelOption?.Value as string ?? "public";
            }

            private object? FindDefinition(CustomType customType)
            {
                foreach (var definition in new[] { _rootDefinition }.Union(_externalDefinitions))
                {
                    var enumDefinitiom = definition.Enums.FirstOrDefault(m => m.Name == customType.TypeName);
                    if (enumDefinitiom != null)
                    {
                        return enumDefinitiom;
                    }

                    var objectDefinition = definition.Objects.FirstOrDefault(m => m.Name == customType.TypeName);
                    if (objectDefinition != null)
                    {
                        return objectDefinition;
                    }
                }

                return null;
            }

            private static string GenerateTypeFullName(string name, params string[] types)
            {
                var result = name switch
                {
                    "AsyncDisposableBase" => "Omnius.Core.AsyncDisposableBase",
                    "ArgumentNullException" => "System.ArgumentNullException",
                    "ArgumentOutOfRangeException" => "System.ArgumentOutOfRangeException",
                    "Array" => "System.Array",
                    "BytesOperations" => "Omnius.Core.BytesOperations",
                    "CancellationToken" => $"System.Threading.CancellationToken",
                    "CollectionHelper" => "Omnius.Core.Helpers.CollectionHelper",
                    "Dictionary<,>" => $"System.Collections.Generic.Dictionary<{types[0]}, {types[1]}>",
                    "FormatException" => "System.FormatException",
                    "HashCode" => "System.HashCode",
                    "IBufferWriter<>" => $"System.Buffers.IBufferWriter<{types[0]}>",
                    "IBytesPool" => "Omnius.Core.IBytesPool",
                    "IConnection" => $"Omnius.Core.Network.Connections.IConnection",
                    "IDisposable" => "System.IDisposable",
                    "IMemoryOwner<>" => $"System.Buffers.IMemoryOwner<{types[0]}>",
                    "IRocketPackFormatter<>" => $"Omnius.Core.RocketPack.IRocketPackFormatter<{types[0]}>",
                    "IRocketPackObject<>" => $"Omnius.Core.RocketPack.IRocketPackObject<{types[0]}>",
                    "Lazy<>" => $"System.Lazy<{types[0]}>",
                    "Memory<>" => $"System.Memory<{types[0]}>",
                    "MemoryOwner<>" => $"Omnius.Core.MemoryOwner<{types[0]}>",
                    "ObjectHelper" => "Omnius.Core.Helpers.ObjectHelper",
                    "ReadOnlyDictionarySlim<,>" => $"Omnius.Core.Collections.ReadOnlyDictionarySlim<{types[0]}, {types[1]}>",
                    "ReadOnlyListSlim<>" => $"Omnius.Core.Collections.ReadOnlyListSlim<{types[0]}>",
                    "ReadOnlyMemory<>" => $"System.ReadOnlyMemory<{types[0]}>",
                    "ReadOnlySequence<>" => $"System.Buffers.ReadOnlySequence<{types[0]}>",
                    "ReadOnlySpan<>" => $"System.ReadOnlySpan<{types[0]}>",
                    "RocketPackMessageBase<>" => $"Omnius.Core.RocketPack.RocketPackMessageBase<{types[0]}>",
                    "RocketPackReader" => "Omnius.Core.RocketPack.RocketPackReader",
                    "RocketPackRpc" => $"Omnius.Core.RocketPack.Remoting.RocketPackRpc",
                    "RocketPackWriter" => "Omnius.Core.RocketPack.RocketPackWriter",
                    "Span<>" => $"System.Span<{types[0]}>",
                    "Task" => $"System.Threading.Tasks.Task",
                    "Task<>" => $"System.Threading.Tasks.Task<{types[0]}>",
                    "Timestamp" => "Omnius.Core.RocketPack.Timestamp",
                    "ValueTask" => $"System.Threading.Tasks.ValueTask",
                    "ValueTask<>" => $"System.Threading.Tasks.ValueTask<{types[0]}>",
                    _ => throw new InvalidOperationException(name)
                };

                return "global::" + result;
            }

            public void Write(CodeBuilder b)
            {
                foreach (var serviceDefinition in _rootDefinition.Services)
                {
                    this.Write_Interface(b, serviceDefinition);
                    this.Write_Sender(b, serviceDefinition);
                    this.Write_Receiver(b, serviceDefinition);
                }
            }

            public void Write_Interface(CodeBuilder b, ServiceDefinition serviceDefinition)
            {
                b.WriteLine($"{_accessLevel} interface {serviceDefinition.CSharpInterfaceName}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    foreach (var func in serviceDefinition.Elements)
                    {
                        if (func.InType is not null && func.OutType is not null)
                        {
                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef
                                && this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                            {
                                b.WriteLine($"{GenerateTypeFullName("ValueTask<>", outTypeObjectDef.CSharpFullName)} {func.CSharpFunctionName}({inTypeObjectDef.CSharpFullName} param, {GenerateTypeFullName("CancellationToken")} cancellationToken);");
                            }
                        }
                        else if (func.InType is null && func.OutType is not null)
                        {
                            if (this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                            {
                                b.WriteLine($"{GenerateTypeFullName("ValueTask<>", outTypeObjectDef.CSharpFullName)} {func.CSharpFunctionName}({GenerateTypeFullName("CancellationToken")} cancellationToken);");
                            }
                        }
                        else if (func.InType is not null && func.OutType is null)
                        {
                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef)
                            {
                                b.WriteLine($"{GenerateTypeFullName("ValueTask")} {func.CSharpFunctionName}({inTypeObjectDef.CSharpFullName} param, {GenerateTypeFullName("CancellationToken")} cancellationToken);");
                            }
                        }
                        else if (func.InType is null && func.OutType is null)
                        {
                            {
                                b.WriteLine($"{GenerateTypeFullName("ValueTask")} {func.CSharpFunctionName}({GenerateTypeFullName("CancellationToken")} cancellationToken);");
                            }
                        }
                    }
                }

                b.WriteLine("}");
            }

            public void Write_Sender(CodeBuilder b, ServiceDefinition serviceDefinition)
            {
                var className = serviceDefinition.Name + "Sender";
                b.WriteLine($"{_accessLevel} class { className } : { GenerateTypeFullName("AsyncDisposableBase") }, { serviceDefinition.CSharpInterfaceFullName }");
                b.WriteLine("{");
                using (b.Indent())
                {
                    b.WriteLine($"private readonly { serviceDefinition.CSharpInterfaceFullName } _impl;");
                    b.WriteLine($"private readonly { GenerateTypeFullName("IConnection") } _connection;");
                    b.WriteLine($"private readonly { GenerateTypeFullName("IBytesPool") } _bytesPool;");
                    b.WriteLine($"private readonly { GenerateTypeFullName("RocketPackRpc") } _rpc;");
                    b.WriteLine($"public { className }({ serviceDefinition.CSharpInterfaceFullName } impl, { GenerateTypeFullName("IConnection") } connection, { GenerateTypeFullName("IBytesPool") } bytesPool)");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("_impl = impl;");
                        b.WriteLine("_connection = connection;");
                        b.WriteLine("_bytesPool = bytesPool;");
                        b.WriteLine($"_rpc = new { GenerateTypeFullName("RocketPackRpc") }(_connection, _bytesPool);");
                    }
                    b.WriteLine("}");
                    b.WriteLine($"protected override async {GenerateTypeFullName("ValueTask")} OnDisposeAsync()");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("await _rpc.DisposeAsync();");
                    }
                    b.WriteLine("}");

                    foreach (var (index, func) in serviceDefinition.Elements.Select((n, i) => (i, n)))
                    {
                        if (func.InType is not null && func.OutType is not null)
                        {
                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef
                                && this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                            {
                                b.WriteLine($"public async {GenerateTypeFullName("ValueTask<>", outTypeObjectDef.CSharpFullName)} {func.CSharpFunctionName}({inTypeObjectDef.CSharpFullName} param, {GenerateTypeFullName("CancellationToken")} cancellationToken)");
                                b.WriteLine("{");
                                using (b.Indent())
                                {
                                    b.WriteLine($"using var stream = await _rpc.ConnectAsync({ index }, cancellationToken);");
                                    b.WriteLine($"return await stream.CallFunctionAsync<{ inTypeObjectDef.CSharpFullName }, { outTypeObjectDef.CSharpFullName }>(param, cancellationToken);");
                                }
                                b.WriteLine("}");
                            }
                        }
                        else if (func.InType is null && func.OutType is not null)
                        {
                            if (this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                            {
                                b.WriteLine($"public async {GenerateTypeFullName("ValueTask<>", outTypeObjectDef.CSharpFullName)} {func.CSharpFunctionName}({GenerateTypeFullName("CancellationToken")} cancellationToken)");
                                b.WriteLine("{");
                                using (b.Indent())
                                {
                                    b.WriteLine($"using var stream = await _rpc.ConnectAsync({ index }, cancellationToken);");
                                    b.WriteLine($"return await stream.CallFunctionAsync<{ outTypeObjectDef.CSharpFullName }>(cancellationToken);");
                                }
                                b.WriteLine("}");
                            }
                        }
                        else if (func.InType is not null && func.OutType is null)
                        {
                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef)
                            {
                                b.WriteLine($"public async {GenerateTypeFullName("ValueTask")} {func.CSharpFunctionName}({inTypeObjectDef.CSharpFullName} param, {GenerateTypeFullName("CancellationToken")} cancellationToken)");
                                b.WriteLine("{");
                                using (b.Indent())
                                {
                                    b.WriteLine($"using var stream = await _rpc.ConnectAsync({ index }, cancellationToken);");
                                    b.WriteLine($"await stream.CallActionAsync<{ inTypeObjectDef.CSharpFullName }>(param, cancellationToken);");
                                }
                                b.WriteLine("}");
                            }
                        }
                        else if (func.InType is null && func.OutType is null)
                        {
                            {
                                b.WriteLine($"public async {GenerateTypeFullName("ValueTask")} {func.CSharpFunctionName}({GenerateTypeFullName("CancellationToken")} cancellationToken)");
                                b.WriteLine("{");
                                using (b.Indent())
                                {
                                    b.WriteLine($"using var stream = await _rpc.ConnectAsync({ index }, cancellationToken);");
                                    b.WriteLine($"await stream.CallActionAsync(cancellationToken);");
                                }
                                b.WriteLine("}");
                            }
                        }
                    }
                }
                b.WriteLine("}");
            }

            public void Write_Receiver(CodeBuilder b, ServiceDefinition serviceDefinition)
            {
                var className = serviceDefinition.Name + "Receiver";
                b.WriteLine($"{_accessLevel} class { className } : { GenerateTypeFullName("AsyncDisposableBase") }");
                b.WriteLine("{");
                using (b.Indent())
                {
                    b.WriteLine($"private readonly { serviceDefinition.CSharpInterfaceFullName } _impl;");
                    b.WriteLine($"private readonly { GenerateTypeFullName("IConnection") } _connection;");
                    b.WriteLine($"private readonly { GenerateTypeFullName("IBytesPool") } _bytesPool;");
                    b.WriteLine($"private readonly { GenerateTypeFullName("RocketPackRpc") } _rpc;");
                    b.WriteLine($"public { className }({ serviceDefinition.CSharpInterfaceFullName } impl, { GenerateTypeFullName("IConnection") } connection, { GenerateTypeFullName("IBytesPool") } bytesPool)");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("_impl = impl;");
                        b.WriteLine("_connection = connection;");
                        b.WriteLine("_bytesPool = bytesPool;");
                        b.WriteLine($"_rpc = new { GenerateTypeFullName("RocketPackRpc") }(_connection, _bytesPool);");
                    }
                    b.WriteLine("}");
                    b.WriteLine($"protected override async {GenerateTypeFullName("ValueTask")} OnDisposeAsync()");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("await _rpc.DisposeAsync();");
                    }
                    b.WriteLine("}");

                    b.WriteLine($"public async {GenerateTypeFullName("Task")} EventLoop({ GenerateTypeFullName("CancellationToken") } cancellationToken = default)");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("while (!cancellationToken.IsCancellationRequested)");
                        b.WriteLine("{");
                        using (b.Indent())
                        {
                            b.WriteLine("cancellationToken.ThrowIfCancellationRequested();");
                            b.WriteLine("using var stream = await _rpc.AcceptAsync(cancellationToken);");

                            b.WriteLine("switch (stream.CallId)");
                            b.WriteLine("{");
                            using (b.Indent())
                            {
                                foreach (var (index, func) in serviceDefinition.Elements.Select((n, i) => (i, n)))
                                {
                                    b.WriteLine($"case { index }:");
                                    using (b.Indent())
                                    {
                                        b.WriteLine("{");
                                        if (func.InType is not null && func.OutType is not null)
                                        {
                                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef
                                                && this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                                            {
                                                using (b.Indent())
                                                {
                                                    b.WriteLine($"await stream.ListenFunctionAsync<{ inTypeObjectDef.CSharpFullName }, { outTypeObjectDef.CSharpFullName }>(_impl.{ func.CSharpFunctionName }, cancellationToken);");
                                                }
                                            }
                                        }
                                        else if (func.InType is null && func.OutType is not null)
                                        {
                                            if (this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                                            {
                                                using (b.Indent())
                                                {
                                                    b.WriteLine($"await stream.ListenFunctionAsync<{ outTypeObjectDef.CSharpFullName }>(_impl.{ func.CSharpFunctionName }, cancellationToken);");
                                                }
                                            }
                                        }
                                        else if (func.InType is not null && func.OutType is null)
                                        {
                                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef)
                                            {
                                                using (b.Indent())
                                                {
                                                    b.WriteLine($"await stream.ListenActionAsync<{ inTypeObjectDef.CSharpFullName }>(_impl.{ func.CSharpFunctionName }, cancellationToken);");
                                                }
                                            }
                                        }
                                        else if (func.InType is null && func.OutType is null)
                                        {
                                            {
                                                using (b.Indent())
                                                {
                                                    b.WriteLine($"await stream.ListenActionAsync(_impl.{ func.CSharpFunctionName }, cancellationToken);");
                                                }
                                            }
                                        }
                                        b.WriteLine("}");
                                        b.WriteLine("break;");
                                    }
                                }
                            }
                            b.WriteLine("}");
                        }
                        b.WriteLine("}");
                    }
                    b.WriteLine("}");
                }
                b.WriteLine("}");
            }
        }
    }
}
