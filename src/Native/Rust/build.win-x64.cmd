setlocal

set BAT_DIR=%~dp0
cd %BAT_DIR%

cd hashcash
cargo build --release
