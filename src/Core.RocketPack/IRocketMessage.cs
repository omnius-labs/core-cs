using System.Buffers;
using Omnius.Core.Base;

namespace Omnius.Core.RocketPack;

public interface IRocketMessage<T> : IEquatable<T>
{
    private static IRocketMessageFormatter<T> _formatter = default!;

    private static T _empty = default!;

    public static IRocketMessageFormatter<T> Formatter
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

    public static T Import(ReadOnlySequence<byte> sequence, IBytesPool bytesPool)
    {
        var reader = new RocketMessageReader(sequence, bytesPool);
        return Formatter.Deserialize(ref reader, 0);
    }

    public virtual void Export(IBufferWriter<byte> bufferWriter, IBytesPool bytesPool)
    {
        var writer = new RocketMessageWriter(bufferWriter, bytesPool);
        Formatter.Serialize(ref writer, (T)this, 0);
    }
}
