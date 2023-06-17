using System.Buffers;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using Omnius.Core.Helpers;
using Omnius.Core.Sql;
using Omnius.Core.Streams;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Omnius.Core.Storages;

public sealed class KeyValueFileStorage : AsyncDisposableBase, IKeyValueStorage
{
    private readonly string _dirPath;
    private readonly string _sqlitePath;
    private readonly IBytesPool _bytesPool;

    private readonly AsyncLock _asyncLock = new();

    internal sealed class KeyValueStorageFactory : IKeyValueStorageFactory
    {
        public IKeyValueStorage Create(string dirPath, IBytesPool bytesPool)
        {
            var result = new KeyValueFileStorage(dirPath, bytesPool);
            return result;
        }
    }

    public static IKeyValueStorageFactory Factory { get; } = new KeyValueStorageFactory();

    internal KeyValueFileStorage(string dirPath, IBytesPool bytesPool)
    {
        DirectoryHelper.CreateDirectory(dirPath);

        _dirPath = dirPath;
        _sqlitePath = Path.Combine(dirPath, "sqlite.db");
        _bytesPool = bytesPool;
    }

    protected override async ValueTask OnDisposeAsync()
    {
    }

    private async ValueTask<SQLiteConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        var sqlConnectionStringBuilder = new SQLiteConnectionStringBuilder
        {
            DataSource = _sqlitePath,
            ForeignKeys = true,
        };
        var connection = new SQLiteConnection(sqlConnectionStringBuilder.ToString());
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    internal string GenFilePath(long id)
    {
        var filePath = Path.Combine(_dirPath, "blocks", ComputeRelativeFilePath(id));
        DirectoryHelper.CreateDirectory(Path.GetDirectoryName(filePath)!);
        return filePath;
    }

    internal static string ComputeRelativeFilePath(long id)
    {
        var res = new int[6];

        for (int i = 0; i < res.Length; i++)
        {
            int v = (int)((id >>> (i * 11)) & 0x7FF);
            res[(res.Length - 1) - i] = v;
        }

        return string.Join("/", res);
    }

    public async ValueTask MigrateAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            using var connection = await this.GetConnectionAsync(cancellationToken);

            var query =
@"
CREATE TABLE IF NOT EXISTS keys (
    id INTEGER NOT NULL PRIMARY KEY,
    name TEXT NOT NULL UNIQUE
);
";
            await connection.ExecuteNonQueryAsync(query, cancellationToken);
        }
    }

    public async ValueTask RebuildAsync(CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            using var connection = await this.GetConnectionAsync(cancellationToken);
            var query = "vacuum;";
            await connection.ExecuteNonQueryAsync(query, cancellationToken);
        }
    }

    public async ValueTask<bool> TryChangeKeyAsync(string oldKey, string newKey, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            using var connection = await this.GetConnectionAsync(cancellationToken);

            var query =
$@"
UPDATE keys
    SET name = '{SqliteQueryHelper.EscapeText(newKey)}'
    WHERE name = '{SqliteQueryHelper.EscapeText(oldKey)}'
;
";

            var result = await connection.ExecuteNonQueryAsync(query, cancellationToken);
            return (result == 1);
        }
    }

    public async ValueTask<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            using var connection = await this.GetConnectionAsync(cancellationToken);

            var query =
$@"
SELECT COUNT(1)
    FROM keys
    WHERE name = '{SqliteQueryHelper.EscapeText(key)}'
    LIMIT 1
