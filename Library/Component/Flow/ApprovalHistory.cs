using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Component.Flow
{
    /// <summary>
    /// 流程記錄類別。
    /// </summary>
    public class ApprovalHistory
    {
        #region Property
        /// <summary>
        /// 流程名稱，即表單名稱。
        /// </summary>
        public string FlowName { get; set; }

        /// <summary>
        /// 關卡。
        /// </summary>
        public string Task { get; set; }

        /// <summary>
        /// 簽核者。
        /// </summary>
        public string Signee { get; set; }

        /// <summary>
        /// 動作。
        /// </summary>
        public string Action { get; set; }

        public string Comments { get; set; }

        /// <summary>
        /// 送達時間。
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 完成時間。
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 歷時(單位：小時)
        /// </summary>
        public double Duration
        {
            get
            {
                if (this.StartTime.HasValue)
                {
                    TimeSpan ts = new TimeSpan((this.EndTime.HasValue ? this.EndTime.Value.Ticks : DateTime.Now.Ticks) - this.StartTime.Value.Ticks);
                    return ts.TotalMinutes / 60;
                }

                return 0.0;
            }
        }

        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ApprovalHistory()
        { 
        }

        private void SetApprovalHistory(DataRow dr)
        {
            this.Task = dr["Task"].ToString();
            this.Signee = dr["Signee"].ToString();
            this.Action = dr["Action"].ToString();
            this.Comments = dr["Comments"].ToString();

            if (dr["StartTime"] != DBNull.Value && DateTime.TryParse(dr["StartTime"].ToString(), out _))
            {
                this.StartTime = DateTime.Parse(dr["StartTime"].ToString());
            }

            if (dr["EndTime"] != DBNull.Value && DateTime.TryParse(dr["EndTime"].ToString(), out _))
            {
                this.EndTime = DateTime.Parse(dr["EndTime"].ToString());
            }
        }

        /// <summary>
        /// 依據傳入之表單流程記錄 DataTable 物件轉換為流程記錄集合物件。
        /// </summary>
        /// <param name="flowName">流程名稱，即為表單名稱</param>
        /// <param name="dt">表單流程記錄 DataTable 物件</param>
        /// <returns></returns>
        public static ApprovalHistoryCollection GetApprovalHistoryCollection(string flowName, DataTable dt)
        {
            ApprovalHistoryCollection col = new ApprovalHistoryCollection();
            //var query = from ah in dt.AsEnumerable()
            //                //where ah.Field<string>("IsShowInHistory").Equals("true", StringComparison.OrdinalIgnoreCase)
            //            group ah by ah.Field<string>("DisplayStatus") into g
            //            select new
            //            {
            //                UpdateBy = g.Aggregate(p => p["UpdateBy"],),
            //                MinUpdateStamp = g.Min(p => (DateTime)p["UpdateStamp"]),
            //                MaxUpdateStamp = g.Max(p => (DateTime)p["UpdateStamp"]),
            //                DisplayStatus = g.Key
            //            }
            //            ;
            foreach (DataRow dr in dt.Rows)
            {
                ApprovalHistory obj = new ApprovalHistory();
                obj.FlowName = flowName;
                obj.SetApprovalHistory(dr);

                col.Add(obj);
            }

            return col;
        }
    }

    /// <summary>
    /// 流程記錄集合類別。
    /// </summary>
    public class ApprovalHistoryCollection : List<ApprovalHistory>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ApprovalHistoryCollection()
        {

        }
    }
}
