using System.Buffers;
using System.Runtime.CompilerServices;
using LiteDB;
using Omnius.Core.Helpers;

namespace Omnius.Core.Storages;

public static class KeyValueLiteDatabaseStorage
{
    internal sealed class KeyValueStorageFactory : IKeyValueStorageFactory
    {
        public IKeyValueStorage<TKey> Create<TKey>(string path, IBytesPool bytesPool)
            where TKey : notnull
        {
            var result = new KeyValueLiteDatabaseStorage<TKey>(path, bytesPool);
            return result;
        }
    }

    public static IKeyValueStorageFactory Factory { get; } = new KeyValueStorageFactory();
}

public sealed class KeyValueLiteDatabaseStorage<TKey> : DisposableBase, IKeyValueStorage<TKey>
    where TKey : notnull
{
    private readonly IBytesPool _bytesPool;
    private readonly LiteDatabase _database;

    private readonly AsyncLock _asyncLock = new();

    internal KeyValueLiteDatabaseStorage(string dirPath, IBytesPool bytesPool)
    {
        DirectoryHelper.CreateDirectory(dirPath);

        _database = new LiteDatabase(Path.Combine(dirPath, "lite.db"));
        _database.UtcDate = true;

        _bytesPool = bytesPool;
    }

    protected override void OnDispose(bool disposing)
    {
        _database.Dispose();
    }

    public async ValueTask MigrateAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            if (_database.UserVersion <= 0)
            {
                var col = this.GetCollection();
                col.EnsureIndex(n => n.Key, true);

                _database.UserVersion = 1;
            }
        }
    }

    public async ValueTask RebuildAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            _database.Rebuild();
        }
    }

    private ILiteStorage<long> GetStorage()
    {
        var storage = _database.GetStorage<long>();
        return storage;
    }

    private ILiteCollection<BlockLink> GetCollection()
    {
        var col = _database.GetCollection<BlockLink>("block_links");
        return col;
    }

    public async ValueTask<bool> TryChangeKeyAsync(TKey oldKey, TKey newKey, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var col = this.GetCollection();

            if (col.Exists(Query.EQ("Key", new BsonValue(newKey)))) return false;

            var meta = col.FindOne(Query.EQ("Key", new BsonValue(oldKey)));
            if (meta is null) return false;

            meta.Key = newKey;

            col.Update(meta);

            return true;
        }
    }

    public async ValueTask<bool> ContainsKeyAsync(TKey key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var col = this.GetCollection();
            return col.Exists(Query.EQ("Key", new BsonValue(key)));
        }
    }

    public async IAsyncEnumerable<TKey> GetKeysAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var col = this.GetCollection();
            foreach (var key in col.FindAll().Select(n => n.Key).ToArray())
            {
                yield return key;
            }
        }
    }

    public async ValueTask<IMemoryOwner<byte>?> TryReadAsync(TKey key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var col = this.GetCollection();
            var meta = col.FindOne(Query.EQ("Key", new BsonValue(key)));
            if (meta is null) return null;

            var storage = this.GetStorage();
            if (!storage.Exists(meta.Id))
            {
                col.Delete(meta.Id);
                return null;
            }

            await using var liteFileStream = storage.OpenRead(meta.Id);

            var memoryOwner = _bytesPool.Memory.Rent((int)liteFileStream.Length).Shrink((int)liteFileStream.Length);

            while (liteFileStream.Position < liteFileStream.Length)
            {
                await liteFileStream.ReadAsync(memoryOwner.Memory[(int)liteFileStream.Position..], cancellationToken);
            }

            return memoryOwner;
        }
    }

    public async ValueTask<bool> TryReadAsync(TKey key, IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var col = this.GetCollection();
            var meta = col.FindOne(Query.EQ("Key", new BsonValue(key)));
            if (meta is null) return false;

            var storage = this.GetStorage();
            if (!storage.Exists(meta.Id))
            {
                col.Delete(meta.Id);
                return false;
            }

            await using var liteFileStream = storage.OpenRead(meta.Id);

            while (liteFileStream.Position < liteFileStream.Length)
            {
                int readLength = await liteFileStream.ReadAsync(bufferWriter.GetMemory(), cancellationToken);
                bufferWriter.Advance(readLength);
            }

            return true;
        }
    }

    public async ValueTask<bool> TryWriteAsync(TKey key, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var col = this.GetCollection();
            if (col.Exists(Query.EQ("Key", new BsonValue(key)))) return false;

            var id = col.Insert(new BlockLink() { Key = key }).AsInt64;

            var storage = this.GetStorage();
            await using var liteFileStream = storage.OpenWrite(id, "-");

            foreach (var memory in sequence)
            {
                await liteFileStream.WriteAsync(memory, cancellationToken);
            }

            return true;
        }
    }

    public async ValueTask<bool> TryWriteAsync(TKey key, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        return await this.TryWriteAsync(key, new ReadOnlySequence<byte>(memory), cancellationToken);
    }

    public async ValueTask<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var col = this.GetCollection();
            var meta = col.FindOne(Query.EQ("Key", new BsonValue(key)));
            if (meta is null) return false;

            var storage = this.GetStorage();
            storage.Delete(meta.Id);

            return col.Delete(meta.Id);
        }
    }

    private sealed class BlockLink
    {
        public long Id { get; set; }

        public TKey Key { get; set; } = default!;
    }
}
