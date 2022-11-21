using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;
using Library.Component.Enums;

namespace Library.Component.BLL
{
    /// <summary>
    /// 表單角色權限與表單團隊類別。
    /// </summary>
    public class ModuleFormRoleRightTeam
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ModuleFormRoleRightTeam));

        #region Property
        public string IDRole { get; set; }

        public string RoleCode { get; set; }

        public string RoleName { get; set; }

        public int Order_ { get; set; }

        public string UperUt { get; set; }

        public string UnitCd { get; set; }

        public string UnitCdV { get; set; }

        public string PostCd { get; set; }

        public string PostCdV { get; set; }

        public RoleTypeEnum RoleTypeEnum { get; set; }

        public bool IsChief { get; set; }

        public string IDPro { get; set; }

        public string RequireCode { get; set; }

        public bool IsEnable { get; set; }

        public string IDFlowTeam { get; set; }

        public long BranchID { get; set; }

        public string EmployID { get; set; }
        #endregion

        private ModuleFormRoleRightTeam()
        {

        }

        private void SetFormRoleRightTeam(DataRow dr)
        {
            this.IDRole = dr["IDRole"].ToString();
            this.RoleCode = dr["RoleCOde"].ToString();
            this.RoleName = dr["RoleName"].ToString();

            if (int.TryParse(dr["Order_"].ToString(), out _))
            {
                this.Order_ = int.Parse(dr["Order_"].ToString());
            }

            this.UperUt = dr["UperUt"].ToString();
            this.UnitCd = dr["UnitCd"].ToString();
            this.UnitCdV = dr["UnitCdV"].ToString();
            this.PostCd = dr["PostCd"].ToString();
            this.PostCdV = dr["PostCdV"].ToString();

            if (Enum.TryParse<RoleTypeEnum>(dr["RoleType"].ToString(), out _))
            {
                this.RoleTypeEnum = (RoleTypeEnum)Enum.Parse(typeof(RoleTypeEnum), dr["RoleType"].ToString());
            }

            if (bool.TryParse(dr["IsChief"].ToString(), out _))
            {
                this.IsChief = bool.Parse(dr["IsChief"].ToString());
            }

            this.IDPro = dr["IDPro"].ToString();
            this.RequireCode = dr["RequireCode"].ToString();

            if (bool.TryParse(dr["IsEnable"].ToString(), out _))
            {
                this.IsEnable = bool.Parse(dr["IsEnable"].ToString());
            }

            this.IDFlowTeam = dr["IDFlowTeam"].ToString();

            if (long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.EmployID = dr["EmployID"].ToString();
        }

        /// <summary>
        /// 擷取表單角色權限與團隊。
        /// </summary>
        /// <param name="moduleName">模組名稱</param>
        /// <param name="formName">表單名稱</param>
        /// <param name="idFlowTeam">表單編號</param>
        /// <param name="proID">流程編號</param>
        /// <param name="enumRoleType">表單角色類型列舉型態</param>
        /// <param name="rot">回傳物件類型列舉型態</param>
        /// <returns></returns>
        public static object FetchFormRoleRightTeam(string moduleName, string formName, string idFlowTeam, string proID, RoleTypeEnum enumRoleType, ReturnObjectTypeEnum rot)
        {
            string sql = string.Format(@"SELECT RD.IDRole, RD.RoleCode, RD.RoleName, RD.Order_
                                                ,RD.UperUt, RD.UnitCd, RD.UnitCdV, RD.PostCd, RD.PostCdV, RD.IsChief, RD.RoleType
                                                ,RR.IDPro, RR.RequireCode, RR.IsEnable
                                                ,RT.IDFlowTeam, RT.BranchID, RT.EmployID, RT.CreateBy, RT.CreateStamp, RT.UpdateBy, RT.UpdateStamp
                                         FROM   fzdb.fzt{0}_role_def RD
                                                INNER JOIN fzdb.fzt{0}_role_right RR ON RD.IDRole = RR.IDRole
                                                LEFT  JOIN fzdb.fzt{0}_role_team  RT ON RD.IDRole = RT.IDRole AND RT.IDFlowTeam = :pIDFlowTeam
                                         WHERE  RD.RoleType = :pRoleType AND RR.IDPro = :pIDPro
                                         ORDER BY RD.Order_, RD.IDRole"
                                        , formName.ToLower());

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFlowTeam", idFlowTeam), new OracleParameter("pRoleType", enumRoleType.ToString()), new OracleParameter("pIDPro", proID) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    ModuleFormRoleRightTeamCollection col = new ModuleFormRoleRightTeamCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ModuleFormRoleRightTeam obj = new ModuleFormRoleRightTeam();
                        obj.SetFormRoleRightTeam(dr);
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
    /// 表單角色權限與表單團隊集合類別。
    /// </summary>
    public class ModuleFormRoleRightTeamCollection : List<ModuleFormRoleRightTeam>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ModuleFormRoleRightTeamCollection()
        {

        }
    }
}