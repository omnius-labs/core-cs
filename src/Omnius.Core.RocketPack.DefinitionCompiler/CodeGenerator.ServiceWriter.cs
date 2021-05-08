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

            public ServiceWriter(RocketPackDefinition rootDefinition, IEnumerable<RocketPackDefinition> externalDefinitions)
            {
                _rootDefinition = rootDefinition;
                _externalDefinitions = externalDefinitions.ToList();
            }

            public void Write(CodeWriter b, ServiceDefinition serviceDefinition, string accessLevel = "public")
            {
                this.Write_Interface(b, serviceDefinition, accessLevel);
                this.Write_ClientAndServerClass(b, serviceDefinition, accessLevel);
            }

            private void Write_Interface(CodeWriter b, ServiceDefinition serviceDefinition, string accessLevel)
            {
                b.WriteLine($"{accessLevel} interface {serviceDefinition.CSharpInterfaceName}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    foreach (var func in serviceDefinition.Functions)
                    {
                        if (func.InType is not null && func.OutType is not null)
                        {
                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef
                                && this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                            {
                                b.WriteLine($"{GenerateTypeFullName("ValueTask<>", outTypeObjectDef.CSharpFullName)} {func.CSharpFunctionName}({inTypeObjectDef.CSharpFullName} param, {GenerateTypeFullName("CancellationToken")} cancellationToken = default);");
                            }
                        }
                        else if (func.InType is null && func.OutType is not null)
                        {
                            if (this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                            {
                                b.WriteLine($"{GenerateTypeFullName("ValueTask<>", outTypeObjectDef.CSharpFullName)} {func.CSharpFunctionName}({GenerateTypeFullName("CancellationToken")} cancellationToken = default);");
                            }
                        }
                        else if (func.InType is not null && func.OutType is null)
                        {
                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef)
                            {
                                b.WriteLine($"{GenerateTypeFullName("ValueTask")} {func.CSharpFunctionName}({inTypeObjectDef.CSharpFullName} param, {GenerateTypeFullName("CancellationToken")} cancellationToken = default);");
                            }
                        }
                        else if (func.InType is null && func.OutType is null)
                        {
                            {
                                b.WriteLine($"{GenerateTypeFullName("ValueTask")} {func.CSharpFunctionName}({GenerateTypeFullName("CancellationToken")} cancellationToken = default);");
                            }
                        }
                    }
                }

                b.WriteLine("}");
            }

            private void Write_ClientAndServerClass(CodeWriter b, ServiceDefinition serviceDefinition, string accessLevel)
            {
                var className = serviceDefinition.Name;
                b.WriteLine($"{accessLevel} class {className}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    this.Write_ClientClass(b, serviceDefinition);
                    this.Write_ServerClass(b, serviceDefinition);
                }

                b.WriteLine("}");
            }

            private void Write_ClientClass(CodeWriter b, ServiceDefinition serviceDefinition)
            {
                var className = "Client";
                b.WriteLine($"public class {className} : {GenerateTypeFullName("AsyncDisposableBase")}, {serviceDefinition.CSharpInterfaceFullName}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    b.WriteLine($"private readonly {GenerateTypeFullName("IConnection")} _connection;");
                    b.WriteLine($"private readonly {GenerateTypeFullName("IBytesPool")} _bytesPool;");
                    b.WriteLine($"private readonly {GenerateTypeFullName("IRemoting")} _remoting;");
                    b.WriteLine($"public {className}({GenerateTypeFullName("IConnection")} connection, {GenerateTypeFullName("IBytesPool")} bytesPool)");
                    b.WriteLine("{");

                    using (b.Indent())
                    {
                        b.WriteLine("_connection = connection;");
                        b.WriteLine("_bytesPool = bytesPool;");
                        b.WriteLine($"_remoting = {GenerateTypeFullName("Remoting")}.Factory.Create(_connection, {GenerateTypeFullName("RemotingMessenger")}.Factory, {GenerateTypeFullName("RemotingFunction")}.Factory, _bytesPool);");
                    }

                    b.WriteLine("}");
                    b.WriteLine($"protected override async {GenerateTypeFullName("ValueTask")} OnDisposeAsync()");
                    b.WriteLine("{");

                    using (b.Indent())
                    {
                        b.WriteLine("await _remoting.DisposeAsync();");
                    }

                    b.WriteLine("}");

                    foreach (var (index, func) in serviceDefinition.Functions.Select((n, i) => (i + 1, n)))
                    {
                        if (func.InType is not null && func.OutType is not null)
                        {
                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef
                                && this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                            {
                                b.WriteLine($"public async {GenerateTypeFullName("ValueTask<>", outTypeObjectDef.CSharpFullName)} {func.CSharpFunctionName}({inTypeObjectDef.CSharpFullName} param, {GenerateTypeFullName("CancellationToken")} cancellationToken = default)");
                                b.WriteLine("{");

                                using (b.Indent())
                                {
                                    b.WriteLine($"using var function = await _remoting.ConnectAsync({index}, cancellationToken);");
                                    b.WriteLine($"return await function.CallFunctionAsync<{inTypeObjectDef.CSharpFullName}, {outTypeObjectDef.CSharpFullName}>(param, cancellationToken);");
                                }

                                b.WriteLine("}");
                            }
                        }
                        else if (func.InType is null && func.OutType is not null)
                        {
                            if (this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                            {
                                b.WriteLine($"public async {GenerateTypeFullName("ValueTask<>", outTypeObjectDef.CSharpFullName)} {func.CSharpFunctionName}({GenerateTypeFullName("CancellationToken")} cancellationToken = default)");
                                b.WriteLine("{");

                                using (b.Indent())
                                {
                                    b.WriteLine($"using var function = await _remoting.ConnectAsync({index}, cancellationToken);");
                                    b.WriteLine($"return await function.CallFunctionAsync<{outTypeObjectDef.CSharpFullName}>(cancellationToken);");
                                }

                                b.WriteLine("}");
                            }
                        }
                        else if (func.InType is not null && func.OutType is null)
                        {
                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef)
                            {
                                b.WriteLine($"public async {GenerateTypeFullName("ValueTask")} {func.CSharpFunctionName}({inTypeObjectDef.CSharpFullName} param, {GenerateTypeFullName("CancellationToken")} cancellationToken = default)");
                                b.WriteLine("{");

                                using (b.Indent())
                                {
                                    b.WriteLine($"using var function = await _remoting.ConnectAsync({index}, cancellationToken);");
                                    b.WriteLine($"await function.CallActionAsync<{inTypeObjectDef.CSharpFullName}>(param, cancellationToken);");
                                }

                                b.WriteLine("}");
                            }
                        }
                        else if (func.InType is null && func.OutType is null)
                        {
                            {
                                b.WriteLine($"public async {GenerateTypeFullName("ValueTask")} {func.CSharpFunctionName}({GenerateTypeFullName("CancellationToken")} cancellationToken = default)");
                                b.WriteLine("{");

                                using (b.Indent())
                                {
                                    b.WriteLine($"using var function = await _remoting.ConnectAsync({index}, cancellationToken);");
                                    b.WriteLine($"await function.CallActionAsync(cancellationToken);");
                                }

                                b.WriteLine("}");
                            }
                        }
                    }
                }

                b.WriteLine("}");
            }

            private void Write_ServerClass(CodeWriter b, ServiceDefinition serviceDefinition)
            {
                var className = "Server";
                b.WriteLine($"public class {className} : {GenerateTypeFullName("AsyncDisposableBase")}");
                b.WriteLine("{");

                using (b.Indent())
                {
                    b.WriteLine($"private readonly {serviceDefinition.CSharpInterfaceFullName} _service;");
                    b.WriteLine($"private readonly {GenerateTypeFullName("IConnection")} _connection;");
                    b.WriteLine($"private readonly {GenerateTypeFullName("IBytesPool")} _bytesPool;");
                    b.WriteLine($"private readonly {GenerateTypeFullName("IRemoting")} _remoting;");
                    b.WriteLine($"public {className}({serviceDefinition.CSharpInterfaceFullName} service, {GenerateTypeFullName("IConnection")} connection, {GenerateTypeFullName("IBytesPool")} bytesPool)");
                    b.WriteLine("{");

                    using (b.Indent())
                    {
                        b.WriteLine("_service = service;");
                        b.WriteLine("_connection = connection;");
                        b.WriteLine("_bytesPool = bytesPool;");
                        b.WriteLine($"_remoting = {GenerateTypeFullName("Remoting")}.Factory.Create(_connection, {GenerateTypeFullName("RemotingMessenger")}.Factory, {GenerateTypeFullName("RemotingFunction")}.Factory, _bytesPool);");
                    }

                    b.WriteLine("}");
                    b.WriteLine($"protected override async {GenerateTypeFullName("ValueTask")} OnDisposeAsync()");
                    b.WriteLine("{");
                    using (b.Indent())
                    {
                        b.WriteLine("await _remoting.DisposeAsync();");
                    }

                    b.WriteLine("}");

                    b.WriteLine($"public async {GenerateTypeFullName("Task")} EventLoopAsync({GenerateTypeFullName("CancellationToken")} cancellationToken = default)");
                    b.WriteLine("{");

                    using (b.Indent())
                    {
                        b.WriteLine("while (!cancellationToken.IsCancellationRequested)");
                        b.WriteLine("{");

                        using (b.Indent())
                        {
                            b.WriteLine("cancellationToken.ThrowIfCancellationRequested();");
                            b.WriteLine("using var function = await _remoting.AcceptAsync(cancellationToken);");

                            b.WriteLine("switch (function.Id)");
                            b.WriteLine("{");

                            using (b.Indent())
                            {
                                foreach (var (index, func) in serviceDefinition.Functions.Select((n, i) => (i + 1, n)))
                                {
                                    b.WriteLine($"case {index}:");

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
                                                    b.WriteLine($"await function.ListenFunctionAsync<{inTypeObjectDef.CSharpFullName}, {outTypeObjectDef.CSharpFullName}>(_service.{func.CSharpFunctionName}, cancellationToken);");
                                                }
                                            }
                                        }
                                        else if (func.InType is null && func.OutType is not null)
                                        {
                                            if (this.FindDefinition(func.OutType) is ObjectDefinition outTypeObjectDef)
                                            {
                                                using (b.Indent())
                                                {
                                                    b.WriteLine($"await function.ListenFunctionAsync<{outTypeObjectDef.CSharpFullName}>(_service.{func.CSharpFunctionName}, cancellationToken);");
                                                }
                                            }
                                        }
                                        else if (func.InType is not null && func.OutType is null)
                                        {
                                            if (this.FindDefinition(func.InType) is ObjectDefinition inTypeObjectDef)
                                            {
                                                using (b.Indent())
                                                {
                                                    b.WriteLine($"await function.ListenActionAsync<{inTypeObjectDef.CSharpFullName}>(_service.{func.CSharpFunctionName}, cancellationToken);");
                                                }
                                            }
                                        }
                                        else if (func.InType is null && func.OutType is null)
                                        {
                                            using (b.Indent())
                                            {
                                                b.WriteLine($"await function.ListenActionAsync(_service.{func.CSharpFunctionName}, cancellationToken);");
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

            private object? FindDefinition(CustomType customType)
            {
                foreach (var definition in new[] { _rootDefinition }.Union(_externalDefinitions))
                {
                    var enumDefinition = definition.Enums.FirstOrDefault(m => m.Name == customType.TypeName);
                    if (enumDefinition != null) return enumDefinition;

                    var objectDefinition = definition.Objects.FirstOrDefault(m => m.Name == customType.TypeName);
                    if (objectDefinition != null) return objectDefinition;
                }

                return null;
            }
        }
    }
}
