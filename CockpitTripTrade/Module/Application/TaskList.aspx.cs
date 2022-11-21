using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Library.Component.Utility;
using Library.Module.FCDS;
using Library.Module.FCDS.Application;
using Library.Module.FCDS.Configuration;
using Library.Module.FZDB;

namespace CockpitTripTrade.Module.Application
{
    /// <summary>
    /// 飛航組員任務換班申請系統前台組員登入成功頁面。
    /// </summary>
    public partial class CockpitTripTrade_Module_Application_TaskList : ApplicationPage
    {
        /// <summary>
        /// 飛航組員任務換班申請系統之機隊職級設定物件。
        /// </summary>
        private FcdsConfig fcdsConfig;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.BaseFormTitle = @"Task List";

            string ac = this.LoginSession.CrewDb.GetGroupByAcID();
            int pos = this.LoginSession.CrewDb.GetGroupByPosID();
            fcdsConfig = new FcdsConfig(ac, pos);

            if (!Page.IsPostBack)
            {
                // 檢查是否有維護職位、機隊
                if (string.IsNullOrEmpty(fcdsConfig.IDFcdsConfig))
                {
                    CIvvAircType aircType = new CIvvAircType(ac);
                    CIvvPositions positions = new CIvvPositions(pos);
                    this.Alert(@"★★ " + aircType.IcaoCode + @" fleet and "+ positions.Code + " rank has not configured! ★★");
                }

                FcdsApplyCollection col = FcdsApply.FetchByApplicantIDOrRespondentID(this.LoginSession.CrewDb.ID, Library.Component.Enums.ReturnObjectTypeEnum.Collection) as FcdsApplyCollection;
                InitForm(col);
                BindRepeater(col);
            }
        }

