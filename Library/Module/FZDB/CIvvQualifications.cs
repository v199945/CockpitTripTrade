using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.BLL;
using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Module.FZDB
{
    /// <summary>
    /// AIMS 組員任職基地、機型與職級資格類別。[fzdb].[ci_vvqualifications] [acdba].[CI_VvQualifications] [CI].[VvQualifications]
    /// </summary>
    public class CIvvQualifications
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CIvvQualifications));

        #region Property
        /// <summary>
        /// 組員員工編號。
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 組員基地。
        /// </summary>
        public string CwBase { get; set; }

        /// <summary>
        /// 組員機型編號。
        /// </summary>
        public int Ac { get; set; }

        /// <summary>
        /// 組員職級編號。
        /// </summary>
        public int Pos { get; set; }

        /// <summary>
        /// 主要資格否， 0 為 Primary、1 為 Secondary Qualification。
        /// </summary>
        public int Primary { get; set; }

        /// <summary>
        /// AIMS 組員任職基地、機型與職級資格序號。
        /// </summary>
        public int Idx { get; set; }

        /// <summary>
        /// AIMS 組員任職基地、機型與職級資格有效起始日
        /// </summary>
        public int ActivBeg { get; set; }

        /// <summary>
        /// AIMS 組員任職基地、機型與職級資格有效結束日
        /// </summary>
        public int ActivEnd { get; set; }

        /// <summary>
        /// AIMS 組員任職基地、機型與職級資格有效起始日期
        /// </summary>
        public DateTime ActiveBegDate { get; set; }

        /// <summary>
        /// AIMS 組員任職基地、機型與職級資格有效結束日期
        /// </summary>
        public DateTime ActiveEndDate { get; set; }

        /// <summary>
        /// AIMS 飛航組員職級代碼。
        /// </summary>
        public string CrewPosCode { get; set; }

        /// <summary>
        /// AIMS 飛航組員機隊代碼。
        /// </summary>
        public string FleetCode { get; set; }
        #endregion

        private CIvvQualifications()
        {

        }

        private void SetCIvvQualifications(DataRow dr)
        {
            this.ID = dr["ID"].ToString();
            this.CwBase = dr["CwBase"].ToString();
            this.Ac = int.Parse(dr["Ac"].ToString());
            this.Pos = int.Parse(dr["Pos"].ToString());
            this.Primary = int.Parse(dr["Primary_"].ToString());
            this.Idx = int.Parse(dr["Idx"].ToString());
            this.ActivBeg = int.Parse(dr["ActivBeg"].ToString());
            this.ActivEnd = int.Parse(dr["ActivEnd"].ToString());
            this.ActiveBegDate = DateTime.Parse(dr["ActiveBegDate"].ToString());
            this.ActiveEndDate = DateTime.Parse(dr["ActiveEndDate"].ToString());
            this.CrewPosCode = dr["CrewPosCode"].ToString();
            this.FleetCode = dr["FleetCode"].ToString();
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT CAST(LPAD(VQ.ID, 6, '0') AS VARCHAR2(6)) AS ID
                            ,VQ.CwBase, VQ.Ac, VQ.Pos, VQ.QualCat, VQ.Primary_, VQ.Idx, VQ.ActivBeg, VQ.ActivEnd
                            ,(TO_DATE('1980/01/01', 'YYYY/MM/DD') + VQ.ActivBeg) AS ActiveBegDate
                            ,(TO_DATE('1980/01/01', 'YYYY/MM/DD') + VQ.ActivEnd) AS ActiveEndDate
                            ,VP.Code AS CrewPosCode
                            ,VACT.IcaoCode AS FleetCode
                     FROM   fzdb.ci_vvqualifications       VQ
                            INNER JOIN fzdb.ci_vvpositions VP   ON VQ.Pos = VP.ID
                            INNER JOIN fzdb.ci_vvairctype  VACT ON VQ.Ac  = VACT.AcType";
        }

        /// <summary>
        /// 以組員員工編號擷取組員任職基地、機型與職級之資格。
        /// </summary>
        /// <param name="id">組員員工編號</param>
        /// <param name="isPrimary">是否擷取主要資格</param>
        /// <param name="isonlyeffective">是否進擷取組員當下時間有效之任職基地、機型與職級資格</param>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchByID(string id, bool isPrimary, bool isOnlyEffective, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE VQ.ID = TO_NUMBER(CAST(LPAD(:pID, 6, '0') AS VARCHAR2(6)))";

            if (isPrimary)
            {
                sql += @" AND VQ.Primary_ = 0";
            }

            if (isOnlyEffective)
            {
                sql += @" AND VQ.ActivBeg <= SYSDATE - TO_DATE('1980/01/01', 'YYYY/MM/DD') AND VQ.ActivEnd >= SYSDATE - TO_DATE('1980/01/01', 'YYYY/MM/DD')";
            }            

            sql += @" ORDER BY VQ.Pos, VQ.ActivBeg, VQ.ActivEnd, VQ.Ac";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pID", id) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    CIvvQualificationsCollection col = new CIvvQualificationsCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        CIvvQualifications obj = new CIvvQualifications();
                        obj.SetCIvvQualifications(dr);
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
    /// AIMS 組員任職基地、機型與職級資格集合類別。
    /// </summary>
    public class CIvvQualificationsCollection : List<CIvvQualifications>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public CIvvQualificationsCollection()
        {

        }
    }
}