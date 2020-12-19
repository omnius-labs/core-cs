using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Omnius.Core.Serialization.Extensions;
using Xunit;

namespace Omnius.Core.Serialization
{
    public class Base58BtcTest
    {
        public class TestCase
        {
            public string? Input { get; init; }
            public string? Output { get; init; }
        }

        [Fact]
        public async Task EncodeAndDecodeTest()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var caseList = JsonSerializer.Deserialize<TestCase[]>(File.ReadAllText("./Data/base58_encode_decode.json"), serializeOptions);

            var base58Btc = new Base58Btc();
            var base16 = new Base16(ConvertStringCase.Lower);

            foreach (var c in caseList!)
            {
                using var hub_base16 = new BytesHub();

                // base16をデコードし、pipeに書き込む。
                base16.TryDecode(c.Input!, hub_base16.Writer);

                // base58Btcのテキストを取得する。
                base58Btc.TryEncode(hub_base16.Reader.GetSequence(), out var text_base58);

                // エンコード結果の検証
                Assert.Equal(UTF8Encoding.Default.GetBytes(c.Output!), text_base58);

                using var hub_base58Btc = new BytesHub();

                // base58Btcをデコードし、pipeに書き込む。
                base58Btc.TryDecode(c.Output!, hub_base58Btc.Writer);

                // デコード結果の検証
                Assert.Equal(hub_base16.Reader.GetSequence().ToArray(), hub_base58Btc.Reader.GetSequence().ToArray());
            }
        }
    }
}
