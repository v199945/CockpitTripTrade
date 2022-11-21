using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using log4net;

using Library.Component.BLL;
using Library.Component.Utility;
using Library.Module.FCDS;
using Library.Module.FCDS.Application;
using Library.Module.FCDS.Configuration;
using Library.Module.FZDB;

namespace CockpitTripTrade.Module.Application
{
    public partial class CockpitTripTrade_Module_Application_NewFcdsApply : ApplicationPage
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CockpitTripTrade_Module_Application_FcdsApply));
        private Stopwatch sw = new Stopwatch();

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
        /// 飛航組員任務換班申請系統主表單物件。
        /// </summary>
        private FcdsApply obj;

        /// <summary>
        /// 飛航組員任務換班申請系統表單流程編號。
        /// </summary>
        private string proID;

        /// <summary>
        /// 勾選飛航組員任務換班申請之班表 UserControl 中須顯示之日期範圍索引鍵和值的集合物件。
        /// </summary>
        private Dictionary<double, DateTime> rosterDates;

        /// <summary>
        /// 飛航組員欲申請任務換班之月份。
        /// </summary>
        private string swapMonth;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            this.BaseFormName = @"FcdsApply";
            ModuleForm moduleForm = new ModuleForm(this.BaseModuleName, this.BaseFormName);
            this.BaseFormTitle = moduleForm.FormTitle;

            string idFcdsApply = Request.QueryString["IDFcdsApply"];
            swapMonth = Request.QueryString["SwapMonth"]; // 飛航組員欲申請任務換班之月份
            obj = new FcdsApply(idFcdsApply);

            if (!string.IsNullOrEmpty(idFcdsApply))
            {
                this.BasePageModeEnum = PageMode.PageModeEnum.Edit;
            }
            else
            {
                this.BasePageModeEnum = PageMode.PageModeEnum.Create;
            }

            switch (this.BasePageModeEnum)
            {
                case PageMode.PageModeEnum.Create:
                    this.proID = moduleForm.InitialProID;
                    this.applicantCrewDb = this.LoginSession.CrewDb;
                    break;

                case PageMode.PageModeEnum.Edit:
                    this.proID = moduleForm.InitialProID;
                    this.applicantCrewDb = new CIvvCrewDb(obj.ApplicantID);
                    this.respondentCrewDb = new CIvvCrewDb(obj.RespondentID);
                    break;

                default:
                    this.proID = moduleForm.ProID;
                    break;
            }

            string ac = this.applicantCrewDb.GetGroupByAcID();
            int pos = this.applicantCrewDb.GetGroupByPosID();
            this.fcdsConfig = new FcdsConfig(ac, pos);

            if (Page.IsPostBack)
            {
                //BindUserControls();
            }
            else
            {
                if (this.BasePageModeEnum == PageMode.PageModeEnum.Create)
                {
                    VerifyCreateApplyForm(swapMonth, true);
                }

                InitForm();
            }

            this.RespondentID.Focus();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            //ModuleFormRight.SetModuleFormRight(this.Page, this.BaseModuleName, this.BaseFormName, this.proID, this.BasePageModeEnum);
        }

        private void InitForm()
        {
            BindForm();
            SetForm();
        }

        /// <summary>
        /// 檢核飛航組員當月申請次數與是否有流程中(一次一單否)之申請單。
        /// </summary>
        /// <param name="isNeedRedirect">檢核不通過是否須重新導向至 TaskList.aspx 頁面</param>
        private bool VerifyCreateApplyForm(string swapMonth, bool isNeedRedirect)
        {
            List<string> vs = FcdsApply.GetAllowCreateString(applicantCrewDb.ID, swapMonth, fcdsConfig);
            // 20221102 648267:更正邏輯
            //if (string.IsNullOrEmpty(swapMonth) || ((swapMonth != DateTime.Now.ToString("yyyyMM")) || (swapMonth == DateTimeUtility.GetTheBeginOfMonth(DateTime.Now.AddMonths(1)).ToString("yyyyMM") && this.LoginSession.CrewDb.CIvvRosterId.PublishDate < DateTimeUtility.GetTheBeginOfMonth(DateTime.Now.AddMonths(1)))))
            //{
            //    // 若 [swapMonth] 參數為 NULL 值或空字串、或不為當月份、或為次月份且飛航組員發佈班表最後日期小於次月一日，則為不合法參數
            //    vs.Add(@"The month you would like to swap is invalid!");
            //}
            string nextMonthString = DateTimeUtility.GetTheBeginOfMonth(DateTime.Now.AddMonths(1)).ToString("yyyyMM");
            if (string.IsNullOrEmpty(swapMonth))
            {
                vs.Add(@"The month selected not exist");
            }
            if (swapMonth != DateTime.Now.ToString("yyyyMM") && swapMonth != nextMonthString)
            {
                vs.Add($"You should be selecting {DateTime.Now.ToString("yyyyMM")} or {DateTimeUtility.GetTheBeginOfMonth(DateTime.Now.AddMonths(1)).ToString("yyyyMM")}");
            }
            if (swapMonth == nextMonthString && this.LoginSession.CrewDb.CIvvRosterId.PublishDate < DateTimeUtility.GetTheBeginOfMonth(DateTime.Now.AddMonths(1)))
            {
                vs.Add($"Your schedule of flight has yet to be published.");
            }


            if (vs.Count > 0)
            {
                string script = @"alert('" + string.Join(@"\n", vs.ToArray()) + @"');";
                if (isNeedRedirect)
                {
                    script += @" window.location='TaskList.aspx';";
                    Response.Write(@"<script type=""text/javascript"">" + script + @"</script>");
                    Response.End();
                }
                else
                {
                    this.RunJavascript(script);
                }

                return false;
            }

            return true;
        }

        private void BindForm()
        {
            if (this.BasePageModeEnum == PageMode.PageModeEnum.Create)
            {
                this.ApplicationDate.Text = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
                this.cvRespondentID.ValueToCompare = applicantCrewDb.ID;
            }
            else
            {
                if (obj.ApplicationDate.HasValue)
                {
                    this.ApplicationDate.Text = obj.ApplicationDate.Value.ToString("ddMMMyyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
                }

                this.cvRespondentID.ValueToCompare = obj.ApplicantID;

                respondentCrewDb = new CIvvCrewDb(obj.RespondentID);
                this.RespondentID.Text = respondentCrewDb.ID + @"/" + respondentCrewDb.PassportName + @"(" + respondentCrewDb.GetGroupByFleetCode() + @" - " + respondentCrewDb.GetGroupByCrewPosCode() + @")";
                this.btnQryRespondentID.Visible = false;
            }

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
                DateTime monthStartDate = DateTimeUtility.GetTheBeginOfMonth(DateTime.Now);
                DateTime monthEndDate = DateTimeUtility.GetTheEndOfMonth(DateTime.Now.Date);

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
                    this.SwapScheduleMonth.Items.Add(new ListItem(dt.ToString("MMM yyy"), item));
                }

                this.SwapScheduleMonth.SelectedValue = obj.SwapScheduleMonth;
            }
        }

        private void SetForm()
        {
            this.RespondentID.Attributes.Add("OnInput", @"javascript:ValidatorOnChange(event); CheckRespondentID();");

            //this.cstvRespondentID.Attributes.Add("Style", @"white-space: nowrap"); // 強制文字在同一行，除非遇到 <br> 換行標籤 window.onbeforeunload=null; 
            this.btnQryRespondentID.Attributes.Add("OnClick", @"javascript:Page_ClientValidate('Respondent'); CheckRespondentID(); " + this.BusyBox1.ShowFunctionCall);
            this.btnSelectRrespondentMonthNext.Attributes.Add("OnClick", @"javascript:Page_ClientValidate('Respondent'); CheckRespondentID(); if (Page_ClientValidate()) {" + this.BusyBox1.ShowFunctionCall + @" return true;} else {/*RequiredFieldValidator_CheckAllValidControl(Page_Validators);*/}");

            //  && CheckRespondentInfo()
            this.btnSave.Attributes.Add("OnClick", @"if (Page_ClientValidate()) {" + this.BusyBox1.ShowFunctionCall + @"} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");
            //this.btnSave.Attributes.Add("OnClick", @"if (Page_ClientValidate()) {if (confirm('Are you sure to Submit?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else{return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");
            //this.btnCancel.Attributes.Add("OnClick", @"if (confirm('Are you sure to exit this page?')) {window.onbeforeunload = null; return true;} else{return false;}");
        }

        /// <summary>
        /// 繫結勾選飛航組員任務換班申請之班表 UserControl。
        /// </summary>
        private void BindApplyRosterUserControl()
        {
            SetObjValue();

            this.FcdsApplyRoster1.BasePageModeEnum = this.BasePageModeEnum;
            this.FcdsApplyRoster1.ApplicantCrewDb = this.applicantCrewDb;
            this.FcdsApplyRoster1.RespondentCrewDb = this.respondentCrewDb;
            this.FcdsApplyRoster1.FcdsApply = obj;

            sw.Start();
            DataTable dtApplicant = CIRoster.FetchApplyRoster(this.applicantCrewDb.ID, this.rosterDates.First().Value, this.rosterDates.Last().Value);
            DataTable dtRespondent = CIRoster.FetchApplyRoster(this.respondentCrewDb.ID, this.rosterDates.First().Value, this.rosterDates.Last().Value);
            sw.Stop();
            Log log = Log.GetLogWithLoginSuccessfully(@"FcdsApply.BindApplyRosterUserControl() cost", sw.Elapsed.TotalSeconds.ToString() + @" seconds", true);
            log.Save(PageMode.PageModeEnum.Create);
            logger.Info(@"FcdsApply.BindApplyRosterUserControl() Fetch Roster Data of Applicant and Respondent cost=" + sw.Elapsed.TotalSeconds + @" seconds");
            Debug.WriteLine(@"FcdsApply.BindApplyRosterUserControl() Fetch Roster Data of Applicant and Respondent cost=" + sw.Elapsed.TotalSeconds + @" seconds");
            sw.Reset();

            sw.Start();
            this.FcdsApplyRoster1.FcdsApplyRosters = FcdsHelper.ProcessRosterData(dtApplicant, dtRespondent, this.rosterDates, this.applicantCrewDb, this.respondentCrewDb);
            sw.Stop();
            log = Log.GetLogWithLoginSuccessfully(@"FcdsHelper.ProcessRosterData() cost", sw.Elapsed.TotalSeconds.ToString() + @" seconds", true);
            log.Save(PageMode.PageModeEnum.Create);
            sw.Reset();

            this.FcdsApplyRoster1.BindRepeaterList();
        }

        /// <summary>
        /// 計算須呈現日期集合物件。
        /// </summary>
        /// <returns></returns>
        private Dictionary<double, DateTime> CalculateRosterDates(string swapMonth)
        {
            Dictionary<double, DateTime> dic = new Dictionary<double, DateTime>();
            DateTime baseDate = new DateTime(1980, 01, 01);
            DateTime? beginDate = null;

            List<DateTime> dateTimes = new List<DateTime>();
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int difference = daysInMonth - DateTime.Now.Day;
            if (this.SwapScheduleMonth.SelectedValue == DateTime.Now.ToString("yyyyMM"))
            {
                // [互換月份]選當月則當日回推 15 日至當月底
                beginDate = DateTime.Now;
            }
            else if (this.SwapScheduleMonth.SelectedValue == DateTime.Now.AddMonths(1).ToString("yyyyMM"))
            {
                // [互換月份]選次月則當日回推 15 日至次月底
                difference += DateTime.DaysInMonth(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month);
                beginDate = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1);
            }
            
            //difference += 15; // 判斷飛航組員已發佈班表之截止日期，若不論當月底或次月底再多推 15 天，以便擷取跨月天數長之 Crew Route
            // 20221108 648267:取得當日~申請月份+10天
            difference += 10;
            if (beginDate.HasValue)
            {
                //for (int i = -15; i <= difference; i++)
                for (int i = 0; i <= difference; i++)
                {
                    dic.Add(new TimeSpan(DateTime.Parse(beginDate.Value.AddDays(i).ToString("yyyy/MM/dd")).Ticks - baseDate.Ticks).TotalDays, 
                            DateTime.Parse(beginDate.Value.AddDays(i).ToString("yyyy/MM/dd")));
                    dateTimes.Add(DateTime.Parse(beginDate.Value.AddDays(i).ToString("yyyy/MM/dd")));
                }
            }
            else
            {
                logger.Error(@"NewFcdsApply.CalculateRosterDates() Exception, beginDate is null, swapMonth=" + swapMonth);
            }

            return dic;
        }

        /// <summary>
        /// 依據 HRM 新差勤系統之法定假日資料與機隊職級設定(FcdsConfig)之前置工作天數(LeadWorkdays)資料，取得可換班日期起始日期。
        /// </summary>
        /// <param name="datetimes">須顯示之日期範圍索引鍵和值的集合物件(Dictionary<double, DateTime>)</param>
        /// <returns></returns>
        private DateTime? CalculateSwappableBeginDate(Dictionary<double, DateTime> dicDates)
        {
            if (fcdsConfig != null && fcdsConfig.LeadWorkdays.HasValue)
            {
                DateTime? swappableBeginDate = DateTimeUtility.CalculateNextWorkingDay(@"TW", DateTime.Now, dicDates.Last().Value, fcdsConfig.LeadWorkdays);
                return swappableBeginDate;
            }

            return null;
        }

        protected void btnQryRespondentID_Click(object sender, EventArgs e)
        {
            if (!IsRespondentIDPassValidation()) return;

            this.RespondentInfo.Text = string.Empty;
            string alert = null;
            if (string.IsNullOrEmpty(this.RespondentID.Text.Trim()) || this.RespondentID.Text.Trim().Length != 6 || !int.TryParse(this.RespondentID.Text.Trim(), out _))
            {
                alert += @"[" + this.lblRespondentID.Text + @"] is required!\n";
            }

            if (this.RespondentID.Text.Trim() == applicantCrewDb.ID)
            {

                alert += @"[" + this.lblRespondentID.Text + @"] cannot be equal to yourself!";

            }

            respondentCrewDb = new CIvvCrewDb(this.RespondentID.Text.Trim());
            if (string.IsNullOrEmpty(respondentCrewDb.ID) || respondentCrewDb.CIvvRosterId.ActTypEnum != CIvvRosterIdActTypEnum.Active)
            {
                string msg = @"[" + this.lblRespondentID.Text + @"] is invalid!";
                this.RespondentInfo.Text = msg;
                alert += msg + @"\n";
                //this.cstvRespondentID.ErrorMessage = msg;
                //this.cstvRespondentID.IsValid = false;
            }
            else
            {
                if (LoginSession.CrewDb.GetGroupByAcID() != respondentCrewDb.GetGroupByAcID())
                {
                    string msg = @"[" + this.lblRespondentID.Text + @"] - " + this.RespondentID.Text + @"'s fleet is not equal to yours!";
                    this.RespondentInfo.Text += msg + @"<br />";
                    //this.cstvRespondentID.ErrorMessage += msg + @"<br />";
                    alert += msg + @"\n";
                }
                // 20221116 648267:OP蕭博元跟工會開會決議讓相同機隊FO跟RP可以互換
                if ( LoginSession.CrewDb.GetGroupByPosID() != respondentCrewDb.GetGroupByPosID() && 
                    ( LoginSession.CrewDb.GetGroupByPosID() == 1 || respondentCrewDb.GetGroupByPosID() == 1))
                {
                    string msg = @"[" + this.lblRespondentID.Text + @"] - " + this.RespondentID.Text + @"'s rank is not equal to yours!";
                    this.RespondentInfo.Text += msg + @"<br />";
                    //this.cstvRespondentID.ErrorMessage += msg + @"<br />";
                    alert += msg + @"\n";
                }
                // 20221103 648267:Check whether this respondent allow others to see their duties
                if (respondentCrewDb.AllowQuery == "0")
                {
                    string msg = $"[{this.lblRespondentID.Text}]-{this.RespondentID.Text} does not allow others to access his/her schedule of eCrew.";
                    this.RespondentInfo.Text += msg + "<br />";
                    alert += msg + @"\n";
                }

                //this.cstvRespondentID.ErrorMessage.TrimEnd(new char[] { '<','b','r','/','>' });
            }

            if (!string.IsNullOrEmpty(alert))
            {
                SetRespondentIDInvalid();
                //this.RespondentInfo.CssClass = @"invalid-feedback";
                //this.RespondentInfo.Attributes.CssStyle.Add("display", "inline");
                //this.RespondentID.Focus();
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "Script_RespondentInvalid", "$('#" + this.RespondentID.ClientID + "').removeClass('is-valid');$('#" + this.RespondentID.ClientID + "').addClass('is-invalid');", true);

                this.Alert(alert);
            }
            else
            {
                this.RespondentInfo.CssClass = @"vdform-plaintext";
                this.RespondentInfo.Text = respondentCrewDb.ID + @" / " + respondentCrewDb.PassportName + @"(" + respondentCrewDb.GetGroupByFleetCode() + @" - " + respondentCrewDb.GetGroupByCrewPosCode() + @")";

                //SetObjValue();
            }

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        protected void btnSelectRrespondentMonthNext_Click(object sender, EventArgs e)
        {
            if (!IsRespondentIDPassValidation()) return;

            if (IsPagePassValidation(false))
            {
                BindApplyRosterUserControl();

                /*
                 * UpdatePanel Button PostBack 後，jQuery Script 失效解決方法
                 * 1. 在伺服器端程式執行完成後呼叫 ScriptManager.RegisterStartupScript(Control, Type, String, String, Boolean) 方法重新綁定
                 * 2. 在用戶端取得 Sys.WebForms.PageRequestManager.getInstance() 後呼叫 add_endRequest 方法重新綁定
                */
                ScriptManager.RegisterStartupScript(this.upNext, this.GetType(), @"BindSwitchGridJQuery", @"initSwitchGrid();", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), @"ShowTickDutiesStep", @"stepper.next();", true);

                CIvvCrewDb respondent = new CIvvCrewDb(obj.RespondentID);
                this.lblSubmitTo.Text = respondent.ID + @" " + respondent.PassportName;// this.RespondentInfo.Text;
            }

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (VerifyCreateApplyForm(swapMonth, false) && IsPagePassValidation(true) && SaveObject(true))
            {
                // 表單驗證通過且儲存主表單、飛航組員班表明細資料與團隊成員成功

                if (!ClientScript.IsStartupScriptRegistered(this.GetType(), @"ShowFinishedStep"))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), @"ShowFinishedStep", @"stepper.next();simulateSubmit();", true);
                    this.hlIDFcdsApply.NavigateUrl = @"FcdsApply.aspx?IDFcdsApply=" + obj.IDFcdsApply;
                    this.hlIDFcdsApply.Text = obj.IDFcdsApply;
                    this.lblRespondent.Text = this.respondentCrewDb.DisplayName;
                    this.Redirect(@"TaskList.aspx");
                    //ShowSaveCompleteModal(this.btnSave.Text);
                }
            }
            else
            {
                // 表單驗證未通過

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), @"StayTickDutiesStep", @"stepper.next();", true);
            }

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        /// <summary>
        /// 判斷主表單是否通過驗證。
        /// </summary>
        /// <param name="isNeedValidateUserControl">是否需要驗證 UserControl</param>
        /// <returns></returns>
        private bool IsPagePassValidation(bool isNeedValidateUserControl)
        {
            if (!IsRespondentIDPassValidation()) return false;

            string alert = null;

            if (string.IsNullOrEmpty(this.SwapScheduleMonth.SelectedValue))
            {
                alert += @"[" + this.lblSwapScheduleMonth.Text + @"] is required!\n";
            }

            // 檢核[受申請人 ID]之資料是否與顯示相符
            CIvvCrewDb respondentCrewDb = new CIvvCrewDb(this.RespondentID.Text.Trim());
            string respondentInfo = respondentCrewDb.ID + @" / " + respondentCrewDb.PassportName + @"(" + respondentCrewDb.GetGroupByFleetCode() + @" - " + respondentCrewDb.GetGroupByCrewPosCode() + @")";
            if (string.IsNullOrEmpty(this.RespondentInfo.Text))
            {
                alert += @"[" + this.lblRespondentID.Text + @"] has not been verified! Please click the [Verify] button again!\n";

                this.RespondentInfo.Text = @"[" + this.lblRespondentID.Text + @"] has not been verified! Please click the [Verify] button again!";
                SetRespondentIDInvalid();
            }
            else
            {
                if (this.RespondentInfo.Text != respondentInfo)
                {
                    alert += @"[" + this.lblRespondentID.Text + @"] is not equal to the value! Please click the [Verify] button again!\n";

                    SetRespondentIDInvalid();
                }
            }

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

        /// <summary>
        /// 判斷 UserControl 是否通過驗證。
        /// </summary>
        /// <returns></returns>
        private string IsUserControlPassValidation()
        {
            string alert = null;

            this.FcdsApplyRoster1.BasePageModeEnum = this.BasePageModeEnum;

            alert += this.FcdsApplyRoster1.ValidateUserControl();

            return alert;
        }

        /// <summary>
        /// 設定[受申請人 ID]欄位為不合法之 CSS Class。
        /// </summary>
        private void SetRespondentIDInvalid()
        {
            this.RespondentInfo.CssClass = @"invalid-feedback";
            this.RespondentInfo.Attributes.CssStyle.Add("display", "inline");
            this.RespondentID.Focus();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "Script_RespondentInvalid", "$('#" + this.RespondentID.ClientID + "').removeClass('is-valid');$('#" + this.RespondentID.ClientID + "').addClass('is-invalid');", true);
            this.upRespondentInfo.Update();
        }

        //public void cstvRespondentID_ServerValidate(object source, ServerValidateEventArgs args)
        //{
        //    if (string.IsNullOrEmpty(this.RespondentInfo.Text))
        //    {
        //        //this.RespondentInfo.Text = @"[" + this.lblRespondentID.Text + @"] has not been verified! Please click the [Verify] button again!";
        //        //this.upRespondentInfo.Update();

        //        args.IsValid = false;
        //    }
        //}

        private bool SaveObject(bool isSaveLog)
        {
            SetObjValue();

            // 儲存[提出申請]飛航組員任務互換申請系統申請主表單物件
            if (obj.Save(this.BasePageModeEnum, isSaveLog, true))
            {
                this.FcdsApplyRoster1.BasePageModeEnum = this.BasePageModeEnum;
                this.FcdsApplyRoster1.FcdsApply = obj;
                this.FcdsApplyRoster1.ApplicantCrewDb = this.applicantCrewDb;
                this.FcdsApplyRoster1.RespondentCrewDb = this.respondentCrewDb;
                if (!this.FcdsApplyRoster1.SaveApplyRoster(isSaveLog))
                {
                    return false;
                }

                obj.SendMailToTeamMember(this.proID);
            }

            return true;
        }

        /// <summary>
        /// 設定表單物件屬性值。
        /// </summary>
        private void SetObjValue()
        {
            obj.ApplicantID = applicantCrewDb.ID;
            obj.IDAcTypeApplicant = applicantCrewDb.GetGroupByAcID();
            obj.IDCrewPosApplicant = applicantCrewDb.GetGroupByPosID();
            obj.IDFcdsConfig = fcdsConfig.IDFcdsConfig;

            if (!string.IsNullOrEmpty(this.ApplicationDate.Text) && DateTime.TryParse(this.ApplicationDate.Text, out _))
            {
                obj.ApplicationDate = DateTime.Parse(this.ApplicationDate.Text);
            }

            obj.ApplicantPublishDate = applicantCrewDb.CIvvRosterId.PublishDate;
            obj.ApplicationDeadline = this.FcdsApplyRoster1.ApplicantMinTickRosterDate; // 取得申請人勾選之最小班表日期

            if (!string.IsNullOrEmpty(this.RespondentID.Text.Trim()))
            {
                this.respondentCrewDb = new CIvvCrewDb(this.RespondentID.Text.Trim());
                obj.RespondentID = this.respondentCrewDb.ID;
                obj.IDAcTypeRespondent = this.respondentCrewDb.GetGroupByAcID();
                obj.IDCrewPosRespondent = this.respondentCrewDb.GetGroupByPosID();
            }

            foreach (ListItem li in this.SwapScheduleMonth.Items)
            {
                obj.SwapScheduleMonthItems += li.Value + ",";
            }
            obj.SwapScheduleMonthItems = obj.SwapScheduleMonthItems.TrimEnd(',');

            if (this.SwapScheduleMonth.SelectedIndex != -1)
            {
                obj.SwapScheduleMonth = this.SwapScheduleMonth.SelectedValue;
                this.rosterDates = CalculateRosterDates(this.SwapScheduleMonth.SelectedValue);
                obj.SwappableBeginDate = CalculateSwappableBeginDate(this.rosterDates);
            }

            obj.LeadWorkdays = fcdsConfig.LeadWorkdays;
            obj.DeadlineOfAcrossMonth = fcdsConfig.DeadlineOfAcrossMonth;

            FlowStatus fs = FlowStatus.FetchByProID(this.BaseModuleName, this.BaseFormName, this.proID);
            obj.ProID = fs.NextProID;

            fs = FlowStatus.FetchByProID(this.BaseModuleName, this.BaseFormName, fs.NextProID);
            obj.StatusCode = fs.StatusCode;
            obj.StatusCodeEnum = Enum.TryParse<FcdsHelper.FcdsApplyStatusCodeEnum>(obj.StatusCode, out _) ? Enum.Parse(typeof(FcdsHelper.FcdsApplyStatusCodeEnum), obj.StatusCode) as FcdsHelper.FcdsApplyStatusCodeEnum? : null;
            obj.DisplayStatus = fs.DisplayStatus;

            obj.TaskOwnerID = this.respondentCrewDb.ID;

            obj.CreateBy = this.LoginSession.CrewDb.ID;
            obj.CreateStamp = DateTime.Now;
            obj.UpdateBy = this.LoginSession.CrewDb.ID;
            obj.UpdateStamp = DateTime.Now;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Redirect(@"TaskList.aspx");
        }

        private bool IsRespondentIDPassValidation()
        {
            this.cvRespondentID.Validate();
            this.revRespondentID.Validate();
            this.rfvRespondentID.Validate();

            return this.cvRespondentID.IsValid && this.revRespondentID.IsValid && this.rfvRespondentID.IsValid;
        }

        private void ShowSaveCompleteModal(string buttonText)
        {
            this.RunJavascript(@"$('#checkSaveCompleteDialogLabel').text('" + buttonText + @"完成'); $('#divSaveComplete').text('表單" + buttonText + @"完成！'); $('#checkSaveCompleteDialogModal').modal('show');");
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), @"script_modal", @"$('#checkSaveCompleteDialogModal').modal('show');", true);
        }

    }
}