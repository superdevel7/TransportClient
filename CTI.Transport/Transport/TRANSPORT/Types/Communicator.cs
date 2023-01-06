#region .NET Base Class Namespace Imports
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
#endregion

namespace CompiledTechnologies.Transport
{
    public class Communicator : IDisposable
    {
        #region ***************************** Public Events *******************************
        public event CommConnEventHandler ConnectionEvent;
        #endregion

        #region ***************************** Private Events ******************************
        private AutoResetEvent WaitActionComplete;
        #endregion

        #region ****************************** Constants **********************************
        public const int CMD_GET_FILE = 0;
        public const int CMD_SEND_FILE = 1;
        public const int CMD_IS_EXIT_FILE = 100;
        public const int CMD_CHK_DIR = 101;
        public const int CMD_DATA_SIZE = 10;
        public const int CMD_DATA = 11;
        public const int CMD_CONN_TEST = 80;
        public const int BUF_SIZE = (8 << 10);
        public static readonly char[] password = { 'P', 'A', 'S', 'S', 'W', 'O', 'D' };
        #endregion

        #region *************************** Private Properties ****************************
        private string theHost;
        private int thePort;
        private readonly TcpClient theTcpClient;
        private Stream theStream;
        private ValueInputStream theInput;
        private ValueOutputStream theOutput;
        private int theReceiveTimeout;
        private int theSendTimeout;
        private bool connected;
        private readonly IAsyncResult result;
        private CommunicatorState theState;
        private bool disposed;
        #endregion

        #region ************************** Private Constructors ***************************
        public Communicator()
        {
            WaitActionComplete = new AutoResetEvent(true);
            theHost = string.Empty;
            thePort = 0;
            theTcpClient = new TcpClient();
            theStream = null;
            theInput = null;
            theOutput = null;
            theReceiveTimeout = 0;
            theSendTimeout = 10000;
            connected = false;
            result = null;
            theState = CommunicatorState.Closed;
            disposed = false;
        }
        #endregion

        #region ***************************** Public Methods ******************************
        public virtual void Abort()
        {
            if (theState != CommunicatorState.Closed)
            {
                WaitActionComplete = new AutoResetEvent(true);
                theStream = null;
                theInput = null;
                theOutput = null;
                connected = false;
                theState = CommunicatorState.Closed;
                if (theTcpClient.Available != 0)
                {
                    theTcpClient.EndConnect(result);
                    theTcpClient.Close();
                }
            }
        }

        public virtual void Open(Stream s)
        {
            theStream = s;
            theInput = new ValueInputStream(theStream);
            theOutput = new ValueOutputStream(theStream);
        }

        public virtual void Open(Socket theSocket)
        {
            theStream = new NetworkStream(theSocket, true);
            theInput = new ValueInputStream(theStream);
            theOutput = new ValueOutputStream(theStream);
        }

        public virtual void Open(string host, int port)
        {
            try
            {
                theState = CommunicatorState.Busy;
                theHost = host;
                thePort = port;
                theTcpClient.SendTimeout = theSendTimeout;
                theTcpClient.ReceiveTimeout = theReceiveTimeout;
                if (!theTcpClient.Connected)
                {
                    theTcpClient.Connect(host, port);
                }
                if (!theTcpClient.Connected) { throw new Exception("Connection not available!"); }
            }
            catch
            {
                Abort();
                return;
            }
            if (theTcpClient.Connected)
            {
                connected = true;
                Open(theTcpClient.GetStream());
                theState = CommunicatorState.Open;
            }
            ConnectionEvent?.Invoke(this, null);
        }

        public virtual void Close()
        {
            Abort();
        }

