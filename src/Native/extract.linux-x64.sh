#!/bin/sh
cwd=`dirname "${0}"`
expr "${0}" : "/.*" > /dev/null || cwd=`(cd "${cwd}" && pwd)`
cd "${cwd}"

cp "./C++/Omnix_Base/Omnix_Base/bin/linux-x64/Omnix_Base.so" "../Omnix.Base/Assemblies/Omnix.Base.linux-x64.so"
cp "./C++/Omnix_Correction/Omnix_Correction/bin/linux-x64/Omnix_Correction.so" "../Omnix.Correction/Assemblies/Omnix.Correction.linux-x64.so"
cp "./C++/Omnix_Cryptography/Omnix_Cryptography/bin/linux-x64/Omnix_Cryptography.so" "../Omnix.Cryptography/Assemblies/Omnix.Cryptography.linux-x64.so"

cp "./Rust/hashcash/target/release/hashcash" "../Omnix.Cryptography/Assemblies/hashcash.linux-x64"