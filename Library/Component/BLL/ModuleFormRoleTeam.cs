using System;
using System.Collections.Generic;
using System.Data;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Component.BLL
{
    /// <summary>
    /// 表單角色團隊類別。
    /// </summary>
    public class ModuleFormRoleTeam
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ModuleFormRoleTeam));

        #region Property
        public string ModuleNmae { get; set; }

        public string FormName { get; set; }

        public string IDFlowTeam { get; set; }

        public long BranchID { get; set; }

        public string IDRole { get; set; }

        public string EmployID { get; set; }

        public string CreateBy { get; set; }

        public DateTime? CreateStamp { get; set; }

        public string UpdateBy { get; set; }

        public DateTime? UpdateStamp { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ModuleFormRoleTeam()
        {

        }

        public ModuleFormRoleTeam(string moduleName, string formName, string idFlowTeam, string idRole)
        {
            if (moduleName != null && formName != null && idFlowTeam != null && idRole != null)
            {
                this.ModuleNmae = moduleName;
                this.FormName = formName;
                this.IDFlowTeam = idFlowTeam;
                this.IDRole = IDRole;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByIDFlowTeamAndIDRole();
            if (dt.Rows.Count > 0)
            {
                SetFormRoleTeam(dt.Rows[0]);
            }
        }

        private void SetFormRoleTeam(DataRow dr)
        {
            this.IDFlowTeam = dr["IDFlowTeam"].ToString();

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.IDRole = dr["IDRole"].ToString();
            this.EmployID = dr["EmployID"].ToString();
            this.CreateBy = dr["CreateBy"].ToString();

            if (dr["CreateStamp"] != DBNull.Value)
            {
                this.CreateStamp = (DateTime)dr["CreateStamp"];
            }

            this.UpdateBy = dr["UpdateBy"].ToString();

            if (dr["UpdateStamp"] != DBNull.Value)
            {
                this.UpdateStamp = (DateTime)dr["UpdateStamp"];
            }
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT IDFlowTeam, BranchID, IDRole, EmployID, CreateBy, CreateStamp, UpdateBy, UpdateStamp FROM fzdb.fzt{0}_role_team";
        }

        private DataTable FetchByIDFlowTeamAndIDRole()
        {
            string sql = string.Format(BuildFetchCommandString() + @" WHERE IDFlowTeam = :pIDFlowTeam", this.FormName.ToLower());
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFlowTeam", this.IDFlowTeam) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public static object FetchByIDFlowTeam(string moduleName, string formName, string idFlowTeam, ReturnObjectTypeEnum rot)
        {
            string sql = string.Format(BuildFetchCommandString() + @" WHERE IDFlowTeam = :pIDFlowTeam", formName.ToLower());
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFlowTeam", idFlowTeam) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    ModuleFormRoleTeamCollection col = new ModuleFormRoleTeamCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ModuleFormRoleTeam obj = new ModuleFormRoleTeam();
                        obj.ModuleNmae = moduleName;
                        obj.FormName = formName;
                        obj.SetFormRoleTeam(dr);
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
        /// 擷取表單團隊日誌。
        /// </summary>
        /// <param name="moduleName">模組名稱</param>
        /// <param name="formName">表單名稱</param>
        /// <param name="idFlowTeam">表單編號</param>
        /// <returns></returns>
        public static DataTable FetchLog(string moduleName, string formName, string idFlowTeam)
        {
            string sql = string.Format(@"SELECT IDFlowTeam, BranchID, IDRole, EmployID, CreateBy, CreateStamp, UpdateBy, UpdateStamp
                                         FROM   fzdb.fzt{0}_role_team_log
                                         WHERE  IDFlowTeam = :pIDFlowTeam
                                         ORDER BY BranchID"
                                       , formName.ToLower()
                                      );
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFlowTeam", idFlowTeam) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// Non Useage。
        /// </summary>
        /// <param name="col"></param>
        /// <param name="enumPageMode"></param>
        /// <param name="isSaveLog"></param>
        /// <returns></returns>
        public bool SaveCollection(ModuleFormRoleTeamCollection col, PageMode.PageModeEnum enumPageMode, bool isSaveLog)
        {
            foreach (ModuleFormRoleTeam obj in col)
            {
                if (!obj.Save(enumPageMode, isSaveLog))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 儲存表單團隊。
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
                case PageMode.PageModeEnum.Task:
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
        /// 新增表單團隊。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            string sql = string.Format(@"INSERT INTO fzdb.fzt{0}_role_team(IDFlowTeam, BranchID, IDRole, EmployID, CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                                         VALUES (:pIDFlowTeam, :pBranchID, :pIDRole, :pEmployID, :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)"
                                        , this.FormName.ToLower()
                                        );

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFlowTeam", this.IDFlowTeam), new OracleParameter("pBranchID", this.BranchID),
                                                                        new OracleParameter("pIDRole", this.IDRole), new OracleParameter("pEmployID", this.EmployID),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新表單團隊。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            // 20221101 648267:[Insert] or [Update] determination
            // 20221104 648267:Add [Delete] function when optional selection been selected
            string sql="";
            if (CheckNotifyRecordExist(this.IDFlowTeam, this.IDRole))
            {
                // 20221104 648267:this.EmployID == "000000"，代表現有資料要清空
                if (this.EmployID == "000000")
                {
                    sql = string.Format(@"DELETE fzdb.fzt{0}_role_team WHERE IDFlowTeam = :pIDFlowTeam AND IDRole = :pIDRole ", this.FormName.ToLower());
                }
                else
                {
                    sql = string.Format(@"UPDATE fzdb.fzt{0}_role_team
                                         SET    BranchID = :pBranchID, IDRole = :pIDRole, EmployID = :pEmployID,
                                                UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                                         WHERE  IDFlowTeam = :pIDFlowTeam AND IDRole = :pIDRole"
                                        , this.FormName.ToLower()
                                      );
                }
            }
            else
            {
                // 20221104 648267:this.EmployID = "000000"，代表該選項未選
                if (this.EmployID != "000000")
                {
                    sql = string.Format(@"INSERT INTO fzdb.fzt{0}_role_team(IDFlowTeam, BranchID, IDRole, EmployID, CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                                         VALUES (:pIDFlowTeam, :pBranchID, :pIDRole, :pEmployID, :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)"
                                        , this.FormName.ToLower()
                                        );
                }
                else
                {
                    return true;
                }
            }

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pIDRole", this.IDRole),
                                                                        new OracleParameter("pEmployID", this.EmployID), new OracleParameter("pUpdateBy", this.UpdateBy),
                                                                        new OracleParameter("pUpdateStamp", this.UpdateStamp), new OracleParameter("pIDFlowTeam", this.IDFlowTeam),
                                                                        new OracleParameter("pIDRole", this.IDRole),new OracleParameter("pCreateBy", this.CreateBy),
                                                                        new OracleParameter("pCreateStamp", this.CreateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 檢查結案通知欄位是否有值
        /// </summary>
        /// <returns>1(有現值); 0(無現值)</returns>
        /// 20221101 648267:結案通知為非必填欄位，之前的Update method 只有update語法沒有insert
        private bool CheckNotifyRecordExist(string idflowteam, string idrole)
        {
            string sql = string.Format(BuildFetchCommandString() + @" WHERE IDFlowTeam = :pIDFlowTeam AND idrole = :pIDRole", this.FormName.ToLower());
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFlowTeam", idflowteam), new OracleParameter("pIDRole", idrole) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt.Rows.Count > 0;
        }

        /// <summary>
        /// 儲存表單團隊日誌。
        /// </summary>
        /// <returns></returns>
        private bool SaveLog()
        {
            string sql = "";
            // 20221104 648267:如果不選擇，加入LOG的員工號為000000
            if (this.EmployID == "000000")
            {
                sql = string.Format(@"INSERT INTO fzdb.fzt{0}_role_team_log(IDFlowTeam, BranchID, IDRole, EmployID, CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                                         VALUES (:pIDFlowTeam, :pBranchID, :pIDRole, :pEmployID, :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)"
                                        , this.FormName.ToLower()
                                        );
            }
            else
            {
                sql = string.Format(@"INSERT INTO fzdb.fzt{0}_role_team_log
                                         SELECT * FROM fzdb.fzt{0}_role_team WHERE IDFlowTeam = :pIDFlowTeam AND BranchID = :pBranchID AND IDRole = :pIDRole"
                                        , this.FormName.ToLower()
                                      );
            }

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pIDRole", this.IDRole),
                                                                        new OracleParameter("pEmployID", this.EmployID), new OracleParameter("pUpdateBy", this.UpdateBy),
                                                                        new OracleParameter("pUpdateStamp", this.UpdateStamp), new OracleParameter("pIDFlowTeam", this.IDFlowTeam),
                                                                        new OracleParameter("pIDRole", this.IDRole),new OracleParameter("pCreateBy", this.CreateBy),
                                                                        new OracleParameter("pCreateStamp", this.CreateStamp)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 擷取來源表單之團隊成員資料，建立目的表單之團隊成員資料。
        /// </summary>
        /// <param name="enumPageMode">頁面模式列舉型態</param>
        /// <param name="sourceModuleName">來源模組名稱</param>
        /// <param name="sourceFormName">來源表單名稱</param>
        /// <param name="sourceIDFlowTeam">來源表單編號</param>
        /// <param name="destinationModuleName">目的模組名稱</param>
        /// <param name="destinationFormName">目的表單編號</param>
        /// <param name="destinationIDFlowTeam">目的表單編號</param>
        /// <param name="destinationBranchID">目的版本值</param>
        /// <param name="isSaveLog"></param>
        /// <returns></returns>
        public static bool SaveFormRoleTeam(PageMode.PageModeEnum enumPageMode, string sourceModuleName, string sourceFormName, string sourceIDFlowTeam, string destinationModuleName, string destinationFormName, string destinationIDFlowTeam, long destinationBranchID, bool isSaveLog)
        {
            switch (enumPageMode)
            {
                case PageMode.PageModeEnum.Create:
                    string sql = string.Format(@"SELECT RT.IDFlowTeam, RT.BranchID, RT.IDRole, RT.EmployID, RT.CreateBy, RT.CreateStamp, RT.UpdateBy, RT.UpdateStamp
                                             FROM   fzdb.fzt{0}_role_def RD
                                                    LEFT  JOIN fzdb.fzt{0}_role_team  RT ON RD.IDRole = RT.IDRole
                                             WHERE  RT.IDFlowTeam = :pIDFlowTeam
                                             ORDER BY RD.Order_, RD.IDRole"
                                                , sourceFormName.ToLower());
                    List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFlowTeam", sourceIDFlowTeam) };
                    DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

                    foreach (DataRow dr in dt.Rows)
                    {
                        ModuleFormRoleTeam frt = new ModuleFormRoleTeam();
                        frt.ModuleNmae = destinationModuleName;
                        frt.FormName = destinationFormName;
                        frt.IDFlowTeam = destinationIDFlowTeam;
                        frt.BranchID = destinationBranchID;
                        frt.IDRole = dr["IDRole"].ToString();
                        frt.EmployID = dr["EmployID"].ToString();
                        frt.CreateBy = LoginSession.GetLoginSession().UserID;
                        frt.CreateStamp = DateTime.Now;
                        frt.UpdateBy = LoginSession.GetLoginSession().UserID;
                        frt.UpdateStamp = DateTime.Now;

                        if (!frt.Save(enumPageMode, isSaveLog))
                            return false;
                    }

                    break;

                default:
                    ModuleFormRoleTeamCollection col = FetchByIDFlowTeam(destinationModuleName, destinationFormName, destinationIDFlowTeam, ReturnObjectTypeEnum.Collection) as ModuleFormRoleTeamCollection;
                    foreach (ModuleFormRoleTeam frt in col)
                    {
                        frt.BranchID = destinationBranchID;
                        frt.UpdateBy = LoginSession.GetLoginSession().UserID;
                        frt.UpdateStamp = DateTime.Now;

                        if (!frt.Save(enumPageMode, isSaveLog))
                            return false;
                    }

                    break;
            }

            return true;
        }
    }

    /// <summary>
    /// 表單角色團隊集合類別。
    /// </summary>
    public class ModuleFormRoleTeamCollection : List<ModuleFormRoleTeam>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ModuleFormRoleTeamCollection()
        {

        }
    }
}