#region .NET Base Class Namespace Imports
using System;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
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
        public static int OpenOneCommunicator(int id)
        {
            if (id < 0 || id >= _comm.Length) return 0;
            _comm[id].Open(_server, _port);
            if (!_comm[id].IsOpen)
            {
                ServerUnavailable();
                return 0;
            }
            return 1;
        }
        public static void CloseOneCommunicator(int id)
        {
            if (id < 0 || id >= _comm.Length) return;
            if (_comm[id].IsOpen)
            {
                _comm[id].Close();
            }
        }
        public static bool SendFile(string strRemoteFile, Stream sSource)
        {
            Trace.WriteLine("Send file start");
            Communicator new_comm = new Communicator();
            new_comm.Open(_server, _port);
            Trace.WriteLine("1. IsOpen = " + new_comm.IsOpen);
            if (!new_comm.IsOpen)
            {
                ServerUnavailable();
                new_comm = null;
                GC.Collect();
                return false;
            }
            bool res = new_comm.SendFile(strRemoteFile, sSource);
            if (new_comm.IsOpen)
            {
                new_comm.Close();
            }
            Trace.WriteLine("2. IsOpen = " + new_comm.IsOpen);
            new_comm = null;
            GC.Collect();
            return res;
        }
        public static void TestConnection()
        {
            for (var i = 0; i < _comm.Length; i++)
            {
                if (_comm[i].IsOpen)
                {
                    _comm[i].TestConnection();
                }
            }
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
