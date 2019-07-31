using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Runtime.InteropServices; // DllImport
using System.ComponentModel;
using System.IO;

namespace JUUL.Manufacture.DataStorage
{
    class DataStorageAccess:IDisposable
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);
        private IntPtr userHandle = IntPtr.Zero;
        private WindowsImpersonationContext impersonationContext;

        private string sAFG_AFG_NetworkUserName = @"tester";
        private string sAFG_AFG_NetworkDomain = @"asteelflash";
        private string sAFG_AFG_NetworkPassword = @"F%Kg21Tc";

        private string sAFG_JUUL_NetworkUserName = @"tester";
        private string sAFG_JUUL_NetworkDomain = @"asteelflash";
        private string sAFG_JUUL_NetworkPassword = @"F%Kg21Tc";

        private string sPega_JUUL_NetworkUserName = @"MTETEST";
        private string sPega_JUUL_NetworkDomain = @"";
        private string sPega_JUUL_NetworkPassword = @"psz!@#123";

        private string networkUserName, networkDomain, networkPassword;

        private bool LoginSuccessful=false;

        public DataStorageAccess(ContractManufacture cm)
        {
            switch (cm)
            {
                case ContractManufacture.AFGServer:
                    networkUserName = sAFG_AFG_NetworkUserName;
                    networkDomain = sAFG_AFG_NetworkDomain;
                    networkPassword = sAFG_AFG_NetworkPassword;
                    break;
                case ContractManufacture.AFG:
                    networkUserName = sAFG_JUUL_NetworkUserName;
                    networkDomain = sAFG_JUUL_NetworkDomain;
                    networkPassword = sAFG_JUUL_NetworkPassword;
                    break;

                //Add other CM support.
                case ContractManufacture.Pegatron:
                    networkUserName = sPega_JUUL_NetworkUserName;
                    networkDomain = sPega_JUUL_NetworkDomain;
                    networkPassword = sPega_JUUL_NetworkPassword;
                    break;
                default:
                    break;
            }

            try
            {
                bool loggedOn = false;
                // Call LogonUser to get a token for the user 
                loggedOn = LogonUser(networkUserName, networkDomain, networkPassword,
                    9 /*(int)LogonType.LOGON32_LOGON_NEW_CREDENTIALS*/,
                    3 /*(int)LogonProvider.LOGON32_PROVIDER_WINNT50*/,
                out userHandle);
                if (!loggedOn)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                // Begin impersonating the user 
                impersonationContext = WindowsIdentity.Impersonate(userHandle);
                LoginSuccessful = true;
            }
            catch
            {
                LoginSuccessful = false;
            }
        }

        public bool GetLoginStatus()
        {
            return LoginSuccessful;
        }

        public void Dispose()
        {
            LoginSuccessful = false;
            try
            {
                if (userHandle != IntPtr.Zero)
                    CloseHandle(userHandle);
                if (impersonationContext != null)
                    impersonationContext.Undo();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }

    
}
