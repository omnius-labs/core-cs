using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Omnius.Core.Base;
using Omnius.Core.Base.Helpers;
using RocksDbSharp;

namespace Omnius.Core.Storages;

public sealed class KeyValueRocksDbStorage : AsyncDisposableBase, IKeyValueStorage
{
    private readonly string _dirPath;
    private readonly RocksDb _rocksDb;
    private readonly IBytesPool _bytesPool;

    private readonly AsyncLock _asyncLock = new();

    private static readonly Lazy<UTF8Encoding> _utf8Encoding = new Lazy<UTF8Encoding>(() => new UTF8Encoding(false));

    internal sealed class KeyValueStorageFactory : IKeyValueStorageFactory
    {
        public IKeyValueStorage Create(string dirPath, IBytesPool bytesPool)
        {
            var result = new KeyValueRocksDbStorage(dirPath, bytesPool);
            return result;
        }
    }

    public static IKeyValueStorageFactory Factory { get; } = new KeyValueStorageFactory();

    internal KeyValueRocksDbStorage(string dirPath, IBytesPool bytesPool)
    {
        DirectoryHelper.CreateDirectory(dirPath);

        _dirPath = dirPath;
        var rocksDbOptions = new DbOptions().SetCreateIfMissing(true);
        _rocksDb = RocksDb.Open(rocksDbOptions, Path.Combine(dirPath, "rocksdb"));
        _bytesPool = bytesPool;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _rocksDb.Dispose();
    }

    public async ValueTask MigrateAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
        }
    }

    public async ValueTask RebuildAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
        }
    }

    public async ValueTask<bool> TryChangeKeyAsync(string oldKey, string newKey, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var bOldKey = _utf8Encoding.Value.GetBytes(oldKey);
            var bNewKey = _utf8Encoding.Value.GetBytes(newKey);

            var value = _rocksDb.Get(bOldKey);
            if (value is null) return false;

            _rocksDb.Put(bNewKey, value);

            _rocksDb.Remove(bOldKey);

            return true;
        }
    }

    public async ValueTask<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            return _rocksDb.HasKey(_utf8Encoding.Value.GetBytes(key));
        }
    }

    public async IAsyncEnumerable<string> GetKeysAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            using var iter = _rocksDb.NewIterator();
            iter.SeekToFirst();

            while (iter.Valid())
            {
                yield return _utf8Encoding.Value.GetString(iter.Key());
                iter.Next();
            }
        }
    }

    public async ValueTask<IMemoryOwner<byte>?> TryReadAsync(string key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var value = _rocksDb.Get(_utf8Encoding.Value.GetBytes(key));
            if (value is null) return null;

            return new MemoryOwner<byte>(value);
        }
    }

    public async ValueTask<bool> TryReadAsync(string key, IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var value = _rocksDb.Get(_utf8Encoding.Value.GetBytes(key));
            if (value is null) return false;

            bufferWriter.Write(value);
            return true;
        }
    }

    public async ValueTask WriteAsync(string key, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var bKey = _utf8Encoding.Value.GetBytes(key);

            if (sequence.IsSingleSegment)
            {
                _rocksDb.Put(bKey, sequence.FirstSpan);
            }
            else
            {
                using var value = sequence.ToMemoryOwner(_bytesPool);
                _rocksDb.Put(bKey, value.Memory.Span);
            }
        }
    }

    public async ValueTask WriteAsync(string key, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            _rocksDb.Put(_utf8Encoding.Value.GetBytes(key), memory.Span);
        }
    }

    public async ValueTask<bool> TryDeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var bKey = _utf8Encoding.Value.GetBytes(key);
            if (!_rocksDb.HasKey(bKey)) return false;

            _rocksDb.Remove(bKey);
            return true;
        }
    }

    public async ValueTask ShrinkAsync(IEnumerable<string> excludedKeys, CancellationToken cancellationToken = default)
    {
        var excludedKeySet = excludedKeys.ToHashSet();

        using (await _asyncLock.LockAsync(cancellationToken))
        {
            using var iter = _rocksDb.NewIterator();
            iter.SeekToFirst();

            while (iter.Valid())
            {
                var bKey = iter.Key();
                var key = _utf8Encoding.Value.GetString(bKey);

                if (!excludedKeySet.Contains(key))
                {
                    _rocksDb.Remove(bKey);
                }

                iter.Next();
            }
        }
    }
}
