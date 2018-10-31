#!/bin/sh
cwd=`dirname "${0}"`
expr "${0}" : "/.*" > /dev/null || cwd=`(cd "${cwd}" && pwd)`

export CFLAGS="-shared -m32 -mfpmath=sse -march=pentium4 -fPIC -Wall -O3 -pipe -D UNIX"
export CXXFLAGS="-shared -m32 -mfpmath=sse -march=pentium4 -fPIC -Wall -O3 -pipe -D UNIX"
export BUILD="bin/linux-x86"

cd ${cwd}/Omnix_Base/Omnix_Base
make 

cd ${cwd}/Omnix_Correction/Omnix_Correction
make 

cd ${cwd}/Omnix_Cryptography/Omnix_Cryptography
make
