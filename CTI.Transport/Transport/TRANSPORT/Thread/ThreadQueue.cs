using System.IO;

namespace COBOL
{
    public class ThreadQueue
    {
        #region **************************** Private Properties *******************************
        private string remotefile;
        private Stream source;
        private FileStream filesource;
        #endregion

        #region ******************************** Constructor **********************************
        public ThreadQueue()
        {
            remotefile = string.Empty;
            source = null;
            filesource = null;
        }
        #endregion

        #region ***************************** Public Properties *******************************
        public string RemoteFile
        {
            get { return remotefile; }
            set { remotefile = value; }
        }
        public Stream Source
        {
            get { return source; }
            set { source = value; }
        }
        public FileStream FileSource
        {
            get { return filesource; }
            set { filesource = value; }
        }
        #endregion
    }
}