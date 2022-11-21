using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;

using log4net;

using Library.Component.BLL;
using Library.Component.DAL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS.Application;
using Library.Module.FZDB;
using Library.Module.HRDB;

namespace Library.Module.FCDS
{
    /// <summary>
    /// 飛航組員任務換班申請系統輔助密封類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class FcdsHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FcdsHelper));
        private static readonly ModuleForm mf = new ModuleForm(@"Application", @"FcdsApply");

        /// <summary>
        /// 系統管理員電子郵件地址。
        /// </summary>
        public static readonly string System_Administrator_Email = mf.GetSystemAdministratorMail();// @"647868@china-airlines.com";

        /// <summary>
        /// 系統所有者(業管單位)電子郵件地址。
        /// </summary>
        public static readonly string Process_Owner_Email = mf.GetProcessOwnerMail();

        /// <summary>
        /// 飛航組員任務換班申請系統新建立申請單流程編號。
        /// </summary>
        public const string PRO_INITIAL = @"PRO1015313275";

        /// <summary>
        /// 飛航組員任務換班申請系統申請單受申請人審核中流程編號。
        /// </summary>
        public const string PRO_RESPONDENT = @"PRO1015313276";

        /// <summary>
        /// 飛航組員任務換班申請系統申請單申請人收回流程編號。
        /// </summary>
        public const string PRO_REVOKE = @"PRO1015313277";

        /// <summary>
        /// 飛航組員任務換班申請系統申請單組員派遣部承辦人審核中流程編號。
        /// </summary>
        public const string PRO_OP_STAFF = @"PRO1015313278";

        /// <summary>
        /// 飛航組員任務換班申請系統申請單組員派遣部組長審核中流程編號。
        /// </summary>
        public const string PRO_OP_MANAGER = @"PRO1015313279";

        /// <summary>
        /// 飛航組員任務換班申請系統申請單組員派遣部副理審核中流程編號。
        /// </summary>
        public const string PRO_OP_ASSISTANT_GENERAL_MANAGER = @"PRO1015313280";

        /// <summary>
        /// 飛航組員任務換班申請系統申請單組員派遣部經理審核中流程編號。
        /// </summary>
        public const string PRO_OP_GENERAL_MANAGER = @"PRO1015313281";

        /// <summary>
        /// 系統名稱。
        /// </summary>
        public const string ROOT_NAME_SPACE = @"FCDS";

        /// <summary>
        /// 飛航組員任務互換申請單狀態代碼列舉型態。
        /// </summary>
        public enum FcdsApplyStatusCodeEnum
        {
            /// <summary>
            /// 新建立申請單狀態代碼。
            /// </summary>
            INITIAL = 1,

            /// <summary>
            /// 受申請人審核中狀態代碼。
            /// </summary>
            RESPONDENT = 10,

            /// <summary>
            /// 申請人收回狀態代碼。
            /// </summary>
            REVOKE = 11,

            /// <summary>
            /// 受申請人不同意狀態代碼。
            /// </summary>
            RESPONDENT_RETURN = 12,

            /// <summary>
            /// 航務處組員派遣部承辦人審核中狀態代碼。
            /// </summary>
            OP_STAFF = 20,

            /// <summary>
            /// 航務處組員派遣部組長審核中狀態代碼。
            /// </summary>
            OP_MANAGER = 30,

            /// <summary>
            /// 航務處組員派遣部副理審核中狀態代碼。
            /// </summary>
            OP_ASSISTANT_GENERAL_MANAGER = 40,

            /// <summary>
            /// 航務處組員派遣部經理審核中狀態代碼。
            /// </summary>
            OP_GENERAL_MANAGER = 50,

            /// <summary>
            /// 申請單結案狀態代碼。
            /// </summary>
            RELEASED = 99,

            /// <summary>
            /// 申請單逾期作廢狀態代碼。
            /// </summary>
            VOID = -1
        }

        private FcdsHelper()
        {

        }

