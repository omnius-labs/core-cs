using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Omnix.Base
{
    public sealed class NativeLibraryManager : DisposableBase
    {
        private IntPtr _moduleHandle = IntPtr.Zero;

        private static class NativeMethods
        {
            public static class Windows
            {
                [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern IntPtr LoadLibrary(string lpFileName);

                [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
                public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

                [DllImport("kernel32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.U1)]
                public static extern bool FreeLibrary(IntPtr hModule);
            }

            public static class Linux
            {
                private const int RTLD_NOW = 2;

                [DllImport("libdl.so")]
                private static extern IntPtr dlopen(string fileName, int flags);

                [DllImport("libdl.so")]
                private static extern IntPtr dlsym(IntPtr handle, string symbol);

                [DllImport("libdl.so")]
                private static extern int dlclose(IntPtr handle);

                [DllImport("libdl.so")]
                private static extern IntPtr dlerror();

                public static IntPtr LoadLibrary(string fileName)
                {
                    return dlopen(fileName, RTLD_NOW);
                }

                public static void FreeLibrary(IntPtr handle)
                {
                    dlclose(handle);
                }

                public static IntPtr GetProcAddress(IntPtr dllHandle, string name)
                {
                    // clear previous errors if any
                    dlerror();

                    var res = dlsym(dllHandle, name);

                    var errPtr = dlerror();
                    if (errPtr != IntPtr.Zero)
                    {
                        throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errPtr));
                    }

                    return res;
                }
            }
        }

        public NativeLibraryManager(string path)
        {
            //string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            string fullPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), path);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _moduleHandle = NativeMethods.Windows.LoadLibrary(fullPath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _moduleHandle = NativeMethods.Linux.LoadLibrary(fullPath);
            }
        }

        public T GetMethod<T>(string method)
            where T : Delegate
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var methodHandle = NativeMethods.Windows.GetProcAddress(_moduleHandle, method);
                return Marshal.GetDelegateForFunctionPointer<T>(methodHandle);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var methodHandle = NativeMethods.Linux.GetProcAddress(_moduleHandle, method);
                return Marshal.GetDelegateForFunctionPointer<T>(methodHandle);
            }

            throw new NotSupportedException();
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {

            }

            if (_moduleHandle != IntPtr.Zero)
            {
                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        NativeMethods.Windows.FreeLibrary(_moduleHandle);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        NativeMethods.Linux.FreeLibrary(_moduleHandle);
                    }
                }
                catch (Exception)
                {

                }

                _moduleHandle = IntPtr.Zero;
            }
        }
    }
}
