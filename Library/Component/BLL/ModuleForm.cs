using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;
using Library.Component.Enums;
using Library.Module.FCDS;

namespace Library.Component.BLL
{
    /// <summary>
    /// 模組表單類別。
    /// </summary>
    public class ModuleForm
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ModuleForm));

        #region Property
        /// <summary>
        /// 模組表單編號。
        /// </summary>
        public string IDBllModuleForm { get; set; }

        /// <summary>
        /// 版本值。
        /// </summary>
        public long BranchID { get; set; }

        /// <summary>
        /// 版本號。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 命名空間。
        /// </summary>
        public string RootNameSpace { get; set; }

        /// <summary>
        /// 模組名稱。
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 模組標題。
        /// </summary>
        public string ModuleTitle { get; set; }

        /// <summary>
        /// 表單名稱。
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 表單標題。
        /// </summary>
        public string FormTitle { get; set; }

        /// <summary>
        /// 表單 URL。
        /// </summary>
        public string FormUrl { get; set; }

        /// <summary>
        /// 表單主流程編號。
        /// </summary>
        public string ProID { get; set; }

        /// <summary>
        /// 表單初始化流程編號。
        /// </summary>
        public string InitialProID { get; set; }

        /// <summary>
        /// 表單使用到的 ASP.NET 使用者控制項。
        /// </summary>
        public string UserControls { get; set; }

        /// <summary>
        /// 表單流程所有者員工編號(業管部門)
        /// </summary>
        public string ProcessOwner { get; set; }

        /// <summary>
        /// 表單流程系統管理者員工編號。
        /// </summary>
        public string SystemAdministrator { get; set; }

        /// <summary>
        /// 建立者員工編號。
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 建立時間。
        /// </summary>
        public DateTime CreateStamp { get; set; }

        /// <summary>
        /// 更新者員工編號。
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新時間。
        /// </summary>
        public DateTime UpdateStamp { get; set; }

        public List<string> UserControlList
        {
            get
            {
                if (string.IsNullOrEmpty(this.UserControls))
                {
                    return null;
                }
                else
                {
                    string[] aryString = this.UserControls.Split(',');;
                    return new List<string>(aryString);
                }
            }
        }

        /// <summary>
        /// 模組表單功能權限集合物件。
        /// </summary>
        public ModuleFormFunctionCollection ModuleFormFunctions { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ModuleForm()
        {

        }

        public ModuleForm(string idBllModuleForm)
        {
            if (!string.IsNullOrEmpty(idBllModuleForm))
            {
                this.IDBllModuleForm = idBllModuleForm;

                Load();
            }
        }
        /// <summary>
        /// 來自[fzdb.fztbllmoduleform]
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="formName"></param>
        public ModuleForm(string moduleName, string formName)
        {
            if (!string.IsNullOrEmpty(moduleName) && !string.IsNullOrEmpty(formName))
            {
                this.ModuleName = moduleName;
                this.FormName = formName;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = null;
            if (!string.IsNullOrEmpty(this.IDBllModuleForm))
            {
                dt = FetchByIDBllModuleForm();
            }
            else if (!string.IsNullOrEmpty(this.ModuleName) && !string.IsNullOrEmpty(this.FormName))
            {
                dt = FetchByModuleNameAndFormName();
            }

            if (dt.Rows.Count > 0)
            {
                SetModuleForm(dt.Rows[0]);
            }
        }

        private void SetModuleForm(DataRow dr)
        {
            this.IDBllModuleForm = dr["IDBllModuleForm"].ToString();
            this.BranchID = long.Parse(dr["BranchID"].ToString());
            this.Version = dr["Version"].ToString();
            this.RootNameSpace = dr["RootNameSpace"].ToString();
            this.ModuleName = dr["ModuleName"].ToString();
            this.ModuleTitle = dr["ModuleTitle"].ToString();
            this.FormName = dr["FormName"].ToString();
            this.FormTitle = dr["FormTitle"].ToString();
            this.FormUrl = dr["FormUrl"].ToString();
            this.ProID = dr["ProID"].ToString();
            this.InitialProID = dr["InitialProID"].ToString();
            this.UserControls = dr["UserControls"].ToString();
            this.ProcessOwner = dr["ProcessOwner"].ToString();
            this.SystemAdministrator = dr["SystemAdministrator"].ToString();
            this.CreateBy = dr["CreateBy"].ToString();
            this.CreateStamp = DateTime.Parse(dr["CreateStamp"].ToString());
            this.UpdateBy = dr["UpdateBy"].ToString();
            this.UpdateStamp = DateTime.Parse(dr["UpdateStamp"].ToString());

            this.ModuleFormFunctions = ModuleFormFunction.FetchByIDBllModuleForm(this.IDBllModuleForm, Enums.ReturnObjectTypeEnum.Collection) as ModuleFormFunctionCollection;
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT IDBllModuleForm, BranchID, Version, RootNameSpace, ModuleName, ModuleTitle, FormName, FormTitle, FormUrl, ProID, InitialProID, UserControls, ProcessOwner, SystemAdministrator, CreateBy, CreateStamp, UpdateBy, UpdateStamp FROM fzdb.fztbllmoduleform";
        }

        private DataTable FetchByIDBllModuleForm()
        {
            string sql = BuildFetchCommandString() + @" WHERE IDBllModuleForm = :pIDBllModuleForm";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllModuleForm", this.IDBllModuleForm) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public DataTable FetchByModuleNameAndFormName()
        {
            string sql = BuildFetchCommandString() + @" WHERE ModuleName = :pModuleName AND FormName = :pFormName";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pModuleName", this.ModuleName), new OracleParameter("pFormName", this.FormName) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 取得表單流程所有者電子郵件。
        /// </summary>
        /// <returns></returns>
        public string GetProcessOwnerMail()
        {
            return this.ProcessOwner + @"@china-airlines.com";
        }

        /// <summary>
        /// 取得表單流程系統管理者電子郵件。
        /// </summary>
        /// <returns></returns>
        public string GetSystemAdministratorMail()
        {
            return this.SystemAdministrator + @"@china-airlines.com";
        }
    }

    /// <summary>
    /// 模組表單集合類別。
    /// </summary>
    public class ModuleFormCollection : List<ModuleForm>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ModuleFormCollection()
        {

        }
    }
}