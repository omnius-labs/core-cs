using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Omnix.Base
{
    public sealed class NativeLibraryManager : DisposableBase
    {
        private IntPtr _moduleHandle = IntPtr.Zero;

        public NativeLibraryManager(string path)
        {
            //string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            string fullPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!, path);
            NativeLibrary.TryLoad(fullPath, out _moduleHandle);
        }

        public T GetMethod<T>(string method)
            where T : Delegate
        {
            var methodHandle = NativeLibrary.GetExport(_moduleHandle, method);
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
}
