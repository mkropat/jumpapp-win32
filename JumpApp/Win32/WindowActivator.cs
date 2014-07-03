using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace JumpApp.Win32
{
    public class WindowActivator
    {
        public static void Activate(IntPtr hWnd)
        {
            ShowWindow(hWnd, ShowWindowEnum.Restore);

            if (!SetForegroundWindow(hWnd))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        // ShowWindow nCmdShow: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633548
        private enum ShowWindowEnum
        {
            Hide              = 0,
            ShowNormal        = 1,
            ShowMinimized     = 2,
            ShowMaximized     = 3,
            ShowNoActivate    = 4,
            Show              = 5,
            Minimize          = 6,
            ShowMinNoActivate = 7,
            ShowNa            = 8,
            Restore           = 9,
            ShowDefault       = 10,
            ForceMinimized    = 11
        };

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}