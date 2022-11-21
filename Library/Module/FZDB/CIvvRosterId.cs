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

namespace Library.Module.FZDB
{
    /// <summary>
    /// View [fzdb].[CI_vvRosterId]
    /// AIMS 資料庫 View 物件 [CI].[VvRosterId]
    /// </summary>
    public class CIvvRosterId
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CIvvRosterId));

        #region Property
        public string ID { get; set; }

        /// <summary>
        /// 組員發佈班表最後日期數值，以 1980/01/01 起算之正整數。
        /// </summary>
        public int? Publish { get; set; }

        /// <summary>
        /// 組員發佈班表最後日期。
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// 組員在職狀態。
        /// 0 為在職中，1 為解雇(Dismissed)，2 為退休(Retired)，3 為辭職(Resigned)，4 為解雇(Laid Off)，5 為約聘(Contract To)，
        /// 6 為跳槽(Jump Ship)，7 為(Medical)，8 為已故(Deceased)，9 為(Inactive)，10 為(Transferred)
        /// </summary>
        public int ActTyp { get; set; }

        /// <summary>
        /// 組員在職狀態列舉。
        /// </summary>
        public CIvvRosterIdActTypEnum? ActTypEnum { get; set; }

        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public CIvvRosterId()
        {

        }

        public CIvvRosterId(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                this.ID = id;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByID();
            if (dt.Rows.Count > 0)
            {
                SetCIvvRosterId(dt.Rows[0]);
            }
        }

        private void SetCIvvRosterId(DataRow dr)
        {
            this.ID = dr["ID"].ToString();

            if (dr["Publish"] != DBNull.Value && int.TryParse(dr["Publish"].ToString(), out _))
            {
                this.Publish = int.Parse(dr["Publish"].ToString());
            }

            if (dr["PublishDate"] != DBNull.Value && DateTime.TryParse(dr["PublishDate"].ToString(), out _))
            {
                this.PublishDate = DateTime.Parse(dr["PublishDate"].ToString());
            }

            if (int.TryParse(dr["ActTyp"].ToString(), out _))
            {
                this.ActTyp = int.Parse(dr["ActTyp"].ToString());

                if (Enum.TryParse<CIvvRosterIdActTypEnum>(dr["ActTyp"].ToString(), out _))
                {
                    this.ActTypEnum = Enum.Parse(typeof(CIvvRosterIdActTypEnum), dr["ActTyp"].ToString()) as CIvvRosterIdActTypEnum?;
                }
            }    
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT VRI.ID, VRI.Publish, TO_DATE('1980/01/01', 'YYYY/MM/DD') + VRI.Publish AS PublishDate, VRI.ActTyp FROM fzdb.ci_vvrosterid VRI";
        }

        private DataTable FetchByID()
        {
            string sql = BuildFetchCommandString() + @" WHERE VRI.ID = :pID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter(":pID", this.ID) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 依據飛航組員員工編號擷取已發佈班表之截止日期。
        /// </summary>
        /// <param name="id">飛航組員員工編號</param>
        /// <returns></returns>
        private static DateTime? FetchPublishDateByID(string id)
        {
            string sql = BuildFetchCommandString() + @" WHERE VRI.ID = :pID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter(":pID", id) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            if (dt.Rows.Count > 0 && dt.Rows[0]["PublishDate"] != DBNull.Value && DateTime.TryParse(dt.Rows[0]["PublishDate"].ToString(), out _))
            {
                return DateTime.Parse(dt.Rows[0]["PublishDate"].ToString());
            }

            return null;
        }
    }
}
