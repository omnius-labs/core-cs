using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using Omnix.Base;
using Omnix.Configuration.Internal;
using Omnix.Cryptography;
using Omnix.Io;
using Omnix.Serialization.RocketPack;

namespace Omnix.Configuration
{
    public class SettingsDatabase : DisposableBase, ISettingsDatabase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly FileStream _lockFileStream;

        private volatile bool _disposed;

        public SettingsDatabase(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            this.DirectoryPath = directoryPath;

            // 排他ロック
            _lockFileStream = new FileStream(Path.Combine(this.DirectoryPath, "lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public string DirectoryPath { get; }

        private enum EntityStatus
        {
            UnCommitted,
            Committed,
            Backup,
        }

        private string GeneratePath(EntityStatus entityStatus)
        {
            return Path.Combine(this.DirectoryPath, "Objects", entityStatus.ToString());
        }

        public static bool TryGet<T>(string directoryPath, string name, out T value, IRocketPackFormatter<T> formatter)
        {
            value = default!;

            string contentPath = Path.Combine(directoryPath, $"{name}.rpk.gz");
            string crcPath = Path.Combine(directoryPath, $"{name}.crc");

            using var hub = new Hub();

            try
            {
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
                        if (readLength < 0)
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

        public static void Set<T>(string directoryPath, string name, T value, IRocketPackFormatter<T> formatter)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string contentPath = Path.Combine(directoryPath, $"{name}.rpk.gz");
            string crcPath = Path.Combine(directoryPath, $"{name}.crc");

            using var hub = new Hub();

            formatter.Serialize(new RocketPackWriter(hub.Writer, BufferPool.Shared), value, 0);
            hub.Writer.Complete();

            var sequence = hub.Reader.GetSequence();

            using (var fileStream = new UnbufferedFileStream(contentPath, FileMode.Create, FileAccess.Write, FileShare.None, FileOptions.None, BufferPool.Shared))
            using (var gzipStream = new GZipStream(fileStream, CompressionLevel.Fastest))
            {
                var position = sequence.Start;

                while (sequence.TryGet(ref position, out var memory))
                {
                    gzipStream.Write(memory.Span);
                }
            }

            using (var fileStream = new UnbufferedFileStream(crcPath, FileMode.Create, FileAccess.Write, FileShare.None, FileOptions.None, BufferPool.Shared))
            {
                Span<byte> buffer = stackalloc byte[4];
                BinaryPrimitives.WriteInt32LittleEndian(buffer, Crc32_Castagnoli.ComputeHash(sequence));
                fileStream.Write(buffer);
            }
        }

        public int GetVersion()
        {
            foreach (var entityStatus in new[] { EntityStatus.UnCommitted, EntityStatus.Committed, EntityStatus.Backup })
            {
                if (TryGet(this.GeneratePath(entityStatus), "#Version", out var version1, SettingsDatabaseVersion.Formatter))
                {
                    return (int)version1.Value;
                }
            }

            return 0;
        }

        public void SetVersion(int version)
        {
            Set(this.GeneratePath(EntityStatus.UnCommitted), "#Version", new SettingsDatabaseVersion((uint)version), SettingsDatabaseVersion.Formatter);
        }

        public bool TryGetContent<T>(string name, out T value) where T : RocketPackMessageBase<T>
        {
            value = default!;

            foreach (var entityStatus in new[] { EntityStatus.UnCommitted, EntityStatus.Committed, EntityStatus.Backup })
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

            foreach (var entityStatus in new[] { EntityStatus.UnCommitted, EntityStatus.Committed, EntityStatus.Backup })
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
            Set(this.GeneratePath(EntityStatus.UnCommitted), name, value, RocketPackMessageBase<T>.Formatter);
        }

        public void SetContent<T>(string name, T value, IRocketPackFormatter<T> formatter)
        {
            Set(this.GeneratePath(EntityStatus.UnCommitted), name, value, formatter);
        }

        public void Commit()
        {
            var source = this.GeneratePath(EntityStatus.UnCommitted);
            var destination = this.GeneratePath(EntityStatus.Committed);

            if (!Directory.Exists(source))
            {
                return;
            }

            foreach (var path in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
            {
                File.Copy(source, Path.Combine(destination, Path.GetFileName(path)), true);
            }

            Directory.Delete(source);
        }

        public void Rollback()
        {
            var path = this.GeneratePath(EntityStatus.UnCommitted);

            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

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
