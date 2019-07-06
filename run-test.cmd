setlocal
cd %~dp0

dotnet restore --locked-mode
dotnet test
powershell -File ./eng/run-test.ps1