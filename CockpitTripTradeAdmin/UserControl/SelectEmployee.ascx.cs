using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.UserControl
{
    public partial class UserControl_SelectEmployee : System.Web.UI.UserControl
    {
        public bool Enable
        {
            get
            {
                return this.Employee.Enabled;
            }

            set
            {
                this.Employee.Enabled = value;
            }
        }

        public string EmployeeID
        {
            get
            {
                return this.Employee.SelectedValue;
            }

            set
            {
                this.Employee.SelectedValue = value;
            }
        }

        public object DataSource { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitForm();
            }
        }

        private void InitForm()
        {
            //this.Employee.DataSource = this.DataSource;
            //this.Employee.DataBind();

            //this.EmployeeID
            if (!string.IsNullOrEmpty(this.Employee.SelectedValue))
            {
                HrVEgEmploy e = new HrVEgEmploy(this.Employee.SelectedValue);//this.EmployeeID
                if (e.ExstFlg == "N")
                {
                    ListItem li = this.Employee.Items.FindByValue(e.EmployID);
                    if (li != null)
                    {
                        li.Text += @"[已失效]";
                    }
                }
            }
            //this.Employee.SelectedValue = this.EmployeeID;
        }
    }
}