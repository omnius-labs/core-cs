setlocal

set BAT_DIR=%~dp0
cd %BAT_DIR%

set MSBUILD_PATH="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"

%MSBUILD_PATH% Core\sse2\Core.sln /p:Configuration=Release;Platform="x64" /t:rebuild /m
