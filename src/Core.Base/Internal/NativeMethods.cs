using System.Runtime.InteropServices;
using Core.Base;

namespace Core.Base.Internal;

internal unsafe sealed partial class NativeMethods
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    static NativeMethods()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    NativeLibraryManager = new NativeLibraryManager("native/omnius-core.x64.dll");
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
                    NativeLibraryManager = new NativeLibraryManager("native/omnius-core.x64.so");
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
            _logger.Error(e, "Failed to load native library");
        }
    }

    internal static NativeLibraryManager? NativeLibraryManager { get; private set; }
}
