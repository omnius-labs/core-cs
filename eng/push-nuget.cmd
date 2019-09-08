set NUGET_DIR=%cd%\bin\nuget

dotnet nuget push %NUGET_DIR%\*.nupkg -s https://api.nuget.org/v3/index.json -k %1