#!/bin/bash

DOTNET_CLI_TELEMETRY_OPTOUT=1

BIN_DIR="$PWD/bin/tools/linux"
TOOL_PATH="$BIN_DIR/Omnius.Core.Serialization.RocketPack.DefinitionCompiler/Omnius.Core.Serialization.RocketPack.DefinitionCompiler"
INCLUDE="$PWD/fmt"

"$TOOL_PATH" compile -s "$PWD/fmt/FormatterBenchmarks/FormatterBenchmarks.Internal.rpd" -i "$INCLUDE" -o "$PWD/perf/FormatterBenchmarks/Internal/_RocketPack/Messages.generated.cs"

"$TOOL_PATH" compile -s "$PWD/fmt/Omnius.Core.Cryptography/Omnius.Core.Cryptography.rpd" -i "$INCLUDE" -o "$PWD/src/Omnius.Core.Cryptography/_RocketPack/Messages.generated.cs"

"$TOOL_PATH" compile -s "$PWD/fmt/Omnius.Core.Network/Omnius.Core.Network.rpd" -i "$INCLUDE" -o "$PWD/src/Omnius.Core.Network/_RocketPack/Messages.generated.cs"
"$TOOL_PATH" compile -s "$PWD/fmt/Omnius.Core.Network/Omnius.Core.Network.Connections.Secure.rpd" -i "$INCLUDE" -o "$PWD/src/Omnius.Core.Network/Connections/Secure/_RocketPack/Messages.generated.cs"
"$TOOL_PATH" compile -s "$PWD/fmt/Omnius.Core.Network/Omnius.Core.Network.Connections.Secure.Internal.rpd" -i "$INCLUDE" -o "$PWD/src/Omnius.Core.Network/Connections/Secure/Internal/_RocketPack/Messages.generated.cs"
"$TOOL_PATH" compile -s "$PWD/fmt/Omnius.Core.Network/Omnius.Core.Network.Connections.Secure.V1.Internal.rpd" -i "$INCLUDE" -o "$PWD/src/Omnius.Core.Network/Connections/Secure/V1/Internal/_RocketPack/Messages.generated.cs"

"$TOOL_PATH" compile -s "$PWD/fmt/Omnius.Core.Remoting/Omnius.Core.Remoting.rpd" -i "$INCLUDE" -o "$PWD/src/Omnius.Core.Remoting/_RocketPack/Messages.generated.cs"

"$TOOL_PATH" compile -s "$PWD/fmt/Omnius.Core.Serialization.RocketPack.DefinitionCompiler.Tests/Omnius.Core.Serialization.RocketPack.DefinitionCompiler.Tests.Internal.rpd" -i "$INCLUDE" -o "$PWD/test/Omnius.Core.Serialization.RocketPack.DefinitionCompiler.Tests/Internal/_RocketPack/Messages.generated.cs"
