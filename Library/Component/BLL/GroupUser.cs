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
    /// 群組與使用者類別。
    /// </summary>
    public class GroupUser
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GroupUser));

        #region Property
        /// <summary>
        /// 使用者群組與使用者編號。
        /// </summary>
        public string IDBllGroupUser { get; set; }

        /// <summary>
        /// 版本值。
        /// </summary>
        public long? BranchID { get; set; }

        /// <summary>
        /// 版本號。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 使用者群組編號。
        /// </summary>
        public string IDBllGroup { get; set; }

        /// <summary>
        /// 使用者員工編號。
        /// </summary>
        public string IDUser { get; set; }
        
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
        #endregion


        /// <summary>
        /// 預設建構子。
        /// </summary>
        public GroupUser()
        {

        }

        public GroupUser(string idBlLGroupUser)
        {
            if (!string.IsNullOrEmpty(idBlLGroupUser))
            {
                this.IDBllGroupUser = IDBllGroupUser;

                Load();
            }
        }

        public GroupUser(string idBllGroup, string idUser)
        {
            if (!string.IsNullOrEmpty(idBllGroup) && !string.IsNullOrEmpty(idUser))
            {
                this.IDBllGroup = idBllGroup;
                this.IDUser = idUser;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = null;
            if (!string.IsNullOrEmpty(this.IDBllGroupUser))
            {

            }
            else
            {
                dt = FetchByIDBllGroupAndIDUser();
            }

            if (dt.Rows.Count > 0)
            {
                SetGroupUser(dt.Rows[0]);
            }
        }

        private void SetGroupUser(DataRow dr)
        {
            if (dr["IDBlLGroupUser"] != DBNull.Value)
            {
                this.IDBllGroupUser = dr["IDBllGroupUser"].ToString();
            }

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();

            if (dr["IDBllGroup"] != DBNull.Value)
            {
                this.IDBllGroup = dr["IDBllGroup"].ToString();
            }

            if (dr["IDUser"] != DBNull.Value)
            {
                this.IDUser = dr["IDUser"].ToString();
            }

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
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT GU.IDBllGroupUser, GU.BranchID, GU.Version, GU.IDBllGroup, GU.IDUser, GU.CreateBy, GU.CreateStamp, GU.UpdateBy, GU.UpdateStamp FROM fzdb.fztBllGroupUser GU";
        }

        private DataTable FetchByIDBllGroupUser()
        {
            string sql = BuildFetchCommandString() + @" WHERE GU.IDBllGroupUser = :pIDBllGroupUser";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroupUser", this.IDBllGroupUser) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        private DataTable FetchByIDBllGroupAndIDUser()
        {
            string sql = BuildFetchCommandString() + @" WHERE GU.IDBllGroup = :pIDBllGroup AND GU.IDUser = :pUserID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pUserID", this.IDUser), new OracleParameter("pIDBllGroup", this.IDBllGroup) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 儲存群組與使用者。
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
        /// 新增群組與使用者。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            if (string.IsNullOrEmpty(this.IDBllGroupUser))
            {
                this.IDBllGroupUser = Document.FetchNextDocNo("ID_GroupUser", Document.DocNoStatusEnum.Reserve);
            }

            string sql = @"INSERT INTO fzdb.fztBllGroupUser(IDBllGroupUser, BranchID, Version, IDBllGroup, IDUser, CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDBllGroupUser, :pBranchID, :pVersion, :pIDBllGroup, :pIDUser, :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroupUser", this.IDBllGroupUser),
                                                                        new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pIDBllGroup", this.IDBllGroup), new OracleParameter("pIDUser", this.IDUser),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新群組與使用者。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztBllGroupUser
                           SET    BranchID = :pBranchID, Version = :pVersion,
                                  IDBllGroup = :pIDBllGroup, IDUser = :pIDUser,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDBllGroupUser = :pIDBllGroupUser";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pIDBllGroup", this.IDBllGroup), new OracleParameter("pIDUser", this.IDUser),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIDBllGroupUser", this.IDBllGroupUser)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存群組與使用者日誌。
        /// </summary>
        /// <returns></returns>
        private bool SaveLog()
        {
            string sql = @"INSERT INTO fzdb.fztBllGroupUser_Log
                           SELECT * FROM fzdb.fztBlLGroupUser WHERE IDBllGroupUser = :pIDBllGroupUser";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroupUser", this.IDBllGroupUser) };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        public static object FetchByIDBllGroup(string idBllGroup, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE GU.IDBllGroup = :pIDBllGroup";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", idBllGroup) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    GroupUserCollection col = new GroupUserCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        GroupUser obj = new GroupUser();
                        obj.SetGroupUser(dr);
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
        /// 儲存群組與使用者集合物件。
        /// </summary>
        /// <param name="idBllGroup">群組編號</param>
        /// <param name="col">群組與使用者集合物件</param>
        /// <param name="isSaveLog">是否儲存日誌</param>
        /// <returns></returns>
        public static bool SaveGroupUserCollection(string idBllGroup, GroupUserCollection col, bool isSaveLog)
        {
            if (DeleteByIDBllGroup(idBllGroup))
            {
                foreach (GroupUser obj in col)
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
            string sql = @"DELETE FROM fzdb.fztBllGroupUser GU WHERE GU.IDBllGroup = :pIDBllGroup";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", idBllGroup) };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result >= 0;
        }
    }

    /// <summary>
    /// 群組與使用者集合類別。
    /// </summary>
    public class GroupUserCollection : List<GroupUser>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public GroupUserCollection()
        {

        }
    }
}
