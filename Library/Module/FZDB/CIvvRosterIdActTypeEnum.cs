using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Module.FZDB
{
    /// <summary>
    /// 組員在職狀態列舉型態。
    /// </summary>
    public enum CIvvRosterIdActTypEnum
    {
        /// <summary>
        /// 在職中。
        /// </summary>
        Active = 0,

        /// <summary>
        /// 解雇。
        /// </summary>
        Dismissed = 1,

        /// <summary>
        /// 退休。
        /// </summary>
        Retired = 2,

        /// <summary>
        /// 辭職。
        /// </summary>
        Resigned = 3,

        /// <summary>
        /// 資遣。
        /// </summary>
        LaidOff = 4,

        ContractTo = 5,

        JumpShip = 6,

        Medical = 7,

        /// <summary>
        /// 身故。
        /// </summary>
        Deceased = 8,

        /// <summary>
        /// 各類留職停薪。
        /// </summary>
        Inactive = 9,

        Transferred = 10
    }
}
