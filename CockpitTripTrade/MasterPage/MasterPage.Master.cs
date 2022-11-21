using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Module.HRDB;

namespace CockpitTripTrade.MasterPage
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        LoginSession ls;

        protected void Page_Load(object sender, EventArgs e)
        {
            ls = LoginSession.GetLoginSession();

            if (!Page.IsPostBack)
            {
                InitForm();
            }
        }

        private void InitForm()
        {
            BindForm();
            SetForm();
        }

        private void BindForm()
        {
            LoginSession ls = LoginSession.GetLoginSession();
            if (ls != null)
            {
                this.lblUserID.Text = ls.UserID;
                this.lblUserName.Text = ls.Employee.EnglishName;

                this.ModalUserID.Text = ls.UserID;
                this.ModalUserName.Text = ls.Employee.EnglishName;
                this.ModalDepName.Text = ls.Employee.DisplayDepName;

                string userrole = string.Empty;
                foreach (UserGroup gu in ls.UserGroups)
                {
                    userrole += gu.GroupName + @"<br />";
                }
                this.ModalUserRole.Text = userrole;
            }
        }

        private void SetForm()
        {
        }

    }
}