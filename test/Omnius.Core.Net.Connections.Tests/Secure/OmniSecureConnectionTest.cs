using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Omnius.Core.Cryptography;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Bridge;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Net.Connections.Secure.V1;
using Omnius.Core.Tasks;
using Xunit;

namespace Omnius.Core.Net.Connections.Secure
{
    public class OmniSecureConnectionTest
    {
        public static IEnumerable<object?[]> GetRandomSendAndReceiveTestCases()
        {
            var clientDigitalSignature = OmniDigitalSignature.Create("client", OmniDigitalSignatureAlgorithmType.EcDsa_P521_Sha2_256);
            var serverDigitalSignature = OmniDigitalSignature.Create("server", OmniDigitalSignatureAlgorithmType.EcDsa_P521_Sha2_256);

            var results = new List<(OmniDigitalSignature?, OmniDigitalSignature?)>{
                (null, null),
                (clientDigitalSignature, serverDigitalSignature),
            };
            return results.Select(n => new object?[] { n.Item1, n.Item2 });
        }

        [Theory]
        [MemberData(nameof(GetRandomSendAndReceiveTestCases))]
        public async Task RandomSendAndReceiveTest(OmniDigitalSignature? clientDigitalSignature, OmniDigitalSignature? serverDigitalSignature)
        {
            var random = new Random();

            var (clientSocket, serverSocket) = SocketHelper.GetSocketPair();

            await using var batchActionDispatcher = new BatchActionDispatcher(TimeSpan.FromMilliseconds(10));

            var bridgeConnectionOptions = new BridgeConnectionOptions
            {
                MaxReceiveByteCount = 1024 * 1024 * 256,
                BatchActionDispatcher = batchActionDispatcher,
                BytesPool = BytesPool.Shared,
            };
            var clientBridgeConnection = new BridgeConnection(new SocketCap(clientSocket), bridgeConnectionOptions);
            var serverBridgeConnection = new BridgeConnection(new SocketCap(serverSocket), bridgeConnectionOptions);

            var clientSecureConnectionOptions = new OmniSecureConnectionOptions()
            {
                Type = OmniSecureConnectionType.Connected,
                DigitalSignature = clientDigitalSignature,
                MaxReceiveByteCount = 1024 * 1024 * 256,
                BatchActionDispatcher = batchActionDispatcher,
                BytesPool = BytesPool.Shared,
            };
            await using var clientSecureConnection = new OmniSecureConnection(clientBridgeConnection, clientSecureConnectionOptions);

            var serverSecureConnectionOptions = new OmniSecureConnectionOptions()
            {
                Type = OmniSecureConnectionType.Accepted,
                DigitalSignature = serverDigitalSignature,
                MaxReceiveByteCount = 1024 * 1024 * 256,
                BatchActionDispatcher = batchActionDispatcher,
                BytesPool = BytesPool.Shared,
            };
            await using var serverSecureConnection = new OmniSecureConnection(serverBridgeConnection, serverSecureConnectionOptions);

            // ハンドシェイクを行う
            var valueTask1 = clientSecureConnection.HandshakeAsync();
            var valueTask2 = serverSecureConnection.HandshakeAsync();
            await Task.WhenAll(valueTask1.AsTask(), valueTask2.AsTask());

            if (clientDigitalSignature != null)
            {
                Assert.Equal(serverSecureConnection.Signature, clientDigitalSignature.GetOmniSignature());
            }
            if (serverDigitalSignature != null)
            {
                Assert.Equal(clientSecureConnection.Signature, serverDigitalSignature.GetOmniSignature());
            }

            await TestHelper.RandomSendAndReceive(random, clientSecureConnection, serverSecureConnection);
            await TestHelper.RandomSendAndReceive(random, serverSecureConnection, clientSecureConnection);
        }
    }
}
