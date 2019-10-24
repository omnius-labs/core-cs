setlocal

setx DOTNET_CLI_TELEMETRY_OPTOUT 1

set BIN_DIR=%cd%\bin\tools\win
set TOOL_PATH=%BIN_DIR%\Omnix.Serialization.RocketPack.DefinitionCompiler\Omnix.Serialization.RocketPack.DefinitionCompiler.exe
set INCLUDE=-i %cd%\fmt

"%TOOL_PATH%" %cd%\fmt\FormatterBenchmarks.Internal.rpd %INCLUDE% -o %cd%\perf\FormatterBenchmarks\Internal\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Configuration.Internal.rpd %INCLUDE% -o %cd%\src\Omnix.Configuration\Internal\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Cryptography.rpd %INCLUDE% -o %cd%\src\Omnix\Cryptography\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Network.rpd %INCLUDE% -o %cd%\src\Omnix.Network\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Secure.rpd %INCLUDE% -o %cd%\src\Omnix.Network\Connections\Secure\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Secure.Internal.rpd %INCLUDE% -o %cd%\src\Omnix.Network\Connections\Secure\Internal\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Secure.V1.Internal.rpd %INCLUDE% -o %cd%\src\Omnix.Network\Connections\Secure\V1\Internal\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Multiplexer.Internal.rpd %INCLUDE% -o %cd%\src\Omnix.Network\Connections\Multiplexer\Internal\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Multiplexer.V1.Internal.rpd %INCLUDE% -o %cd%\src\Omnix.Network\Connections\Multiplexer\V1\Internal\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Remoting.rpd %INCLUDE% -o %cd%\src\Omnix.Remoting\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Serialization.RocketPack.DefinitionCompiler.Tests.Internal.rpd %INCLUDE% -o %cd%\test\Omnix.Serialization.RocketPack.DefinitionCompiler.Tests\Internal\_RocketPack\Messages.generated.cs
