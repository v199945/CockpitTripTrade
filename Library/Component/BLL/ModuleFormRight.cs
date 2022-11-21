using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using log4net;

using Library.Component.Utility;

namespace Library.Component.BLL
{
	/// <summary>
	/// 表單欄位權限類別。
	/// </summary>
    public class ModuleFormRight
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof(ModuleFormRight));

		private ModuleFormRight()
        {

        }

		/// <summary>
		/// 依據流程編號設定表單欄位權限。
		/// </summary>
		/// <param name="page">頁面</param>
		/// <param name="moduleName">模組名稱</param>
		/// <param name="formName">表單名稱</param>
		/// <param name="proID">流程編號</param>
		/// <param name="enumPageMode">頁面模式列舉型態</param>
        public static void SetModuleFormRight(Page page, string moduleName, string formName, string proID, PageMode.PageModeEnum enumPageMode)
        {
            DataTable dt = ModuleFormDefinition.FetchFormDefinitionRight(moduleName, formName, proID, enumPageMode);
            if (dt.Rows.Count > 0)
            {
                SetControls(page, dt, enumPageMode);
            }
        }

		/// <summary>
		/// 設定表單欄位控制項
		/// </summary>
		/// <param name="page">頁面</param>
		/// <param name="dt">某流程編號之表單欄位與權限定義</param>
		/// <param name="pageMode">頁面模式列舉型態</param>
        public static void SetControls(Page page, DataTable dt, PageMode.PageModeEnum pageMode)
		{
			string position = null;

			foreach (DataRow dr in dt.Rows)
            {
                string idcomponent = dr["IDComponent"].ToString();
                bool isenable = dr["IsEnable"] == DBNull.Value ? false : bool.Parse(dr["IsEnable"].ToString());
                bool isrequire = dr["IsRequire"] == DBNull.Value ? false : bool.Parse(dr["IsRequire"].ToString());
                string labelid = @"lbl" + idcomponent;

				Label labelcontrol = (Label) ControlUtility.FindControlRecursive(page.Master, labelid);
				WebControl webcontrol = (WebControl) ControlUtility.FindControlRecursive(page.Master, idcomponent);

                if (webcontrol == null)
                {
					Label lbl = new Label() { Text = idcomponent + @" Control not found, FormName=" + dr["FormName"].ToString(), ForeColor = Color.Red };
					page.Form.Controls.AddAt(0, lbl);
					//HttpContext.Current.Response.Write(@"<h1 style=""color: red; z-index: 9;"">" + idcomponent + @" Control not found, FormName=" + dr["FormName"].ToString() + @"</h1>");
					break;
                }

				if (labelcontrol != null)
                {
					labelcontrol.Text = dr["ComponentDisplayName"].ToString();


				}

                // TODO: customized, special control

                switch (pageMode)
                {
                    case PageMode.PageModeEnum.View:
                        webcontrol.Enabled = false;
                        break;

                    default:
                        if (isenable)
                        {
                            webcontrol.Enabled = true;
                            webcontrol.Visible = true;

                            if (isrequire)
                            {
								int i = page.Form.Controls.IndexOf(webcontrol);
								RequiredFieldValidator rfv = GetRequiredFieldValidator(webcontrol, dr["ComponentDisplayName"].ToString());
								webcontrol.Parent.Controls.Add(rfv);
								/*
								string attributesValue = webcontrol.Attributes["OnInput"];
								if (attributesValue != null)
								{
									string[] aryAttributeValue = attributesValue.Split(new string[] { @"javascript:" }, StringSplitOptions.RemoveEmptyEntries);
									//Regex.Split(attributesValue, @"javascript:");
									string avs = null;
									foreach (string av in aryAttributeValue)
									{
										avs += av;
									}
									webcontrol.Attributes.Add("OnInput", @"javascript:ValidatorOnChange(event); RequiredFieldValidator_CheckValidControl('" + rfv.ClientID + @"', '" + webcontrol.ClientID + @"'); " + avs);
								}
								else
								{
									webcontrol.Attributes.Add("OnInput", @"javascript:ValidatorOnChange(event); RequiredFieldValidator_CheckValidControl('" + rfv.ClientID + @"', '" + webcontrol.ClientID + @"');");
								}
								*/
								webcontrol.Attributes.Add("OnInput", @"javascript:ValidatorOnChange(event); RequiredFieldValidator_CheckValidControl('" + rfv.ClientID + @"', '" + webcontrol.ClientID + @"');");
								//page.Form.Controls.AddAt(i + 1, GetRequiredFieldValidator(webcontrol, dr["ComponentDisplayName"].ToString()));
                                //page.Form.Controls.Add(GetRequiredFieldValidator(webcontrol, dr["ComponentDisplayName"].ToString()));
								
								Label requiredlabel = new Label() { CssClass = @"vdform-mustfill", Text = @"*" };
								//requiredlabel.ForeColor = Color.Red;
								//requiredlabel.Style.Add("position", "absolute");
								//requiredlabel.Style.Add("z-index", "3");
								//requiredlabel.Text = "*";
								//requiredlabel.Style.Add(HtmlTextWriterStyle.Color, "red");
								labelcontrol.Parent.Controls.AddAt(0, requiredlabel);
								//labelcontrol.Parent.Controls.Add(requiredlabel);

                                position += "Position.set('" + requiredlabel.ClientID + "', Position.get('" + labelcontrol.ClientID + "', 'Tail', 1, -2));" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            webcontrol.Enabled = false;
                        }
                        break;
                }

				if (webcontrol.Visible)
                {
					int offsetLeft = 6;
					if (!isrequire)
						offsetLeft = 2;

					//position += @"Position.set('', Position.get('" + labelcontrol.ClientID + "','Tail'," + offsetLeft.ToString() + ",0));";
				}

            }
			
			//SetRequiredAsteriskPosition(page, position);
        }

		/// <summary>
		/// 產生 RequiredFieldValidator 物件。
		/// </summary>
		/// <param name="webControl">網頁控制項</param>
		/// <param name="componentDisplayName">控制項顯示名稱</param>
		/// <returns></returns>
        private static RequiredFieldValidator GetRequiredFieldValidator(WebControl webControl, string componentDisplayName)
        {
            string controlTypeName = webControl.GetType().Name;
            RequiredFieldValidator rfv = new RequiredFieldValidator() { ControlToValidate = webControl.ID, CssClass = @"invalid-feedback", Display = ValidatorDisplay.Dynamic, ErrorMessage = @"[ " + componentDisplayName + @" ] is required!", SetFocusOnError = true };
			//rfv.Text = @"*";UniqueID

			switch (controlTypeName.ToLower().ToString())
            {
                case "textbox":
                    break;

                case "dropdownlist":
                case "radiobuttonlist":
                case "checkboxlist":
                    break;

				case "usercontrol_selectemployee":
					break;

				default:
					break;
            }

            return rfv;
        }

        #region "NoUsage"
        private static void SetRequiredAsteriskPosition(Page page, string position)
        {
			string js = GetScript() + Environment.NewLine;
			js += @"function setPosition() { " + Environment.NewLine + position + Environment.NewLine + " };" + Environment.NewLine;

			string resizeWindow = @"top.window.moveTo(0,0);
									if (document.all) {
										// this function is disabled to force user maximize window size by clicking
										window.resizeTo(screen.availWidth, screen.availHeight);
									}
									else if (document.layers||document.getElementById) {
										if (top.window.outerHeight < screen.availHeight || top.window.outerWidth < screen.availWidth){
										}
									}";
			js += resizeWindow;

			page.ClientScript.RegisterClientScriptBlock(page.GetType(), "RequiredText3", "<script>" + js + "</script>");
			/*
			HtmlGenericControl body = (HtmlGenericControl) page.Master.FindControl("MasterBody");
			if (body != null)
			{
				//body.Attributes.Add("onload", "setPosition();");
				//body.Attributes.Add("onresize", "setPosition();");
			}
			*/
		}

        private static string GetScript()
        {
			string str = @"var Position = (function() {
								// Resolve a string identifier to an object
								// ========================================
								function resolveObject(s) {
									if (document.getElementById && document.getElementById(s)!=null) {
										return document.getElementById(s);
									}
									else if (document.all && document.all[s]!=null) {
										return document.all[s];
									}
									else if (document.anchors && document.anchors.length && document.anchors.length>0 && document.anchors[0].x) {
										for (var i=0; i<document.anchors.length; i++) {
											if (document.anchors[i].name==s) {
												return document.anchors[i]
											}
										}//end for
									}
								}

								var pos = {};
								pos.$VERSION = 1.0;

								// Set the position of an object
								// =============================
								pos.set = function(o, left, top, width) {
									if (typeof(o)=='string') {
										o = resolveObject(o);
									}
									if (o==null || !o.style) {
										return false;
									}

									// If the second parameter is an object, it is assumed to be the result of getPosition()
									if (typeof(left)=='object') {
										var pos = left;

										top = pos.top;
										left = pos.left + pos.width ;
									}

									o.style.left = left + 'px';
									o.style.top = top + 'px';
									return true;
								};
								//-------------------start point --------------
								// Retrieve the position and size of an object
								// ===========================================
								pos.get = function(o, HeadTail, extraLeft, extraTop) {
									var fixBrowserQuirks = true;
									// If a string is passed in instead of an object ref, resolve it
									if (typeof(o)=='string') {
										o = resolveObject(o);
									}

									if (o==null) {
										return null;
									}

									var left = 0;
									var top = 0;
									var width = 0;
									var height = 0;
									var parentNode = o.parentNode;
									var offsetParent = o.offsetParent;
									var originalObject = o;
									/*
									if (o.offsetLeft) {
										left += o.offsetLeft;
									}
									*/
									if (originalObject.offsetWidth) {
										width = originalObject.offsetWidth;
									}
									if (originalObject.offsetHeight) {
										height = originalObject.offsetHeight;
									}
									//alert('333');
									if (HeadTail=='Head'){left=left-width;}
									return {'left':left + extraLeft, 'top':top + extraTop, 'width':width, 'height':height};
								};//end function pos.get

								// Retrieve the position of an object's center point
								// =================================================
								pos.getCenter = function(o) {
									var c = this.get(o);
									if (c==null) { return null; }
										c.left = c.left + (c.width/2);
										c.top = c.top + (c.height/2);
										return c;
									};

								return pos;
						   })();";

			return str;
        }
        #endregion
    }
}