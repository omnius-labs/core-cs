using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Serialization;
using Omnix.Serialization.Extensions;

namespace Omnix.Cryptography
{
    public static class OmniMiner
    {
        public static async ValueTask<OmniHashcash> Create(ReadOnlySequence<byte> sequence, OmniHashcashAlgorithmType hashcashAlgorithmType, int limit, TimeSpan timeout, CancellationToken token)
        {
            if (!EnumHelper.IsValid(hashcashAlgorithmType)) throw new ArgumentException(nameof(OmniHashcashAlgorithmType));

            return await Task.Run(() =>
            {
                if (hashcashAlgorithmType == OmniHashcashAlgorithmType.Sha2_256)
                {
                    var key = MinerHelper.Compute_OmniHashcash_Sha2_256(Sha2_256.ComputeHash(sequence), limit, timeout, token);

                    return new OmniHashcash(OmniHashcashAlgorithmType.Sha2_256, key);
                }

                throw new NotSupportedException(nameof(hashcashAlgorithmType));
            }, token);
        }

        public static int Verify(OmniHashcash hashcash, ReadOnlySequence<byte> sequence)
        {
            if (hashcash is null) throw new ArgumentNullException(nameof(hashcash));

            if (hashcash.AlgorithmType == OmniHashcashAlgorithmType.Sha2_256)
            {
                return MinerHelper.Verify_OmniHashcash_Sha2_256(hashcash.Key.Span, Sha2_256.ComputeHash(sequence));
            }

            return 0;
        }

        private static class MinerHelper
        {
            private static string _path;

            static MinerHelper()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                    {
                        _path = "Assemblies/Omnix.Cryptography.HashcashComputer.win-x64.exe";
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
                        _path = "Assemblies/Omnix.Cryptography.HashcashComputer.linux-x64";
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

            public static byte[] Compute_OmniHashcash_Sha2_256(ReadOnlySpan<byte> value, int limit, TimeSpan timeout, CancellationToken token)
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Length != 32) throw new ArgumentOutOfRangeException(nameof(value));

                if (limit < 0) limit = 256;

                var info = new ProcessStartInfo(_path);
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;

                var base16 = new Base16(ConvertStringCase.Lower);

                info.Arguments = $"compute --type=OmniHashcash_Sha2_256 --limit={limit} --timeout={timeout.TotalSeconds} --value={base16.BytesToString(value)} --bind={Process.GetCurrentProcess().Id}";

                using (var process = Process.Start(info))
                using (token.Register(() => process.Kill()))
                {
                    process.PriorityClass = ProcessPriorityClass.Idle;

                    try
                    {
                        string result = process.StandardOutput.ReadLine();

                        process.WaitForExit();

                        token.ThrowIfCancellationRequested();

                        if (process.ExitCode != 0) throw new MinerException(result);

                        return base16.StringToBytes(result);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (MinerException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        throw new MinerException(e.Message, e);
                    }
                }
            }

            public static int Verify_OmniHashcash_Sha2_256(ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                if (key.Length != 32) throw new ArgumentOutOfRangeException(nameof(key));
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Length != 32) throw new ArgumentOutOfRangeException(nameof(value));

                try
                {
                    Span<byte> buffer = stackalloc byte[64];

                    byte[] result;
                    {
                        BytesOperations.Copy(key, buffer, key.Length);
                        BytesOperations.Copy(value, buffer.Slice(key.Length), value.Length);
                        result = Sha2_256.ComputeHash(buffer);
                    }

                    int count = 0;

                    for (int i = 0; i < 32; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (((result[i] << j) & 0x80) == 0) count++;
                            else goto End;
                        }
                    }
                End:

                    return count;
                }
                catch (Exception)
                {
                    return 0;
                }
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
