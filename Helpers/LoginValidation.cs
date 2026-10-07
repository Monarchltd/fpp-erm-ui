
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace DotNet8.WebApi.Factory.Helpers
{
    public class LoginValidation
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        public bool getDetails(string username, string email, string domain, string password)
        {
            string retVal = String.Empty;

            const int LOGON32_PROVIDER_DEFAULT = 0;
            //This parameter causes LogonUser to create a primary token.
            const int LOGON32_LOGON_INTERACTIVE = 2;

            // Call LogonUser to obtain a handle to an access token.
            SafeAccessTokenHandle safeAccessTokenHandle;

            bool returnValue = LogonUser(username, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out safeAccessTokenHandle);

            
            return returnValue;
        }

    }
}
