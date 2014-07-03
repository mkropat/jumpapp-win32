using System.Diagnostics;

namespace JumpApp
{
    public class AppLauncher
    {
        private readonly string _name;
        private readonly string _extPath;

        public AppLauncher(string name, string exePath)
        {
            _name = name;
            _extPath = exePath;
        }

        public Process[] GetProcesses()
        {
            return Process.GetProcessesByName(_name);
        }

        public void StartProcess()
        {
            Process.Start(_extPath);
        }
    }
}