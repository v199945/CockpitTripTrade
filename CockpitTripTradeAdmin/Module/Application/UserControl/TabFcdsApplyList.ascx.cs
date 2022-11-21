using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using log4net;

using Library.Module.FCDS.Application;

namespace CockpitTripTradeAdmin.Module.Application.UserControl
{
    public partial class CockpitTripTradeAdmin_Module_Application_UserControl_TabFcdsApplyList : ApplicationUserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CockpitTripTradeAdmin_Module_Application_UserControl_TabFcdsApplyList));

        public List<FcdsApply> FcdsApplies { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            BindList();
        }

        private void BindList()
        {
            this.gvList.DataSource = this.FcdsApplies;
            this.gvList.DataBind();
        }

        private const int APPLICATION_DATE_INDEX = 3;
        private const int APPLICATION_DEADLINE_INDEX = 4;
        private const int APPLICANT_INDEX = 5;
        private const int RESPONDENT_INDEX = 6;
        private const int FLEET_INDEX = 7;
        private const int POS_INDEX = 8;
        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            GridViewRow gvr = e.Row;
            HyperLink hl = gvr.FindControl("Control") as HyperLink;
            if (hl != null)
            {
            }

            FcdsApply row = e.Row.DataItem as FcdsApply;
            if (row != null)
            {
                e.Row.Cells[FLEET_INDEX].Text = row.CIvvAircType.IcaoCode;
                e.Row.Cells[POS_INDEX].Text = row.CIvvPositions.Code;
                e.Row.Cells[APPLICATION_DATE_INDEX].Text = row.ApplicationDate.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
                e.Row.Cells[APPLICATION_DEADLINE_INDEX].Text = row.ApplicationDeadline.Value.ToString("ddMMMyyyy", CultureInfo.CreateSpecificCulture("en-US"));
                e.Row.Cells[APPLICANT_INDEX].Text = row.ApplicantCrew.DisplayName;
                e.Row.Cells[RESPONDENT_INDEX].Text = row.RespondentCrew.DisplayName;
            }
        }

        protected void gvList_PreRender(object sender, EventArgs e)
        {
            this.gvList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}