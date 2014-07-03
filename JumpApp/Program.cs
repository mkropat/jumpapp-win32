using System;
using System.Windows.Forms;

namespace JumpApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            var form = new Form1();
            form.Show();
            form.Visible = false;
            form.Closed += (sender, args) => Application.Exit();

            Application.Run();
        }
    }
}
