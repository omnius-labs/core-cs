using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Omnius.Core.Network.Caps;
using Xunit;

namespace Omnius.Core.Network
{
    public class OmniAddressTest
    {
        [Fact]
        public async Task SimpleParseTest()
        {
            var sample = OmniAddress.CreateTcpEndpoint(IPAddress.Loopback, 32321);
            Assert.True(sample.TryGetTcpEndpoint(out var ipAddress, out var port));
            Assert.Equal(IPAddress.Loopback, ipAddress);
            Assert.Equal(32321, port);
        }
    }
}
