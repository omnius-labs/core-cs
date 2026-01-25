using System.Buffers;

namespace Omnius.Core.Base.Pipelines;

public interface IBytesReader
{
    long RemainBytes { get; }
    void Advance(int count);
    ReadOnlySequence<byte> GetSequence();
}
