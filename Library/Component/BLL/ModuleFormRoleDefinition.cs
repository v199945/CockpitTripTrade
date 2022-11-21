using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using log4net;

using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Component.BLL
{
    /// <summary>
    /// 表單角色定義類別。
    /// </summary>
    public class ModuleFormRoleDefinition
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ModuleFormRoleDefinition));

        #region Property
        public string IDRole { get; set; }

        public string RoleCode { get; set; }

        public string RoleName { get; set; }

        public int Order_ { get; set; }

        public string UnitCd { get; set; }

        public string UnitCdV { get; set; }

        public string PostCd { get; set; }

        public string PostCdV { get; set; }

        public RoleTypeEnum RoleTypeEnum { get; set; }

        public bool IsChief { get; set; }

        public string CreateBy { get; set; }

        public DateTime? CreateStamp { get; set; }

        public string UpdateBy { get; set; }

        public DateTime? UpdateStamp { get; set; }

        #endregion

        private ModuleFormRoleDefinition()
        {

        }

        private void SetFormRole(DataRow dr)
        {
            this.IDRole = dr["IDRole"].ToString();
            this.RoleCode = dr["RoleCOde"].ToString();
            this.RoleName = dr["RoleName"].ToString();

            if (int.TryParse(dr["Order_"].ToString(), out _))
            {
                this.Order_ = int.Parse(dr["Order_"].ToString());
            }

            this.UnitCd = dr["UnitCd"].ToString();
            this.UnitCdV = dr["unitCdv"].ToString();
            this.PostCd = dr["PostCd"].ToString();
            this.PostCdV = dr["PostCdV"].ToString();

            if (Enum.TryParse<RoleTypeEnum>(dr["RoleType"].ToString(), out _))
            {
                this.RoleTypeEnum = (RoleTypeEnum) Enum.Parse(typeof(RoleTypeEnum), dr["RoleType"].ToString());
            }

            if (bool.TryParse(dr["IsChief"].ToString(), out _))
            {
                this.IsChief = bool.Parse(dr["IsChief"].ToString());
            }

            this.CreateBy = dr["CreateBy"].ToString();

            if (dr["CreateStamp"] != DBNull.Value && DateTime.TryParse(dr["CreateStamp"].ToString(), out _))
            {
                this.CreateStamp = DateTime.Parse(dr["CreateStamp"].ToString());
            }

            this.UpdateBy = dr["UpdateBy"].ToString();

            if (dr["UpdateStamp"] != DBNull.Value && DateTime.TryParse(dr["UpdateStamp"].ToString(), out _))
            {
                this.UpdateStamp = DateTime.Parse(dr["UpdateStamp"].ToString());
            }
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT '{0}' AS FormName, RD.IDRole, RD.RoleCode, RD.RoleName, RD.Order_, RD.UnitCd, RD.UnitCdV, RD.PostCd, RD.PostCdV, RD.IsChief, RD.RoleType FROM fzdb.fzt{0}_role_def RD";
        }

        /// <summary>
        /// 擷取表單角色定義。
        /// </summary>
        /// <param name="moduleName">模組名稱</param>
        /// <param name="formName">表單名稱</param>
        /// <returns></returns>
        public static DataTable FetchModuleFormRoleDefinition(string moduleName, string formName)
        {
            string sql = string.Format(BuildFetchCommandString() + @" ORDER BY Order_", formName.ToLower());
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

            return dt;
        }
    }
}