using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Component.BLL
{
    /// <summary>
    /// 版本號類別。
    /// </summary>
    public class Version
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Version));

        #region Property
        public long BranchID { get; set; }

        public string VersionValue { get; set; }

        public string UpdateBy { get; set; }

        public DateTime UpdateStamp { get; set; }
        #endregion

        public Version()
        {

        }

        private void SetVersion(DataRow dr)
        {
            this.BranchID = long.Parse(dr["BranchID"].ToString());
            this.VersionValue = dr["Version"].ToString();
            this.UpdateBy = dr["UpdateBy"].ToString();
            this.UpdateStamp = DateTime.Parse(dr["UpdateStamp"].ToString());
        }

        /// <summary>
        /// 擷取表單物件之版本記錄。
        /// </summary>
        /// <param name="formName">表單名稱</param>
        /// <param name="id">表單編號</param>
        /// <param name="rot">回傳類型列舉型態</param>
        /// <returns></returns>
        public static object FetObjectLog(string formName, string id, ReturnObjectTypeEnum rot)
        {
            string sql = string.Format(@"SELECT '{0}' AS FormName, T.* FROM fzdb.fzt{0}_log T WHERE ID{0} = :pID ORDER BY BranchID DESC", formName.ToLower());
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pFormName", formName.ToLower()), new OracleParameter("pID", id) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    VersionCollection col = new VersionCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Version obj = new Version();
                        obj.SetVersion(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

        /// <summary>
        /// 取得版本資料表 DataTable 物件轉換為版本集合物件 Versions。
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static VersionCollection GetVersionCollection(DataTable dt)
        {
            VersionCollection col = new VersionCollection();
            foreach (DataRow dr in dt.Rows)
            {
                Version obj = new Version();
                obj.SetVersion(dr);
                col.Add(obj);
            }

            return col;
        }

        /// <summary>
        /// 版本號初始值。
        /// </summary>
        /// <returns></returns>
        public static string GetInitialVersion()
        {
            return @"A.1";
        }
    }

    /// <summary>
    /// 版本集合類別。
    /// </summary>
    public class VersionCollection : List<Version>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public VersionCollection()
        {

        }

        /// <summary>
        /// 自版本集合物件取得前一版本值之物件。
        /// </summary>
        /// <param name="branchID">前一版本值</param>
        /// <returns></returns>
        public Version GetPreviousVersion(long branchID)
        {
            foreach (Version v in this)
            {
                if (v.BranchID == branchID)
                {
                    if (this.IndexOf(v) > 0)
                        return this[this.IndexOf(v) - 1];
                    else
                        return this[0];
                }
            }

            return null;
        }

        /// <summary>
        /// 自版本集合物件取得當前版本值之物件。
        /// </summary>
        /// <param name="branchID">當前版本值</param>
        /// <returns></returns>
        public Version GetVersion(long branchID)
        {
            foreach (Version v in this)
            {
                if (v.BranchID == branchID)
                    return v;
            }

            return null;
        }
    }
}