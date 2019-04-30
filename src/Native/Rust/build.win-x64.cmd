setlocal
set BAT_DIR=%~dp0

cd %BAT_DIR%hashcash
cargo build --release
