using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Omnius.Core.Cryptography;
using Omnius.Core.Io;
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

        private readonly FileStream _lockFileStream;

        public OmniFileDatabase(string directoryPath)
        {
            this.DirectoryPath = directoryPath;

            // フォルダを作成する
            var directoryPathList = new List<string>
            {
                this.DirectoryPath,
                this.GeneratePath(EntityStatus.Temp),
                this.GeneratePath(EntityStatus.Committed),
                this.GeneratePath(EntityStatus.Backup)
            };

            foreach (var path in directoryPathList)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            // 排他ロック
            _lockFileStream = new FileStream(Path.Combine(this.DirectoryPath, "lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        protected override async ValueTask OnDisposeAsync()
        {
            await _lockFileStream.DisposeAsync();
        }

        public string DirectoryPath { get; }

        private string GeneratePath(EntityStatus entityStatus)
        {
            return Path.Combine(this.DirectoryPath, "objects", entityStatus.ToString().ToLower());
        }

        private static bool InternalLoad<T>(string basePath, string name, out T value, IRocketPackFormatter<T> formatter)
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

        private static void InternalSave<T>(string basePath, string name, T value, IRocketPackFormatter<T> formatter)
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
            var temp = Path.Combine(this.GeneratePath(EntityStatus.Temp), name);
            var committed = Path.Combine(this.GeneratePath(EntityStatus.Committed), name);
            var backup = Path.Combine(this.GeneratePath(EntityStatus.Backup), name);

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

        public async ValueTask<T> LoadAsync<T>(string key) where T : IRocketPackObject<T>
        {
            return await Task.Run(() =>
            {
                foreach (var entityStatus in new[] { EntityStatus.Committed, EntityStatus.Backup })
                {
                    if(InternalLoad(this.GeneratePath(entityStatus), key, out var result, IRocketPackObject<T>.Formatter))
                    {
                        return result;
                    }
                }

                return IRocketPackObject<T>.Empty;
            });
        }

        public async ValueTask SaveAsync<T>(string key, T value) where T : IRocketPackObject<T>
        {
            await Task.Run(() =>
            {
                InternalSave(this.GeneratePath(EntityStatus.Temp), key, value, IRocketPackObject<T>.Formatter);
                this.Commit(key);
            });
        }
    }

    public class SettingsDatabaseException : Exception
    {
        public SettingsDatabaseException() { }
        public SettingsDatabaseException(string message) : base(message) { }
    }
}
