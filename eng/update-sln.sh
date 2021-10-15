#!/bin/bash

dotnet new sln --force -n core
dotnet sln add ./src/**/*.csproj
dotnet sln add ./test/**/*.csproj
