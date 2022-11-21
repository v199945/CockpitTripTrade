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

namespace Library.Component.BLL
{
    /// <summary>
    /// 使用者模組表單功能權限類別。
    /// </summary>
    public class UserModuleFormFunction
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UserGroup));

        #region Porperty
        /// <summary>
        /// 模組表單功能權限編號。
        /// </summary>
        public string IDBllModuleFormFunction { get; set; }

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
        public string Description { get; set; }

        /// <summary>
        /// 群組之模組表單功能權限編號。
        /// </summary>
        public string IDBllGroupModuleFormFunction { get; set; }

        /// <summary>
        /// 使用者群組編號。
        /// </summary>
        public string IDBllGroup { get; set; }
        #endregion

        private UserModuleFormFunction()
        {

        }

        private void SetUserModuleFormFunction(DataRow dr)
        {
            this.IDBllModuleFormFunction = dr["IDBllModuleFormFunction"].ToString();
            this.IDBllModuleForm = dr["IDBllModuleForm"].ToString();

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
            this.Description = dr["Description_"].ToString();
            this.IDBllGroupModuleFormFunction = dr["IDBllGroupModuleFormFunction"].ToString();
            this.IDBllGroup = dr["IDBllGroup"].ToString();
        }

        /// <summary>
        /// 擷取使用者模組表單功能權限。
        /// </summary>
        /// <param name="unitCd">使用者部門代碼</param>
        /// <param name="idUser">使用者員工編號</param>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchByUnitCdAndIDUser(string unitCd, string idUser, ReturnObjectTypeEnum rot)
        {
            string sql = @"SELECT MFF.IDBllModuleFormFunction, MFF.IDBllModuleForm, MFF.IDFunction, MFF.FunctionCode, MFF.Description_
                                  ,GMFF.IDBllGroupModuleFormFunction, GMFF.IDBllGroup
                           FROM   fzdb.fztbllmoduleformfunction             MFF
                                  LEFT JOIN fzdb.fztbllgroupmdlfrmfunction  GMFF ON MFF.IDBllModuleFormFunction = GMFF.IDBllModuleFormFunction
                                  LEFT JOIN fzdb.fztbllgroupunit            GUT  ON GMFF.IDBllGroup             = GUT.IDBllGroup
                           WHERE  GUT.UnitCd = :pUnitCd
                           UNION ALL
                           SELECT MFF.IDBllModuleFormFunction, MFF.IDBllModuleForm, MFF.IDFunction, MFF.FunctionCode, MFF.Description_
                                  ,GMFF.IDBllGroupModuleFormFunction, GMFF.IDBllGroup
                           FROM   fzdb.fztbllmoduleformfunction             MFF
                                  LEFT JOIN fzdb.fztbllgroupmdlfrmfunction  GMFF ON MFF.IDBllModuleFormFunction = GMFF.IDBllModuleFormFunction
                                  LEFT JOIN fzdb.fztbllgroupuser            GUS     ON GMFF.IDBllGroup          = GUS.IDBllGroup
                           WHERE  GUS.IDUser = :pIDUser
                           ORDER BY IDBllModuleForm, IDFunction";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pUnitCd", unitCd), new OracleParameter("pIDUser", idUser) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    UserModuleFormFunctionCollection col = new UserModuleFormFunctionCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        UserModuleFormFunction obj = new UserModuleFormFunction();
                        obj.SetUserModuleFormFunction(dr);
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
    public class UserModuleFormFunctionCollection : List<UserModuleFormFunction>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public UserModuleFormFunctionCollection()
        {

        }
    }
}