        private void InitForm(FcdsApplyCollection fcdsApplies)
        {
            string currentMonth = DateTime.Now.ToString("yyyyMM");
            string nextMonth = DateTime.Now.AddMonths(1).ToString("yyyyMM");

            HtmlAnchor tabCurrentMonth = ControlUtility.FindControlRecursive(this.Page.Master, @"tab_currentmonth") as HtmlAnchor;
            HtmlAnchor tabNextMonth = ControlUtility.FindControlRecursive(this.Page.Master, @"tab_nextmonth") as HtmlAnchor;
            if (tabCurrentMonth != null && tabNextMonth != null)
            {
                tabCurrentMonth.InnerText = DateTime.Now.ToString("MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                tabNextMonth.InnerText = DateTime.Now.AddMonths(1).ToString("MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            }

            // 20221103 648267: Remain = Applied - ( withdraw + rejected )
            int currentMonthUsed = fcdsApplies.Where(o => o.ApplicantID == this.LoginSession.CrewDb.ID && o.SwapScheduleMonth == DateTime.Now.ToString("yyyyMM")).Count();
            int currentMonthReturned = fcdsApplies.Where(o => o.ApplicantID == this.LoginSession.CrewDb.ID && 
                                                              o.SwapScheduleMonth == DateTime.Now.ToString("yyyyMM") && 
                                                              (o.StatusCode == "REVOKE" || o.StatusCode == "RESPONDENT_RETURN" || (o.StatusCode == "RELEASED" && o.IsApproval == false))
                                                         ).Count();
            this.lblCurrentMonthRemaining.Text = this.fcdsConfig.NumOfMonth == null ? @"N/A" : (this.fcdsConfig.NumOfMonth - currentMonthUsed + currentMonthReturned).ToString();

            this.lblCurrentMonthUsed.Text = (currentMonthUsed- currentMonthReturned).ToString();

            List<string> vs = FcdsApply.GetAllowCreateString(this.LoginSession.CrewDb.ID, currentMonth, this.fcdsConfig);
            if (vs.Count > 0)
            {
                // 若判斷飛航組員是否可建立申請單方法回傳之字串集合項目數大於零，則不啟用新增本月申請單之 ASP.NET 連結控制項[hlNewCurrentMonthFcdsApply]
                this.hlNewCurrentMonthFcdsApply.CssClass += @" disabled";
                this.hlNewCurrentMonthFcdsApply.Attributes.Add(@"OnClick", @"alert('" + string.Join(@"\n", vs.ToArray()) + @"')");
                this.hlNewCurrentMonthFcdsApply.Enabled = false;
                this.hlNewCurrentMonthFcdsApply.ToolTip = string.Join(@"&#10;", vs.ToArray());// &#13;
            }
            else
            {
                this.hlNewCurrentMonthFcdsApply.NavigateUrl = @"~/Module/Application/NewFcdsApply.aspx?SwapMonth=" + currentMonth;
            }

            // 20221103 648267: Remain = Quota - Applied + Returned 
            int nextMonthUsage = fcdsApplies.Where(o => o.ApplicantID == this.LoginSession.CrewDb.ID && o.SwapScheduleMonth == DateTime.Now.AddMonths(1).ToString("yyyyMM")).Count();
            int nextMonthReturned = fcdsApplies.Where(o => o.ApplicantID == this.LoginSession.CrewDb.ID &&
                                                           o.SwapScheduleMonth == DateTime.Now.AddMonths(1).ToString("yyyyMM") &&
                                                           (o.StatusCode == "REVOKE" || o.StatusCode == "RESPONDENT_RETURN" || (o.StatusCode == "RELEASED" && o.IsApproval == false))
                                                     ).Count();
            this.lblNextMonthRemaining.Text = (this.fcdsConfig.NumOfMonth - nextMonthUsage + nextMonthReturned).ToString();
            this.lblNextMonthUsed.Text = (nextMonthUsage-nextMonthReturned).ToString();

            vs = FcdsApply.GetAllowCreateString(this.LoginSession.CrewDb.ID, nextMonth, this.fcdsConfig);
            if (this.LoginSession.CrewDb.CIvvRosterId.PublishDate < DateTimeUtility.GetTheBeginOfMonth(DateTime.Now.AddMonths(1)))
            {
                // 飛航組員發佈班表最後日期小於次月一日
                vs.Add(@"The roster of next month has not been published!");
            }

            if (vs.Count > 0)
            {
                // 若判斷飛航組員是否可建立申請單方法回傳之字串集合項目數大於零，則不啟用新增次月申請單之 ASP.NET 連結控制項[hlNewNextMonthFcdsApply]
                this.hlNewNextMonthFcdsApply.CssClass += @" disabled";
                this.hlNewNextMonthFcdsApply.Attributes.Add(@"OnClick", @"alert('" + string.Join(@"\n", vs.ToArray()) + @"')");
                this.hlNewNextMonthFcdsApply.Enabled = false;
                this.hlNewNextMonthFcdsApply.ToolTip = string.Join(@"&#10;", vs.ToArray());// &#13;
            }
            else
            {
                this.hlNewNextMonthFcdsApply.NavigateUrl = @"~/Module/Application/NewFcdsApply.aspx?SwapMonth=" + nextMonth;
            }

            this.lblInProcess.Text = fcdsApplies.Where(o => o.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.RELEASED && o.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.REVOKE && o.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.RESPONDENT_RETURN).Count().ToString();
        }

        private void BindRepeater(FcdsApplyCollection fcdsApplies)
        {
            this.repInProcess.DataSource = fcdsApplies.Where(o => o.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.RELEASED && o.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.REVOKE && o.StatusCodeEnum != FcdsHelper.FcdsApplyStatusCodeEnum.RESPONDENT_RETURN).ToList();
            this.repInProcess.DataBind();
        }

        protected void repInProcess_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FcdsApply item = e.Item.DataItem as FcdsApply;

                HyperLink hlIDFcdsApply = e.Item.FindControl("hlIDFcdsApply") as HyperLink;
                if (hlIDFcdsApply != null)
                {
                    if (item.StatusCodeEnum == FcdsHelper.FcdsApplyStatusCodeEnum.RESPONDENT && item.ApplicantID == this.LoginSession.CrewDb.ID)
                    {
                        hlIDFcdsApply.NavigateUrl = @"~/Module/Application/FcdsApply.aspx?IDFcdsApply=" + item.IDFcdsApply + @"&ProID=" + FcdsHelper.PRO_REVOKE;
                    }
                    else
                    {
                        hlIDFcdsApply.NavigateUrl = @"~/Module/Application/FcdsApply.aspx?IDFcdsApply=" + item.IDFcdsApply + (item.TaskOwnerID == this.LoginSession.CrewDb.ID ? @"&ProID=" + item.ProID : string.Empty);
                    }
                }

                Label lblApplicant = e.Item.FindControl("lblApplicant") as Label;
                if (lblApplicant != null)
                {
                    lblApplicant.Text = item.ApplicantID == this.LoginSession.CrewDb.ID ? @"Myself" : new CIvvCrewDb(item.ApplicantID).PassportName;
                }

                Label lblApplicantDate = e.Item.FindControl("lblApplicantDate") as Label;
                if (lblApplicantDate != null && item.ApplicationDate.HasValue)
                {
                    lblApplicantDate.Text = item.ApplicationDate.Value.ToString("ddMMM", CultureInfo.CreateSpecificCulture("en-US"));
                }

                HtmlGenericControl liRespondent = e.Item.FindControl("liRespondent") as HtmlGenericControl;
                HtmlGenericControl divRespondentID = e.Item.FindControl("divRespondentID") as HtmlGenericControl;
                HtmlGenericControl divRespondent = e.Item.FindControl("divRespondent") as HtmlGenericControl;
                HtmlGenericControl liOP = e.Item.FindControl("liOP") as HtmlGenericControl;
                HtmlGenericControl liFinished = e.Item.FindControl("liFinished") as HtmlGenericControl;
                if (liRespondent != null && divRespondentID != null && divRespondent != null && liOP != null && liFinished != null)
                {
                    if (item.RespondentID == this.LoginSession.CrewDb.ID)
                    {
                        divRespondent.InnerText = @"Myself";
                    }
                    else
                    {
                        CIvvCrewDb respondent = new CIvvCrewDb(item.RespondentID);
                        divRespondentID.InnerText = respondent.ID;
                        divRespondent.InnerText = respondent.PassportName;
                    }

                    switch (item.StatusCodeEnum)
                    {
                        case FcdsHelper.FcdsApplyStatusCodeEnum.RESPONDENT:
                            liRespondent.Attributes.Add(@"class", @"scs-stepitem   active");
                            break;

                        case FcdsHelper.FcdsApplyStatusCodeEnum.OP_STAFF:
                        case FcdsHelper.FcdsApplyStatusCodeEnum.OP_MANAGER:
                        case FcdsHelper.FcdsApplyStatusCodeEnum.OP_ASSISTANT_GENERAL_MANAGER:
                        case FcdsHelper.FcdsApplyStatusCodeEnum.OP_GENERAL_MANAGER:
                            liOP.Attributes.Add(@"class", @"scs-stepitem   active");
                            break;

                        case FcdsHelper.FcdsApplyStatusCodeEnum.RELEASED:
                            liFinished.Attributes.Add(@"class", @"scs-stepitem   active");
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}