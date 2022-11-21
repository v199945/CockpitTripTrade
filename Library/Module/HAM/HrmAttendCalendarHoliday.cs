using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;

using Library.Component.BLL;
using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Module.HAM
{
    /// <summary>
    /// HRM 行事曆法定假日類別，來源[db_HAM].[View_HRM_Attend_Calendar_Holiday]資料表。
    /// </summary>
    public class HrmAttendCalendarHoliday
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HrmAttendCalendarHoliday));

        #region Property
        /// <summary>
        /// 行事曆法定假日日期。
        /// </summary>
        public DateTime? CalnDt { get; set; }

        /// <summary>
        /// 行事曆法定假日類型代碼。
        /// </summary>
        public string CalnCd { get; set; }

        /// <summary>
        /// 行事曆法定假日類型代碼中文名稱。
        /// </summary>
        public string CalnCDesc { get; set; }

        /// <summary>
        /// 行事曆法定假日類型代碼英文名稱。
        /// </summary>
        public string CalnEDesc { get; set; }

        /// <summary>
        /// 行事曆法定假日代碼，如休息日(R)、例假日(Z)、民俗假日(FGBL)、國定假日(GBL)、國定假日補假(CGBL)。
        /// </summary>
        public string RtCd { get; set; }

        /// <summary>
        /// 行事曆法定假日代碼名稱，如休息日(R)、例假日(Z)、民俗假日(FGBL)、國定假日(GBL)、國定假日補假(CGBL)。
        /// </summary>
        public string RtCDesc { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public HrmAttendCalendarHoliday()
        {

        }

        public HrmAttendCalendarHoliday(DateTime? calnDt, string calnCd)
        {
            if (calnDt.HasValue & !string.IsNullOrEmpty(calnCd))
            {
                this.CalnDt = calnDt.Value;
                this.CalnCd = calnCd;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByCalnDtAndCalnCd();
            if (dt.Rows.Count > 0)
            {
                SetHrmAttendCalendarHoliday(dt.Rows[0]);
            }
            else
            {
                this.CalnDt = null;
                this.CalnCd = null;
            }
        }
        /// <summary>
        /// 使用datarow(Object)設定行事曆by天物件
        /// </summary>
        /// <param name="dr"></param>
        private void SetHrmAttendCalendarHoliday(DataRow dr)
        {
            if (dr["CalnDt"] != DBNull.Value && DateTime.TryParse(dr["CalnDt"].ToString(), out _))
            {
                this.CalnDt = DateTime.Parse(dr["CalnDt"].ToString());
            }

            this.CalnCd = dr["CalnCd"].ToString();
            this.CalnCDesc = dr["CalnCDesc"].ToString();
            this.CalnEDesc = dr["CalnEDesc"].ToString();
            this.RtCd = dr["RtCd"].ToString();
            this.RtCDesc = dr["RtCDesc"].ToString();
        }

        private static string BuildFetchCommandString()
        {
            return @"SELECT CalnDt, CalnCd, CalnCDesc, CalnEDesc,RtCd, RtCDesc FROM [db_HAM].[dbo].[View_HRM_ATTEND_CALENDAR_HOLIDAY] t";
        }

        private DataTable FetchByCalnDtAndCalnCd()
        {
            string sql = BuildFetchCommandString() + @" WHERE t.CalnDt = @CalnDt AND t.CalnCd = @CalnCd";
            List<SqlParameter> sps = new List<SqlParameter>() { new SqlParameter("@CalnDt", this.CalnDt), new SqlParameter("@CalnCd", this.CalnCd) };
            DataTable dt = SqlHelper.ExecuteDataset(Config.HAMDBConnectionString, CommandType.Text, sql, sps.ToArray()).Tables[0];

            return dt;
        }

        public static bool IsDateHoliday(DateTime date, string calnCd)
        {
            string sql = BuildFetchCommandString() + @" WHERE t.CalnDt = @CalnDt AND t.CalnCd = @CalnCd";
            List<SqlParameter> sps = new List<SqlParameter>() { new SqlParameter("@CalnDt", date), new SqlParameter("@CalnCd", calnCd) };
            DataTable dt = SqlHelper.ExecuteDataset(Config.HAMDBConnectionString, CommandType.Text, sql, sps.ToArray()).Tables[0];

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 依據起始日期與結束日期擷取法定假日資料。
        /// </summary>
        /// <param name="beginDate">起始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="calnCd">行事曆代碼，如：TW 為中華民國行事曆</param>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchHolidayByBeginAndEndDate(DateTime beginDate, DateTime endDate, string calnCd, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE t.CalnDt >= @BeginDate AND t.CalnDt <= @EndDate AND t.CalnCd = @CalnCd";
            List<SqlParameter> sps = new List<SqlParameter>() { new SqlParameter("@BeginDate", beginDate), new SqlParameter("@EndDate", endDate), new SqlParameter("@CalnCd", calnCd) };
            DataTable dt = SqlHelper.ExecuteDataset(Config.HAMDBConnectionString, CommandType.Text, sql, sps.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    HrmAttendCalendarHolidayCollection col = new HrmAttendCalendarHolidayCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        HrmAttendCalendarHoliday obj = new HrmAttendCalendarHoliday();
                        obj.SetHrmAttendCalendarHoliday(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }
    }

    /// <summary>
    /// HRM 行事曆法定假日集合類別。
    /// </summary>
    public class HrmAttendCalendarHolidayCollection : List<HrmAttendCalendarHoliday>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public HrmAttendCalendarHolidayCollection()
        {

        }
    }

}
