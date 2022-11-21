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
    /// 系統文件類別。
    /// Primary Key：IDDocument
    /// </summary>
    public class Document
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Document));

        #region Property
        //系統文件編號。
        public int? IDDocument { get; set; }

        //系統表單編號類型值。
        public string DocType { get; set; }

        /// <summary>
        /// 表單編號模版。
        /// </summary>
        public string DocNoTemplate { get; set; }

        /// <summary>
        /// 表單編號最後號數。
        /// </summary>
        public string LastNo { get; set; }

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
        #endregion

        public enum DocNoStatusEnum
        {
            Reserve,
            Open
        }

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public Document()
        {

        }

        public Document(string doctype)
        {
            if (doctype == null)
            {

            }
            else
            {
                this.DocType = doctype;
                Load();
            }
        }

        private void Load()
        {
            DataTable dt = FetchByDocType();
            if (dt.Rows.Count == 0)
            {
                this.IDDocument = null;
            }
            else
            {
                SetDocument(dt.Rows[0]);
            }
        }

        private void SetDocument(DataRow dr)
        {
            this.IDDocument = (int)dr["IDDocument"];
            this.DocType = dr["DocType"].ToString();
            this.DocNoTemplate = dr["DocNoTemplate"].ToString();
            
            if (dr["LastNo"] != DBNull.Value)
            {
                this.LastNo = dr["LastNo"].ToString();
            }

            this.CreateBy = dr["CreateBy"].ToString();

            if (dr["CreateStamp"] != DBNull.Value && DateTime.TryParse(dr["CreateStamp"].ToString(), out _))
            {
                this.CreateStamp = DateTime.Parse(dr["CreateStamp"].ToString());
            }
            else
            {
                this.CreateStamp = null;
            }

            this.UpdateBy = dr["UpdateBy"].ToString();

            if (dr["UpdateStamp"] != DBNull.Value && DateTime.TryParse(dr["UpdateStamp"].ToString(), out _))
            {
                this.UpdateStamp = DateTime.Parse(dr["UpdateStamp"].ToString());
            }
            else
            {
                this.UpdateStamp = null;
            }    
        }

        /// <summary>
        /// 建構 SQL 擷取指令字串。
        /// </summary>
        /// <returns></returns>
        private static string BuildFetchCommandString()
        {
            return @"SELECT IDDocument, DocType, DocnoTemplate, LastNo, CreateBy, CreateStamp, UpdateBy, UpdateStamp FROM fzdb.fztdocument";
        }

        public DataTable FetchByDocType()
        {
            string sql = BuildFetchCommandString() + @" WHERE DocType = :pDocType";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pDocType", this.DocType) };
            DataTable dt = OracleHelper.ExecuteDataset(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray()).Tables[0];

            return dt;
        }

        public bool Save()
        {
            bool result = false;
            if (this.IDDocument == null)
            {

            }
            else
            {

            }

            return result;
        }

        private bool Update()
        {
            string sql = @"UPDATE fzdb.fztdocument
                           SET    DocType       = :pDocType,
                                  DocNoTemplate = :pDocNoTemplate,
                                  LastNo        = :pLastNo,
                                  UpdateBy      = 'SystemAdmin',
                                  UpdateStamp   = SYSDATE
                           WHERE IDDocument  = :pIDDocument";
            List<OracleParameter> ops = new List<OracleParameter>() { new OracleParameter("pDocType", this.DocType), new OracleParameter("pDocNoTemplate", this.DocNoTemplate),
                                                                      new OracleParameter("pLastNo", this.LastNo), new OracleParameter("pIDDocument", this.IDDocument)
            };

            int result = OracleHelper.ExecuteNonQuery(Config.FZDBConnectionString, CommandType.Text, sql, ops.ToArray());
            return result > 0;
        }

        /// <summary>
        /// 擷取表單下個表單編號值。
        /// </summary>
        /// <param name="doctype"></param>
        /// <param name="enumDocNoStatus"></param>
        /// <returns></returns>
        public static string FetchNextDocNo(string doctype, DocNoStatusEnum enumDocNoStatus)
        {
            Document d = new Document(doctype);
            string nextdocno = null;
            if (d.IDDocument != null)
            {
                nextdocno = d.DocNoTemplate;
                nextdocno = nextdocno.Replace("%Y", DateTime.Today.ToString("yyyy"));
                nextdocno = nextdocno.Replace("%M", DateTime.Today.ToString("MM"));

                int serialno = nextdocno.IndexOf("%S", StringComparison.OrdinalIgnoreCase);
                if (serialno >= 0)
                {
                    if (int.TryParse(nextdocno.Substring(serialno + 2), out int digitlength))
                    {
                        string nextno;
                        string pattern = nextdocno.Replace("%S" + digitlength.ToString(), "");
                        if (d.LastNo != null && d.LastNo.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            // 同一月份，流水號遞增
                            string lastno = d.LastNo;
                            nextno = (Convert.ToInt32(lastno.Replace(pattern, "")) + 1).ToString().PadLeft(digitlength, '0');
                        }
                        else
                        {
                            // 不同月份或[LastNo]為 NULL 值，流水號重新開始
                            nextno = (1).ToString().PadLeft(digitlength, '0');
                        }

                        nextdocno = nextdocno.Replace("%S" + digitlength, nextno);
                        d.LastNo = nextdocno;
                    }
                }
            }

            switch (enumDocNoStatus)
            {
                case DocNoStatusEnum.Reserve:
                    int i = 0;
                    while (!d.Update() && i < 5)
                    {
                        logger.Error(@"第 " + (i + 1) + @" 次, DocType=[" + d.DocType + @"]取號失敗, LastNo=[" + d.LastNo + "]");
                        i++;
                    }
                    break;

                case DocNoStatusEnum.Open:
                    break;

                default:
                    break;
            }

            return nextdocno;
        }
    }
}