        public MemoryStream GetStream(string strRemoteFile)
        {   
            Command sCommand;
            sCommand.nCommand = CMD_GET_FILE;
            sCommand.UNCPath = strRemoteFile;
            int nSize = Marshal.SizeOf(sCommand);
            byte[] arStruct = new byte[nSize];
            IntPtr pData = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(sCommand, pData, true);
            Marshal.Copy(pData, arStruct, 0, nSize);
            Marshal.FreeHGlobal(pData);
            theState = CommunicatorState.Busy;
            SendAction(arStruct, nSize);
            nSize = ReceiveAction();
            if (nSize > 0)
            {
                arStruct = new byte[nSize];
                ReceiveAction(arStruct, nSize);
            }
            MemoryStream memStream = new MemoryStream(arStruct, 0, nSize);
            theState = CommunicatorState.Open;
            return memStream;
        }
        public bool FileExists(string strRemoteFile)
        {
            Command sCommand;
            sCommand.nCommand = CMD_IS_EXIT_FILE;
            sCommand.UNCPath = strRemoteFile;
            int nSize = Marshal.SizeOf(sCommand);
            byte[] arStruct = new byte[nSize];
            IntPtr pData = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(sCommand, pData, true);
            Marshal.Copy(pData, arStruct, 0, nSize);
            Marshal.FreeHGlobal(pData);
            theState = CommunicatorState.Busy;
            SendAction(arStruct, nSize);
            nSize = ReceiveAction();
            if (nSize > 0)
            {
                arStruct = new byte[2];
                ReceiveAction(arStruct, nSize);
                if (arStruct[0] == 'Y')
                {
                    theState = CommunicatorState.Open;
                    return true;
                }
            }
            theState = CommunicatorState.Open;
            return false;
        }
        public bool CheckDirectoryExists(string strRemoteFile)
        {
            Command sCommand;
            sCommand.nCommand = CMD_CHK_DIR;
            sCommand.UNCPath = strRemoteFile;
            int nSize = Marshal.SizeOf(sCommand);
            byte[] arStruct = new byte[nSize];
            IntPtr pData = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(sCommand, pData, true);
            Marshal.Copy(pData, arStruct, 0, nSize);
            Marshal.FreeHGlobal(pData);
            theState = CommunicatorState.Busy;
            SendAction(arStruct, nSize);
            nSize = ReceiveAction();
            if (nSize > 0)
            {
                arStruct = new byte[2];
                ReceiveAction(arStruct, nSize);
                if (arStruct[0] == 'Y')
                {
                    theState = CommunicatorState.Open;
                    return true;
                }
            }
            theState = CommunicatorState.Open;
            return false;
        }
        public bool SendFile(string strRemoteFile, FileStream fsSource)
        {
            Command sCommand;
            sCommand.nCommand = CMD_SEND_FILE;
            sCommand.UNCPath = strRemoteFile;
            int nSize = Marshal.SizeOf(sCommand);
            byte[] arStruct = new byte[nSize];
            IntPtr pData = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(sCommand, pData, true);
            Marshal.Copy(pData, arStruct, 0, nSize);
            Marshal.FreeHGlobal(pData);
            theState = CommunicatorState.Busy;
            SendAction(arStruct, nSize);
            int bFileCreated = ReceiveAction();
            if (bFileCreated > 0)
            {
                fsSource.Seek(0, SeekOrigin.Begin);
                Int32 nFileSize = (Int32)fsSource.Length;
                byte[] pSize = BitConverter.GetBytes(nFileSize);
                SendAction(pSize, pSize.Length);
                byte[] pBuffer = new byte[BUF_SIZE];
                Int32 nBytesToSend = nFileSize;
                int offset = 0;
                while (nBytesToSend > 0)
                {
                    int nReadByte = fsSource.Read(pBuffer, 0, BUF_SIZE);
                    if (nReadByte == 0)
                    {
                        theState = CommunicatorState.Open;
                        return false;
                    }
                    //WaitActionComplete.WaitOne();
                    offset = encryptData(pBuffer, offset);
                    SendAction(pBuffer, nReadByte);
                    nBytesToSend -= nReadByte;
                    //ReceiveWaitAction();
                }
                theState = CommunicatorState.Open;
                return true;
            }
            theState = CommunicatorState.Open;
            return false;
        }
        public bool SendFile(string strRemoteFile, Stream sSource)
        {
            theState = CommunicatorState.Busy;
            Command sCommand;
            sCommand.nCommand = CMD_SEND_FILE;
            sCommand.UNCPath = strRemoteFile;
            int nSize = Marshal.SizeOf(sCommand);
            byte[] arStruct = new byte[nSize];
            IntPtr pData = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(sCommand, pData, true);
            Marshal.Copy(pData, arStruct, 0, nSize);
            Marshal.FreeHGlobal(pData);
            SendAction(arStruct, nSize);
            int bFileCreated = ReceiveAction();
            if (bFileCreated > 0)
            {
                sSource.Seek(0, SeekOrigin.Begin);
                Int32 nFileSize = (Int32)sSource.Length;
                byte[] pSize = BitConverter.GetBytes(nFileSize);
                SendAction(pSize, pSize.Length);
                byte[] pBuffer = new byte[BUF_SIZE];
                Int32 nBytesToSend = nFileSize;
                int offset = 0;
                WaitActionComplete.WaitOne();
                while (nBytesToSend > 0)
                {
                    int nReadByte = sSource.Read(pBuffer, 0, BUF_SIZE);
                    if (nReadByte == 0)
                    {
                        break;
                    }
                    offset = encryptData(pBuffer, offset);
                    SendAction(pBuffer, nReadByte);
                    nBytesToSend -= nReadByte;
                }
                ReceiveWaitAction();
                theState = CommunicatorState.Open;
                return true;
            }
            theState = CommunicatorState.Open;
            return false;
        }
        public void TestConnection()
        {
            theState = CommunicatorState.Busy;
            Command sCommand;
            sCommand.nCommand = CMD_CONN_TEST;
            sCommand.UNCPath = "TEST";
            int nSize = Marshal.SizeOf(sCommand);
            byte[] arStruct = new byte[nSize];
            IntPtr pData = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(sCommand, pData, true);
            Marshal.Copy(pData, arStruct, 0, nSize);
            Marshal.FreeHGlobal(pData);
            SendAction(arStruct, nSize);
            theState = CommunicatorState.Open;
        }
        #endregion

