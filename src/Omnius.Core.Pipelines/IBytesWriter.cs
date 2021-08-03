using System.Buffers;

namespace Omnius.Core.Pipelines
{
    public interface IBytesWriter : IBufferWriter<byte>
    {
        long WrittenBytes { get; }
    }
}
