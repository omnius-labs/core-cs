using System.Buffers;
using Omnius.Core.Base;
using Omnius.Core.Base.Pipelines;

namespace Omnius.Core.RocketPack;

public interface IRocketPackStruct<T> where T : IRocketPackStruct<T>
{
    static abstract void Pack(ref RocketPackBytesEncoder encoder, in T value);
    static abstract T Unpack(ref RocketPackBytesDecoder decoder);
}

public sealed class RocketPackStruct
{
    public static T Import<T>(ReadOnlyMemory<byte> memory, IBytesPool pool)
        where T : IRocketPackStruct<T>
    {
        var seq = new ReadOnlySequence<byte>(memory);
        var decoder = new RocketPackBytesDecoder(seq, pool);
        return T.Unpack(ref decoder);
    }

    public static T Import<T>(ReadOnlySequence<byte> seq, IBytesPool pool)
        where T : IRocketPackStruct<T>
    {
        var decoder = new RocketPackBytesDecoder(seq, pool);
        return T.Unpack(ref decoder);
    }

    public static IMemoryOwner<byte> Export<T>(T value, IBytesPool pool)
        where T : IRocketPackStruct<T>
    {
        var pipe = new BytesPipe(pool);
        var encoder = new RocketPackBytesEncoder(pipe.Writer, pool);
        T.Pack(ref encoder, value);
        return pipe.Reader.GetSequence().ToMemoryOwner(pool);
    }
}
