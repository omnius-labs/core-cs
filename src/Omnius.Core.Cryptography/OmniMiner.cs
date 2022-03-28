using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Helpers;

namespace Omnius.Core.Cryptography;

public static class OmniMiner
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public static async ValueTask<OmniHashcash> ComputeAsync(ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> key, OmniHashcashAlgorithmType hashcashAlgorithmType, int limit, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (!EnumHelper.IsValid(hashcashAlgorithmType)) throw new ArgumentException(nameof(OmniHashcashAlgorithmType));

        await Task.Delay(1, cancellationToken).ConfigureAwait(false);

        if (hashcashAlgorithmType == OmniHashcashAlgorithmType.Sha2_256)
        {
            var value = Hmac_Sha2_256.ComputeHash(sequence, key.Span);
            var result = await Computer.ComputeAsync(value, limit, timeout, cancellationToken);

            return new OmniHashcash(OmniHashcashAlgorithmType.Sha2_256, result);
        }

        throw new NotSupportedException(nameof(hashcashAlgorithmType));
    }

    private static class Computer
    {
        private static readonly string _path;

        static Computer()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    _path = "hashcash.x64.exe";
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    _path = "hashcash.x64";
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static async ValueTask<byte[]> ComputeAsync(ReadOnlyMemory<byte> value, int limit, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            if (value.Length != 32) throw new ArgumentOutOfRangeException(nameof(value));

            var stopwatch = Stopwatch.StartNew();

            if (limit < 0) limit = 256;

            var info = new ProcessStartInfo(_path)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                Arguments = $"--type=simple_sha2_256 --value={Convert.ToBase64String(value.Span)}",
            };

            var status = new Status();

            using (var process = Process.Start(info))
            {
                if (process is null) throw new Exception("Failed to Process Start");
                process.PriorityClass = ProcessPriorityClass.Idle;

                var readTask = ReadAsync(process, status, cancellationToken);
                var writeTask = WriteAsync(process, status, limit, timeout, stopwatch, cancellationToken);

                await Task.WhenAll(readTask, writeTask);
            }

            return status.GetResult();
        }

        private static async Task ReadAsync(Process process, Status status, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1).ConfigureAwait(false);

            try
            {
                while (!process.HasExited)
                {
                    var line = process.StandardOutput.ReadLine();
                    if (line is null) return;

                    var pair = line.Split(" ");
                    var difficulty = int.Parse(pair[0]);
                    var result = Convert.FromBase64String(pair[1]);

                    status.SetDifficultyAndResult(difficulty, result);
                }
            }
            catch (IOException e)
            {
                _logger.Debug(e);
            }
        }

        private static async Task WriteAsync(Process process, Status status, int limit, TimeSpan timeout, Stopwatch stopwatch, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1).ConfigureAwait(false);

            try
            {
                var writer = new StreamWriter(process.StandardInput.BaseStream, new UTF8Encoding(false));
                writer.NewLine = "\n";

                for (; ; )
                {
                    if (process.HasExited) return;
                    if (stopwatch.Elapsed > timeout || status.GetDifficulty() >= limit) break;

                    // keep alive command
                    writer.WriteLine("a");
                    writer.Flush();

                    if (cancellationToken.WaitHandle.WaitOne(1000)) break;
                }

                // stop command
                writer.WriteLine("e");
                writer.Flush();
            }
            catch (IOException e)
            {
                _logger.Debug(e);
            }
        }

        private sealed class Status
        {
            private int _difficulty;
            private byte[] _result = Array.Empty<byte>();
            private object _lockObject = new();

            public int GetDifficulty()
            {
                lock (_lockObject)
                {
                    return _difficulty;
                }
            }

            public byte[] GetResult()
            {
                lock (_lockObject)
                {
                    return _result;
                }
            }

            public void SetDifficultyAndResult(int difficulty, byte[] result)
            {
                lock (_lockObject)
                {
                    _difficulty = difficulty;
                    _result = result;
                }
            }
        }
    }

    public static uint Verify(OmniHashcash hashcash, ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> key)
    {
        if (hashcash is null) throw new ArgumentNullException(nameof(hashcash));

        if (hashcash.AlgorithmType == OmniHashcashAlgorithmType.Sha2_256)
        {
            var value = Hmac_Sha2_256.ComputeHash(sequence, key.Span);
            return Verifier.Verify(hashcash.Result.Span, value);
        }

        return 0;
    }

    private static class Verifier
    {
        public static uint Verify(ReadOnlySpan<byte> result, ReadOnlySpan<byte> value)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (result.Length != 32) throw new ArgumentOutOfRangeException(nameof(result));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length != 32) throw new ArgumentOutOfRangeException(nameof(value));

            var hash = ComputeHash(result, value);
            var difficulty = CalculateDifficulty(hash);

            return difficulty;
        }

        private static byte[] ComputeHash(ReadOnlySpan<byte> result, ReadOnlySpan<byte> value)
        {
            Span<byte> buffer = stackalloc byte[64];

            byte[] hash;
            {
                BytesOperations.Copy(result, buffer, result.Length);
                BytesOperations.Copy(value, buffer[result.Length..], value.Length);
                hash = Sha2_256.ComputeHash(buffer);
            }

            return hash;
        }

        private static uint CalculateDifficulty(ReadOnlySpan<byte> hash)
        {
            uint count = 0;

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (((hash[i] << j) & 0x80) == 0)
                    {
                        count++;
                    }
                    else
                    {
                        goto End;
                    }
                }
            }

        End:
            return count;
        }
    }
}

public class MinerException : Exception
{
    public MinerException()
        : base()
    {
    }

    public MinerException(string message)
        : base(message)
    {
    }

    public MinerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
