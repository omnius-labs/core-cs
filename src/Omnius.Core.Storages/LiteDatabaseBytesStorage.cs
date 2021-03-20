using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Nito.AsyncEx;
using Omnius.Core.Helpers;

namespace Omnius.Core.Storages
{
    public static class LiteDatabaseBytesStorage
    {
        internal sealed class BytesStorageFactory : IBytesStorageFactory
        {
            public IBytesStorage<TKey> Create<TKey>(string path, IBytesPool bytesPool)
                where TKey : notnull, IEquatable<TKey>
            {
                var result = new LiteDatabaseBytesStorage<TKey>(path, bytesPool);
                return result;
            }
        }

        public static IBytesStorageFactory Factory { get; } = new BytesStorageFactory();
    }

    public sealed class LiteDatabaseBytesStorage<TKey> : DisposableBase, IBytesStorage<TKey>
        where TKey : notnull, IEquatable<TKey>
    {
        private readonly IBytesPool _bytesPool;
        private readonly LiteDatabase _database;

        private readonly AsyncReaderWriterLock _asyncLock = new();

        internal LiteDatabaseBytesStorage(string dirPath, IBytesPool bytesPool)
        {
            DirectoryHelper.CreateDirectory(dirPath);

            _database = new LiteDatabase(Path.Combine(dirPath, "lite.db"));
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
                    var col = this.GetCollection();
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

        private ILiteCollection<BlockLink> GetCollection()
        {
            var col = _database.GetCollection<BlockLink>("block_links");
            return col;
        }

        public async ValueTask ChangeKeyAsync(TKey oldKey, TKey newKey, CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.WriterLockAsync(cancellationToken))
            {
                var col = this.GetCollection();
                if (col.Exists(n => n.Key.Equals(newKey))) throw new DuplicateKeyException();

                var meta = col.FindOne(n => n.Key.Equals(oldKey));
                if (meta is null) throw new KeyNotFoundException();

                meta.Key = newKey;

                col.Update(meta);
            }
        }

        public async ValueTask<bool> ContainsKeyAsync(TKey key, CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.ReaderLockAsync(cancellationToken))
            {
                var col = this.GetCollection();
                return col.Exists(n => n.Key.Equals(key));
            }
        }

        public async IAsyncEnumerable<TKey> GetKeysAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.ReaderLockAsync(cancellationToken))
            {
                var col = this.GetCollection();
                foreach (var key in col.FindAll().Select(n => n.Key))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return key;
                }
            }
        }

        public async ValueTask WriteAsync(TKey key, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.WriterLockAsync(cancellationToken))
            {
                var col = this.GetCollection();
                if (col.Exists(n => n.Key.Equals(key))) throw new DuplicateKeyException();

                var id = col.Insert(new BlockLink() { Key = key }).AsInt64;

                var storage = this.GetStorage();
                await using var liteFileStream = storage.OpenWrite(id, "-");

                foreach (var memory in sequence)
                {
                    await liteFileStream.WriteAsync(memory, cancellationToken);
                }
            }
        }

        public async ValueTask WriteAsync(TKey key, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
        {
            await this.WriteAsync(key, new ReadOnlySequence<byte>(memory), cancellationToken);
        }

        public async ValueTask<IMemoryOwner<byte>?> TryReadAsync(TKey key, CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.ReaderLockAsync(cancellationToken))
            {
                var col = this.GetCollection();
                var meta = col.FindOne(n => n.Key.Equals(key));
                if (meta is null) return null;

                var storage = this.GetStorage();
                await using var liteFileStream = storage.OpenRead(meta.Id);

                var memoryOwner = _bytesPool.Memory.Rent((int)liteFileStream.Length);

                while (liteFileStream.Position < liteFileStream.Length)
                {
                    await liteFileStream.ReadAsync(memoryOwner.Memory[(int)liteFileStream.Position..], cancellationToken);
                }

                return memoryOwner;
            }
        }

        public async ValueTask<bool> TryReadAsync(TKey key, IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.ReaderLockAsync(cancellationToken))
            {
                var col = this.GetCollection();
                var meta = col.FindOne(n => n.Key.Equals(key));
                if (meta is null) return false;

                var storage = this.GetStorage();
                await using var liteFileStream = storage.OpenRead(meta.Id);

                while (liteFileStream.Position < liteFileStream.Length)
                {
                    int readLength = await liteFileStream.ReadAsync(bufferWriter.GetMemory(), cancellationToken);
                    bufferWriter.Advance(readLength);
                }

                return true;
            }
        }

        public async ValueTask<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.WriterLockAsync(cancellationToken))
            {
                var col = this.GetCollection();
                var meta = col.FindOne(n => n.Key.Equals(key));
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
}
