using System.Buffers;
using Nito.AsyncEx;
using Omnius.Core.Helpers;

namespace Omnius.Core.Storages;

public sealed class SingleValueFileStorage : DisposableBase, ISingleValueStorage
{
    private readonly string _filePath;
    private readonly IBytesPool _bytesPool;

    private readonly AsyncReaderWriterLock _asyncLock = new();

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

    protected override void OnDispose(bool disposing)
    {
    }

    public async ValueTask<IMemoryOwner<byte>?> TryReadAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.ReaderLockAsync(cancellationToken))
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
        using (await _asyncLock.ReaderLockAsync(cancellationToken))
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

    public async ValueTask<bool> TryWriteAsync(ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.WriterLockAsync(cancellationToken))
        {
            await using var fileStream = new FileStream(_filePath, FileMode.Create);

            foreach (var memory in sequence)
            {
                await fileStream.WriteAsync(memory, cancellationToken);
            }

            return true;
        }
    }

    public async ValueTask<bool> TryWriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        return await this.TryWriteAsync(new ReadOnlySequence<byte>(memory), cancellationToken);
    }

    public async ValueTask<bool> TryDeleteAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.WriterLockAsync(cancellationToken))
        {
            if (!File.Exists(_filePath)) return false;

            File.Delete(_filePath);

            return true;
        }
    }
}
