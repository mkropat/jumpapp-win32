using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using JumpApp.Win32;
using Newtonsoft.Json;

namespace JumpApp
{
    public partial class Form1 : Form
    {
        private const string AppName = "JumpApp";

        public Form1()
        {
            InitializeComponent();
            components = new Container();
            CreateNotifyIcon();
        }

        private void CreateNotifyIcon()
        {
            new NotifyIcon(components)
            {
                ContextMenu = new ContextMenu(CreateTrayMenu()),
                Icon = new Icon("appicon.ico"),
                Text = AppName,
                Visible = true,
            };
        }

        private MenuItem[] CreateTrayMenu()
        {
            var exitItem = new MenuItem
            {
                Index = 0,
                Text = "E&xit",
            };
            exitItem.Click += (sender, args) => Close();

            return new[] { exitItem };
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // WM_HOTKEY: http://msdn.microsoft.com/en-us/library/windows/desktop/ms646279
            const int WM_HOTKEY = 0x312;
            if (m.Msg == WM_HOTKEY)
                _hotKeyRegistry.HandleHotKeyEvent(m.WParam, m.LParam);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            _hotKeyRegistry = new SystemHotKeyRegistry(Handle);

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var settingsFile = Path.Combine(localAppData, AppName, "settings.json");
            var settings = Settings.Load(settingsFile);

            foreach (var launcher in settings.Launchers)
            {
                var appLauncher = new AppLauncher(launcher.Shortcut.Match, launcher.Shortcut.Target);
                var modifiers = launcher.HotKey.Modifiers.Aggregate(System.Windows.Input.ModifierKeys.None,
                    (combined, modifier) => combined | modifier);
                _hotKeyRegistry.RegisterHotKey(modifiers, launcher.HotKey.Key, HotKeyHandler, appLauncher);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _hotKeyRegistry.Dispose();
        }

        static void HotKeyHandler(ModifierKeys modifier, Key key, object data)
        {
            JumpApp.Jump(data as AppLauncher);
        }

        private SystemHotKeyRegistry _hotKeyRegistry;
    }
}
