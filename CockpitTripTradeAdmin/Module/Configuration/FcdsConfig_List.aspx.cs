using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Module.FCDS.Configuration;

namespace CockpitTripTradeAdmin.Module.Configuration
{
    public partial class CockpitTripTradeAdmin_Module_Configuration_FcdsConfig_List : ConfigurationPage
    {
        private ModuleForm moduleForm;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.BaseFormTitle = @"機隊職級設定清單";
            moduleForm = new ModuleForm(this.BaseModuleName, @"FcdsConfig");

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
                BindList();
            }
        }

        private void BindList()
        {
            DataTable dt = FcdsConfig.FetchAimsConfigAndFCDS();
            this.gvList.DataSource = dt;
            this.gvList.DataBind();
        }

        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
            
            GridViewRow gvr = e.Row;
            HyperLink hl = (HyperLink) gvr.FindControl("Control");
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