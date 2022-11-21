using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS;
using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.UserControl
{
    public partial class UserControl_TeamGroup : BaseUserControl
    {
        public RoleTypeEnum RoleTypeEnum { get; set; }

        public string ModuleName { get; set; }

        public string FormName { get; set; }

        public string IDFlowTeam { get; set; }

        public long BranchID { get; set; }

        public string ProID { get; set; }

        public string DepID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindTeamGroupList();
            }            
        }

        private void BindTeamGroupList()
        {
            ModuleFormRoleRightTeamCollection col;
            switch (RoleTypeEnum)
            {
                case RoleTypeEnum.Flow:
                    col = (ModuleFormRoleRightTeamCollection) ModuleFormRoleRightTeam.FetchFormRoleRightTeam(this.ModuleName, this.FormName, this.IDFlowTeam, this.ProID, RoleTypeEnum.Flow, ReturnObjectTypeEnum.Collection);
                    break;

                case RoleTypeEnum.Notify:
                    col = (ModuleFormRoleRightTeamCollection) ModuleFormRoleRightTeam.FetchFormRoleRightTeam(this.ModuleName, this.FormName, this.IDFlowTeam, this.ProID, RoleTypeEnum.Notify, ReturnObjectTypeEnum.Collection);
                    break;

                default:
                    col = (ModuleFormRoleRightTeamCollection) ModuleFormRoleRightTeam.FetchFormRoleRightTeam(this.ModuleName, this.FormName, this.IDFlowTeam, this.ProID, RoleTypeEnum.Flow, ReturnObjectTypeEnum.Collection);
                    break;
            }

            this.gvTeamGroup.DataSource = col;
            this.gvTeamGroup.DataBind();
        }

        /// <summary>
        /// 重新繫結團隊 GridView，FcdsConfig.aspx 頁面，當觸發機隊對應部門[FleetDepCode]下拉選單選取項目值改變時使用。
        /// </summary>
        public void ReBindFlowTeamList()
        {
            BindTeamGroupList();
        }

        protected void gvFlowTeam_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            GridViewRow gvr = e.Row;
            ModuleFormRoleRightTeam row = (ModuleFormRoleRightTeam) e.Row.DataItem;
            //DataRowView row = (DataRowView) e.Row.DataItem;
            if (row != null)
            {
                UserControl_SelectEmployee ucse = (UserControl_SelectEmployee) gvr.FindControl("SelectEmployee1");
                if (ucse != null)
                {
                    //ucse.Enable = row.IsEnable;
                    DropDownList employee = (DropDownList) ucse.FindControl("Employee");
                    if (employee != null)
                    {
                        // && (this.PageModeEnum == PageMode.PageModeEnum.Create || this.PageModeEnum == PageMode.PageModeEnum.Edit)
                        employee.Enabled = row.IsEnable && !(this.BasePageModeEnum == PageMode.PageModeEnum.View);

                        if (string.IsNullOrEmpty(row.UnitCd) && string.IsNullOrEmpty(row.UnitCdV))
                        {
                            string[] ary = this.DepID.Split(',');
                            if (ary != null && ary.Length > 1)
                            {
                                //ucse
                                employee.DataSource = (DataTable) HrVEgEmploy.FetchByUnitCdAndUnitCdV(ary[0], ary[1], row.IsChief, ReturnObjectTypeEnum.DataTable);
                            }
                        }
                        else
                        {
                            //ucse
                            employee.DataSource = (DataTable) HrVEgEmploy.FetchByUnitCdAndUnitCdV(row.UnitCd, row.UnitCdV, row.IsChief, ReturnObjectTypeEnum.DataTable);
                        }
                        //ucse.DataSource = (DataTable) HrVEgEmploy.FetchByUnitCdAndUnitCdV(row.UnitCd, row.UnitCdV, row.IsChief, ReturnObjectTypeEnum.DataTable);
                        employee.DataBind();

                        ListItem li = employee.Items.FindByValue(row.EmployID);
                        if (li == null)
                        {
                            employee.Items.Add(new ListItem(new HrVEgEmploy(row.EmployID).DisplayName + @"(人員異動，請重新選擇)", row.EmployID));
                        }
                        employee.SelectedValue = row.EmployID;
                        //ucse.EmployeeID = row.EmployID;

                        if (row.RequireCode == "R")
                        {
                            RequiredFieldValidator rfv = (RequiredFieldValidator) ucse.FindControl("RequiredFieldValidator1");
                            //rfv.CssClass = @"invalid-feedback";
                            rfv.Enabled = true;

                            switch (this.RoleTypeEnum)
                            {
                                case RoleTypeEnum.None:
                                    break;

                                case RoleTypeEnum.Flow:
                                    rfv.ErrorMessage = @"Flow Team - [ " + row.RoleName + @" ] is required!";
                                    break;

                                case RoleTypeEnum.Notify:
                                    rfv.ErrorMessage = @"Notify Team - [ " + row.RoleName + @" ] is required!";
                                    break;
                            }
                            //rfv.ForeColor = Color.Red;

                            employee.Attributes.Add("OnChange", @"javascript:ValidatorOnChange(event); RequiredFieldValidator_CheckValidControl('" + rfv.ClientID + @"', '" + employee.ClientID + @"');");
                        }
                    }
                }

            }
        }

        protected void gvTeamGroup_PreRender(object sender, EventArgs e)
        {
            this.gvTeamGroup.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        public bool SaveTeam(bool isSaveLog)
        {
            DateTime dt = DateTime.Now;

            foreach (GridViewRow gvr in this.gvTeamGroup.Rows)
            {
                UserControl_SelectEmployee ucse = (UserControl_SelectEmployee) gvr.FindControl("SelectEmployee1");
                HiddenField hdRequireCode = gvr.FindControl("RequireCode") as HiddenField;

                if (ucse != null && hdRequireCode != null && hdRequireCode.Value == "R")
                {
                    ModuleFormRoleTeam frt = new ModuleFormRoleTeam();
                    frt.ModuleNmae = this.ModuleName;
                    frt.FormName = this.FormName;
                    frt.IDFlowTeam = this.IDFlowTeam;// gvTeamGroup.DataKeys[gvr.RowIndex]["IDFlowTeam"].ToString();
                    frt.BranchID = this.BranchID;

                    DropDownList employee = (DropDownList) ucse.FindControl("Employee");
                    if (employee != null)
                    {
                        frt.EmployID = employee.SelectedValue;//ucse.EmployeeID;
                    }

                    frt.IDRole = gvTeamGroup.DataKeys[gvr.RowIndex]["IDRole"].ToString();
                    frt.CreateBy = this.LoginSession.UserID;
                    frt.CreateStamp = dt;
                    frt.UpdateBy = this.LoginSession.UserID;
                    frt.UpdateStamp = dt;

                    if (!frt.Save(this.BasePageModeEnum, isSaveLog))
                    {
                        return false;
                    }
                }
                // 20221101 648267:新增NotifyTeam1"結案通知"儲存功能
                else if(ucse != null && hdRequireCode != null && hdRequireCode.Value == "O")
                {
                    ModuleFormRoleTeam frt = new ModuleFormRoleTeam();
                    frt.ModuleNmae = this.ModuleName;
                    frt.FormName = this.FormName;
                    frt.IDFlowTeam = this.IDFlowTeam;
                    frt.BranchID = this.BranchID;

                    DropDownList employee = (DropDownList)ucse.FindControl("Employee");
                    if (employee != null)
                    {
                        frt.EmployID = employee.SelectedValue;
                        frt.IDRole = gvTeamGroup.DataKeys[gvr.RowIndex]["IDRole"].ToString();
                        frt.CreateBy = this.LoginSession.UserID;
                        frt.CreateStamp = dt;
                        frt.UpdateBy = this.LoginSession.UserID;
                        frt.UpdateStamp = dt;
                        int number = 0;
                        if (int.TryParse(employee.SelectedValue, out number))
                        {
                            if (number > 0)
                            {
                                if (!frt.Save(this.BasePageModeEnum, isSaveLog))
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            //20221104 648267:如果選空的，將當前的資料刪除
                            frt.EmployID = "000000";
                            if (!frt.Save(this.BasePageModeEnum, isSaveLog))
                            {
                                return false;
                            }
                        }
                    }
                    
                }
            }

            return true;
        }
    }
}