using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.Enums;
using Library.Component.DAL;

namespace Library.Component.BLL
{
    /// <summary>
    /// 流程狀態類別。
    /// </summary>
    public class FlowStatus
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FlowStatus));

        #region Property
        /// <summary>
        /// 模組名稱。
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 表單名稱。
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 流程狀態編號。
        /// </summary>
        public int IDStatus { get; set; }

        /// <summary>
        /// 流程狀態代碼。
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 流程狀態中午名稱。
        /// </summary>
        public string StatusCName { get; set; }

        /// <summary>
        /// 流程狀態英文名稱。
        /// </summary>
        public string StatusEName { get; set; }

        /// <summary>
        /// 流程狀態顯示。
        /// </summary>
        public string DisplayStatus { get; set; }

        /// <summary>
        /// 流程編號。
        /// </summary>
        public string ProID { get; set; }

        /// <summary>
        /// 下一流程編號。
        /// </summary>
        public string NextProID { get; set; }

        /// <summary>
        /// 流程狀態順序。
        /// </summary>
        public int Order_ { get; set; }

        public enum StatusCodeEnum
        {
            /// <summary>
            /// 表單新建立狀態代碼。
            /// </summary>
            INITIAL = 1,

            /// <summary>
            /// 表單結案/已發行狀態代碼。
            /// </summary>
            RELEASED = 99
        }
        #endregion

        private FlowStatus()
        {

        }

        //public FlowStatus(string moduleName, string formName, string proID)
        //{
        //    if (!string.IsNullOrEmpty(moduleName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(proID))
        //    {
        //        this.ModuleName = moduleName;
        //        this.FormName = FormName;
        //        this.ProID = proID;
        //    }
        //}

        public FlowStatus(string moduleName, string formName, string statusCode)
        {
            if (!string.IsNullOrEmpty(moduleName) && !string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(statusCode))
            {
                this.ModuleName = moduleName;
                this.FormName = formName;
                this.StatusCode = statusCode;

                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByStatusCode();
            if (dt.Rows.Count > 0)
            {
                SetFlowStatus(dt.Rows[0]);
            }
        }

        private void SetFlowStatus(DataRow dr)
        {
            if (dr["IDStatus"] != DBNull.Value && int.TryParse(dr["IDStatus"].ToString(), out _))
            {
                this.IDStatus = int.Parse(dr["IDStatus"].ToString());
            }

            this.StatusCode = dr["StatusCode"].ToString();
            this.StatusCName = dr["StatusCName"].ToString();
            this.StatusEName = dr["StatusEName"].ToString();
            this.DisplayStatus = dr["DisplayStatus"].ToString();
            this.ProID = dr["ProID"].ToString();
            this.NextProID = dr["NextProID"].ToString();

            if (dr["Order_"] != DBNull.Value && int.TryParse(dr["Order_"].ToString(), out _))
            {
                this.Order_ = int.Parse(dr["Order_"].ToString());
            }
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildeFetchCommandString()
        {
            return @"SELECT '{0}' AS FormName, FS.IDStatus, FS.StatusCode, FS.StatusCName, FS.StatusEName, FS.DisplayStatus, FS.Order_, FS.ProID, FS.NextProID FROM fzdb.fzt{0}_flow_status FS";
        }

        private DataTable FetchByProID()
        {
            string sql = string.Format(BuildeFetchCommandString() + @" WHERE FS.ProID = :pProID" , this.FormName.ToLower());
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pProID", this.ProID) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        private DataTable FetchByStatusCode()
        {
            string sql = string.Format(BuildeFetchCommandString() + @" WHERE FS.StatusCode = :pStatusCode", this.FormName.ToLower());
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pStatusCode", this.StatusCode) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public static FlowStatusCollection FetchAllFlowStatus(string moduleName, string formName)
        {
            string sql = string.Format(BuildeFetchCommandString() + @" ORDER BY FS.Order_" , formName.ToLower());
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

            FlowStatusCollection col = new FlowStatusCollection();
            foreach (DataRow dr in dt.Rows)
            {
                FlowStatus obj = new FlowStatus() { ModuleName = moduleName, FormName = formName };
                obj.SetFlowStatus(dr);

                col.Add(obj);
            }

            return col;
        }

        /// <summary>
        /// 依據流程編號(ProID)擷取表單之 FlowStatus 物件。
        /// </summary>
        /// <param name="moduleName">模組名稱</param>
        /// <param name="formName">表單名稱</param>
        /// <param name="proID">流程編號</param>
        /// <returns></returns>
        public static FlowStatus FetchByProID(string moduleName, string formName, string proID)
        {
            string sql = string.Format(BuildeFetchCommandString() + @" WHERE FS.ProID = :pProID ORDER BY FS.Order_" , formName.ToLower());
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pProID", proID) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            if (dt != null && dt.Rows.Count > 0)
            {
                FlowStatus obj = new FlowStatus();
                obj.SetFlowStatus(dt.Rows[0]);

                return obj;
            }

            return null;
        }

        /// <summary>
        /// 依顯示狀態名稱[DisplayStatus]欄位群組化且排除 INITIAL 狀態代碼，擷取表單流程狀態。
        /// 20220921 648267=>修改return datatable為中文狀態+英文狀態，修改DB欄位值
        /// </summary>
        /// <returns></returns>
        public static DataTable FetchGroupByDisplayStatus()
        {
            string sql = @"SELECT FS.DisplayStatus, LISTAGG(FS.Order_, ',') WITHIN GROUP (ORDER BY FS.Order_) AS StatusOrders, LISTAGG(FS.StatusCode, ',') WITHIN GROUP (ORDER BY FS.Idstatus) AS StatusCodes FROM fzdb.FzTFcdsApply_Flow_Status FS WHERE FS.Statuscode <> 'INITIAL' GROUP BY FS.DisplayStatus ORDER BY StatusOrders";
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

            return dt;
        }
    }

    /// <summary>
    /// 流程狀態集合類別。
    /// </summary>
    public class FlowStatusCollection : List<FlowStatus>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public FlowStatusCollection()
        {

        }
    }
}
