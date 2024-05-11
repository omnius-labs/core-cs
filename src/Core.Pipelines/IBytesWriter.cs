using System.Buffers;

namespace Core.Pipelines;

public interface IBytesWriter : IBufferWriter<byte>
{
    long WrittenBytes { get; }
}
