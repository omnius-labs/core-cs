using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Omnius.Wpf
{
    public static class ProcessExtensions
    {
        [DllImport("ntdll.dll")]
        private static extern uint NtSetInformationProcess(IntPtr processHandle, uint processInformationClass, ref uint processInformation, uint processInformationLength);

        private const uint ProcessInformationMemoryPriority = 0x27;

        public static void SetMemoryPriority(this Process process, int priority)
        {
            uint memoryPriority = (uint)priority;
            ProcessExtensions.NtSetInformationProcess(process.Handle, ProcessExtensions.ProcessInformationMemoryPriority, ref memoryPriority, sizeof(uint));
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        public static bool IsActivated(this Process process)
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            int procId = process.Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }
    }
}
