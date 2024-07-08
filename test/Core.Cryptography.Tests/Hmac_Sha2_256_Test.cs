using System.Buffers;
using System.Text.Json;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Serialization;
using Xunit;

namespace Omnius.Core.Cryptography;

public class HmacSha2256Test
{
    public class ComputeHashTestCase
    {
        public string? Key { get; init; }
        public string? Value { get; init; }
        public string? Expected { get; init; }
    }

    [Fact]
    public void ComputeHashTest()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var caseList = JsonSerializer.Deserialize<ComputeHashTestCase[]>(File.ReadAllText("./Data/Hmac_Sha2_256.json"), serializeOptions) ?? throw new NullReferenceException();

        foreach (var c in caseList)
        {
            var base16 = new Base16(Base16Case.Lower);

            var key = base16.StringToBytes(c.Key ?? throw new NullReferenceException());
            var value = base16.StringToBytes(c.Value ?? throw new NullReferenceException());
            var actual = base16.BytesToString(new ReadOnlySequence<byte>(Hmac_Sha2_256.ComputeHash(value, key)));

            Assert.Equal(c.Expected, actual);
        }
    }
}
