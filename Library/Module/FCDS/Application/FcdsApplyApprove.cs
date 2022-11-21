using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.BLL;
using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Module.FCDS.Application
{
    /// <summary>
    /// 飛航組員任務換班申請系統申請主表單組員派遣部審核意見類別。
    /// </summary>
    public class FcdsApplyApprove
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FcdsApplyApprove));

        #region Property
        /// <summary>
        /// 
        /// </summary>
        public string IDFcdsApplyApprove { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單表單編號。
        /// </summary>
        public string IDFcdsApply { get; set; }

        /// <summary>
        /// 版本值。
        /// </summary>
        public long? BranchID { get; set; }

        /// <summary>
        /// 版本號。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單組員派遣部是否同意。
        /// </summary>
        public bool? IsApproval { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單組員派遣部不同意原因 ID。
        /// </summary>
        public string IDFcdsRejectReason { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單組員派遣部審核意見。
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

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public FcdsApplyApprove()
        {

        }

        public FcdsApplyApprove(string idFcdsApply)
        {
            if (!string.IsNullOrEmpty(idFcdsApply))
            {
                this.IDFcdsApply = idFcdsApply;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByIDFcdsApply();
            if (dt.Rows.Count > 0)
            {
                SetFcdsApplyApprove(dt.Rows[0]);
            }
            else
            {
                this.IDFcdsApplyApprove = null;
            }
        }

        private void SetFcdsApplyApprove(DataRow dr)
        {
            this.IDFcdsApplyApprove = dr["IDFcdsApplyApprove"].ToString();
            this.IDFcdsApply = dr["IDFcdsApply"].ToString();

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();

            if (dr["IsApproval"] != DBNull.Value && bool.TryParse(dr["IsApproval"].ToString(), out _))
            {
                this.IsApproval = bool.Parse(dr["IsApproval"].ToString());
            }

            this.IDFcdsRejectReason = dr["IDFcdsRejectReason"].ToString();
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
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// 擷取飛航組員任務互換申請主表單組員派遣部審核資料。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT IDFcdsApplyApprove, IDFcdsApply, BranchID, Version, IsApproval, IDFcdsRejectReason, Comments,
                            CreateBy, CreateStamp, UpdateBy, UpdateStamp
                     FROM   fzdb.fztfcdsapplyapprove";
        }

        public DataTable FetchByIDFcdsApply()
        {
            string sql = BuildFetchCommandString() + @" WHERE IDFcdsApply = :pIDFcdsApply";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public DataTable FetchLog()
        {
            string sql = @"SELECT IDFcdsApplyApprove, IDFcdsApply, BranchID, Version, IsApproval, IDFcdsRejectReason, Comments,
                                  CreateBy, CreateStamp, UpdateBy, UpdateStamp
                           FROM   fzdb.fztfcdsapplyapprove_log
                           WHERE  IDFcdsApply = :pIDFcdsApply
                           ORDER BY BranchID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 儲存飛航組員任務互換申請主表單組員派遣部審核資料。
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
        /// 新增飛航組員任務互換申請主表單組員派遣部審核資料。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            if (string.IsNullOrEmpty(this.IDFcdsApplyApprove))
            {
                this.IDFcdsApplyApprove = Document.FetchNextDocNo("ID_FcdsApplyApprove", Document.DocNoStatusEnum.Reserve);
            }

            if (string.IsNullOrEmpty(this.IDFcdsApply))
            {
                this.BranchID = long.Parse(Document.FetchNextDocNo("ID_Branch", Document.DocNoStatusEnum.Reserve));
                this.Version = Component.BLL.Version.GetInitialVersion();
            }

            string sql = @"INSERT INTO fzdb.fztfcdsapplyapprove(IDFcdsApplyApprove, IDFcdsApply, BranchID, Version,
                                                                IsApproval, IDFcdsRejectReason, Comments,
                                                                CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDFcdsApplyApprove, :pIDFcdsApply, :pBranchID, :pVersion, :pIsApproval, :pIDFcdsRejectReason, :pComments,
                                   :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApplyApprove", this.IDFcdsApplyApprove), new OracleParameter("pIDFcdsApply", this.IDFcdsApply),
                                                                        new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pIsApproval", this.IsApproval.Value.ToString()), new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason), new OracleParameter("pComments", this.Comments),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新飛航組員任務互換申請主表單組員派遣部審核資料。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztfcdsapplyapprove
                           SET    IDFcdsApply = :pIDFcdsApply, BranchID = :pBranchID, Version = :pVersion,
                                  IsApproval = :pIsApproval, IDFcdsRejectReason = :pIDFcdsRejectReason, Comments = :pComments,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDFcdsApplyApprove = :pIDFcdsApplyApprove";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply), new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pIsApproval", this.IsApproval.Value.ToString()), new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason), new OracleParameter("pComments", this.Comments),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIDFcdsApplyApprove", this.IDFcdsApplyApprove)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存飛航組員任務互換申請主表單日誌。
        /// </summary>
        /// <returns></returns>
        private bool SaveLog()
        {
            string sql = @"INSERT INTO fzdb.fztfcdsapplyapprove_log
                           SELECT * FROM fzdb.fztfcdsapplyapprove WHERE IDFcdsApplyapprove = :pIDFcdsApplyapprove";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApplyApprove", this.IDFcdsApplyApprove) };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }
    }
}
