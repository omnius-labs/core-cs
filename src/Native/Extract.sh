#!/bin/sh
cwd=`dirname "${0}"`
expr "${0}" : "/.*" > /dev/null || cwd=`(cd "${cwd}" && pwd)`
cd "${cwd}"

cp "./C++/Omnix_Base/Omnix_Base/bin/linux-x86/Omnix_Base.so" "../Omnix.Core/Assemblies/Omnix_Base_x86.so"
cp "./C++/Omnix_Base/Omnix_Base/bin/linux-x64/Omnix_Base.so" "../Omnix.Core/Assemblies/Omnix_Base_x64.so"

cp "./C++/Omnix_Cryptography/Omnix_Cryptography/bin/linux-x86/Omnix_Cryptography.so" "../Omnix.Cryptography/Assemblies/Omnix_Cryptography_x86.so"
cp "./C++/Omnix_Cryptography/Omnix_Cryptography/bin/linux-x64/Omnix_Cryptography.so" "../Omnix.Cryptography/Assemblies/Omnix_Cryptography_x64.so"

