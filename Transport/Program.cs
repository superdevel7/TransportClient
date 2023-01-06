using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


#region CompiledTechnologies Assemblies
using CompiledTechnologies.Network;
#endregion


namespace Transport
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string [] args = new string[2];
            args[0] = "192.168.1.123";
            args[1] = "7030";
            Transporter.OpenTransport(args);
            Transporter.TestConnection();
            Transporter.CloseTransport();
            Application.Run(new Form1());
        }
    }
}
