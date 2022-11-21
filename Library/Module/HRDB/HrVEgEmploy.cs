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

namespace Library.Module.HRDB
{
    /// <summary>
    /// HRDB 員工基本資料類別。[hrdb].[HrvEgEmploy]
    /// </summary>
    public class HrVEgEmploy
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HrVEgEmploy));

        #region Property
        /// <summary>
        /// 員工編號。
        /// </summary>
        public string EmployID { get; set; }

        /// <summary>
        /// 在職否識別，是否在職中。
        /// </summary>
        public string ExstFlg { get; set; }

        /// <summary>
        /// 員工中文姓名。
        /// </summary>
        public string CName { get; set; }

        /// <summary>
        /// 中文姓英譯。
        /// </summary>
        public string LName { get; set; }

        /// <summary>
        /// 中文名英譯。
        /// </summary>
        public string FName { get; set; }

        /// <summary>
        /// 人員屬性 / 職務類別。
        /// 000 為一級正主管，100 為飛航組員，200 為客艙組員，300 為修護人員，
        /// 400 為客運營業人員，410 為貨運營業人員，420 為營運人員，500 為運務人員，
        /// 600 為稽核及財會人員，700 為資訊人員，800 為其他人員。
        /// </summary>
        public string AnalySa { get; set; }

        /// <summary>
        /// 員工單位編號。
        /// </summary>
        public string UnitCd { get; set; }

        /// <summary>
        /// 員工單位編號版本。
        /// </summary>
        public string UnitCdV { get; set; }

        /// <summary>
        /// 員工職稱代碼。
        /// </summary>
        public string PostCd { get; set; }

        /// <summary>
        /// 員工職稱代碼版本。
        /// </summary>
        public string PostCdV { get; set; }

        /// <summary>
        /// 主管代碼。999 非主管、11 一級正主管、12 一級副主管、21 二級正主管，以此類推。
        /// </summary>
        public string ChiefCd { get; set; }

        /// <summary>
        /// 員工編號/中文姓名。
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 中文姓名英譯。
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 員工編號/中文姓名英譯。
        /// </summary>
        public string DisplayEnglishName { get; set; }

        /// <summary>
        /// 單位中文名稱，來源 [hrdb].[hrvpbunitcd].[CDesc] 欄位。
        /// </summary>
        public string DepCDesc { get; set; }

        /// <summary>
        /// 單位英文名稱，來源 [hrdb].[hrvpbunitcd].[EDesc] 欄位。
        /// </summary>
        public string DepEDesc { get; set; }

        /// <summary>
        /// 一級單位中文名稱。
        /// </summary>
        public string DivCDesc { get; set; }

        /// <summary>
        /// 一級單位英文名稱。
        /// </summary>
        public string DivEDesc { get; set; }

        /// <summary>
        /// 一級單位中文名稱/單位中文名稱。
        /// </summary>
        public string DisplayDepName { get; set; }

        /// <summary>
        /// 一級單位英文名稱/單位英文名稱。
        /// </summary>
        public string DisplayDepEnglishName { get; set; }


        /// <summary>
        /// 員工職稱中文名稱。
        /// </summary>
        public string PosCDesc { get; set; }

        /// <summary>
        /// 員工職稱英文名稱。
        /// </summary>
        public string PosEDesc { get; set; }

        /// <summary>
        /// 員工電子郵件。AnalySa 為 100 飛航組員與 200 客艙組員僅有全員信箱，其餘人員皆有 IBM Lotus Notes 與全員信箱。
        /// </summary>
        public string Email { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public HrVEgEmploy()
        {

        }

        public HrVEgEmploy(string employID)
        {
            if (!string.IsNullOrEmpty(employID))
            {
                this.EmployID = employID;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByEmployID();
            if (dt.Rows.Count > 0)
            {
                SetHrVEgEmploy(dt.Rows[0]);
            }
            else
            {
                this.EmployID = null;
            }
        }

        private void SetHrVEgEmploy(DataRow dr)
        {
            this.EmployID = dr["EmployID"].ToString();
            this.ExstFlg = dr["ExstFlg"].ToString();
            this.CName = dr["CName"].ToString();
            this.LName = dr["LName"].ToString();
            this.FName = dr["FName"].ToString();
            this.AnalySa = dr["AnalySa"].ToString();
            this.UnitCd = dr["UnitCd"].ToString();
            this.UnitCdV = dr["UnitCdV"].ToString();
            this.PostCd = dr["PostCd"].ToString();
            this.PostCdV = dr["PostCDV"].ToString();
            this.DisplayName = dr["DisplayName"].ToString();
            this.EnglishName = dr["EnglishName"].ToString();
            this.DisplayEnglishName = dr["DisplayEnglishName"].ToString();
            this.DepCDesc = dr["DepCDesc"].ToString();
            this.DepEDesc = dr["DepEDesc"].ToString();
            this.DivCDesc = dr["DivCDesc"].ToString();
            this.DivEDesc = dr["DivEDesc"].ToString();
            this.DisplayDepName = dr["DisplayDepName"].ToString();
            this.DisplayDepEnglishName = dr["DisplayDepEnglishName"].ToString();
            this.PosCDesc = dr["PosCDesc"].ToString();
            this.PosEDesc = dr["PosEDesc"].ToString();
            this.Email = this.EmployID + (this.AnalySa == @"100" || this.AnalySa == @"200" ? "@cal.aero" : "@china-airlines.com");
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT E.EmployID, E.ExstFlg, E.CName, E.LName, E.FName, E.AnalySa, E.UnitCd, E.UnitCdV, E.PostCd, E.PostCdV
                            ,(E.EmployID || '/' || E.CName) AS DisplayName, (E.LName || '/' || E.FName) AS EnglishName, (E.EmployID || ' / ' || E.LName || ',' || E.FName) AS DisplayEnglishName
                            ,Dep.CDesc AS DepCDesc, Dep.EDesc AS DepEDesc
                            ,Div.CDesc AS DivCDesc, Div.EDesc AS DivEDesc
                            ,(Div.CDesc || '/' || Dep.CDesc) AS DisplayDepName, (Div.EDesc || '/' || Dep.EDesc) AS DisplayDepEnglishName
                            ,Pos.CDesc AS PosCDesc, Pos.EDesc AS PosEDesc
                     FROM   hrdb.hrvegemploy E
                            INNER JOIN hrdb.hrvpbunitcd Dep ON E.UnitCd        = Dep.UnitCd
                            INNER JOIN hrdb.hrvpbunitcd Div ON Dep.UperUt      = Div.UnitCd
                            INNER JOIN hrdb.hrvpbpostcd Pos ON Pos.PostCd      = E.PostCd
                                                               AND Pos.PostCdv = E.PostCdv
                    ";
                     //WHERE  E.ExstFlg = 'Y'
        }

        private DataTable FetchByEmployID()
        {
            string sql = BuildFetchCommandString() + @" WHERE E.EmployID = :pEmployID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pEmployID", this.EmployID) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 依據部門編號與部門編號版本擷取員工。
        /// </summary>
        /// <param name="unitCd">部門編號</param>
        /// <param name="unitCdV">部門編號版本</param>
        /// <param name="isChief">是否擷取主管</param>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchByUnitCdAndUnitCdV(string unitCd, string unitCdV, bool isChief, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE E.ExstFlg = 'Y' AND E.UnitCd = :pUnitCd AND E.UnitCdV = :pUnitCdV";
            if (isChief)
            {
                sql += @" AND E.ChiefCd <> '999'";// 非主管職為 999
            }
            sql += @" ORDER BY E.EmployID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pUnitCd", unitCd), new OracleParameter("pUnitCdV", unitCdV) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    HrVEgEmployCollection col = new HrVEgEmployCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        HrVEgEmploy obj = new HrVEgEmploy();
                        obj.SetHrVEgEmploy(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

        public static object FetchByUnitCdAndUnitCdVAndPostCd(string unitCd, string unitCdV, string postCd, ReturnObjectTypeEnum rot)
        {
            string sql = BuildFetchCommandString() + @" WHERE E.ExstFlg = 'Y' AND E.UnitCd = :pUnitCd AND E.UnitCdV = :pUnitCdV AND E.PostCd = :pPostCd ORDER BY E.EmployID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pUnitCd", unitCd), new OracleParameter("pUnitCd", unitCd),
                                                                        new OracleParameter("pUnitCdV", unitCdV), new OracleParameter(":pPostCd", postCd) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    HrVEgEmployCollection col = new HrVEgEmployCollection();
                    foreach(DataRow dr in dt.Rows)
                    {
                        HrVEgEmploy obj = new HrVEgEmploy();
                        obj.SetHrVEgEmploy(dr);
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
    /// HRDB 員工基本資料集合類別。
    /// </summary>
    public class HrVEgEmployCollection : List<HrVEgEmploy>
    {
        public HrVEgEmployCollection()
        {

        }
    }
}