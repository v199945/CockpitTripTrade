using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Library.Component.BLL;

namespace Library.Module.FCDS.Admin
{
    /// <summary>
    /// Adminintrator 模組頁面父類別。
    /// </summary>
    public class AdminPage : BasePage
    {
        protected override void OnPreInit(EventArgs e)
        {
            LoginSession.VerifyAdmin(this.Page);

            base.OnPreInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.BaseModuleName = @"Adminintrator";
            this.BaseModuleTitle = @"Adminintrator";

            base.OnLoad(e);
        }
    }
}
