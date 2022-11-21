using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;
using Library.Component.Utility;
using Library.Module.FZDB;
using Library.Module.HRDB;

namespace Library.Component.BLL
{
    /// <summary>
    /// 日誌類別。
    /// </summary>
    public class Log
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Log));

        #region Property
        /// <summary>
        /// 日誌編號。
        /// </summary>
        public string IDLog { get; set; }

        /// <summary>
        /// ASP.NET Web 應用程式名稱。
        /// </summary>
        public string WebApplication { get; set; }

        /// <summary>
        /// Session 權杖。
        /// </summary>

        public string SessionToken { get; set; }


        /// <summary>
        /// 登入時間。
        /// </summary>
        public DateTime? LoginDateTime { get; set; }

        /// <summary>
        /// 日誌主旨。
        /// </summary>
        public string LogSubject { get; set; }

        /// <summary>
        /// 日誌明細。
        /// </summary>
        public string LogDetail { get; set; }

        /// <summary>
        /// 是否成功。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 使用者帳號。
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 使用者帳號是否為管理員。
        /// </summary>
        public bool? IsAdmin { get; set; }

        /// <summary>
        /// 使用者瀏覽器的原始使用者代理字串。
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 使用者 IP 位址。
        /// </summary>
        public string UserIP { get; set; }

        /// <summary>
        /// 使用者連結伺服器名稱。
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 使用者群組集合物件。
        /// </summary>
        public UserGroupCollection UserGroups { get; set; }

        /// <summary>
        /// 使用者模組表單功能權限集合物件。
        /// </summary>
        public UserModuleFormFunctionCollection UserModuleFormFunctions { get; set; }

        /// <summary>
        /// 使用者模組表單功能權限集合物件。
        /// </summary>
        //public ModuleFormFunctionCollection ModuleFormFunctions { get; set; }

        /// <summary>
        /// 使用者之 HRDB 員工基本資料物件。
        /// </summary>
        public HrVEgEmploy Employee { get; set; }

        /// <summary>
        /// 使用者之 AIMS [CI].[VvCrewDb] 組員基本資料物件。
        /// </summary>
        public CIvvCrewDb CrewDb { get; set; }

        /// <summary>
        /// 日誌建立者。
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 日誌建立時間。
        /// </summary>
        public DateTime? CreateStamp { get; set; }

        /// <summary>
        /// 日誌更新者。
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 日誌建立時間。
        /// </summary>
        public DateTime UpdateStamp { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public Log()
        {

        }

        /// <summary>
        /// 取得日誌物件並設定日誌物件[LogSubject]、[LogDetail]、[IsSuccess]等屬性值。
        /// 成功登入系統後方可呼叫此方法。
        /// </summary>
        /// <param name="logSubject">日誌主旨，即欲記錄之動作名稱</param>
        /// <param name="logDetial">日誌內容，即欲記錄動作名稱之相關參數</param>
        /// <param name="result">布林值結果，即欲記錄動作之執行結果是否成功</param>
        /// <returns></returns>
        public static Log GetLogWithLoginSuccessfully(string logSubject, string logDetial, bool result)
        {
            Log log = new Log()
            {
                WebApplication = LoginSession.GetLoginSession().WebApplicationEnum.ToString(),
                SessionToken = LoginSession.GetLoginSession().SessionToken,
                LoginDateTime = LoginSession.GetLoginSession().LoginDateTime,
                LogSubject = logSubject,
                LogDetail = logDetial,
                IsSuccess = result,
                UserID = LoginSession.GetLoginSession().UserID,
                IsAdmin = LoginSession.GetLoginSession().IsAdmin,
                UserAgent = LoginSession.GetLoginSession().UserAgent,
                UserIP = LoginSession.GetLoginSession().ClientIP,
                ServerName = RequestUtility.ServerName,
                UserGroups = LoginSession.GetLoginSession().UserGroups,
                UserModuleFormFunctions = LoginSession.GetLoginSession().UserModuleFormFunctions,
                //ModuleFormFunctions = LoginSession.GetLoginSession().ModuleFormFunctions,
                Employee = LoginSession.GetLoginSession().Employee,
                CrewDb = LoginSession.GetLoginSession().CrewDb,
                CreateBy = Config.SYSTEM_ADMIN,
                CreateStamp = DateTime.Now,
                UpdateBy = Config.SYSTEM_ADMIN,
                UpdateStamp = DateTime.Now
            };

            return log;
        }

        /// <summary>
        /// 儲存日誌。
        /// </summary>
        /// <param name="enumPageMode">頁面模式列舉型態</param>
        /// <returns></returns>
        public bool Save(PageMode.PageModeEnum enumPageMode)
        {
            bool result = false;
            switch (enumPageMode)
            {
                case PageMode.PageModeEnum.Create:
                    result = Insert();
                    break;

                case PageMode.PageModeEnum.Edit:
                case PageMode.PageModeEnum.Task:
                    result = Update();
                    break;

                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// 新增日誌。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            if (string.IsNullOrEmpty(this.IDLog))
            {
                this.IDLog = Document.FetchNextDocNo("ID_Log", Document.DocNoStatusEnum.Reserve);
            }

            string sql = @"INSERT INTO fzdb.fztBllLog(IDLog, WebApplication, SessionToken, LoginDateTime, LogSubject, LogDetail, IsSuccess,
                                                      UserID, IsAdmin, UserAgent, UserIP, ServerName, UserGroups, UserModuleFormFunctions, Employee, CrewDb,
                                                      CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDLog, :pWebApplication, :pSessionToken, :pLoginDateTime, :pLogSubject, :pLogDetail, :pIsSuccess,
                                   :pUserID, :pIsAdmin, :pUserAgent, :pUserIP, :pServerName, :pUserGroups, :pUserModuleFormFunctions, :pEmployee, :pCrewDb,
                                   :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDLog", this.IDLog), new OracleParameter("pWebApplication", this.WebApplication),
                                                                        new OracleParameter("pSessionToken", this.SessionToken), new OracleParameter("pLoginDateTime", this.LoginDateTime.HasValue ? this.LoginDateTime.Value : null as DateTime?),
                                                                        new OracleParameter("pLogSubject", this.LogSubject),new OracleParameter("pLogDetail", this.LogDetail),
                                                                        new OracleParameter("pIsSuccess", this.IsSuccess.ToString()), new OracleParameter("pUserID", this.UserID),
                                                                        new OracleParameter("pIsAdmin", this.IsAdmin.HasValue ? this.IsAdmin.Value.ToString() : null),
                                                                        new OracleParameter("pUserAgent", this.UserAgent), new OracleParameter("pUserIP", this.UserIP),
                                                                        new OracleParameter("pServerName", this.ServerName),
                                                                        new OracleParameter("pUserGroups", JsonConvert.SerializeObject(this.UserGroups)),
                                                                        new OracleParameter("pUserModuleFormFunctions", JsonConvert.SerializeObject(this.UserModuleFormFunctions)),
                                                                        new OracleParameter("pEmployee", JsonConvert.SerializeObject(this.Employee)),
                                                                        new OracleParameter("pCrewDb", JsonConvert.SerializeObject(this.CrewDb)),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新日誌。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztBllLog
                           SET    WebApplication = :pWebApplication, SessionToken = :pSessionToken, LoginDateTime = :pLoginDateTime, LogSubject = :pLogSubject, LogDetail = :pLogDetail, IsSuccess = :pIsSuccess, UserID = :pUserID, IsAdmin = :pIsAdmin,
                                  UserAgent = :pUserAgent, UserIP = :pUserIP, ServerName = :pServerName, 
                                  UserGroups = :pUserGroups, UserModuleFormFunctions = :pUserModuleFormFunctions, Employee = :pEmployee, CrewDb = :pCrewDb,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDLog = :pIDLog";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pWebApplication", this.WebApplication), new OracleParameter("pSessionToken", this.SessionToken),
                                                                        new OracleParameter("pLoginDateTime", this.LoginDateTime.HasValue ? this.LoginDateTime.Value : null as DateTime?),
                                                                        new OracleParameter("pLogSubject", this.LogSubject), new OracleParameter("pLogDetail", this.LogDetail),
                                                                        new OracleParameter("pIsSuccess", this.IsSuccess.ToString()),
                                                                        new OracleParameter("pUserID", this.UserID), new OracleParameter("pIsAdmin", this.IsAdmin.HasValue ? this.IsAdmin.Value.ToString() : null),
                                                                        new OracleParameter("pUserAgent", this.UserAgent), new OracleParameter("pUserIP", this.UserIP),
                                                                        new OracleParameter("pServerName", this.ServerName),
                                                                        new OracleParameter("pUserGroups", JsonConvert.SerializeObject(this.UserGroups)),
                                                                        new OracleParameter("pUserModuleFormFunctions", JsonConvert.SerializeObject(this.UserModuleFormFunctions)),
                                                                        new OracleParameter("pEmployee", JsonConvert.SerializeObject(this.Employee)),
                                                                        new OracleParameter("pCrewDb", JsonConvert.SerializeObject(this.CrewDb)),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIDLog", this.IDLog)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

    }
}
