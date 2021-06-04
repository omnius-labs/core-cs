using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Serialization;
using Omnius.Core.Serialization.Extensions;
using Xunit;

namespace Omnius.Core.Cryptography
{
    public class Hmac_Sha2_256_Test
    {
        public class TestCase
        {
            public string? Key { get; init; }
            public string? Value { get; init; }
            public string? Result { get; init; }
        }

        [Fact]
        public void ComputeHashTest()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var caseList = JsonSerializer.Deserialize<TestCase[]>(File.ReadAllText("./Data/Hmac_Sha2_256.json"), serializeOptions);

            foreach (var c in caseList!)
            {
                var base16 = new Base16(ConvertStringCase.Lower);

                var key = base16.StringToBytes(c.Key!);
                var value = base16.StringToBytes(c.Value!);
                var result = base16.BytesToString(new ReadOnlySequence<byte>(Hmac_Sha2_256.ComputeHash(value, key)));

                Assert.Equal(c.Result, result);
            }
        }

        [Fact]
        public void RandomComputeHashTest()
        {
            var random = new Random();

            var caseList = new List<int>();
            caseList.AddRange(Enumerable.Range(1, 64));
            caseList.AddRange(new int[] { 100, 1000, 10000, 1024 * 1024, 1024 * 1024 * 32 });

            var buffer = new byte[1024 * 32];
            random.NextBytes(buffer);

            for (int i = 0; i < caseList.Count; i++)
            {
                var key = new byte[caseList[i]];
                random.NextBytes(key);

                using (var stream = new MemoryStream(buffer))
                using (var hmacSha256 = new HMACSHA256(key))
                {
                    // .Net標準のHMACSHA256と同一の結果になるはず。
                    Assert.Equal(hmacSha256.ComputeHash(stream), Hmac_Sha2_256.ComputeHash(buffer, key));
                }
            }
        }
    }
}
