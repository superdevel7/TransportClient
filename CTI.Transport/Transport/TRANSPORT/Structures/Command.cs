#region .NET Base Class Namespace Import
using System.Runtime.InteropServices;
#endregion

namespace CompiledTechnologies.Transport
{
    internal struct Command
    {
        public int nCommand;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string UNCPath;
    }
}
