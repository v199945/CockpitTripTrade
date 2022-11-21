using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Module.FCDS.Application;
using Library.Module.FZDB;

namespace CockpitTripTradeAdmin.Module.Application
{
    public partial class CockpitTripTradeAdmin_Module_Application_ReviewFcdsApply_List : ApplicationPage
    {
        private ModuleForm moduleForm;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.BaseFormTitle = @"審核申請單";
            moduleForm = new ModuleForm(this.BaseModuleName, @"FcdsApply");

            if (LoginSession.HasEditAuthority(this.Page, moduleForm.IDBllModuleForm))
            {
                this.BasePageModeEnum = PageMode.PageModeEnum.Edit;
            }
            else
            {
                if (LoginSession.HasViewAuthority(this.Page, moduleForm.IDBllModuleForm))
                {
                    this.BasePageModeEnum = PageMode.PageModeEnum.View;
                }
                else
                {
                    // Redirect to Unauthorized Page.
                    this.Page.Response.Redirect(@"~/Module/ErrorHandler/UnauthorizedPage.aspx");
                }
            }

            BindUserControls();
        }

        private void BindUserControls()
        {
            FcdsApplyCollection col = FcdsApply.FetchToBeReview(ReturnObjectTypeEnum.Collection, LoginSession.IsAdmin) as FcdsApplyCollection;

            this.ucAllFleetTabFcdsApplyList.FcdsApplies = col.OrderBy(o => o.ApplicationDeadline).ToList();
            this.AllFleetCount.InnerText = this.ucAllFleetTabFcdsApplyList.FcdsApplies.Count.ToString();

            this.ucB738TabFcdsApplyList.FcdsApplies = col.Where(o => o.IDAcTypeApplicant.Equals(@"3")).OrderBy(o => o.ApplicationDeadline).ToList();
            this.B738FleetCount.InnerText = this.ucB738TabFcdsApplyList.FcdsApplies.Count.ToString();

            this.ucB744TabFcdsApplyList.FcdsApplies = col.Where(o => o.IDAcTypeApplicant.Equals(@"12,13")).OrderBy(o => o.ApplicationDeadline).ToList();
            this.B744FleetCount.InnerText = this.ucB744TabFcdsApplyList.FcdsApplies.Count.ToString();

            this.ucB777TabFcdsApplyList.FcdsApplies = col.Where(o => o.IDAcTypeApplicant.Equals(@"5,6")).OrderBy(o => o.ApplicationDeadline).ToList();
            this.B777FleetCount.InnerText = this.ucB777TabFcdsApplyList.FcdsApplies.Count.ToString();

            this.ucA21NTabFcdsApplyList.FcdsApplies = col.Where(o => o.IDAcTypeApplicant.Equals(@"7")).OrderBy(o => o.ApplicationDeadline).ToList();
            this.A21NFleetCount.InnerText = this.ucA21NTabFcdsApplyList.FcdsApplies.Count.ToString();

            this.ucA330TabFcdsApplyList.FcdsApplies = col.Where(o => o.IDAcTypeApplicant.Equals(@"4")).OrderBy(o => o.ApplicationDeadline).ToList();
            this.A330FleetCount.InnerText = this.ucA330TabFcdsApplyList.FcdsApplies.Count.ToString();

            this.ucA350TabFcdsApplyList.FcdsApplies = col.Where(o => o.IDAcTypeApplicant.Equals(@"14")).OrderBy(o => o.ApplicationDeadline).ToList();
            this.A350FleetCount.InnerText = this.ucA350TabFcdsApplyList.FcdsApplies.Count.ToString();
        }
    }
}