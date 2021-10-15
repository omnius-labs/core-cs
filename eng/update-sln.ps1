dotnet new sln --force -n core
dotnet sln add (ls -r ./src/**/*.csproj)
dotnet sln add (ls -r ./test/**/*.csproj)
