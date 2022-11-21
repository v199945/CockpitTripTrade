using System.Collections.Generic;
using System.Data;
using System.Web.UI;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;

namespace Library.Component.BLL
{
    /// <summary>
    /// 表單欄位定義類別。
    /// {0}_form_def
    /// </summary>
    public class ModuleFormDefinition
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ModuleFormDefinition));

        private ModuleFormDefinition()
        {

        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT '{0}' AS FormName, IDComponent, ComponentDisplayName, DefaultValue, IsShowHint, Hint, Order_ FROM fzdb.fzt{0}_form_def";
        }

        /// <summary>
        /// 擷取表單欄位定義。
        /// </summary>
        /// <param name="moduleName">模組名稱></param>
        /// <param name="formName">表單名稱</param>
        /// <returns></returns>
        public static DataTable FetchFormDefinition(string moduleName, string formName)
        {
            string sql = string.Format(BuildFetchCommandString() + @" ORDER BY Order_", formName.ToLower());
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql).Tables[0];

            return dt;
        }

        /// <summary>
        /// 依據流程編號擷取表單欄位與權限定義。
        /// </summary>
        /// <param name="moduleName">模組名稱</param>
        /// <param name="formName">表單名稱</param>
        /// <param name="proID">流程編號</param>
        /// <param name="enumPageMode">頁面模式列舉型態</param>
        /// <returns></returns>
        public static DataTable FetchFormDefinitionRight(string moduleName, string formName, string proID, PageMode.PageModeEnum enumPageMode)
        {
            ModuleForm mf = new ModuleForm(moduleName, formName);

            string sql = string.Format(@"SELECT '{0}' AS FormName, T1.IDComponent, T1.ComponentDisplayName, T1.DefaultValue, T1.IsShowHint, T1.Hint, T1.Order_
                                                ,T2.IDPro, T2.IsEnable, T2.IsRequire
                                         FROM   fzdb.fzt{0}_form_def T1
                                                LEFT JOIN (SELECT R.IDComponent, R.IDPro, R.IsEnable, R.IsRequire FROM fzdb.fzt{0}_form_right R WHERE R.IDPro = :pIDPro) T2 ON T1.IDComponent = T2.IDComponent
                                         ORDER BY T1.Order_"
                                       , formName.ToLower()
                                      );
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDPro", proID) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            if (mf.UserControlList != null && mf.UserControlList.Count > 0)
            {
                foreach (string uc in mf.UserControlList)
                {
                    DataTable dtUserContronl = FetchFormDefinitionRight(moduleName, uc, proID, enumPageMode);
                    dt.Merge(dtUserContronl);
                }
            }

            return dt;
        }
    }
}