using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Omnix.Base;
using Omnix.Serialization;
using Omnix.Serialization.Extensions;
using Xunit;

namespace Omnix.Cryptography.Tests
{
    public class Hmac_Sha2_256_Tests
    {
        [Fact]
        public void ComputeHashTest()
        {
            // http://tools.ietf.org/html/rfc4868#section-2.7.1
            {
                var base16 = new Base16(ConvertStringCase.Lower);

                var key = base16.StringToBytes("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b");
                var value = base16.StringToBytes("4869205468657265");
                var result = base16.BytesToString(Hmac_Sha2_256.ComputeHash(value, key));

                Assert.True(result == "b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7");
            }

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
                        Assert.True(BytesOperations.SequenceEqual(hmacSha256.ComputeHash(stream), Hmac_Sha2_256.ComputeHash(buffer, key)));
                    }
                }
            }
        }
    }
}
