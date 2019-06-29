setlocal

setx DOTNET_CLI_TELEMETRY_OPTOUT 1

mkdir bin

del /Q bin\*.nupkg
del /Q bin\*.snupkg

dotnet pack --configuration Release --output bin