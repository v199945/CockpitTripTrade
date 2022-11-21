using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using log4net;
using Oracle.ManagedDataAccess.Client;

using CockpitTripTrade.App_Code.BLL;
using CockpitTripTrade.App_Code.DAL;
using CockpitTripTrade.App_Code.Enums;

namespace CockpitTripTrade.App_Code.Module.Configuration
{
    public class OPEmpConfig
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(OPEmpConfig));

        public int? IDAcType { get; set; }

        public string FleetCode { get; set; }
        
        public int? IDCrewPos { get; set; }

        public string CrewPosCode { get; set; }
        
        public long? BranchID { get; set; }
        
        public string Version { get; set; }
        
        public string OPStaffID { get; set; }

        public string OPDeputyID { get; set; }

        public string CreateBy { get; set; }
        
        public DateTime? CreateStamp { get; set; }
        
        public string UpdateBy { get; set; }
        
        public DateTime? UpdateStamp { get; set; }

        public OPEmpConfig()
        {

        }

        public OPEmpConfig(int? idAcType, int? idCrewPos)
        {
            if (idAcType.HasValue && idCrewPos.HasValue)
            {
                this.IDAcType = idAcType;
                this.IDCrewPos = idCrewPos;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByIDAcTypeAndIDCrewPos();
            //
            if (dt.Rows.Count == 0 || dt.Rows[0]["IDAcType"] == DBNull.Value || dt.Rows[0]["IDCrewPos"] == DBNull.Value)
            {
                this.IDAcType = null;
                this.FleetCode = dt.Rows[0]["FleetCode"].ToString();
                this.IDCrewPos = null;
                this.CrewPosCode = dt.Rows[0]["CrewPosCode"].ToString();
            }
            else
            {
                SetOpEmpConfig(dt.Rows[0]);
            }    
        }

        private void SetOpEmpConfig(DataRow dr)
        {
            if (dr["IDAcType"] != DBNull.Value)
            {
                this.IDAcType = (int)dr["IDAcType"];
            }
            this.FleetCode = dr["FleetCode"].ToString();

            if (dr["IDCrewPos"] != DBNull.Value)
            {
                this.IDCrewPos = (int)dr["IDCrewPos"];
            }
            this.CrewPosCode = dr["CrewPosCode"].ToString();

            if (dr["BranchID"] != DBNull.Value)
            {
                this.BranchID = (long)dr["BranchID"];
            }

            this.Version = dr["Version"].ToString();
            this.OPStaffID = dr["OPStaffID"].ToString();
            this.OPDeputyID = dr["OPDeputyID"].ToString();
            this.CreateBy = dr["CreateBy"].ToString();

            if (dr["CreateStamp"] != DBNull.Value)
            {
                this.CreateStamp = (DateTime)dr["CreateStamp"];
            }

            this.UpdateBy = dr["UpdateBy"].ToString();

            if (dr["UpdateStamp"] != DBNull.Value)
            {
                this.UpdateStamp = (DateTime)dr["UpdateStamp"];
            }
        }

        private static string BuildFetchCommandString()
        {
            return @"SELECT   ACT.ACTYPE, ACT.IATACODE AS FleetCode
                              ,VP.ID AS CrewPos, VP.Code AS CrewPosCode
                              ,OPEmp.*
                     FROM     (SELECT ACTYPE, IATACODE FROM fzdb.ci_airctype WHERE IATACODE NOT IN ('AB6', 'E90', 'ERD', 'APQ', 'AT7')) ACT
                              CROSS JOIN (SELECT ID, CODE FROM fzdb.ci_vvpositions WHERE CK_CB = 0)                                     VP
                              LEFT JOIN fzdb.fztfcsropempconfig  OPEmp ON ACT.ACTYPE = OPEmp.IDAcType AND VP.ID  = OPEmp.IDCrewPos";
        }

        public static object FetAll(ReturnObjectTypeEnum enumReturnObjectType)
        {
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, BuildFetchCommandString()).Tables[0];
            switch (enumReturnObjectType)
            {
                case ReturnObjectTypeEnum.Collection:
                    OPEmpConfigCollection col = new OPEmpConfigCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        OPEmpConfig obj = new OPEmpConfig();
                        obj.SetOpEmpConfig(dr);
                        col.Add(obj);
                    }

                    return col;

                case ReturnObjectTypeEnum.DataTable:
                    return dt;

                default:
                    return dt;
            }
        }

        public static DataTable FetchAimsConfigAndFCSR()
        {
            string sql = BuildFetchCommandString() + @" ORDER BY ACT.ACTYPE, VP.Id";
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

        public bool Save(PageMode.PageModeEnum enumPageMode)
        {
            bool result = false;
            switch (enumPageMode)
            {
                case PageMode.PageModeEnum.Create:
                    result = Insert();
                    break;

                case PageMode.PageModeEnum.Edit:
                    result = Update();
                    break;

                default:
                    break;
            }

            return result;
        }

        private bool Insert()
        {
            int result = 0;

            return result > 0;
        }
        private bool Update()
        {
            int result = 0;

            return result > 0;
        }
    }

    public class OPEmpConfigCollection : List<OPEmpConfig>
    {
        public OPEmpConfigCollection()
        {

        }
    }
}