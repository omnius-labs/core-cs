#!/bin/bash

dotnet new sln --force
dotnet sln core.sln add ./src/**/*.csproj
dotnet sln core.sln add ./test/**/*.csproj
