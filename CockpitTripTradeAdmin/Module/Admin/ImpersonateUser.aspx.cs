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
using Library.Module.FCDS.Application;
using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.Module.Admin
{
    public partial class CockpitTripTradeAdmin_Module_Admin_ImpersonateUser : AdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.BaseFormTitle = @"Impersonate Employee";

            if (!Page.IsPostBack)
            {
                LoginSession ls = LoginSession.GetLoginSession();
                if (ls != null && ls.IsSucceed && ls.IsAdmin)
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
            //this.btnImpersonate.Attributes.Add("OnClick", "if (Page_ClientValidate()) {if (confirm('Are you sure to Impersonate this user?')) {" + this.BusyBox1.ShowFunctionCall + @" return true;} else {return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");            
            this.btnCancel.Attributes.Add("OnClick", "if (confirm('Are you sure to exit this page?')) {return true;} else{return false;}");
        }

        protected void btnImpersonate_Click(object sender, EventArgs e)
        {
            if (IsPassValidation())
            {
                SaveImpersonateLog(this.LoginSession.UserID + @" impersonate " + this.Impersonate.Text + @" employee", true);
                
                LoginSession.SetLoginSession(Session["State"].ToString(), this.Impersonate.Text, WebApplicationEnum.CockpitTripTradeAdmin);
                LoginSession ls = LoginSession.GetLoginSession();
                if (ls != null && ls.IsSucceed)
                {
                    SaveImpersonateLog(this.LoginSession.UserID + @" impersonate " + this.Impersonate.Text + @" employee successfully", true);

                    if (ls.IsAdmin || LoginSession.HasEditAuthority(this.Page, FcdsApply.IDMODULEFORM))
                    {
                        // 模擬航務處組員派遣部同仁登入成功
                        // 20221108 648267:TODO:將頁面轉跳放至CallBack.aspx
                        this.Redirect(@"../Application/ReviewFcdsApply_List.aspx");
                    }
                    else
                    {
                        this.Redirect(@"../Application/FcdsApply_List.aspx");
                    }
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

            if (string.IsNullOrEmpty(this.Impersonate.Text))
            {
                alert += @"[" + this.lblImpersonate.Text + @"] is required！\n";
            }

            HrVEgEmploy employ = new HrVEgEmploy(this.Impersonate.Text.Trim());
            if (this.Impersonate.Text.Length != 6 || !int.TryParse(this.Impersonate.Text, out _) ||
                string.IsNullOrEmpty(employ.EmployID) || employ.ExstFlg == "N")
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
            Log log = Log.GetLogWithLoginSuccessfully(@"Impersonate Employee", logDetail, result);
            log.Save(PageMode.PageModeEnum.Create);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Redirect(@"~/Main/Default.aspx");
        }

    }
}