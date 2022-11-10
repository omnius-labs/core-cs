// <auto-generated/>
#nullable enable

namespace Omnius.Core.RocketPack.Remoting.Tests.Internal;

public sealed partial class TestParam : global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam>
{
    public static global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam> Formatter => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam>.Formatter;
    public static global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam Empty => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam>.Empty;

    static TestParam()
    {
        global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam>.Formatter = new ___CustomFormatter();
        global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam>.Empty = new global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam(0);
    }

    private readonly global::System.Lazy<int> ___hashCode;

    public TestParam(int value)
    {
        this.Value = value;

        ___hashCode = new global::System.Lazy<int>(() =>
        {
            var ___h = new global::System.HashCode();
            if (value != default) ___h.Add(value.GetHashCode());
            return ___h.ToHashCode();
        });
    }

    public int Value { get; }

    public static global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
    {
        var reader = new global::Omnius.Core.RocketPack.RocketMessageReader(sequence, bytesPool);
        return Formatter.Deserialize(ref reader, 0);
    }
    public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
    {
        var writer = new global::Omnius.Core.RocketPack.RocketMessageWriter(bufferWriter, bytesPool);
        Formatter.Serialize(ref writer, this, 0);
    }

    public static bool operator ==(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam? left, global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam? right)
    {
        return (right is null) ? (left is null) : right.Equals(left);
    }
    public static bool operator !=(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam? left, global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam? right)
    {
        return !(left == right);
    }
    public override bool Equals(object? other)
    {
        if (other is not global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam) return false;
        return this.Equals((global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam)other);
    }
    public bool Equals(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam? target)
    {
        if (target is null) return false;
        if (object.ReferenceEquals(this, target)) return true;
        if (this.Value != target.Value) return false;

        return true;
    }
    public override int GetHashCode() => ___hashCode.Value;

    private sealed class ___CustomFormatter : global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam>
    {
        public void Serialize(ref global::Omnius.Core.RocketPack.RocketMessageWriter w, scoped in global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam value, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            if (value.Value != 0)
            {
                w.Write((uint)1);
                w.Write(value.Value);
            }
            w.Write((uint)0);
        }
        public global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam Deserialize(ref global::Omnius.Core.RocketPack.RocketMessageReader r, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            int p_value = 0;

            for (; ; )
            {
                uint id = r.GetUInt32();
                if (id == 0) break;
                switch (id)
                {
                    case 1:
                        {
                            p_value = r.GetInt32();
                            break;
                        }
                }
            }

            return new global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam(p_value);
        }
    }
}
public sealed partial class TestResult : global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>
{
    public static global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult> Formatter => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>.Formatter;
    public static global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult Empty => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>.Empty;

    static TestResult()
    {
        global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>.Formatter = new ___CustomFormatter();
        global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>.Empty = new global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult(0);
    }

    private readonly global::System.Lazy<int> ___hashCode;

    public TestResult(int value)
    {
        this.Value = value;

        ___hashCode = new global::System.Lazy<int>(() =>
        {
            var ___h = new global::System.HashCode();
            if (value != default) ___h.Add(value.GetHashCode());
            return ___h.ToHashCode();
        });
    }

    public int Value { get; }

    public static global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
    {
        var reader = new global::Omnius.Core.RocketPack.RocketMessageReader(sequence, bytesPool);
        return Formatter.Deserialize(ref reader, 0);
    }
    public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
    {
        var writer = new global::Omnius.Core.RocketPack.RocketMessageWriter(bufferWriter, bytesPool);
        Formatter.Serialize(ref writer, this, 0);
    }

    public static bool operator ==(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult? left, global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult? right)
    {
        return (right is null) ? (left is null) : right.Equals(left);
    }
    public static bool operator !=(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult? left, global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult? right)
    {
        return !(left == right);
    }
    public override bool Equals(object? other)
    {
        if (other is not global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult) return false;
        return this.Equals((global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult)other);
    }
    public bool Equals(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult? target)
    {
        if (target is null) return false;
        if (object.ReferenceEquals(this, target)) return true;
        if (this.Value != target.Value) return false;

        return true;
    }
    public override int GetHashCode() => ___hashCode.Value;

