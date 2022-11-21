using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.Flow;
using Library.Module.FZDB;
using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.UserControl
{
    public partial class UserControl_ApprovalHistory : System.Web.UI.UserControl
    {
        /// <summary>
        /// 流程記錄集合物件。
        /// </summary>
        public ApprovalHistoryCollection ApprovalHistories { get; set; }

        /// <summary>
        /// 是否顯示[Duration](簽核時間)欄位。
        /// </summary>
        public bool? IsShowDurationField { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindApprovalHistoryList();
            }
        }

        private void BindApprovalHistoryList()
        {
            this.gvApprovalHistory.DataSource = this.ApprovalHistories;
            this.gvApprovalHistory.DataBind();
        }

        private const int SIGNEE_INDEX = 2;
        private const int STARTTIME_INDEX = 4;
        private const int ENDTIME_INDEX = 5;
        protected void gvApprovalHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            ApprovalHistory row = e.Row.DataItem as ApprovalHistory;
            if (row != null)
            {
                if (int.TryParse(row.Signee, out _))
                {
                    HrVEgEmploy signee = new HrVEgEmploy(row.Signee);
                    if (signee.AnalySa == "100")
                    {
                        CIvvCrewDb crewDb = new CIvvCrewDb(row.Signee);
                        e.Row.Cells[SIGNEE_INDEX].Text = crewDb.DisplayName;
                    }
                    else
                    {
                        e.Row.Cells[SIGNEE_INDEX].Text = signee.DisplayName;
                    }
                }
                else
                    e.Row.Cells[SIGNEE_INDEX].Text = Server.HtmlEncode(row.Signee);

                if (row.StartTime.HasValue)
                {
                    e.Row.Cells[STARTTIME_INDEX].Text = row.StartTime.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
                }

                if (row.EndTime.HasValue)
                {
                    e.Row.Cells[ENDTIME_INDEX].Text = row.EndTime.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
                }
            }
        }

        private const int DURATION_INDEX = 6;
        protected void gvApprovalHistory_PreRender(object sender, EventArgs e)
        {
            this.gvApprovalHistory.HeaderRow.TableSection = TableRowSection.TableHeader;

            if (this.IsShowDurationField.HasValue)
            {
                this.gvApprovalHistory.Columns[DURATION_INDEX].Visible = this.IsShowDurationField.Value;
            }
        }
    }
}