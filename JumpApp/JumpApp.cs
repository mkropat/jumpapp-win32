using System.Collections.Generic;
using System.Linq;
using JumpApp.Win32;

namespace JumpApp
{
    public static class JumpApp
    {
        public static void Jump(AppLauncher launcher)
        {
            if (launcher == null)
                return;

            var procs = launcher.GetProcesses();
            if (procs.Length > 0)
                Focus(procs.Select(p => (uint) p.Id));
            else
                launcher.StartProcess();
        }

        private static void Focus(IEnumerable<uint> processIds)
        {
            var nextWindow = GetNextWindow(
                from id in processIds
                from window in WindowEnumerator.GetWindows(id)
                let windowId = window.Handle.ToInt64()
                orderby window.IsMinimized, windowId
                select window);

            if (nextWindow != null)
                WindowActivator.Activate(nextWindow.Handle);   
        }

        private static Window GetNextWindow(IEnumerable<Window> windows)
        {
            var asArray = windows.ToList();
            
            if (asArray.Count == 0)
                return null;

            var fgIndex = asArray.FindIndex(w => w.IsForeground);
            return fgIndex == -1
                ? asArray[0]
                : asArray[(fgIndex + 1) % asArray.Count];
        }
    }
}