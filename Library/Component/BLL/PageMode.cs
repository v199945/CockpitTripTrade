using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Component.BLL
{
    /// <summary>
    /// 頁面模式類別，包含頁面資料的新增與修改。
    /// </summary>
    public class PageMode
    {
        /// <summary>
        /// 頁面模式列舉型態。
        /// </summary>
        public enum PageModeEnum
        {
            /// <summary>
            /// 頁面模式為無。
            /// </summary>
            None,

            /// <summary>
            /// 頁面模式為新建立。
            /// </summary>
            Create,

            /// <summary>
            /// 頁面模式為編輯。
            /// </summary>
            Edit,

            /// <summary>
            /// 頁面模式為工作流程。
            /// </summary>
            Task,

            /// <summary>
            /// 頁面模式為瀏覽。
            /// </summary>
            View
        }

        /// <summary>
        /// 預設建構子。
        /// </summary>
	    public PageMode()
	    {
	    }

        /// <summary>
        /// 檢核頁面模式。
        /// </summary>
        /// <param name="id">表單編號</param>
        /// <returns></returns>
        public static PageModeEnum CheckPageMode(string id, string proID, FlowStatus.StatusCodeEnum sce, string pageMode)
        {
            PageModeEnum? pm = null;
            if (string.IsNullOrEmpty(pageMode))
            {
                if (string.IsNullOrEmpty(id))
                {
                    pm = PageModeEnum.Create;
                }
                else
                {
                    if (string.IsNullOrEmpty(proID))
                    {
                        switch (sce)
                        {
                            case FlowStatus.StatusCodeEnum.RELEASED:
                                pm = PageModeEnum.View;
                                break;

                            default:
                                pm = PageModeEnum.View;
                                break;
                        }
                    }
                    else
                    {
                        pm = PageModeEnum.Task;
                    }
                }
            }
            else if (Enum.TryParse<PageModeEnum>(pageMode, out _))
            {
                pm = Enum.Parse(typeof(PageModeEnum), pageMode) as PageModeEnum?;
            }

            return pm.Value;
        }
    }
}