using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Component.Enums
{
    /// <summary>
    /// 飛航組員任務互換申請系統網站應用程式列舉型態。
    /// </summary>
    public enum WebApplicationEnum
    {
        /// <summary>
        /// 無。
        /// </summary>
        None = 0, 

        /// <summary>
        /// 飛航組員任務換班申請系統前台(飛航組員使用)。
        /// </summary>
        CockpitTripTrade = 1,

        /// <summary>
        /// 飛航組員任務換班申請系統後台(航務處組員派遣部人員使用)。
        /// </summary>
        CockpitTripTradeAdmin = 2

        
    }
}
