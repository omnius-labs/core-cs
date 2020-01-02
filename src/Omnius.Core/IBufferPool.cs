using System.Buffers;

namespace Omnius.Core
{
    public interface IBufferPool<T>
    {
        int MaxBufferSize { get; }

        IMemoryOwner<T> RentMemory(int size);

        T[] RentArray(int minimumLength);
        void ReturnArray(T[] array);
    }
}
