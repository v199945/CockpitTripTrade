using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using log4net;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS;
using Library.Module.FCDS.Application;

namespace CockpitTripTradeAdmin.Module.Schedule
{
    public partial class CockpitTripTradeAdmin_Module_Schedule_FcdsApply_Auto : BasePage
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CockpitTripTradeAdmin_Module_Schedule_FcdsApply_Auto));
        private bool isAuto = false;
        private bool isClose = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            isAuto = !string.IsNullOrEmpty(Request["Auto"]) && bool.TryParse(Request["Auto"], out _) ? bool.Parse(Request["Auto"]) : false;
            isClose = !string.IsNullOrEmpty(Request["Auto"]) && bool.TryParse(Request["Close"], out _) ? bool.Parse(Request["Close"]) : false;

            if (isAuto)
            {
                bool? result = null;

                // 擷取逾期之申請單
                FcdsApplyCollection col = FcdsApply.FetchExpiry(ReturnObjectTypeEnum.Collection) as FcdsApplyCollection;
                foreach (FcdsApply obj in col)
                {
                    if (obj.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.VOID)
                    {
                        result = obj.VoidForm(@"SysAdm");
                    }

                    SaveLog(@"FcdsApply_Auto - Auto", obj, result);
                    result = null;
                }
            }

            if (isClose)
            {
                this.Close();
            }
        }

        protected void btnCheckStatus_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.IDFcdsApply.Text))
            {
                FcdsApply obj = new FcdsApply(this.IDFcdsApply.Text);
                if (string.IsNullOrEmpty(obj.IDFcdsApply))
                {
                    this.lblFcdsApplyInfo.Text = @"Cannot find this Form!";
                }
                else
                {
                    this.lblFcdsApplyInfo.Text = @"StatusCode=" + obj.StatusCode + @", Deadline=" + obj.ApplicationDeadline.Value.ToString("yyyy/MM/dd");
                }
            }
        }

        protected void btnVoidFcdsApply_Click(object sender, EventArgs e)
        {
            bool? result = null;

            if (!string.IsNullOrEmpty(this.IDFcdsApply.Text))
            {
                if (string.IsNullOrEmpty(this.lblFcdsApplyInfo.Text))
                {
                    this.Alert(@"請先點擊[Check Status]按鈕確認表單狀態與到期日！");
                    return;
                }

                FcdsApply obj = new FcdsApply(this.IDFcdsApply.Text);
                if (obj != null && obj.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.VOID)
                {
                    result = obj.VoidForm(@"SysAdm");
                    Response.Write(@"FcdsApply_Auto.btnVoidFcdsApply_Click=" + result.Value);
                }

                SaveLog(@"FcdsApply_Auto.btnVoidFcdsApply_Click", obj, result);
            }
        }

        private bool SaveLog(string logSubject, FcdsApply obj, bool? result)
        {
            string logDetail = @"IDFcdsApply=" + obj.IDFcdsApply + @", StatusCode=" + obj.StatusCode + @", UpdateBy=" + obj.UpdateBy + @", UpdateStamp=" + obj.UpdateStamp;

            if (result.HasValue)
            {

            }
            else
            {
                logDetail += @"因 StatusCode 已為 VOID，未執行逾期作廢";
            }

            Log log = Log.GetLogWithLoginSuccessfully(logSubject, logDetail, result.Value);
            log.Save(PageMode.PageModeEnum.Create);

            logger.Info(@"logSubject=" + logSubject + @", logDetail=" + logDetail + @", result=" + result.Value.ToString());

            return true;
        }

        protected void btnTestSendMail_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Email.Text))
            {
                MailAddressCollection to = new MailAddressCollection();
                string subject = null;
                string body = @"<p>This is a FCDS system TEST Email to check SMTP Server.</p>";

                subject = @"***TEST MAIL [FCDS System]***";
                to.Add(this.Email.Text);

                MailUtility mu = new MailUtility() { Subject = subject, Body = body, To = to, Cc = null, Bcc = null, IsBodyHtml = true };
                bool result = false;
                Exception ex = null;
                try
                {
                    mu.SendMail();
                    result = true;
                }
                catch (Exception _ex)
                {
                    ex = _ex;
                }
                finally
                {
                    string logDetail = @"Trigger Test Email function of FcdsApply_Auto.aspx page by " + LoginSession.GetLoginSession().UserID;

                    if (ex != null)
                    {
                        logDetail += @", Exception=" + ex.ToString();
                    }

                    Log log = Log.GetLogWithLoginSuccessfully(@"FcdsApply_Auto.TestEmail", logDetail, result);
                    log.Save(PageMode.PageModeEnum.Create);

                    logger.Info(@"FcdsApply_Auto.TestEmail, " + logDetail + @", result=" + result.ToString());

                    Response.Write(@"FcdsApply_Auto.TestEmail=, " + result.ToString());
                }
            }
        }
    }
}