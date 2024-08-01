using System.Runtime.InteropServices;
using Omnius.Core.Base;

namespace Omnius.Core.Base.Internal;

internal unsafe sealed partial class NativeMethods
{
    static NativeMethods()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                NativeLibraryManager = new NativeLibraryManager("native/omnius-core.x64.dll");
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                NativeLibraryManager = new NativeLibraryManager("native/omnius-core.x64.so");
            }
        }
    }

    internal static NativeLibraryManager? NativeLibraryManager { get; private set; }
}
