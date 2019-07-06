setlocal
cd %~dp0

dotnet restore --force-evaluate
dotnet test
powershell -File ./eng/run-test.ps1