using System;
using System.IO;
using Omnix.Base;
using Omnix.Io;

namespace Omnix.Configuration
{
    public class Settings
    {
        private const int _bufferSize = 1024 * 256;

        public Settings(string path)
        {
            this.BasePath = path;
        }

        public string BasePath { get; }

        static UnbufferedFileStream GetUniqueFileStream(string path)
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
        {
            return Load(BasePath, name, defaultValueFactory);
        }

        public T Load<T>(string name)
        {
            return Load(BasePath, name, () => default(T));
        }

        public void Save<T>(string name, T value)
        {
            Settings.Save(BasePath, name, value, false);
        }

        public void Save<T>(string name, T value, bool isTypeNameHandling)
        {
            Settings.Save(BasePath, name, value, isTypeNameHandling);
        }

        public static T Load<T>(string directoryPath, string name, Func<T> defaultValueFactory)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            foreach (string extension in new string[] { ".json.gz", ".json.gz.bak" })
            {
                try
                {
                    string configPath = Path.Combine(directoryPath, name + extension);
                    if (!File.Exists(configPath))
                        continue;

                    using (var stream = new UnbufferedFileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.None, BufferPool.Instance))
                    {
                        return JsonNetUtils.Load<T>(stream);
                    }
                }
                catch (Exception e)
                {
                    Log.Warning(e);
                }
            }

            return defaultValueFactory();
        }

        public static T Load<T>(string directoryPath, string name)
        {
            return Load(directoryPath, name, () => default(T));
        }

        public static void Save<T>(string directoryPath, string name, T value, bool isTypeNameHandling)
        {
            string uniquePath = null;

            using (var stream = GetUniqueFileStream(Path.Combine(directoryPath, name + ".tmp")))
            {
                uniquePath = stream.Name;

                JsonNetUtils.Save<T>(stream, value, isTypeNameHandling);
            }

            string newPath = Path.Combine(directoryPath, name + ".json.gz");
            string bakPath = Path.Combine(directoryPath, name + ".json.gz.bak");

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
