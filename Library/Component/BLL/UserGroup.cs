using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Component.BLL
{
    /// <summary>
    /// 使用者群組類別。
    /// </summary>
    public class UserGroup
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UserGroup));

        #region Property
        /// <summary>
        /// 群組編號。
        /// </summary>
        public string IDBllGroup { get; set; }

        /// <summary>
        /// 群組代碼。
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// 群組名稱。
        /// </summary>
        public string GroupName { get; set; }

        #endregion


        /// <summary>
        /// 預設建構子。
        /// </summary>
        private UserGroup()
        {

        }

        private void SetUserGroup(DataRow dr)
        {
            this.IDBllGroup = dr["IDBllGroup"].ToString();
            this.GroupCode = dr["GroupCode"].ToString();
            this.GroupName = dr["GroupName"].ToString();
        }

        /// <summary>
        /// 依據使用者員工編號與使用者部門代碼擷取使用者群組資料。
        /// </summary>
        /// <param name="userID">使用者員工編號</param>
        /// <param name="unitCd">使用者部門代碼</param>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchByUseridAndUnitCd(string userID, string unitCd, ReturnObjectTypeEnum rot)
        {
            string sql = @"SELECT GUS.IDBllGroup, G.GroupCode, G.GroupName
                           FROM   fzdb.fztbllgroupuser GUS
                                  INNER JOIN fzdb.fztbllgroup G ON GUS.IDBllGroup = G.IDBllGroup
                           WHERE  GUS.IDUser = :pIDUser
                           UNION ALL
                           SELECT GUT.IDBllGroup, G.GroupCode, G.GroupName
                           FROM   fzdb.fztbllgroupunit GUT
                                  INNER JOIN fzdb.fztbllgroup G ON GUT.IDBllGroup = G.IDBllGroup
                           WHERE  GUT.UnitCd = :pUnitCd";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDUser", userID), new OracleParameter("pUnitCd", unitCd) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    UserGroupCollection col = new UserGroupCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        UserGroup obj = new UserGroup();
                        obj.SetUserGroup(dr);
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
    /// 使用者群組集合類別。
    /// </summary>
    public class UserGroupCollection : List<UserGroup>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public UserGroupCollection()
        {

        }
    }
}
