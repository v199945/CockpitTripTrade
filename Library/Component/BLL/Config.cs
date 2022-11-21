using System;
using System.Linq;
using System.Web.Configuration;

using log4net;

namespace Library.Component.BLL
{
    /// <summary>
    /// Web.Config 網站組態設定檔類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class Config
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Config));

        #region Property
        /// <summary>
        /// OIDC Client ID，Authentication Requestc 認證。
        /// </summary>
        public static string ClientID { get; } = WebConfigurationManager.AppSettings["ClientID"].ToString();

        /// <summary>
        /// OIDC Client Secret，Token Request 認證。
        /// </summary>
        public static string ClientSecret { get; } = WebConfigurationManager.AppSettings["ClientSecret"].ToString();

        /// <summary>
        /// OIDC 帳密驗證後的系統導向網址，並於 OIDC Request 做認證用(建議使用 https 加密網址)。
        /// </summary>
        public static string RedirectURI { get; } = WebConfigurationManager.AppSettings["RedirectURI"].ToString();

        /// <summary>
        /// OIDC Authentication Request 網址。
        /// </summary>
        public static string AuthorizeEndpoint { get; } = WebConfigurationManager.AppSettings["AuthorizeEndpoint"].ToString();

        /// <summary>
        /// OIDC Token Request 網址。
        /// </summary>
        public static string TokenEndpoint { get; } = WebConfigurationManager.AppSettings["TokenEndpoint"].ToString();

        /// <summary>
        /// OIDC 登出網址。
        /// </summary>
        public static string LogoutOIDCUri { get; } = WebConfigurationManager.AppSettings["LogoutOIDCUri"].ToString();

        /// <summary>
        /// SMTP Server。
        /// </summary>
        public static string SmtpServer { get; } = WebConfigurationManager.AppSettings["SmtpServer"].ToString();

        /// <summary>
        /// 依據 Environment 參數，回傳 FZDB@ORP3 Or FZDB@ORT1 資料庫連線字串。
        /// </summary>
        public static string FZDBConnectionString
        {
            get
            {
                if (Config.Environment.Equals("PROD"))
                {
                    return WebConfigurationManager.ConnectionStrings["FZDBProdConnectionString"].ConnectionString;
                }
                else
                {
                    return WebConfigurationManager.ConnectionStrings["FZDBTestConnectionString"].ConnectionString;
                }
            }
        }

        /// <summary>
        /// 依據 Environment 參數，回傳正式或測試 HAM 資料庫連線字串。
        /// </summary>
        public static string HAMDBConnectionString
        {
            get
            {
                if (Config.Environment.Equals("PROD"))
                {
                    return WebConfigurationManager.ConnectionStrings["HAMProdConnectionString"].ConnectionString;
                }
                else
                {
                    return WebConfigurationManager.ConnectionStrings["HAMTestConnectionString"].ConnectionString;
                }
            }
        }

        /// <summary>
        /// 系統環境，正式(PROD)、測試(TEST)或開發環境(DEVE)環境。
        /// </summary>
        public static string Environment { get; } = WebConfigurationManager.AppSettings["Environment"];

        /// <summary>
        /// 是否偵錯。
        /// </summary>
        public static bool Debug { get; } = bool.Parse(WebConfigurationManager.AppSettings["Debug"]);


        /// <summary>
        /// 網頁應用程式之站台與通訊協定及應用程式名稱。
        /// </summary>
        public static string WebRootUrl { get; } = WebConfigurationManager.AppSettings["WebRootUrl"];
        #endregion

        /// <summary>
        /// 管理後台網頁應用程式之站台與通訊協定及應用程式名稱。
        /// </summary>
        public static string AdminWebRootUrl { get; } = WebConfigurationManager.AppSettings["AdminWebRootUrl"];

        /// <summary>
        /// 此環境是否為正式環境。
        /// </summary>
        public static bool IsProduction { get; } = Environment.Equals(@"PROD");

        /// <summary>
        /// 系統帳號名稱。
        /// </summary>
        public static readonly string SYSTEM_ADMIN = @"SystemAdmin";

        /// <summary>
        /// 將建構子之存取修飾詞改為 private 防止建立本類別物件。
        /// </summary>
        private Config()
        {
            
        }

        public static void PrintConfigProperty()
        {
            logger.Info("Config.Environment=" + Config.Environment);
            logger.Info("Config.Debug=" + Config.Debug.ToString());
        }
    }
}