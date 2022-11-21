using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.BLL;
using Library.Component.DAL;

namespace Library.Module.HRDB
{
    /// <summary>
    /// HRDB 部門資料類別。[hrdb].[HrVPbUnitCd]
    /// </summary>
    public class HrVPbUnitCd
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HrVEgEmploy));

        #region Property
        public string DepValue { get; set; }

        public string UnitCd { get; set; }

        public string UnitCdV { get; set; }

        public string CDesc { get; set; }

        public string EDesc { get; set; }

        public string UnitLvl { get; set; }

        public string UperUt { get; set; }

        public string UperUtv { get; set; }

        public DateTime? EffDt { get; set; }

        public DateTime? ExprDt { get; set; }
        #endregion

        public HrVPbUnitCd()
        {

        }

        public HrVPbUnitCd(string unitCd, string unitCdV)
        {
            if (!string.IsNullOrEmpty(unitCd) && !string.IsNullOrEmpty(unitCdV))
            {
                this.UnitCd = unitCd;
                this.UnitCdV = unitCdV;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByUnitCdAndUnitCdV();
            if (dt.Rows.Count > 0)
            {
                SetHrVPbUnitCd(dt.Rows[0]);
            }
            else
            {
                this.UnitCd = null;
                this.UnitCdV = null;
            }
        }

        private void SetHrVPbUnitCd(DataRow dr)
        {
            this.DepValue = dr["DepValue"].ToString();
            this.UnitCd = dr["UnitCD"].ToString();
            this.UnitCdV = dr["UnitCdV"].ToString();
            this.CDesc = dr["Cdesc"].ToString();
            this.EDesc = dr["EDesc"].ToString();
            this.UnitLvl = dr["UnitLvl"].ToString();
            this.UperUt = dr["UperUt"].ToString();
            this.UperUtv = dr["UperUtv"].ToString();

            if (dr["EffDt"] != DBNull.Value && DateTime.TryParse(dr["EffDt"].ToString(), out _))
            {
                this.EffDt = DateTime.Parse(dr["EffDt"].ToString());
            }

            if (dr["ExprDt"] != DBNull.Value && DateTime.TryParse(dr[""].ToString(), out _))
            {
                this.ExprDt = DateTime.Parse(dr["ExprDt"].ToString());
            }
        }

        private static string BuildFetchCommandString()
        {
            return @"SELECT (UnitCd || ',' || UnitCdV) AS DepValue, UnitCd, UnitCdV, CDesc, EDesc, UnitLvl, UperUt, UperUtv, EffDt, ExprDt FROM hrdb.hrvpbunitcd U";
        }

        private DataTable FetchByUnitCdAndUnitCdV()
        {
            string sql = BuildFetchCommandString() + @" WHERE UnitCd = :pUnitCd AND UnitCdV = :pUnitCdV";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pUnitCd", this.UnitCd), new OracleParameter("pUnitCdV", this.UnitCdV) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 依據上級單位編碼擷取部門單位。
        /// </summary>
        /// <param name="uperUt">上級單位編碼</param>
        /// <param name="isEffective">是否擷取當下有效部門</param>
        /// <returns></returns>
        public static DataTable FetchByUperUt(string uperUt, bool isEffective)
        {
            string sql = BuildFetchCommandString() + @" WHERE U.UperUt = :pUperUt" + (isEffective ? @" AND U.EffDt<=TRUNC(SYSDATE) AND U.ExprDt>=TRUNC(SYSDATE)" : string.Empty) + @" ORDER BY U.UnitCd";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pUperUt", uperUt) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 依據一級單位編碼擷取部門單位。
        /// </summary>
        /// <param name="leadUt">一級單位編碼</param>
        /// <param name="isEffective">是否擷取當下有效部門</param>
        /// <returns></returns>
        public static DataTable FetchByLeadUt(string leadUt, bool isEffective)
        {
            string sql = BuildFetchCommandString() + @" WHERE U.LeadUt = :pLeadUt" + (isEffective ? @" AND U.EffDt<=TRUNC(SYSDATE) AND U.ExprDt>=TRUNC(SYSDATE)" : string.Empty) + @" ORDER BY U.unitlvl, U.listseq";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pLeadUt", leadUt) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }
    }
}