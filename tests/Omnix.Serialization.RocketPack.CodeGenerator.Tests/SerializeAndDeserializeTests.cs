using System;
using Xunit;
using Omnix.Serialization.RocketPack.CodeGenerator;
using Omnix.Base;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Omnix.Serialization.Extensions;
using System.Security.Cryptography;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public class LoaderTests
    {
        [Fact]
        public void HelloMessageTest()
        {
            var text = @"
syntax = v1.0;

message NullableHelloMessage {
    X0: bool? = 0,
    X1: int8? = 1,
    X2: int16? = 2,
    X3: int32? = 3,
    X4: int64? = 4,
    X5: uint8? = 5,
    X6: uint16? = 6,
    X7: uint32? = 7,
    X8: uint64? = 8,
    X9: Enum1? = 9,
    X10: Enum2? = 10,
    X11: Enum3? = 11,
    X12: Enum4? = 12,
    X13: Enum5? = 13,
    X14: Enum6? = 14,
    X15: Enum7? = 15,
    X16: Enum8? = 16,
    X17: float32? = 17,
    X18: float64? = 18,
    X19: string?(128) = 19, // maximum bytes size is 128
    X20: timestamp? = 20,
    X21: memory?(256) = 21, // maximum bytes size is 256
    [Recyclable]
    X22: memory?(256) = 22, // use recyclable memory
    X23: vector<string(128)>?(16) = 23,
    X24: map<uint8, string(128)>?(32) = 24,
    X25: Message1 = 25,
    X26: SmallMessage1 = 26,
}";
            RocketFormatParser.ParseV1_0(text);
        }
    }
}