        #region **************************** Private Methods ******************************
        private int encryptData(byte[] data, int offset)
        {
            for (int i = 0; i < data.Length; i++)
            {
                // data[i] = (byte)(data[i] ^ password[i % password.Length]);
                data[i] = (byte)(data[i] ^ password[offset]);
                offset++;
                if (offset >= password.Length) offset = 0;
            }
            return offset;
        }
        private void SendAction(byte[] arData, int nCount)
        {
            try
            {
                if (theState == CommunicatorState.Closed)
                {
                    throw new InvalidOperationException("Attempt to send action through a closed communicator");
                }
                theOutput.Write(arData, 0, nCount);
            }
            catch
            {
                theState = CommunicatorState.Closed;
            }
        }

        private void ReceiveAction(byte[] arData, int nCount)
        {
            int nRead = 0;
            try
            {
                if (theState == CommunicatorState.Closed)
                {
                    throw new InvalidOperationException("Attempt to send action through a closed communicator");
                }
                while (nRead < nCount)
                {
                    nRead += theInput.Read(arData, nRead, nCount - nRead);
                }
            }
            catch
            {
                theState = CommunicatorState.Closed;
            }
        }

        private Int32 ReceiveAction()
        {
            try
            {
                if (theState == CommunicatorState.Closed)
                {
                    throw new InvalidOperationException("Attempt to send action through a closed communicator");
                }
                return theInput.ReadInt32();
            }
            catch
            {
                theState = CommunicatorState.Closed;
            }
            return 0;
        }
        private Int32 ReceiveWaitAction()
        {
            try
            {
                if (theState == CommunicatorState.Closed)
                {
                    throw new InvalidOperationException("Attempt to send action through a closed communicator");
                }
                WaitActionComplete.Set();
                return theInput.ReadInt32();
            }
            catch
            {
                theState = CommunicatorState.Closed;
            }
            WaitActionComplete.Set();
            return 0;
        }
        #endregion

        #region **************************** Public Properties ****************************
        public bool IsOpen
        {
            get { return (theState != CommunicatorState.Closed); }
        }
        public bool isBusy
        {
            get { return (theState == CommunicatorState.Busy); }
        }
        public bool isConnected
        {
            get { return connected; }
        }
        public int ReceiveTimeout
        {
            get { return theReceiveTimeout; }
            set { theReceiveTimeout = value; }
        }
        public int SendTimeout
        {
            get { return theSendTimeout; }
            set { theSendTimeout = value; }
        }
        #endregion

        #region **************************** Dispose Methods ******************************
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) { return; }
            if (disposing)
            {
                theStream?.Dispose();
                theInput?.Dispose();
                theOutput?.Dispose();
            }
            disposed = true;
        }
        #endregion
    }
}

