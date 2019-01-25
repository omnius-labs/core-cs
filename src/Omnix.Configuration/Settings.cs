using System;
using System.IO;
using System.IO.Compression;
using Omnix.Base;
using Omnix.Io;
using Omnix.Serialization.RocketPack;

namespace Omnix.Configuration
{
    public class Settings
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private const int _bufferSize = 1024 * 256;

        public Settings(string path)
        {
            this.BasePath = path;
        }

        public string BasePath { get; }

        private static UnbufferedFileStream GetUniqueFileStream(string path)
        {
            if (!File.Exists(path))
            {
                try
                {
                    return new UnbufferedFileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, FileOptions.None, BufferPool.Shared);
                }
                catch (DirectoryNotFoundException)
                {
                    throw;
                }
                catch (IOException)
                {

                }
            }

            for (int index = 1; ; index++)
            {
                string text = string.Format(@"{0}\{1} ({2}){3}",
                    Path.GetDirectoryName(path),
                    Path.GetFileNameWithoutExtension(path),
                    index,
                    Path.GetExtension(path));

                if (!File.Exists(text))
                {
                    try
                    {
                        return new UnbufferedFileStream(text, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, FileOptions.None, BufferPool.Shared);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        throw;
                    }
                    catch (IOException)
                    {
                        if (index > 1024) throw;
                    }
                }
            }
        }

        public T Load<T>(string name, Func<T> defaultValueFactory)
            where T : RocketPackMessageBase<T>
        {
            return Load(BasePath, name, defaultValueFactory);
        }

        public static T Load<T>(string directoryPath, string name)
            where T : RocketPackMessageBase<T>
        {
            return Load(directoryPath, name, () => default(T));
        }

        public T Load<T>(string name)
            where T : RocketPackMessageBase<T>
        {
            return Load(BasePath, name, () => default(T));
        }

        public void Save<T>(string name, T value)
            where T : RocketPackMessageBase<T>
        {
            Save(BasePath, name, value, false);
        }

        public void Save<T>(string name, T value, bool isTypeNameHandling)
            where T : RocketPackMessageBase<T>
        {
            Save(BasePath, name, value, isTypeNameHandling);
        }

        public static T Load<T>(string directoryPath, string name, Func<T> defaultValueFactory)
            where T : RocketPackMessageBase<T>
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            foreach (string extension in new string[] { ".rpk.gz", ".rpk.gz.bak" })
            {
                string configPath = Path.Combine(directoryPath, name + extension);
                var hub = new Hub();

                try
                {
                    if (!File.Exists(configPath))
                    {
                        continue;
                    }

                    using (var fileStream = new UnbufferedFileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.None, BufferPool.Shared))
                    using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        for (; ; )
                        {
                            var readLength = gzipStream.Read(hub.Writer.GetSpan(1024 * 4));
                            if (readLength < 0) break;

                            hub.Writer.Advance(readLength);
                        }
                    }

                    hub.Writer.Complete();

                    var result = RocketPackMessageBase<T>.Import(hub.Reader.GetSequence(), BufferPool.Shared);

                    hub.Reader.Complete();

                    return result;
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
                finally
                {
                    hub.Reset();
                }
            }

            return defaultValueFactory();
        }


        public static void Save<T>(string directoryPath, string name, T value, bool isTypeNameHandling)
            where T : RocketPackMessageBase<T>
        {
            string uniquePath = null;
            var hub = new Hub();

            value.Export(hub.Writer, BufferPool.Shared);
            hub.Writer.Complete();

            using (var fileStream = GetUniqueFileStream(Path.Combine(directoryPath, name + ".tmp")))
            using (var gzipStream = new GZipStream(fileStream, CompressionLevel.Fastest))
            {
                uniquePath = fileStream.Name;

                var sequence = hub.Reader.GetSequence();
                var position = sequence.Start;

                while (sequence.TryGet(ref position, out var memory))
                {
                    gzipStream.Write(memory.Span);
                }
            }

            string newPath = Path.Combine(directoryPath, name + ".rpk.gz");
            string bakPath = Path.Combine(directoryPath, name + ".rpk.gz.bak");

            if (File.Exists(newPath))
            {
                if (File.Exists(bakPath))
                {
                    File.Delete(bakPath);
                }

                File.Move(newPath, bakPath);
            }

            File.Move(uniquePath, newPath);
        }
    }
}
