using System;
using System.Runtime.InteropServices;
using BOOL = System.Boolean;
using DWORD = System.UInt32;
using LPWSTR = System.String;
using NET_API_STATUS = System.UInt32;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace BizTalk.Archiving.Common
{
    public class UNCAccessWithCredentials : IDisposable
    {
        internal static class NativeMethods
        {
            [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern NET_API_STATUS NetUseAdd(
                LPWSTR UncServerName,
                DWORD Level,
                ref USE_INFO_2 Buf,
                out DWORD ParmError);

            [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern NET_API_STATUS NetUseDel(
                LPWSTR UncServerName,
                LPWSTR UseName,
                DWORD ForceCond);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct USE_INFO_2
        {
            internal LPWSTR ui2_local;
            internal LPWSTR ui2_remote;
            internal LPWSTR ui2_password;
            internal DWORD ui2_status;
            internal DWORD ui2_asg_type;
            internal DWORD ui2_refcount;
            internal DWORD ui2_usecount;
            internal LPWSTR ui2_username;
            internal LPWSTR ui2_domainname;
        }

        private bool disposed = false;

        private string sUNCPath;
        private string sUser;
        private string sPassword;
        private string sDomain;
        private int iLastError;

        /// <summary>
        /// A disposeable class that allows access to a UNC resource with credentials.
        /// </summary>
        public UNCAccessWithCredentials()
        {
        }

        /// <summary>
        /// The last system error code returned from NetUseAdd or NetUseDel.  Success = 0
        /// </summary>
        public int LastError
        {
            get { return iLastError; }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose Implementation
        /// </summary>
        /// <param name="isDisposing"></param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (disposed)
                return;

            if (isDisposing)
            {
                //NetUseDelete();
            }
            disposed = true;
        }

        /// <summary>
        /// Connects to a UNC path using the credentials supplied.
        /// </summary>
        /// <param name="UNCPath">Fully qualified domain name UNC path</param>
        /// <param name="User">A user with sufficient rights to access the path.</param>
        /// <param name="Domain">Domain of User.</param>
        /// <param name="Password">Password of User</param>
        /// <returns>True if mapping succeeds.  Use LastError to get the system error code.</returns>
        public bool NetUseWithCredentials(string UNCPath, string User, string Domain, string Password)
        {
            sUNCPath = UNCPath;
            sUser = User;
            sPassword = Password;
            sDomain = Domain;
            return NetUseWithCredentials();
        }

        private bool NetUseWithCredentials()
        {
            uint returncode;
            try
            {
                USE_INFO_2 useinfo = new USE_INFO_2();

                useinfo.ui2_remote = sUNCPath;
                useinfo.ui2_username = sUser;
                useinfo.ui2_domainname = sDomain;
                useinfo.ui2_password = sPassword;
                useinfo.ui2_asg_type = 0;
                useinfo.ui2_usecount = 1;
                uint paramErrorIndex;
                returncode = NativeMethods.NetUseAdd(null, 2, ref useinfo, out paramErrorIndex);
                iLastError = (int)returncode;
            }
            catch
            {
                iLastError = Marshal.GetLastWin32Error();
            }

            //If 0 it means there's no error
            if (iLastError == 0)
            {
                return true;
            }
            //Default error codes that are being ignored.
            //1394 = No Session Key
            //64   = The Specified Network name is no longer available
            //1326,1219 Multiple connections to a server or shared resource by the same user, using more than one user name, are not allowed. Disconnect all previous connections to the server or shared resource and try again. It means that there's already connection to this folder.
            else if (ApplicationConstant.GetIgnoredErrorCodes().Contains(iLastError.ToString()))
            {
                //Error is being ignored
                iLastError = 0;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Ends the connection to the remote resource 
        /// </summary>
        /// <returns>True if it succeeds.  Use LastError to get the system error code</returns>
        public bool NetUseDelete()
        {
            uint returncode;
            try
            {
                returncode = NativeMethods.NetUseDel(null, sUNCPath, 2);
                iLastError = (int)returncode;
                return (returncode == 0);
            }
            catch
            {
                iLastError = Marshal.GetLastWin32Error();
                return false;
            }
        }
    }
}