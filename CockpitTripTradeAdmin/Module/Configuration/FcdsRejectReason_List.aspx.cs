using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Module.FCDS.Configuration;

namespace CockpitTripTradeAdmin.Module.Configuration
{
    public partial class CockpitTripTradeAdmin_Module_Configuration_FcdsRejectReason_List : ConfigurationPage
    {
        private ModuleForm moduleForm;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.BaseFormTitle = @"不同意原因設定清單";
            moduleForm = new ModuleForm(this.BaseModuleName, @"FcdsRejectReason");

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

            if (!this.IsPostBack)
            {
                InitForm();
                BindList();
            }
        }

        private void InitForm()
        {
            SetForm();
        }

        private void SetForm()
        {
            LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, this.btnNew);
        }

        private void BindList()
        {
            DataTable dt = FcdsRejectReason.FetchAll(ReturnObjectTypeEnum.DataTable) as DataTable;
            this.gvList.DataSource = dt;
            this.gvList.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            this.Redirect(@"FcdsRejectReason.aspx");
        }

        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            GridViewRow gvr = e.Row;
            HyperLink hl = (HyperLink)gvr.FindControl("Control");
            if (hl != null)
            {
                if (this.BasePageModeEnum == PageMode.PageModeEnum.Create || this.BasePageModeEnum == PageMode.PageModeEnum.Edit)
                {
                    hl.Text = @"Edit";
                }
                else
                {
                    hl.Text = @"View";
                }
            }
        }

        protected void gvList_PreRender(object sender, EventArgs e)
        {
            this.gvList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}