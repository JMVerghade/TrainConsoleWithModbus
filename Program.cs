using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cs_Modbus_Maitre
{
    static class Program
    {
        /// <summary>
        /// Point d'entr�e principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
