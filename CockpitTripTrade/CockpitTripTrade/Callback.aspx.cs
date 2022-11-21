using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using log4net;
using Newtonsoft.Json;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.OIDC;
using Library.Component.Utility;

namespace CockpitTripTrade
{
    public partial class Callback : System.Web.UI.Page
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Callback));

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyLogin();
        }

        private void VerifyLogin()
        {
            string tokenURL = Config.TokenEndpoint;
            string clientId = Config.ClientID;
            string clientSecret = Config.ClientSecret;
            string redirectUri = Config.RedirectURI;

            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];

            // Authentication Error
            if (code == null)
            {
                string error = Request.QueryString["Error"] != null ? Request.QueryString["Error"] : "QueryString[Error] is null";
                string errorDescription = Request.QueryString["ErrorDescription"] != null ? Request.QueryString["ErrorDescription"] : "QueryString[ErrorDescription] is null";

                ErrorHandling(error, errorDescription);
            }

            // DO NOT forget to check the state!
            // The state in the received authentication response must match the state
            // specified in the previous outgoing authentication request.
            string sessionState = (string) Session["State"];

            // 若驗證 sessionState 為 NULL 值(可能停留在帳密頁面過久導致過期)
            if (sessionState is null)
            {
                // 重新導向 SetLoginSession 頁面取驗證碼(小心無限導向循環)
                ErrorHandling("Original State is null", "Original State: " + sessionState + ", Return State: " + state);
                Response.Redirect("Login.aspx");
            }
            // 驗證當初傳送的的 State 是否一致，防範CSRF(跨站台要求偽造)
            else if (!sessionState.Equals(state))
            {
                ErrorHandling("State not equal", "Original State: " + sessionState + ", Return State: " + state);
            }

            //TuneService(); // 服務異常時調整設定

            // Post Data
            // 須透過 ParseQueryString() 方法來建立 NameValueCollection 物件，ToString() 才能轉換成 queryString 且 key 和 value 會自動 UrlEncode
            NameValueCollection postData = HttpUtility.ParseQueryString(string.Empty);
            postData.Add("grant_type", "authorization_code");
            postData.Add("code", code);
            postData.Add("redirect_uri", redirectUri);//Server.UrlEncode()

            // Token Request
            WebHeaderCollection webHeaders = new WebHeaderCollection() { @"Authorization: Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(clientId + @":" + clientSecret)) };
            //webHeaders.Add(@"Authorization", @"Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(clientId + @":" + clientSecret)));

            // KeepAlive = false 解決偶發之「基礎連接已關閉：應該保持運作的連接卻被伺服器關閉」之錯誤訊息
            //string data = "grant_type=authorization_code&code=" + code + "&redirect_uri=" + Server.UrlEncode(redirectUri);
            string responseData = HttpWebUtility.PostData(tokenURL, webHeaders, false, postData, @"application/x-www-form-urlencoded");
            if (string.IsNullOrEmpty(responseData))
            {
                ErrorHandling("Response Data is null or empty", "responseData: " + responseData);
                logger.Error("responseData Is Null Or Empty.");
            }
            else
            {
                TokenResponse tr = JsonConvert.DeserializeObject<TokenResponse>(responseData);
                byte[] bytes = Encoding.UTF8.GetBytes(clientSecret);
                string json = Jose.JWT.Decode(tr.id_token, bytes);
                IDToken it = JsonConvert.DeserializeObject<IDToken>(json);

                // Check Group Name
                var groups = it.group?.Where(x => x == @"CI");
                if (groups == null || groups.Count() <= 0)
                {
                    ErrorHandling(@"Invalid Group", @"Apologize that you are Unauthorized!", false);
                }

                // Check SetLoginSession ID
                if (string.IsNullOrEmpty(it.sub))
                {
                    // SetLoginSession Failed
                    ErrorHandling(@"Login UserID Is NULL", @"Login UserID Is Invalid!", false);
                }
                else
                {
                    // SetLoginSession Successfully
                    //************* Create User Session *************
                    LoginSession.SetLoginSession(sessionState, it.sub, WebApplicationEnum.CockpitTripTrade);
                    LoginSession ls = LoginSession.GetLoginSession();
                    if (ls == null)
                    {
                        // 登入失敗
                        Response.Redirect("~/Module/ErrorHandler/UnauthorizedPage.aspx");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ls.Employee.EmployID) && ls.IsSucceed)
                        {
                            // 登入成功
                            SaveLoginLog(ls, @"Login successfully");

                            if (ls.IsAdmin)
                            {
                                Response.Redirect("~/Module/Admin/ImpersonateUser.aspx");
                            }
                            else if (ls.Employee.AnalySa == @"100" && ls.CrewDb != null)
                            {
                                Response.Redirect("~/Module/Application/TaskList.aspx");
                            }
                            else
                            {
                                Response.Redirect("~/Module/ErrorHandler/UnauthorizedPage.aspx");
                            }
                        }
                        else
                        {
                            // 登入失敗
                            SaveLoginLog(ls, @"Login failed");
                            Response.Redirect("~/Module/ErrorHandler/UnauthorizedPage.aspx");
                        }
                    }
                    //***********************************************
                }
            }
        }

        private static void SaveLoginLog(LoginSession ls, string logDetail)
        {
            Log log = Log.GetLogWithLoginSuccessfully(@"Login", logDetail, true);
            log.Save(PageMode.PageModeEnum.Create);
        }

        /// <summary>
        /// OIDC 認證異常錯誤處理。
        /// </summary>
        /// <param name="subject">錯誤主旨</param>
        /// <param name="description">錯誤描述</param>
        /// <param name="isErrorPage">是否轉到錯誤頁</param>
        private void ErrorHandling(string subject, string description, bool isErrorPage = true)
        {
            logger.Error(@"OIDC 認證錯誤, [subject]=" + subject + @", [description]=" + description);
            SaveOIDCErrorHandlingLog(@"[OIDC 認證錯誤]" + subject, description);

            if (isErrorPage)
            {
                Response.Redirect("~/Module/ErrorHandler/Exception.aspx");
            }
            else
            {
                Response.Write("Error: " + subject);
                Response.Write("Error Description: " + description);
                Response.End();
            }
        }

        /// <summary>
        /// 必要時，手動調整 OIDC Service 連線設定，避開可能的連線錯誤。
        /// </summary>
        private void TuneService()
        {
            // hack 1: disable certificate validation (正式環境需驗證憑證，請註解該程式碼)
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //--------------------------

            // hack 2: 解決偶而連線錯誤問題----------
            // Error: 基礎連接已關閉: 應該保持運作的連接卻被伺服器關閉 
            // The underlying connection was closed: A connection that was expected to be kept alive was closed by the server

            // 試解1: .Net is sending first Expect 100 in one senddata of socket then send the actual request.
            //ServicePointManager.Expect100Continue = false;

            // 試解2: 只要設定的時間短於 Load Balancer 的 Idle Timeout 時間，連線就不會被 Load Balancer 切斷
            //Service​Point​Manager.SetTcpKeepAlive(true, 500, 10); //預設1000,10

            // 試解3: 基礎連接已關閉 : 應該保持運作的連接卻被伺服器關閉
            //ServicePointManager.DefaultConnectionLimit = 50;
            //--------------------------

            // hack 3: 解決SSL協定失敗問題--------------------------

            // .net 2.0~4.0
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3; //| SecurityProtocolType.Tls;
            // .net 4.5up
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3; // | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //--------------------------
        }

        /// <summary>
        /// OIDC 認證異常時儲存日誌。
        /// </summary>
        /// <param name="logSubject">日誌主旨</param>
        /// <param name="logDetail">日誌明細</param>
        /// <returns></returns>
        private bool SaveOIDCErrorHandlingLog(string logSubject, string logDetail)
        {
            Log log = new Log()
            {   
                WebApplication = Enum.TryParse<WebApplicationEnum>(RequestUtility.Url.Segments[1].Replace("/", ""), out _) ? (Enum.Parse(typeof(WebApplicationEnum), RequestUtility.Url.Segments[1].Replace("/", "")) as WebApplicationEnum?).ToString() : null,
                LogSubject = logSubject,
                LogDetail = logDetail,
                IsSuccess = false,
                UserAgent = RequestUtility.UserAgent,
                UserIP = RequestUtility.GetClientIP(),
                ServerName = RequestUtility.ServerName,
                CreateBy = Config.SYSTEM_ADMIN, CreateStamp = DateTime.Now,
                UpdateBy = Config.SYSTEM_ADMIN, UpdateStamp = DateTime.Now
            };

            return log.Save(PageMode.PageModeEnum.Create);
        }
    }
}