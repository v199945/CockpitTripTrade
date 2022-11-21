using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Module.FCDS.Application
{
    /// <summary>
    /// Application 模組頁面父類別。
    /// </summary>
    public class ApplicationPage : BasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            this.BaseModuleName = @"Application";
            this.BaseModuleTitle = @"Application";

            base.OnLoad(e);
        }
    }
}