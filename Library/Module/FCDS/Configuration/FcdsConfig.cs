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

namespace Library.Module.FCDS.Configuration
{
    /// <summary>
    /// 飛航組員任務換班申請系統之機隊職級設定類別。
    /// </summary>
    public class FcdsConfig
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FcdsConfig));

        #region Property
        /// <summary>
        /// 表單編號。
        /// </summary>
        public string IDFcdsConfig { get; set; }

        /// <summary>
        /// 飛航組員機隊 ID，來源為[fzdb].[ci_vvairctype]，即為[CI].[AircType].[AcType] AIMS AcType ID 。
        /// </summary>
        public string IDAcType { get; set; }

        /// <summary>
        /// 飛航組員機隊 ID，來源為[fzdb].[ci_vvairctype]，即為[CI].[AircType].[AcType] AIMS AcType ID 。
        /// </summary>
        public string AcType { get; set; }

        /// <summary>
        /// 飛航組員機隊代碼。
        /// </summary>
        public string FleetCode { get; set; }

        /// <summary>
        /// 飛航組員職級 ID，來源為[fzdb].[ci_vvpositions]，即為[CI].VvPositions].[ID] AIMS CrewPos ID 。
        /// </summary>
        public int? IDCrewPos { get; set; }

        /// <summary>
        /// 飛航組員職級 ID，來源為[fzdb].[ci_vvpositions]，即為[CI].VvPositions].[ID] AIMS CrewPos ID 。
        /// </summary>
        public int CrewPos { get; set; }

        /// <summary>
        /// 飛航組員職級。
        /// </summary>
        public string CrewPosCode { get; set; }
        
        /// <summary>
        /// 版本值。
        /// </summary>
        public long? BranchID { get; set; }
        
        /// <summary>
        /// 版本號。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 機隊對應部門。
        /// </summary>
        public string FleetDepCode { get; set; }

        /// <summary>
        /// 每月申請次數。
        /// </summary>
        public int? NumOfMonth { get; set; }

        /// <summary>
        /// 一次一單否。
        /// </summary>
        public bool? IsOneReqATime { get; set; }

        /// <summary>
        /// 可撤回否。
        /// </summary>
        public bool? IsApplicantRevoke { get; set; }

        /// <summary>
        /// 前置工作天。
        /// </summary>
        public int? LeadWorkdays { get; set; }

        /// <summary>
        /// 跨月班期限。
        /// </summary>
        public int? DeadlineOfAcrossMonth { get; set; }

        /// <summary>
        /// 建立者。
        /// </summary>
        public string CreateBy { get; set; }
        
        /// <summary>
        /// 建立時間。
        /// </summary>
        public DateTime? CreateStamp { get; set; }
        
        /// <summary>
        /// 更新者。
        /// </summary>
        public string UpdateBy { get; set; }
        
        /// <summary>
        /// 更新時間。
        /// </summary>
        public DateTime? UpdateStamp { get; set; }

        /// <summary>
        /// 版本集合物件。
        /// </summary>
        public VersionCollection Versions
        {
            get
            {
                return Component.BLL.Version.GetVersionCollection(FetchLog());
            }
        }
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public FcdsConfig()
        {

        }

        public FcdsConfig(string idFcdsConfig)
        {
            if (!string.IsNullOrEmpty(idFcdsConfig))
            {
                this.IDFcdsConfig = idFcdsConfig;

                Load();
            }
        }

        public FcdsConfig(string idAcType, int? idCrewPos)
        {
            if (!string.IsNullOrEmpty(idAcType) && idCrewPos.HasValue)
            {
                this.IDAcType = idAcType;
                this.IDCrewPos = idCrewPos;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt;
            if (!string.IsNullOrEmpty(this.IDFcdsConfig))
            {
                dt = FetchByIDFcdsConfig();
            }
            else
            {
                dt = FetchByIDAcTypeAndIDCrewPos();
            }
            
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["IDAcType"] == DBNull.Value || dt.Rows[0]["IDCrewPos"] == DBNull.Value)
                {
                    this.IDAcType = null;
                    this.AcType = dt.Rows[0]["AcType"].ToString();
                    this.FleetCode = dt.Rows[0]["FleetCode"].ToString();
                    this.IDCrewPos = null;

                    if (int.TryParse(dt.Rows[0]["CrewPos"].ToString(), out _))
                    {
                        this.CrewPos = int.Parse(dt.Rows[0]["CrewPos"].ToString());
                    }

                    this.CrewPosCode = dt.Rows[0]["CrewPosCode"].ToString();
                }
                else
                {
                    SetFcdsConfig(dt.Rows[0]);
                }
            }
        }

        private void SetFcdsConfig(DataRow dr)
        {
            if (dr["IDFcdsConfig"] != DBNull.Value)
            {
                this.IDFcdsConfig = dr["IDFcdsConfig"].ToString();
            }

            this.IDAcType = dr["IDAcType"].ToString();
            this.AcType = dr["AcType"].ToString();
            this.FleetCode = dr["FleetCode"].ToString();

            if (dr["IDCrewPos"] != DBNull.Value)
            {
                this.IDCrewPos = (int) dr["IDCrewPos"];
            }
            else
            {
                this.IDCrewPos = (int) dr["CrewPos"];
            }
            this.CrewPos = (int) dr["CrewPos"];
            this.CrewPosCode = dr["CrewPosCode"].ToString();

            if (dr["BranchID"] != DBNull.Value && long.TryParse(dr["BranchID"].ToString(), out _))
            {
                this.BranchID = long.Parse(dr["BranchID"].ToString());
            }

            this.Version = dr["Version"].ToString();
            this.FleetDepCode = dr["FleetDepCode"].ToString();

            if (dr["NumOfMonth"] != DBNull.Value && int.TryParse(dr["NumOfMonth"].ToString(), out _))
            {
                this.NumOfMonth = int.Parse(dr["NumOfMonth"].ToString());
            }

            if (dr["IsOneReqATime"] != DBNull.Value && bool.TryParse(dr["IsOneReqATime"].ToString(), out _))
            {
                this.IsOneReqATime = bool.Parse(dr["IsOneReqATime"].ToString());
            }

            if (dr["IsApplicantRevoke"] != DBNull.Value && bool.TryParse(dr["IsApplicantRevoke"].ToString(), out _))
            {
                this.IsApplicantRevoke = bool.Parse(dr["IsApplicantRevoke"].ToString());
            }

            if (dr["LeadWorkdays"] != DBNull.Value && int.TryParse(dr["LeadWorkdays"].ToString(), out _))
            {
                this.LeadWorkdays = int.Parse(dr["LeadWorkdays"].ToString());
            }

            if (dr["DeadlineOfAcrossMonth"] != DBNull.Value && int.TryParse(dr["DeadlineOfAcrossMonth"].ToString(), out _))
            {
                this.DeadlineOfAcrossMonth = int.Parse(dr["DeadlineOfAcrossMonth"].ToString());
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
        /// 擷取 AIMS 機隊、飛航組員職級與飛航組員任務互換申請系統。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT   ACT.ACTYPE, ACT.ICAOCODE AS FleetCode
                              ,VP.ID AS CrewPos, VP.Code AS CrewPosCode
                              ,Config.*
                     FROM     (SELECT ICAOCODE, LISTAGG(ACTYPE, ',') WITHIN GROUP (ORDER BY ACTYPE) AS AcType FROM fzdb.ci_vvairctype WHERE IATACODE NOT IN ('AB6', 'E90', 'ERD', 'APQ', 'AT7','100') GROUP BY ICAOCODE) ACT
                              CROSS JOIN (SELECT ID, CODE FROM fzdb.ci_vvpositions WHERE CK_CB = 0 AND CODE NOT IN ('OE', 'STF')) VP
                              LEFT JOIN fzdb.fztfcdsconfig Config ON ACT.ACTYPE = Config.IDAcType AND VP.ID = Config.IDCrewPos";
            //FROM     (SELECT ACTYPE, IATACODE FROM fzdb.ci_airctype WHERE IATACODE NOT IN ('AB6', 'E90', 'ERD', 'APQ', 'AT7')) ACT
        }

        private DataTable FetchByIDFcdsConfig()
        {
            string sql = BuildFetchCommandString() + @" WHERE IDFcdsConfig = :pIDFcdsConfig";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsConfig", this.IDFcdsConfig) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public static object FetchAll(ReturnObjectTypeEnum rot)
        {
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, BuildFetchCommandString()).Tables[0];
            switch (rot)
            {
                case ReturnObjectTypeEnum.Collection:
                    FcdsConfigCollection col = new FcdsConfigCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        FcdsConfig obj = new FcdsConfig();
                        obj.SetFcdsConfig(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

        public static DataTable FetchAimsConfigAndFCDS()
        {
            string sql = BuildFetchCommandString() + @" ORDER BY ACT.ICAOCODE, VP.Id";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];
            return dt;
        }

        public DataTable FetchByIDAcTypeAndIDCrewPos()
        {
            string sql = BuildFetchCommandString() + @" WHERE ACT.ACTYPE = :pIDAcType AND VP.Id = :pIDCrewPos";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDAcType", this.IDAcType), new OracleParameter("pIDCrewPos", this.IDCrewPos) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 取得所屬機隊部門之上級單位編碼。
        /// </summary>
        /// <param name="moduleName">模組名稱</param>
        /// <param name="formName">表單名稱</param>
        /// <param name="proID">流程編號</param>
        /// <param name="enumRoleType">表單角色類型列舉型態</param>
        /// <returns></returns>
        public string GetCPUperUT(string moduleName, string formName, string proID, RoleTypeEnum enumRoleType)
        {
            ModuleFormRoleRightTeamCollection col = (ModuleFormRoleRightTeamCollection)ModuleFormRoleRightTeam.FetchFormRoleRightTeam(moduleName, formName, this.IDFcdsConfig, proID, enumRoleType, ReturnObjectTypeEnum.Collection);
            var result = col.Where(o => o.RoleCode.Equals(@"OC_CP_ACP")).GroupBy(o => o.UperUt);

            return result.First().Key;
        }

        public DataTable FetchLog()
        {
            string sql = @"SELECT IDFcdsConfig, IDAcType, IDCrewPos, BranchID, Version, FleetDepCode, NumOfMonth, IsOneReqATime, IsApplicantRevoke, LeadWorkdays, DeadlineOfAcrossMonth, CreateBy, CreateStamp, UpdateBy, UpdateStamp
                           FROM   fzdb.fztfcdsconfig_log
                           WHERE  IDFcdsConfig = :pIDFcdsConfig
                           ORDER BY BranchID";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsConfig", this.IDFcdsConfig) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public bool IsSafeUpdate(DateTime updateStamp)
        {
            string sql = BuildFetchCommandString() + @" WHERE IDFcdsConfig = :pIDFcdsConfig";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsConfig", this.IDFcdsConfig) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];
            if (dt.Rows.Count > 0 && DateTime.TryParse(dt.Rows[0]["UpdateStamp"].ToString(), out _))
            {
                if (updateStamp >= DateTime.Parse(dt.Rows[0]["UpdateStamp"].ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 儲存機隊職級設定。
        /// </summary>
        /// <param name="enumPageMode">頁面模式列舉型態</param>
        /// <param name="isSaveLog">是否儲存日誌</param>
        /// <returns></returns>
        public bool Save(PageMode.PageModeEnum enumPageMode, bool isSaveLog)
        {
            bool result = false;
            switch (enumPageMode)
            {
                case PageMode.PageModeEnum.Create:
                    result = Insert();
                    break;

                case PageMode.PageModeEnum.Edit:
                case PageMode.PageModeEnum.Task:
                    if (isSaveLog)
                    {
                        this.BranchID = long.Parse(Document.FetchNextDocNo("ID_Branch", Document.DocNoStatusEnum.Reserve));
                        this.Version = Branch.CalcNextVersion(this.Version, Branch.BranchTypeEnum.Iteration);
                    }
                    result = Update();
                    break;

                default:
                    break;
            }

            if (result && isSaveLog)
            {
                result = SaveLog();
            }

            return result;
        }

        /// <summary>
        /// 新增機隊職級設定。
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            if (string.IsNullOrEmpty(this.IDFcdsConfig))
            {
                this.IDFcdsConfig = Document.FetchNextDocNo("ID_FcdsConfig", Document.DocNoStatusEnum.Reserve);
                this.BranchID = long.Parse(Document.FetchNextDocNo("ID_Branch", Document.DocNoStatusEnum.Reserve));
                this.Version = Component.BLL.Version.GetInitialVersion();
            }

            string sql = @"INSERT INTO fzdb.fztfcdsconfig(IDFcdsConfig, IDAcType, IDCrewPos, BranchID, Version, FleetDepCode, NumOfMonth, IsOneReqATime, IsApplicantRevoke, LeadWorkdays, DeadlineOfAcrossMonth, CreateBy, CreateStamp, UpdateBy, UpdateStamp)
                           VALUES (:pIDFcdsConfig, :pIDAcType, :pIDCrewPos, :pBranchID, :pVersion, :pFleetDepCode, :pNumOfMonth, :pIsOneReqATime, :pIsApplicantRevoke, :pLeadWorkdays, :pDeadlineOfAcrossMonth, :pCreateBy, :pCreateStamp, :pUpdateBy, :pUpdateStamp)";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsConfig", this.IDFcdsConfig), new OracleParameter("pIDAcType", this.IDAcType),
                                                                        new OracleParameter("pIDCrewPos", this.IDCrewPos), new OracleParameter("pBranchID", this.BranchID),
                                                                        new OracleParameter("pVersion", this.Version), new OracleParameter("pFleetDepCode", this.FleetDepCode),
                                                                        new OracleParameter("pNumOfMonth", this.NumOfMonth),
                                                                        new OracleParameter("pIsOneReqATime", this.IsOneReqATime.Value.ToString()), new OracleParameter("pIsApplicantRevoke", this.IsApplicantRevoke.Value.ToString()),
                                                                        new OracleParameter("pLeadWorkdays", this.LeadWorkdays), new OracleParameter("pDeadlineOfAcrossMonth", this.DeadlineOfAcrossMonth),
                                                                        new OracleParameter("pCreateBy", this.CreateBy), new OracleParameter("pCreateStamp", this.CreateStamp),
                                                                        new OracleParameter("pUpdateBy", this.UpdateBy), new OracleParameter("pUpdateStamp", this.UpdateStamp)
                                                                    };
            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 更新機隊職級設定。
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztfcdsconfig
                           SET    BranchID = :pBranchID, Version = :pVersion, FleetDepCode = :pFleetDepCode, NumOfMonth = :pNumOfMonth,
                                  IsOneReqATime = :pIsOneReqATime, IsApplicantRevoke = :pIsApplicantRevoke, LeadWorkdays = :pLeadWorkdays,
                                  DeadlineOfAcrossMonth = :pDeadlineOfAcrossMonth, UpdateBy = :pUpdateBy, UpdateStamp = :pUpdateStamp
                           WHERE  IDFcdsConfig = :pIDFcdsConfig";

            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pBranchID", this.BranchID), new OracleParameter("pVersion", this.Version),
                                                                        new OracleParameter("pFleetDepCode", this.FleetDepCode),
                                                                        new OracleParameter("pNumOfMonth", this.NumOfMonth), new OracleParameter("pIsOneReqATime", this.IsOneReqATime.ToString()),
                                                                        new OracleParameter("pIsApplicantRevoke", this.IsApplicantRevoke.ToString()), new OracleParameter("pLeadWorkdays", this.LeadWorkdays),
                                                                        new OracleParameter("pDeadlineOfAcrossMonth", this.DeadlineOfAcrossMonth), new OracleParameter("pUpdateBy", this.UpdateBy),
                                                                        new OracleParameter("pUpdateStamp", this.UpdateStamp), new OracleParameter("pIDFcdsConfig", this.IDFcdsConfig)
                                                                    };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 儲存機隊職級設定日誌。
        /// </summary>
        /// <returns></returns>
        private bool SaveLog()
        {
            string sql = @"INSERT INTO fzdb.fztfcdsconfig_log
                           SELECT * FROM fzdb.fztfcdsconfig WHERE IDFcdsConfig = :pIDFcdsConfig";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDFcdsConfig", this.IDFcdsConfig) };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }
    }

    /// <summary>
    /// 設定機隊職級設定集合類別。
    /// </summary>
    public class FcdsConfigCollection : List<FcdsConfig>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public FcdsConfigCollection()
        {

        }
    }
}