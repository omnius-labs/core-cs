#!/usr/env bash
set -euo pipefail

DOTNET_CLI_TELEMETRY_OPTOUT=1

# *.rpf
RPFC_PATH="$PWD/src/Omnius.Core.RocketPack.DefinitionCompiler"
dotnet run -p $RPFC_PATH -- -c "$PWD/rpfs/config.yml"
