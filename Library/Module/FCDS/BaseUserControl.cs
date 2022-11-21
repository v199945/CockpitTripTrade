using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using BusyBoxDotNet;

using Library.Component.BLL;

namespace Library.Module.FCDS
{
    public class BaseUserControl : UserControl
    {
        #region Property
        /// <summary>
        /// 頁面模式列舉型態。
        /// </summary>
        public PageMode.PageModeEnum BasePageModeEnum { get; set; }

        public LoginSession LoginSession { get; set; }

        public BusyBox BusyBox { get; set; }
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            LoginSession = LoginSession.GetLoginSession();

            base.OnLoad(e);
        }

        protected void SetControlBusyBox(List<WebControl> webControls)
        {
            foreach (WebControl wc in webControls)
            {
                string controlTypeName = wc.GetType().Name;
                switch (controlTypeName.ToLower().ToString())
                {
                    case "textbox":
                        break;

                    case "dropdownlist":
                        wc.Attributes.Add(@"OnChange", this.BusyBox.ShowFunctionCall);
                        break;

                    case "button":
                    case "linkbutton":
                        wc.Attributes.Add(@"OnClick", this.BusyBox.ShowFunctionCall);
                        break;

                    case "radiobuttonlist":
                        RadioButtonList rbl = wc as RadioButtonList;
                        foreach (ListItem li in rbl.Items)
                        {
                            li.Attributes.Add(@"OnClick", @"javascript:if (this.value!='" + rbl.SelectedValue + @"') {" + this.BusyBox.ShowFunctionCall + @"};");
                        }
                        break;
                }
            }
        }
    }
}
