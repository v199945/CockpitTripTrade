using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using log4net;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS.Application;
using Library.Module.FCDS.Configuration;
using Library.Module.FZDB;

namespace CockpitTripTradeAdmin.Module.Application.UserControl
{
    public partial class CockpitTripTradeAdmin_Module_Application_UserControl_FcdsApplyApprove : ApplicationUserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CockpitTripTradeAdmin_Module_Application_UserControl_FcdsApplyApprove));

        /// <summary>
        /// 飛航組員任務換班申請系統申請主表單物件。
        /// </summary>
        public FcdsApply FcdsApply;

        /// <summary>
        /// 飛航組員任務換班申請系統申請主表單組員派遣部審核意見物件。
        /// </summary>
        public FcdsApplyApprove FcdsApplyApprove;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitForm();
            }

            this.SetControlBusyBox(new List<WebControl>() { this.IsApproval });
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //[審核意見]下拉式選單選擇[任務受影響，擬不予調整](false)時，則[不同意原因]必填
            if (!string.IsNullOrEmpty(this.IsApproval.SelectedValue) && bool.TryParse(this.IsApproval.SelectedValue, out _))
            {
                SetRejectReason(!Boolean.Parse(this.IsApproval.SelectedValue) && !(this.BasePageModeEnum == PageMode.PageModeEnum.View));
            }
        }

        private void InitForm()
        {
            BindForm();
        }

        private void BindForm()
        {
            if (FcdsApplyApprove.IsApproval.HasValue)
            {
                this.IsApproval.SelectedValue = FcdsApplyApprove.IsApproval.ToString();
            }

            this.RejectReason.DataSource = FcdsRejectReason.FetchAllValid(ReturnObjectTypeEnum.Collection);
            this.RejectReason.DataBind();
            this.RejectReason.SelectedValue = FcdsApplyApprove.IDFcdsRejectReason;

            this.Comments.Text = FcdsApplyApprove.Comments;

            SetApproveDialogueModal();
        }

        private void SetRejectReason(bool isEnable)
        {
            this.lblMustFillRejectReason.Visible = isEnable;
            this.lblRejectReason.Enabled = isEnable;
            this.RejectReason.Enabled = isEnable;
            this.rfvRejectReason.Enabled = isEnable;

            if (isEnable)
            {
                this.RejectReason.Attributes.Add("OnInput", @"javascript:ValidatorOnChange(event); RequiredFieldValidator_CheckValidControl('" + this.rfvRejectReason.ClientID + @"', '" + this.RejectReason.ClientID + @"');");
            }
            else
            {
                this.RejectReason.Attributes.Remove("OnInput");
            }
        }

        protected void IsApproval_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RejectReason.SelectedIndex = 0;

            SetApproveDialogueModal();

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox.HideFunctionCall);
        }

        private void SetApproveDialogueModal()
        {
            HtmlGenericControl spanApprove = ControlUtility.FindControlRecursive(this.Page, @"spanApprove") as HtmlGenericControl;
            if (spanApprove != null)
            {
                Label lbl = new Label() { ForeColor = Color.Red };
                lbl.Font.Bold = true;
                
                if (bool.TryParse(this.IsApproval.SelectedValue, out _) && bool.Parse(this.IsApproval.SelectedValue))
                {
                    lbl.Text = @"同意";
                }
                else
                {
                    lbl.Text = @"不同意";
                }

                spanApprove.Controls.AddAt(0, lbl);
            }
        }

        public bool SaveApplyApprove(bool isSaveLog)
        {
            SetApplyApprove(isSaveLog);

            if (string.IsNullOrEmpty(FcdsApplyApprove.IDFcdsApplyApprove))
            {
                return FcdsApplyApprove.Save(PageMode.PageModeEnum.Create, isSaveLog);
            }
            else
            {
                return FcdsApplyApprove.Save(this.BasePageModeEnum, isSaveLog);
            }
        }

        private void SetApplyApprove(bool isSaveLog)
        {
            FcdsApplyApprove.IDFcdsApplyApprove = FcdsApplyApprove.IDFcdsApplyApprove;
            FcdsApplyApprove.IDFcdsApply = FcdsApply.IDFcdsApply;
            FcdsApplyApprove.BranchID = FcdsApply.BranchID;
            FcdsApplyApprove.Version = FcdsApply.Version;

            if (bool.TryParse(this.IsApproval.SelectedValue, out _))
            {
                FcdsApplyApprove.IsApproval = bool.Parse(this.IsApproval.SelectedValue);
            }

            FcdsApplyApprove.IDFcdsRejectReason = this.RejectReason.SelectedValue;
            FcdsApplyApprove.Comments = this.Comments.Text;

            FcdsApplyApprove.CreateBy = FcdsApply.UpdateBy;
            FcdsApplyApprove.CreateStamp = FcdsApply.UpdateStamp;
            FcdsApplyApprove.UpdateBy = FcdsApply.UpdateBy;
            FcdsApplyApprove.UpdateStamp = FcdsApply.UpdateStamp;
        }

        public string ValidateUserControl()
        {
            string alert = null;

            if (string.IsNullOrEmpty(this.IsApproval.SelectedValue))
            {
                alert += @"[" + this.lblIsApproval.Text + @"] is required!\n";
            }
            else
            {
                if (bool.TryParse(this.IsApproval.SelectedValue, out _) && !bool.Parse(this.IsApproval.SelectedValue) && string.IsNullOrEmpty(this.RejectReason.SelectedValue))
                {
                    alert += @"[" + this.lblRejectReason.Text + @"] is required!\n";
                }
            }

            return alert;
        }
    }
}