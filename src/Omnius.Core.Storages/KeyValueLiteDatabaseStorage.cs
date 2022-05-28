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

    private readonly Nito.AsyncEx.AsyncReaderWriterLock _asyncLock = new();

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
        using (await _asyncLock.WriterLockAsync(cancellationToken))
        {
            if (_database.UserVersion <= 0)
            {
                var col = this.GetLinkCollection();
                col.EnsureIndex(n => n.Key, true);

                _database.UserVersion = 1;
            }
        }
    }

    public async ValueTask RebuildAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.WriterLockAsync(cancellationToken))
        {
            _database.Rebuild();
        }
    }

    private ILiteStorage<long> GetStorage()
    {
        var storage = _database.GetStorage<long>();
        return storage;
    }

    private ILiteCollection<Link> GetLinkCollection()
    {
        var col = _database.GetCollection<Link>("links");
        return col;
    }

    public async ValueTask<bool> TryChangeKeyAsync(TKey oldKey, TKey newKey, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.WriterLockAsync(cancellationToken))
        {
            var links = this.GetLinkCollection();
            if (links.Exists(Query.EQ("Key", new BsonValue(newKey)))) return false;

            var link = links.FindOne(Query.EQ("Key", new BsonValue(oldKey)));
            if (link is null) return false;

            link.Key = newKey;

            links.Update(link);

            return true;
        }
    }

    public async ValueTask<bool> ContainsKeyAsync(TKey key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.ReaderLockAsync(cancellationToken))
        {
            var links = this.GetLinkCollection();
            return links.Exists(Query.EQ("Key", new BsonValue(key)));
        }
    }

    public async IAsyncEnumerable<TKey> GetKeysAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.ReaderLockAsync(cancellationToken))
        {
            var links = this.GetLinkCollection();

            foreach (var key in links.FindAll().Select(n => n.Key))
            {
                yield return key;
            }
        }
    }

    public async ValueTask<IMemoryOwner<byte>?> TryReadAsync(TKey key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.ReaderLockAsync(cancellationToken))
        {
            var links = this.GetLinkCollection();
            var link = links.FindOne(Query.EQ("Key", new BsonValue(key)));
            if (link is null) return null;

            var storage = this.GetStorage();
            if (!storage.Exists(link.Id))
            {
                links.Delete(link.Id);
                return null;
            }

            using var liteFileStream = storage.OpenRead(link.Id);

            var memoryOwner = _bytesPool.Memory.Rent((int)liteFileStream.Length).Shrink((int)liteFileStream.Length);

            while (liteFileStream.Position < liteFileStream.Length)
            {
                liteFileStream.Read(memoryOwner.Memory[(int)liteFileStream.Position..].Span);
            }

            return memoryOwner;
        }
    }

    public async ValueTask<bool> TryReadAsync(TKey key, IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.ReaderLockAsync(cancellationToken))
        {
            var links = this.GetLinkCollection();
            var link = links.FindOne(Query.EQ("Key", new BsonValue(key)));
            if (link is null) return false;

            var storage = this.GetStorage();
            if (!storage.Exists(link.Id))
            {
                links.Delete(link.Id);
                return false;
            }

            using var liteFileStream = storage.OpenRead(link.Id);

            while (liteFileStream.Position < liteFileStream.Length)
            {
                int readLength = liteFileStream.Read(bufferWriter.GetMemory().Span);
                bufferWriter.Advance(readLength);
            }

            return true;
        }
    }

    public async ValueTask WriteAsync(TKey key, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.ReaderLockAsync(cancellationToken))
        {
            var links = this.GetLinkCollection();
            if (links.Exists(Query.EQ("Key", new BsonValue(key)))) return;

            var id = links.Insert(new Link() { Key = key }).AsInt64;

            var storage = this.GetStorage();
            using var liteFileStream = storage.OpenWrite(id, "-");

            foreach (var memory in sequence)
            {
                liteFileStream.Write(memory.Span);
            }
        }
    }

    public async ValueTask WriteAsync(TKey key, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        await this.WriteAsync(key, new ReadOnlySequence<byte>(memory), cancellationToken);
    }

    public async ValueTask<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.ReaderLockAsync(cancellationToken))
        {
            var links = this.GetLinkCollection();
            var link = links.FindOne(Query.EQ("Key", new BsonValue(key)));
            if (link is null) return false;

            var storage = this.GetStorage();
            storage.Delete(link.Id);

            return links.Delete(link.Id);
        }
    }

    private sealed class Link
    {
        public long Id { get; set; }

        public TKey Key { get; set; } = default!;
    }
}
