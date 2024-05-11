using System.Buffers;

namespace Core.Pipelines;

public interface IBytesReader
{
    long RemainBytes { get; }
    void Advance(int count);
    ReadOnlySequence<byte> GetSequence();
}
