using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using log4net;

using Library.Component.BLL;
using Library.Component.Flow;
using Library.Component.Utility;
using Library.Module.FCDS;
using Library.Module.FCDS.Application;
using Library.Module.FCDS.Configuration;
using Library.Module.FZDB;
using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.Module.Application
{
    public partial class CockpitTripTradeAdmin_Module_Application_FcdsApply : ApplicationPage
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CockpitTripTradeAdmin_Module_Application_FcdsApply));

        /// <summary>
        /// 飛航組員任務換班申請系統申請人 AIMS 組員基本資料物件。
        /// </summary>
        private CIvvCrewDb applicantCrewDb;

        /// <summary>
        /// 飛航組員任務換班申請系統受申請人 AIMS 組員基本資料物件。
        /// </summary>
        private CIvvCrewDb respondentCrewDb;

        /// <summary>
        /// 飛航組員任務換班申請系統之機隊職級設定物件。
        /// </summary>
        private FcdsConfig fcdsConfig;

        /// <summary>
        /// 飛航組員任務換班申請系統申請主表單物件。
        /// </summary>
        private FcdsApply obj;

        /// <summary>
        /// 飛航組員任務換班申請系統申請主表單之申請人與受申請人之班表集合物件。
        /// </summary>
        private FcdsApplyRosterCollection fcdsApplyRosters;

        /// <summary>
        /// 飛航組員任務換班申請系統申請主表單組員派遣部審核意見物件。
        /// </summary>
        private FcdsApplyApprove fcdsApplyApprove;

        /// <summary>
        /// 飛航組員任務換班申請系統表單流程編號。
        /// </summary>
        private string proID;

        /// <summary>
        /// 勾選飛航組員任務換班申請之班表 UserControl 中須顯示之日期範圍索引鍵和值的集合物件。
        /// </summary>
        private Dictionary<double, DateTime> rosterDates;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            this.BaseFormName = @"FcdsApply";
            ModuleForm moduleForm = new ModuleForm(this.BaseModuleName, this.BaseFormName);
            this.BaseFormTitle = moduleForm.FormTitle;

            proID = Request.QueryString["ProID"];
            string idFcdsApply = Request.QueryString["IDFcdsApply"];
            obj = new FcdsApply(idFcdsApply);
            if (string.IsNullOrEmpty(idFcdsApply))
            {
                string script = @"<script type=""text/javascript"">alert('Invalid Link!'); window.location='../ErrorHandler/NotFoundPage.aspx';</script>";
                Response.Write(script);
                Response.End();
            }

            if (obj.StatusCodeEnum == null)
            {
                string script = @"<script type=""text/javascript"">alert('The Form No. " + idFcdsApply + @" is invalid!'); window.location='../ErrorHandler/NotFoundPage.aspx';</script>";
                Response.Write(script);
                Response.End();
            }

            if (!string.IsNullOrEmpty(proID) && proID != FcdsHelper.PRO_REVOKE && obj.ProID != proID)
            {
                string script = @"<script type=""text/javascript"">alert('The process of Form No. " + idFcdsApply + @" is invalid!'); window.location='../ErrorHandler/NotFoundPage.aspx';</script>";
                Response.Write(script);
                Response.End();
            }

            this.fcdsApplyRosters = FcdsApplyRoster.FetchByIDFcdsApply(idFcdsApply, Library.Component.Enums.ReturnObjectTypeEnum.Collection) as FcdsApplyRosterCollection;
            this.fcdsApplyApprove = new FcdsApplyApprove(idFcdsApply);
            this.BasePageModeEnum = PageMode.CheckPageMode(idFcdsApply, proID, (FlowStatus.StatusCodeEnum)obj.StatusCodeEnum , Request.QueryString["PageMode"]);

            HtmlGenericControl divContentBody = null;
            switch (this.BasePageModeEnum)
            {
                case PageMode.PageModeEnum.Create:
                    proID = moduleForm.InitialProID;
                    applicantCrewDb = this.LoginSession.CrewDb;
                    break;

                case PageMode.PageModeEnum.Task:
                    if (!LoginSession.HasEditAuthority(this.Page, moduleForm.IDBllModuleForm))
                    {
                        // Redirect to Unauthorized Page.
                        this.Page.Response.Redirect(@"~/Module/ErrorHandler/UnauthorizedPage.aspx");
                    }

                    divContentBody = this.Master.FindControl(@"divContentBody") as HtmlGenericControl;
                    divContentBody.Attributes.Add(@"class", @"page-audit");
                    applicantCrewDb = new CIvvCrewDb(obj.ApplicantID);
                    respondentCrewDb = new CIvvCrewDb(obj.RespondentID);
                    this.BaseFormTitle = this.BaseFormTitle.Insert(0, "審核");
                    break;

                case PageMode.PageModeEnum.View:
                    if (!LoginSession.HasViewAuthority(this.Page, moduleForm.IDBllModuleForm))
                    {
                        // Redirect to Unauthorized Page.
                        this.Page.Response.Redirect(@"~/Module/ErrorHandler/UnauthorizedPage.aspx");
                    }

                    divContentBody = this.Master.FindControl(@"divContentBody") as HtmlGenericControl;
                    divContentBody.Attributes.Add(@"class", @"page-audit");
                    applicantCrewDb = new CIvvCrewDb(obj.ApplicantID);
                    respondentCrewDb = new CIvvCrewDb(obj.RespondentID);
                    this.BaseFormTitle = this.BaseFormTitle.Insert(0, "查詢");
                    /*
                    if (this.LoginSession.CrewDb.ID != obj.ApplicantID && this.LoginSession.CrewDb.ID != obj.RespondentID)
                    {
                        string script = @"<script type=""text/javascript"">alert('You do not have permissions to view this form!'); window.location='TaskList.aspx';</script>";
                        Response.Write(script);
                        Response.End();
                    }
                    */
                    break;

                default:
                    proID = moduleForm.ProID;
                    break;
            }

            string ac = applicantCrewDb.GetGroupByAcID();
            int pos = applicantCrewDb.GetGroupByPosID();
            fcdsConfig = new FcdsConfig(ac, pos);

            if (!Page.IsPostBack)
            {
                InitForm();
            }

            BindUserControls();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ModuleFormRight.SetModuleFormRight(this.Page, this.BaseModuleName, this.BaseFormName, this.proID, this.BasePageModeEnum);
        }

        private void InitForm()
        {
            BindForm();
            SetForm();
        }

        private void BindForm()
        {
            this.IDFcdsApply.Text = obj.IDFcdsApply;

            //if (obj.IsApplicantRevoke.HasValue)
            //{
            //    this.IsApplicantRevoke.Text = obj.IsApplicantRevoke.Value ? @"Yes" : @"No";
            //}

            this.StatusCode.Text = obj.StatusCName;

            if (obj.ApplicationDate.HasValue)
            {
                this.ApplicationDate.Text = obj.ApplicationDate.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
            }
                
            if (obj.ApplicationDeadline.HasValue)
            {
                this.ApplicationDeadline.Text = obj.ApplicationDeadline.Value.ToString("ddMMMyyyy", CultureInfo.CreateSpecificCulture("en-US"));
            }
                
            respondentCrewDb = new CIvvCrewDb(obj.RespondentID);
            //this.RespondentID.MaxLength = 0;
            this.RespondentID.Text = respondentCrewDb.ID + @"/" + respondentCrewDb.PassportName + @"(" + respondentCrewDb.GetGroupByFleetCode() + @" - " + respondentCrewDb.GetGroupByCrewPosCode() + @")";

            this.ApplicantID.Text = applicantCrewDb.ID + @"/" + applicantCrewDb.PassportName + @"(" + applicantCrewDb.GetGroupByFleetCode() + @" - " + applicantCrewDb.GetGroupByCrewPosCode() + @")";

            BindSwapScheduleMonth();
        }

        /// <summary>
        /// 繫結[互換月份]選項。
        /// </summary>
        private void BindSwapScheduleMonth()
        {
            // BasePageModeEnum == Create 則動態產生
            if (this.BasePageModeEnum == PageMode.PageModeEnum.Create)
            {
                DateTime? applicantPublishDate = applicantCrewDb.CIvvRosterId.PublishDate;// CIvvRosterId.FetchPublishDateByID(applicantCrewDb.ID);
                DateTime monthStartDate = DateTimeUtility.GetTheBeginOfMonth(DateTime.Now);//new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime monthEndDate = DateTimeUtility.GetTheEndOfMonth(DateTime.Now.Date);//new DateTime(monthStartDate.Year, monthStartDate.Month, DateTime.DaysInMonth(monthStartDate.Year, monthStartDate.Month));

                // [互換月份]之本月份選項為必須
                this.SwapScheduleMonth.Items.Add(new ListItem(monthStartDate.ToString("MMM yyyy", CultureInfo.CreateSpecificCulture("en-US")), monthStartDate.ToString("yyyyMM")));
                if (applicantPublishDate.HasValue && fcdsConfig.DeadlineOfAcrossMonth.HasValue)
                {
                    if (DateTime.Now.Day > fcdsConfig.DeadlineOfAcrossMonth.Value && applicantPublishDate.Value > monthEndDate)
                    {
                        // 每月下旬發佈次月班表後，建立[互換月份]之次月份選項
                        this.SwapScheduleMonth.Items.Add(new ListItem(monthStartDate.AddMonths(1).ToString("MMM yyyy", CultureInfo.CreateSpecificCulture("en-US")), monthStartDate.AddMonths(1).ToString("yyyyMM")));
                    }
                }

                if (this.SwapScheduleMonth.Items.Count == 1)
                {
                    this.SwapScheduleMonth.SelectedIndex = 0;
                }
            }
            // 擷取自資料庫[FzTFcdsApply].[SwapScheduleMonthItems]欄位
            else
            {
                string[] arySwapScheduleMonthItems = obj.SwapScheduleMonthItems.Split(',');
                foreach (string item in arySwapScheduleMonthItems)
                {
                    DateTime dt = new DateTime(int.Parse(item.Substring(0, 4)), int.Parse(item.Substring(4, 2)), 1);
                    this.SwapScheduleMonth.Items.Add(new ListItem(dt.ToString("MMM yyy", CultureInfo.CreateSpecificCulture("en-US")), item));
                }

                this.SwapScheduleMonth.SelectedValue = obj.SwapScheduleMonth;
            }
        }

        private void SetForm()
        {
            string nextTaskOwnerID = null;
            FcdsHelper.FcdsApplyStatusCodeEnum? enumFcdsApplyStatusCode = null;
            GetNextProInfo(ref nextTaskOwnerID, ref enumFcdsApplyStatusCode);

            HrVEgEmploy emp = new HrVEgEmploy(nextTaskOwnerID);
            switch (this.BasePageModeEnum)
            {
                case PageMode.PageModeEnum.View:
                    this.checkApprove.Visible = false;
                    this.checkSave.Visible = false;
                    this.checkReturn.Visible = false;
                    this.checkRevoke.Visible = false;
                    this.btnApprove.Visible = false;
                    this.btnSave.Visible = false;
                    this.btnReturn.Visible = false;
                    this.btnRevoke.Visible = false;

                    //if (obj.ApplicantID == this.LoginSession.UserID)
                    //{
                    //    this.btnRevoke.Visible = true;
                    //    this.btnRevoke.Attributes.Add("OnClick", @"if (confirm('Are you sure to revoke?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else{return false;}");
                    //}
                    break;

                case PageMode.PageModeEnum.Task:
                    switch (this.proID)
                    {
                        case FcdsHelper.PRO_RESPONDENT:
                            this.checkApprove.Visible = false;
                            this.checkSave.Attributes.Add("class", "btn btn-primary-sp");
                            this.checkSave.InnerText = @"Confirm to Submit Crew Scheduling Dept.";
                            this.checkRevoke.Visible = false;

                            this.btnApprove.Visible = false;
                            this.btnSave.CssClass = @"btn btn-primary-sp";
                            this.btnSave.Text = @"Submit Crew Scheduling Dept.";
                            this.btnRevoke.Visible = false;

                            this.lblReturnTo.Text = @"[" + this.applicantCrewDb.ID + @" " + this.applicantCrewDb.PassportName + @"]";

                            this.btnSave.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
                            this.btnReturn.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
                            //this.btnSave.Attributes.Add("OnClick", @"if (Page_ClientValidate()) {if (confirm('Are you sure to Submit to Crew Scheduling Dept?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else{return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");
                            //this.btnReturn.Attributes.Add("OnClick", @"if (Page_ClientValidate()) {if (confirm('Are you sure to Return to [" + this.applicantCrewDb.ID + @" " + this.applicantCrewDb.PassportName + @"] ?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else{return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");
                            break;

                        case FcdsHelper.PRO_REVOKE:
                            this.checkApprove.Visible = false;
                            this.checkSave.Visible = false;
                            this.checkReturn.Visible = false;
                            this.checkRevoke.Visible = fcdsConfig.IsApplicantRevoke.HasValue && fcdsConfig.IsApplicantRevoke.Value;

                            this.btnApprove.Visible = false;
                            this.btnSave.Visible = false;
                            this.btnReturn.Visible = false;
                            this.btnRevoke.Visible = fcdsConfig.IsApplicantRevoke.HasValue && fcdsConfig.IsApplicantRevoke.Value;

                            if (this.btnRevoke.Visible) this.btnRevoke.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
                            //this.btnRevoke.Attributes.Add("OnClick", @"if (Page_ClientValidate()) {if (confirm('Are you sure to Revoke?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else{return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");
                            break;

                        case FcdsHelper.PRO_OP_STAFF:
                            this.checkApprove.Visible = false;
                            this.checkSave.Attributes.Add("class", "btn btn-primary-sp");
                            this.checkSave.InnerText = @"確認呈核";
                            this.spanSubmitTo.InnerText += @" " + emp.DepCDesc + @" " + emp.DisplayName + @" " + emp.PosCDesc + @"？";
                            this.checkReturn.Visible = false;
                            this.checkRevoke.Visible = false;

                            this.btnApprove.Visible = false;
                            this.btnSave.CssClass = @"btn btn-primary-sp";
                            this.btnSave.Text = @"呈核";
                            this.btnReturn.Visible = false;
                            this.btnRevoke.Visible = false;

                            this.btnSave.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
                            //this.btnSave.Attributes.Add("OnClick", @"if (Page_ClientValidate()) {if (confirm('Are you sure to Submit?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else{return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");
                            break;

                        case FcdsHelper.PRO_OP_MANAGER:
                        case FcdsHelper.PRO_OP_ASSISTANT_GENERAL_MANAGER:
                        case FcdsHelper.PRO_OP_GENERAL_MANAGER:
                            // btnApprove [批核]與 btnSave [呈核]
                            this.checkApprove.Attributes.Add("class", "btn btn-primary-sp");
                            this.checkSave.Visible = (this.proID != FcdsHelper.PRO_OP_GENERAL_MANAGER);
                            this.checkSave.InnerText = @"確認呈核";
                            this.spanSubmitTo.InnerText += @" " + emp.DepCDesc + @" " + emp.DisplayName + @" " + emp.PosCDesc + @"？";
                            this.checkReturn.Visible = false;
                            this.checkRevoke.Visible = false;

                            this.btnApprove.CssClass = @"btn btn-primary-sp";
                            this.btnSave.Visible = (this.proID != FcdsHelper.PRO_OP_GENERAL_MANAGER);
                            this.btnSave.Text = @"呈核";
                            this.btnReturn.Visible = false;
                            this.btnRevoke.Visible = false;

                            this.btnApprove.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
                            this.btnSave.Attributes.Add("OnClick", this.BusyBox1.ShowFunctionCall);
                            //this.btnApprove.Attributes.Add("OnClick", @"if (Page_ClientValidate()) {if (confirm('Are you sure to Approve?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else{return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");
                            //this.btnSave.Attributes.Add("OnClick", @"if (Page_ClientValidate()) {if (confirm('Are you sure to Submit?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else{return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");
                            break;
                    }
                    break;
            }
        }

        private void BindUserControls()
        {
            BindApplyRosterUserControl();
            BindApplyApproveUserControl();

            this.ApprovalHistory1.ApprovalHistories = ApprovalHistory.GetApprovalHistoryCollection(this.BaseFormName, obj.FetchApprovalHistory(false));
            this.ApprovalHistory1.IsShowDurationField = true;

            this.ChangeHistory1.BasePageModeEnum = this.BasePageModeEnum;
            this.ChangeHistory1.ModuleName = this.BaseModuleName;
            this.ChangeHistory1.FormName = this.BaseFormName;
            this.ChangeHistory1.IDObject = obj.IDFcdsApply;
            this.ChangeHistory1.Object = obj;
        }

        /// <summary>
        /// 繫結勾選飛航組員任務換班申請之班表 UserControl。
        /// </summary>
        private void BindApplyRosterUserControl()
        {
            this.FcdsApplyRoster1.BasePageModeEnum = this.BasePageModeEnum;
            this.FcdsApplyRoster1.ApplicantCrewDb = this.applicantCrewDb;
            this.FcdsApplyRoster1.RespondentCrewDb = this.respondentCrewDb;
            this.FcdsApplyRoster1.FcdsApply = this.obj;
            this.FcdsApplyRoster1.FcdsApplyRosters = this.fcdsApplyRosters;
            this.FcdsApplyRoster1.BindRepeaterList();
        }

        /// <summary>
        /// 繫結勾選飛航組員任務換班申請主表單組員派遣部審核意見 UserControl。
        /// </summary>
        private void BindApplyApproveUserControl()
        {
            this.FcdsApplyApprove1.BasePageModeEnum = this.BasePageModeEnum;
            this.FcdsApplyApprove1.FcdsApply = obj;
            this.FcdsApplyApprove1.FcdsApplyApprove = fcdsApplyApprove;
            this.FcdsApplyApprove1.BusyBox = this.BusyBox1;
        }

        /// <summary>
        /// 計算須顯示之日期範圍索引鍵和值的集合物件(Dictionary<double, DateTime>)。TODO: 可考慮獨立一申請單輔助類別之方法   
        /// </summary>
        /// <returns></returns>
        private Dictionary<double, DateTime> CalculateRosterDates(string swapMonth)
        {
            Dictionary<double, DateTime> dic = new Dictionary<double, DateTime>();

            if (string.IsNullOrEmpty(swapMonth) || !int.TryParse(swapMonth, out _))
            {
                logger.Error(@"swapMonth=" + swapMonth + @" is invalid.");
                return dic;
            }

            DateTime baseDate = new DateTime(1980, 01, 01);

            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int difference = daysInMonth - DateTime.Now.Day;
            if (this.SwapScheduleMonth.SelectedValue == DateTime.Now.ToString("yyyy/MM"))
            {
                // [互換月份]選當月則當日回推 15 日至當月底
                //for (int i = -15; i <= difference; i++)
                //{
                //    dic.Add(new TimeSpan(DateTime.Parse(DateTime.Now.AddDays(i).ToString("yyyy/MM/dd")).Ticks - baseDate.Ticks).TotalDays, DateTime.Now.AddDays(i));
                //    dateTimes.Add(DateTime.Parse(DateTime.Now.AddDays(i).ToString("yyyy/MM/dd")));
                //}
            }
            else if (this.SwapScheduleMonth.SelectedValue == DateTime.Now.AddMonths(1).ToString("yyyy/MM"))
            {
                // [互換月份]選次月則當日回推 15 日至次月底
                difference += DateTime.DaysInMonth(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month);
                //for (int i = -15; i <= daysInMonth; i++)
                //{
                //    dic.Add(new TimeSpan(DateTime.Parse(DateTime.Now.AddDays(i).ToString("yyyy/MM/dd")).Ticks - baseDate.Ticks).TotalDays, DateTime.Now.AddDays(i));
                //    dateTimes.Add(DateTime.Parse(DateTime.Now.AddDays(i).ToString("yyyy/MM/dd")));
                //}
            }
            // 20221108 648267:取得當日~申請月份+10天
            //difference += 15; // 判斷飛航組員已發佈班表之截止日期，不論當月底或次月底再多推 15 天，以便擷取到跨月天數長之 Crew Route
            difference += 10;
            //for (int i = -15; i <= difference; i++)
            for (int i = 0; i <= difference; i++)
            {
                dic.Add(new TimeSpan(DateTime.Parse(DateTime.Now.AddDays(i).ToString("yyyy/MM/dd")).Ticks - baseDate.Ticks).TotalDays, 
                        DateTime.Parse(DateTime.Now.AddDays(i).ToString("yyyy/MM/dd")));
            }

            return dic;
        }

        /// <summary>
        /// 依據 HRM 新差勤系統之法定假日資料與機隊職級設定(FcdsConfig)之前置工作天數(LeadWorkdays)資料，計算可換班日期起始日期。
        /// </summary>
        /// <param name="dicDates">須顯示之日期範圍索引鍵和值的集合物件(Dictionary<double, DateTime>)</param>
        /// <returns>回傳可換班起始日</returns>
        private DateTime? CalculateSwappableBeginDate(Dictionary<double, DateTime> dicDates)
        {
            if (fcdsConfig != null && fcdsConfig.LeadWorkdays.HasValue)
            {
                DateTime? swappableBeginDate = DateTimeUtility.CalculateNextWorkingDay(@"TW", DateTime.Now, dicDates.Last().Value, fcdsConfig.LeadWorkdays);
                return swappableBeginDate;
            }

            //HrmAttendCalendarHolidayCollection col = HrmAttendCalendarHoliday.FetchHolidayByBeginAndEndDate(DateTime.Now.Date, dicDates.Last().Value, @"TW", Library.Component.Enums.ReturnObjectTypeEnum.Collection) as HrmAttendCalendarHolidayCollection;

            //if (fcdsConfig != null && fcdsConfig.LeadWorkdays.HasValue)
            //{
            //    if (col.Find(o => o.CalnDt == DateTime.Now.AddDays(fcdsConfig.LeadWorkdays.Value).Date) == null)
            //    {
            //        return DateTime.Now.AddDays(fcdsConfig.LeadWorkdays.Value).Date;
            //    }
            //    else
            //    {
            //        int indexDay = 1;
            //        int countWorkdays = 0;
            //        while (countWorkdays != fcdsConfig.LeadWorkdays)
            //        {
            //            if (col.Find(o => o.CalnDt == DateTime.Now.AddDays(indexDay).Date) == null)
            //                countWorkdays++;

            //            indexDay++;
            //        }

            //        return DateTime.Now.AddDays(fcdsConfig.LeadWorkdays.Value + indexDay).Date;
            //    }
            //}

            return null;
        }

        /// <summary>
        /// [批核]按鈕 Click 事件，OP_MANAGER、OP_ASSISTANT_GENERAL_MANAGER、OP_GENERAL_MANAGER 關卡啟用。
        /// 完成批核飛航組員任務互換申請系統申請主表單。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (SaveObject(null, null, FcdsHelper.FcdsApplyStatusCodeEnum.RELEASED, true))
            {
                ShowSaveCompleteModal(this.btnApprove.Text);

                //20221107 648267:After Approve redirect the page
                this.Redirect("ReviewFcdsApply_List.aspx");
                //string script = @"alert('Approve successfully!'); window.location='ReviewFcdsApply_List.aspx';";
                //this.RunJavascript(script);
            }
            else
            {
                // this.Alert(@"Approve failed! Please try again!");
                this.Alert("Something went wrong, please check your submission.");
                this.Redirect("ReviewFcdsApply_List.aspx");
            }

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        /// <summary>
        /// [呈核]按鈕 Click 事件，RESPONDENT(受申請人審核中)、OP_STAFF(承辦人審核中)、OP_MANAGER(組長審核中)、
        /// OP_ASSISTANT_GENERAL_MANAGER(副理審核中)、OP_GENERAL_MANAGER(經理審核中) 關卡啟用。
        /// 將飛航組員任務互換申請系統申請主表單呈核至下一個關卡(NextProID)。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string nextTaskOwnerID = null;
            FcdsHelper.FcdsApplyStatusCodeEnum? enumFcdsApplyStatusCode = null;
            GetNextProInfo(ref nextTaskOwnerID, ref enumFcdsApplyStatusCode);

            FlowStatus fs = FlowStatus.FetchByProID(this.BaseModuleName, this.BaseFormName, this.proID);
            if (SaveObject(fs.NextProID, nextTaskOwnerID, enumFcdsApplyStatusCode.Value, true))
            {
                ShowSaveCompleteModal(this.btnSave.Text);
                Server.Transfer("ReviewFcdsApply_List.aspx");
                //string script = @"alert('Submit successfully!'); window.location='ReviewFcdsApply_List.aspx';";
                //this.RunJavascript(script);
            }
            else
            {
                this.Alert(@"Something went wrong, please check your submission.");
                Server.Transfer("ReviewFcdsApply_List.aspx");
            }

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        /// <summary>
        /// 擷取下一個流程關卡人員與表單狀態列舉型態資訊。
        /// </summary>
        /// <param name="nextTaskOwnerID">下一個流程關卡人員員工編號</param>
        /// <param name="enumFcdsApplyStatusCode">下一個流程關卡表單狀態列舉型態</param>
        private void GetNextProInfo(ref string nextTaskOwnerID, ref FcdsHelper.FcdsApplyStatusCodeEnum? enumFcdsApplyStatusCode)
        {
            ModuleFormRoleTeamCollection col = ModuleFormRoleTeam.FetchByIDFlowTeam(this.BaseModuleName, this.BaseFormName, obj.IDFcdsApply, Library.Component.Enums.ReturnObjectTypeEnum.Collection) as ModuleFormRoleTeamCollection;
            switch (this.proID)
            {
                case FcdsHelper.PRO_RESPONDENT:
                    // 送交組派部 OP 承辦人
                    nextTaskOwnerID = col.Find(o => o.IDRole == @"068D_0").EmployID;
                    enumFcdsApplyStatusCode = FcdsHelper.FcdsApplyStatusCodeEnum.OP_STAFF;
                    break;

                case FcdsHelper.PRO_OP_STAFF:
                    // 呈核 OP 組長
                    nextTaskOwnerID = col.Find(o => o.IDRole == @"068D_1").EmployID;
                    enumFcdsApplyStatusCode = FcdsHelper.FcdsApplyStatusCodeEnum.OP_MANAGER;
                    break;

                case FcdsHelper.PRO_OP_MANAGER:
                    // 呈核 OP 副理
                    nextTaskOwnerID = col.Find(o => o.IDRole == @"066C_0").EmployID;
                    enumFcdsApplyStatusCode = FcdsHelper.FcdsApplyStatusCodeEnum.OP_ASSISTANT_GENERAL_MANAGER;
                    break;

                case FcdsHelper.PRO_OP_ASSISTANT_GENERAL_MANAGER:
                    // 呈核 OP 經理
                    nextTaskOwnerID = col.Find(o => o.IDRole == @"066C_1").EmployID;
                    enumFcdsApplyStatusCode = FcdsHelper.FcdsApplyStatusCodeEnum.OP_GENERAL_MANAGER;
                    break;

                case FcdsHelper.PRO_OP_GENERAL_MANAGER:
                    enumFcdsApplyStatusCode = FcdsHelper.FcdsApplyStatusCodeEnum.RELEASED;
                    break;
            }
        }

        /// <summary>
        /// [收回]按鈕 Click 事件。
        /// 飛航組員任務換班申請系統申請單申請人將收回申請主表單。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRevoke_Click(object sender, EventArgs e)
        {
            if (SaveObject(null, null, FcdsHelper.FcdsApplyStatusCodeEnum.REVOKE, true))
            {
                ShowSaveCompleteModal(this.btnRevoke.Text);

                //string script = @"alert('Revoke successfully!'); window.location='TaskList.aspx';";
                //this.RunJavascript(script);
            }

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        /// <summary>
        /// [退回]按鈕 Click 事件。
        /// 飛航組員任務換班申請系統申請單受申請人退回申請單至申請人。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            FlowStatus fs = new FlowStatus(this.BaseModuleName, this.BaseFormName, FcdsHelper.FcdsApplyStatusCodeEnum.RESPONDENT_RETURN.ToString());
            //fs.NextProID, obj.ApplicantID
            if (SaveObject(null, null, FcdsHelper.FcdsApplyStatusCodeEnum.RESPONDENT_RETURN, true))
            {
                ShowSaveCompleteModal(this.btnReturn.Text);

                //string script = @"alert('Return successfully!'); window.location='TaskList.aspx';";
                //this.RunJavascript(script);
            }

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        private void ShowSaveCompleteModal(string buttonText)
        {
            this.RunJavascript(@"$('#checkSaveCompleteDialogLabel').text('" + buttonText + @"完成'); $('#divSaveComplete').text('表單" + buttonText + @"完成！'); $('#checkSaveCompleteDialogModal').modal('show');");
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), @"script_modal", @"$('#checkSaveCompleteDialogModal').modal('show');", true);
        }

        /// <summary>
        /// 判斷主表單是否通過驗證。
        /// </summary>
        private bool IsPagePassValidation(bool isNeedValidateUserControl)
        {
            //if (!IsRespondentIDPassValidation()) return false;

            string alert = null;

            if (this.BasePageModeEnum != PageMode.PageModeEnum.Create)
            {
                if (string.IsNullOrEmpty(this.IDFcdsApply.Text))
                {
                    alert += @"[" + this.lblIDFcdsApply.Text + @"] is required!\n";
                }

                if (string.IsNullOrEmpty(this.StatusCode.Text))
                {
                    alert += @"[" + this.lblStatusCode.Text + @"] is required!\n";
                }
            }

            if (string.IsNullOrEmpty(this.SwapScheduleMonth.SelectedValue))
            {
                alert += @"[" + this.lblSwapScheduleMonth.Text + @"] is required!\n";
            }

            // 檢核[受申請人 ID]之資料是否與顯示相符
            CIvvCrewDb respondentCrewDb = new CIvvCrewDb(this.RespondentID.Text.Trim());
            string respondentInfo = respondentCrewDb.ID + @" / " + respondentCrewDb.PassportName + @"(" + respondentCrewDb.GetGroupByFleetCode() + @" - " + respondentCrewDb.GetGroupByCrewPosCode() + @")";
            //if (this.RespondentInfo.Text != respondentInfo)
            //{
            //    alert += @"[" + this.lblRespondentID.Text + @"] is not equal to the value! Please press the [Verify] button again!\n";
            //}

            if (isNeedValidateUserControl) alert += IsUserControlPassValidation();

            if (string.IsNullOrEmpty(alert))
            {
                return true;
            }
            else
            {
                this.Alert(alert);
                return false;
            }
        }

        private string IsUserControlPassValidation()
        {
            string alert = null;

            this.FcdsApplyRoster1.BasePageModeEnum = this.BasePageModeEnum;

            alert += this.FcdsApplyRoster1.ValidateUserControl();
            alert += this.FcdsApplyApprove1.ValidateUserControl();

            return alert;
        }

        private bool SaveObject(string nextProID, string taskOwnerID, FcdsHelper.FcdsApplyStatusCodeEnum enumFcdsApplyStatusCode, bool isSaveLog)
        {
            SetObjValue(nextProID, taskOwnerID, enumFcdsApplyStatusCode);
            if (obj.Save(this.BasePageModeEnum, isSaveLog, true))
            {
                this.FcdsApplyRoster1.BasePageModeEnum = this.BasePageModeEnum;
                this.FcdsApplyRoster1.FcdsApply = obj;
                if (!this.FcdsApplyRoster1.UpdateApplyRoster(isSaveLog))
                {
                    return false;
                }

                this.FcdsApplyApprove1.BasePageModeEnum = this.BasePageModeEnum;
                this.FcdsApplyApprove1.FcdsApply = obj;
                if (!this.FcdsApplyApprove1.SaveApplyApprove(isSaveLog))
                {
                    return false;
                }
            }

            if (obj.StatusCodeEnum == FcdsHelper.FcdsApplyStatusCodeEnum.RELEASED) 
            {
                // 審核完畢通知
                return obj.SendReleaseMail();
            }
            else 
            {
                // 簽核通知
                return obj.SendMailToTeamMember(this.proID);
            }
        }

        /// <summary>
        /// 設定表單物件屬性值。
        /// </summary>
        private void SetObjValue(string nextProID, string taskOwnerID, FcdsHelper.FcdsApplyStatusCodeEnum enumFcdsApplyStatusCode)
        {
            switch (this.proID)
            {
                case FcdsHelper.PRO_REVOKE:
                    obj.IsApplicantRevoke = true;
                    break;

                default:
                    break;
            }

            obj.ProID = nextProID;
            obj.TaskOwnerID = taskOwnerID;
            obj.StatusCode = enumFcdsApplyStatusCode.ToString();
            obj.StatusCodeEnum = enumFcdsApplyStatusCode;
            obj.DisplayStatus = new FlowStatus(this.BaseModuleName, this.BaseFormName, obj.StatusCode).DisplayStatus;
            obj.UpdateBy = this.LoginSession.UserID;
            obj.UpdateStamp = DateTime.Now;
        }

        //private bool IsRespondentIDPassValidation()
        //{
        //    this.cvRespondentID.Validate();
        //    this.revRespondentID.Validate();
        //    this.rfvRespondentID.Validate();

        //    return this.cvRespondentID.IsValid && this.revRespondentID.IsValid && this.rfvRespondentID.IsValid;
        //}

    }
}