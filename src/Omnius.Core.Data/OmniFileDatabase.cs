using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Cryptography;
using Omnius.Core.Data.Internal;
using Omnius.Core.Io;
using Omnius.Core.Serialization;
using Omnius.Core.Serialization.RocketPack;

namespace Omnius.Core.Data
{
    public class OmniFileDatabase : AsyncDisposableBase, IOmniDatabase
    {
        private enum EntityStatus
        {
            Temp,
            Committed,
            Backup,
        }

        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string _configPath;

        private readonly Dictionary<string, string> _map = new Dictionary<string, string>();
        private readonly FileStream _lockFileStream;

        private readonly AsyncLock _asyncLock = new AsyncLock();

        internal sealed class OmniFileDatabaseFactory : IOmniDatabaseFactory
        {
            public async ValueTask<IOmniDatabase> CreateAsync(string configPath, IBytesPool bytesPool)
            {
                var result = new OmniFileDatabase(configPath, bytesPool);
                await result.InitAsync();

                return result;
            }
        }

        public static IOmniDatabaseFactory Factory { get; } = new OmniFileDatabaseFactory();

        internal OmniFileDatabase(string configPath, IBytesPool bytesPool)
        {
            _configPath = configPath;

            // フォルダを作成する
            var directoryPathList = new List<string>
            {
                _configPath,
                this.GetObjectsPath(EntityStatus.Temp),
                this.GetObjectsPath(EntityStatus.Committed),
                this.GetObjectsPath(EntityStatus.Backup)
            };

            foreach (var path in directoryPathList)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            // 排他ロック
            _lockFileStream = new FileStream(Path.Combine(_configPath, "lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        internal async ValueTask InitAsync()
        {
            await this.LoadAsync();
        }

        protected override async ValueTask OnDisposeAsync()
        {
            await this.SaveAsync();
            await _lockFileStream.DisposeAsync();
        }

        private async ValueTask LoadAsync()
        {
            if (InternalRead<OmniFileDatabaseConfig>(_configPath, "config", out var config, OmniFileDatabaseConfig.Formatter))
            {
                foreach (var (key, value) in config.Map)
                {
                    _map.Add(key, value);
                }
            }
        }

        private async ValueTask SaveAsync()
        {
            var config = new OmniFileDatabaseConfig(0, _map);
            InternalSet(_configPath, "config", config, OmniFileDatabaseConfig.Formatter);
        }

        private string GetObjectsPath(EntityStatus entityStatus)
        {
            return Path.Combine(_configPath, "objects", entityStatus.ToString().ToLower());
        }

        private string GeneratePath(string key)
        {
            Span<byte> buffer = stackalloc byte[32];
            Sha2_256.TryComputeHash(key, buffer);
            return OmniBase.ToBase16String(buffer);
        }

        private static bool InternalRead<T>(string basePath, string name, out T value, IRocketPackFormatter<T> formatter)
        {
            value = IRocketPackObject<T>.Empty;

            try
            {
                string directoryPath = Path.Combine(basePath, name);
                string contentPath = Path.Combine(directoryPath, "rpb.gz");
                string crcPath = Path.Combine(directoryPath, "crc");

                using var hub = new Hub(BytesPool.Shared);

                if (!File.Exists(contentPath) || !File.Exists(crcPath))
                {
                    return false;
                }

                using (var fileStream = new UnbufferedFileStream(contentPath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.None, BytesPool.Shared))
                using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    for (; ; )
                    {
                        var readLength = gzipStream.Read(hub.Writer.GetSpan(1024 * 4));
                        if (readLength <= 0)
                        {
                            break;
                        }

                        hub.Writer.Advance(readLength);
                    }
                }

                var sequence = hub.Reader.GetSequence();

                using (var fileStream = new UnbufferedFileStream(crcPath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.None, BytesPool.Shared))
                {
                    var buffer = new byte[4];
                    fileStream.Read(buffer);

                    if (BinaryPrimitives.ReadInt32LittleEndian(buffer) != Crc32_Castagnoli.ComputeHash(sequence))
                    {
                        return false;
                    }
                }

                var reader = new RocketPackReader(sequence, BytesPool.Shared);
                value = formatter.Deserialize(ref reader, 0);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw e;
            }
        }

        private static void InternalSet<T>(string basePath, string name, T value, IRocketPackFormatter<T> formatter)
        {
            try
            {
                string directoryPath = Path.Combine(basePath, name);
                string contentPath = Path.Combine(directoryPath, "opb.gz");
                string crcPath = Path.Combine(directoryPath, "crc");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using var hub = new Hub(BytesPool.Shared);

                var writer = new RocketPackWriter(hub.Writer, BytesPool.Shared);
                formatter.Serialize(ref writer, value, 0);

                var sequence = hub.Reader.GetSequence();

                using (var fileStream = new UnbufferedFileStream(contentPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, FileOptions.None, BytesPool.Shared))
                using (var gzipStream = new GZipStream(fileStream, CompressionLevel.Fastest))
                {
                    var position = sequence.Start;

                    while (sequence.TryGet(ref position, out var memory))
                    {
                        gzipStream.Write(memory.Span);
                    }
                }

                using (var fileStream = new UnbufferedFileStream(crcPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, FileOptions.None, BytesPool.Shared))
                {
                    Span<byte> buffer = stackalloc byte[4];
                    BinaryPrimitives.WriteInt32LittleEndian(buffer, Crc32_Castagnoli.ComputeHash(sequence));
                    fileStream.Write(buffer);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw e;
            }
        }

        private void Commit(string name)
        {
            var temp = Path.Combine(this.GetObjectsPath(EntityStatus.Temp), name);
            var committed = Path.Combine(this.GetObjectsPath(EntityStatus.Committed), name);
            var backup = Path.Combine(this.GetObjectsPath(EntityStatus.Backup), name);

            try
            {
                if (Directory.Exists(backup))
                {
                    Directory.Delete(backup, true);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw e;
            }

            try
            {
                if (Directory.Exists(committed))
                {
                    Directory.Move(committed, backup);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw e;
            }

            try
            {
                if (Directory.Exists(temp))
                {
                    Directory.Move(temp, committed);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw e;
            }
        }

        public async IAsyncEnumerable<string> GetKeysAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.LockAsync())
            {
                foreach (var key in _map.Keys)
                {
                    yield return key;
                }
            }
        }

        public async ValueTask DeleteKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.LockAsync())
            {
                if (_map.TryGetValue(key, out var path))
                {
                    File.Delete(path);
                    _map.Remove(key);
                }
            }
        }

        public async ValueTask<T> ReadAsync<T>(string key, CancellationToken cancellationToken = default) where T : IRocketPackObject<T>
        {
            using (await _asyncLock.LockAsync())
            {
                return await Task.Run(() =>
                {
                    var path = this.GeneratePath(key);

                    foreach (var entityStatus in new[] { EntityStatus.Committed, EntityStatus.Backup })
                    {
                        if (InternalRead(this.GetObjectsPath(entityStatus), path, out var result, IRocketPackObject<T>.Formatter))
                        {
                            return result;
                        }
                    }

                    return IRocketPackObject<T>.Empty;
                });
            }
        }

        public async ValueTask WriteAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : IRocketPackObject<T>
        {
            using (await _asyncLock.LockAsync())
            {
                await Task.Run(() =>
                {
                    var path = this.GeneratePath(key);

                    InternalSet(this.GetObjectsPath(EntityStatus.Temp), path, value, IRocketPackObject<T>.Formatter);
                    this.Commit(path);

                    _map[key] = path;
                });
            }
        }
    }
}
