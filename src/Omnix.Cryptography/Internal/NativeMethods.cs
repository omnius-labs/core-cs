using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Omnix.Base;
using System.Runtime.Intrinsics.X86;

namespace Omnix.Internal
{
    internal unsafe sealed partial class NativeMethods
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly NativeLibraryManager? _nativeLibraryManager;

        internal static NativeLibraryManager? NativeLibraryManager => _nativeLibraryManager;

        static NativeMethods()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                    {
                        if (Avx2.IsSupported)
                        {
                            _nativeLibraryManager = new NativeLibraryManager("omnix-algorithms.x64-avx2.dll");
                        }
                        else
                        {
                            _nativeLibraryManager = new NativeLibraryManager("omnix-algorithms.x64.dll");
                        }
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
                        _nativeLibraryManager = new NativeLibraryManager("omnix-algorithms.x64.so");
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
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}
