using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Omnius.Core.Base;
using Omnius.Core.Streams;
using Omnius.Core.Omnikit.Converters;
using Omnius.Core.RocketPack;
using Omnius.Core.Testkit;
using Xunit;
using Xunit.Abstractions;
using System.Diagnostics;

namespace Omnius.Core.Omnikit.Remoting;

public class CommunicationTest : TestBase<CommunicationTest>
{
    public CommunicationTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task CommunicationTestAsync()
    {
        const int MaxFrameLength = 1024 * 1024;
        const uint FunctionId = 1;
        var bytesPool = BytesPool.Shared;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var cancellationToken = cts.Token;

        var (serverStream, clientStream) = DuplexStream.CreatePair();

        var listenerTask = Task.Run(async () =>
        {
            await using var listener = await OmniRemotingListener.Create(serverStream, MaxFrameLength, bytesPool, cancellationToken);

            await listener.ListenStreamAsync(async (stream, ct) =>
            {
                var received = await stream.ReceiveAsync<TestMessage>(ct);
                this.Output.WriteLine($"listener receive: {received.Value}");

                await stream.SendAsync(new TestMessage { Value = received.Value + 1 });
                this.Output.WriteLine("listener send");
            }, cancellationToken);

            return listener.FunctionId;
        });

        await using var caller = await OmniRemotingCaller.Create(clientStream, FunctionId, MaxFrameLength, bytesPool, cancellationToken);

        var stream = caller.CallStream();

        await stream.SendAsync(new TestMessage { Value = 1 }, cancellationToken);
        this.Output.WriteLine("caller send");

        var received = await stream.ReceiveAsync<TestMessage>(cancellationToken);
        this.Output.WriteLine($"caller receive: {received.Value}");

        Assert.Equal(2, received.Value);
        Assert.Equal(FunctionId, await listenerTask.WaitAsync(cancellationToken));

        await clientStream.DisposeAsync();
        await serverStream.DisposeAsync();
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
