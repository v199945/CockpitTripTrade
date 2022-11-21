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
    /// 使用者群組基本資料類別。
    /// </summary>
    public class Group
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Group));

        /// <summary>
        /// 系統管理員群組編號。
        /// </summary>
        public const string SYSTEM_ADMIN_GROUP = @"GRP-202011-002";

        /// <summary>
        /// 使用者管理員群組編號。
        /// </summary>
        public const string USER_ADMIN_GROUP = @"GRP-202011-001";

        /// <summary>
        /// 組員派遣部一般使用者。
        /// </summary>
        public const string SCHEDULING_USER_GROUP = @"GRP-202011-003";

        /// <summary>
        /// OG系統管理員。
        /// </summary>
        public const string OG_ADMIN_GROUP = @"GRP-202107-001";


        #region Property
        /// <summary>
        /// 群組編號。
        /// </summary>
        public string IDBllGroup { get; set; }

        /// <summary>
        /// 系統名稱。
        /// </summary>
        public string RootNameSpace { get; set; }

        /// <summary>
        /// 群組代碼。
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// 版本值。
        /// </summary>
        public long BranchID { get; set; }

        /// <summary>
        /// 版本。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 群組名稱。
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 群組備註。
        /// </summary>
        public string Comments { get; set; }

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
        /// 使用者群組與組織部門集合物件。
        /// </summary>
        public GroupUnitCollection GroupUnits { get; set; }

        /// <summary>
        /// 使用者群組與使用者集合物件。
        /// </summary>
        public GroupUserCollection GroupUsers { get; set; }

        /// <summary>
        /// 使用者群組之模組表單功能權限集合物件。
        /// </summary>
        public GroupModuleFormFunctionCollection GroupModuleFormFunctions { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public Group()
        {

        }

        public Group(string idBllGroup)
        {
            if (!string.IsNullOrEmpty(idBllGroup))
            {
                this.IDBllGroup = idBllGroup;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByIDBllGroup();
            if (dt.Rows.Count > 0)
            {
                SetGroup(dt.Rows[0]);
            }
        }

        private void SetGroup(DataRow dr)
        {
            this.IDBllGroup = dr["IDBllGroup"].ToString();
            this.RootNameSpace = dr["RootNameSpace"].ToString();
            this.GroupCode = dr["GroupCode"].ToString();

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();
            this.GroupName = dr["GroupName"].ToString();
            this.Comments = dr["Comments"].ToString();

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

            this.GroupUnits = GroupUnit.FetchByIDBllGroup(this.IDBllGroup, ReturnObjectTypeEnum.Collection) as GroupUnitCollection;
            this.GroupUsers = GroupUser.FetchByIDBllGroup(this.IDBllGroup, ReturnObjectTypeEnum.Collection) as GroupUserCollection;
            this.GroupModuleFormFunctions = GroupModuleFormFunction.FetchByIDBllGroup(this.IDBllGroup, ReturnObjectTypeEnum.Collection) as GroupModuleFormFunctionCollection;
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT IDBllGroup, BranchID, Version, RootNameSpace, GroupCode, GroupName, Comments, CreateBy, CreateStamp, UpdateBy, UpdateStamp
                     FROM fzdb.fztBllGroup G WHERE G.RootNameSpace = '" + FcdsHelper.ROOT_NAME_SPACE + @"'";
        }

        private DataTable FetchByIDBllGroup()
        {
            string sql = BuildFetchCommandString() + @" AND G.IDBllGroup = :pIDBllGroup";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", this.IDBllGroup) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 擷取所有使用者群組。
        /// </summary>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchAll(ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" ORDER BY G.IDBllGroup";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];
            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    GroupCollection col = new GroupCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Group obj = new Group();
                        obj.SetGroup(dr);
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
            string sql = @"SELECT IDBllGroup, BranchID, Version, RootNameSpace, GroupCode, GroupName, Comments, CreateBy, CreateStamp, UpdateBy, UpdateStamp FROM fzdb.fztBllGroup_Log G WHERE G.IDBllGroup = :pIDBllGroup ORDER BY GU.BranchID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", this.IDBllGroup) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 儲存使用者群組。
        /// </summary>
        /// <param name="rootNameSpace">系統名稱</param>
        /// <param name="enumPageMode">頁面模式列舉型態</param>
        /// <param name="isSaveLog">是否儲存日誌</param>
        /// <returns></returns>
        public bool Save(string rootNameSpace, PageMode.PageModeEnum enumPageMode, bool isSaveLog)
        {
            bool result = false;
            switch (enumPageMode)
            {
                case PageMode.PageModeEnum.Create:
                    result = Insert(rootNameSpace);
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
        /// 新增使用者群組。
        /// </summary>
        /// <param name="rootNameSpace">系統名稱</param>
        /// <returns></returns>
        private bool Insert(string rootNameSpace)
        {
            if (string.IsNullOrEmpty(this.IDBllGroup))
            {
                this.IDBllGroup = Document.FetchNextDocNo("ID_Group", Document.DocNoStatusEnum.Reserve);
                this.BranchID = long.Parse(Document.FetchNextDocNo("ID_Branch", Document.DocNoStatusEnum.Reserve));
                this.Version = Component.BLL.Version.GetInitialVersion();
            }

            string sql = @"INSERT INTO fzdb.fztBllGroup(IDBllGroup, BranchID, Version, RootNameSpace, GroupCode, GroupName, Comments, CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDBllGroup, :pBranchID, :pVersion, :pRootNameSpace, :pGroupCode, :pGroupName, :pComments, :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", this.IDBllGroup),
                                                                        new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pRootNameSpace", rootNameSpace), new OracleParameter("pGroupCode", this.GroupCode),
                                                                        new OracleParameter("pGroupName", this.GroupName), new OracleParameter("pComments", this.Comments),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新使用者群組。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztBllGroup
                           SET    BranchID = :pBranchID, Version = :pVersion,
                                  GroupCode = :pGroupCode, GroupName = :pGroupName, Comments = :pComments,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDBllGroup = :pIDBllGroup";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pGroupCode", this.GroupCode), new OracleParameter("pGroupName", this.GroupName), new OracleParameter("pComments", this.Comments),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIDBllGroup", this.IDBllGroup)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存使用者群組日誌。
        /// </summary>
        /// <returns></returns>
        private bool SaveLog()
        {
            string sql = @"INSERT INTO fzdb.fztBllGroup_Log
                           SELECT * FROM fzdb.fztBlLGroup WHERE IDBllGroup = :pIDBllGroup";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", this.IDBllGroup) };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 判斷使用者是否為系統管理員，使用者群組集合物件是否含系統管理員群組。
        /// </summary>
        /// <param name="userGroups">使用者之群組集合物件</param>
        /// <returns></returns>
        public static bool IsSystemAdmin(UserGroupCollection userGroups)
        {
            return userGroups?.Where(x => x.IDBllGroup == SYSTEM_ADMIN_GROUP).Count() > 0;
        }

        /// <summary>
        /// 判斷使用者是否為使用者管理員，使用者群組集合物件是否含使用者管理員群組。
        /// </summary>
        /// <param name="userGroups">使用者之群組集合物件</param>
        /// <returns></returns>
        public static bool IsUserAdmin(UserGroupCollection userGroups)
        {
            return userGroups?.Where(x => x.IDBllGroup == USER_ADMIN_GROUP).Count() > 0;
        }

        /// <summary>
        /// 判斷使用者是否為OG群組，使用者群組集合物件是否含OG使用者管理員群組。
        /// </summary>
        /// <param name="userGroups">使用者之群組集合物件</param>
        /// <returns></returns>
        public static bool IsOGAdmin(UserGroupCollection userGroups) 
        {
            return userGroups?.Where(x=>x.IDBllGroup == OG_ADMIN_GROUP).Count() > 0;
        }


    }

    /// <summary>
    /// 使用者群組集合物件類別。
    /// </summary>
    public class GroupCollection : List<Group>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public GroupCollection()
        {

        }
    }
}
