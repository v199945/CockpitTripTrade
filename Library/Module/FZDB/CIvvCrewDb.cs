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
    /// AIMS 組員基本資料類別。[fzdb].[ci_vvCrewDb] [acdba].[CI_VvCrewDb] [CI].[VvCrewDb]
    /// </summary>
    public class CIvvCrewDb
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CIvvCrewDb));

        #region Property
        /// <summary>
        /// 組員員工編號。
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 組員電子郵件信箱。
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 組員護照姓名。
        /// </summary>
        public string PassportName { get; set; }

        /// <summary>
        /// 組員顯示姓名，[員工編號]/[護照姓名]。
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 組員班表是否開放讓他人查詢，1/0
        /// </summary>
        /// 20221103 648267:新增屬性[AllowQuery]，以符合eCrew設定是否讓他人查詢自己班表
        public string AllowQuery { get; set; }

        /// <summary>
        /// 組員當下時間有效之任職基地、機型與職級資格集合物件。
        /// </summary>
        public CIvvQualificationsCollection EffectiveQualifications
        {
            get
            {
                return (CIvvQualificationsCollection) CIvvQualifications.FetchByID(this.ID, true, true, ReturnObjectTypeEnum.Collection);
            }
        }

        /// <summary>
        /// 組員之任職基地、機型與職級資格集合物件(包含當下時間有效與無效)。
        /// </summary>
        public CIvvQualificationsCollection AllQualifications
        {
            get
            {
                return (CIvvQualificationsCollection) CIvvQualifications.FetchByID(this.ID, false, false, ReturnObjectTypeEnum.Collection);
            }
        }

        /// <summary>
        /// CIvvRosterId 物件。
        /// </summary>
        public CIvvRosterId CIvvRosterId { get; set; }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public CIvvCrewDb()
        {

        }

        public CIvvCrewDb(string id)
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
                SetCIvvCrewDb(dt.Rows[0]);
            }
            else
            {
                this.ID = null;
            }
        }

        private void SetCIvvCrewDb(DataRow dr)
        {
            this.ID = dr["ID"].ToString();
            this.Email = dr["Email"].ToString();
            this.PassportName = dr["PassportName"].ToString();
            this.DisplayName = dr["DisplayName"].ToString();
            // 20221103 648267:設定AllowQuery屬性
            this.AllowQuery = dr["AllowQuery"].ToString();

            this.CIvvRosterId = new CIvvRosterId(this.ID);
            
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            // 20221103 648267:修改Query string，多取得AllowQuery欄位
            return @"SELECT CAST(LPAD(ID, 6, '0') AS VARCHAR2(6)) AS ID, TRIM(Email)                                                               AS Email
                            ,TRIM(PasspNnames_1 || ' ' || PasspNnames_2 || ' ' || PasspNnames_3)                                                   AS PassportName
                            ,(CAST(LPAD(ID, 6, '0') AS VARCHAR2(6)) || '/' || TRIM(PasspNnames_1 || ' ' || PasspNnames_2 || ' ' || PasspNnames_3)) AS DisplayName
                            ,ALLOW_OTHER_CREW_VIEW_MY_SCHED AS AllowQuery
                     FROM   fzdb.ci_vvcrewdb";
        }

        private DataTable FetchByID()
        {
            string sql = BuildFetchCommandString() + @" WHERE ID = TO_NUMBER(CAST(LPAD(:pID, 6, '0') AS VARCHAR2(6)))";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pID", this.ID) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 取得群組化後的 AIMS 系統機隊編號。
        /// </summary>
        /// <returns></returns>
        public string GetGroupByAcID()
        {
            var acs = this.EffectiveQualifications.GroupBy(o => o.Ac).OrderBy(o=>o.Key);
            string resule = null;
            foreach (var ac in acs)
            {
                resule += ac.Key + @",";
            }

            return resule.TrimEnd(',');
        }

        /// <summary>
        /// 取得群組化後的 AIMS 系統職級編號 (1:CA  2:FO  3:RP)。
        /// </summary>
        /// <returns></returns>
        public int GetGroupByPosID()
        {
            var ps = this.EffectiveQualifications.GroupBy(o => o.Pos);
            if (ps.Count() > 1)
            {
                // TODO: Throw exception
                return 0;
            }
            else
            {
                return ps.First().Key;
            }
        }

        /// <summary>
        /// 取得群組化後的 AIMS 系統機隊代碼。
        /// </summary>
        /// <returns></returns>
        public string GetGroupByFleetCode()
        {
            var fleetcodes = this.EffectiveQualifications.GroupBy(o => o.FleetCode);
            string result = null;
            foreach (var fc in fleetcodes)
            {
                result += fc.Key + @",";
            }

            return result.TrimEnd(',');
        }

        /// <summary>
        /// 取得群組化後的 AIMS 系統職級代碼。
        /// </summary>
        /// <returns></returns>
        public string GetGroupByCrewPosCode()
        {
            var crewposcodes = this.EffectiveQualifications.GroupBy(o => o.CrewPosCode);
            string result = null;
            foreach (var cpc in crewposcodes)
            {
                result += cpc.Key + @",";
            }

            return result.TrimEnd(',');
        }
    }
}