;
";

            var result = await connection.ExecuteScalarAsync(query, cancellationToken);
            if ((long)result! == 1) return true;

            return false;
        }
    }

    public async IAsyncEnumerable<string> GetKeysAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            using var connection = await this.GetConnectionAsync(cancellationToken);
            var compiler = new SqliteCompiler();
            using var db = new QueryFactory(connection, compiler);

            const int ChunkSize = 5000;
            int offset = 0;
            int limit = ChunkSize;

            for (; ; )
            {
                var rows = await db.Query("keys")
                    .Select("name")
                    .Offset(offset)
                    .Limit(limit)
                    .GetAsync();
                if (!rows.Any()) yield break;

                foreach (var name in rows.Select(n => n.name).OfType<string>())
                {
                    yield return name;
                }

                offset = limit;
                limit += ChunkSize;
            }
        }
    }

    public async ValueTask<IMemoryOwner<byte>?> TryReadAsync(string key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var id = await this.TryReadIdAsync(key, cancellationToken);
            if (id < 0) return null;

            var filePath = this.GenFilePath(id);
            using var stream = new UnbufferedFileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.None, _bytesPool);
            return await stream.ToBytesAsync(_bytesPool, cancellationToken);
        }
    }

    public async ValueTask<bool> TryReadAsync(string key, IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var id = await this.TryReadIdAsync(key, cancellationToken);
            if (id < 0) return false;

            var filePath = this.GenFilePath(id);
            using var stream = new UnbufferedFileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.None, _bytesPool);
            await bufferWriter.WriteAsync(stream, cancellationToken);
            return true;
        }
    }

    private async ValueTask<long> TryReadIdAsync(string key, CancellationToken cancellationToken = default)
    {
        using var connection = await this.GetConnectionAsync(cancellationToken);
        var compiler = new SqliteCompiler();
        using var db = new QueryFactory(connection, compiler);

        var rows = await db.Query("keys")
            .Select("id")
            .Where("name", "=", key)
            .GetAsync();
        if (!rows.Any()) return -1;

        var id = rows
            .Select(n => n.id)
            .OfType<long>()
            .First();
        return id;
    }

    public async ValueTask WriteAsync(string key, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var id = await this.WriteIdAsync(key, cancellationToken);

            var filePath = this.GenFilePath(id);
            using var stream = new UnbufferedFileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, FileOptions.None, _bytesPool);
            await stream.WriteAsync(sequence);
        }
    }

    public async ValueTask WriteAsync(string key, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            var id = await this.WriteIdAsync(key, cancellationToken);

            var filePath = this.GenFilePath(id);
            using var stream = new UnbufferedFileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, FileOptions.None, _bytesPool);
            await stream.WriteAsync(memory);
        }
    }

    private async ValueTask<long> WriteIdAsync(string key, CancellationToken cancellationToken = default)
    {
        using var connection = await this.GetConnectionAsync(cancellationToken);

        var query =
$@"
INSERT
    INTO keys (name)
    VALUES ('{SqliteQueryHelper.EscapeText(key)}')
    RETURNING id
;
";

        var result = await connection.ExecuteScalarAsync(query, cancellationToken);
        return (long)result!;
    }

    public async ValueTask<bool> TryDeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            using var connection = await this.GetConnectionAsync(cancellationToken);

            var query =
$@"
DELETE
    FROM keys
    WHERE name = '{SqliteQueryHelper.EscapeText(key)}'
;
";

            var result = await connection.ExecuteNonQueryAsync(query, cancellationToken);
            return (result == 1);
        }
    }

    public async ValueTask ShrinkAsync(IEnumerable<string> excludedKeys, CancellationToken cancellationToken = default)
    {
        using (await _asyncLock.LockAsync(cancellationToken))
        {
            using var connection = await this.GetConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();
            var compiler = new SqliteCompiler();
            using var db = new QueryFactory(connection, compiler);

            {
                var query =
@"
CREATE TEMP TABLE excluded_keys (
    name TEXT NOT NULL
);
";
                await transaction.ExecuteNonQueryAsync(query, cancellationToken);
            }

            foreach (var chunkedKeys in excludedKeys.Chunk(500))
            {
                var columns = new[] { "name" };
                var values = chunkedKeys.Select(name => new object[] { name.ToString() });
                await db.Query("excluded_keys")
                    .InsertAsync(columns, values, transaction, null, cancellationToken);
            }

            {
                var query =
@"
DELETE FROM keys
    WHERE (name) NOT IN (SELECT (name) FROM excluded_keys);
";
                await transaction.ExecuteNonQueryAsync(query, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
    }
}
