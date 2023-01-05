#region .NET Base Class Namespace Imports
using System;
using System.Windows.Forms;
using System.Globalization;
#endregion

#region CompiledTechnologies Assemblies
using CompiledTechnologies.Transport;
#endregion

namespace CompiledTechnologies.Network
{
    public static class Transporter
    {
        #region **************************** Private Properties ****************************
        private static Communicator[] _comm;
        private static readonly string _device;
        private static string _server;
        private static int _port;
        #endregion

        #region **************************** Public Properties *****************************
        public static Communicator[] Comm
        {
            get { return _comm; }
            set { _comm = value; }
        }
        public static string Device
        {
            get { return _device; }
        }
        public static string Server
        {
            get { return _server; }
            set { _server = value; }
        }
        public static int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        #endregion

        #region ****************************** Constructor *********************************
        static Transporter()
        {
            _comm = new Communicator[2];
            _comm[0] = new Communicator();
            _comm[1] = new Communicator();
            _device = Environment.MachineName.ToUpper(CultureInfo.InvariantCulture);
            _server = Environment.MachineName.ToUpper(CultureInfo.InvariantCulture);
            _port = 7030;
        }
        #endregion

        #region ********************** Public Communicator Methods *************************
        public static void OpenTransport(string[] args)
        {
            GetCommandLine(args);
            OpenCommunicators();
        }
        public static void CloseTransport()
        {
            CloseCommunicators();
        }
        #endregion

        #region ********************* Private Communicator Methods *************************
        private static void GetCommandLine(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[2];
                args[0] = _device;
                args[1] = _port.ToString(CultureInfo.InvariantCulture);
            }
            if (args.Length != 2)
            {
                MessageBox.Show("Argument usage [Server] [Port]", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _server = args[0].ToUpper(CultureInfo.InvariantCulture);
            //_port = Convert.ToInt16(args[1], CultureInfo.InvariantCulture);
        }
        private static void OpenCommunicators()
        {
            for (var i = 0; i < _comm.Length; i++)
            {
                _comm[i].Open(_server, _port);
                if (!_comm[i].IsOpen)
                {
                    ServerUnavailable();
                    break;
                }
            }
        }
        private static void CloseCommunicators()
        {
            for (var i = 0; i < _comm.Length; i++)
            {
                if (_comm[i].IsOpen)
                {
                    _comm[i].Close();
                }
            }
        }
        #endregion

        #region *************************** Private Methods ********************************
        private static void ServerUnavailable()
        {
            MessageBox.Show("The connection to " + _server + " " + _port + " is unavailable!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }
}
