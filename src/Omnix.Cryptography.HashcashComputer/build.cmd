setx DOTNET_CLI_TELEMETRY_OPTOUT 1

setlocal

set BAT_DIR=%~dp0
set DIST_DIR=%BAT_DIR%dist\

mkdir %DIST_DIR%

dotnet publish %BAT_DIR%Omnix.Cryptography.HashcashComputer.csproj --configuration Release --output "%DIST_DIR%win-x64" --runtime win-x64
dotnet publish %BAT_DIR%Omnix.Cryptography.HashcashComputer.csproj --configuration Release --output "%DIST_DIR%linux-x64" --runtime linux-x64
