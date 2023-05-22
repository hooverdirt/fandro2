using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using libfandro2.lib.Finding;

namespace Fandro2
{
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0) {
                FindOptions n = new FindOptions(args);
                Application.Run(new mainForm(n));

            } else {
                Application.Run(new mainForm());
            }
        }
    }
}