    private sealed class ___CustomFormatter : global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>
    {
        public void Serialize(ref global::Omnius.Core.RocketPack.RocketMessageWriter w, scoped in global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult value, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            if (value.Value != 0)
            {
                w.Write((uint)1);
                w.Write(value.Value);
            }
            w.Write((uint)0);
        }
        public global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult Deserialize(ref global::Omnius.Core.RocketPack.RocketMessageReader r, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            int p_value = 0;

            for (; ; )
            {
                uint id = r.GetUInt32();
                if (id == 0) break;
                switch (id)
                {
                    case 1:
                        {
                            p_value = r.GetInt32();
                            break;
                        }
                }
            }

            return new global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult(p_value);
        }
    }
}
public interface ITestService
{
    global::System.Threading.Tasks.ValueTask<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult> Unary1Async(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam param, global::System.Threading.CancellationToken cancellationToken = default);
    global::System.Threading.Tasks.ValueTask Unary2Async(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam param, global::System.Threading.CancellationToken cancellationToken = default);
    global::System.Threading.Tasks.ValueTask<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult> Unary3Async(global::System.Threading.CancellationToken cancellationToken = default);
    global::System.Threading.Tasks.ValueTask Unary4Async(global::System.Threading.CancellationToken cancellationToken = default);
}
public class TestServiceRemoting
{
    public class Client<TError> : global::Omnius.Core.RocketPack.Remoting.Tests.Internal.ITestService
        where TError : global::Omnius.Core.RocketPack.IRocketMessage<TError>
    {
        private readonly global::Omnius.Core.RocketPack.Remoting.IRocketRemotingCallerFactory<TError> _callerFactory;
        private readonly global::Omnius.Core.IBytesPool _bytesPool;
        public Client(global::Omnius.Core.RocketPack.Remoting.IRocketRemotingCallerFactory<TError> callerFactory, global::Omnius.Core.IBytesPool bytesPool)
        {
            _callerFactory = callerFactory;
            _bytesPool = bytesPool;
        }
        public async global::System.Threading.Tasks.ValueTask<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult> Unary1Async(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam param, global::System.Threading.CancellationToken cancellationToken = default)
        {
            await using var caller = await _callerFactory.CreateAsync(1, cancellationToken);
            return await caller.CallFunctionAsync<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam, global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>(param, cancellationToken);
        }
        public async global::System.Threading.Tasks.ValueTask Unary2Async(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam param, global::System.Threading.CancellationToken cancellationToken = default)
        {
            await using var caller = await _callerFactory.CreateAsync(2, cancellationToken);
            await caller.CallActionAsync<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam>(param, cancellationToken);
        }
        public async global::System.Threading.Tasks.ValueTask<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult> Unary3Async(global::System.Threading.CancellationToken cancellationToken = default)
        {
            await using var caller = await _callerFactory.CreateAsync(3, cancellationToken);
            return await caller.CallFunctionAsync<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>(cancellationToken);
        }
        public async global::System.Threading.Tasks.ValueTask Unary4Async(global::System.Threading.CancellationToken cancellationToken = default)
        {
            await using var caller = await _callerFactory.CreateAsync(4, cancellationToken);
            await caller.CallActionAsync(cancellationToken);
        }
    }
    public class Server<TError>
        where TError : global::Omnius.Core.RocketPack.IRocketMessage<TError>
    {
        private readonly global::Omnius.Core.RocketPack.Remoting.Tests.Internal.ITestService _service;
        private readonly global::Omnius.Core.RocketPack.Remoting.IRocketRemotingListenerFactory<TError> _listenerFactory;
        private readonly global::Omnius.Core.IBytesPool _bytesPool;
        public Server(global::Omnius.Core.RocketPack.Remoting.Tests.Internal.ITestService service, global::Omnius.Core.RocketPack.Remoting.IRocketRemotingListenerFactory<TError> listenerFactory, global::Omnius.Core.IBytesPool bytesPool)
        {
            _service = service;
            _listenerFactory = listenerFactory;
            _bytesPool = bytesPool;
        }
        public async global::System.Threading.Tasks.Task EventLoopAsync(global::System.Threading.CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await using var listener = await _listenerFactory.CreateAsync(cancellationToken);
                switch (listener.FunctionId)
                {
                    case 1:
                        {
                            await listener.ListenFunctionAsync<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam, global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>(_service.Unary1Async, cancellationToken);
                        }
                        break;
                    case 2:
                        {
                            await listener.ListenActionAsync<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestParam>(_service.Unary2Async, cancellationToken);
                        }
                        break;
                    case 3:
                        {
                            await listener.ListenFunctionAsync<global::Omnius.Core.RocketPack.Remoting.Tests.Internal.TestResult>(_service.Unary3Async, cancellationToken);
                        }
                        break;
                    case 4:
                        {
                            await listener.ListenActionAsync(_service.Unary4Async, cancellationToken);
                        }
                        break;
                }
            }
        }
    }
}
