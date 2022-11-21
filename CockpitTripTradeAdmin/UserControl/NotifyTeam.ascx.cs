using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CockpitTripTrade.App_Code.BLL;
using CockpitTripTrade.App_Code.Enums;

namespace CockpitTripTrade.UserControl
{
    public partial class UserControl_NotifyTeam : System.Web.UI.UserControl
    {
        public string ModuleName { get; set; }

        public string FormName { get; set; }

        public string IDFlowTeam { get; set; }

        public long BranchID { get; set; }

        public string ProID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private void BindNotifyTeamList()
        {
            this.gvNotifyTeam.DataSource = FormRole.FetchFormRoleRightAndTeam(this.ModuleName, this.FormName, this.IDFlowTeam, this.ProID, RoleTypeEnum.Notify, ReturnObjectType.Collection);
            this.gvNotifyTeam.DataBind();
        }

    }
}