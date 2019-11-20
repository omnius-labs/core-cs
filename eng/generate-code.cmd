setlocal

setx DOTNET_CLI_TELEMETRY_OPTOUT 1

set BIN_DIR=%cd%\bin\tools\win
set TOOL_PATH=%BIN_DIR%\Omnius.Core.Serialization.RocketPack.DefinitionCompiler\Omnius.Core.Serialization.RocketPack.DefinitionCompiler.exe
set INCLUDE=-i %cd%\fmt

"%TOOL_PATH%" %cd%\fmt\FormatterBenchmarks.Internal.rpd %INCLUDE% -o %cd%\perf\FormatterBenchmarks\Internal\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Configuration.Internal.rpd %INCLUDE% -o %cd%\src\Omnius.Core.Configuration\Internal\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Cryptography.rpd %INCLUDE% -o %cd%\src\Omnius.Core.Cryptography\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Network.rpd %INCLUDE% -o %cd%\src\Omnius.Core.Network\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Network.Connections.Secure.rpd %INCLUDE% -o %cd%\src\Omnius.Core.Network\Connections\Secure\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Network.Connections.Secure.Internal.rpd %INCLUDE% -o %cd%\src\Omnius.Core.Network\Connections\Secure\Internal\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Network.Connections.Secure.V1.Internal.rpd %INCLUDE% -o %cd%\src\Omnius.Core.Network\Connections\Secure\V1\Internal\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Network.Connections.Multiplexer.Internal.rpd %INCLUDE% -o %cd%\src\Omnius.Core.Network\Connections\Multiplexer\Internal\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Network.Connections.Multiplexer.V1.Internal.rpd %INCLUDE% -o %cd%\src\Omnius.Core.Network\Connections\Multiplexer\V1\Internal\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Remoting.rpd %INCLUDE% -o %cd%\src\Omnius.Core.Remoting\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnius.Core.Serialization.RocketPack.DefinitionCompiler.Tests.Internal.rpd %INCLUDE% -o %cd%\test\Omnius.Core.Serialization.RocketPack.DefinitionCompiler.Tests\Internal\_RocketPack\Messages.generated.cs
