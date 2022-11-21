using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Module.FCDS.Application;
using Library.Module.FZDB;

namespace CockpitTripTrade.Module.Application.UserControl
{
    public partial class UserControl_FcdsApplyRoster : ApplicationUserControl
    {
        /// <summary>
        /// 飛航組員班表日期索引鍵和值的集合物件。
        /// </summary>
        public Dictionary<double, DateTime> DateTimes { get; set; }

        /// <summary>
        /// 飛航組員申請人員工編號。
        /// </summary>
        public string ApplicantID { get; set; }

        /// <summary>
        /// 飛航組員受申請人員工編號。
        /// </summary>
        public string RespondentID { get; set; }

        public FcdsApply FcdsApply { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && this.BasePageModeEnum != PageMode.PageModeEnum.Create)
            {
                BindRepeaterList();
            }
        }

        public void BindRepeaterList()
        {
            if (Page.IsPostBack)
            {
                if (this.BasePageModeEnum == PageMode.PageModeEnum.Create && !string.IsNullOrEmpty(this.RespondentID))
                {
                    DataTable dtApplicant = CIRoster.FetchTickRoster(this.ApplicantID, this.DateTimes.First().Value, this.DateTimes.Last().Value);
                    DataTable dtRespondent = CIRoster.FetchTickRoster(this.RespondentID, this.DateTimes.First().Value, this.DateTimes.Last().Value);

                    ProcessRosterData(dtApplicant, dtRespondent);
                }

            }
            else
            {
                if (this.BasePageModeEnum == PageMode.PageModeEnum.Create)
                {

                }
                else
                {

                }
            }
        }

        private DataTable ProcessRosterData(DataTable dtApplicant, DataTable dtRespondent)
        {
            DataTable dtTickRoster = new DataTable("dtTickRoster");

            foreach (KeyValuePair<double, DateTime> item in this.DateTimes)
            {
                //p.Field<DateTime>("Duty_Str_Dt_Tpe") != null && 
                var rowApplicant = dtApplicant.AsEnumerable().Where(p => p.Field<DateTime?>("Duty_Str_Dt_Tpe") >= item.Value);
                foreach (DataRow dr in rowApplicant)
                {

                }
                var rowRespondent = dtRespondent.AsEnumerable().Where(p => p.Field<DateTime?>("Duty_Str_Dt_Tpe") >= item.Value);
            }

            return dtTickRoster;
        }
    }
}