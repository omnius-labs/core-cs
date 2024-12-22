using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Omnius.Core.Testkit;

public unsafe class FixtureFactory
{
    private readonly Random _random;

    public FixtureFactory(int seed)
    {
        _random = new Random(seed);
    }

    public IDisposable GenTempDirectory(out string path)
    {
        path = Path.Combine(Path.GetTempPath(), "_Omnius_Test_" + DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture) + "_" + this.GenRandomString(16));
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return new DirectoryRemover(path);
    }

    private sealed class DirectoryRemover : IDisposable
    {
        private readonly string _path;
        public DirectoryRemover(string path) => _path = path;

        public void Dispose() => Directory.Delete(_path, true);
    }

    public string GenRandomFile(string directoryPath, long size)
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

    public FileStream GenRandomFileStream(string directoryPath)
    {
        int count = 0;

        for (; ; )
        {
            var randomText = this.GenRandomString(16);
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

    public byte[] GenRandomBytes(int length)
    {
        var result = new byte[length];
        _random.NextBytes(result);
        return result;
    }

    // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
    private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public string GenRandomString(int size)
    {
        byte[] buf = new byte[size * 4];
        _random.NextBytes(buf);

        var result = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(buf, i * 4);
            var idx = rnd % _chars.Length;

            result.Append(_chars[idx]);
        }

        return result.ToString();
    }

    // https://stackoverflow.com/questions/14505932/random-datetime-between-range-not-unified-output/14511053
    public DateTime GenRandomDateTimeUtc(DateTime from, DateTime to)
    {
        var range = to - from;
        var randTimeSpan = new TimeSpan((long)(_random.NextDouble() * range.Ticks));
        return (from + randTimeSpan).ToUniversalTime();
    }

    public byte GenRandomUInt8()
    {
        Span<byte> buf = stackalloc byte[1];
        _random.NextBytes(buf);
        return buf[0];
    }

    public sbyte GenRandomInt8()
    {
        Span<byte> buf = stackalloc byte[1];
        _random.NextBytes(buf);
        return (sbyte)buf[0];
    }

    public short GenRandomInt16()
    {
        Span<byte> buf = stackalloc byte[2];
        _random.NextBytes(buf);
        return System.Buffers.Binary.BinaryPrimitives.ReadInt16LittleEndian(buf);
    }

    public ushort GenRandomUInt16()
    {
        Span<byte> buf = stackalloc byte[2];
        _random.NextBytes(buf);
        return System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(buf);
    }

    public int GenRandomInt32()
    {
        Span<byte> buf = stackalloc byte[4];
        _random.NextBytes(buf);
        return System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(buf);
    }

    public uint GenRandomUInt32()
    {
        Span<byte> buf = stackalloc byte[4];
        _random.NextBytes(buf);
        return System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(buf);
    }

    public long GenRandomInt64()
    {
        Span<byte> buf = stackalloc byte[8];
        _random.NextBytes(buf);
        return System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(buf);
    }

    public ulong GenRandomUInt64()
    {
        Span<byte> buf = stackalloc byte[8];
        _random.NextBytes(buf);
        return System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(buf);
    }

    public TcpListener GenTcpListener(IPAddress address, int retry)
    {
        for (int i = 0; ; i++)
        {
            try
            {
                var listener = new TcpListener(address, _random.Next(1024, 65535));
                listener.Start();
                return listener;
            }
            catch (SocketException)
            {
                if (i >= retry) throw;
            }
        }
    }
}
