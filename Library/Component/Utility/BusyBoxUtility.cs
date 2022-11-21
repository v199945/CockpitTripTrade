using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Library.Component.Utility
{
    /// <summary>
    /// BusyBox 控制項輔助密封類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class BusyBoxUtility
    {
        private BusyBoxUtility()
        {

        }

        /// <summary>
        /// 隱藏 BusyBox 控制項。
        /// </summary>
        /// <param name="page">ASP.NET 頁面</param>
        /// <param name="e">事件資料</param>
        /// <param name="js">JavaScript 字串</param>
        public static void HideBusyBox(Page page, EventArgs e, string js)
        {
            ScriptManager.RegisterStartupScript(page, e.GetType(), @"hideBusyBox", js, true);
        }
    }
}