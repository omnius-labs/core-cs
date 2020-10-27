#!/bin/bash

Remove-Item -Path core.sln -Force
dotnet new sln
dotnet sln core.sln add (ls -r ./src/**/*.csproj)
dotnet sln core.sln add (ls -r ./test/**/*.csproj)
