set BATDIR=%~dp0
cd %BATDIR%

copy /Y /B "C++\Omnix_Base\Omnix_Base\bin\win-x86\Release\Omnix_Base.dll" "..\Omnix.Base\Assemblies\Omnix_Base_x86.dll"
copy /Y /B "C++\Omnix_Base\Omnix_Base\bin\win-x64\Release\Omnix_Base.dll" "..\Omnix.Base\Assemblies\Omnix_Base_x64.dll"

copy /Y /B "C++\Omnix_Correction\Omnix_Correction\bin\win-x86\Release\Omnix_Correction.dll" "..\Omnix.Correction\Assemblies\Omnix_Correction_x86.dll"
copy /Y /B "C++\Omnix_Correction\Omnix_Correction\bin\win-x64\Release\Omnix_Correction.dll" "..\Omnix.Correction\Assemblies\Omnix_Correction_x64.dll"

copy /Y /B "C++\Omnix_Cryptography\Omnix_Cryptography\bin\win-x86\Release\Omnix_Cryptography.dll" "..\Omnix.Cryptography\Assemblies\Omnix_Cryptography_x86.dll"
copy /Y /B "C++\Omnix_Cryptography\Omnix_Cryptography\bin\win-x64\Release\Omnix_Cryptography.dll" "..\Omnix.Cryptography\Assemblies\Omnix_Cryptography_x64.dll"
