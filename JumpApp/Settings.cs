using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;

namespace JumpApp
{
    class Settings
    {
        public Launcher[] Launchers { get; set; }

        public static Settings Load(string path)
        {
            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
        }
    }

    class Launcher
    {
        public HotKey HotKey { get; set; }
        public Shortcut Shortcut { get; set; }
    }

    class HotKey
    {
        public Key Key { get; set; }
        public ModifierKeys[] Modifiers { get; set; }
    }

    class Shortcut
    {
        public string Match { get; set; }
        public string Target { get; set; }
        public string StartIn { get; set; }
    }
}