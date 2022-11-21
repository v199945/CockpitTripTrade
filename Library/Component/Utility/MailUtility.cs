using System;
using System.Collections;
using System.Net.Mail;

using log4net;

using Library.Component.BLL;

namespace Library.Component.Utility
{
    /// <summary>
    /// 電子郵件輔助密封類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class MailUtility
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MailUtility));
        private const string FROM = @"FCDS_System@china-airlines.com";

        #region Property
        /// <summary>
        /// 電子郵件主旨。
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 電子郵件內文。
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 是否使用安全通訊端層 (SSL) 來加密連線。
        /// </summary>
        public bool IsEnableSsl { get; set; } = false;

        /// <summary>
        /// 電子郵件收件者集合物件。
        /// </summary>
        public MailAddressCollection To { get; set; } = new MailAddressCollection();//MailToHashTable

        /// <summary>
        /// 電子郵件副本抄送集合物件。
        /// </summary>
        public MailAddressCollection Cc { get; set; } = new MailAddressCollection();

        /// <summary>
        /// 電子郵件密件副本集合物件。
        /// </summary>
        public MailAddressCollection Bcc { get; set; } = new MailAddressCollection();

        /// <summary>
        /// 內文是否為 HTML 格式。
        /// </summary>
        public bool IsBodyHtml { get; set; }

        //public AttachmentCollection Attachments { get; set; };

        /// <summary>
        /// SMTP 伺服器位址。
        /// </summary>
        public string SmtpServer { get; } = Config.SmtpServer;
        #endregion

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public MailUtility()
        {

        }

        public MailUtility(string subject, string body, bool? isBodyHtml)
        {
            if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body) && isBodyHtml.HasValue)
            {
                this.Subject = subject;
                this.Body = body;
                this.IsBodyHtml = isBodyHtml.Value;
            }
        }

        /// <summary>
        /// 寄發電子郵件。
        /// </summary>
        /// <returns></returns>
        public void SendMail()
        {
            MailMessage mm = new MailMessage() { Subject = this.Subject, Body = this.Body, BodyEncoding = System.Text.Encoding.UTF8, From = new MailAddress(FROM), IsBodyHtml = this.IsBodyHtml };

            foreach (MailAddress to in this.To)
            {
                mm.To.Add(to);
            }

            if (this.Cc != null)
            {
                foreach (MailAddress cc in this.Cc)
                {
                    mm.CC.Add(cc);
                }
            }

            if (this.Bcc != null)
            {
                foreach (MailAddress bcc in this.Bcc)
                {
                    mm.Bcc.Add(bcc);
                }
            }

            //foreach (Attachment a in this.Attachments)
            //{
            //    mm.Attachments.Add(a);
            //}

            // SMTP 伺服器 Intranet 未支援 SSL 安全連接  { EnableSsl = true }
            SmtpClient sc = new SmtpClient(this.SmtpServer);
            try
            {
                sc.Send(mm);
                logger.Info(@"MailUtility.SendMail() Successfully");
            }
            catch (Exception ex)
            {
                logger.Error(@"MailUtility.SendMail() Exception: " + ex.ToString());
                throw;
            }
        }

        public override string ToString()
        {
            return @"MailUtility [Subject=" + this.Subject+ @", From=" + FROM + @", To=, IsBodyHtml=" + this.IsBodyHtml.ToString() + @", SmtpServer=" + this.SmtpServer + @"]";
        }
    }

    public class MailToHashTable
    {
        public Hashtable MailTo { get; }

        public void Add(string email)
        {
            MailTo.Add(MailTo.Count, email);
        }

        public void Clear()
        {
            MailTo.Clear();
        }
    }

    public class MailCcHashTable
    {
        public Hashtable MailCc { get; }

        public void Add(string email)
        {
            MailCc.Add(MailCc.Count, email);
        }

        public void Clear()
        {
            MailCc.Clear();
        }
    }

    public class MailBccHashTable
    {
        public Hashtable MailBcc { get; }

        public void Add(string email)
        {
            MailBcc.Add(MailBcc.Count, email);
        }

        public void Clear()
        {
            MailBcc.Clear();
        }
    }
}