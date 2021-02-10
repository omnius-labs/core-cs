using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Helpers;

namespace Omnius.Core.Cryptography
{
    public static class OmniMiner
    {
        public static async ValueTask<OmniHashcash> Create(ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> key, OmniHashcashAlgorithmType hashcashAlgorithmType, int limit, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (!EnumHelper.IsValid(hashcashAlgorithmType)) throw new ArgumentException(nameof(OmniHashcashAlgorithmType));

            await Task.Delay(1, cancellationToken).ConfigureAwait(false);

            if (hashcashAlgorithmType == OmniHashcashAlgorithmType.Sha2_256)
            {
                var target = Hmac_Sha2_256.ComputeHash(sequence, key.Span);
                var hashcashKey = MinerHelper.Compute_Simple_Sha2_256(target, limit, timeout, cancellationToken);

                return new OmniHashcash(OmniHashcashAlgorithmType.Sha2_256, hashcashKey);
            }

            throw new NotSupportedException(nameof(hashcashAlgorithmType));
        }

        public static uint Verify(OmniHashcash hashcash, ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> key)
        {
            if (hashcash is null) throw new ArgumentNullException(nameof(hashcash));

            if (hashcash.AlgorithmType == OmniHashcashAlgorithmType.Sha2_256)
            {
                var target = Hmac_Sha2_256.ComputeHash(sequence, key.Span);

                return MinerHelper.Verify_Simple_Sha2_256(hashcash.Key.Span, target);
            }

            return 0;
        }

        private static class MinerHelper
        {
            private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
            private static readonly string _path;

            static MinerHelper()
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

            public static byte[] Compute_Simple_Sha2_256(ReadOnlySpan<byte> value, int limit, TimeSpan timeout, CancellationToken cancellationToken)
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Length != 32) throw new ArgumentOutOfRangeException(nameof(value));

                if (limit < 0)
                {
                    limit = 256;
                }

                var sw = Stopwatch.StartNew();

                var info = new ProcessStartInfo(_path)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    Arguments = $"--type=simple_sha2_256 --value={Convert.ToBase64String(value)}",
                };

                int difficulty = 0;
                byte[] key = new byte[32];

                using (var process = Process.Start(info))
                {
                    if (process is null) throw new Exception("Failed to Process.Start()");

                    process.PriorityClass = ProcessPriorityClass.Idle;

                    var readTask = Task.Run(
                        () =>
                        {
                            try
                            {
                                while (!process.HasExited)
                                {
                                    var line = process.StandardOutput.ReadLine();
                                    if (line is null) return;

                                    var result = line.Split(" ");
                                    difficulty = int.Parse(result[0]);
                                    key = Convert.FromBase64String(result[1]);
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.Debug(e);
                            }
                        }, cancellationToken);

                    var writeTask = Task.Run(
                        () =>
                        {
                            try
                            {
                                using var w = new StreamWriter(process.StandardInput.BaseStream, new UTF8Encoding(false));
                                w.NewLine = "\n";

                                while (!process.HasExited && !cancellationToken.WaitHandle.WaitOne(1000) && sw.Elapsed < timeout && difficulty < limit)
                                {
                                    w.WriteLine("a");
                                    w.Flush();
                                }

                                w.WriteLine("e");
                                w.Flush();
                            }
                            catch (Exception e)
                            {
                                _logger.Debug(e);
                            }
                        }, cancellationToken);

                    Task.WaitAll(readTask, writeTask);
                }

                return key;
            }

            public static uint Verify_Simple_Sha2_256(ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                if (key.Length != 32) throw new ArgumentOutOfRangeException(nameof(key));
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Length != 32) throw new ArgumentOutOfRangeException(nameof(value));

                Span<byte> buffer = stackalloc byte[64];

                byte[] hash;
                {
                    BytesOperations.Copy(key, buffer, key.Length);
                    BytesOperations.Copy(value, buffer.Slice(key.Length), value.Length);
                    hash = Sha2_256.ComputeHash(buffer);
                }

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
}
