using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;

namespace Library.Component.BLL
{
    /// <summary>
    /// 版本編號類別。
    /// </summary>
    public class Branch
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Branch));

        #region Property
        /// <summary>
        /// 版本值 ID。
        /// </summary>
        public long? IDBranch { get; set; }

        /// <summary>
        /// 版本號。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 版號。
        /// </summary>
        public string RevisionCode { get; set; }

        /// <summary>
        /// 版序。
        /// </summary>
        public int IterationID { get; set; }
        #endregion

        /// <summary>
        /// 版本值遞增類型列舉型態。
        /// </summary>
        public enum BranchTypeEnum
        {
            /// <summary>
            /// 無遞增。
            /// </summary>
            None,

            /// <summary>
            /// 版號遞增。
            /// </summary>
            Revision,

            /// <summary>
            /// 版序遞增。
            /// </summary>
            Iteration
        }

        private Branch()
        {

        }

        private Branch(int id)
        {
            this.IDBranch = id;

            Load();
        }

        private void Load()
        {
            DataTable dt = FetchByIDBranch();
            if (dt.Rows.Count == 0)
            {
                this.IDBranch = null;
            }
            else
            {
                SetBranch(dt.Rows[0]);
            }
        }

        private void SetBranch(DataRow dr)
        {
            this.IDBranch = (Int64)dr["IDBranch"];
            this.Version = dr["Version"].ToString();
            this.RevisionCode = dr["RevisionCode"].ToString();
            this.IterationID = (int)dr["IterationID"];
        }

        private static string BuildFetchCommandString()
        {
            return @"SELECT idbranch, version, revisioncode, iterationid FROM fzdb.fztbranch";
        }

        public DataTable FetchByIDBranch()
        {
            string sql = BuildFetchCommandString() + @" WHERE idbranch = :pIDBranch";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pIDBranch", this.IDBranch) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        /// <summary>
        /// 計算下一個版本號。
        /// </summary>
        /// <param name="currentVersion">目前版本號</param>
        /// <param name="enumBranchType">版本值遞增類型列舉型態</param>
        /// <returns></returns>
        public static string CalcNextVersion(string currentVersion, BranchTypeEnum enumBranchType)
        {
            string nextversion = null;
            string[] version = currentVersion.Split('.');

            switch (enumBranchType)
            {
                case BranchTypeEnum.Revision:
                    nextversion = CalcNextRevision(version[0]) + @".1";
                    break;

                case BranchTypeEnum.Iteration:
                    nextversion = version[0] + @"." + (Convert.ToInt32(version[1]) + 1);
                    break;

                default:
                    break;
            }

            return nextversion;
        }

        /// <summary>
        /// 計算下一個版序。
        /// </summary>
        /// <param name="currentRevision">目前版序</param>
        /// <remarks>
        /// AB
        /// Z   => AA
        /// AZ  => BA
        /// BA  => BB
        /// ZZ  => AAA
        /// AAZ => ABA
        /// AZZ => BAA
        /// </remarks>
        /// <returns></returns>
        public static string CalcNextRevision(string currentRevision)
        {
            string nextrevision;
            char[] codes = currentRevision.ToCharArray();

            if (currentRevision.Substring(currentRevision.Length - 1, 1).IndexOf("Z", StringComparison.Ordinal) >= 0)
            {
                // 若 currentRevision 結尾為 Z 須進位

                bool isaddcode = false;
                Array.Reverse(codes);
                codes[0] = 'A';

                if (codes.Length == 1) isaddcode = true;

                for (int i = 1; i < codes.Length; i++)
                {
                    if (codes[i].ToString() == "Z")
                    {
                        codes[i] = 'A';

                        if (i == codes.Length - 1) isaddcode = true;
                    }
                    else
                    {
                        codes[i] = Convert.ToChar((int)codes[i] + 1);
                        break;
                    }
                }

                Array.Reverse(codes);
                nextrevision = new string(codes);
                if (isaddcode) nextrevision = @"A" + nextrevision;
            }
            else
            {
                // 若 currentRevision 結尾不為 Z，則取最後一碼前面字元加最後一碼遞增
                nextrevision = currentRevision.Substring(0, currentRevision.Length - 1) + Convert.ToChar((int)(codes[codes.Length - 1]) + 1).ToString();
            }

            return nextrevision;
        }
    }
}