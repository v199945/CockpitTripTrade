using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;
using Library.Component.Enums;
using Library.Module.FCDS;

namespace Library.Component.BLL
{
    /// <summary>
    /// 使用者群組之模組表單功能權限類別。
    /// </summary>
    public class GroupModuleFormFunction
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GroupModuleFormFunction));

        #region Property
        /// <summary>
        /// 群組之模組表單功能權限編號。
        /// </summary>
        public string IDBllGroupModuleFormFunction { get; set; }

        /// <summary>
        /// 版本值。
        /// </summary>
        public long? BranchID { get; set; }

        /// <summary>
        /// 版本號。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 模組表單功能編號。
        /// </summary>
        public string IDBllModuleFormFunction { get; set; }

        /// <summary>
        /// 使用者群組編號。
        /// </summary>
        public string IDBllGroup { get; set; }

        /// <summary>
        /// 建立者。
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 建立時間。
        /// </summary>
        public DateTime? CreateStamp { get; set; }

        /// <summary>
        /// 更新者。
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新時間。
        /// </summary>
        public DateTime? UpdateStamp { get; set; }

        public ModuleFormFunction ModuleFormFunction { get; set; }

        public ModuleForm ModuleForm { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public GroupModuleFormFunction()
        {

        }

        public GroupModuleFormFunction(string idBllGroupModuleFormFunction)
        {
            if (!string.IsNullOrEmpty(idBllGroupModuleFormFunction))
            {
                this.IDBllGroupModuleFormFunction = idBllGroupModuleFormFunction;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = null;
            if (!string.IsNullOrEmpty(this.IDBllGroupModuleFormFunction))
            {
                dt = FetchByIDBllGroupModuleFormFunction();
            }
            else
            {
                dt = FetchByIDBllModuleFormFunctinAndIDBllGroup();
            }

            if (dt.Rows.Count > 0)
            {
                SetGroupModuleFormFunction(dt.Rows[0]);
            }
        }

        private void SetGroupModuleFormFunction(DataRow dr)
        {
            this.IDBllGroupModuleFormFunction = dr["IDBllGroupModuleFormFunction"].ToString();

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();
            this.IDBllModuleFormFunction = dr["IDBllModuleFormFunction"].ToString();
            this.IDBllGroup = dr["IDBllGroup"].ToString();
            this.CreateBy = dr["CreateBy"].ToString();

            if (dr["CreateStamp"] != DBNull.Value && DateTime.TryParse(dr["CreateStamp"].ToString(), out _))
            {
                this.CreateStamp = DateTime.Parse(dr["CreateStamp"].ToString());
            }

            this.UpdateBy = dr["UpdateBy"].ToString();

            if (dr["UpdateStamp"] != DBNull.Value && DateTime.TryParse(dr["UpdateStamp"].ToString(), out _))
            {
                this.UpdateStamp = DateTime.Parse(dr["UpdateStamp"].ToString());
            }

            this.ModuleFormFunction = new ModuleFormFunction(this.IDBllModuleFormFunction);
            this.ModuleForm = new ModuleForm(this.ModuleFormFunction.IDBllModuleForm);
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT IDBllGroupModuleFormFunction, BranchID, Version, IDBllModuleFormFunction, IDBllGroup, CreateBy, CreateStamp, UpdateBy, UpdateStamp FROM fzdb.fztBllGroupMdlFrmFunction GMFF";
        }

        private DataTable FetchByIDBllGroupModuleFormFunction()
        {
            string sql = BuildFetchCommandString() + @" WHERE GMFF.IDBllGroupModuleFormFunction = :pIDBllGroupModuleFormFunction";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("ppIDBllGroupModuleFormFunction", this.IDBllGroupModuleFormFunction) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        private DataTable FetchByIDBllModuleFormFunctinAndIDBllGroup()
        {
            string sql = BuildFetchCommandString() + @" WHERE GMFF.IDBllModuleFormFunction = :pIDBllModuleFormFunction AND GMFF.IDBllGroup = :pIDBllGroup";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllModuleFormFunction", this.IDBllModuleFormFunction), new OracleParameter("pIDBllGroup", this.IDBllGroup) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public static object FetchByIDBllGroup(string idBllGroup, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE GMFF.IDBllGroup = :pIDBllGroup";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", idBllGroup) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    GroupModuleFormFunctionCollection col = new GroupModuleFormFunctionCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        GroupModuleFormFunction obj = new GroupModuleFormFunction();
                        obj.SetGroupModuleFormFunction(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

        public static object FetchAllModuleFormFunctionWithGroup(ReturnObjectTypeEnum rot)
        {
            string sql = @"SELECT MF.RootNameSpace, MF.IDBllModuleForm, MF.ModuleName, MF.FormName, MF.FormTitle,
                                  MFF.IDBllModuleFormFunction, MFF.IdFunction, MFF.FunctionCode,
                                  GMFF.IDBllGroupModuleFormFunction, GMFF.BranchID, GMFF.Version, GMFF.IDBllGroup, GMFF.CreateBy, GMFF.CreateStamp, GMFF.UpdateBy, GMFF.UpdateStamp
                           FROM   fzdb.fztbllmoduleform MF
                                  INNER JOIN fzdb.fztbllmoduleformfunction  MFF  ON MFF.Idbllmoduleform          = MF.Idbllmoduleform
                                  LEFT  JOIN fzdb.fztBllGroupMdlFrmFunction GMFF ON GMFF.Idbllmoduleformfunction = MFF.Idbllmoduleformfunction
                           WHERE  MF.RootNameSpace IN ('" + FcdsHelper.ROOT_NAME_SPACE + @"', 'BLL')";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    GroupModuleFormFunctionCollection col = new GroupModuleFormFunctionCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        GroupModuleFormFunction obj = new GroupModuleFormFunction();
                        obj.SetGroupModuleFormFunction(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

        public DataTable FetchLog()
        {
            string sql = @"SELECT IDBllGroupModuleFormFunction, BranchID, Version, IDBllModuleFormFunction, IDBllGroup, CreateBy, CreateStamp, UpdateBy, UpdateStamp FROM fzdb.fztBllGroupMdlFrmFunction_Log GMFF WHERE GMFF.IDBllGroupModuleFormFunction = :pIDBllGroupModuleFormFunction ORDER BY GMFF.BranchID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroupModuleFormFunction", this.IDBllGroupModuleFormFunction) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 儲存使用者群組之模組表單功能權限。
        /// </summary>
        /// <param name="enumPageMode">頁面模式列舉型態</param>
        /// <param name="isSaveLog">是否儲存日誌</param>
        /// <returns></returns>
        public bool Save(PageMode.PageModeEnum enumPageMode, bool isSaveLog)
        {
            bool result = false;
            switch (enumPageMode)
            {
                case PageMode.PageModeEnum.Create:
                    result = Insert();
                    break;

                case PageMode.PageModeEnum.Edit:
                    if (isSaveLog)
                    {
                        this.BranchID = long.Parse(Document.FetchNextDocNo("ID_Branch", Document.DocNoStatusEnum.Reserve));
                        this.Version = Branch.CalcNextVersion(this.Version, Branch.BranchTypeEnum.Iteration);
                    }
                    result = Update();
                    break;

                default:
                    break;
            }

            if (result && isSaveLog)
            {
                result = SaveLog();
            }

            return result;
        }

        /// <summary>
        /// 新增使用者群組之模組表單功能權限。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            if (string.IsNullOrEmpty(this.IDBllGroupModuleFormFunction))
            {
                this.IDBllGroupModuleFormFunction = Document.FetchNextDocNo("ID_BllGroupModuleFormFunction", Document.DocNoStatusEnum.Reserve);
            }

            string sql = @"INSERT INTO fzdb.fztBllGroupMdlFrmFunction(IDBllGroupModuleFormFunction, BranchID, Version, IDBllModuleFormFunction, IDBllGroup, CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDBllGroupModuleFormFunction, :pBranchID, :pVersion, :pIDBllModuleFormFunction, :pIDBllGroup, :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroupModuleFormFunction", this.IDBllGroupModuleFormFunction),
                                                                        new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pIDBllModuleFormFunction", this.IDBllModuleFormFunction), new OracleParameter("pIDBllGroup", this.IDBllGroup),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新使用者群組之模組表單功能權限。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztBllGroupMdlFrmFunction
                           SET    BranchID = :pBranchID, Version = :pVersion,
                                  IDBllModuleFormFunction = :pIDBllModuleFormFunction, IDBllGroup = :pIDBllGroup,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDBllGroupModuleFormFunction = :pIDBllGroupModuleFormFunction";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pIDBllGroupModuleFormFunction", this.IDBllGroupModuleFormFunction), new OracleParameter("pIDBllGroup", this.IDBllGroup),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIIDBllGroupModuleFormFunction", this.IDBllGroupModuleFormFunction)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存使用者群組之模組表單功能權限日誌。
        /// </summary>
        /// <returns></returns>
        private bool SaveLog()
        {
            string sql = @"INSERT INTO fzdb.fztBllGroupMdlFrmFunction_Log
                           SELECT * FROM fzdb.fztBllGroupMdlFrmFunction WHERE IDBllGroupModuleFormFunction = :pIDBllGroupModuleFormFunction";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroupModuleFormFunction", this.IDBllGroupModuleFormFunction) };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存使用者群組之模組表單功能權限集合物件。
        /// </summary>
        /// <param name="idBllGroup">群組編號</param>
        /// <param name="col">群組與使用者集合物件</param>
        /// <param name="isSaveLog">是否儲存日誌</param>
        /// <returns></returns>
        public static bool SaveGroupModuleFormFunctionCollection(string idBllGroup, GroupModuleFormFunctionCollection col, bool isSaveLog)
        {
            if (DeleteByIDBllGroup(idBllGroup))
            {
                foreach (GroupModuleFormFunction obj in col)
                {
                    if (!obj.Save(PageMode.PageModeEnum.Create, isSaveLog))
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 依據群組編號(IDBllGroup)刪除使用者群組與使用者。
        /// </summary>
        /// <param name="idBllGroup">群組編號</param>
        /// <returns></returns>
        private static bool DeleteByIDBllGroup(string idBllGroup)
        {
            string sql = @"DELETE FROM fzdb.fztBllGroupMdlFrmFunction GMF WHERE GMF.IDBllGroup = :pIDBllGroup";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", idBllGroup) };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result >= 0;
        }
    }

    /// <summary>
    /// 使用者群組之模組表單功能權限集合類別。
    /// </summary>
    public class GroupModuleFormFunctionCollection : List<GroupModuleFormFunction>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public GroupModuleFormFunctionCollection()
        {

        }
    }
}
