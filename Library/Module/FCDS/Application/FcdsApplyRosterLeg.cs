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
    public class FcdsApplyRosterLeg
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FcdsApply));

        #region Property
        public int Leg_Day { get; set; }

        public string Leg_Dep { get; set; }

        public int Leg_Carrier { get; set; }

        public int Leg_Flt { get; set; }

        public string Leg_LegCd { get; set; }

        public int R_ReportTime { get; set; }

        public int R_DebriefTime { get; set; }

        public int R_GrdDutyBeg { get; set; }

        public int R_GrdDutyEnd { get; set; }

        public int? Positioning_Code_DutyTime { get; set; }

        public string Cop_Duty_Cd { get; set; }

        public int? CL_RepON { get; set; }

        public int? CL_RepOff { get; set; }

        public int? CL_Pax { get; set; }

        public int? CL_ExtraPosCode { get; set; }

        public int? CL_Order { get; set; }

        public string LM_Dep { get; set; }

        public string LM_Arr { get; set; }

        public DateTime? LM_Sch_Str_Dt_Tm_Utc { get; set; }

        public DateTime? LM_Sch_End_Dt_Tm_Utc { get; set; }

        public string VCC_Cc { get; set; }

        public int? VCC_Report_Time { get; set; }

        public int? VCC_Debrief_Time { get; set; }

        public bool Is_Swappable { get; set; }

        public string Flight_Num { get; set; }

        public DateTime Str_Dt_Utc { get; set; }

        public DateTime End_Dt_Utc { get; set; }

        public DateTime Str_Dt_Tpe { get; set; }

        public DateTime End_Dt_Tpe { get; set; }

        public int? Prev_Min_Diff { get; set; }

        public int? Next_Min_Diff { get; set; }

        public string Carrier_Code { get; set; }

        public int? Reporting_Time { get; set; }

        public int? Debriefing_Time { get; set; }

        public DateTime? Duty_Str_Dt_Utc { get; set; }

        public DateTime? Duty_End_Dt_Utc { get; set; }

        public DateTime? Duty_Str_Dt_Tpe { get; set; }

        public DateTime? Duty_End_Dt_Tpe { get; set; }

        /// <summary>
        /// Crew Route 第一個勤務之臺北起始時間。
        /// </summary>
        public DateTime Cr_First_Duty_Str_Dt_Tpe { get; set; }

        /// <summary>
        /// Crew Route 最後一個勤務之臺北結束時間。
        /// </summary>
        public DateTime Cr_Last_Duty_End_Dt_Tpe { get; set; }

        /// <summary>
        /// Crew Route 是否跨月，以[Cr_First_Duty_Str_Dt_Tpe]與[Cr_Last_Duty_End_Dt_Tpe]屬性判斷若為不同月份則為是，若為相同月份則是否。
        /// </summary>
        public bool Is_AcrossMonth { get; set; }
        #endregion

        public void SetFcdsApplyRosterLeg(DataRow dr)
        {
            this.Leg_Day = int.Parse(dr["Leg_Day"].ToString());
            this.Leg_Dep = dr["Leg_Dep"].ToString();
            this.Leg_Carrier = int.Parse(dr["Leg_Carrier"].ToString());
            this.Leg_Flt = int.Parse(dr["Leg_Flt"].ToString());
            this.Leg_LegCd = dr["Leg_LegCd"].ToString();
            this.R_ReportTime = int.Parse(dr["R_ReportTime"].ToString());
            this.R_DebriefTime = int.Parse(dr["R_DebriefTime"].ToString());
            this.R_GrdDutyBeg = int.Parse(dr["R_GrdDutyBeg"].ToString());
            this.R_GrdDutyEnd = int.Parse(dr["R_GrdDutyEnd"].ToString());
            this.Positioning_Code_DutyTime = dr["Positioning_Code_DutyTime"] == DBNull.Value ? null as int? : int.Parse(dr["Positioning_Code_DutyTime"].ToString());
            this.Cop_Duty_Cd = dr["Cop_Duty_Cd"].ToString();
            this.CL_RepON = dr["CL_RepON"] == DBNull.Value ? null as int? : int.Parse(dr["CL_RepON"].ToString());
            this.CL_RepOff = dr["CL_RepOff"] == DBNull.Value ? null as int? : int.Parse(dr["CL_RepOff"].ToString());
            this.CL_Pax = dr["CL_Pax"] == DBNull.Value ? null as int? : int.Parse(dr["CL_Pax"].ToString());
            this.CL_ExtraPosCode = dr["CL_ExtraPosCode"] == DBNull.Value ? null as int? : int.Parse(dr["CL_ExtraPosCode"].ToString());
            this.CL_Order = dr["CL_Order"] == DBNull.Value ? null as int? : int.Parse(dr["CL_Order"].ToString());
            this.LM_Dep = dr["LM_Dep"].ToString();
            this.LM_Arr = dr["LM_Arr"].ToString();
            this.LM_Sch_Str_Dt_Tm_Utc = dr["LM_Sch_Str_Dt_Tm_Utc"] == DBNull.Value ? null as DateTime? : DateTime.Parse(dr["LM_Sch_Str_Dt_Tm_Utc"].ToString());
            this.LM_Sch_End_Dt_Tm_Utc = dr["LM_Sch_End_Dt_Tm_Utc"] == DBNull.Value ? null as DateTime? : DateTime.Parse(dr["LM_Sch_End_Dt_Tm_Utc"].ToString());
            this.VCC_Cc = dr["VCC_Cc"].ToString();
            this.VCC_Report_Time = dr["VCC_Report_Time"] == DBNull.Value ? null as int? : int.Parse(dr["VCC_Report_Time"].ToString());
            this.VCC_Debrief_Time = dr["VCC_Debrief_Time"] == DBNull.Value ? null as int? : int.Parse(dr["VCC_Debrief_Time"].ToString());

            // 擷取組員班表 SQL 指令之[Is_Swappable]欄位不為 NULL 值且等於 1，則 Is_Swappable 屬性為 true，否則為 false
            this.Is_Swappable = (dr["Is_Swappable"] != DBNull.Value && dr["Is_Swappable"].ToString().Equals("1")) ? true : false;
            
            this.Flight_Num = dr["Flight_Num"].ToString();
            this.Str_Dt_Utc = DateTime.Parse(dr["Str_Dt_Utc"].ToString());
            this.End_Dt_Utc = DateTime.Parse(dr["End_Dt_Utc"].ToString());
            this.Str_Dt_Tpe = DateTime.Parse(dr["Str_Dt_Tpe"].ToString());
            this.End_Dt_Tpe = DateTime.Parse(dr["End_Dt_Tpe"].ToString());
            this.Prev_Min_Diff = dr["Prev_Min_Diff"] == DBNull.Value ? null as int? : int.Parse(dr["Prev_Min_Diff"].ToString());
            this.Next_Min_Diff = dr["Next_Min_Diff"] == DBNull.Value ? null as int? : int.Parse(dr["Next_Min_Diff"].ToString());
            this.Carrier_Code = dr["Carrier_Code"].ToString();
            this.Reporting_Time = dr["Reporting_Time"] == DBNull.Value ? null as int? : int.Parse(dr["Reporting_Time"].ToString());
            this.Debriefing_Time = dr["Debriefing_Time"] == DBNull.Value ? null as int? : int.Parse(dr["Debriefing_Time"].ToString());
            this.Duty_Str_Dt_Utc = dr["Duty_Str_Dt_Utc"] == DBNull.Value ? null as DateTime? : DateTime.Parse(dr["Duty_Str_Dt_Utc"].ToString());
            this.Duty_End_Dt_Utc = dr["Duty_End_Dt_Utc"] == DBNull.Value ? null as DateTime? : DateTime.Parse(dr["Duty_End_Dt_Utc"].ToString());
            this.Duty_Str_Dt_Tpe = dr["Duty_Str_Dt_Tpe"] == DBNull.Value ? null as DateTime? : DateTime.Parse(dr["Duty_Str_Dt_Tpe"].ToString());
            this.Duty_End_Dt_Tpe = dr["Duty_End_Dt_Tpe"] == DBNull.Value ? null as DateTime? : DateTime.Parse(dr["Duty_End_Dt_Tpe"].ToString());
            this.Cr_First_Duty_Str_Dt_Tpe = DateTime.Parse(dr["Cr_First_Duty_Str_Dt_Tpe"].ToString());
            this.Cr_Last_Duty_End_Dt_Tpe = DateTime.Parse(dr["Cr_Last_Duty_End_Dt_Tpe"].ToString());
            this.Is_AcrossMonth = this.Cr_First_Duty_Str_Dt_Tpe.Month != this.Cr_Last_Duty_End_Dt_Tpe.Month;
        }
    }

    public class FcdsApplyRosterLegCollection : List<FcdsApplyRosterLeg>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public FcdsApplyRosterLegCollection()
        {

        }
    }
}
