using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Library.Component.Utility
{
    /// <summary>
    /// ASP.NET HTTP 要求的 HttpRequest 物件輔助密封類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class RequestUtility
    {
        /// <summary>
        /// Web 伺服器的伺服器主機名稱、DNS 別名或 IP 位址。
        /// </summary>
        public static string ServerName { get; } = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

        /// <summary>
        /// 要求的 URL 資訊。
        /// </summary>
        public static Uri Url { get; } = HttpContext.Current.Request.Url;

        /// <summary>
        /// 用戶端瀏覽器的原始使用者代理字串。
        /// </summary>
        public static string UserAgent { get; } = HttpContext.Current.Request.UserAgent;

        public static string UserHostName { get; } = HttpContext.Current.Request.UserHostName;

        /// <summary>
        /// 建構子之存取修飾詞改為 private 防止建立本類別物件。
        /// </summary>
        private RequestUtility()
        {

        }

        /// <summary>
        /// 取得用戶端來源 IP 位址。
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            // 判斷來源用戶端是否使用代理伺服器(Proxy Server)
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] == null)
            {
                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            else
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
        }

    }
}
