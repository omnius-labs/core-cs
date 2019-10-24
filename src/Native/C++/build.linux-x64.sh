#!/bin/sh
cwd=`dirname "${0}"`
expr "${0}" : "/.*" > /dev/null || cwd=`(cd "${cwd}" && pwd)`

export CFLAGS="-shared -m64 -fPIC -Wall -O3 -pipe -D UNIX"
export CXXFLAGS="-shared -m64 -fPIC -Wall -O3 -pipe -D UNIX" 
export BUILD="bin/linux-x64"

cd ${cwd}/Omnix_Base/Omnix_Base
make
