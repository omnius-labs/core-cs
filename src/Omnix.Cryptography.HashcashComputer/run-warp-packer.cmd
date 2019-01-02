rem Windows
warp-packer --arch windows-x64 --input_dir dist\win-x64 --exec Omnix.Cryptography.HashcashComputer.exe --output dist\Omnix.Cryptography.HashcashComputer.win-x64.exe

rem Linux
warp-packer --arch linux-x64 --input_dir dist\linux-x64 --exec Omnix.Cryptography.HashcashComputer --output dist\Omnix.Cryptography.HashcashComputer.linux-x64
