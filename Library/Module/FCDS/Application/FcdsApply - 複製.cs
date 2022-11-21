using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.BLL;
using Library.Component.DAL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS.Configuration;
using Library.Module.FZDB;
using Library.Module.HRDB;

namespace Library.Module.FCDS.Application
{
    /// <summary>
    /// 飛航組員任務互換申請主表單類別。
    /// </summary>
    public class FcdsApply
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FcdsApply));

        /*
         * RELEASED                     已批核
         * INITIAL	                    申請中
         * REVOKE                       收回
         * RESPONDENT                   受申請人審核中
         * OP_STAFF                     承辦人審核中      組派部審核中
         * OP_MANAGER                   組長審核中        組派部審核中
         * OP_ASSISTANT_GENERAL_MANAGER 副理審核中        組派部審核中
         * OP_GENERAL_MANAGER           經理審核中        組派部審核中
         * 
        */

        #region Property
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
        /// 飛航組員任務互換申請者員工編號。
        /// </summary>
        public string ApplicantID { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請者機隊 ID。
        /// </summary>
        public string IDAcTypeApplicant { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請者職級 ID。
        /// </summary>
        public int? IDCrewPosApplicant { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單申請日期。
        /// </summary>
        public DateTime? ApplicationDate { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請者 AIMS 系統班表發佈截止日期。
        /// </summary>
        public DateTime? ApplicantPublishDate { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單申請截止日期。
        /// </summary>
        public DateTime? ApplicationDeadline { get; set; }

        /// <summary>
        /// 飛航組員任務互換受申請者員工編號。
        /// </summary>
        public string RespondentID { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請者機隊 ID。
        /// </summary>
        public string IDAcTypeRespondent { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請者職級 ID。
        /// </summary>
        public int? IDCrewPosRespondent { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請月份。
        /// </summary>
        public string SwapScheduleMonth { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請月份選項值清單(以逗號分隔)。
        /// </summary>
        public string SwapScheduleMonthItems { get; set; }

        /// <summary>
        /// 飛航組員任務互換可申請起始日期。
        /// </summary>
        public DateTime? SwappableBeginDate { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單可撤回否。
        /// </summary>
        public bool? IsApplicantRevoke { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單前置工作天。
        /// </summary>
        public int? LeadWorkdays { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單跨月班期限。
        /// </summary>
        public int? DeadlineOfAcrossMonth { get; set; }

        public string ProID { get; set; }

        public string TaskOwnerID { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單狀態代碼。
        /// </summary>
        public string StatusCode { get; set; }

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
        public FcdsApply()
        {

        }

        public FcdsApply(string idFcdsApply)
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
                SetFcdsApply(dt.Rows[0]);
            }
            else
            {
                this.IDFcdsApply = null;
            }
        }

        private void SetFcdsApply(DataRow dr)
        {
            this.IDFcdsApply = dr["IDFcdsApply"].ToString();

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();
            this.ApplicantID = dr["ApplicantID"].ToString();
            this.IDAcTypeApplicant = dr["IDAcTypeApplicant"].ToString();

            if (dr["IDCrewPosApplicant"] != DBNull.Value && int.TryParse(dr["IDCrewPosApplicant"].ToString(), out _))
            {
                this.IDCrewPosApplicant = int.Parse(dr["IDCrewPosApplicant"].ToString());
            }

            if (dr["ApplicationDate"] != DBNull.Value && DateTime.TryParse(dr["ApplicationDate"].ToString(), out _))
            {
                this.ApplicationDate = DateTime.Parse(dr["ApplicationDate"].ToString());
            }

            if (dr["ApplicantPublishDate"] != DBNull.Value && DateTime.TryParse(dr["ApplicantPublishDate"].ToString(), out _))
            {
                this.ApplicantPublishDate = DateTime.Parse(dr["ApplicantPublishDate"].ToString());
            }

            if (dr["ApplicationDeadline"] != DBNull.Value && DateTime.TryParse(dr["ApplicationDeadline"].ToString(), out _))
            {
                this.ApplicationDeadline = DateTime.Parse(dr["ApplicationDeadline"].ToString());
            }

            this.RespondentID = dr["RespondentID"].ToString();
            this.IDAcTypeApplicant = dr["IDAcTypeApplicant"].ToString();

            if (dr["IDCrewPosRespondent"] != DBNull.Value && int.TryParse(dr["IDCrewPosRespondent"].ToString(), out _))
            {
                this.IDCrewPosRespondent = int.Parse(dr["IDCrewPosRespondent"].ToString());
            }

            this.SwapScheduleMonth = dr["SwapScheduleMonth"].ToString();
            this.SwapScheduleMonthItems = dr["SwapScheduleMonthItems"].ToString();

            if (dr["SwappableBeginDate"] != DBNull.Value && DateTime.TryParse(dr["SwappableBeginDate"].ToString(), out _))
            {
                this.SwappableBeginDate = DateTime.Parse(dr["SwappableBeginDate"].ToString());
            }

            if (dr["IsApplicantRevoke"] != DBNull.Value && bool.TryParse(dr["IsApplicantRevoke"].ToString(), out _))
            {
                this.IsApplicantRevoke = bool.Parse(dr["IsApplicantRevoke"].ToString());
            }

            if (dr["LeadWorkdays"] != DBNull.Value && int.TryParse(dr["LeadWorkdays"].ToString(), out _))
            {
                this.LeadWorkdays = int.Parse(dr["LeadWorkdays"].ToString());
            }

            if (dr["DeadlineOfAcrossMonth"] != DBNull.Value && int.TryParse(dr["DeadlineOfAcrossMonth"].ToString(), out _))
            {
                this.DeadlineOfAcrossMonth = int.Parse(dr["DeadlineOfAcrossMonth"].ToString());
            }

            this.ProID = dr["ProID"].ToString();
            this.TaskOwnerID = dr["TaskOwnerID"].ToString();
            this.StatusCode = dr["StatusCode"].ToString();
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
        /// 擷取飛航組員任務互換申請主表單。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT IDFcdsApply, BranchID, Version, ApplicantID, IDAcTypeApplicant, IDCrewPosApplicant, ApplicationDate, ApplicantPublishDate, ApplicationDeadline,
                            RespondentID, IDAcTypeRespondent, IDCrewPosRespondent, SwapScheduleMonth, SwapScheduleMonthItems, SwappableBeginDate,
                            /*IsApproval, IDFcdsRejectReason, Comments,*/ IsApplicantRevoke, LeadWorkdays, DeadlineOfAcrossMonth,
                            ProID, TaskOwnerID, StatusCode,
                            CreateBy, CreateStamp, UpdateBy, UpdateStamp
                     FROM   fzdb.fztfcdsapply A";
        }

        private DataTable FetchByIDFcdsApply()
        {
            string sql = BuildFetchCommandString() + @" WHERE IDFcdsApply = :pIDFcdsApply";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public static void FetchByIDApplicant(ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE IDApplicant = :pIDApplicant";
        }

        public static object FetchAll(ReturnObjectTypeEnum rot)
        {
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, BuildFetchCommandString()).Tables[0];
            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    FcdsApplyCollection col = new FcdsApplyCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        FcdsApply obj = new FcdsApply();
                        obj.SetFcdsApply(dr);
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
        /// 以飛航組員申請者員工編號擷取飛航組員任務換班申請主表單。
        /// </summary>
        /// <param name="applicantID">飛航組員申請者員工編號</param>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchByApplicantID(string applicantID, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE ApplicantID = :pApplicantID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pApplicantID", applicantID) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    FcdsApplyCollection col = new FcdsApplyCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        FcdsApply obj = new FcdsApply();
                        obj.SetFcdsApply(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

        public DataTable FetchAllTeamMember()
        {
            string sql = @"SELECT RD.RoleType, RT.IDFlowTeam, RT.BranchID, RT.IDRole, RT.EmployID, RT.CreateBy, RT.CreateStamp, RT.UpdateBy, RT.UpdateStamp FROM fzdb.fztfcdsconfig_role_def RD LEFT JOIN fzdb.fztfcdsapply_role_team RT ON RD.IDRole = RT.IDRole WHERE A.IDFlowTeam = :pIDFlowTeam";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFlowTeam", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 判斷飛航組員是否可建立申請單。
        /// </summary>
        /// <param name="applicantID">飛航組員任務換班申請系統申請人員工編號</param>
        /// <param name="fcdsConfig">飛航組員任務換班申請系統之機隊職級設定物件</param>
        /// <returns></returns>
        public static List<string> GetAllowCreateString(string applicantID, FcdsConfig fcdsConfig)
        {
            List<string> vs = new List<string>();

            FcdsApplyCollection fcdsApplies = FetchByApplicantID(applicantID, Library.Component.Enums.ReturnObjectTypeEnum.Collection) as FcdsApplyCollection;
            DateTime monthStartDate = DateTimeUtility.GetTheBeginOfMonth(DateTime.Now);
            DateTime monthEndDate = DateTimeUtility.GetTheEndOfMonth(DateTime.Now);

            var colThisMonth = fcdsApplies.Where(o => o.ApplicationDate >= monthStartDate && o.ApplicationDate <= monthEndDate).ToList();
            if (colThisMonth != null)
            {
                // 檢核飛航組員當月申請次數
                if (colThisMonth.Count >= fcdsConfig.NumOfMonth)
                {
                    vs.Add("The number of application you have applied exceed the maximum allowed this month.");
                }

                // 檢核飛航組員當月是否有流程中(一次一單否)之申請單
                if (fcdsConfig.IsOneReqATime.HasValue && fcdsConfig.IsOneReqATime.Value ? colThisMonth.Where(o => o.StatusCode != "RELEASED").Count() > 0 : false)
                {
                    vs.Add("You still have a request that is proceeding.");
                }
            }

            return vs;
        }

        public DataTable FetchLog()
        {
            string sql = @"SELECT IDFcdsApply, BranchID, Version, ApplicantID, IDAcTypeApplicant, IDCrewPosApplicant, ApplicationDate, ApplicationPublishDate, ApplicationDeadline,
                                  RespondentID, IDAcTypeRespondent, IDCrewPosRespondent, SwapScheduleMonth, SwapScheduleMonthItems, SwappableBeginDate,
                                  /*IsApproval, IDFcdsRejectReason, Comments, IsApplicantRevoke,*/ LeadWorkdays, DeadlineOfAcrossMonth,
                                  ProID, TaskOwnerID, StatusCode,
                                  CreateBy, CreateStamp, UpdateBy, UpdateStamp
                           FROM   fzdb.fztfcdsapply_log
                           WHERE  IDFcdsApply = :pIDFcdsApply
                           ORDER BY BranchID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 儲存飛航組員任務互換申請主表單。
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
        /// 新增飛航組員任務互換申請主表單。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            if (string.IsNullOrEmpty(this.IDFcdsApply))
            {
                this.IDFcdsApply = Document.FetchNextDocNo("ID_FcdsApply", Document.DocNoStatusEnum.Reserve);
                this.BranchID = long.Parse(Document.FetchNextDocNo("ID_Branch", Document.DocNoStatusEnum.Reserve));
                this.Version = Component.BLL.Version.GetInitialVersion();
            }
            // IsApproval, IDFcdsRejectReason, Comments,
            // :pIsApproval, :pIDFcdsRejectReason, :pComments,
            string sql = @"INSERT INTO fzdb.fztfcdsapply(IDFcdsApply, BranchID, Version,
                                                         SwappableBeginDate, ApplicantpublishDate,
                                                         ApplicantID, IDAcTypeApplicant, IDCrewPosApplicant, ApplicationDate, ApplicationDeadline,
                                                         RespondentID, IDAcTypeRespondent, IDCrewPosRespondent,
                                                         SwapScheduleMonth, SwapScheduleMonthItems, IsApplicantRevoke, LeadWorkdays, DeadlineOfAcrossMonth,
                                                         ProID, TaskOwnerID, StatusCode,
                                                         CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDFcdsApply, :pBranchID, :pVersion,
                                   :pSwappableBeginDate, :pApplicantpublishDate,
                                   :pApplicantID, :pIDAcTypeApplicant, :pIDCrewPosApplicant, :pApplicationDate, :pApplicationDeadline,
                                   :pRespondentID, :pIDAcTypeRespondent, :pIDCrewPosRespondent,
                                   :pSwapScheduleMonth, :pSwapScheduleMonthItems, :pIsApplicantRevoke, :pLeadWorkdays, :pDeadlineOfAcrossMonth,
                                   :pProID, :pTaskOwnerID, :pStatusCode,
                                   :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply), new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pSwappableBeginDate", this.SwappableBeginDate.Value), new OracleParameter("pApplicantpublishDate", this.ApplicantPublishDate.Value),
                                                                        new OracleParameter("pApplicantID", this.ApplicantID), new OracleParameter("pIDAcTypeApplicant", this.IDAcTypeApplicant), new OracleParameter("pIDCrewPosApplicant", this.IDCrewPosApplicant),
                                                                        new OracleParameter("pApplicationDate", this.ApplicationDate), new OracleParameter("pApplicationDeadline", this.ApplicationDeadline),
                                                                        new OracleParameter("pRespondentID", this.RespondentID), new OracleParameter("pIDAcTypeRespondent", this.IDAcTypeRespondent), new OracleParameter("pIDCrewPosRespondent", this.IDCrewPosRespondent),
                                                                        new OracleParameter("pSwapScheduleMonth", this.SwapScheduleMonth), new OracleParameter("pSwapScheduleMonthItems", this.SwapScheduleMonthItems),
                                                                        //new OracleParameter("pIsApproval", this.IsApproval.Value.ToString()), new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason), new OracleParameter("pComments", this.Comments),
                                                                        new OracleParameter("pIsApplicantRevoke", this.IsApplicantRevoke == null ? null : this.IsApplicantRevoke.Value.ToString()), new OracleParameter("pLeadWorkdays", this.LeadWorkdays), new OracleParameter("pDeadlineOfAcrossMonth", this.DeadlineOfAcrossMonth),
                                                                        new OracleParameter("pProID", this.ProID), new OracleParameter("pTaskOwnerID", this.TaskOwnerID), new OracleParameter("pStatusCode", this.StatusCode),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新飛航組員任務互換申請主表單。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            //  IsApproval = :pIsApproval, IDFcdsRejectReason = :pIDFcdsRejectReason, Comments = :pComments,
            string sql = @"UPDATE fzdb.fztfcdsapply
                           SET    BranchID = :pBranchID, Version = :pVersion,
                                  SwappableBeginDate = :pSwappableBeginDate, ApplicantpublishDate = :pApplicantpublishDate,
                                  ApplicantID = :pApplicantID, IDAcTypeApplicant = :pIDAcTypeApplicant, IDCrewPosApplicant = :pIDCrewPosApplicant, ApplicationDate = :pApplicationDate, ApplicationDeadline = :pApplicationDeadline
                                  RespondentID = :pRespondentID, IDAcTypeRespondent = :pIDAcTypeRespondent, IDCrewPosRespondent = :pIDCrewPosRespondent,
                                  SwapScheduleMonth = :pSwapScheduleMonth, SwapScheduleMonthItems = :pSwapScheduleMonthItems, IsApplicantRevoke = :pIsApplicantRevoke, LeadWorkdays = :pLeadWorkdays, DeadlineOfAcrossMonth = :pDeadlineOfAcrossMonth,
                                  ProID = :pProID, TaskOwnerID = :pTaskOwnerID, StatusCode = :pStatusCode,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDFcdsApply = :pIDFcdsApply";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pSwappableBeginDate", this.SwappableBeginDate.Value), new OracleParameter("pApplicantpublishDate", this.ApplicantPublishDate.Value),
                                                                        new OracleParameter("pApplicantID", this.ApplicantID), new OracleParameter("pIDAcTypeApplicant", this.IDAcTypeApplicant), new OracleParameter("pIDCrewPosApplicant", this.IDCrewPosApplicant),
                                                                        new OracleParameter("pApplicationDate", this.ApplicationDate), new OracleParameter("pApplicationDeadline", this.ApplicationDeadline),
                                                                        new OracleParameter("pRespondentID", this.RespondentID), new OracleParameter("pIDAcTypeRespondent", this.IDAcTypeRespondent), new OracleParameter("pIDCrewPosRespondent", this.IDCrewPosRespondent),
                                                                        new OracleParameter("pSwapScheduleMonth", this.SwapScheduleMonth), new OracleParameter("pSwapScheduleMonthItems", this.SwapScheduleMonthItems),
                                                                        //new OracleParameter("pIsApproval", this.IsApproval.Value.ToString()), new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason), new OracleParameter("pComments", this.Comments),
                                                                        new OracleParameter("pIsApplicantRevoke", this.IsApplicantRevoke.Value.ToString()), new OracleParameter("pLeadWorkdays", this.LeadWorkdays), new OracleParameter("pDeadlineOfAcrossMonth", this.DeadlineOfAcrossMonth),
                                                                        new OracleParameter("pProID", this.ProID), new OracleParameter("pTaskOwnerID", this.TaskOwnerID), new OracleParameter("pStatusCode", this.StatusCode),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIDFcdsdApply", this.IDFcdsApply)
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
            string sql = @"INSERT INTO fzdb.fztfcdsapply_log
                           SELECT * FROM fzdb.fztfcdsapply WHERE IDFcdsApply = :pIDFcdsApply";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 擷取機隊職級設定物件之團隊成員，儲存為飛航組員任務互換申請主表單之團隊成員。
        /// </summary>
        /// <param name="fcdsConfig">飛航組員任務換班申請系統之機隊職級設定物件</param>
        /// <param name="isSavLog">是否儲存日誌</param>
        /// <returns></returns>
        public bool SaveRoleTeam(FcdsConfig fcdsConfig, bool isSavLog)
        {
            return ModuleFormRoleTeam.CreateFormRoleTeam(@"Configuration", @"FcdsConfig", fcdsConfig.IDFcdsConfig, @"Application", @"FcdsApply", this.IDFcdsApply, fcdsConfig.BranchID.Value, isSavLog);
        }

        public bool SendMailToTeamMember(string proID, RoleTypeEnum rte, string idRole, string subject, bool isReleased)
        {
            HrVEgEmploy employ;
            MailAddressCollection to = new MailAddressCollection();
            MailAddressCollection cc = new MailAddressCollection();
            MailAddressCollection bcc = new MailAddressCollection() { FcdsHelper.SYSTEM_ADMIN_EMAIL };

            if (Config.Environment.Equals(@"TEST"))
            {
                // 測試環境
                to.Add(FcdsHelper.SYSTEM_ADMIN_EMAIL);
            }
            else
            {
                // 正式環境
                if (proID == FcdsHelper.PRO_RESPONDENT)
                {
                    to.Add(new CIvvCrewDb(this.RespondentID).Email);
                    cc.Add(new CIvvCrewDb(this.ApplicantID).Email);
                    subject = @"[FCDS] You have a duty swap request form to be review";
                }
                else if (proID == FcdsHelper.PRO_REVOKE)
                {
                    to.Add(new CIvvCrewDb(this.ApplicantID).Email);
                    cc.Add(new CIvvCrewDb(this.RespondentID).Email);
                    subject = @"[FCDS] Your duty swap request form have been revoked";
                }
                else
                {
                    DataTable dt = FetchAllTeamMember();
                    DataRow[] drs = null;

                    if (isReleased)
                    {

                    }
                    else
                    {
                        to.Add(new HrVEgEmploy(this.TaskOwnerID).Email);
                            
                        drs = dt.Select(@"EmployID <> '" + this.TaskOwnerID + @"'");
                        foreach (DataRow dr in drs)
                        {
                            cc.Add(new HrVEgEmploy(dr["EmployID"].ToString()).Email);
                        }

                        subject = @"[FCDS] You have a duty swap request form to be approve";
                    }

                    switch (rte)
                    {
                        case RoleTypeEnum.Flow:
                            /*
                            string[] vs = idRole.Split(',');
                            foreach (string s in vs)
                            {
                                drs = dt.Select(@"RoleType = '" + rte.ToString() + @"' AND IDRole = '" + s + @"'");
                                if (drs != null && drs.Length > 0)
                                {
                                    employ = new HrVEgEmploy(drs[0]["EmployID"].ToString());
                                    to.Add(employ.Email);
                                }
                            }
                            */
                            break;

                        case RoleTypeEnum.Notify:
                            foreach (DataRow dr in dt.Rows)
                            {
                                employ = new HrVEgEmploy(dr["EmployID"].ToString());
                                to.Add(employ.Email);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            string body = null;
            switch (rte)
            {
                case RoleTypeEnum.Flow:
                    body = @"Dear Sir,<br />The duty swap request application of Form No. " + this.IDFcdsApply + @" has been submitted to you.<br />Please check Flight Crew Duty Swap Application System<br />" + Config.WebRootUrl + @"FcdsApply.aspx?ID=" + this.IDFcdsApply;
                    break;

                case RoleTypeEnum.Notify:
                    body = @"Dear Sir,<br />The duty swap request application of Form No. XXXXX has been approved.";
                    break;

                default:
                    break;
            }

            MailUtility mu = new MailUtility() { Subject = subject, Body = body, To = to, Cc = cc, Bcc = bcc, IsBodyHtml = true };
            return mu.SendMail();
        }
    }

    /// <summary>
    /// 飛航組員任務互換申請主表單集合類別。
    /// </summary>
    public class FcdsApplyCollection : List<FcdsApply>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public FcdsApplyCollection()
        {

        }
    }
}