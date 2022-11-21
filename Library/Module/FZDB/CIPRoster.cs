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
    public class CIPRoster
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CIPRoster));

        #region Property
        #endregion

        public static DateTime? FetchMaxPublishDate()
        {
            string sql = @"SELECT TO_DATE('1980/01/01', 'YYYY/MM/DD') + MAX(PR.DutyDay) AS MaxPublishDate FROM fzdb.ci_proster PR";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];
            if (dt.Rows.Count > 0 && dt.Rows[0]["MaxPublishDate"] != DBNull.Value && DateTime.TryParse(dt.Rows[0]["MaxPublishDate"].ToString(), out _))
            {
                return DateTime.Parse(dt.Rows[0]["MaxPublishDate"].ToString());
            }

            return null;
        }
    }
}
