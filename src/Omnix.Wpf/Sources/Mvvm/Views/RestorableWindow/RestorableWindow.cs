using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Omnius.Wpf
{
    // http://grabacr.net/archives/1585
    public class RestorableWindow : Window
    {
        public WindowSettings WindowSettings
        {
            get { return (WindowSettings)GetValue(WindowSettingsProperty); }
            set { SetValue(WindowSettingsProperty, value); }
        }

        public static readonly DependencyProperty WindowSettingsProperty =
            DependencyProperty.Register("WindowSettings", typeof(WindowSettings), typeof(Window), new PropertyMetadata(null));

        private WindowSettings GetWindowSettings()
        {
            try
            {
                Native.WINDOWPLACEMENT placement;
                var hwnd = new WindowInteropHelper(this).Handle;
                Native.Methods.GetWindowPlacement(hwnd, out placement);

                return new WindowSettings() { Placement = placement };
            }
            catch (Exception)
            {

            }

            return null;
        }

        private void SetWindowSettings(WindowSettings value)
        {
            if (value == null || !value.Placement.HasValue) return;

            var hwnd = new WindowInteropHelper(this).Handle;
            var placement = value.Placement.Value;
            placement.length = Marshal.SizeOf(typeof(Native.WINDOWPLACEMENT));
            placement.flags = 0;
            placement.showCmd = (placement.showCmd == Native.SW.SHOWMINIMIZED) ? Native.SW.SHOWNORMAL : placement.showCmd;

            Native.Methods.SetWindowPlacement(hwnd, ref placement);

            this.UpdateLayout();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (this.WindowSettings == null)
            {
                this.WindowSettings = this.GetWindowSettings();
            }

            this.SetWindowSettings(this.WindowSettings);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                this.WindowSettings = this.GetWindowSettings();
            }

            base.OnClosing(e);
        }

        public class Native
        {
            public class Methods
            {
                [DllImport("user32.dll")]
                public static extern bool SetWindowPlacement(
                    IntPtr hWnd,
                    [In] ref WINDOWPLACEMENT lpwndpl);

                [DllImport("user32.dll")]
                public static extern bool GetWindowPlacement(
                    IntPtr hWnd,
                    out WINDOWPLACEMENT lpwndpl);
            }

            [Serializable]
            [StructLayout(LayoutKind.Sequential)]
            public struct WINDOWPLACEMENT
            {
                public int length;
                public int flags;
                public SW showCmd;
                public POINT minPosition;
                public POINT maxPosition;
                public RECT normalPosition;
            }

            [Serializable]
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }
            }

            [Serializable]
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;

                public RECT(int left, int top, int right, int bottom)
                {
                    this.Left = left;
                    this.Top = top;
                    this.Right = right;
                    this.Bottom = bottom;
                }
            }

            public enum SW
            {
                HIDE = 0,
                SHOWNORMAL = 1,
                SHOWMINIMIZED = 2,
                SHOWMAXIMIZED = 3,
                SHOWNOACTIVATE = 4,
                SHOW = 5,
                MINIMIZE = 6,
                SHOWMINNOACTIVE = 7,
                SHOWNA = 8,
                RESTORE = 9,
                SHOWDEFAULT = 10,
            }
        }
    }
}
