using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;

namespace CockpitTripTrade
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // State: 會隨權杖回應傳回之要求中所包含的值。其可以是您想要之任何內容的字串。 
            //        建議"隨機產生"的唯一值通常用於防止跨站台要求偽造攻擊。 
            //        此狀態也用於在驗證要求出現之前，於應用程式中編碼使用者的狀態資訊，例如之前所在的網頁或檢視。
            // 建議每次亂數產生，回傳時驗證此 Request，可防範 CSRF(跨站台要求偽造)攻擊。
            //string token = Guid.NewGuid().ToString();
            //Session[token] = DateTime.Now.ToString("HHmmss");
            Session["State"] = Guid.NewGuid().ToString();

            string authorizeURL = Config.AuthorizeEndpoint;
            string responseType = "code";                   // OAuth 2.0 Response Type value that determines the authorization processing flow to be used.
            string scope = "openid";                        // OpenID  Connect requests MUST contain the openid scope value.
            string clientID = Config.ClientID;              // OAuth 2.0 Client Identifier
            string state = (string) Session["State"];       // Opaque value used to maintain state between the request and the callback.
            string redirectURI = Config.RedirectURI;        // Redirection URI to which the response will be sent.

            // Redirect to IAM SetLoginSession Page
            string redirectURL = @authorizeURL
                 + @"?response_type=" + Server.UrlEncode(responseType)
                 + @"&scope=" + Server.UrlEncode(scope)
                 + @"&client_id=" + Server.UrlEncode(clientID)
                 + @"&state=" + Server.UrlEncode(state)
                 + @"&redirect_uri=" + Server.UrlEncode(redirectURI);

            // 強烈建議系統使用 https 加密，避免 request 金鑰資訊被竊取。
            Response.Redirect(redirectURL);
        }
    }
}