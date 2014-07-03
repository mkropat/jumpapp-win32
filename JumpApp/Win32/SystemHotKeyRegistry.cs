using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace JumpApp.Win32
{
    public class SystemHotKeyRegistry : IDisposable
    {
        public delegate void HotKeyFired(ModifierKeys modifier, Key key, object data);
        
        private readonly IntPtr _windowHandle;
        private readonly Dictionary<short, HandlerInfo> _callbacks = new Dictionary<short, HandlerInfo>();
        private short _nextId = 1; // id 0 would be NULL, which has special meaning to RegisterHotKey, so skip it

        public SystemHotKeyRegistry(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
        }

        public void RegisterHotKey(ModifierKeys modifier, Key key, HotKeyFired handler, object data)
        {
            var id = _nextId++;
            _callbacks.Add(id, new HandlerInfo { Handler = handler, Data = data });

            var vkey = KeyInterop.VirtualKeyFromKey(key);

            if (! RegisterHotKey(_windowHandle, id, (uint) modifier, (uint) vkey))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public void HandleHotKeyEvent(IntPtr wParam, IntPtr lParam)
        {
            var id = (short) wParam;
            
            var modifierMask = (int) lParam >> 16;
            var modifier = (ModifierKeys) modifierMask;
            
            var vkey = (int) lParam & 0xffff;
            var key = KeyInterop.KeyFromVirtualKey(vkey);

            HandleHotKeyPress(id, modifier, key);
        }

        private void HandleHotKeyPress(short id, ModifierKeys modifier, Key key)
        {
            if (!_callbacks.ContainsKey(id))
                return;

            var handlerInfo = _callbacks[id];
            handlerInfo.Handler.Invoke(modifier, key, handlerInfo.Data);
        }
    
        public void Dispose()
        {
            foreach (var id in _callbacks.Keys)
                if (! UnregisterHotKey(_windowHandle, id))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        class HandlerInfo
        {
            public HotKeyFired Handler { get; set; }
            public object Data { get; set; }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}