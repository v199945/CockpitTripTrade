using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Library.Component.Utility
{
    /// <summary>
    /// 模擬 Windows 使用者類別，跨網路讀取檔案須使用。
    /// </summary>
    public class ImpersonateUtility
    {
        [DllImport("advapi32.dll")]
        protected static extern bool LogonUser(string UserName, string Domain, string Password, int LogonType, int LogonProvider, out IntPtr token);

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool CloseHandle(IntPtr handle);

        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        private IntPtr _token = IntPtr.Zero;
        private WindowsImpersonationContext _impcontext = null;

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ImpersonateUtility()
        {
        }

        /// <summary>
        /// 設定模擬使用者並且登入。
        /// </summary>
        /// <param name="username">使用者帳號</param>
        /// <param name="password">使用者密碼</param>
        /// <param name="domain">使用者網域</param>
        /// <returns></returns>
        public bool Logon(string username, string password, string domain)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            else
            {
                bool isLogin = LogonUser(username, domain, password, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_DEFAULT, out _token);
                if (isLogin)
                {
                    WindowsIdentity wi = new WindowsIdentity(_token);
                    wi.Impersonate();
                }

                return isLogin;
            }
        }

        /// <summary>
        /// 模擬使用者登出。
        /// </summary>
        public void Logout()
        {
            if (_impcontext != null)
            {
                _impcontext.Undo();
            }

            if (_token != IntPtr.Zero)
            {
                CloseHandle(_token);
            }
        }
    }
}
