setlocal

setx DOTNET_CLI_TELEMETRY_OPTOUT 1

set NUGET_DIR=%cd%\bin\nuget

mkdir %NUGET_DIR%

del /Q %NUGET_DIR%\*.nupkg
del /Q %NUGET_DIR%\*.snupkg

dotnet pack --configuration Release --output %NUGET_DIR%