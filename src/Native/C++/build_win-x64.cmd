setlocal

set BAT_DIR=%~dp0
cd %BAT_DIR%

set MSBUILD_PATH="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"

%MSBUILD_PATH% Omnix_Base\Omnix_Base.sln /p:Configuration=Release;Platform="x64" /t:rebuild /m
%MSBUILD_PATH% Omnix_Correction\Omnix_Correction.sln /p:Configuration=Release;Platform="x64" /t:rebuild /m
%MSBUILD_PATH% Omnix_Cryptography\Omnix_Cryptography.sln /p:Configuration=Release;Platform="x64" /t:rebuild /m