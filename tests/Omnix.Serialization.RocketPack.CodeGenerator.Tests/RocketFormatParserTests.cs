using System;
using Xunit;
using Omnix.Serialization.RocketPack.CodeGenerator;

namespace Omnix.Serialization.RocketPack.CodeGenerator.Tests
{
    public class RocketFormatParserTests
    {
        [Fact]
        public void ParseV1Test()
        {
            var text =
@"  
syntax = v1;

option csharp_namespace = ""RocketPack.Messages"";

enum AgreementAlgorithm : uint8 {
    EcDhP521_Sha256 = 0,
}

message Agreement {
    CreationTime: timestamp = 0,
    AgreementAlgorithm: AgreementAlgorithm = 1,
    PublicKey: memory(8192, true) = 2,
    PrivateKey: memory(8192) = 3,
}

small message Agreement2 {
    CreationTime: timestamp,
    AgreementAlgorithm: AgreementAlgorithm,
    PublicKey: memory(8192, true),
    PrivateKey: memory(8192),
}
";

            var s = RocketFormatParser.ParseV1(text);
            var ss = RocketCodeGenerator.Generate(s, new RocketFormatInfo[] { });
        }
    }
}
