#!/bin/bash
cwd=$(cd $(dirname $0); pwd)

cd ${cwd}/hashcash
cargo build --release
