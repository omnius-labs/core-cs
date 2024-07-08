using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Omnius.Core.UnitTestToolkit;

public class FixtureFactory
{
    private static readonly Random _random = new();

    public static IDisposable GenTempDirectory(out string path)
    {
        path = Path.Combine(Path.GetTempPath(), "_Omnius_Test_", DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture) + "_" + Guid.NewGuid().ToString("N"));
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return new DirectoryRemover(path);
    }

    private sealed class DirectoryRemover : IDisposable
    {
        private readonly string _path;
        public DirectoryRemover(string path) => _path = path;

        public void Dispose() => Directory.Delete(_path, true);
    }

    public static string GenRandomFile(string directoryPath, long size)
    {
        using var stream = GenRandomFileStream(directoryPath);

        var buffer = new byte[1024 * 8];

        for (long remain = size; remain > 0; remain -= buffer.Length)
        {
            _random.NextBytes(buffer);
            stream.Write(buffer, 0, (int)Math.Min(buffer.Length, remain));
        }

        return stream.Name;
    }

    public static FileStream GenRandomFileStream(string directoryPath)
    {
        var buffer = new byte[32];

        int count = 0;

        for (; ; )
        {
            var randomText = Guid.NewGuid().ToString("N");
            var tempFilePath = Path.Combine(directoryPath, randomText);

            try
            {
                var stream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 1);
                return stream;
            }
            catch (IOException)
            {
                if (count++ < 1000) continue;
                throw;
            }
        }
    }

    public static byte[] GetRandomBytes(int length)
    {
        var result = new byte[length];
        _random.NextBytes(result);
        return result;
    }

    // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
    private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public static string GetRandomString(int size)
    {
        byte[] data = RandomNumberGenerator.GetBytes(4 * size);

        var result = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % _chars.Length;

            result.Append(_chars[idx]);
        }

        return result.ToString();
    }

    // https://stackoverflow.com/questions/14505932/random-datetime-between-range-not-unified-output/14511053
    public static DateTime GetRandomDateTimeUtc(DateTime from, DateTime to)
    {
        var range = to - from;
        var randTimeSpan = new TimeSpan((long)(_random.NextDouble() * range.Ticks));
        return (from + randTimeSpan).ToUniversalTime();
    }
}
