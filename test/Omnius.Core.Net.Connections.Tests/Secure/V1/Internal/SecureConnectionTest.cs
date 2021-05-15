using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public class SecureConnectionTest
    {
        [Fact]
        public async Task IncrementTest()
        {
            var caseList = new List<(byte[] value, byte[] actual)>
            {
                (new byte[] { 0x00, 0x00 }, new byte[] { 0x01, 0x00 }),
                (new byte[] { 0xff, 0x00 }, new byte[] { 0x00, 0x01 }),
                (new byte[] { 0xff, 0xff }, new byte[] { 0x00, 0x00 })
            };

            foreach (var (value, actual) in caseList)
            {
                var result = value;
                SecureConnection.Increment(result);

                Assert.Equal(result, actual);
            }
        }
    }
}
