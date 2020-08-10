using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Helpers;

namespace Omnius.Core.Cryptography
{
    /// <summary>
    /// Omniusのネイティブマイナーライブラリ（Rust製）
    /// 現在はシンプルにSHA-2を計算するのみ
    /// </summary>
    public static class OmniMiner
    {
        public static async ValueTask<OmniHashcash> Create(ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> key, OmniHashcashAlgorithmType hashcashAlgorithmType, int limit, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (!EnumHelper.IsValid(hashcashAlgorithmType))
            {
                throw new ArgumentException(nameof(OmniHashcashAlgorithmType));
            }

            return await Task.Run(() =>
            {
                if (hashcashAlgorithmType == OmniHashcashAlgorithmType.Sha2_256)
                {
                    var target = Hmac_Sha2_256.ComputeHash(sequence, key.Span);
                    var hashcashKey = MinerHelper.Compute_Simple_Sha2_256(target, limit, timeout, cancellationToken);

                    return new OmniHashcash(OmniHashcashAlgorithmType.Sha2_256, hashcashKey);
                }

                throw new NotSupportedException(nameof(hashcashAlgorithmType));
            }, cancellationToken);
        }

        public static uint Verify(OmniHashcash hashcash, ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> key)
        {
            if (hashcash is null)
            {
                throw new ArgumentNullException(nameof(hashcash));
            }

            if (hashcash.AlgorithmType == OmniHashcashAlgorithmType.Sha2_256)
            {
                var target = Hmac_Sha2_256.ComputeHash(sequence, key.Span);

                return MinerHelper.Verify_Simple_Sha2_256(hashcash.Key.Span, target);
            }

            return 0;
        }

        /// <summary>
        /// OmniHashのマイナーを表すクラス
        /// </summary>
        private static class MinerHelper
        {
            private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
            private static readonly string _path;

            /// <summary>
            /// OmniHashのマイナーを起動する
            /// </summary>
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
                        // システムがOmniHashのマイナーに対応していない
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
                        // システムがOmniHashのマイナーに対応していない
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    // システムがOmniHashのマイナーに対応していない
                    throw new NotSupportedException();
                }
            }

            /// <summary>
            /// シンプルにSha2-256を計算する
            /// </summary>
            /// <param name="value">値（byte配列）</param>
            /// <param name="limit">制限bit</param>
            /// <param name="timeout">タイムアウトになる時間</param>
            /// <param name="cancellationToken">キャンセルトークン</param>
            /// <returns></returns>
            public static byte[] Compute_Simple_Sha2_256(ReadOnlySpan<byte> value, int limit, TimeSpan timeout, CancellationToken cancellationToken)
            {
                // valueがnullならArgumentNullExceptionを投げる
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                // valueが最大値を超えていたらArgumentOutOfRangeExceptionを投げる
                if (value.Length != 32)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                // limitが0の場合、256bitである。
                if (limit < 0)
                {
                    limit = 256;
                }

                // タイマーを起動する
                var sw = Stopwatch.StartNew();

                // プロセスを起動する
                var info = new ProcessStartInfo(_path)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    Arguments = $"--type=simple_sha2_256 --value={Convert.ToBase64String(value)}"
                };

                int difficulty = 0;
                byte[] key = new byte[32];

                using (var process = Process.Start(info))
                {
                    process.PriorityClass = ProcessPriorityClass.Idle;

                    var readTask = Task.Run(() =>
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
                    });

                    var writeTask = Task.Run(() =>
                    {
                        try
                        {
                            using (var w = new StreamWriter(process.StandardInput.BaseStream, new UTF8Encoding(false)))
                            {
                                w.NewLine = "\n";

                                while (!process.HasExited && !cancellationToken.WaitHandle.WaitOne(1000) && sw.Elapsed < timeout && difficulty < limit)
                                {
                                    w.WriteLine("a");
                                    w.Flush();
                                }

                                w.WriteLine("e");
                                w.Flush();
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.Debug(e);
                        }
                    });

                    Task.WaitAll(readTask, writeTask);
                }

                return key;
            }

            /// <summary>
            /// シンプルにSHA2-256を承認する
            /// </summary>
            /// <param name="key">キー</param>
            /// <param name="value">値</param>
            /// <returns></returns>
            public static uint Verify_Simple_Sha2_256(ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (key.Length != 32)
                {
                    throw new ArgumentOutOfRangeException(nameof(key));
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value.Length != 32)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

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
        public MinerException() : base() { }
        public MinerException(string message) : base(message) { }
        public MinerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
