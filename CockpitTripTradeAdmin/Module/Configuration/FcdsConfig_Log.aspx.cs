using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Utility;
using Library.Module.FZDB;
using Library.Module.FCDS.Configuration;
using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.Module.Configuration
{
    public partial class CockpitTripTradeAdmin_Module_Configuration_FcdsConfig_Log : ConfigurationPage
    {
        private string IDFcdsConfig = null;
        private long? TargetBranchID = null;
        private FcdsConfig obj = null;
        private Library.Component.BLL.Version objBaseVersion = null;
        private Library.Component.BLL.Version objTargetVersion = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.IDFcdsConfig = Request.QueryString["IDFcdsConfig"];
            this.obj = new FcdsConfig(this.IDFcdsConfig);
            if (long.TryParse(Request.QueryString["BranchID"], out _))
            {
                this.TargetBranchID = long.Parse(Request.QueryString["BranchID"]);
            }
            this.BaseModuleName = Request.QueryString["ModuleName"];
            this.BaseFormName = Request.QueryString["FormName"];
            this.BaseFormTitle = @"設定版本紀錄比較";

            if (!Page.IsPostBack)
            {
                InitForm();

                if (!string.IsNullOrEmpty(this.BaseVersion.SelectedValue) && !string.IsNullOrEmpty(this.TargetVersion.SelectedValue))
                {
                    BingFormLogList();
                    BindTeamLogList();
                }
            }
        }

        private void InitForm()
        {
            objBaseVersion = (Library.Component.BLL.Version) obj.Versions.GetPreviousVersion(TargetBranchID.Value);
            objTargetVersion = (Library.Component.BLL.Version) obj.Versions.GetVersion(TargetBranchID.Value);

            if (objBaseVersion != null && objTargetVersion != null)
            {
                this.BaseVersion.DataSource = obj.Versions;
                this.BaseVersion.DataBind();
                this.BaseVersion.SelectedValue = objBaseVersion.BranchID.ToString();

                this.TargetVersion.DataSource = obj.Versions;
                this.TargetVersion.DataBind();
                this.TargetVersion.SelectedValue = objTargetVersion.BranchID.ToString();
            }

            BindForm();
        }

        private void BindForm()
        {
            if (objBaseVersion != null && objTargetVersion != null)
            {
                this.BaseVersionUpdateBy.Text = new HrVEgEmploy(objBaseVersion.UpdateBy).DisplayName;
                this.BaseVersionUpdateStamp.Text = objBaseVersion.UpdateStamp.ToString("yyyy/MM/dd HH:mm:ss");

                this.TargetVersionUpdateBy.Text = new HrVEgEmploy(objTargetVersion.UpdateBy).DisplayName;
                this.TargetVersionUpdateStamp.Text = objTargetVersion.UpdateStamp.ToString("yyyy/MM/dd HH:mm:ss");
            }

            SetForm();
        }

        private void SetForm()
        {
            this.BaseVersion.Attributes.Add("OnInput", @"javascript:ValidatorOnChange(event); RequiredFieldValidator_CheckValidControl('" + this.rfvBaseVersion.ClientID + @"', '" + this.BaseVersion.ClientID + @"');");
            this.TargetVersion.Attributes.Add("OnInput", @"javascript:ValidatorOnChange(event); RequiredFieldValidator_CheckValidControl('" + this.rfvTargetVersion.ClientID + @"', '" + this.TargetVersion.ClientID + @"');");
            this.btnCompare.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
        }

        private void BingFormLogList()
        {
            DataTable formlog = obj.FetchLog();
            DataRow drBaseVersion = formlog.Select(@"BranchID=" + this.BaseVersion.SelectedValue)[0];
            DataRow drTargetVersion = formlog.Select(@"BranchID=" + this.TargetVersion.SelectedValue)[0];

            DataTable dt = ModuleFormDefinition.FetchFormDefinition(this.BaseModuleName, this.BaseFormName);
            DataTableUtility.CreateDataTableColumn(dt, @"BaseVersion", @"System.String", true, null, null);
            DataTableUtility.CreateDataTableColumn(dt, @"TargetVersion", @"System.String", true, null, null);

            foreach (DataRow dr in dt.Rows)
            {
                if (drBaseVersion.Table.Columns.Contains(dr["IDComponent"].ToString()))
                {
                    dr["BaseVersion"] = drBaseVersion[dr["IDComponent"].ToString()].ToString();
                    dr["TargetVersion"] = drTargetVersion[dr["IDComponent"].ToString()].ToString();
                }
            }

            this.gvFormLogList.DataSource = dt;
            this.gvFormLogList.DataBind();
        }

        private void BindTeamLogList()
        {
            DataTable teamlog = ModuleFormRoleTeam.FetchLog(this.BaseModuleName, this.BaseFormName, this.IDFcdsConfig);

            DataTable dt = ModuleFormRoleDefinition.FetchModuleFormRoleDefinition(this.BaseModuleName, this.BaseFormName);
            DataTableUtility.CreateDataTableColumn(dt, @"BaseVersion", @"System.String", true, null, null);
            DataTableUtility.CreateDataTableColumn(dt, @"TargetVersion", @"System.String", true, null, null);

            foreach (DataRow dr in dt.Rows)
            {
                DataRow[] drsBaseVersion = teamlog.Select(@"BranchID=" + this.BaseVersion.SelectedValue + @" AND IDRole='" + dr["IDRole"].ToString() + @"'");
                DataRow[] drsTargetVersion = teamlog.Select(@"BranchID=" + this.TargetVersion.SelectedValue + @" AND IDRole='" + dr["IDRole"].ToString() + @"'");

                if (drsBaseVersion != null && drsBaseVersion.Length > 0)
                {
                    dr["BaseVersion"] = drsBaseVersion[0]["EmployID"].ToString();
                }

                if (drsTargetVersion != null && drsTargetVersion.Length > 0)
                {
                    dr["TargetVersion"] = drsTargetVersion[0]["EmployID"].ToString();
                }
            }

            this.gvTeamLogList.DataSource = dt;
            this.gvTeamLogList.DataBind();
        }

        protected void btnCompare_Click(object sender, EventArgs e)
        {
            objBaseVersion = obj.Versions.GetVersion(long.Parse(this.BaseVersion.SelectedValue));
            objTargetVersion = obj.Versions.GetVersion(long.Parse(this.TargetVersion.SelectedValue));

            BindForm();
            BingFormLogList();
            BindTeamLogList();
        }

        protected void gvFormLogList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            DataRowView row = (DataRowView) e.Row.DataItem;
            if (row["IDComponent"].ToString() == "IDAcType")
            {
                CIvvAircType actype = new CIvvAircType(row["BaseVersion"].ToString());
                e.Row.Cells[2].Text = actype.IcaoCode;

                actype = new CIvvAircType(row["TargetVersion"].ToString());
                e.Row.Cells[3].Text = actype.IcaoCode;
            }

            if (row["IDComponent"].ToString() == "IDCrewPos")
            {
                CIvvPositions crewpos = new CIvvPositions(int.Parse(row["BaseVersion"].ToString()));
                e.Row.Cells[2].Text = crewpos.Code;

                crewpos = new CIvvPositions(int.Parse(row["TargetVersion"].ToString()));
                e.Row.Cells[3].Text = crewpos.Code;
            }

            if (row["IDComponent"].ToString() == "FleetDepCode")
            {
                string[] ary = row["BaseVersion"].ToString().Split(',');
                HrVPbUnitCd dep;
                if (ary != null && ary.Length > 1)
                {
                    dep = new HrVPbUnitCd(ary[0], ary[1]);
                    e.Row.Cells[2].Text = dep.CDesc;
                }

                ary = row["TargetVersion"].ToString().Split(',');
                if (ary != null && ary.Length > 1)
                {
                    dep = new HrVPbUnitCd(ary[0], ary[1]);
                    e.Row.Cells[3].Text = dep.CDesc;
                }
            }
        }

        protected void gvFormLogList_PreRender(object sender, EventArgs e)
        {
            this.gvFormLogList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void gvTeamLogList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            DataRowView row = (DataRowView)e.Row.DataItem;
            HrVEgEmploy employ = new HrVEgEmploy(row["BaseVersion"].ToString());
            e.Row.Cells[2].Text = employ.DisplayName;

            employ = new HrVEgEmploy(row["TargetVersion"].ToString());
            e.Row.Cells[3].Text = employ.DisplayName;
        }

        protected void gvTeamLogList_PreRender(object sender, EventArgs e)
        {
            this.gvTeamLogList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}