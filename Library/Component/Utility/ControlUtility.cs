using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Library.Component.Utility
{
    /// <summary>
    /// ASP.NET WebControl 輔助密封類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class ControlUtility
    {
        /// <summary>
        /// 建構子之存取修飾詞改為 private 防止建立本類別物件。
        /// </summary>
        private ControlUtility()
        {

        }

        /// <summary>
        /// 以遞迴尋找控制項。
        /// </summary>
        /// <param name="parentControl">父控制項</param>
        /// <param name="id">控制項 ID</param>
        /// <returns></returns>
        public static Control FindControlRecursive(Control parentControl, string id)
        {
            if (parentControl.ID == id)
                return parentControl;

            Control foundControl = null;
            foreach (Control c in parentControl.Controls)
            {
                foundControl = FindControlRecursive(c, id);
                if (foundControl != null)
                    return foundControl;
            }

            return foundControl;
        }
    }
}