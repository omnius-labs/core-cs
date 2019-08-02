using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Omnix.Base.Internal
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
                        _nativeLibraryManager = new NativeLibraryManager("omnix-base.x64.dll");
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
                        _nativeLibraryManager = new NativeLibraryManager("omnix-base.x64.so");
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
