using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS.Admin;
using Library.Module.FZDB;
using Library.Module.HRDB;

namespace CockpitTripTrade.Module.Admin
{
    public partial class CockpitTripTrade_Module_Admin_ImpersonateUser : AdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.BaseFormTitle = @"Impersonate Crew";

            if (!Page.IsPostBack)
            {
                LoginSession ls = LoginSession.GetLoginSession();
                if(ls != null && ls.IsSucceed )
                {
                    InitForm();

                }
                else
                {
                    Response.Redirect(@"~/Login.aspx");
                }
                
            }

            this.Impersonate.Focus();
        }

        private void InitForm()
        {
            SetForm();
        }

        private void SetForm()
        {
            this.Impersonate.Attributes.Add("OnInput", @"javascript:ValidatorOnChange(event); RequiredFieldValidator_CheckValidControl('" + this.rfvImpersonate.ClientID + @"', '" + this.Impersonate.ClientID + @"');");

            this.btnImpersonate.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
            //this.btnImpersonate.Attributes.Add("OnClick", "if (Page_ClientValidate()) {if (confirm('Are you sure to Impersonate this crew?')) {" + this.BusyBox1.ShowFunctionCall + @" return true;} else {return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");            
            this.btnCancel.Attributes.Add("OnClick", "if (confirm('Are you sure to exit this page?')) {return true;} else{return false;}");
        }

        protected void btnImpersonate_Click(object sender, EventArgs e)
        {
            if (IsPassValidation())
            {
                SaveImpersonateLog(this.LoginSession.UserID + @" impersonate " + this.Impersonate.Text + @" crew", true);

                LoginSession.SetLoginSession(Session["State"].ToString(), this.Impersonate.Text, WebApplicationEnum.CockpitTripTrade);
                LoginSession ls = LoginSession.GetLoginSession();
                if (ls != null && ls.IsSucceed) // 模擬飛航組員登入成功
                {
                    SaveImpersonateLog(this.LoginSession.UserID + @" impersonate " + this.Impersonate.Text + @" crew successfully", true);
                    this.Redirect(@"../Application/TaskList.aspx");
                }
                else
                {
                    this.Redirect("../ErrorHandler/UnauthorizedPage.aspx");
                }
            }

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        private bool IsPassValidation()
        {
            string alert = null;

            if (string.IsNullOrEmpty(this.Impersonate.Text.Trim()))
            {
                alert += @"[" + this.lblImpersonate.Text + @"] is required！\n";
            }

            HrVEgEmploy employ = new HrVEgEmploy(this.Impersonate.Text.Trim());
            CIvvCrewDb crewDb = new CIvvCrewDb(this.Impersonate.Text.Trim());
            if (this.Impersonate.Text.Trim().Length != 6 || !int.TryParse(this.Impersonate.Text.Trim(), out _) ||
                string.IsNullOrEmpty(employ.EmployID) || employ.ExstFlg == "N" || employ.AnalySa != "100" ||
                string.IsNullOrEmpty(crewDb.ID) || crewDb.CIvvRosterId.ActTypEnum != CIvvRosterIdActTypEnum.Active)
            {
                alert += @"[" + this.lblImpersonate.Text + @"] is invalid！\n";
            }

            if (string.IsNullOrEmpty(alert))
            {
                return true;
            }
            else
            {
                this.Alert(alert);
                return false;
            }
        }

        private void SaveImpersonateLog(string logDetail, bool result)
        {
            Log log = Log.GetLogWithLoginSuccessfully(@"Impersonate Crew", logDetail, result);
            log.Save(PageMode.PageModeEnum.Create);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Redirect(@"~/Main/Default.aspx");
        }
    }
}