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
    /// 使用者群組與組織部門類別。
    /// </summary>
    public class GroupUnit
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GroupUser));

        #region Property
        /// <summary>
        /// 使用者群組與組織部門編號。
        /// </summary>
        public string IDBllGroupUnit { get; set; }

        /// <summary>
        /// 版本值。
        /// </summary>
        public long BranchID { get; set; }

        /// <summary>
        /// 版本。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 使用者群組編號。
        /// </summary>
        public string IDBllGroup { get; set; }

        /// <summary>
        /// 組織部門代碼。
        /// </summary>
        public string UnitCd { get; set; }

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
        public GroupUnit()
        {

        }

        public GroupUnit(string idBllGroupUnit)
        {
            if (!string.IsNullOrEmpty(idBllGroupUnit))
            {
                this.IDBllGroupUnit = idBllGroupUnit;

                Load();
            }
        }

        public GroupUnit(string idBllGroup, string unitCd)
        {
            if (!string.IsNullOrEmpty(idBllGroup) && !string.IsNullOrEmpty(unitCd))
            {
                this.IDBllGroup = IDBllGroup;
                this.UnitCd = unitCd;

                Load();
            }
        }

        public void Load()
        {
            DataTable dt = null;
            if (!string.IsNullOrEmpty(this.IDBllGroupUnit))
            {
                dt = FetchByIDBllGroupUnit();
            }
            else
            {
                dt = FetchByIDBllGroupAndUnitCd();
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                SetGroupUnit(dt.Rows[0]);
            }
        }

        public void SetGroupUnit(DataRow dr)
        {
            if (dr["IDBllGroupUnit"] != DBNull.Value)
            {
                this.IDBllGroupUnit = dr["IDBllGroupUnit"].ToString();
            }

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();
            this.IDBllGroup = dr["IDBllGroup"].ToString();
            this.UnitCd = dr["UnitCd"].ToString();
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
        public static string BuildFetchCommandString()
        {
            return @"SELECT IDBllGroupUnit, IDBllGroup, UnitCd, BranchID, Version, CreateBy, CreateStamp, UpdateBy, UpdateStamp FROM fzdb.fztBllGroupUnit GU";
        }

        private DataTable FetchByIDBllGroupUnit()
        {
            string sql = BuildFetchCommandString() + @" WHERE GU.IDBllGroupUnit = :pIDBllGroupUnit";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroupUnit", this.IDBllGroupUnit) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        private DataTable FetchByIDBllGroupAndUnitCd()
        {
            string sql = BuildFetchCommandString() + @" WHERE GU.IDBllGroup = :pIDBllGroup AND GU.UnitCd = :pUnitCd";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", this.IDBllGroup), new OracleParameter("pUnitCd", this.UnitCd) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 儲存使用者群組與組織部門。
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
        /// 新增使用者群組與組織部門。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            if (string.IsNullOrEmpty(this.IDBllGroupUnit))
            {
                this.IDBllGroupUnit = Document.FetchNextDocNo("ID_GroupUnit", Document.DocNoStatusEnum.Reserve);
            }

            string sql = @"INSERT INTO fzdb.fztBllGroupUnit(IDBllGroupUnit, BranchID, Version, IDBllGroup, UnitCd, CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDBllGroupUnit, :pBranchID, :pVersion, :pIDBllGroup, :pUnitCd, :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroupUnit", this.IDBllGroupUnit),
                                                                        new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pIDBllGroup", this.IDBllGroup), new OracleParameter("pUnitCd", this.UnitCd),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新使用者群組與組織部門。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztBllGroupUnit
                           SET    BranchID = :pBranchID, Version = :pVersion,
                                  IDBllGroup = :pIDBllGroup, UnitCd = :pUnitCd,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDBllGroupUnit = :pIDBllGroupUnit";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pIDBllGroup", this.IDBllGroup), new OracleParameter("pUnitCd", this.UnitCd),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIDBllGroupUnit", this.IDBllGroupUnit)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存使用者群組與組織部門日誌。
        /// </summary>
        /// <returns></returns>
        private bool SaveLog()
        {
            string sql = @"INSERT INTO fzdb.fztBllGroupUnit_Log
                           SELECT * FROM fzdb.fztBlLGroupUnit WHERE IDBllGroupUnit = :pIDBllGroupUnit";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroupUnit", this.IDBllGroupUnit) };

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
                    GroupUnitCollection col = new GroupUnitCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        GroupUnit obj = new GroupUnit();
                        obj.SetGroupUnit(dr);
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
        /// 儲存群組與組織部門集合物件。
        /// </summary>
        /// <param name="idBllGroup">群組編號</param>
        /// <param name="col">群組與組織部門集合物件</param>
        /// <param name="isSaveLog">是否儲存日誌</param>
        public static bool SaveGroupUnitCollection(string idBllGroup, GroupUnitCollection col, bool isSaveLog)
        {
            if (DeleteByIDBllGroup(idBllGroup))
            {
                foreach (GroupUnit obj in col)
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
        /// 依據群組編號(IDBllGroup)刪除使用者群組與組織部門。
        /// </summary>
        /// <param name="idBllGroup">群組編號</param>
        /// <returns></returns>
        public static bool DeleteByIDBllGroup(string idBllGroup)
        {
            string sql = @"DELETE FROM fzdb.fztBllGroupUnit GU WHERE GU.IDBllGroup = :pIDBllGroup";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBllGroup", idBllGroup) };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result >= 0;
        }
    }

    /// <summary>
    /// 使用者群組與組織部門集合類別。
    /// </summary>
    public class GroupUnitCollection : List<GroupUnit>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public GroupUnitCollection()
        {

        }
    }
}
