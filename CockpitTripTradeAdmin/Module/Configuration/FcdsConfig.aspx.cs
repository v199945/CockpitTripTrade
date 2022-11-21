using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS;
using Library.Module.FCDS.Configuration;
using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.Module.Configuration
{
    public partial class CockpitTripTradeAdmin_Module_Configuration_FcdsConfig : ConfigurationPage
    {
        private ModuleForm moduleForm;
        private FcdsConfig obj;
        private string proID;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            this.BaseFormName = @"FcdsConfig";
            moduleForm = new ModuleForm(this.BaseModuleName, this.BaseFormName);
            //this.BaseFormTitle += moduleForm.FormTitle;

            string idAcType = Page.Request.QueryString["IDActype"];
            int? idCrewPos = null;
            if (int.TryParse(Page.Request.QueryString["IDCrewPos"], out _))
            {
                idCrewPos = int.Parse(Page.Request.QueryString["IDCrewPos"]);
            }
            obj = new FcdsConfig(idAcType, idCrewPos.Value);

            if (string.IsNullOrEmpty(obj.AcType) || obj.CrewPos == 0)
            {
                string script = @"<script type=""text/javascript"">alert('Invalid Fleet or Rank!'); window.location='../ErrorHandler/NotFoundPage.aspx';</script>";
                Response.Write(script);
                Response.End();
            }

            if (LoginSession.HasEditAuthority(this.Page, moduleForm.IDBllModuleForm))
            {
                if (!string.IsNullOrEmpty(idAcType) && idCrewPos.HasValue)
                {
                    if (!string.IsNullOrEmpty(obj.IDAcType) && obj.IDCrewPos.HasValue)
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
                    this.Redirect(@"../ErrorHandler/Exception.aspx");
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
                this.BaseFormTitle = moduleForm.FormTitle.Insert(2, obj.CrewPosCode).Insert(0, obj.FleetCode);// + @" " + obj.CrewPosCode + moduleForm.FormTitle;
                //this.BaseFormTitle = obj.FleetCode + this.BaseFormTitle.Insert(2, obj.CrewPosCode);

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
            BindForm(true);
            SetForm();
        }

        /// <summary>
        /// 繫結表單。
        /// </summary>
        /// <param name="isAppendDeadlineOfAcrossMonthItems">是否附加[跨月班期限](DeadlineOfAcrossMonth)下拉式選單項目。</param>
        private void BindForm(bool isAppendDeadlineOfAcrossMonthItems)
        {
            this.IDFcdsConfig.Text = obj.IDFcdsConfig;

            this.IDAcType.Text = obj.FleetCode;
            this.hfIDAcType.Value = obj.AcType;

            this.IDCrewPos.Text = obj.CrewPosCode;
            this.hfIDCrewPos.Value = obj.CrewPos.ToString();

            /*
              // 0510 為航務處航務管理部
              // 2021/06/01 原航務處航務管理部轄下之五個空勤機隊之三級單位 (B744/B777/B738/A330/A350)，自2021-06-01 調整為航務處五個二級單位，另新增二級單位 "A321 機隊"，
                            共計6個二級單位歸 OY 陳淳德協理督導；而航務管理部僅保留"行政組"及"管理組"兩個三級單位。
            */
            this.FleetDepCode.DataSource = FcdsHelper.FetchFleetDep(obj.GetCPUperUT(this.BaseModuleName, this.BaseFormName, this.proID, RoleTypeEnum.Notify), true);// HrVPbUnitCd.FetchByUperUt("0510", true);
            this.FleetDepCode.DataBind();
            this.FleetDepCode.SelectedValue = obj.FleetDepCode;
            if (!string.IsNullOrEmpty(obj.FleetDepCode))
            {
                ListItem li = this.FleetDepCode.Items.FindByValue(obj.FleetDepCode);
                if (li == null)
                {
                    this.FleetDepCode.Items[0].Text = @"組織異動，請重新選擇！";
                }
            }

            if (obj.NumOfMonth.HasValue)
            {
                this.NumOfMonth.Text = obj.NumOfMonth.Value.ToString();
            }

            this.IsOneReqATime.SelectedValue = obj.IsOneReqATime.ToString();
            this.IsApplicantRevoke.SelectedValue = obj.IsApplicantRevoke.ToString();

            if (obj.LeadWorkdays.HasValue)
            {
                this.LeadWorkdays.Text = obj.LeadWorkdays.Value.ToString();
            }

            if (isAppendDeadlineOfAcrossMonthItems)
            {
                for (int i = 1; i <= 30; i++)
                {
                    this.DeadlineOfAcrossMonth.Items.Add(new ListItem(i.ToString("D2"), i.ToString()));
                }
            }

            if (obj.DeadlineOfAcrossMonth.HasValue)
            {
                this.DeadlineOfAcrossMonth.SelectedValue = obj.DeadlineOfAcrossMonth.Value.ToString();
            }
        }

        private void SetForm()
        {
            this.FleetDepCode.Attributes.Add(@"OnChange", this.BusyBox1.ShowFunctionCall);
            this.btnSave.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
            //this.IsOneReqATime.Attributes.Add("style", "font-size:0.875rem");
            // " + this.FleetDepCode.ClientID + @".onchange='Show_ContentPlaceHolder1_BusyBox1(); setTimeout('__doPostBack(\'ctl00$ContentPlaceHolder1$FleetDepCode\',\'\')', 0)';
            //this.btnSave.Attributes.Add("OnClick", "if (Page_ClientValidate()) {if (confirm('Are you sure to save?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else{return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators); Page_BlockSubmit=false; return false;};");

            LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, this.btnCheckSave);
            LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, this.btnSave);
        }

        private void BindUserControls()
        {
            this.FlowTeam1.BasePageModeEnum = this.BasePageModeEnum;
            this.FlowTeam1.RoleTypeEnum = RoleTypeEnum.Flow;
            this.FlowTeam1.ModuleName = this.BaseModuleName;
            this.FlowTeam1.FormName = this.BaseFormName;
            this.FlowTeam1.IDFlowTeam = obj.IDFcdsConfig;
            this.FlowTeam1.ProID = this.proID;

            this.NotifyTeam1.BasePageModeEnum = this.BasePageModeEnum;
            this.NotifyTeam1.RoleTypeEnum = RoleTypeEnum.Notify;
            this.NotifyTeam1.ModuleName = this.BaseModuleName;
            this.NotifyTeam1.FormName = this.BaseFormName;
            this.NotifyTeam1.IDFlowTeam = obj.IDFcdsConfig;
            this.NotifyTeam1.ProID = this.proID;
            this.NotifyTeam1.DepID = this.FleetDepCode.SelectedValue;

            this.ChangeHistory1.BasePageModeEnum = this.BasePageModeEnum;
            this.ChangeHistory1.ModuleName = this.BaseModuleName;
            this.ChangeHistory1.FormName = this.BaseFormName;
            this.ChangeHistory1.IDObject = obj.IDFcdsConfig;
            this.ChangeHistory1.Object = obj;
        }

        protected void FleetDepCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.NotifyTeam1.RoleTypeEnum = RoleTypeEnum.Notify;
            this.NotifyTeam1.ModuleName = this.BaseModuleName;
            this.NotifyTeam1.FormName = this.BaseFormName;
            this.NotifyTeam1.IDFlowTeam = obj.IDFcdsConfig;
            this.NotifyTeam1.ProID = this.proID;
            this.NotifyTeam1.DepID = this.FleetDepCode.SelectedValue;
            this.NotifyTeam1.ReBindFlowTeamList();

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsPassValidation() && SaveObject(true))
            {
                BindForm(false);
                this.ChangeHistory1.IDObject = obj.IDFcdsConfig;
                this.ChangeHistory1.BindList();
                BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
                
                this.BootstrapAlerts(this.divBootstrapAlert, @"<strong>Save Complete!</strong> 您所做的修改已於 " + obj.UpdateStamp.Value.ToString("yyyy/MM/dd HH:mm:ss") + @" 儲存成功！", true, BootstrapAlertsTypeEnum.Success);
                //this.Redirect(@"FcdsConfig_List.aspx");
            }
            else
            {
                this.BootstrapAlerts(this.divBootstrapAlert, @"<strong>Save Failed!</strong> 您所做的修改儲存失敗，請再試一次！", true, BootstrapAlertsTypeEnum.Danger);
                return;
            }
        }
        /// <summary>
        /// 檢查IDFcdsConfig必填與基本資料格式
        /// </summary>
        /// <returns></returns>
        private bool IsPassValidation()
        {
            string alert = null;

            if (this.BasePageModeEnum != PageMode.PageModeEnum.Create && string.IsNullOrEmpty(this.IDFcdsConfig.Text))
            {
                alert += @"[" + this.lblIDFcdsConfig.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.IDAcType.Text))
            {
                alert += @"[" + this.lblIDAcType.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.IDCrewPos.Text))
            {
                alert += @"[" + this.IDCrewPos.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.FleetDepCode.SelectedValue))
            {
                alert += @"[" + this.lblFleetDepCode.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.NumOfMonth.Text))
            {
                alert += @"[" + this.lblNumOfMonth.Text + @"] is required!\n";
            }
            else
            {
                if (int.TryParse(this.NumOfMonth.Text, out int nom))
                {
                    if (nom < 1)
                    {
                        alert += @"[" + this.lblNumOfMonth.Text + @"] must be greater than or equal to 1!\n";
                    }
                    if (nom > 9)
                    {
                        alert += @"[" + this.lblNumOfMonth.Text + @"] must be less than or equal to 9!\n";
                    }
                }
                else
                {
                    alert += @"[" + this.lblNumOfMonth.Text + @"] must be number!\n";
                }
            }

            if (string.IsNullOrEmpty(this.IsOneReqATime.SelectedValue))
            {
                alert += @"[" + this.lblIsOneReqATime.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.IsApplicantRevoke.SelectedValue))
            {
                alert += @"[" + this.lblIsApplicantRevoke.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.LeadWorkdays.Text))
            {
                alert += @"[" + this.lblLeadWorkdays.Text + @"] is required!\n";
            }
            else
            {
                if (int.TryParse(this.LeadWorkdays.Text, out int lw))
                {
                    if (lw < 1)
                    {
                        alert += @"[" + this.lblLeadWorkdays.Text + @"] must be greater than or equal to 1!\n";
                    }

                    if (lw > 9)
                    {
                        alert += @"[" + this.lblLeadWorkdays.Text + @"] must be less than or equal to 9!\n";
                    }
                }
            }

            if (string.IsNullOrEmpty(this.DeadlineOfAcrossMonth.Text))
            {
                alert += @"[" + this.lblDeadlineOfAcrossMonth.Text + @"] is required!\n";
            }
            else
            {
                if (int.TryParse(this.DeadlineOfAcrossMonth.Text, out int doam))
                {
                    if (doam < 1)
                    {
                        alert += @"[" + this.lblDeadlineOfAcrossMonth.Text + @"] must be greater than or equal to 0!\n";
                    }

                    if (doam > 30)
                    {
                        alert += @"[" + this.lblDeadlineOfAcrossMonth.Text + @"] must be less than or equal to 30!\n";
                    }
                }
                else
                {
                    alert += @"[" + this.lblDeadlineOfAcrossMonth.Text + @"] must be a number!\n";
                }
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

            this.FlowTeam1.ModuleName = this.BaseModuleName;
            this.FlowTeam1.FormName = this.BaseFormName;
            this.FlowTeam1.IDFlowTeam = obj.IDFcdsConfig;
            this.FlowTeam1.BranchID = obj.BranchID.Value;
            if (!this.FlowTeam1.SaveTeam(isSaveLog))
            {
                return false;
            }

            this.NotifyTeam1.ModuleName = this.BaseModuleName;
            this.NotifyTeam1.FormName = this.BaseFormName;
            this.NotifyTeam1.IDFlowTeam = obj.IDFcdsConfig;
            this.NotifyTeam1.BranchID = obj.BranchID.Value;
            if (!this.NotifyTeam1.SaveTeam(isSaveLog))
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
            obj.IDAcType = this.hfIDAcType.Value;

            if (int.TryParse(this.hfIDCrewPos.Value, out _))
            {
                obj.IDCrewPos = int.Parse(this.hfIDCrewPos.Value);
            }

            obj.FleetDepCode = this.FleetDepCode.SelectedValue;

            if (int.TryParse(this.NumOfMonth.Text, out _))
            {
                obj.NumOfMonth = int.Parse(this.NumOfMonth.Text);
            }

            obj.IsOneReqATime = bool.Parse(this.IsOneReqATime.SelectedValue);
            obj.IsApplicantRevoke = bool.Parse(this.IsApplicantRevoke.SelectedValue);

            if (int.TryParse(this.LeadWorkdays.Text, out _))
            {
                obj.LeadWorkdays = int.Parse(this.LeadWorkdays.Text);
            }

            if (int.TryParse(this.DeadlineOfAcrossMonth.Text, out _))
            {
                obj.DeadlineOfAcrossMonth = int.Parse(this.DeadlineOfAcrossMonth.Text);
            }

            obj.CreateBy = this.LoginSession.UserID;
            obj.CreateStamp = DateTime.Now;
            obj.UpdateBy = this.LoginSession.UserID;
            obj.UpdateStamp = DateTime.Now;
        }
    }
}