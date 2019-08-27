setlocal

setx DOTNET_CLI_TELEMETRY_OPTOUT 1

set TOOL_PATH=%cd%\tools\win\Omnix.Serialization.RocketPack.CodeGenerator\Omnix.Serialization.RocketPack.CodeGenerator.exe
set INCLUDE=-i %cd%\formats %cd%\refs\omnix\formats

"%TOOL_PATH%" %cd%\formats\FormatterBenchmarks.Internal.rpf %INCLUDE% -o %cd%\perf\FormatterBenchmarks\Internal\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\formats\Omnix.Configuration.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Configuration\Internal\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\formats\Omnix.Algorithms.Cryptography.rpf %INCLUDE% -o %cd%\src\Omnix.Algorithms\Cryptography\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\formats\Omnix.Network.rpf %INCLUDE% -o %cd%\src\Omnix.Network\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\formats\Omnix.Network.Connections.Secure.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Secure\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\formats\Omnix.Network.Connections.Secure.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Secure\Internal\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\formats\Omnix.Network.Connections.Secure.V1.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Secure\V1\Internal\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\formats\Omnix.Network.Connections.Multiplexer.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Multiplexer\Internal\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\formats\Omnix.Network.Connections.Multiplexer.V1.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Multiplexer\V1\Internal\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\formats\Omnix.Remoting.rpf %INCLUDE% -o %cd%\src\Omnix.Remoting\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\formats\Omnix.Serialization.RocketPack.CodeGenerator.Tests.Internal.rpf %INCLUDE% -o %cd%\tests\Omnix.Serialization.RocketPack.CodeGenerator.Tests\Internal\_RocketPack\Messages.generated.cs
