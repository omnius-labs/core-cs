using System.Buffers;

namespace Core.Base;

public static class ReadOnlySequenceExtensions
{
    public static IMemoryOwner<byte> ToMemoryOwner(this ReadOnlySequence<byte> sequence, IBytesPool bytesPool)
    {
        var memoryOwner = bytesPool.Memory.Rent((int)sequence.Length);
        sequence.CopyTo(memoryOwner.Memory.Span);
        return memoryOwner;
    }
}
