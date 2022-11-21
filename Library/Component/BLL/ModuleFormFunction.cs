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
    /// 模組表單功能權限類別。
    /// </summary>
    public partial class ModuleFormFunction
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ModuleFormFunction));

        #region Property
        /// <summary>
        /// 模組表單功能權限編號。
        /// </summary>
        public string IDBllModuleFormFunction { get; set; }

        /// <summary>
        /// 版本值。
        /// </summary>
        public long? BranchID { get; set; }

        /// <summary>
        /// 版本號。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 模組表單編號。
        /// </summary>
        public string IDBllModuleForm { get; set; }

        /// <summary>
        /// 表單功能編號。
        /// </summary>
        public string IDFunction { get; set; }

        /// <summary>
        /// 表單功能列舉型態。
        /// </summary>
        public IDFunctionEnum? IDFunctionEnum { get; set; }

        /// <summary>
        /// 表單功能代碼。
        /// </summary>
        public string FunctionCode { get; set; }

        /// <summary>
        /// 表單功能描述。
        /// </summary>
        public string Description_ { get; set; }

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

        /// <summary>
        /// 模組標題。
        /// </summary>
        public string ModuleTitle { get; set; }

        /// <summary>
        /// 表單標題。
        /// </summary>
        public string FormTitle { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ModuleFormFunction()
        {

        }

        public ModuleFormFunction(string idBllModuleFormFunction)
        {
            if (!string.IsNullOrEmpty(idBllModuleFormFunction))
            {
                this.IDBllModuleFormFunction = idBllModuleFormFunction;

                Load();
            }
        }

        public ModuleFormFunction(string idBllModuleForm, IDFunctionEnum idFunctionEnum)
        {
            if (!string.IsNullOrEmpty(idBllModuleForm))
            {
                this.IDBllModuleForm = idBllModuleForm;
                this.IDFunctionEnum = idFunctionEnum;
                //this.IDFunction = idFunctionEnum...ToString();

                Load();
            }
        }

        public ModuleFormFunction(string idBllModuleForm, string idFunction)
        {
            if (!string.IsNullOrEmpty(idBllModuleForm) && !string.IsNullOrEmpty(idFunction))
            {
                this.IDBllModuleForm = idBllModuleForm;
                this.IDFunction = idFunction;

                if (Enum.TryParse<IDFunctionEnum>(idFunction, out _))
                {
                    this.IDFunction = idFunction;

                    this.IDFunctionEnum = Enum.Parse(typeof(IDFunctionEnum), idFunction) as IDFunctionEnum?;
                    if (!Enum.IsDefined(typeof(IDFunctionEnum), this.IDFunctionEnum))
                    {
                        this.IDFunctionEnum = null;
                    }
                }

                Load();
            }

        }

        private void Load()
        {
            DataTable dt = null;
            if (!string.IsNullOrEmpty(this.IDBllModuleFormFunction))
            {
                dt = FetchByIDBllModuleFormFunction();
            }
            else
            {
                dt = FetchByIDBllModuleFormAndIDFunction();
            }

            if (dt.Rows.Count > 0)
            {
                SetModuleFormFunction(dt.Rows[0]);
            }
        }

        private void SetModuleFormFunction(DataRow dr)
        {
            if (dr["IDBllModuleFormFunction"] != DBNull.Value)
            {
                this.IDBllModuleFormFunction = dr["IDBllModuleFormFunction"].ToString();
            }

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();

            if (dr["IDBllModuleForm"] != DBNull.Value)
            {
                this.IDBllModuleForm = dr["IDBllModuleForm"].ToString();
            }

            if (dr["IDFunction"] != DBNull.Value && Enum.TryParse<IDFunctionEnum>(dr["IDFunction"].ToString(), out _))
            {
                this.IDFunction = dr["IDFunction"].ToString();

                this.IDFunctionEnum = Enum.Parse(typeof(IDFunctionEnum), dr["IDFunction"].ToString()) as IDFunctionEnum?;
                if (!Enum.IsDefined(typeof(IDFunctionEnum), this.IDFunctionEnum))
                {
                    this.IDFunctionEnum = null;
                }
            }

            this.FunctionCode = dr["FunctionCode"].ToString();

            this.Description_ = dr["Description_"].ToString();
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

            this.ModuleTitle = dr["ModuleTitle"].ToString();
            this.FormTitle = dr["FormTitle"].ToString();
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT MFF.IDBllModuleFormFunction, MFF.BranchID, MFF.Version, MFF.IDBllModuleForm, MFF.IDFunction, MFF.FunctionCode, MFF.Description_, MFF.CreateBy, MFF.CreateStamp, MFF.UpdateBy, MFF.UpdateStamp
                            ,MF.ModuleName, MF.ModuleTitle, MF.FormName, MF.FormTitle
                     FROM   fzdb.fztbllmoduleformfunction MFF
                            INNER JOIN fzdb.fztbllmoduleform MF ON MFF.IDBllModuleForm = MF.IDBllModuleForm";
        }
        /// <summary>
        /// 建構 SQL 擷取之條件指令字串。
        /// </summary>
        /// <returns></returns>

        private static string BuildConditionCommandString()
        {
            return @" WHERE  MF.RootNameSpace IN ('" + FcdsHelper.ROOT_NAME_SPACE + @"', 'BLL')";
        }

        private DataTable FetchByIDBllModuleFormFunction()
        {
            string sql = BuildFetchCommandString() + @" WHERE MFF.IDBllModuleFormFunction = :pIDBllModuleFormFunction";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllModuleFormFunction", this.IDBllModuleFormFunction) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        private DataTable FetchByIDBllModuleFormAndIDFunction()
        {
            string sql = BuildFetchCommandString() + @" WHERE MFF.IDBllModuleForm = :pIDBllModuleForm AND MFF.IDFunction = :pIDFunction";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllModuleForm", this.IDBllModuleForm), new OracleParameter("pIDFunction", this.IDFunction) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public static object FetchByIDBllModuleForm(string idBllModuleForm, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE MFF.IDBllModuleForm = :pIDBllModuleForm";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllModuleForm", idBllModuleForm) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    ModuleFormFunctionCollection col = new ModuleFormFunctionCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ModuleFormFunction obj = new ModuleFormFunction();
                        obj.SetModuleFormFunction(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

        /// <summary>
        /// No Usage
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="unitCd"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static object FetchByIDUserAndUnitCd(string idUser, string unitCd, ReturnObjectTypeEnum rot)
        {
            string sql = @"SELECT MFF.IDBllModuleForm, MFF.IDFunction, MFF.FunctionCode, MFF.BranchID, MFF.Version, MFF.Description_--, MFF.Comments, MFF.IDBllGroup
                                  ,MFF.CreateBy, MFF.CreateStamp, MFF.UpdateBy, MFF.UpdateStamp
                           FROM   fzdb.fztbllmoduleformfunction MFF
                                  LEFT JOIN fzdb.fztbllgroupunit GUT ON MFF.IDBllGroup = GUT.IDBllGroup
                           WHERE  GUT.UnitCd = :pUnitCd
                           UNION ALL
                           SELECT MFF.IDBllModuleForm, MFF.IDFunction, MFF.FunctionCode, MFF.BranchID, MFF.Version, MFF.Description_--, MFF.Comments, MFF.IDBllGroup
                                  ,MFF.CreateBy, MFF.CreateStamp, MFF.UpdateBy, MFF.UpdateStamp
                           FROM   fzdb.fztbllmoduleformfunction MFF
                                  LEFT JOIN fzdb.fztbllgroupuser GUS ON MFF.IDBllGroup = GUS.IDBllGroup
                           WHERE  GUS.IDUser = :pIDUser
                           ORDER BY IDBllModuleForm, IDFunction";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDUser", idUser), new OracleParameter("pUnitCd", unitCd) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    ModuleFormFunctionCollection col = new ModuleFormFunctionCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ModuleFormFunction obj = new ModuleFormFunction();
                        obj.SetModuleFormFunction(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

        public static object FetchAllModuleFormFunction(ReturnObjectTypeEnum rot)
        {
            /*
             * @"SELECT MF.ModuleName, MF.ModuleTitle, MF.FormName, MF.FormTitle
                                  ,MFF.IDBllModuleFormFunction, MFF.BranchID, MFF.Version, MFF.IDBllModuleForm, MFF.IDFunction, MFF.FunctionCode, MFF.Description_, MFF.CreateBy, MFF.CreateStamp, MFF.UpdateBy, MFF.UpdateStamp
                           FROM   fzdb.fztbllmoduleform MF
                                  INNER JOIN fzdb.fztbllmoduleformfunction MFF ON MF.IdBllModuleForm = MFF.IdBllModuleForm
                           WHERE  MF.RootNameSpace IN ('" + FcdsHelper.ROOT_NAME_SPACE + @"', 'BLL')
                           ORDER BY MFF.IdBllModuleForm, MFF.IdBllModuleFormFunction"
            */
            string sql = BuildFetchCommandString() + BuildConditionCommandString() + @" ORDER BY MFF.IdBllModuleForm, MFF.IdBllModuleFormFunction";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    ModuleFormFunctionCollection col = new ModuleFormFunctionCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ModuleFormFunction obj = new ModuleFormFunction();
                        obj.SetModuleFormFunction(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

    }

    /// <summary>
    /// 使用者模組表單功能權限集合類別。
    /// </summary>
    public class ModuleFormFunctionCollection : List<ModuleFormFunction>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ModuleFormFunctionCollection()
        {

        }
    }
}
