using System.Buffers;

namespace Omnix.Base
{
    public interface IMemoryPool<T>
    {
        int MaxBufferSize { get; }
        IMemoryOwner<T> Rent(int length);
    }
}
