#!/bin/bash

dotnet new sln --force
dotnet sln add ./src/**/*.csproj
dotnet sln add ./test/**/*.csproj
