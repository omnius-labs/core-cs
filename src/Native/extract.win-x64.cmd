setlocal

set BAT_DIR=%~dp0
cd %BAT_DIR%

copy /Y /B "C++\Omnix_Base\Omnix_Base\bin\win-x64\Release\Omnix_Base.dll" "..\Omnix.Base\Assemblies\Omnix.Base.win-x64.dll"
copy /Y /B "C++\Omnix_Correction\Omnix_Correction\bin\win-x64\Release\Omnix_Correction.dll" "..\Omnix.Correction\Assemblies\Omnix.Correction.win-x64.dll"
copy /Y /B "C++\Omnix_Cryptography\Omnix_Cryptography\bin\win-x64\Release\Omnix_Cryptography.dll" "..\Omnix.Cryptography\Assemblies\Omnix.Cryptography.win-x64.dll"

copy /Y /B "Rust\hashcash\target\release\hashcash.exe" "..\Omnix.Cryptography\Assemblies\hashcash.win-x64.exe"
