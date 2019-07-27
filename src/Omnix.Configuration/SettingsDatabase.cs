using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Omnix.Algorithms.Cryptography;
using Omnix.Base;
using Omnix.Configuration.Internal;
using Omnix.Io;
using Omnix.Serialization.RocketPack;

namespace Omnix.Configuration
{
    public class SettingsDatabase : DisposableBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly FileStream _lockFileStream;

        public SettingsDatabase(string directoryPath)
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

        public string DirectoryPath { get; }

        private enum EntityStatus
        {
            Temp,
            Committed,
            Backup,
        }

        private string GeneratePath(EntityStatus entityStatus)
        {
            return Path.Combine(this.DirectoryPath, "Objects", entityStatus.ToString());
        }

        private static bool TryGet<T>(string basePath, string name, out T value, IRocketPackFormatter<T> formatter)
        {
            value = default!;

            try
            {
                string directoryPath = Path.Combine(basePath, name);
                string contentPath = Path.Combine(directoryPath, "rpb.gz");
                string crcPath = Path.Combine(directoryPath, "crc");

                using var hub = new Hub();

                if (!File.Exists(contentPath) || !File.Exists(crcPath))
                {
                    return false;
                }

                using (var fileStream = new UnbufferedFileStream(contentPath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.None, BufferPool.Shared))
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

                hub.Writer.Complete();

                var sequence = hub.Reader.GetSequence();

                using (var fileStream = new UnbufferedFileStream(crcPath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.None, BufferPool.Shared))
                {
                    Span<byte> buffer = stackalloc byte[4];
                    fileStream.Read(buffer);

                    if (BinaryPrimitives.ReadInt32LittleEndian(buffer) != Crc32_Castagnoli.ComputeHash(sequence))
                    {
                        return false;
                    }
                }

                value = formatter.Deserialize(new RocketPackReader(sequence, BufferPool.Shared), 0);

                hub.Reader.Complete();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return false;
        }

        private static void Set<T>(string basePath, string name, T value, IRocketPackFormatter<T> formatter)
        {
            try
            {
                string directoryPath = Path.Combine(basePath, name);
                string contentPath = Path.Combine(directoryPath, "rpb.gz");
                string crcPath = Path.Combine(directoryPath, "crc");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using var hub = new Hub();

                formatter.Serialize(new RocketPackWriter(hub.Writer, BufferPool.Shared), value, 0);
                hub.Writer.Complete();

                var sequence = hub.Reader.GetSequence();

                using (var fileStream = new UnbufferedFileStream(contentPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, FileOptions.None, BufferPool.Shared))
                using (var gzipStream = new GZipStream(fileStream, CompressionLevel.Fastest))
                {
                    var position = sequence.Start;

                    while (sequence.TryGet(ref position, out var memory))
                    {
                        gzipStream.Write(memory.Span);
                    }
                }

                using (var fileStream = new UnbufferedFileStream(crcPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, FileOptions.None, BufferPool.Shared))
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

        public bool TryGetVersion(out uint version)
        {
            version = 0;

            if (this.TryGetContent("#Version", out var databaseVersion, SettingsDatabaseVersion.Formatter))
            {
                version = databaseVersion.Value;
                return true;
            }

            return false;
        }

        public void SetVersion(uint version)
        {
            this.SetContent("#Version", new SettingsDatabaseVersion(version), SettingsDatabaseVersion.Formatter);
        }

        public bool TryGetContent<T>(string name, out T value) where T : RocketPackMessageBase<T>
        {
            value = default!;

            foreach (var entityStatus in new[] { EntityStatus.Committed, EntityStatus.Backup })
            {
                if (TryGet(this.GeneratePath(entityStatus), name, out var result, RocketPackMessageBase<T>.Formatter))
                {
                    value = result;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetContent<T>(string name, out T value, IRocketPackFormatter<T> formatter)
        {
            value = default!;

            foreach (var entityStatus in new[] { EntityStatus.Committed, EntityStatus.Backup })
            {
                if (TryGet(this.GeneratePath(entityStatus), name, out var result, formatter))
                {
                    value = result;
                    return true;
                }
            }

            return false;
        }

        public void SetContent<T>(string name, T value) where T : RocketPackMessageBase<T>
        {
            Set(this.GeneratePath(EntityStatus.Temp), name, value, RocketPackMessageBase<T>.Formatter);

            this.Commit(name);
        }

        public void SetContent<T>(string name, T value, IRocketPackFormatter<T> formatter)
        {
            Set(this.GeneratePath(EntityStatus.Temp), name, value, formatter);

            this.Commit(name);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _lockFileStream.Dispose();
            }
        }
    }

    public class SettingsDatabaseException : Exception
    {
        public SettingsDatabaseException() { }
        public SettingsDatabaseException(string message) : base(message) { }
    }
}
