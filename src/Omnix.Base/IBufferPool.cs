using System.Buffers;

namespace Omnix.Base
{
    public interface IBufferPool<T>
    {
        int MaxBufferSize { get; }

        IMemoryOwner<T> RentMemory(int minimumLength);

        T[] RentArray(int minimumLength);
        void ReturnArray(T[] array);
    }
}
