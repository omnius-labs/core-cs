using System.Buffers;

namespace Omnius.Core.Base.Pipelines;

public interface IBytesWriter : IBufferWriter<byte>
{
    long WrittenBytes { get; }
}