        public static FcdsApplyRosterCollection ProcessRosterData(DataTable dtApplicant, DataTable dtRespondent, Dictionary<double, DateTime> dicRosterDates, CIvvCrewDb applicantCrewDb, CIvvCrewDb respondentCrewDb)
        {
            FcdsApplyRosterCollection fcdsApplyRosters = new FcdsApplyRosterCollection();
            foreach (KeyValuePair<double, DateTime> item in dicRosterDates)
            {
                FcdsApplyRoster ar = new FcdsApplyRoster() { RosterDate = item.Value, ApplicantDutyDay = int.Parse(item.Key.ToString()), RespondentDutyDay = int.Parse(item.Key.ToString()) };
                fcdsApplyRosters.Add(ar);
            }

            #region Process Applicant Roster Data
            for (int i = 0; i < dtApplicant.Rows.Count; i++)
            {
                DataRow prevDataRow = i > 0 ? dtApplicant.Rows[i - 1] : null;
                DataRow currDataRow = dtApplicant.Rows[i];

                DateTime dutyStrDtTpe = currDataRow["Duty_Str_Dt_Tpe"] == DBNull.Value ? DateTime.Parse(currDataRow["Str_Dt_Tpe"].ToString()).Date : DateTime.Parse(currDataRow["Duty_Str_Dt_Tpe"].ToString()).Date;
                if (prevDataRow != null && dutyStrDtTpe > applicantCrewDb.CIvvRosterId.PublishDate && (currDataRow["DutyDay"].ToString() != prevDataRow["DutyDay"].ToString() || currDataRow["DutyNo"].ToString() != prevDataRow["DutyNo"].ToString()))
                {
                    // 若勤務起始日期大於申請者發佈班表最後日期，且當前資料列[DutyDay]與[DutyNo]欄位與上一資料列不相同時，則跳出迴圈
                    break;
                }

                FcdsApplyRoster fcdsApplyRoster = fcdsApplyRosters.Find(o => o.RosterDate == dutyStrDtTpe);
                if (fcdsApplyRoster != null)
                {
                    fcdsApplyRoster.ApplicantID = applicantCrewDb.ID;
                    fcdsApplyRoster.ApplicantDutyDay = int.Parse(currDataRow["DutyDay"].ToString());
                    fcdsApplyRoster.ApplicantDutyNo = long.Parse(currDataRow["DutyNo"].ToString());
                    fcdsApplyRoster.ApplicantNumOfLeg = int.Parse(currDataRow["Num_Of_Leg"].ToString());

                    // 若勤務起始日期(報到日期時間)或航機表訂起飛時間(STD，因同一報到報離第二段無報到時間，以航機表訂起飛時間)大於當下時間，
                    // 且[Is_Swappable]為 NULL 值(表示為飛航勤務)或[Is_Swappable]為 1 時，則此筆資料可啟用勾選
                    //fcdsApplyRoster.IsApplicantEnable =
                    //    (dutyStrDtTpe >= this.SwappableBeginDate) &&
                    //    (currDataRow["Is_Swappable"] == DBNull.Value || (currDataRow["Is_Swappable"] != DBNull.Value && currDataRow["Is_Swappable"].ToString().Equals("1")))
                    //     ? true : false;

                    if (fcdsApplyRoster.ApplicantRosterLegs == null) fcdsApplyRoster.ApplicantRosterLegs = new FcdsApplyRosterLegCollection();
                    FcdsApplyRosterLeg fcdsApplyRosterLeg = new FcdsApplyRosterLeg();
                    fcdsApplyRosterLeg.SetFcdsApplyRosterLeg(currDataRow);
                    fcdsApplyRoster.ApplicantRosterLegs.Add(fcdsApplyRosterLeg);
                }
            }
            #endregion

            #region Process Respondent Roster Data
            for (int i = 0; i < dtRespondent.Rows.Count; i++)
            {
                DataRow prevDataRow = i > 0 ? dtRespondent.Rows[i - 1] : null;
                DataRow currDataRow = dtRespondent.Rows[i];

                DateTime dutyStrDtTpe = currDataRow["Duty_Str_Dt_Tpe"] == DBNull.Value ? DateTime.Parse(currDataRow["Str_Dt_Tpe"].ToString()).Date : DateTime.Parse(currDataRow["Duty_Str_Dt_Tpe"].ToString()).Date;
                if (prevDataRow != null && dutyStrDtTpe > respondentCrewDb.CIvvRosterId.PublishDate && (currDataRow["DutyDay"].ToString() != prevDataRow["DutyDay"].ToString() || currDataRow["DutyNo"].ToString() != prevDataRow["DutyNo"].ToString()))
                {
                    // 若勤務起始日期大於受申請者發佈班表最後日期，且當前資料列[DutyDay]與[DutyNo]欄位與上一資料列不相同時，則跳出迴圈
                    break;
                }

                FcdsApplyRoster fcdsApplyRoster = fcdsApplyRosters.Find(o => o.RosterDate == dutyStrDtTpe);
                if (fcdsApplyRoster != null)
                {
                    fcdsApplyRoster.RespondentID = respondentCrewDb.ID;
                    fcdsApplyRoster.RespondentDutyDay = int.Parse(currDataRow["DutyDay"].ToString());
                    fcdsApplyRoster.RespondentDutyNo = long.Parse(currDataRow["DutyNo"].ToString());
                    fcdsApplyRoster.RespondentNumOfLeg = int.Parse(currDataRow["Num_Of_Leg"].ToString());

                    // 若勤務起始日期(報到日期時間)或航機表訂起飛時間(STD，因同一報到報離第二段無報到時間，以航機表訂起飛時間)大於當下時間，
                    // 且[Is_Swappable]為 NULL 值(表示為飛航勤務)或[Is_Swappable]為 1 時，則此筆資料可啟用勾選
                    //fcdsApplyRoster.IsRespondentEnable = 
                    //    (dutyStrDtTpe >= this.SwappableBeginDate) &&
                    //    (currDataRow["Is_Swappable"] == DBNull.Value || (currDataRow["Is_Swappable"] != DBNull.Value && currDataRow["Is_Swappable"].ToString().Equals("1")))
                    //     ? true : false;

                    if (fcdsApplyRoster.RespondentRosterLegs == null) fcdsApplyRoster.RespondentRosterLegs = new FcdsApplyRosterLegCollection();
                    FcdsApplyRosterLeg fcdsApplyRosterLeg = new FcdsApplyRosterLeg();
                    fcdsApplyRosterLeg.SetFcdsApplyRosterLeg(currDataRow);
                    fcdsApplyRoster.RespondentRosterLegs.Add(fcdsApplyRosterLeg);
                }
            }
            #endregion

            return fcdsApplyRosters;
        }

