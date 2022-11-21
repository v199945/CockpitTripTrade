using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using log4net;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS.Application;
using Library.Module.FCDS.Configuration;
using Library.Module.FZDB;

namespace CockpitTripTrade.Module.Application.UserControl
{
    public partial class CockpitTripTrade_Module_Application_UserControl_FcdsApplyApprove : ApplicationUserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CockpitTripTrade_Module_Application_UserControl_FcdsApplyApprove));

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
                BindForm();
            }

            this.SetControlBusyBox(new List<WebControl>() { this.IsApproval });
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //[審核意見]下拉式選單選擇[任務受影響，擬不予調整](false)時，則[不同意原因]必填
            if (!string.IsNullOrEmpty(this.IsApproval.SelectedValue) && bool.TryParse(this.IsApproval.SelectedValue, out _))
            {
                SetRejectReason(!Boolean.Parse(this.IsApproval.SelectedValue));
            }
        }

        private void InitForm()
        {
            this.RejectReason.DataSource = FcdsRejectReason.FetchAllValid(ReturnObjectTypeEnum.Collection);
            this.RejectReason.DataBind();
        }

        private void BindForm()
        {
            if (FcdsApplyApprove == null) FcdsApplyApprove = new FcdsApplyApprove();

            if (FcdsApplyApprove.IsApproval.HasValue)
            {
                this.IsApproval.SelectedValue = FcdsApplyApprove.IsApproval.ToString();
            }

            this.RejectReason.SelectedValue = FcdsApplyApprove.IDFcdsRejectReason;
            this.Comments.Text = FcdsApplyApprove.Comments;
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
            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox.HideFunctionCall);
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

            FcdsApplyApprove.CreateBy = this.LoginSession.UserID;
            FcdsApplyApprove.CreateStamp = DateTime.Now;
            FcdsApplyApprove.UpdateBy = this.LoginSession.UserID;
            FcdsApplyApprove.UpdateStamp = DateTime.Now;
        }
    }
}