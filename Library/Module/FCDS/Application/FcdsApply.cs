using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;

using log4net;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

using Library.Component.BLL;
using Library.Component.DAL;
using Library.Component.Enums;
using Library.Component.Flow;
using Library.Component.Utility;
using Library.Module.FCDS.Configuration;
using Library.Module.FZDB;
using Library.Module.HRDB;

namespace Library.Module.FCDS.Application
{
    /// <summary>
    /// 飛航組員任務互換申請系統申請主表單類別。
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
         * VOID                         作廢
         * 
        */

        /// <summary>
        /// 申請單表單編號。
        /// </summary>
        public const string IDMODULEFORM = @"202011000001";

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
        /// 飛航組員任務互換申請者組員物件。
        /// </summary>
        public CIvvCrewDb ApplicantCrew { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請者機隊 ID。
        /// </summary>
        public string IDAcTypeApplicant { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請者機隊物件。
        /// </summary>
        public CIvvAircType CIvvAircType { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請者職級 ID。
        /// </summary>
        public int? IDCrewPosApplicant { get; set; }

        public CIvvPositions CIvvPositions { get; set; }

        /// <summary>
        /// 飛航組員任務換班申請系統之申請單所套用機隊職級設定物件表單編號。
        /// </summary>
        public string IDFcdsConfig { get; set; }

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
        /// 飛航組員任務互換受申請者物件。
        /// </summary>
        public CIvvCrewDb RespondentCrew { get; set; }

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
        /// 飛航組員任務互換申請單跨月班申請期限。
        /// </summary>
        public int? DeadlineOfAcrossMonth { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單跨月班申請期限日期。
        /// </summary>
        public DateTime? DeadlineOfAcrossMonthDate
        {
            get
            {
                // 若[DeadlineOfAcrossMonth]與[ApplicantPublishDate]皆為有效值，且[SwapScheduleMonth]不為 NULL 值或空字串
                if (this.DeadlineOfAcrossMonth.HasValue && this.ApplicantPublishDate.HasValue
                    && !string.IsNullOrEmpty(this.SwapScheduleMonth) && int.TryParse(this.SwapScheduleMonth.Substring(0, 4), out _) && int.TryParse(this.SwapScheduleMonth.Substring(4, 2), out _))
                {
                    return new DateTime(int.Parse(this.SwapScheduleMonth.Substring(0, 4)), int.Parse(this.SwapScheduleMonth.Substring(4, 2)), this.DeadlineOfAcrossMonth.Value);

                    //// 若當下月份等於[ApplicantPublishDate](申請者 AIMS 系統班表發佈截止日期)之月份
                    //if (DateTime.Now.Month == this.ApplicantPublishDate.Value.Month)
                    //{
                    //    // 跨月班期限日期為當下月份與跨月班期限日
                    //    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, this.DeadlineOfAcrossMonth.Value);
                    //}
                    //else
                    //{
                    //    // 跨月班期限日期為隔月份與跨月班期限日
                    //    return new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, this.DeadlineOfAcrossMonth.Value);
                    //}
                }

                return null;
            }
        }

        public string ProID { get; set; }

        public string TaskOwnerID { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單狀態代碼。
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 飛航組員任務互換申請單狀態代碼列舉型態。
        /// </summary>
        public FcdsHelper.FcdsApplyStatusCodeEnum? StatusCodeEnum { get; set; }

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
        /// 表單狀態代碼中文名稱，來源[fzdb].[FzTFcdsApply_Flow_Status].[StatusCName]欄位。
        /// </summary>
        public string StatusCName { get; set; }

        /// <summary>
        /// 表單狀態代碼英文名稱，來源[fzdb].[FzTFcdsApply_Flow_Status].[StatusEName]欄位。
        /// </summary>
        public string StatusEName { get; set; }

        /// <summary>
        /// 表單狀態代碼顯示名稱，來源[fzdb].[FzTFcdsApply_Flow_Status].[DisplayStatus]欄位。
        /// </summary>
        public string DisplayStatus { get; set; }

        public string IDFcdsApplyApprove { get; set; }
                
        public bool? IsApproval { get; set; }
        
        public string IDFcdsRejectReason { get; set; }
        
        public string Comments { get; set; }

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

        /// <summary>
        /// 用idFcdsApply取得該表單
        /// </summary>
        /// <param name="idFcdsApply"></param>
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
            this.ApplicantCrew = new CIvvCrewDb(this.ApplicantID);
            this.IDAcTypeApplicant = dr["IDAcTypeApplicant"].ToString();
            this.CIvvAircType = new CIvvAircType(this.IDAcTypeApplicant);

            if (dr["IDCrewPosApplicant"] != DBNull.Value && int.TryParse(dr["IDCrewPosApplicant"].ToString(), out _))
            {
                this.IDCrewPosApplicant = int.Parse(dr["IDCrewPosApplicant"].ToString());
                this.CIvvPositions = new CIvvPositions(this.IDCrewPosApplicant.Value);
            }


            this.IDFcdsConfig = dr["IDFcdsConfig"].ToString();

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
            this.RespondentCrew = new CIvvCrewDb(this.RespondentID);
            this.IDAcTypeRespondent = dr["IDAcTypeRespondent"].ToString();

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

            if (dr["StatusCode"] != DBNull.Value && Enum.TryParse<FcdsHelper.FcdsApplyStatusCodeEnum>(dr["StatusCode"].ToString(), out _))
            {
                this.StatusCodeEnum = Enum.Parse(typeof(FcdsHelper.FcdsApplyStatusCodeEnum), dr["StatusCode"].ToString()) as FcdsHelper.FcdsApplyStatusCodeEnum?;
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

            this.StatusCName = dr["StatusCName"].ToString();
            this.StatusEName = dr["StatusEName"].ToString();
            this.DisplayStatus = dr["DisplayStatus"].ToString();

            this.IDFcdsApplyApprove = dr["IDFcdsApplyApprove"].ToString();

            if (dr["IsApproval"] != DBNull.Value && bool.TryParse(dr["IsApproval"].ToString(), out _))
            {
                this.IsApproval = bool.Parse(dr["IsApproval"].ToString());
            }

            this.IDFcdsRejectReason = dr["IDFcdsRejectReason"].ToString();
            this.Comments = dr["Comments"].ToString();
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// 擷取飛航組員任務互換申請主表單。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT A.IDFcdsApply, A.BranchID, A.Version, A.ApplicantID, A.IDAcTypeApplicant, A.IDCrewPosApplicant, A.IDFcdsConfig, A.ApplicationDate, A.ApplicantPublishDate, A.ApplicationDeadline,
                            A.RespondentID, A.IDAcTypeRespondent, A.IDCrewPosRespondent, A.SwapScheduleMonth, A.SwapScheduleMonthItems, A.SwappableBeginDate,
                            /*IsApproval, IDFcdsRejectReason, Comments,*/ A.IsApplicantRevoke, A.LeadWorkdays, A.DeadlineOfAcrossMonth,
                            A.ProID, A.TaskOwnerID, A.StatusCode,
                            A.CreateBy, A.CreateStamp, A.UpdateBy, A.UpdateStamp,
                            FS.StatusCName, FS.StatusEName, FS.DisplayStatus,
                            AA.IDFcdsApplyApprove, AA.IsApproval, AA.IDFcdsRejectReason, AA.Comments
                     FROM   fzdb.fztfcdsapply A
                            LEFT JOIN fzdb.FzTFcdsApply_Flow_Status FS ON A.StatusCode  = FS.StatusCode
                            LEFT JOIN fzdb.FzTFcdsApplyApprove      AA ON A.IDFcdsApply = AA.IDFcdsApply";
        }

        /// <summary>
        /// 使用完整IDFcdsApply取得特定單號
        /// </summary>
        /// <returns type="DataTable"></returns>
        private DataTable FetchByIDFcdsApply()
        {
            string sql = BuildFetchCommandString() + @" WHERE A.IDFcdsApply = :pIDFcdsApply";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];
            
            return dt;
        }
        /// <summary>
        /// 20221121 648267:拒絕信件中加入理由
        /// 使用Reason ID取得Reason
        /// </summary>
        /// <param name="idreason"></param>
        /// <returns></returns>
        private string FetchReasonByIDRejectReason(string idreason) 
        {
            string query_string = @"select * from fzdb.fztfcdsrejectreason r WHERE r.idfcdsrejectreason = :pIDFcdsRejectReason";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsRejectReason", idreason) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, query_string,ops.ToArray()).Tables[0];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                return dr["rejectreason"].ToString();
            }
            return "";
        }

        /// <summary>
        /// 擷取所有飛航組員任務互換申請系統申請主表單類別。
        /// </summary>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
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
        /// 擷取待審核之飛航組員任務互換申請主表單。
        /// </summary>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <param name="isAdmin">是否為系統管理者群組或系統管理員</param>
        /// <returns></returns>
        public static object FetchToBeReview(ReturnObjectTypeEnum rot, bool isAdmin)
        {
            string sql = BuildFetchCommandString() + @" WHERE";
            if (!isAdmin) sql += @" A.StatusCode = '" + FcdsHelper.FcdsApplyStatusCodeEnum.OP_STAFF + @"' AND";
            sql += @" A.ProID <> :pProID AND A.ProID IS NOT NULL AND A.TaskOwnerID IS NOT NULL";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pProID", FcdsHelper.PRO_RESPONDENT) };
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

        /// <summary>
        /// 以飛航組員申請者員工編號擷取飛航組員任務換班申請主表單。
        /// </summary>
        /// <param name="applicantID">飛航組員申請者員工編號</param>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchByApplicantID(string applicantID, ReturnObjectTypeEnum rot)
        {
            // 20221107 648267:調整[被申請人]可申請的條件
            string sql = BuildFetchCommandString() + @" WHERE A.ApplicantID = :pApplicantID OR A.Respondentid = :pApplicantID";
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

        /// <summary>
        /// 以飛航組員申請者或受申請者擷取飛航組員任務換班申請主表單。
        /// </summary>
        /// <param name="crewID">登入之飛航組員員工編號</param>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchByApplicantIDOrRespondentID(string crewID, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE A.ApplicantID = :pApplicantID OR A.RespondentID = :pRespondentID ORDER BY A.ApplicationDeadline DESC, A.IDFcdsApply DESC";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pApplicantID", crewID), new OracleParameter("pRespondentID", crewID) };
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

        /// <summary>
        /// 擷取已逾期之飛航組員任務換班申請主表單。
        /// </summary>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchExpiry(ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE A.ApplicationDeadline < SYSDATE AND A.StatusCode <> '" + FlowStatus.StatusCodeEnum.RELEASED.ToString() + @"'";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

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
        /// 擷取飛航組員任務互換申請主表單流程團隊與結案通知團隊。
        /// </summary>
        /// <returns></returns>
        public DataTable FetchAllTeamMember()
        {
            string sql = @"SELECT RD.RoleType, RT.IDFlowTeam, RT.BranchID, RT.IDRole, RT.EmployID, RT.CreateBy, RT.CreateStamp, RT.UpdateBy, RT.UpdateStamp FROM fzdb.fztfcdsconfig_role_def RD LEFT JOIN fzdb.fztfcdsapply_role_team RT ON RD.IDRole = RT.IDRole WHERE RT.IDFlowTeam = :pIDFlowTeam";
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
        public static List<string> GetAllowCreateString(string applicantID, string swapMonth, FcdsConfig fcdsConfig)
        {
            List<string> vs = new List<string>();

            FcdsApplyCollection fcdsApplies = FetchByApplicantID(applicantID, Library.Component.Enums.ReturnObjectTypeEnum.Collection) as FcdsApplyCollection;
            DateTime monthStartDate = DateTimeUtility.GetTheBeginOfMonth(DateTime.Now);
            DateTime monthEndDate = DateTimeUtility.GetTheEndOfMonth(DateTime.Now);

            // 20221107 648267:Find [Applicant] application in this month
            var colThisMonth = fcdsApplies.Where(o => o.SwapScheduleMonth == swapMonth && o.ApplicantID == applicantID).ToList();
            // 20221103 648267:Add my return quota( 1. Revoke 2. Respondent return 3. release and not approve )
            var colThisMonthReturn = fcdsApplies.Where(o => o.SwapScheduleMonth == swapMonth && o.ApplicantID == applicantID &&
                                                           (o.StatusCode == "REVOKE" || o.StatusCode == "RESPONDENT_RETURN" || (o.StatusCode == "RELEASED" && o.IsApproval == false))
                                                      ).ToList();
            //var colThisMonth = fcdsApplies.Where(o => o.ApplicationDate >= monthStartDate && o.ApplicationDate <= monthEndDate).ToList();
            // 20221107 648267:Responding applications in progress, then block submission
            var colThisMonthRespond = fcdsApplies.Where(o=>o.SwapScheduleMonth == swapMonth && o.RespondentID == applicantID && 
                                                          (o.StatusCode != "REVOKE" && o.StatusCode != "RESPONDENT_RETURN" && o.StatusCode != "RELEASED")
                                                       ).ToList();
            if (colThisMonth != null)
            {
                // 檢核飛航組員當月申請次數 : 已申請-退回 >= 額度
                if ( (colThisMonth.Count - colThisMonthReturn.Count) >= fcdsConfig.NumOfMonth)
                {
                    vs.Add("The number of application forms you have applied exceed the maximum allowed this month.");
                }

                // 檢核飛航組員當月是否有流程中(一次一單否)之申請單
                if (fcdsConfig.IsOneReqATime.HasValue && fcdsConfig.IsOneReqATime.Value ? colThisMonth.Where(o => o.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.RELEASED && o.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.REVOKE && o.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.RESPONDENT_RETURN).Count() > 0 : false)
                {
                    vs.Add("You still have a request that is proceeding.");
                }
            }
            if (colThisMonthRespond.Count > 0)
            {
                vs.Add("As a respondent, you still have a request that is proceeding.");
            }

            return vs;
        }

        /// <summary>
        /// 擷取飛航組員任務互換申請主表單之日誌紀錄。
        /// </summary>
        /// <returns></returns>
        public DataTable FetchLog()
        {
            string sql = @"SELECT AL.IDFcdsApply, AL.BranchID, AL.Version, AL.ApplicantID, AL.IDAcTypeApplicant, AL.IDCrewPosApplicant, AL.IDFcdsConfig, AL.ApplicationDate, AL.ApplicantPublishDate, AL.ApplicationDeadline,
                                  AL.RespondentID, AL.IDAcTypeRespondent, AL.IDCrewPosRespondent, AL.SwapScheduleMonth, AL.SwapScheduleMonthItems, AL.SwappableBeginDate,
                                  /*IsApproval, IDFcdsRejectReason, Comments, IsApplicantRevoke,*/ AL.LeadWorkdays, AL.DeadlineOfAcrossMonth,
                                  AL.ProID, AL.TaskOwnerID, AL.StatusCode,
                                  AL.CreateBy, AL.CreateStamp, AL.UpdateBy, AL.UpdateStamp,
                                  FS.StatusCName, FS.StatusEName, FS.DisplayStatus
                           FROM   fzdb.fztfcdsapply_log AL
                                  INNER JOIN fzdb.FzTFcdsApply_Flow_Status FS ON AL.StatusCode = FS.StatusCode
                           WHERE  IDFcdsApply = :pIDFcdsApply
                           ORDER BY BranchID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        private DataTable FetchApprovalHistory1()
        {
            string sql = @"SELECT FS.DisplayStatus                                                AS Task,
                                  MIN(AL.UpdateStamp)                                             AS StartTime,
                                  MAX(AL.UpdateStamp)                                             AS EndTime,
                                  LISTAGG(FS.ProID, ',') WITHIN GROUP (ORDER BY AL.BranchID)      AS ProIDs,
                                  FS.DisplayStatus                                                AS Action,--LISTAGG(AL.StatusCode, ',') WITHIN GROUP (ORDER BY AL.BranchID)
                                  LISTAGG(AL.UpdateBy, ',') WITHIN GROUP (ORDER BY AL.BranchID)   AS Signee,
                                  NULL                                                            AS Comments
                           FROM   fzdb.fztfcdsapply_log AL
                                  INNER JOIN fzdb.FzTFcdsApply_Flow_Status FS ON AL.StatusCode  = FS.StatusCode
                                  LEFT  JOIN fzdb.FzTFcdsApplyApprove      AA ON AL.IDFcdsApply = AA.IDFcdsApply
                           WHERE  AL.IDFcdsApply = :pIDFcdsApply
                           GROUP BY FS.DisplayStatus
                           ORDER BY ProIDs";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 擷取飛航組員任務互換申請主表單簽核流程記錄。
        /// </summary>
        /// <param name="isDisplay">是否供飛航組員檢視</param>
        /// <returns></returns>
        public DataTable FetchApprovalHistory(bool isDisplay)
        {
            string sql = @"SELECT LISTAGG(AH.BranchID, ',') WITHIN GROUP (ORDER BY AH.BranchID)                AS BranchID
                                  ,AH.Task
                                  ,LISTAGG(AH.ProID, ',') WITHIN GROUP (ORDER BY AH.BranchID)                  AS ProID";

            if (isDisplay)
            {
                sql += @"         ,CASE
                                     WHEN INSTR(LISTAGG(ProID, ',') WITHIN GROUP (ORDER BY BranchID), 'PRO1015313279') > 0 THEN '承辦人'
                                     WHEN INSTR(LISTAGG(ProID, ',') WITHIN GROUP (ORDER BY BranchID), 'PRO1015313280') > 0 THEN '承辦人'
                                     WHEN INSTR(LISTAGG(ProID, ',') WITHIN GROUP (ORDER BY BranchID), 'PRO1015313281') > 0 THEN '承辦人'
                                     ELSE LISTAGG(Signee, ',') WITHIN GROUP (ORDER BY BranchID)
                                  END AS Signee";
            }
            else
            {
                sql += @"         ,LISTAGG(AH.Signee, ',') WITHIN GROUP (ORDER BY AH.BranchID)                 AS Signee";
            }

            sql += @"             ,MAX(AH.Action) KEEP(DENSE_RANK FIRST ORDER BY AH.Action NULLS FIRST)        AS Action
                                  --,LISTAGG(AH.Action, ',') WITHIN GROUP (ORDER BY BranchID)                    AS Action
                                  ,LISTAGG(AH.Comments, ',') WITHIN GROUP (ORDER BY AH.BranchID)               AS Comments
                                  ,MIN(AH.StartTime)                                                           AS StartTime
                                  ,MAX(AH.EndTime) KEEP(DENSE_RANK FIRST ORDER BY AH.EndTime DESC NULLS FIRST) AS EndTIme
                                  --,MAX(AH.EndTime)                                                             AS EndTIme   
                           FROM   (
                                   SELECT AL.BranchID
                                          ,FS.ProID";

            if (isDisplay)
            {
                sql += @"                 ,FS.DisplayTaskName                                                           AS Task";
            }
            else
            {
                sql += @"                 ,FS.TaskName                                                                  AS Task";
            }

            sql += @"
                                          ,AL.UpdateBy                                                                  AS Signee
                                          ,FS.ActionName                                                                AS Action
                                          ,NULL                                                                         AS Comments
                                          ,COALESCE(LAG(AL.UpdateStamp) OVER (ORDER BY AL.BranchID), AL.UpdateStamp)    AS StartTime
                                          ,AL.UpdateStamp                                                               AS EndTime
                                   FROM   fzdb.fztfcdsapply_log AL --WHERE AL.Idfcdsapply='FCDSA-202101-00043'
                                          INNER JOIN fzdb.FzTFcdsApply_Flow_Status FS ON AL.StatusCode = FS.StatusCode
                                   WHERE  AL.IDFcdsApply = :pIDFcdsApply --ORDER BY AL.BranchID
                                   UNION ALL
                                   SELECT A.BranchID
                                          ,FS.NextProID";

            if (isDisplay)
            {
                sql += @"                 ,FS.DisplayStatus                                                             AS Task";
            }
            else
            {
                sql += @"                 ,FS.StatusCName                                                               AS Task";
            }

            sql += @"                     ,A.Taskownerid                                                                AS Signee
                                          ,NULL                                                                         AS Action
                                          ,NULL                                                                         AS Comments
                                          ,A.UpdateStamp                                                                AS StartTime
                                          ,NULL                                                                         AS EndTime
                                   FROM   fzdb.fztfcdsapply A
                                          INNER JOIN fzdb.FzTFcdsApply_Flow_Status FS ON A.ProID = FS.ProID
                                   WHERE  A.IDFcdsApply = :pIDFcdsApply
                                   ORDER BY BranchID, EndTime
                                  ) AH
                           GROUP BY AH.Task
                           ORDER BY ProID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            if (!isDisplay)
            {
                string proID = dt.Rows[dt.Rows.Count - 2]["ProID"].ToString();
                if (ProID != FcdsHelper.PRO_OP_GENERAL_MANAGER)
                {
                    FlowStatus fs = FlowStatus.FetchByProID(@"Application", @"FcdsApply", proID);
                    dt.Rows[dt.Rows.Count - 1]["Task"] = fs.StatusCName;
                }
            }

            return dt;
        }

        private DataTable FetchApprovalHistory2()
        {
            string sql = @"SELECT AL.BranchID
                                  ,FS.TaskName                                                                  AS Task
                                  ,AL.UpdateBy                                                                  AS Signee
                                  ,FS.ActionName                                                                AS Action
                                  ,NULL                                                                         AS Comments
                                  ,COALESCE(LAG(AL.UpdateStamp) OVER (ORDER BY AL.BranchID), AL.UpdateStamp)    AS StartTime
                                  ,AL.UpdateStamp                                                               AS EndTime
                           FROM   fzdb.fztfcdsapply_log AL --WHERE AL.Idfcdsapply='FCDSA-202101-00043'
                                  INNER JOIN fzdb.FzTFcdsApply_Flow_Status FS ON AL.StatusCode = FS.StatusCode
                           WHERE  AL.IDFcdsApply = :pIDFcdsApply --ORDER BY AL.BranchID
                           UNION ALL
                           SELECT A.BranchID
                                  ,FS.DisplayStatus                                                             AS Task
                                  ,A.Taskownerid                                                                AS Signee
                                  ,NULL                                                                         AS Action
                                  ,NULL                                                                         AS Comments
                                  ,A.UpdateStamp                                                                AS StartTime
                                  ,NULL                                                                         AS EndTime
                           FROM   fzdb.fztfcdsapply A
                                  INNER JOIN fzdb.FzTFcdsApply_Flow_Status FS ON A.ProID = FS.ProID
                           WHERE  A.IDFcdsApply = :pIDFcdsApply
                           ORDER BY BranchID, EndTime";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public ApprovalHistoryCollection GetDispalyApprovalHistories()
        {
            ApprovalHistoryCollection col = new ApprovalHistoryCollection();
            DataTable dt = FetchLog();
            var query = from ah in dt.AsEnumerable()
                        group ah by ah.Field<string>("DisplayStatus") into g
                        select new
                        {
                        }
                        ;
            foreach (DataRow dr in dt.Rows)
            {
                ApprovalHistory obj = new ApprovalHistory();
                //obj
            }

            return col;
        }

        /// <summary>
        /// 儲存飛航組員任務互換申請主表單。
        /// </summary>
        /// <param name="enumPageMode">頁面模式列舉型態</param>
        /// <param name="isSaveLog">是否儲存日誌</param>
        /// <param name="isSaveRoleTeam">是否儲存表單團隊成員</param>
        /// <returns></returns>
        public bool Save(PageMode.PageModeEnum enumPageMode, bool isSaveLog, bool isSaveRoleTeam)
        {
            bool result = false;
            switch (enumPageMode)
            {
                case PageMode.PageModeEnum.Create:
                    result = Insert();
                    break;

                case PageMode.PageModeEnum.Edit:
                case PageMode.PageModeEnum.Task:
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

            if (isSaveRoleTeam)
            {
                result = SaveRoleTeam(enumPageMode, isSaveLog);
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
                                                         ApplicantID, IDAcTypeApplicant, IDCrewPosApplicant, IDFcdsConfig, ApplicationDate, ApplicationDeadline,
                                                         RespondentID, IDAcTypeRespondent, IDCrewPosRespondent,
                                                         SwapScheduleMonth, SwapScheduleMonthItems, IsApplicantRevoke, LeadWorkdays, DeadlineOfAcrossMonth,
                                                         ProID, TaskOwnerID, StatusCode,
                                                         CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDFcdsApply, :pBranchID, :pVersion,
                                   :pSwappableBeginDate, :pApplicantpublishDate,
                                   :pApplicantID, :pIDAcTypeApplicant, :pIDCrewPosApplicant, :pIDFcdsConfig, :pApplicationDate, :pApplicationDeadline,
                                   :pRespondentID, :pIDAcTypeRespondent, :pIDCrewPosRespondent,
                                   :pSwapScheduleMonth, :pSwapScheduleMonthItems, :pIsApplicantRevoke, :pLeadWorkdays, :pDeadlineOfAcrossMonth,
                                   :pProID, :pTaskOwnerID, :pStatusCode,
                                   :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsApply", this.IDFcdsApply), new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pSwappableBeginDate", this.SwappableBeginDate.Value), new OracleParameter("pApplicantpublishDate", this.ApplicantPublishDate.Value),
                                                                        new OracleParameter("pApplicantID", this.ApplicantID), new OracleParameter("pIDAcTypeApplicant", this.IDAcTypeApplicant),
                                                                        new OracleParameter("pIDCrewPosApplicant", this.IDCrewPosApplicant), new OracleParameter("pIDFcdsConfig", this.IDFcdsConfig),
                                                                        new OracleParameter("pApplicationDate", this.ApplicationDate), new OracleParameter("pApplicationDeadline", this.ApplicationDeadline),
                                                                        new OracleParameter("pRespondentID", this.RespondentID), new OracleParameter("pIDAcTypeRespondent", this.IDAcTypeRespondent), new OracleParameter("pIDCrewPosRespondent", this.IDCrewPosRespondent),
                                                                        new OracleParameter("pSwapScheduleMonth", this.SwapScheduleMonth), new OracleParameter("pSwapScheduleMonthItems", this.SwapScheduleMonthItems),
                                                                        //new OracleParameter("pIsApproval", this.IsApproval.Value.ToString()), new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason), new OracleParameter("pComments", this.Comments),
                                                                        new OracleParameter("pIsApplicantRevoke", this.IsApplicantRevoke.HasValue ? this.IsApplicantRevoke.Value.ToString() : null), new OracleParameter("pLeadWorkdays", this.LeadWorkdays), new OracleParameter("pDeadlineOfAcrossMonth", this.DeadlineOfAcrossMonth),
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
                                  ApplicantID = :pApplicantID, IDAcTypeApplicant = :pIDAcTypeApplicant, IDCrewPosApplicant = :pIDCrewPosApplicant, IDFcdsConfig = :pIDFcdsConfig, ApplicationDate = :pApplicationDate, ApplicationDeadline = :pApplicationDeadline,
                                  RespondentID = :pRespondentID, IDAcTypeRespondent = :pIDAcTypeRespondent, IDCrewPosRespondent = :pIDCrewPosRespondent,
                                  SwapScheduleMonth = :pSwapScheduleMonth, SwapScheduleMonthItems = :pSwapScheduleMonthItems, IsApplicantRevoke = :pIsApplicantRevoke, LeadWorkdays = :pLeadWorkdays, DeadlineOfAcrossMonth = :pDeadlineOfAcrossMonth,
                                  ProID = :pProID, TaskOwnerID = :pTaskOwnerID, StatusCode = :pStatusCode,
                                  UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDFcdsApply = :pIDFcdsApply";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pSwappableBeginDate", this.SwappableBeginDate.Value), new OracleParameter("pApplicantpublishDate", this.ApplicantPublishDate.Value),
                                                                        new OracleParameter("pApplicantID", this.ApplicantID), new OracleParameter("pIDAcTypeApplicant", this.IDAcTypeApplicant),
                                                                        new OracleParameter("pIDCrewPosApplicant", this.IDCrewPosApplicant), new OracleParameter("pIDFcdsConfig", this.IDFcdsConfig),
                                                                        new OracleParameter("pApplicationDate", this.ApplicationDate), new OracleParameter("pApplicationDeadline", this.ApplicationDeadline),
                                                                        new OracleParameter("pRespondentID", this.RespondentID), new OracleParameter("pIDAcTypeRespondent", this.IDAcTypeRespondent), new OracleParameter("pIDCrewPosRespondent", this.IDCrewPosRespondent),
                                                                        new OracleParameter("pSwapScheduleMonth", this.SwapScheduleMonth), new OracleParameter("pSwapScheduleMonthItems", this.SwapScheduleMonthItems),
                                                                        //new OracleParameter("pIsApproval", this.IsApproval.Value.ToString()), new OracleParameter("pIDFcdsRejectReason", this.IDFcdsRejectReason), new OracleParameter("pComments", this.Comments),
                                                                        new OracleParameter("pIsApplicantRevoke", this.IsApplicantRevoke.HasValue ? this.IsApplicantRevoke.Value.ToString() : null), new OracleParameter("pLeadWorkdays", this.LeadWorkdays), new OracleParameter("pDeadlineOfAcrossMonth", this.DeadlineOfAcrossMonth),
                                                                        new OracleParameter("pProID", this.ProID), new OracleParameter("pTaskOwnerID", this.TaskOwnerID), new OracleParameter("pStatusCode", this.StatusCode),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp),
                                                                        new OracleParameter("pIDFcdsApply", this.IDFcdsApply)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存飛航組員任務互換申請主表單日誌。
        /// *** 此LOG schema完全跟表單相同，只會Insert不會Update ***
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
        /// <param name="enumPageMode">頁面模式列舉型態</param>
        /// <param name="fcdsConfig">飛航組員任務換班申請系統之機隊職級設定物件</param>
        /// <param name="isSavLog">是否儲存日誌</param>
        /// <returns></returns>
        public bool SaveRoleTeam(PageMode.PageModeEnum enumPageMode, bool isSavLog)
        {
            FcdsConfig fcdsConfig = new FcdsConfig(this.IDFcdsConfig);
            return ModuleFormRoleTeam.SaveFormRoleTeam(enumPageMode, @"Configuration", @"FcdsConfig", fcdsConfig.IDFcdsConfig, @"Application", @"FcdsApply", this.IDFcdsApply, this.BranchID.Value, isSavLog);
        }

        /// <summary>
        /// 作廢逾期申請單。
        /// </summary>
        /// <param name="updateBy">更新者</param>
        /// <returns></returns>
        public bool VoidForm(string updateBy)
        {
            //this.StatusCName = 
            //this.StatusEName = 
            this.StatusCode = FcdsHelper.FcdsApplyStatusCodeEnum.VOID.ToString();
            this.StatusCodeEnum = FcdsHelper.FcdsApplyStatusCodeEnum.VOID;
            this.DisplayStatus = new FlowStatus(@"Application", @"FcdsApply", FcdsHelper.FcdsApplyStatusCodeEnum.VOID.ToString()).DisplayStatus;
            this.UpdateBy = updateBy;
            this.UpdateStamp = DateTime.Now;

            return Save(PageMode.PageModeEnum.Edit, true, true) && SendVoidMail();
        }

        /// <summary>
        /// 寄送電子郵件通知審核人。
        /// 20221115 648267:新增TEST環境寄信功能
        /// </summary>
        /// <returns></returns>
        public bool SendMailToTeamMember(string currentProID)
        {
            string subject = null;
            //MailAddress ma = new MailAddress();
            MailAddressCollection to = new MailAddressCollection();
            MailAddressCollection cc = new MailAddressCollection();
            MailAddressCollection bcc = new MailAddressCollection() { FcdsHelper.System_Administrator_Email };
            
            // Default Email content : submission
            string body = @"<p>Greetings,<br />The duty swap request application of Form No. " + this.IDFcdsApply + @" has been submitted to you on " + this.UpdateStamp.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) + @".</p>";

            #region Email Title Prefix
            if (Config.IsProduction)
            {
                // 正式環境:信件標題前綴字  *** Notification ***XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //subject = "*** Notification ***";
            }
            else
            {
                // 非正式環境(測試或開發)
                subject = @"***TEST MAIL***";
                to.Add(FcdsHelper.System_Administrator_Email); // Store Data:fzdb.fztbllmoduleform 

                if (Config.Environment == @"TEST")
                    to.Add(FcdsHelper.Process_Owner_Email); // Store Data:fzdb.fztbllmoduleform
            }
            #endregion Email Title Prefix


            if (currentProID == FcdsHelper.PRO_REVOKE) // 1. Revoke
            {
                if (Config.IsProduction && Config.Environment == "PROD")
                {
                    to.Add(new CIvvCrewDb(this.ApplicantID).Email);
                    cc.Add(new CIvvCrewDb(this.RespondentID).Email);
                }
                else if (Config.Environment == "TEST")
                {
                    to.Add(new CIvvCrewDb(this.ApplicantID).Email);
                    cc.Add(new CIvvCrewDb(this.RespondentID).Email);
                }
                subject += $@" [FCDS] Your duty swap request form {this.IDFcdsApply} have been revoked";
                body = $@"<p>Greetings,<br />The duty swap request application of Form No. {this.IDFcdsApply} has been revoked by {this.ApplicantID} on {this.UpdateStamp.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"))}.</p>";
                body += @"<p>Please check Flight Crew Duty Swap Application System<br /><a href=""" + Config.WebRootUrl + @"Module/Application/FcdsApply.aspx?ID=" + this.IDFcdsApply + @""">" + this.IDFcdsApply + @"</a></p>";
            }
            else
            {
                if (this.StatusCodeEnum == FcdsHelper.FcdsApplyStatusCodeEnum.RESPONDENT) // 2. Respondent
                {
                    if (Config.IsProduction && Config.Environment == "PROD")
                    {
                        to.Add(new CIvvCrewDb(this.RespondentID).Email);
                        //cc.Add(new CIvvCrewDb(this.ApplicantID).Email);
                    }
                    else if (Config.Environment == "TEST") 
                    {
                        to.Add(new CIvvCrewDb(this.RespondentID).Email);
                    }

                    subject += $@" [FCDS] You have a duty swap request form to approve";
                    body = @"<p>Greetings,<br />The duty swap request application of Form No. " + this.IDFcdsApply + @" has been submitted to you on " + this.UpdateStamp.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) + @".</p>";
                    body += @"<p>Please check Flight Crew Duty Swap Application System<br /><a href=""" + Config.WebRootUrl + @"Module/Application/FcdsApply.aspx?ID=" + this.IDFcdsApply + @""">" + this.IDFcdsApply + @"</a></p>";
                }
                else if (this.StatusCodeEnum == FcdsHelper.FcdsApplyStatusCodeEnum.RESPONDENT_RETURN) // 3. Respondent return
                {
                    if (Config.IsProduction)
                    {
                        to.Add(new CIvvCrewDb(this.ApplicantID).Email);
                        cc.Add(new CIvvCrewDb(this.RespondentID).Email);
                    }
                    else if (Config.Environment =="TEST")
                    {
                        to.Add(new CIvvCrewDb(this.ApplicantID).Email);
                    }

                    subject += @" [FCDS] Your duty swap request form has been returned";
                    body = @"<p>Greetings,<br />Your duty swap request application of Form No. " + this.IDFcdsApply + @" has been returned by " + new CIvvCrewDb(this.RespondentID).DisplayName + @" on " + this.UpdateStamp.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) + @".</p>";
                }
                else
                {
                    if (Config.IsProduction) // 4. TaskOwner approval
                    {
                        to.Add(new HrVEgEmploy(this.TaskOwnerID).Email);

                        DataTable dt = FetchAllTeamMember();
                        var query = from tm in dt.AsEnumerable()
                                    where tm.Field<string>("EmployID") != this.TaskOwnerID && tm.Field<string>("RoleType") == RoleTypeEnum.Flow.ToString()
                                    select tm;
                        foreach (var q in query)
                        {
                            cc.Add(new HrVEgEmploy(q.Field<string>("EmployID")).Email);
                        }

                        //DataRow[] drs = dt.Select(@"EmployID <> '" + this.TaskOwnerID + @"'");
                        //foreach (DataRow dr in drs)
                        //{
                        //    cc.Add(new HrVEgEmploy(dr["EmployID"].ToString()).Email);
                        //}
                    }
                    else if (Config.Environment == "TEST")
                    {
                        to.Add(new HrVEgEmploy(this.TaskOwnerID).Email);
                    }

                    subject += @" [FCDS] You have a duty swap request form to approve";
                    body += @"<p>Please check Flight Crew Duty Swap Application System<br /><a href=""" + Config.AdminWebRootUrl + @"Module/Application/FcdsApply.aspx?ID=" + this.IDFcdsApply + @""">" + this.IDFcdsApply + @"</a></p>";
                }
            }

            MailUtility mu = new MailUtility() { Subject = subject, Body = body, To = to, Cc = cc, Bcc = bcc, IsBodyHtml = true };
            bool result = false;
            Exception ex = null;
            try
            {
                mu.SendMail();
                result = true;
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                SaveMailLog(@"SendMailToTeamMember", currentProID, subject, to, cc, bcc, body, result, ex);
            }

            return result;
        }

        /// <summary>
        /// 寄送結案電子郵件通知予申請人、受申請人、流程團隊與結案通知團隊。
        /// </summary>
        /// <returns></returns>
        public bool SendReleaseMail()
        {
            string subject = null;
            bool result = false;
            Exception ex = null;
            MailUtility mu = null;
            MailAddressCollection to = new MailAddressCollection();
            MailAddressCollection cc = new MailAddressCollection();
            MailAddressCollection bcc = new MailAddressCollection() { FcdsHelper.System_Administrator_Email };
            DataTable dt = FetchAllTeamMember();

            #region 申請人、受申請人
            if (Config.IsProduction)
            {
                // 正式環境
                to.Add(new CIvvCrewDb(this.ApplicantID).Email);
                cc.Add(new CIvvCrewDb(this.RespondentID).Email);
            }
            else if(Config.Environment =="TEST")
            {
                // 非正式環境(測試或開發)
                subject = @"***TEST MAIL***";
                //to.Add(FcdsHelper.System_Administrator_Email);
                //to.Add(FcdsHelper.Process_Owner_Email);
                to.Add(new CIvvCrewDb(this.ApplicantID).Email);
                to.Add(new CIvvCrewDb(this.RespondentID).Email);
            }

            subject += $@" [FCDS] Your duty swap request form {this.IDFcdsApply} has been completed";
            string body = $@"<p>Greetings,<br />The duty swap request application of Form No. {this.IDFcdsApply} has been "+ (this.IsApproval.HasValue && this.IsApproval.Value ? "approved" : "disagreed") + " by Crew Scheduling Dept on " + this.UpdateStamp.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) + @".</p>";
            //20221121 648267:信件內容新增拒絕原因
            if ( !(this.IsApproval.HasValue && this.IsApproval.Value) )
            {
                body += $@"<br /><p>Reason : {FetchReasonByIDRejectReason(this.IDFcdsRejectReason)}</p>";
                body += $@"<br /><p>Remark : {this.Comments}</p>";
            }
                
            body += @"<br /><p>Please check Flight Crew Duty Swap Application System<br /><a href=""" + Config.WebRootUrl + @"Module/Application/FcdsApply.aspx?ID=" + this.IDFcdsApply + @""">" + this.IDFcdsApply + @"</a></p>";

            mu = new MailUtility() { Subject = subject, Body = body + (Config.IsProduction ? string.Empty : @"<p>Note: This email is for applicant and respondent.</p>"), To = to, Cc = cc, Bcc = bcc, IsBodyHtml = true };          
            try
            {
                mu.SendMail();
                result = true;
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                SaveMailLog(@"SendReleaseMail to Applicant and Respondent", null, subject, to, cc, bcc, body, result, ex);
            }
            #endregion 申請人、受申請人

            #region 流程團隊
            result = false;
            ex = null;
            to.Clear();
            cc.Clear();
            // TODO:PROD和TEST可以寄信給審核人，DEVE可改成ADMIN
            if (Config.IsProduction && Config.Environment == "PROD")
            {
                var query = from tm in dt.AsEnumerable()
                            where tm.Field<string>("RoleType").Equals(RoleTypeEnum.Flow.ToString(), StringComparison.OrdinalIgnoreCase)
                            select tm;
                foreach (var q in query)
                {
                    to.Add(new HrVEgEmploy(q.Field<string>("EmployID")).Email);
                }
            }
            else if(Config.Environment == "TEST")
            {
                var query = from tm in dt.AsEnumerable()
                            where tm.Field<string>("RoleType").Equals(RoleTypeEnum.Flow.ToString(), StringComparison.OrdinalIgnoreCase)
                            select tm;
                foreach (var q in query)
                {
                    to.Add(new HrVEgEmploy(q.Field<string>("EmployID")).Email);
                }
            }
            else
            {
                to.Add( FcdsHelper.Process_Owner_Email ); // 轉寄給Process Owner Email
            }

            mu = new MailUtility() { Subject = subject, 
                                     Body = body + (this.IsApproval.HasValue && this.IsApproval.Value ? @"<p>Please swap both crews' shedule in AIMS system.</p>" : string.Empty) + (Config.IsProduction ? string.Empty : @"<p>Note: This email is for Flow Team member,</p>"), 
                                     To = to, 
                                     Cc = cc, 
                                     Bcc = bcc, 
                                     IsBodyHtml = true };
            try
            {
                mu.SendMail();
                result = true;
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                SaveMailLog(@"SendReleaseMail to Flow Team(OP Dept.)", null, subject, to, cc, bcc, body, result, ex);
            }
            #endregion 流程團隊

            #region 結案通知團隊
            result = false;
            ex = null;
            to.Clear();
            cc.Clear();
            if (Config.IsProduction && Config.Environment =="PROD")
            {
                var query = from tm in dt.AsEnumerable()
                            where tm.Field<string>("RoleType").Equals(RoleTypeEnum.Notify.ToString(), StringComparison.OrdinalIgnoreCase)
                            select tm;
                foreach (var q in query)
                {
                    to.Add(new HrVEgEmploy(q.Field<string>("EmployID")).Email);
                }
            }
            else if (Config.Environment == "TEST")
            {
                var query = from tm in dt.AsEnumerable()
                            where tm.Field<string>("RoleType").Equals(RoleTypeEnum.Notify.ToString(), StringComparison.OrdinalIgnoreCase)
                            select tm;
                foreach (var q in query)
                {
                    to.Add(new HrVEgEmploy(q.Field<string>("EmployID")).Email);
                }
            }
            else
            {
                to.Add(FcdsHelper.Process_Owner_Email);
            }

            mu = new MailUtility() { Subject = subject, Body = body + (Config.IsProduction ? string.Empty : @"Note: This email is for Notify Team member."), To = to, Cc = cc, Bcc = bcc, IsBodyHtml = true };
            try
            {
                mu.SendMail();
                result = true;
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                SaveMailLog(@"SendReleaseMail to Notify Team(OC Dept.)", null, subject, to, cc, bcc, body, result, ex);
            }
            #endregion 結案通知團隊

            return true;
        }

        /// <summary>
        /// 寄送逾期作廢電子郵件通知予申請人、受申請人。
        /// </summary>
        /// <returns></returns>
        private bool SendVoidMail()
        {
            string subject = null;
            bool result = false;
            Exception ex = null;
            MailUtility mu = null;
            MailAddressCollection to = new MailAddressCollection();
            MailAddressCollection cc = new MailAddressCollection();
            MailAddressCollection bcc = new MailAddressCollection() { FcdsHelper.System_Administrator_Email };
            DataTable dt = FetchAllTeamMember();

            if (Config.IsProduction && Config.Environment == "PROD")
            {
                // 正式環境
                to.Add(new CIvvCrewDb(this.ApplicantID).Email);
                cc.Add(new CIvvCrewDb(this.RespondentID).Email);
            }
            else if(Config.Environment == "TEST")
            {
                // 非正式環境(測試)
                subject = @"***TEST MAIL***";
                to.Add(new CIvvCrewDb(this.ApplicantID).Email);
                cc.Add(new CIvvCrewDb(this.RespondentID).Email);
            }
            else
            {
                //to.Add(FcdsHelper.System_Administrator_Email);
                to.Add(FcdsHelper.Process_Owner_Email);
            }

            subject += @" [FCDS] Your duty swap request form has been voided by reason of expiry";
            string body = @"<p>Greetings,<br />The duty swap request application of Form No. " + this.IDFcdsApply + @" has been voided automatically by system on " + this.UpdateStamp.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) + @".</p>";
            
            mu = new MailUtility() { Subject = subject, Body = body + (Config.IsProduction ? string.Empty : @"<p>Note: This email is for applicant and respondent.</p>"), To = to, Cc = cc, Bcc = bcc, IsBodyHtml = true };
            try
            {
                mu.SendMail();
                result = true;
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                SaveMailLog(@"SendVoidMail to Applicant and Respondent", null, subject, to, cc, bcc, body, result, ex);
            }

            return result;
        }

        /// <summary>
        /// 儲存發送電子郵件日誌。
        /// </summary>
        /// <param name="methodName">方法名稱</param>
        /// <param name="currentProID">目前流程編號</param>
        /// <param name="subject">電子郵件主旨</param>
        /// <param name="to">電子郵件收件者集合物件</param>
        /// <param name="cc">電子郵件副本抄送集合物件</param>
        /// <param name="bcc">電子郵件密件副本抄送集合物件</param>
        /// <param name="body">電子郵件內文</param>
        /// <param name="result">發送電子郵件結果</param>
        /// <param name="ex">例外物件</param>
        private void SaveMailLog(string methodName, string currentProID, string subject, MailAddressCollection to, MailAddressCollection cc, MailAddressCollection bcc, string body, bool result, Exception ex)
        {
            string logDetail = @"IDFcdsApply=" + this.IDFcdsApply + @", CurrentProID=" + currentProID + @", StatusCode=" + this.StatusCode + @", DisplayStatus=" + this.DisplayStatus + @", Subject=" + subject + 
                @", Body=" + body + @", To=" + JsonConvert.SerializeObject(to) + @", Cc=" + JsonConvert.SerializeObject(cc) + @", Bcc=" + JsonConvert.SerializeObject(bcc);
            
            if (ex != null)
            {
                logDetail += @", Exception=" + ex.ToString();
            }

            Log log = Log.GetLogWithLoginSuccessfully(methodName, logDetail, result);            
            log.Save(PageMode.PageModeEnum.Create);

            logger.Info(@"FcdsApply." + methodName + @"(), " + logDetail + @", result=" + result.ToString());
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