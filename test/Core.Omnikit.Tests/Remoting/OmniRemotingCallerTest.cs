using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Omnius.Core.Base;
using Omnius.Core.Omnikit.Converters;
using Omnius.Core.RocketPack;
using Omnius.Core.Testkit;
using Xunit;
using Xunit.Abstractions;

namespace Omnius.Core.Omnikit.Remoting;

public class OmniRemotingCallerTest : TestBase<OmniRemotingCallerTest>
{
    public OmniRemotingCallerTest(ITestOutputHelper output) : base(output) { }

    [Fact(Skip = "Echo server is required.")]
    public async Task CallTest()
    {
        var client = new TcpClient();

        async ValueTask<Stream> Connect()
        {
            await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 50000);
            return client.GetStream();
        }

        var factory = new OmniRemotingCallerFactory<OmniRemotingDefaultErrorMessage>(Connect, 1024 * 1024, BytesPool.Shared);
        var fn = await factory.CreateAsync(1);
        var result = await fn.CallAsync<TestMessage, TestMessage>(new TestMessage() { Value = 1 });
        this.Output.WriteLine($"Result: {result.Value}");
    }
}

public class TestMessage : RocketMessage<TestMessage>
{
    public required int Value { get; init; }

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(this.Value);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public override bool Equals(TestMessage? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Value == other.Value;
    }

    static TestMessage()
    {
        Formatter = new CustomSerializer();
        Empty = new TestMessage() { Value = 0 };
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<TestMessage>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in TestMessage value, scoped in int depth)
        {
            w.Put(value.Value);
        }
        public TestMessage Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var value = r.GetInt32();

            return new TestMessage()
            {
                Value = value
            };
        }
    }
}
