using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Library.Module.HAM;

namespace Library.Component.Utility
{
    /// <summary>
    /// DateTime 輔助密封類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class DateTimeUtility
    {
        private DateTimeUtility()
        {

        }

        /// <summary>
        /// 取得日期參數月份第一天之日期。
        /// </summary>
        /// <param name="dt">欲取得月份第一天日期之日期參數</param>
        /// <returns></returns>
        public static DateTime GetTheBeginOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        /// <summary>
        /// 取得日期參數月份最後一天之日期。
        /// </summary>
        /// <param name="dt">欲取得月份最後一天日期之日期參數</param>
        /// <returns></returns>
        public static DateTime GetTheEndOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        }

        /// <summary>
        /// 依據 HRM 新差勤系統之法定假日資料、起始日期、結束日期與前置工作天數參數，以起始日期計算可換日期。
        /// 20221110 648267:[申請日][換班起始日]中間需間隔兩個工作天
        /// </summary>
        /// <param name="calnCd">行事曆代碼，如：TW 為中華民國行事曆</param>
        /// <param name="beginDate">起始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="leadWorkDays">前置工作天數</param>
        /// <returns></returns>
        public static DateTime? CalculateNextWorkingDay(string calnCd, DateTime beginDate, DateTime endDate, int? leadWorkDays)
        {
            HrmAttendCalendarHolidayCollection col = HrmAttendCalendarHoliday.FetchHolidayByBeginAndEndDate(beginDate, endDate, calnCd, Library.Component.Enums.ReturnObjectTypeEnum.Collection) as HrmAttendCalendarHolidayCollection;

            if (leadWorkDays.HasValue)
            {
                // 由[beginDate]和[leadWorkDays]算[可換班起始日]
                int indexDay = 0;
                int countWorkdays = 0;
                while (countWorkdays != leadWorkDays)
                {
                    indexDay++;

                    if (col.Find(o => o.CalnDt == beginDate.AddDays(indexDay).Date) == null)
                        countWorkdays++;
                }
                // 檢查[可換班起始日]是否在[endDate]之前
                if (beginDate.AddDays(indexDay).Date <= endDate)
                {
                    return beginDate.AddDays(indexDay).Date;
                }

                return null;
            }
            
            return null;
        }

    }
}
