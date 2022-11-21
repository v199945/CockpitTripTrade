using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.MasterPage
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
                InitMenu();
            }
        }

        private void InitForm()
        {
            BindForm();
            SetForm();
        }

        private void BindForm()
        {
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

        /// <summary>
        /// 初始化選單。
        /// </summary>
        private void InitMenu()
        {
            if (ls != null)
            {
                IterateMenu(this.ulNavMenu.Controls);
            }
        }

        /// <summary>
        /// 遍曆選單控制項，判斷權限。
        /// </summary>
        /// <param name="cc">選單控制項集合物件</param>
        private void IterateMenu(ControlCollection cc)
        {
            foreach (Control c in cc)
            {
                if (c.HasControls())
                {
                    IterateMenu(c.Controls);
                    c.Visible = c.Controls.Cast<Control>().Where(o => o is HtmlGenericControl && o.Visible == true).Count() > 0;
                }

                if (c is HtmlGenericControl)
                {
                    HtmlGenericControl li = c as HtmlGenericControl;
                    if (li != null && !string.IsNullOrEmpty(li.Attributes["IDModuleForm"]))
                    {
                        List<string> moduelFunc = li.Attributes["IDModuleForm"].Split(',').ToList();
                        if (moduelFunc.Count > 1)
                        {
                            li.Visible = ls.UserModuleFormFunctions.Where(o => o.IDBllModuleForm == moduelFunc[0] && o.IDFunction == moduelFunc[1]).Count() > 0;
                        }
                        else
                        {
                            li.Visible = ls.UserModuleFormFunctions.Where(o => o.IDBllModuleForm == moduelFunc[0]).Count() > 0;
                        }
                    }
                }

                //if (c is HyperLink)
                //{
                //    HyperLink hl = c as HyperLink;
                //    if (hl != null && !string.IsNullOrEmpty(hl.Attributes["IDModuleForm"]))
                //    {
                //        List<string> moduelFunc = hl.Attributes["IDModuleForm"].Split(',').ToList();
                //        if (moduelFunc.Count > 1)
                //        {
                //            hl.Visible = ls.ModuleFormFunctions.Where(o => o.IDBllModuleForm == moduelFunc[0] && o.IDFunction == moduelFunc[1]).Count() > 0;
                //        }
                //        else
                //        {
                //            hl.Visible = ls.ModuleFormFunctions.Where(o => o.IDBllModuleForm == moduelFunc[0]).Count() > 0;
                //        }
                //    }
                //}
            }
        }

        protected void lbtnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect(Config.LogoutOIDCUri);
        }

    }
}