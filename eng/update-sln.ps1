dotnet new sln --force
dotnet sln core.sln add (ls -r ./src/**/*.csproj)
dotnet sln core.sln add (ls -r ./test/**/*.csproj)
