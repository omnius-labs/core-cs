using System.Buffers;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;

namespace Omnius.Core.RocketPack;

public abstract class RocketMessage<T> : IEquatable<T>
    where T : RocketMessage<T>
{
    private static IRocketMessageSerializer<T> _formatter = default!;
    private static T _empty = default!;

    public static IRocketMessageSerializer<T> Formatter
    {
        get
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
            return _formatter;
        }
        protected set
        {
            _formatter = value;
        }
    }

    public static T Empty
    {
        get
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
            return _empty;
        }
        protected set
        {
            _empty = value;
        }
    }

    public abstract bool Equals(T? other);

    public static T Import(ReadOnlyMemory<byte> memory, IBytesPool bytesPool)
    {
        return Import(new ReadOnlySequence<byte>(memory), bytesPool);
    }

    public static T Import(ReadOnlySequence<byte> sequence, IBytesPool bytesPool)
    {
        var reader = new RocketMessageReader(sequence, bytesPool);
        return Formatter.Deserialize(ref reader, 0);
    }

    public IMemoryOwner<byte> Export(IBytesPool bytesPool)
    {
        var pipe = new BytesPipe(bytesPool);
        this.Export(pipe.Writer, bytesPool);
        return pipe.Reader.GetSequence().ToMemoryOwner(bytesPool);
    }

    public void Export(IBufferWriter<byte> bufferWriter, IBytesPool bytesPool)
    {
        var writer = new RocketMessageWriter(bufferWriter, bytesPool);
        Formatter.Serialize(ref writer, (T)this, 0);
    }
}
