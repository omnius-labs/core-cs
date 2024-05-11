using System.Buffers;

namespace Core.Base;

public static class IMemoryOwnerExtensions
{
    public static IMemoryOwner<T> Shrink<T>(this IMemoryOwner<T> memoryOwner, int length)
    {
        if (memoryOwner.Memory.Length < length) throw new ArgumentOutOfRangeException(nameof(length));

        if (memoryOwner.Memory.Length == length) return memoryOwner;

        return new CustomMemoryOwner<T>(memoryOwner, length);
    }

    public sealed class CustomMemoryOwner<T> : IMemoryOwner<T>
    {
        private readonly IMemoryOwner<T> _memoryOwner;

        public CustomMemoryOwner(IMemoryOwner<T> memoryOwner, int length)
        {
            _memoryOwner = memoryOwner;
            this.Memory = _memoryOwner.Memory[..length];
        }

        public Memory<T> Memory { get; }

        public void Dispose()
        {
            _memoryOwner.Dispose();
        }
    }
}
