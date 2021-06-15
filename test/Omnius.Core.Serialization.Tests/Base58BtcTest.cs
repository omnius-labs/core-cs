using System;
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
            public string? Base16 { get; init; }
            public string? Base58 { get; init; }
        }

        [Fact]
        public async Task EncodeAndDecodeTest()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var caseList = JsonSerializer.Deserialize<TestCase[]>(File.ReadAllText("./Data/base58_encode_decode.json"), serializeOptions) ?? throw new NullReferenceException();

            foreach (var c in caseList)
            {
                var b16String = c.Base16 ?? throw new NullReferenceException();
                var b58String = c.Base58 ?? throw new NullReferenceException();

                Assert.True(TryBase16StringToBytes(b16String, out var b16Bytes));

                Assert.True(TryBase58StringToBytes(b58String, out var decodedResult));
                Assert.Equal(b16Bytes, decodedResult);

                Assert.True(TryBytesToBase58String(b16Bytes!, out var encodedResult));
                Assert.Equal(b58String, encodedResult);
            }
        }

        private static bool TryBase16StringToBytes(string text, out byte[]? result)
        {
            result = null;

            var base16 = new Base16(ConvertStringCase.Lower);
            var hub = new BytesHub();
            if (!base16.TryDecode(text, hub.Writer)) return false;

            result = hub.Reader.GetSequence().ToArray();
            return true;
        }

        private static bool TryBase58StringToBytes(string text, out byte[]? result)
        {
            result = null;

            var base58Btc = new Base58Btc();
            var hub = new BytesHub();
            if (!base58Btc.TryDecode(text, hub.Writer)) return false;

            result = hub.Reader.GetSequence().ToArray();
            return true;
        }

        private static bool TryBytesToBase58String(byte[] value, out string? result)
        {
            result = null;

            var base58Btc = new Base58Btc();
            if (!base58Btc.TryEncode(new ReadOnlySequence<byte>(value), out var utf8bytes)) return false;

            result = Encoding.UTF8.GetString(utf8bytes);
            return true;
        }
    }
}
