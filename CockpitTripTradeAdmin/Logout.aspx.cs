using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;

namespace CockpitTripTradeAdmin
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitForm();
            }
        }

        private void InitForm()
        {
            SetForm();
        }

        private void SetForm()
        {
            this.lbtnLogout.Attributes.Add(@"OnClick", "if (confirm('Are you sure to logout this system?')) {return true;} else{return false;}");
        }

        protected void lbtnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();            
            Response.Redirect(Config.LogoutOIDCUri); // 登出 OIDC
        }
    }
}