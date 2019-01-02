setlocal

set BAT_DIR=%~dp0

if %PROCESSOR_ARCHITECTURE% == x86 (
    set TOOL_PATH=%BAT_DIR%tools\win-x86\Omnix.Serialization.RocketPack.CodeGenerator\Omnix.Serialization.RocketPack.CodeGenerator.exe
)

if %PROCESSOR_ARCHITECTURE% == AMD64 (
    set TOOL_PATH=%BAT_DIR%tools\win-x64\Omnix.Serialization.RocketPack.CodeGenerator\Omnix.Serialization.RocketPack.CodeGenerator.exe
)

"%TOOL_PATH%" %BAT_DIR%formats\FormatterBenchmarks.rpf %BAT_DIR%\benchmarks\FormatterBenchmarks\_RocketPack\Messages.generated.cs

"%TOOL_PATH%" %BAT_DIR%formats\Omnix.Cryptography.rpf %BAT_DIR%src\Omnix.Cryptography\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %BAT_DIR%formats\Omnix.Network.rpf %BAT_DIR%src\Omnix.Network\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %BAT_DIR%formats\Omnix.Network.Connection.Secure.rpf %BAT_DIR%src\Omnix.Network.Connection\Secure\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %BAT_DIR%formats\Omnix.Network.Connection.Secure.V1.rpf %BAT_DIR%src\Omnix.Network.Connection\Secure\V1\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %BAT_DIR%formats\Omnix.Serialization.RocketPack.CodeGenerator.Tests.rpf %BAT_DIR%\tests\Omnix.Serialization.RocketPack.CodeGenerator.Tests\_RocketPack\Messages.generated.cs
