using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Module.FCDS;
using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.UserControl
{
    public partial class UserControl_ChangeHistory : BaseUserControl
    {
        /// <summary>
        /// 模組名稱。
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 表單名稱。
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 表單物件編號，即為表單號。
        /// </summary>
        public string IDObject { get; set; }

        /// <summary>
        /// 表單物件，暫未使用。
        /// </summary>
        public object Object { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                switch (this.BasePageModeEnum)
                {
                    case PageMode.PageModeEnum.Create:
                        break;

                    case PageMode.PageModeEnum.Edit:
                    default:
                        break;
                }

                BindList();

            }
        }

        public void BindList()
        {
            this.gvChangeHistory.DataSource = Library.Component.BLL.Version.FetObjectLog(this.FormName, this.IDObject, ReturnObjectTypeEnum.DataTable);//this.Object.Versions();
            this.gvChangeHistory.DataBind();
        }

        protected void gvChangeHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            DataRowView row = (DataRowView)e.Row.DataItem;
            if (row != null)
            {
                //HyperLink hl = (HyperLink) e.Row.Cells[1].Controls[0];
                //if (hl != null)
                //{
                //    hl.NavigateUrl = Request.RawUrl.Remove(Request.RawUrl.IndexOf("?")).Replace(".aspx", "_Log.aspx") + @"?ID" + this.FormName + @"=" + this.IDObject + @"&BranchID=" + row["BranchID"].ToString() + @"&ModuleName=" + this.ModuleName + @"&FormName=" + this.FormName;
                //}

                Label lblUpdateBy = (Label) e.Row.FindControl("UpdateBy");
                if (lblUpdateBy != null)
                {
                    HrVEgEmploy emp = new HrVEgEmploy(row["UpdateBy"].ToString());
                    lblUpdateBy.Text = emp.DisplayName;
                }
            }
        }

        protected void gvChangeHistory_PreRender(object sender, EventArgs e)
        {
            this.gvChangeHistory.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}