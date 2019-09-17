setlocal

setx DOTNET_CLI_TELEMETRY_OPTOUT 1

set BIN_DIR=%cd%\bin\tools\win
set TOOL_PATH=%BIN_DIR%\Omnix.Serialization.OmniPack.CodeGenerator\Omnix.Serialization.OmniPack.CodeGenerator.exe
set INCLUDE=-i %cd%\fmt

"%TOOL_PATH%" %cd%\fmt\FormatterBenchmarks.Internal.rpf %INCLUDE% -o %cd%\perf\FormatterBenchmarks\Internal\_OmniPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Configuration.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Configuration\Internal\_OmniPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Algorithms.Cryptography.rpf %INCLUDE% -o %cd%\src\Omnix.Algorithms\Cryptography\_OmniPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Network.rpf %INCLUDE% -o %cd%\src\Omnix.Network\_OmniPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Secure.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Secure\_OmniPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Secure.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Secure\Internal\_OmniPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Secure.V1.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Secure\V1\Internal\_OmniPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Multiplexer.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Multiplexer\Internal\_OmniPack\Messages.generated.cs
"%TOOL_PATH%" %cd%\fmt\Omnix.Network.Connections.Multiplexer.V1.Internal.rpf %INCLUDE% -o %cd%\src\Omnix.Network.Connections\Multiplexer\V1\Internal\_OmniPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Remoting.rpf %INCLUDE% -o %cd%\src\Omnix.Remoting\_OmniPack\Messages.generated.cs

"%TOOL_PATH%" %cd%\fmt\Omnix.Serialization.OmniPack.CodeGenerator.Tests.Internal.rpf %INCLUDE% -o %cd%\test\Omnix.Serialization.OmniPack.CodeGenerator.Tests\Internal\_OmniPack\Messages.generated.cs
