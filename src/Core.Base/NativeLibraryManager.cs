using System.Runtime.InteropServices;

namespace Omnius.Core.Base;

public sealed class NativeLibraryManager : DisposableBase
{
    private IntPtr _moduleHandle = IntPtr.Zero;

    public NativeLibraryManager(string path)
    {
        var basePath = System.AppContext.BaseDirectory;
        basePath ??= Directory.GetCurrentDirectory();

        string fullPath = Path.Combine(basePath, path);
        if (!NativeLibrary.TryLoad(fullPath, out _moduleHandle))
        {
            throw new DllNotFoundException(fullPath);
        }
    }

    public T GetMethod<T>(string method)
        where T : Delegate
    {
        var methodHandle = NativeLibrary.GetExport(_moduleHandle, method);
        if (methodHandle == IntPtr.Zero) throw new NotSupportedException();

        return Marshal.GetDelegateForFunctionPointer<T>(methodHandle);
    }

    protected override void OnDispose(bool disposing)
    {
        if (_moduleHandle != IntPtr.Zero)
        {
            try
            {
                NativeLibrary.Free(_moduleHandle);
            }
            catch (Exception)
            {
            }

            _moduleHandle = IntPtr.Zero;
        }
    }
}
