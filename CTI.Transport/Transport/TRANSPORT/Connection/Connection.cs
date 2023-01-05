#region .NET Base Class Namespace Imports
using System;
using System.IO;
using System.Net.Sockets;
#endregion

namespace CompiledTechnologies.Transport
{
    public sealed class Connection : IDisposable
    {
        #region ********************************** Private Fields *****************************************
        private int theBytesRead;
        private int theBytesWritten;
        private ValueInputStream theInput;
        private ValueOutputStream theOutput;
        private int theReceiveTimeout;
        private int theSendTimeout;
        private Socket theSocket;
        private Stream theStream;
        private bool disposed;
        #endregion

        #region ******************************* Public Initialization *************************************
        ~Connection()
        {
            disposed = false;
            Abort();
        }
        public Connection()
        {

        }
        public Connection(Socket s)
        {
            Open(s);
        }
        #endregion

        #region ********************************** Public Methods *****************************************
        public void Abort()
        {
            if (theInput != null)
            {
                theBytesRead = theInput.BytesRead;
            }
            if (theOutput != null)
            {
                theBytesWritten = theOutput.BytesWritten;
            }
            theInput = null;
            theOutput = null;
            theStream?.Close();
            theSocket?.Close();
            theStream = null;
            theSocket = null;
        }
        public void Open(Stream s)
        {
            theSocket = null;
            theStream = s;
            theInput = new ValueInputStream(theStream, this);
            theOutput = new ValueOutputStream(theStream);
        }

        public void Open(Socket s)
        {
            theSocket = s ?? throw new InvalidOperationException("Attempt to open a connection with a null socket");
            if (SendTimeout != 0)
            {
                theSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, theSendTimeout);
            }
            if (ReceiveTimeout != 0)
            {
                theSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, theReceiveTimeout);
            }
            theStream = new NetworkStream(theSocket, true);
            theInput = new ValueInputStream(theStream, this);
            theOutput = new ValueOutputStream(theStream);
        }
        public void Close()
        {
            if (theStream != null)
            {
                theOutput.Flush();
                Abort();
            }
        }
        #endregion

        #region ********************************* Public Properties ***************************************
        public int BytesRead
        {
            get
            {
                if (theInput == null)
                {
                    return theBytesRead;
                }
                return theInput.BytesRead;
            }
        }
        public int BytesWritten
        {
            get
            {
                if (theOutput == null)
                {
                    return theBytesWritten;
                }
                return theOutput.BytesWritten;
            }
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

        #region ********************************** Dispose Methods ****************************************
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposed) { return; }
            if (disposing)
            {
                theInput?.Dispose();
                theOutput?.Dispose();
                theSocket?.Dispose();
                theStream?.Dispose();
            }
            disposed = true;
        }
        #endregion
    }
}