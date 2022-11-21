using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

using Library.Component.BLL;
using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Module.FCDS.Application
{
    /// <summary>
    /// 飛航組員任務換班申請系統申請主表單之申請人與受申請人之班表類別。
    /// </summary>
    public class FcdsApplyRoster
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FcdsApply));

        #region Property
        public string IDFcdsApplyRoster { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單表單編號。
        /// </summary>
        public string IDFcdsApply { get; set; }

        public DateTime RosterDate { get; set; }

        /// <summary>
        /// 版本值。
        /// </summary>
        public long? BranchID { get; set; }

        /// <summary>
        /// 版本號。
        /// </summary>
        public string Version { get; set; }

        public string ApplicantID { get; set; }

        public int ApplicantDutyDay { get; set; }

        public long ApplicantDutyNo { get; set; }

        /// <summary>
        /// 申請人 Crew Route 之總航段數。
        /// </summary>
        public int ApplicantNumOfLeg { get; set; }

        public bool IsApplicantEnable { get; set; }

        public bool IsApplicantTick { get; set; }

        public FcdsApplyRosterLegCollection ApplicantRosterLegs { get; set; }

        public string RespondentID { get; set; }

        public int RespondentDutyDay { get; set; }

        public long RespondentDutyNo { get; set; }

        /// <summary>
        /// 受申請人 Crew Route 之總航段數。
        /// </summary>
        public int RespondentNumOfLeg { get; set; }

        public bool IsRespondentEnable { get; set; }

        public bool IsRespondentTick { get; set; }

        public FcdsApplyRosterLegCollection RespondentRosterLegs { get; set; }

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
        public FcdsApplyRoster()
        {
            this.ApplicantRosterLegs = new FcdsApplyRosterLegCollection();
            this.RespondentRosterLegs = new FcdsApplyRosterLegCollection();
        }

        private void Load()
        {

        }

        private void SetFcdsApplyRoster(DataRow dr)
        {
            this.IDFcdsApplyRoster = dr["IDFcdsApplyRoster"].ToString();
            this.IDFcdsApply = dr["IDFcdsApply"].ToString();

            if (dr["RosterDate"] != DBNull.Value && DateTime.TryParse(dr["RosterDate"].ToString(), out _))
            {
                this.RosterDate = DateTime.Parse(dr["RosterDate"].ToString());
            }

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();
            this.ApplicantID = dr["ApplicantID"].ToString();

            if (dr["ApplicantDutyDay"] != DBNull.Value && int.TryParse(dr["ApplicantDutyDay"].ToString(), out _))
            {
                this.ApplicantDutyDay = int.Parse(dr["ApplicantDutyDay"].ToString());
            }

            if (dr["ApplicantDutyNo"] != DBNull.Value && int.TryParse(dr["ApplicantDutyNo"].ToString(), out _))
            {
                this.ApplicantDutyNo = int.Parse(dr["ApplicantDutyNo"].ToString());
            }

            if (dr["ApplicantNumOfLeg"] != DBNull.Value && int.TryParse(dr["ApplicantNumOfLeg"].ToString(), out _))
            {
                this.ApplicantNumOfLeg = int.Parse(dr["ApplicantNumOfLeg"].ToString());
            }

            if (dr["IsApplicantEnable"] != DBNull.Value && bool.TryParse(dr["IsApplicantEnable"].ToString(), out _))
            {
                this.IsApplicantEnable = bool.Parse(dr["IsApplicantEnable"].ToString());
            }

            if (dr["IsApplicantTick"] != DBNull.Value && bool.TryParse(dr["IsApplicantTick"].ToString(), out _))
            {
                this.IsApplicantTick = bool.Parse(dr["IsApplicantTick"].ToString());
            }

            this.ApplicantRosterLegs = JsonConvert.DeserializeObject<FcdsApplyRosterLegCollection>(dr["ApplicantRosterLegs"].ToString());

            this.RespondentID = dr["RespondentID"].ToString();

            if (dr["RespondentDutyDay"] != DBNull.Value && int.TryParse(dr["RespondentDutyDay"].ToString(), out _))
            {
                this.RespondentDutyDay = int.Parse(dr["RespondentDutyDay"].ToString());
            }

            if (dr["RespondentDutyNo"] != DBNull.Value && int.TryParse(dr["RespondentDutyNo"].ToString(), out _))
            {
                this.RespondentDutyNo = int.Parse(dr["RespondentDutyNo"].ToString());
            }

            if (dr["IsRespondentEnable"] != DBNull.Value && bool.TryParse(dr["IsRespondentEnable"].ToString(), out _))
            {
                this.IsRespondentEnable = bool.Parse(dr["IsRespondentEnable"].ToString());
            }

            if (dr["IsRespondentTick"] != DBNull.Value && bool.TryParse(dr["IsRespondentTick"].ToString(), out _))
            {
                this.IsRespondentTick = bool.Parse(dr["IsRespondentTick"].ToString());
            }

            this.RespondentRosterLegs = JsonConvert.DeserializeObject<FcdsApplyRosterLegCollection>(dr["RespondentRosterLegs"].ToString());

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

        private static string BuildFetchCommandString()
        {
            return @"SELECT AR.IDFcdsapplyRoster, AR.IDFcdsApply, AR.RosterDate, AR.BranchID, AR.Version,
                            AR.ApplicantID, AR.ApplicantDutyDay, AR.ApplicantDutyNo, AR.ApplicantNumOfLeg, AR.IsApplicantEnable, AR.IsApplicantTick, AR.ApplicantRosterLegs,
                            AR.RespondentID, AR.RespondentDutyDay, AR.RespondentDutyNo, AR.RespondentNumOfLeg, AR.IsRespondentEnable, AR.IsRespondentTick, AR.RespondentRosterLegs,
                            AR.CreateBy, AR.CreateStamp, AR.UpdateBy, AR.UpdateStamp
                     FROM   fzdb.fztfcdsapplyroster AR";
        }

        public static object FetchByIDFcdsApply(string idFcdsApply, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE AR.IDFcdsApply = :pIDFcdsApply ORDER BY AR.IDFcdsapplyRoster";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", idFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    FcdsApplyRosterCollection col = new FcdsApplyRosterCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        FcdsApplyRoster obj = new FcdsApplyRoster();
                        obj.SetFcdsApplyRoster(dr);
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
            string sql = @"SELECT ARL.IDFcdsapplyRoster, ARL.IDFcdsApply, ARL.RosterDate, ARL.BranchID, ARL.Version,
                                  ARL.ApplicantID, ARL.ApplicantDutyDay, ARL.ApplicantDutyNo, ARL.ApplicantNumOfLeg, ARL.IsApplicantEnable, ARL.IsApplicantTick, ARL.ApplicantRosterLegs,
                                  ARL.RespondentID, ARL.RespondentDutyDay, ARL.RespondentDutyNo, ARL.RespondentNumOfLeg, ARL.IsRespondentEnable, ARL.IsRespondentTick, ARL.RespondentRosterLegs,
                                  ARL.CreateBy, ARL.CreateStamp, ARL.UpdateBy, ARL.UpdateStamp
                           FROM   fzdb.fztfcdsapplyroster_log ARL
                           WHERE  ARL.IDFcdsApply = :pIDFcdsApply
                           ORDER BY ARL.BranchID, ARL.IDFcdsapplyRoster";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 儲存設定表單。
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
        /// 新增飛航組員任務換班申請單明細班表資料。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            if (string.IsNullOrEmpty(this.IDFcdsApplyRoster))
            {
                this.IDFcdsApplyRoster = Document.FetchNextDocNo("ID_FcdsApplyRoster", Document.DocNoStatusEnum.Reserve);
            }

            string sql = @"INSERT INTO fzdb.fztfcdsapplyroster(IDFcdsApplyRoster, IDFcdsApply, RosterDate, BranchID, Version,
                                                                ApplicantID, ApplicantDutyDay, ApplicantDutyNo, ApplicantNumOfLeg, IsApplicantEnable, IsApplicantTick, ApplicantRosterLegs,
                                                                RespondentID, RespondentDutyDay, RespondentDutyNo, RespondentNumOfLeg, IsRespondentEnable, IsRespondentTick, RespondentRosterLegs,
                                                                CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDFcdsApplyRoster, :pIDFcdsApply, :pRosterDate, :pBranchID, :pVersion,
                                    :pApplicantID, :pApplicantDutyDay, :pApplicantDutyNo, :pApplicantNumOfLeg, :pIsApplicantEnable, :pIsApplicantTick, :pApplicantRosterLegs,
                                    :pRespondentID, :pRespondentDutyDay, :pRespondentDutyNo, :pRespondentNumOfLeg, :pIsRespondentEnable, :pIsRespondentTick, :pRespondentRosterLegs,
                                    :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApplyRoster", this.IDFcdsApplyRoster), new OracleParameter("pIDFcdsApply", this.IDFcdsApply), new OracleParameter("pRosterDate", this.RosterDate),
                                                                        new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pApplicantID", this.ApplicantID), new OracleParameter("pApplicantDutyDay", this.ApplicantDutyDay),
                                                                        new OracleParameter("pApplicantDutyNo", this.ApplicantDutyNo), new OracleParameter("pApplicantNumOfLeg", this.ApplicantNumOfLeg),
                                                                        new OracleParameter("pIsApplicantEnable", this.IsApplicantEnable.ToString()), new OracleParameter("pIsApplicantTick", this.IsApplicantTick.ToString()),
                                                                        new OracleParameter("pApplicantRosterLegs", JsonConvert.SerializeObject(this.ApplicantRosterLegs)),
                                                                        new OracleParameter("pRespondentID", this.RespondentID), new OracleParameter("pRespondentDutyDay", this.RespondentDutyDay),
                                                                        new OracleParameter("pRespondentDutyNo", this.RespondentDutyNo), new OracleParameter("pRespondentNumOfLeg", this.RespondentNumOfLeg),
                                                                        new OracleParameter("pIsRespondentEnable", this.IsRespondentEnable.ToString()), new OracleParameter("pIsRespondentTick", this.IsRespondentTick.ToString()),
                                                                        new OracleParameter("pRespondentRosterLegs", JsonConvert.SerializeObject(this.RespondentRosterLegs)),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新飛航組員任務換班申請單明細班表資料。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztfcdsapplyroster
                           SET    IDFcdsApply = :pIDFcdsApply, RosterDate = :pRosterDate, BranchID = :pBranchID, Version = :pVersion,
                                  ApplicantID = :pApplicantID, ApplicantDutyDay = :pApplicantDutyDay, ApplicantDutyNo = :pApplicantDutyNo, ApplicantNumOfLeg = :pApplicantNumOfLeg, IsApplicantEnable = :pIsApplicantEnable, IsApplicantTick = :pIsApplicantTick, ApplicantRosterLegs = :pApplicantRosterLegs,
                                  RespondentID = :pRespondentID, RespondentDutyDay = :pRespondentDutyDay, RespondentDutyNo = :pRespondentDutyNo, RespondentNumOfLeg = :pRespondentNumOfLeg, IsRespondentEnable = :pIsRespondentEnable, IsRespondentTick = :pIsRespondentTick, RespondentRosterLegs = :pRespondentRosterLegs,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDFcdsApplyRoster = :pIDFcdsApplyRoster";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply), new OracleParameter("pRosterDate", this.RosterDate),
                                                                        new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pApplicantID", this.ApplicantID), new OracleParameter("pApplicantDutyDay", this.ApplicantDutyDay), new OracleParameter("pApplicantDutyNo", this.ApplicantDutyNo),
                                                                        new OracleParameter("pApplicantNumOfLeg", this.ApplicantNumOfLeg), new OracleParameter("pIsApplicantEnable", this.IsApplicantEnable.ToString()),
                                                                        new OracleParameter("pIsApplicantTick", this.IsApplicantTick.ToString()), new OracleParameter("pApplicantRosterLegs", JsonConvert.SerializeObject(this.ApplicantRosterLegs)),
                                                                        new OracleParameter("pRespondentID", this.ApplicantID), new OracleParameter("pRespondentDutyDay", this.RespondentDutyDay), new OracleParameter("pRespondentDutyNo", this.RespondentDutyNo),
                                                                        new OracleParameter("pRespondentNumOfLeg", this.RespondentNumOfLeg), new OracleParameter("pIsRespondentEnable", this.IsRespondentEnable.ToString()),
                                                                        new OracleParameter("pIsRespondentTick", this.IsRespondentTick.ToString()), new OracleParameter("pRespondentRosterLegs", JsonConvert.SerializeObject(this.RespondentRosterLegs)),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIDFcdsApplyRoster", this.IDFcdsApplyRoster)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存飛航組員任務換班申請單明細班表資料日誌。
        /// </summary>
        /// <returns></returns>
        private bool SaveLog()
        {
            string sql = @"INSERT INTO fzdb.fztfcdsapplyroster_log
                           SELECT * FROM fzdb.fztfcdsapplyroster WHERE IDFcdsApplyRoster = :pIDFcdsApplyRoster";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApplyRoster", this.IDFcdsApplyRoster) };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }
    }

    /// <summary>
    /// 飛航組員任務換班申請系統申請主表單之申請人與受申請人之班表集合類別。
    /// </summary>
    public class FcdsApplyRosterCollection : List<FcdsApplyRoster>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public FcdsApplyRosterCollection()
        {
            
        }
    }
}
