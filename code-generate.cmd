setlocal

set BAT_DIR=%~dp0

if %PROCESSOR_ARCHITECTURE% == x86 (
    set TOOL_PATH=%BAT_DIR%tools\win-x86\Omnix.Serialization.RocketPack.CodeGenerator\Omnix.Serialization.RocketPack.CodeGenerator.exe
)

if %PROCESSOR_ARCHITECTURE% == AMD64 (
    set TOOL_PATH=%BAT_DIR%tools\win-x64\Omnix.Serialization.RocketPack.CodeGenerator\Omnix.Serialization.RocketPack.CodeGenerator.exe
)

"%TOOL_PATH%" %BAT_DIR%formats\Omnix.Cryptography.rpf %BAT_DIR%src\Omnix.Cryptography\_RocketPack\Messages.generated.cs
"%TOOL_PATH%" %BAT_DIR%formats\Omnix.Network.Connection.rpf %BAT_DIR%src\Omnix.Network.Connection\_RocketPack\Messages.generated.cs

pause