using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Enums;

namespace Library.Module.FCDS
{
    /// <summary>
    /// 頁面父類別。
    /// </summary>
    public class BasePage : System.Web.UI.Page
    {
        #region Property
        /// <summary>
        /// 父類別模組名稱。
        /// </summary>
        public string BaseModuleName { get; set; }

        /// <summary>
        /// 父類別模組標題。
        /// </summary>
        public string BaseModuleTitle { get; set; }

        /// <summary>
        /// 父類別表單名稱。
        /// </summary>
        public string BaseFormName { get; set; }

        /// <summary>
        /// 父類別表單標題。
        /// </summary>
        public string BaseFormTitle
        {
            get
            {
                Label lbl = this.Master.FindControl("lblPgmid") as Label;
                if (lbl != null) return lbl.Text;
                return null;
            }
            set
            {
                Label lbl = this.Master.FindControl("lblPgmid") as Label;
                if (lbl != null) lbl.Text = value;
            }
        }

        /// <summary>
        /// 頁面模式列舉型態。
        /// </summary>
        public PageMode.PageModeEnum BasePageModeEnum { get; set; }

        public LoginSession LoginSession { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public BasePage()
	    {
	    }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }

        protected override void OnInit(EventArgs e)
        {
            // Assuming that your page makes use of ASP.NET session state and the SessionID is stable
            ViewStateUserKey = Session.SessionID;

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            // TODO: 檢查 Session
            LoginSession.VerifyLogin(this.Page, string.Empty);
            LoginSession = LoginSession.GetLoginSession();

            base.OnLoad(e);
        }

        /// <summary>
        /// Script 計數器。
        /// </summary>
        private int countScripts = 0;

        /// <summary>
        /// JavaScript 警告視窗。
        /// </summary>
        /// <param name="msg"></param>
        protected void Alert(string msg)
        {
            msg = msg.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r");
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Scripts_" + countScripts.ToString(), "alert('" + msg + "');", true);
            countScripts++;
        }

        /// <summary>
        /// JavaScript 關閉頁面。
        /// </summary>
        protected void Close()
        {
            string js = @"window.openr=null; window.open("""",""_self""); window.close;";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), @"ClosePage", js, true);
        }

        /// <summary>
        /// 以 JavaScript window.location 方式重新導向用戶端至新的 URL。
        /// </summary>
        /// <param name="url">URL 字串</param>
        protected void Redirect(string url)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "Scripts_" + countScripts.ToString(), "window.location='" + url + "';", true);
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Scripts_" + countScripts.ToString(), "window.location='" + url + "';", true);
            countScripts++;
        }

        /// <summary>
        /// 以 ScriptManager.RegisterStartupScript()方法 執行 JavaScript 指令。
        /// </summary>
        /// <remarks>
        /// RegisterClientScriptBlock() 方法產生於 <form  runat="server"> 標籤後，比 RegisterStartupScript() 更早執行，如須為全域變數可使用。
        /// RegisterStartupScript() 方法產生於 </form> 結尾標籤上方，方便用戶端取值，因為全部的控制項繪製(Render)後，它才會執行。
        /// </remarks>
        /// <param name="scripts">Script 指令</param>
        protected void RunJavascript(string scripts)
        {
            if (!scripts.EndsWith(";"))
            {
                scripts += ";";
            }
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Scripts_" + countScripts.ToString(), scripts, true);
            countScripts++;
        }

        /// <summary>
        /// 顯示 Bootstrap Alerts。
        /// </summary>
        /// <param name="div">頁面 DIV HTML 物件</param>
        /// <param name="msg">提示訊息文字</param>
        /// <param name="isDismissible">是否可關閉</param>
        /// <param name="enumBootstrapAlertsType">Bootstrap Alert 類型列舉型態</param>
        protected void BootstrapAlerts(HtmlGenericControl div, string msg, bool isDismissible, BootstrapAlertsTypeEnum enumBootstrapAlertsType = BootstrapAlertsTypeEnum.Plain)
        {
            string type = null;

            switch (enumBootstrapAlertsType)
            {
                case BootstrapAlertsTypeEnum.Plain:
                    type = @"";
                    break;

                case BootstrapAlertsTypeEnum.Primary:
                    type = @"primary";
                    break;

                case BootstrapAlertsTypeEnum.Secondary:
                    type = @"secondary";
                    break;

                case BootstrapAlertsTypeEnum.Success:
                    type = @"success";
                    break;

                case BootstrapAlertsTypeEnum.Danger:
                    type = @"danger";
                    break;

                case BootstrapAlertsTypeEnum.Warning:
                    type = @"warning";
                    break;

                case BootstrapAlertsTypeEnum.Information:
                    type = @"info";
                    break;

                case BootstrapAlertsTypeEnum.Light:
                    type = @"light";
                    break;

                case BootstrapAlertsTypeEnum.Dark:
                    type = @"dark";
                    break;
            }

            if (!(div.Page.IsPostBack || div.Page.IsPostBack) && string.IsNullOrEmpty(msg))
            {
                div.Visible = false;
            }
            else
            {
                div.Attributes.Add(@"class", @"alert alert-" + type + (isDismissible ? @" alert-dismissible fade show  col-xl-8" : string.Empty));
                div.Attributes.Add(@"role", @"alert");
                div.InnerHtml = msg;
                div.Visible = true;

                if (isDismissible)
                {
                    div.InnerHtml += @"<button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close"">
                        <span aria-hidden = ""true"">&times;</span>
                        </ button>";
                }

                div.Focus();

                //ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), @"Scripts_" + System.Guid.NewGuid().ToString(), @"", true);
            }
        }
    }
}