        /// <summary>
        /// 擷取飛航組員機隊部門資料。
        /// </summary>
        /// <param name="uperUt">上級單位編碼</param>
        /// <param name="isEffective">是否擷取當下有效部門</param>
        /// <returns></returns>
        public static DataView FetchFleetDep(string uperUt, bool isEffective)
        {
            DataTable dt = HrVPbUnitCd.FetchByUperUt(uperUt, isEffective);
            /*
            var rows = dt.AsEnumerable()
                         .Where(dr => dr.Field<string>("CDesc").EndsWith("機隊"))
                         .Select(dr => new
                         {
                             DepValue = dr.Field<string>("DepValue"),
                             CDesc = dr.Field<string>("CDesc")
                         });
            rows.ToList();
            */
            var results = from dr in dt.AsEnumerable()
                          where dr.Field<string>("CDesc").EndsWith("機隊")
                          select dr;
            DataView dv = results.AsDataView();

            return dv;
        }

        public static DataSet FetchGroupDep()
        {
            string sql = @"SELECT   '001'                   AS UnitCd
                                    ,'中華航空'             AS CDesc
                                    ,NULL                   AS UperUt
                                    ,'中華航空'             AS CDescIndent
                                    ,'/'                    AS CDescPath
                                    ,0                      AS ""Level""
                            FROM    DUAL
                            UNION ALL
                            SELECT   t.UnitCd
                                    ,t.CDesc
                                    ,CASE
                                        WHEN t.unitcd = t.uperut THEN '001'
                                        ELSE t.uperut
                                    END                                             AS UperUt
                                    ----內縮排版可用 LPAD() 實現
                                    ,LPAD(' ', (Level - 1) * 4) || t.CDesc          AS CDescIndent
                                    ----SYS_CONNECT_BY_PATH() 可快速串接各層欄位字串
                                    ,SYS_CONNECT_BY_PATH(t.cdesc,'/')               AS CDescPath
                                    ,Level
                           FROM     hrdb.hrvpbunitcd t
                           WHERE    t.leadut IN ('050', '170D') AND t.nation = '中華民國' AND t.effdt <= SYSDATE AND t.exprdt >= SYSDATE
                           --以ParentPartNo='ROOT'這筆做為起始點開始長樹
                           START WITH t.unitcd IN ('050', '170D')
                           --欄位名前方的一元運算符PRIOR用於指定父資料欄位
                           --故此處為將PartNo等於本筆ParentPartNo的資料列視為父資料列
                           CONNECT BY NOCYCLE PRIOR t.unitcd = t.uperut";

            DataSet ds = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql);

            //將 DataRelation 加進 DataSet 中
            DataRelation drOrg = new DataRelation("OrgRelation", ds.Tables[0].Columns["UnitCd"], ds.Tables[0].Columns["UperUt"]);
            ds.Relations.Add(drOrg);

            return ds;
        }
    }
}
