using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace CockpitTripTrade
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // 在應用程式啟動時執行的程式碼
            string log4netpath = Server.MapPath("~/Log4Net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(log4netpath));

            //ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition { Path = "~/asset/v3.0.0/js/jquery-3.5.1.js" });
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // 在新的工作階段啟動時執行的程式碼

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // 在發生未處理的錯誤時執行的程式碼

        }

        protected void Session_End(object sender, EventArgs e)
        {
            // 在工作階段結束時執行的程式碼
            // 注意: 只有在  Web.config 檔案中將 sessionstate 模式設定為 InProc 時，
            // 才會引起 Session_End 事件。如果將 session 模式設定為 StateServer 
            // 或 SQLServer，則不會引起該事件。

        }

        protected void Application_End(object sender, EventArgs e)
        {
            //  在應用程式關閉時執行的程式碼

        }
    }
}