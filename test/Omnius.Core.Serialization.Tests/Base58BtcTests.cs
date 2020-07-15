using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Serialization.Extensions;
using Xunit;

namespace Omnius.Core.Serialization
{
    public class Base58BtcTests
    {
        [Fact]
        public async Task EncodeAndDecodeTest()
        {
            var base58Btc = new Base58Btc();
            var pattern = new Dictionary<string, string>();

            // Load
            {
                string basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly()!.Location)!;

                //https://github.com/bitcoin/bitcoin/blob/master/src/test/data/base58_encode_decode.json
                using (var fs = File.OpenRead(Path.Combine(basePath, "Data/base58_encode_decode.json")))
                {
                    var array = await JsonSerializer.DeserializeAsync<string[][]>(fs);

                    foreach (var pair in array)
                    {
                        pattern.Add(pair[0], pair[1]);
                    }
                }
            }

            {
                var base16 = new Base16(ConvertStringCase.Lower);

                foreach (var (input, output) in pattern)
                {
                    using var hub_base16 = new BytesHub();

                    // base16をデコードし、pipeに書き込む。
                    base16.TryDecode(input, hub_base16.Writer);

                    // base58Btcのテキストを取得する。
                    base58Btc.TryEncode(hub_base16.Reader.GetSequence(), out var text_base58);

                    // エンコード結果の検証
                    Assert.Equal(UTF8Encoding.Default.GetBytes(output), text_base58);

                    using var hub_base58Btc = new BytesHub();

                    // base58Btcをデコードし、pipeに書き込む。
                    base58Btc.TryDecode(output, hub_base58Btc.Writer);

                    // デコード結果の検証
                    Assert.Equal(hub_base16.Reader.GetSequence().ToArray(), hub_base58Btc.Reader.GetSequence().ToArray());
                }
            }
        }
    }
}
