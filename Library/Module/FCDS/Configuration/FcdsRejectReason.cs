using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.BLL;
using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Module.FCDS.Configuration
{
    /// <summary>
    /// 飛航組員任務換班申請系統不同意原因選項類別。
    /// </summary>
    public class FcdsRejectReason
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FcdsRejectReason));

        #region Property
        public string IDFcdsRejectReason { get; set; }

        /// <summary>
        /// 版本值。
        /// </summary>
        public long? BranchID { get; set; }

        /// <summary>
        /// 版本號。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 不同意原因。
        /// </summary>
        public string RejectReason { get; set; }

        /// <summary>
        /// 有效否。
        /// </summary>
        public bool? IsValidFlag { get; set; }

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
        /// 版本集合物件。
        /// </summary>
        public VersionCollection Versions
        {
            get
            {
                return Component.BLL.Version.GetVersionCollection(FetchLog());
            }
        }
        #endregion

        public FcdsRejectReason()
        {

        }

        public FcdsRejectReason(string idfcdsrejectreason)
        {
            if (!string.IsNullOrEmpty(idfcdsrejectreason))
            {
                this.IDFcdsRejectReason = idfcdsrejectreason;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByIDFcdsRejectReason();
            if (dt.Rows.Count > 0)
            {
                SetFcdsRejectReason(dt.Rows[0]);
            }
            else
            {
                this.IDFcdsRejectReason = null;
            }
        }

        private void SetFcdsRejectReason(DataRow dr)
        {
            if (dr["IDFcdsRejectReason"] != DBNull.Value)
            {
                this.IDFcdsRejectReason = dr["IDFcdsRejectReason"].ToString();
            }

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();
            this.RejectReason = dr["RejectReason"].ToString();

            if (dr["IsValidFlag"] != DBNull.Value && bool.TryParse(dr["IsValidFlag"].ToString(), out _))
            {
                this.IsValidFlag = bool.Parse(dr["IsValidFlag"].ToString());
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
            return @"SELECT IDFcdsRejectReason, BranchID, Version, RejectReason, IsValidFlag, CreateBy, CreateStamp, UpdateBy, UpdateStamp FROM fzdb.fztfcdsrejectreason";
        }

        private DataTable FetchByIDFcdsRejectReason()
        {
            string sql = BuildFetchCommandString() + @" WHERE IDFcdsRejectReason = :pIDFcdsRejectReason";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public static object FetchAllValid(ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE IsValidFlag = 'True' ORDER BY IDFcdsRejectReason";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    FcdsRejectReasonCollection col = new FcdsRejectReasonCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        FcdsRejectReason obj = new FcdsRejectReason();
                        obj.SetFcdsRejectReason(dr);
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
        /// 擷取所有不同意原因選項。
        /// </summary>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchAll(ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" ORDER BY IDFcdsRejectReason";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];
            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    FcdsRejectReasonCollection col = new FcdsRejectReasonCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        FcdsRejectReason obj = new FcdsRejectReason();
                        obj.SetFcdsRejectReason(dr);
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
            string sql = @"SELECT IDFcdsRejectReason, BranchID, Version, RejectReason, IsValidFlag, CreateBy, CreateStamp, UpdateBy, UpdateStamp
                           FROM   fzdb.fztfcdsrejectreason_log
                           WHERE  IDFcdsRejectReason = :pIDFcdsRejectReason
                           ORDER BY BranchID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 儲存不同意原因選項。
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
        /// 新增不同意原因選項。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            if (string.IsNullOrEmpty(this.IDFcdsRejectReason))
            {
                this.IDFcdsRejectReason = Document.FetchNextDocNo("ID_FcdsRejectReason", Document.DocNoStatusEnum.Reserve);
                this.BranchID = long.Parse(Document.FetchNextDocNo("ID_Branch", Document.DocNoStatusEnum.Reserve));
                this.Version = Component.BLL.Version.GetInitialVersion();
            }

            string sql = @"INSERT INTO fzdb.fztfcdsrejectreason(IDFcdsRejectReason, BranchID, Version, RejectReason, IsValidFlag, CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDFcdsRejectReason, :pBranchID, :pVersion, :pRejectReason, :pIsValidFlag, :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason), new OracleParameter("pBranchID", this.BranchID),
                                                                        new OracleParameter("pVersion", this.Version), new OracleParameter("pRejectReason", this.RejectReason),
                                                                        new OracleParameter("pIsValidFlag", this.IsValidFlag.Value.ToString()),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新不同意原因選項。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztfcdsrejectreason
                           SET    BranchID = :pBranchID, Version = :pVersion,
                                  RejectReason = :pRejectReason, IsValidFlag = :pIsValidFlag,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDFcdsRejectReason = :pIDFcdsRejectReason";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pRejectReason", this.RejectReason), new OracleParameter("pIsValidFlag", this.IsValidFlag.Value.ToString()),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存不同意原因選項日誌。
        /// </summary>
        /// <returns></returns>
        private bool SaveLog()
        {
            string sql = @"INSERT INTO fzdb.fztfcdsrejectreason_log
                           SELECT * FROM fzdb.fztfcdsrejectreason WHERE IDFcdsRejectReason = :pIDFcdsRejectReason";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason) };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }
    }

    /// <summary>
    /// 不同意原因選項集合類別。
    /// </summary>
    public class FcdsRejectReasonCollection : List<FcdsRejectReason>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public FcdsRejectReasonCollection()
        {

        }
    }
}