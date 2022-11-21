using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Module.FCDS.Configuration
{
    /// <summary>
    /// Configuration 模組頁面父類別。
    /// </summary>
    public class ConfigurationPage : BasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            this.BaseModuleName = @"Configuration";
            this.BaseModuleTitle = @"Configuration";

            base.OnLoad(e);
        }
    }
}