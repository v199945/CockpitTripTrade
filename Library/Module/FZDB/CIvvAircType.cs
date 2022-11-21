using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.BLL;
using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Module.FZDB
{
    /// <summary>
    /// AIMS 機型定義類別。[fzdb].[CI_vvAircType] [acdba].[CI_vvAircType] [CI].[vvAircType]
    /// </summary>
    public class CIvvAircType
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CIvvAircType));

        #region Property
        /// <summary>
        /// AIMS 機型編號。
        /// </summary>
        public string AcType { get; set; }

        /// <summary>
        /// AIMS 機型 IATA 代碼。
        /// </summary>
        public string IcaoCode { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public CIvvAircType()
        {

        }

        public CIvvAircType(string acType)
        {
            if (!string.IsNullOrEmpty(acType))
            {
                this.AcType = acType;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByAcType();
            if (dt.Rows.Count > 0)
            {
                SetCIAcType(dt.Rows[0]);
            }
            else
            {
                this.AcType = null;
            }
        }

        private void SetCIAcType(DataRow dr)
        {
            this.AcType = dr["AcType"].ToString();
            this.IcaoCode = dr["IcaoCode"].ToString();
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            //return @"SELECT ICAOCODE, LISTAGG(ACTYPE, ',') WITHIN GROUP (ORDER BY ACTYPE) AS AcType FROM fzdb.ci_vvairctype WHERE IATACODE NOT IN ('AB6', 'E90', 'ERD', 'APQ', 'AT7','100') GROUP BY ICAOCODE";
            //return @"SELECT AcType, ICAOCode FROM (SELECT ICAOCODE, LISTAGG(ACTYPE, ',') WITHIN GROUP (ORDER BY ACTYPE) AS AcType FROM fzdb.ci_vvairctype WHERE IATACODE NOT IN ('AB6', 'E90', 'ERD', 'APQ', 'AT7','100') GROUP BY ICAOCODE ORDER BY AcType)";
            return @"SELECT AcType, ICAOCODE from ( SELECT ICAOCODE, LISTAGG(ACTYPE, ',') WITHIN GROUP (ORDER BY ACTYPE) AS AcType FROM fzdb.ci_vvairctype WHERE ICAOCODE NOT IN ('AB6', 'E90', 'ERD', 'APQ', 'AT7','100') GROUP BY ICAOCODE ORDER BY AcType )";
        }

        private DataTable FetchByAcType()
        {
            string sql = BuildFetchCommandString() + @" WHERE AcType = :pAcType";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pAcType", this.AcType) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public static object FetchAll(ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" ORDER BY ICAOCODE, AcType";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    CIvvAircTypeCollection col = new CIvvAircTypeCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        CIvvAircType obj = new CIvvAircType();
                        obj.SetCIAcType(dr);

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

    public class CIvvAircTypeCollection : List<CIvvAircType>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public CIvvAircTypeCollection()
        {

        }
    }
}