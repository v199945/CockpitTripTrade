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
using Library.Component.Utility;

namespace Library.Module.FZDB
{
    public class CIRoster
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CIRoster));

        #region Property
        #endregion

        private static string BuildFetchCommandString()
        {
            return @"";
        }

        /// <summary>
        /// 建構 SQL 擷取飛航組員班表指令字串。
        /// </summary>
        /// <remarks>
        /// C# DateTime 預設會對應成 OracleDbType.Timestamp，當資料庫端欄位為 Date 型別，會因小數秒差造成 WHERE 比對不符，另外會因無法套用索引造成查詢效率不彰。
        /// 在查詢語法中使月 CAST(:dateParam AS DATE) 轉型可以克服上述問題。
        /// 若資料庫以 Date 為主未用到 Timestamp，透過 Dapper SqlMapper.AddTypeMap(typeof(DateTime), DbType.Date) 將 C# DateTime 改對應至 DbType.Date，可更巧妙避開問題
        /// 20221111 648267:調整Ground Duty顯示Duty Code而非Duty Description=>line:141 將VCC.CCDESC換成VCC.CC
        /// </remarks>
        /// <returns></returns>
        private static string BuildFetchTickRosterCommandString()
        {
            return @"
                    SELECT  TR.*
                            ,DECODE(TR.R_REPORTTIME , 0, TR.Str_Dt_Utc + TR.Reporting_Time  / 60 / 24, TO_DATE('1980/01/01', 'YYYY/MM/DD') + TR.LEG_DAY + TR.R_REPORTTIME  / 60 / 24)          AS duty_str_dt_utc
                            ,DECODE(TR.R_DEBRIEFTIME, 0, TR.End_Dt_Utc + TR.Debriefing_Time / 60 / 24, TO_DATE('1980/01/01', 'YYYY/MM/DD') + TR.LEG_DAY + TR.R_DEBRIEFTIME / 60 / 24)          AS duty_end_dt_utc
                            ,DECODE(TR.R_REPORTTIME , 0, TR.Str_Dt_Tpe + TR.Reporting_Time  / 60 / 24, TO_DATE('1980/01/01', 'YYYY/MM/DD') + TR.LEG_DAY + TR.R_REPORTTIME  / 60 / 24 + 8 / 24) AS duty_str_dt_tpe
                            ,DECODE(TR.R_DEBRIEFTIME, 0, TR.End_Dt_Tpe + TR.Debriefing_Time / 60 / 24, TO_DATE('1980/01/01', 'YYYY/MM/DD') + TR.LEG_DAY + TR.R_DEBRIEFTIME / 60 / 24 + 8 / 24) AS duty_end_dt_tpe
                            ,FIRST_VALUE(DECODE(TR.R_REPORTTIME , 0, TR.Str_Dt_Tpe + TR.Reporting_Time  / 60 / 24, TO_DATE('1980/01/01', 'YYYY/MM/DD') + TR.LEG_DAY + TR.R_REPORTTIME  / 60 / 24 + 8 / 24)) OVER (PARTITION BY TR.DUTYDAY, TR.DUTYNO ORDER BY TR.CL_ORDER RANGE BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS cr_first_duty_str_dt_tpe
                            ,LAST_VALUE(DECODE(TR.R_DEBRIEFTIME, 0, TR.End_Dt_Tpe + TR.Debriefing_Time / 60 / 24, TO_DATE('1980/01/01', 'YYYY/MM/DD') + TR.LEG_DAY + TR.R_DEBRIEFTIME / 60 / 24 + 8 / 24)) OVER (PARTITION BY TR.DUTYDAY, TR.DUTYNO ORDER BY TR.CL_ORDER RANGE BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING)  AS cr_last_duty_end_dt_tpe
                    FROM    (
                             SELECT  CR.*
                                     ,TRIM(VC.CODE)                                                                                                                                                             AS Carrier_Code
                                     ,VC.CAT                                                                                                                                                                    AS Carrier_Category --0 is Air, 1 is Ground
                                     ,CASE
                                         WHEN CR.DUTYNO < 0 THEN -CR.VCC_REPORT_TIME
                                         ELSE CASE
                                                 WHEN CR.prev_min_diff IS NULL OR CR.prev_min_diff > 4 * 60
                                                 THEN
                                                     CASE
                                                       --WHEN CR.R_REPORTTIME <> 0 THEN CR.R_REPORTTIME
                                                       WHEN CR.CL_REPON     <> 0 THEN CR.CL_REPON
                                                       ELSE
                                                         DECODE(CR.Positioning_Code_Dutytime, 0, 0
                                                                                            , CASE
                                                                                                WHEN CR.CL_Pax <> 0 OR CR.CL_Extraposcode <> 0
                                                                                                  THEN CASE
                                                                                                         WHEN TRIM(CR.LEG_DEP) IN ('TPE', 'TSA') THEN -VCPRDT.Reporting_Time
                                                                                                         ELSE -VCOPRDT.Reporting_Time
                                                                                                         END
                                                                                                  ELSE CASE
                                                                                                         WHEN TRIM(CR.LEG_DEP) IN ('TPE', 'TSA') THEN -VCRDT.Reporting_Time
                                                                                                         ELSE -VCORDT.Reporting_Time
                                                                                                       END
                                                                                              END
                                                               )
                                                     END
                                                 ELSE NULL
                                              END
                                     END                                                                                                                                                                        AS Reporting_Time
                                     ,CASE
                                        WHEN CR.DUTYNO < 0 THEN CR.VCC_Debrief_Time
                                        ELSE CASE
                                               WHEN CR.next_min_diff IS NULL OR CR.next_min_diff > 4 * 60
                                               THEN CASE
                                                      --WHEN CR.R_DEBRIEFTIME <> 0 THEN CR.R_DEBRIEFTIME
                                                      WHEN CR.CL_REPOFF     <> 0 THEN CR.CL_REPOFF
                                                      ELSE
                                                        DECODE(CR.Positioning_Code_Dutytime, 0, 0
                                                                                           , CASE
                                                                                               WHEN CR.CL_Pax <> 0 OR CR.CL_Extraposcode <> 0
                                                                                               THEN CASE
                                                                                                      WHEN TRIM(CR.LEG_DEP) IN ('TPE', 'TSA') THEN VCPRDT.Debriefing_Time
                                                                                                      ELSE VCOPRDT.Debriefing_Time
                                                                                                    END
                                                                                               ELSE CASE
                                                                                                      WHEN TRIM(CR.LEG_DEP) IN ('TPE', 'TSA') THEN VCRDT.Debriefing_Time
                                                                                                      ELSE VCORDT.Debriefing_Time
                                                                                                    END
                                                                                             END
                                                              )
                                                    END
                                               ELSE NULL
                                             END
                                     END                                                                                                                                                                        AS Debriefing_Time
                                     --每日與[申請日期]比較，小於申請日期則無法勾選
 
                                     /*
                                     ,VCRDT.*            --Reporting and Debriefing Time of Cockpit Crew Home Base As Operation
                                     ,VCPRDT.*           --Reporting and Debriefing Time of Cockpit Crew Home Base As Passenger(Pax)
                                     ,VCORDT.*           --Reporting and Debriefing Time of Cockpit Crew Out of Base As Operation
                                     ,VCOPRDT.*          --Reporting and Debriefing Time of Cockpit Crew Out of Base As Passenger(Pax)
                                     ,VRA.*
                                     */
                             FROM   (
                                     SELECT CIRoster.*
                                            ,ROUND(TO_NUMBER(CIRoster.str_dt_utc - (LAG(CIRoster.end_dt_utc) OVER (PARTITION BY CIRoster.DUTYDAY, CIRoster.DUTYNO ORDER BY CIRoster.CL_ORDER))) * 24 * 60)  AS prev_min_diff        
                                            ,ROUND(TO_NUMBER((LEAD(CIRoster.str_dt_utc) OVER (PARTITION BY CIRoster.DUTYDAY, CIRoster.DUTYNO ORDER BY CIRoster.CL_ORDER)) - CIRoster.end_dt_utc) * 24 * 60) AS next_min_diff
                                     FROM   (
                                             SELECT  --DR.EveryDay
                                                     --,TO_DATE('1980/01/01', 'YYYY/MM/DD') + DR.EveryDay                                                                                                                                              AS EveryDay_Utc
                                                     VR.PUBLISH
                                                     ,R.ID, R.POS, R.DUTYDAY, R.DUTYNO, R.LEG_DAY, R.LEG_DEP, R.LEG_CARRIER, R.LEG_FLT, R.LEG_LEGCD
                                                     ,R.REPORTTIME AS R_REPORTTIME, R.DEBRIEFTIME AS R_DEBRIEFTIME, R.GRDDUTYBEG AS R_GRDDUTYBEG, R.GRDDUTYEND AS R_GRDDUTYEND

                                                     ,DECODE(VFI2.Posing_Code, 'ACM', 100, VEPC.Positioning_Code_Dutytime)                                                                                                                           AS Positioning_Code_Dutytime
                                                     ,COALESCE(VEPC.POSITIONING_CODE, VFI2.POSING_CODE)                                                                                                                                              AS cop_duty_cd

                                                     -- 擷取[REPON]、[REPOFF]欄位，以便於判斷若有 ETD Delay 變更報到時間時，是否仍以 [CrewLeg].[REPON]、[REPOFF]為報到報離時間
                                                     ,CL.CDATE, CL.CROUTE, CL.REPON AS CL_REPON, CL.REPOFF AS CL_REPOFF, CL.PAX AS CL_PAX, CL.EXTRAPOSCODE AS CL_EXTRAPOSCODE, CL.ORDER_ AS CL_Order
                                                     ,COUNT('x') OVER (PARTITION BY R.DUTYDAY, R.DUTYNO) AS Num_Of_Leg--計算每個[Roster].[DutyDay]與[Roster].[DutyNo]分群下資料筆數
                                                     ,CR.CR_DESC
                                                     ,CLG.AC AS CLG_AC
                                                     ,TRIM(LM.DEP) AS LM_DEP, TRIM(LM.ARR) AS LM_ARR, LM.AC AS LM_AC, LM.LM_SCH_STR_DT_TM_UTC, LM.LM_SCH_END_DT_TM_UTC

                                                     ,VCC.CC                                                                                                                                                                                         AS VCC_CC
                                                     ,VCC.CCDESC                                                                                                                                                                                     AS VCC_CCDESC
                                                     ,VCC.REPORT_TIME                                                                                                                                                                                AS VCC_REPORT_TIME--Grnd_Reporting_Time
                                                     ,VCC.DEBRIEF_TIME                                                                                                                                                                               AS VCC_DEBRIEF_TIME--Grnd_Debriefing_Time
                                                     ,COALESCE(VCC.CSC_CATEGORY_37, 1)                                                                                                                                                               AS Is_Swappable -- VCC.CSC_CATEGORY_37 為 NULL 值即為飛航任務可換班

                                                     ,DECODE(SIGN(R.DUTYNO), 1, CONCAT(LPAD(R.LEG_FLT, 4, '0'), TRIM(R.LEG_LEGCD)), -1, TRIM(VCC.CC))                                                                                            AS Flight_Num
                                                     --,CONCAT(LPAD(R.LEG_FLT, 4, '0'), TRIM(R.LEG_LEGCD))                                                                                                                                           AS flight_num
                                                     ,DECODE(SIGN(R.DUTYNO), 1, LM.LM_SCH_STR_DT_TM_UTC, -1, COALESCE(CLG.CLG_STR_DT_TM_UTC, TO_DATE('1980/01/01', 'YYYY/MM/DD') + R.DUTYDAY + R.GRDDUTYBEG / 60 / 24))                              AS str_dt_utc
                                                     ,DECODE(SIGN(R.DUTYNO), 1, LM.LM_SCH_END_DT_TM_UTC, -1, COALESCE(CLG.CLG_END_DT_TM_UTC, TO_DATE('1980/01/01', 'YYYY/MM/DD') + R.DUTYDAY + R.GRDDUTYEND / 60 / 24))                              AS end_dt_utc
                                                     ,DECODE(SIGN(R.DUTYNO), 1, LM.LM_SCH_STR_DT_TM, -1, COALESCE(CLG.CLG_STR_DT_TM, TO_DATE('1980/01/01', 'YYYY/MM/DD') + R.DUTYDAY + R.GRDDUTYBEG / 60 / 24 + DECODE(R.GRDDUTYBEG, 0, 0, 8 / 24))) AS str_dt_tpe
                                                     ,DECODE(SIGN(R.DUTYNO), 1, LM.LM_SCH_END_DT_TM, -1, COALESCE(CLG.CLG_END_DT_TM, TO_DATE('1980/01/01', 'YYYY/MM/DD') + R.DUTYDAY + R.GRDDUTYEND / 60 / 24 + DECODE(R.GRDDUTYEND, 0, 0, 8 / 24))) AS end_dt_tpe
                                             FROM    fzdb.ci_vvrosterid                       VR
                                                     INNER JOIN fzdb.ci_roster                R         ON VR.ID              = R.ID
                                                                                                           --AND DR.EveryDay    = R.DUTYDAY
                                                     LEFT JOIN fzdb.ci_crewleg                CL        ON R.LEG_DAY          = CL.DAY
                                                                                                           AND R.LEG_DEP      = CL.DEP
                                                                                                           AND R.LEG_CARRIER  = CL.CARRIER
                                                                                                           AND R.LEG_FLT      = CL.FLT
                                                                                                           AND R.LEG_LEGCD    = CL.LEGCD
                                                                                                           AND R.DUTYDAY      = CL.CDATE
                                                                                                           AND R.DUTYNO       = CL.CROUTE
                                                     LEFT JOIN fzdb.mv_vvfmsinfo2             VFI2      ON CL.PAX             = VFI2.CODE
                                                     LEFT JOIN fzdb.mv_extraposcd             VEPC      ON CL.EXTRAPOSCODE    = VEPC.CODE
                                                                                                           AND CL.PAX = 1
                                                     LEFT JOIN fzdb.ci_crroute                CR        ON CL.CDATE           = CR.CDATE
                                                                                                           AND CL.CROUTE      = CR.CROUTE
                                                     LEFT JOIN fzdb.ci_crlegground            CLG       ON CL.DAY             = CLG.DAY
                                                                                                           AND CL.DEP         = CLG.DEP
                                                                                                           AND CL.CARRIER     = CLG.CARRIER
                                                                                                           AND CL.FLT         = CLG.FLT
                                                                                                           AND CL.LEGCD       = CLG.LEGCD
                                                                                                           AND CL.CDATE       = CLG.CDATE
                                                                                                           AND CL.CROUTE      = CLG.CROUTE
                                                     LEFT JOIN fzdb.ci_legmain                LM        ON CL.DAY             = LM.DAY
                                                                                                           AND CL.DEP         = LM.DEP
                                                                                                           AND CL.CARRIER     = LM.CARRIER
                                                                                                           AND CL.FLT         = LM.FLT
                                                                                                           AND CL.LEGCD       = LM.LEGCD
                                                     LEFT JOIN fzdb.ci_crcodes                VCC       ON R.DUTYNO           = VCC.ID
                                                     --Oracle Parameter 
                                             WHERE   VR.ID = :pCrewID AND R.DUTYDAY >= CAST(:pBeginDate AS DATE) - TO_DATE('1980/01/01', 'YYYY/MM/DD') AND R.DUTYDAY <= CAST(:pEndDate AS DATE) - TO_DATE('1980/01/01', 'YYYY/MM/DD')
                                                     AND R.DUTYNO NOT IN (-4, -5, -7)-- 排除[DutyNo]為 -4「OFF< Pre-flight rest/day off」、-5「>OFF Post-flight rest/day off」、-7「> Rest day」，OP 蕭博元 20210105
                                            ) CIRoster
                                    )                                        CR
                                    LEFT JOIN fzdb.ci_vvcarriers             VC        ON CR.LEG_CARRIER                             = VC.ID
                                    LEFT JOIN fzdb.ci_vvairctype             VACT      ON CR.LM_AC                                   = VACT.ACTYPE
                                    LEFT JOIN fzdb.ci_vvairctype             VACTCLG   ON CR.CLG_AC                                  = VACTCLG.ACTYPE
                                    --Calculate Reporting Time and Debriefing Time
                                    LEFT JOIN fzdb.mv_fdrptdrftimes          VCRDT     ON COALESCE(VACTCLG.Iatacode, VACT.IATACODE)  = VCRDT.FLEET
                                    LEFT JOIN fzdb.mv_fdpaxrptdrftimes       VCPRDT    ON VC.CAT                                     = VCPRDT.PAXCATEGORY
                                    LEFT JOIN fzdb.mv_fdoutrptdrftimes       VCORDT    ON COALESCE(VACTCLG.Iatacode, VACT.IATACODE)  = VCORDT.FLEET
                                    LEFT JOIN fzdb.mv_fdoutpaxrptdrftimes    VCOPRDT   ON VC.CAT                                     = VCOPRDT.PAXCATEGORY
                                    --CROSS JOIN fzdb.fdreporttimeadjust       VRA--若有 ETD Delay 變更報到時間，AIMS 系統會將變更後報到時間寫入[Roster].[ReportTime]欄位
                            ) TR
                    ORDER   BY TR.ID/**/, DECODE(SIGN(TR.DUTYNO), 1, TR.LEG_DAY, -1, TR.DUTYDAY), TR.CL_ORDER--, TR.Str_Dt_Utc
            ";

            /*
                                                     CROSS JOIN
                                                               (
                                                                --2020/11/25 16 11/10-30
                                                                --2020/11/26 16 10/26-11/30
                                                                -- [互換月份]選當月則當日回推 10 天至月底跨月班，帶入 當日-當月底-當日
                                                                -- 選次月則當月跨月班至次月跨月班，帶入 當月底-次月底-當月底
                                                                SELECT  (:pBeginDate - 16 + LEVEL) -TO_DATE('1980/01/01', 'YYYY/MM/DD')            AS EveryDay --Oracle Parameter
                                                               FROM    DUAL
                                                               CONNECT BY LEVEL <= FLOOR(LAST_DAY(TRUNC(:pEndDate, 'DD')) - TRUNC(:pBeginDate, 'DD') + 16)--Oracle Parameter
                                                               )                              DR--Date Range
            */
        }

        /// <summary>
        /// 擷取飛航組員班表。
        /// </summary>
        /// <param name="crewID">飛航組員員工編號</param>
        /// <param name="beginDate">起始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns></returns>
        public static DataTable FetchApplyRoster(string crewID, DateTime beginDate, DateTime endDate)
        {
            string sql = BuildFetchTickRosterCommandString();
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pCrewID", crewID), new OracleParameter("pBeginDate", beginDate), new OracleParameter("pEndDate", endDate)};
            /*
            ops.Add((OracleParameter) (new OracleParameter("pBeginDate", OracleDbType.Date).Value = beginDate));
            ops.Add((OracleParameter) (new OracleParameter("pEndDate", OracleDbType.Date).Value = endDate));
            ops.Add((OracleParameter) (new OracleParameter("pBeginDate", OracleDbType.Date).Value = beginDate));
            ops.Add((OracleParameter) (new OracleParameter("pCrewID", OracleDbType.Varchar2).Value = crewID));
            */
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            //DataTableUtility.CreateDataTableColumn(dt, "RosterDate", "System.String", false, null, "SUBSTRING(CONVERT(ISNULL(Duty_Str_Dt_Tpe, Str_Dt_Tpe), 'System.String'), 1, 10)");
            //DataTableUtility.CreateDataTableColumn(dt, "Duty_Str_Dt_Utc", "System.DateTime", true, null, "Str_Dt_Utc + Reporting_Time");
            //DataTableUtility.CreateDataTableColumn(dt, "Duty_End_Dt_Utc", "System.DateTime", true, null, "End_Dt_Utc + Debriefing_Time");

            return dt;
        }
    }
}
