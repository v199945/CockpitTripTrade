using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS.Configuration;

namespace CockpitTripTradeAdmin.Module.Configuration
{
    public partial class CockpitTripTradeAdmin_Module_Configuration_FcdsRejectReason : ConfigurationPage
    {
        private ModuleForm moduleForm;
        private FcdsRejectReason obj;
        private string proID;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            this.BaseFormName = @"FcdsRejectReason";
            moduleForm = new ModuleForm(this.BaseModuleName, this.BaseFormName);
            this.BaseFormTitle = moduleForm.FormTitle;

            string idFcdsRejectReason = Page.Request.QueryString["IDFcdsRejectReason"];
            obj = new FcdsRejectReason(idFcdsRejectReason);

            if (!string.IsNullOrEmpty(idFcdsRejectReason) && string.IsNullOrEmpty(obj.IDFcdsRejectReason))
            {
                string script = @"<script type=""text/javascript"">alert('Invalid Link!'); window.location='../ErrorHandler/NotFoundPage.aspx';</script>";
                Response.Write(script);
                Response.End();
            }

            if (LoginSession.HasEditAuthority(this.Page, moduleForm.IDBllModuleForm))
            {
                if (!string.IsNullOrEmpty(idFcdsRejectReason))
                {
                    this.BasePageModeEnum = PageMode.PageModeEnum.Edit;
                }
                else
                {
                    this.BasePageModeEnum = PageMode.PageModeEnum.Create;
                }
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

            switch (this.BasePageModeEnum)
            {
                case PageMode.PageModeEnum.Create:
                    proID = moduleForm.InitialProID;
                    break;

                case PageMode.PageModeEnum.Edit:
                    proID = moduleForm.InitialProID;
                    break;

                default:
                    proID = moduleForm.InitialProID;
                    break;
            }

            if (!Page.IsPostBack)
            {
                InitForm();
            }

            BindUserControls();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ModuleFormRight.SetModuleFormRight(this.Page, this.BaseModuleName, this.BaseFormName, this.proID, this.BasePageModeEnum);
        }

        private void InitForm()
        {
            BindForm();
            SetForm();
        }

        private void BindForm()
        {
            this.IDFcdsRejectReason.Text = obj.IDFcdsRejectReason;
            this.RejectReason.Text = obj.RejectReason;

            if (obj.IsValidFlag.HasValue)
            {
                this.IsValidFlag.SelectedValue = obj.IsValidFlag.ToString();
            }
        }

        private void SetForm()
        {
            this.btnSave.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
            //this.btnSave.Attributes.Add("OnClick", "if (Page_ClientValidate()) {if (confirm('Are you sure to save?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else {return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");

            LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, this.btnCheckSave);
            LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, this.btnSave);
        }

        private void BindUserControls()
        {
            this.ChangeHistory1.BasePageModeEnum = this.BasePageModeEnum;
            this.ChangeHistory1.ModuleName = this.BaseModuleName;
            this.ChangeHistory1.FormName = this.BaseFormName;
            this.ChangeHistory1.IDObject = obj.IDFcdsRejectReason;
            this.ChangeHistory1.Object = obj;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsPassValidation() && SaveObject(true))
            {
                BindForm();
                this.ChangeHistory1.IDObject = obj.IDFcdsRejectReason;
                this.ChangeHistory1.BindList();
                BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);

                this.BootstrapAlerts(this.divBootstrapAlert, @"<strong>Save Complete!</strong> 您所做的修改已於 " + obj.UpdateStamp.Value.ToString("yyyy/MM/dd HH:mm:ss") + @" 儲存成功！", true, BootstrapAlertsTypeEnum.Success);
                //this.Redirect(@"FcdsRejectReason_List.aspx");
            }
            else
            {
                this.BootstrapAlerts(this.divBootstrapAlert, @"<strong>Save Failed!</strong> 您所做的修改儲存失敗，請再試一次！", true, BootstrapAlertsTypeEnum.Danger);
                return;
            }
        }

        private bool IsPassValidation()
        {
            string alert = null;

            if (this.BasePageModeEnum != PageMode.PageModeEnum.Create && string.IsNullOrEmpty(this.IDFcdsRejectReason.Text))
            {
                alert += @"[" + this.lblIDFcdsRejectReason.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.RejectReason.Text))
            {
                alert += @"[" + this.lblRejectReason.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.IsValidFlag.SelectedValue))
            {
                alert += @"[" + this.lblIsValidFlag.Text + @"] is required!\n";
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

        private bool SaveObject(bool isSaveLog)
        {
            SetObjValue();
            if (!obj.Save(this.BasePageModeEnum, isSaveLog))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 設定表單物件屬性值。
        /// </summary>
        private void SetObjValue()
        {
            obj.RejectReason = this.RejectReason.Text;
            obj.IsValidFlag = bool.Parse(this.IsValidFlag.SelectedValue);

            obj.CreateBy = this.LoginSession.UserID;
            obj.CreateStamp = DateTime.Now;
            obj.UpdateBy = this.LoginSession.UserID;
            obj.UpdateStamp = DateTime.Now;
        }
    }
}