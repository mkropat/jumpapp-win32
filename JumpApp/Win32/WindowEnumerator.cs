using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace JumpApp.Win32
{
    public class Window
    {
        public IntPtr Handle { get; set; }
        public bool IsForeground { get; set; }
        public bool IsMinimized { get; set; }
    }

    public class WindowEnumerator
    {
        public static IEnumerable<Window> GetWindows(uint processId)
        {
            var fgHandle = GetForegroundWindow();
            return GetWindowHandles(processId)
                .Select(handle => new Window
                {
                    Handle = handle,
                    IsForeground = handle == fgHandle,
                    IsMinimized = IsIconic(handle)
                });
        }

        private static IntPtr[] GetWindowHandles(uint processId)
        {
            var windows = new List<IntPtr>();

            EnumWindows(delegate(IntPtr hwnd, IntPtr lParam)
            {
                uint windowProcessId;
                GetWindowThreadProcessId(hwnd, out windowProcessId);
                var hasOwner = GetWindow(hwnd, GetWindow_Cmd.GW_OWNER) != IntPtr.Zero;
                var isVisible = IsWindowVisible(hwnd);

                if (windowProcessId == processId && !hasOwner && isVisible)
                    windows.Add(hwnd);

                return true;
            }, IntPtr.Zero);

            return windows.ToArray();
        }

        private delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}
