using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using log4net;

using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FZDB;
using Library.Module.HRDB;

namespace Library.Component.BLL
{
    /// <summary>
    /// 使用者成功登入系統之 Session 類別。
    /// 禁止於 ASPX 取得或指派任何 Session 值，統一透過此類別之 GetLoginSession() 靜態方法，取得登入 Session 物件。
    /// </summary>
    public class LoginSession
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LoginSession));

        #region Property
        /// <summary>
        /// Session 權杖。
        /// </summary>
        public string SessionToken { get; set; }

        /// <summary>
        /// 使用者帳號，即員工編號。
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 是否登入成功。
        /// </summary>
        public bool IsSucceed { get; set; }

        /// <summary>
        /// 是否為管理者。
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 登入時間。
        /// </summary>
        public DateTime LoginDateTime { get; set; }

        /// <summary>
        /// 使用者 IP。
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 使用者瀏覽器原始使用者代理程式字串。
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 使用者之群組集合物件。
        /// </summary>
        public UserGroupCollection UserGroups { get; set; }

        /// <summary>
        /// 使用者之功能權限集合物件。
        /// </summary>
        public UserModuleFormFunctionCollection UserModuleFormFunctions { get; set; }

        /// <summary>
        /// 使用者之功能權限集合物件。
        /// </summary>
        //public ModuleFormFunctionCollection ModuleFormFunctions { get; set; }

        /// <summary>
        /// ASP.NET Web 應用程式列舉型態。
        /// </summary>
        public WebApplicationEnum? WebApplicationEnum { get; set; }

        /// <summary>
        /// 使用者之 HRDB 員工基本資料物件。
        /// </summary>
        public HrVEgEmploy Employee { get; set; }

        /// <summary>
        /// 使用者之 AIMS [CI].[VvCrewDb] 組員基本資料物件。
        /// </summary>
        public CIvvCrewDb CrewDb { get; set; }
        #endregion

        private LoginSession()
        {

        }

        /// <summary>
        /// 設定成功登入 Session 物件。
        /// </summary>
        /// <param name="sessionToken">Session 權杖</param>
        /// <param name="userID">員工編號</param>
        /// <param name="enumWebApplication">飛航組員任務互換申請系統網站應用程式列舉型態</param>
        public static void SetLoginSession(string sessionToken, string userID, WebApplicationEnum enumWebApplication)
        {
            if (!string.IsNullOrEmpty(sessionToken) && !string.IsNullOrEmpty(userID))
            {
                LoginSession ls = null;
                HrVEgEmploy employ = new HrVEgEmploy(userID);
                UserGroupCollection userGroups = UserGroup.FetchByUseridAndUnitCd(userID, employ.UnitCd, ReturnObjectTypeEnum.Collection) as UserGroupCollection;

                if (userGroups.Count == 0 && (enumWebApplication != Enums.WebApplicationEnum.CockpitTripTrade && employ.AnalySa != @"100"))
                {
                    // 使用者群組集合個數為零，且系統不為前台與[人員屬性/職務類別]不為飛航組員，代表無權限。
                }
                else
                {
                    CIvvCrewDb crewDb = null;
                    if (employ.AnalySa == @"100")
                    {
                        crewDb = new CIvvCrewDb(userID);
                    }

                    ls = new LoginSession
                    {
                        SessionToken = sessionToken,
                        UserID = employ.EmployID,
                        IsSucceed = true,
                        IsAdmin = Group.IsSystemAdmin(userGroups) || Group.IsUserAdmin(userGroups),
                        LoginDateTime = DateTime.Now,
                        ClientIP = RequestUtility.GetClientIP(),
                        UserAgent = RequestUtility.UserAgent,
                        UserGroups = userGroups,
                        UserModuleFormFunctions = UserModuleFormFunction.FetchByUnitCdAndIDUser(employ.UnitCd, userID, ReturnObjectTypeEnum.Collection) as UserModuleFormFunctionCollection,
                        //ModuleFormFunctions = ModuleFormFunction.FetchByIDUserAndUnitCd(userID, employ.UnitCd, ReturnObjectTypeEnum.Collection) as ModuleFormFunctionCollection,
                        WebApplicationEnum = Enum.TryParse<WebApplicationEnum>(RequestUtility.Url.Segments[1].Replace("/", ""), out _) ? Enum.Parse(typeof(WebApplicationEnum), RequestUtility.Url.Segments[1].Replace("/", "")) as WebApplicationEnum? : null,
                        Employee = employ,
                        CrewDb = crewDb
                    };

                    switch (enumWebApplication)
                    {
                        case Enums.WebApplicationEnum.None:
                            break;

                        case Enums.WebApplicationEnum.CockpitTripTrade:
                            if (!ls.IsAdmin && ls.CrewDb == null) ls = null;
                            break;

                        case Enums.WebApplicationEnum.CockpitTripTradeAdmin:
                            if (ls.Employee.AnalySa == @"100") ls = null;
                            break;
                    }

                }

                HttpContext.Current.Session["LoginSession"] = ls;
            }
        }

        /// <summary>
        /// 取得 Session 轉型為 LoginSession 物件。
        /// </summary>
        /// <returns></returns>
        public static LoginSession GetLoginSession()
        {
            return HttpContext.Current.Session["LoginSession"] as LoginSession;
        }

        /// <summary>
        /// 檢核登入狀態，Session 是否為 NULL 值，否則跳轉至 Session 逾時(Timeout)頁面提示使用者。
        /// </summary>
        /// <param name="page">來源頁面</param>
        /// <param name="redirectPage">欲跳轉之頁面</param>
        public static void VerifyLogin(Page page, string redirectPage)
        {
            LoginSession ls = GetLoginSession();
            if (ls == null)
            {
                if (!page.IsCallback)
                {
                    // Redirect to Session Timeout Page.
                    page.Response.Redirect(@"~/Module/ErrorHandler/Timeout.aspx");
                }
            }
            else
            {
                if ((ls.WebApplicationEnum == Enums.WebApplicationEnum.CockpitTripTrade && ls.CrewDb == null && !ls.IsAdmin) || (ls.WebApplicationEnum == Enums.WebApplicationEnum.CockpitTripTradeAdmin && ls.CrewDb != null))
                {
                    // Redirect to Unauthorized Page.
                    page.Response.Redirect(@"~/Module/ErrorHandler/UnauthorizedPage.aspx");
                }
                else
                {
                    // 若不判斷 page.Request.RawUrl.EndsWith(@"ImpersonateUser.aspx")，將出現「將您重新導向的次數過多」錯誤訊息
                    if (ls.WebApplicationEnum == Enums.WebApplicationEnum.CockpitTripTrade && ls.CrewDb == null && ls.IsAdmin && !page.Request.RawUrl.EndsWith(@"ImpersonateUser.aspx"))
                    {
                        page.Response.Redirect(@"~/Module/Admin/ImpersonateUser.aspx");
                    }

                    if (ls.IsSucceed && !page.IsCallback && !string.IsNullOrEmpty(redirectPage))
                    {
                        // Redirect to Specific Page.
                        page.Response.Redirect(redirectPage);
                    }
                }
            }
        }

        /// <summary>
        /// 檢核使用者是否為管理員，須為管理員方可開啟~/Module/Admin 下之頁面，非管理員跳轉頁面至 ~/Module/ErrorHandler/UnauthorizedPage.aspx。
        /// </summary>
        /// <param name="page">來源頁面</param>
        public static void VerifyAdmin(Page page)
        {
            LoginSession ls = LoginSession.GetLoginSession();
            if (ls == null)
            {
                if (!page.IsCallback)
                {
                    // Redirect to Session Timeout Page.
                    page.Response.Redirect(@"~/Module/ErrorHandler/Timeout.aspx");
                }
            }
            else
            {
                if (!ls.IsAdmin)
                {
                    // Redirect to Unauthorized Page.
                    page.Response.Redirect(@"~/Module/ErrorHandler/UnauthorizedPage.aspx");
                }
            }
        }

        public static bool IsValidPage()
        {
            return true;
        }

        /// <summary>
        /// 是否有編輯權限。
        /// </summary>
        /// <param name="page">頁面</param>
        /// <param name="idBllModuleForm">模組頁面編號</param>
        /// <returns></returns>
        public static bool HasEditAuthority(Page page, string idBllModuleForm)
        {
            return HasAuthority(idBllModuleForm, IDFunctionEnum.Edit);
        }


        /// <summary>
        /// 是否有檢視權限。
        /// </summary>
        /// <param name="page">頁面</param>
        /// <param name="idBllModuleForm">模組頁面編號</param>
        /// <returns></returns>
        public static bool HasViewAuthority(Page page, string idBllModuleForm)
        {
            return HasAuthority(idBllModuleForm, IDFunctionEnum.View);
        }

        /// <summary>
        /// 驗證控制項之授權。
        /// </summary>
        /// <param name="page">頁面</param>
        /// <param name="idBllModuleForm">模組頁面編號</param>
        /// <param name="webControl">頁面控制項</param>
        public static void VerifyAuthorization(Page page, string idBllModuleForm, WebControl webControl)
        {
            SetWebControl(webControl, HasAuthority(idBllModuleForm, IDFunctionEnum.Edit), HasAuthority(idBllModuleForm, IDFunctionEnum.View));
        }

        public static bool IsAuthorization(Page page, string idBllModuleForm, WebControl webControl, IDFunctionEnum enumIDFunction)
        {
            return HasAuthority(idBllModuleForm, enumIDFunction);
        }

        /// <summary>
        /// 是否有模組頁面某功能權限。
        /// </summary>
        /// <param name="idBllModuleForm">模組頁面編號</param>
        /// <param name="enumIDFunction">使用者模組表單功能列舉型態</param>
        /// <returns></returns>
        public static bool HasAuthority(string idBllModuleForm, IDFunctionEnum enumIDFunction)
        {
            LoginSession ls = GetLoginSession();
            if (ls == null)
            {
                return false;
            }
            else
            {
                //ModuleFormFunctions
                return ls.UserModuleFormFunctions?.Where(x => x.IDBllModuleForm == idBllModuleForm && x.IDFunctionEnum == enumIDFunction).Count() > 0;
            }
        }

        /// <summary>
        /// 設定頁面上控制項是否啟用與轉譯(可見)。
        /// </summary>
        /// <param name="webControl">頁面控制項物件</param>
        /// <param name="isEnable">是否啟用</param>
        /// <param name="isVisible">是否轉譯(可見)</param>
        private static void SetWebControl(WebControl webControl, bool isEnable, bool isVisible)
        {
            webControl.Enabled = isEnable;
            webControl.Visible = isVisible;
        }
    }
}