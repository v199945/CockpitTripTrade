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
    /// AIMS 組員職級定義類別。[fzdb].[ci_vvpositions] [acdba].[CI_VvPositions] [CI].[VvPositions]
    /// </summary>
    public class CIvvPositions
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CIvvPositions));

        #region Property
        /// <summary>
        /// AIMS 組員職級編號。
        /// </summary>
        public int ID { get; set; }        
        
        /// <summary>
        /// AIMS 組員職級代碼。
        /// </summary>
        public string Code { get; set; }
        
        public string Code3 { get; set; }
        
        public string Code4 { get; set; }
        
        /// <summary>
        /// AIMS 組員職級描述。
        /// </summary>
        public string Desc { get; set; }
        
        public int AlTrn { get; set; }
        
        /// <summary>
        /// 飛航或客艙組員，0 為飛航組員，1 為客艙組員。
        /// </summary>
        public int CkCb { get; set; }

        /// <summary>
        /// AIMS 組員職級定義等級。
        /// </summary>
        public int Rank { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public CIvvPositions()
        {

        }

        public CIvvPositions(int id)
        {
            if (id > 0)
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
                SetCIVvPositions(dt.Rows[0]);
            }
            else
            {
                this.ID = 0;
            }    
        }

        private void SetCIVvPositions(DataRow dr)
        {
            this.ID = int.Parse(dr["ID"].ToString());
            this.Code = dr["Code"].ToString();
            this.Code3 = dr["Code3"].ToString();
            this.Code4 = dr["Code4"].ToString();
            this.Desc = dr["Desc_"].ToString();
            this.AlTrn = int.Parse(dr["AlTrn"].ToString());
            this.CkCb = int.Parse(dr["Ck_Cb"].ToString());
            this.Rank = int.Parse(dr["Rank"].ToString());
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuilderFetchCommandString()
        {
            return @"SELECT ID, Code, Code3, Code4, Desc_, Altrn, Ck_Cb, Rank FROM fzdb.ci_vvpositions VP";
        }

        private DataTable FetchByID()
        {
            string sql = BuilderFetchCommandString() + @" WHERE ID = :pID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pID", this.ID) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public static object FetchFlightCrewAllPosition(ReturnObjectTypeEnum rot)
        {
            string sql = BuilderFetchCommandString() + @" WHERE VP.Ck_Cb = 0 AND VP.Code <> 'STF'";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    CivvPositionsCollection col = new CivvPositionsCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        CIvvPositions obj = new CIvvPositions();
                        obj.SetCIVvPositions(dr);

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

    public class CivvPositionsCollection : List<CIvvPositions>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public CivvPositionsCollection()
        {

        }
    }
}