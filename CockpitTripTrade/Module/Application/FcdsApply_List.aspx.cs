using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS;
using Library.Module.FCDS.Application;
using Library.Module.FZDB;

namespace CockpitTripTrade.Module.Application
{
    public partial class CockpitTripTrade_Module_Application_FcdsApply_List : ApplicationPage
    {
        private ModuleForm moduleForm;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.BaseFormTitle = @"Search application";
            moduleForm = new ModuleForm(this.BaseModuleName, @"FcdsApply");

            if (LoginSession.HasEditAuthority(this.Page, moduleForm.IDBllModuleForm))
            {
                this.BasePageModeEnum = PageMode.PageModeEnum.Edit;
            }
            else
            {
                this.BasePageModeEnum = PageMode.PageModeEnum.View;
            }

            if (!this.IsPostBack)
            {
                InitForm();
                BindList(GetFilterDictionary());
            }            
        }

        private void InitForm()
        {
            this.txtApplicationDatePeriod.Text = DateTimeUtility.GetTheBeginOfMonth(DateTime.Now).ToString("ddMMMyyyy", CultureInfo.CreateSpecificCulture("en-US")) + @" - " + DateTimeUtility.GetTheEndOfMonth(DateTime.Now).ToString("ddMMMyyyy", CultureInfo.CreateSpecificCulture("en-US"));
            //this.txtApplicationBeginDate.Text = DateTimeUtility.GetTheBeginOfMonth(DateTime.Now).ToString("ddMMMyyyy", CultureInfo.CreateSpecificCulture("en-US"));

            this.ddlAcType.DataSource = CIvvAircType.FetchAll(ReturnObjectTypeEnum.Collection);
            this.ddlAcType.DataBind();
            this.ddlAcType.SelectedValue = this.LoginSession.CrewDb.GetGroupByAcID();

            this.ddlCrewPos.DataSource = CIvvPositions.FetchFlightCrewAllPosition(ReturnObjectTypeEnum.Collection);
            this.ddlCrewPos.DataBind();
            this.ddlCrewPos.SelectedValue = this.LoginSession.CrewDb.GetGroupByPosID().ToString();

            this.ddlStatusCode.DataSource = FlowStatus.FetchGroupByDisplayStatus();
            this.ddlStatusCode.DataBind();

            this.btnSearch.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
        }

        private void BindList(Dictionary<string, string> dic)
        {
            var col = (FcdsApply.FetchByApplicantIDOrRespondentID(this.LoginSession.CrewDb.ID, ReturnObjectTypeEnum.Collection) as FcdsApplyCollection).ToList();
            
            if (dic.Count > 0)
            {
                foreach (string key in dic.Keys)
                {
                    if (key == @"ApplicationBeginDate" && DateTime.TryParse(dic[key], out _))
                        col = col.Where(o => o.ApplicationDate >= DateTime.Parse(dic[key])).ToList();
                    else if (key == @"ApplicationEndDate" && DateTime.TryParse(dic[key], out _))
                        col = col.Where(o => o.ApplicationDate <= DateTime.Parse(dic[key])).ToList();
                    else if (key == @"StatusCode")
                    {
                        string[] vs = dic[key].Split(',');
                        var q = from a in col where vs.Contains(a.StatusCode) select a;
                        col = q.ToList();
                    }
                    else if (key == @"IsApproval" && bool.TryParse(dic[key], out _))
                        col = col.Where(o => o.IsApproval == bool.Parse(dic[key])).ToList();
                    else if (key == @"Keyword")
                        col = col.Where(o => o.RespondentID == dic[key]).ToList();
                }
            }

            this.gvList.DataSource = col.OrderByDescending(o => o.IDFcdsApply);
            this.gvList.DataBind();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindList(GetFilterDictionary());

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        private Dictionary<string, string> GetFilterDictionary()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(this.txtApplicationDatePeriod.Text))
            {
                dic.Add(@"ApplicationBeginDate", this.txtApplicationDatePeriod.Text.Substring(0, 9));
                dic.Add(@"ApplicationEndDate", this.txtApplicationDatePeriod.Text.Substring(this.txtApplicationDatePeriod.Text.IndexOf("-") + 2) + @" 23:59:59");
            }

            if (this.ddlStatusCode.SelectedIndex != 0)
            {
                dic.Add(@"StatusCode", this.ddlStatusCode.SelectedValue);
            }

            if (this.ddlIsApproval.SelectedIndex != 0)
            {
                dic.Add(@"IsApproval", this.ddlIsApproval.SelectedValue);
            }

            if (!string.IsNullOrEmpty(this.txtKeyword.Text))
            {
                dic.Add(@"Keyword", this.txtKeyword.Text);
            }

            return dic;
        }

        private const int APPLICATION_DATE_INDEX = 3;
        private const int APPLICATION_DEADLINE_INDEX = 4;
        private const int APPLICANT_INDEX = 5;
        private const int RESPONDENT_INDEX = 6;
        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            GridViewRow gvr = e.Row;
            HyperLink hl = gvr.FindControl("Control") as HyperLink;
            if (hl != null)
            {
            }

            FcdsApply row = e.Row.DataItem as FcdsApply;
            if (row != null)
            {
                e.Row.Cells[APPLICATION_DATE_INDEX].Text = row.ApplicationDate.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
                e.Row.Cells[APPLICATION_DEADLINE_INDEX].Text = row.ApplicationDeadline.Value.ToString("ddMMMyyyy", CultureInfo.CreateSpecificCulture("en-US"));
                e.Row.Cells[APPLICANT_INDEX].Text = row.ApplicantCrew.DisplayName;
                e.Row.Cells[RESPONDENT_INDEX].Text = row.RespondentCrew.DisplayName;

                Label lblIsApproval = gvr.FindControl("IsApproval") as Label;
                if (row.StatusCodeEnum == FcdsHelper.FcdsApplyStatusCodeEnum.RELEASED && lblIsApproval != null && row.IsApproval.HasValue)
                {   // 20221108 648267:Change "Agree" to "Approved"
                    lblIsApproval.Text = row.IsApproval.Value ? @"Approved" : @"Disagree";
                }
            }
        }

        protected void gvList_PreRender(object sender, EventArgs e)
        {
            this.gvList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}