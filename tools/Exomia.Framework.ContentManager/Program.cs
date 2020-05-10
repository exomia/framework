using System;
using System.Windows.Forms;

namespace Exomia.Framework.ContentManager
{
    /// <summary>
    ///     A program.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
