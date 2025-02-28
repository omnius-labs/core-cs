using System.Buffers;
using Omnius.Core.Base;
using Omnius.Core.Base.Helpers;

namespace Omnius.Core.Storages;

public sealed class SingleValueFileStorage : AsyncDisposableBase, ISingleValueStorage
{
    private readonly string _filePath;
    private readonly IBytesPool _bytesPool;

    private readonly AsyncLock _asyncLock = new();

    internal sealed class SingleValueStorageFactory : ISingleValueStorageFactory
    {
        public ISingleValueStorage Create(string path, IBytesPool bytesPool)
        {
            var result = new SingleValueFileStorage(path, bytesPool);
            return result;
        }
    }

    public static ISingleValueStorageFactory Factory { get; } = new SingleValueStorageFactory();

    internal SingleValueFileStorage(string filePath, IBytesPool bytesPool)
    {
        _filePath = filePath;
        _bytesPool = bytesPool;

        DirectoryHelper.CreateDirectory(Path.GetDirectoryName(filePath)!);
    }

    protected override async ValueTask OnDisposeAsync()
    {
    }

    public async ValueTask<IMemoryOwner<byte>?> TryReadAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            if (!File.Exists(_filePath)) return null;

            await using var fileStream = new FileStream(_filePath, FileMode.Open);

            var memoryOwner = _bytesPool.Memory.Rent((int)fileStream.Length).Shrink((int)fileStream.Length);

            while (fileStream.Position < fileStream.Length)
            {
                await fileStream.ReadAsync(memoryOwner.Memory[(int)fileStream.Position..], cancellationToken);
            }

            return memoryOwner;
        }
    }

    public async ValueTask<bool> TryReadAsync(IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            if (!File.Exists(_filePath)) return false;

            await using var fileStream = new FileStream(_filePath, FileMode.Open);

            while (fileStream.Position < fileStream.Length)
            {
                int readLength = await fileStream.ReadAsync(bufferWriter.GetMemory(), cancellationToken);
                bufferWriter.Advance(readLength);
            }

            return true;
        }
    }

    public async ValueTask WriteAsync(ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            await using var fileStream = new FileStream(_filePath, FileMode.Create);

            foreach (var memory in sequence)
            {
                await fileStream.WriteAsync(memory, cancellationToken);
            }
        }
    }

    public async ValueTask WriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        await this.WriteAsync(new ReadOnlySequence<byte>(memory), cancellationToken);
    }

    public async ValueTask<bool> TryDeleteAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            if (!File.Exists(_filePath)) return false;

            File.Delete(_filePath);

            return true;
        }
    }
}
