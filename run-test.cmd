setlocal
cd %~dp0

dotnet restore
dotnet test
powershell -File ./eng/run-test.ps1