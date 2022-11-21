﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using log4net;

using Library.Component.BLL;
using Library.Module.FCDS.Application;
using Library.Module.FZDB;

namespace CockpitTripTrade.Module.Application.UserControl
{
    public partial class UserControl_FcdsApplyRoster : ApplicationUserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UserControl_FcdsApplyRoster));
        private Stopwatch sw = new Stopwatch();

        /// <summary>
        /// 飛航組員班表日期索引鍵和值的集合物件。
        /// </summary>
        public Dictionary<double, DateTime> RosterDates { get; set; }

        /// <summary>
        /// 飛航組員申請人 CIvvCrewDb 物件。
        /// </summary>
        public CIvvCrewDb ApplicantCrewDb { get; set; }

        /// <summary>
        /// 飛航組員受申請人 CIvvCrewDb 物件。
        /// </summary>
        public CIvvCrewDb RespondentCrewDb { get; set; }

        public FcdsApply FcdsApply { get; set; }

        //public DateTime? SwappableBeginDate { get; set; }

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
                if (this.BasePageModeEnum == PageMode.PageModeEnum.Create && !string.IsNullOrEmpty(this.RespondentCrewDb.ID))
                {
                    sw.Start();
                    DataTable dtApplicant = CIRoster.FetchApplyRoster(this.ApplicantCrewDb.ID, this.RosterDates.First().Value, this.RosterDates.Last().Value);
                    DataTable dtRespondent = CIRoster.FetchApplyRoster(this.RespondentCrewDb.ID, this.RosterDates.First().Value, this.RosterDates.Last().Value);
                    sw.Stop();
                    logger.Info(@"FcdsApplyRoster.BindRepeaterList() Fetch Roster Data of Applicant and Respondent cost=" + sw.Elapsed.TotalSeconds + @" Sec");
                    Debug.WriteLine(@"FcdsApplyRoster.BindRepeaterList() Fetch Roster Data of Applicant and Respondent cost=" + sw.Elapsed.TotalSeconds + @" Sec");
                    sw.Reset();

                    this.repSchedule.DataSource = ProcessRosterData(dtApplicant, dtRespondent);

                    sw.Start();
                    this.repSchedule.DataBind();
                    sw.Stop();
                    logger.Info(@"FcdsApplyRoster.repSchedule_ItemDataBound() Bind Repeater cost=" + sw.Elapsed.TotalSeconds + @" Sec");
                    Debug.WriteLine(@"FcdsApplyRoster.repSchedule_ItemDataBound() Bind Repeater cost=" + sw.Elapsed.TotalSeconds + @" Sec");
                    sw.Reset();
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

        private FcdsApplyRosterCollection ProcessRosterData(DataTable dtApplicant, DataTable dtRespondent)
        {
            DataTable dtTickRoster = new DataTable("dtTickRoster");

            FcdsApplyRosterCollection fcdsApplyRosters = new FcdsApplyRosterCollection();
            foreach (KeyValuePair<double, DateTime> item in this.RosterDates)
            {
                FcdsApplyRoster ar = new FcdsApplyRoster() { RosterDate = item.Value, ApplicantDutyDay = int.Parse(item.Key.ToString()), RespondentDutyDay = int.Parse(item.Key.ToString()) };
                fcdsApplyRosters.Add(ar);
            }

            #region Process Applicant Roster Data
            for (int i = 0; i < dtApplicant.Rows.Count; i++)
            {
                DataRow prevDataRow = i > 0 ? dtApplicant.Rows[i - 1] : null;
                DataRow currDataRow = dtApplicant.Rows[i];

                DateTime dutyStrDtTpe = currDataRow["Duty_Str_Dt_Tpe"] == DBNull.Value ? DateTime.Parse(currDataRow["Str_Dt_Tpe"].ToString()).Date : DateTime.Parse(currDataRow["Duty_Str_Dt_Tpe"].ToString()).Date;
                if (prevDataRow != null && dutyStrDtTpe > ApplicantCrewDb.CIvvRosterId.PublishDate && (currDataRow["DutyDay"].ToString() != prevDataRow["DutyDay"].ToString() || currDataRow["DutyNo"].ToString() != prevDataRow["DutyNo"].ToString()))
                {
                    // 若勤務起始日期大於申請者發佈班表最後日期，且當前資料列[DutyDay]與[DutyNo]欄位與上一資料列不相同時，則跳出迴圈
                    break;
                }
                
                FcdsApplyRoster fcdsApplyRoster = fcdsApplyRosters.Find(o => o.RosterDate == dutyStrDtTpe);
                if (fcdsApplyRoster != null)
                {
                    fcdsApplyRoster.ApplicantID = this.ApplicantCrewDb.ID;
                    fcdsApplyRoster.ApplicantDutyDay = int.Parse(currDataRow["DutyDay"].ToString());
                    fcdsApplyRoster.ApplicantDutyNo = long.Parse(currDataRow["DutyNo"].ToString());
                    fcdsApplyRoster.ApplicantNumOfLeg = int.Parse(currDataRow["Num_Of_Leg"].ToString());

                    // 若勤務起始日期(報到日期時間)或航機表訂起飛時間(STD，因同一報到報離第二段無報到時間，以航機表訂起飛時間)大於當下時間，
                    // 且[Is_Swappable]為 NULL 值(表示為飛航勤務)或[Is_Swappable]為 1 時，則此筆資料可啟用勾選
                    //fcdsApplyRoster.IsApplicantEnable =
                    //    (dutyStrDtTpe >= this.SwappableBeginDate) &&
                    //    (currDataRow["Is_Swappable"] == DBNull.Value || (currDataRow["Is_Swappable"] != DBNull.Value && currDataRow["Is_Swappable"].ToString().Equals("1")))
                    //     ? true : false;

                    if (fcdsApplyRoster.ApplicantRosterLegs == null) fcdsApplyRoster.ApplicantRosterLegs = new FcdsApplyRosterLegCollection();
                    FcdsApplyRosterLeg fcdsApplyRosterLeg = new FcdsApplyRosterLeg();
                    fcdsApplyRosterLeg.SetFcdsApplyRosterLeg(currDataRow);
                    fcdsApplyRoster.ApplicantRosterLegs.Add(fcdsApplyRosterLeg);
                }
            }
            #endregion

            #region Process Respondent Roster Data
            for (int i = 0; i < dtRespondent.Rows.Count; i++)
            {
                DataRow prevDataRow = i > 0 ? dtRespondent.Rows[i - 1] : null;
                DataRow currDataRow = dtRespondent.Rows[i];

                DateTime dutyStrDtTpe = currDataRow["Duty_Str_Dt_Tpe"] == DBNull.Value ? DateTime.Parse(currDataRow["Str_Dt_Tpe"].ToString()).Date : DateTime.Parse(currDataRow["Duty_Str_Dt_Tpe"].ToString()).Date;
                if (prevDataRow != null && dutyStrDtTpe > RespondentCrewDb.CIvvRosterId.PublishDate && (currDataRow["DutyDay"].ToString() != prevDataRow["DutyDay"].ToString() || currDataRow["DutyNo"].ToString() != prevDataRow["DutyNo"].ToString()))
                {
                    // 若勤務起始日期大於受申請者發佈班表最後日期，且當前資料列[DutyDay]與[DutyNo]欄位與上一資料列不相同時，則跳出迴圈
                    break;
                }

                FcdsApplyRoster fcdsApplyRoster = fcdsApplyRosters.Find(o => o.RosterDate == dutyStrDtTpe);
                if (fcdsApplyRoster != null)
                {
                    fcdsApplyRoster.RespondentID = this.RespondentCrewDb.ID;
                    fcdsApplyRoster.RespondentDutyDay = int.Parse(currDataRow["DutyDay"].ToString());
                    fcdsApplyRoster.RespondentDutyNo = long.Parse(currDataRow["DutyNo"].ToString());
                    fcdsApplyRoster.RespondentNumOfLeg = int.Parse(currDataRow["Num_Of_Leg"].ToString());

                    // 若勤務起始日期(報到日期時間)或航機表訂起飛時間(STD，因同一報到報離第二段無報到時間，以航機表訂起飛時間)大於當下時間，
                    // 且[Is_Swappable]為 NULL 值(表示為飛航勤務)或[Is_Swappable]為 1 時，則此筆資料可啟用勾選
                    //fcdsApplyRoster.IsRespondentEnable = 
                    //    (dutyStrDtTpe >= this.SwappableBeginDate) &&
                    //    (currDataRow["Is_Swappable"] == DBNull.Value || (currDataRow["Is_Swappable"] != DBNull.Value && currDataRow["Is_Swappable"].ToString().Equals("1")))
                    //     ? true : false;

                    if (fcdsApplyRoster.RespondentRosterLegs == null) fcdsApplyRoster.RespondentRosterLegs = new FcdsApplyRosterLegCollection();
                    FcdsApplyRosterLeg fcdsApplyRosterLeg = new FcdsApplyRosterLeg();
                    fcdsApplyRosterLeg.SetFcdsApplyRosterLeg(currDataRow);
                    fcdsApplyRoster.RespondentRosterLegs.Add(fcdsApplyRosterLeg);
                }
            }
            #endregion

            return fcdsApplyRosters;
        }

        private FcdsApplyRoster prevItem;
        private int ApplicantRosterLegIndex = 1;
        private int ApplicantRosterGroupDutyDay = 0;
        private int RespondentRosterLegIndex = 1;
        private int RespondentRosterGroupDutyDay = 0;
        protected void repSchedule_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {

                Label lblApplicant = e.Item.FindControl("lblApplicant") as Label;
                if (lblApplicant != null)
                {
                    lblApplicant.Text = ApplicantCrewDb.ID + " " + ApplicantCrewDb.PassportName + @" (" + ApplicantCrewDb.GetGroupByFleetCode() + @" - " + ApplicantCrewDb.GetGroupByCrewPosCode() + @")"; ;
                }

                Label lblRespondent = e.Item.FindControl("lblRespondent") as Label;
                if (lblRespondent != null)
                {
                    lblRespondent.Text = RespondentCrewDb.ID + " " + RespondentCrewDb.PassportName + @" (" + RespondentCrewDb.GetGroupByFleetCode() + @" - " + RespondentCrewDb.GetGroupByCrewPosCode() + @")"; ;
                }
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FcdsApplyRoster currItem = e.Item.DataItem as FcdsApplyRoster;
                if (currItem != null)
                {
                    #region Bind Applicant Roster Data
                    HtmlGenericControl divApplicantRoster = e.Item.FindControl("divApplicantRoster") as HtmlGenericControl;
                    if (divApplicantRoster != null)
                    {
                        CheckBox chkApplicantDate = e.Item.FindControl("chkApplicantDate") as CheckBox;
                        if (chkApplicantDate != null)
                        {
                            if (currItem.ApplicantRosterLegs.Count > 0 && currItem.ApplicantRosterLegs[0].CL_Order == 0)
                            {
                                // 為 Crew Route 飛航任務之第一個航段，若是後續隸屬同一 Crew Route 之航段，僅保留第一個航段之 CheckBox 確認方塊，其餘航段之 CheckBox 確認方塊皆加入 CSS Class invisible 隱藏

                                ApplicantRosterLegIndex = 1; // 全新的 Crew Route 則須恢復初始值

                                //// 1. 判斷 Crew Route 第一個班表日期之月份和[互換月份]欄位值之月份是否相同且班表日期是否大於等於該機隊職級設定(FcdsConfig)之前置工作天數(LeadWorkdays)
                                //// 2. 或班表日期之航段集合物件(ApplicantRosterLegs)內是否有可換班之勤務(Is_Swappable)
                                //// 3. 是則啟用可勾選，否則不啟用不可勾選
                                currItem.IsApplicantEnable =
                                    (currItem.RosterDate.Month.ToString() == this.FcdsApply.SwapScheduleMonth.Substring(4, 2) && currItem.RosterDate >= FcdsApply.SwappableBeginDate)
                                    || (currItem.ApplicantRosterLegs.Where(o => o.Is_Swappable == true).Count() > 0);

                                ApplicantRosterGroupDutyDay = currItem.ApplicantDutyDay;
                                divApplicantRoster.Attributes.Remove("class");
                                divApplicantRoster.Attributes.Add("class", "ss-1pdata   ss-selectable ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString());//group01
                                divApplicantRoster.Attributes.Add("data-selectablegroup", "ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString());
                                //divApplicantRoster.Attributes.Add("OnClick", "javascript:onClickRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                                //divApplicantRoster.Attributes.Add("OnMouseOver", "javascript:onMouseOverRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                                //divApplicantRoster.Attributes.Add("OnMouseOut", "javascript:onMouseOutRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                            }
                            else
                            {
                                // 為獨立空白班或非 Crew Route 飛航任務第一個航段

                                //// 1. 判斷班表日期之月份和[互換月份]欄位值之月份是否相同，且班表日期是否大於等於該機隊職級設定(FcdsConfig)之前置工作天數(LeadWorkdays)
                                //// 2. 是則啟用可勾選，否則不啟用不可勾選
                                currItem.IsApplicantEnable =
                                    currItem.RosterDate.Month.ToString() == this.FcdsApply.SwapScheduleMonth.Substring(4, 2) &&
                                    currItem.RosterDate >= FcdsApply.SwappableBeginDate;

                                if (prevItem != null)// && currItem.IsApplicantEnable
                                {
                                    if (
                                            // 判斷是否為同一 Crew Route 之連續航段
                                            (currItem.ApplicantRosterLegs.Count > 0 && currItem.ApplicantRosterLegs[0].CL_Order != 0) && // 若不是 Crew Route 第一個 Leg，進而判斷[DutyDay]和[DutyNo]是否相同或總航段數是否大於累加航段數
                                            (
                                                (currItem.ApplicantDutyDay == prevItem.ApplicantDutyDay && currItem.ApplicantDutyNo == prevItem.ApplicantDutyNo) ||
                                                (currItem.ApplicantRosterLegs.Count != 0 && currItem.ApplicantNumOfLeg > ApplicantRosterLegIndex && (ApplicantRosterLegIndex != 1 || prevItem.ApplicantRosterLegs.Count == 0))
                                            ) ||
                                            (currItem.ApplicantRosterLegs.Count == 0 && prevItem.ApplicantRosterLegs.Count != prevItem.ApplicantNumOfLeg && prevItem.ApplicantNumOfLeg > ApplicantRosterLegIndex) // 同 Crew Route 但為 Layover 故為空白班
                                       )
                                    {
                                        // 同一 Crew Route 之連續航段
                                        currItem.IsApplicantEnable = prevItem.IsApplicantEnable; // 同 Crew Route 則是否啟用可選與前一個航段相同

                                        divApplicantRoster.Attributes.Remove("class");
                                        divApplicantRoster.Attributes.Add("class", "ss-1pdata   ss-selectable ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString());//group01
                                        divApplicantRoster.Attributes.Add("data-selectablegroup", "ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString());
                                        //divApplicantRoster.Attributes.Add("OnClick", "javascript:onClickRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                                        //divApplicantRoster.Attributes.Add("OnMouseOver", "javascript:onMouseOverRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                                        //divApplicantRoster.Attributes.Add("OnMouseOut", "javascript:onMouseOutRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");

                                        chkApplicantDate.CssClass += @"   invisible"; // 加入隱藏 CheckBox 之 CSS Class
                                        if (currItem.ApplicantRosterLegs.Count != 0) ApplicantRosterLegIndex++;
                                    }
                                    else
                                    {
                                        // 判斷班表日期之航段集合物件(ApplicantRosterLegs)數量是否大於 0 個，且是否有可換班之勤務(Is_Swappable)
                                        if (currItem.ApplicantRosterLegs.Count > 0)
                                        {
                                            currItem.IsApplicantEnable = currItem.IsApplicantEnable && currItem.ApplicantRosterLegs.Where(o => o.Is_Swappable == true).Count() > 0;
                                        }

                                        ApplicantRosterLegIndex = 1;
                                        ApplicantRosterGroupDutyDay = 0;
                                    }
                                }
                            }

                            chkApplicantDate.Checked = currItem.IsApplicantTick;
                        }

                        Label lblApplicantDate = e.Item.FindControl("lblApplicantDate") as Label;
                        if (lblApplicantDate != null)
                        {
                            lblApplicantDate.Text = currItem.RosterDate.ToString("MMM dd", CultureInfo.CreateSpecificCulture("en-US"));
                        }

                        divApplicantRoster.Attributes.Remove("class");
                        if (currItem.IsApplicantEnable)// && !chkApplicantDate.CssClass.Contains(@"invisible")
                        {
                            divApplicantRoster.Attributes.Add("class", @"ss-1pdata   ss-selectable");
                        }
                        else
                        {
                            divApplicantRoster.Attributes.Add("class", @"ss-1pdata   ss-unselectable");
                        }

                        HiddenField hfApplicantDutyDay = e.Item.FindControl("hfApplicantDutyDay") as HiddenField;
                        if (hfApplicantDutyDay != null) hfApplicantDutyDay.Value = currItem.ApplicantDutyDay.ToString();

                        HiddenField hfApplicantDutyNo = e.Item.FindControl("hfApplicantDutyNo") as HiddenField;
                        if (hfApplicantDutyNo != null) hfApplicantDutyNo.Value = currItem.ApplicantDutyNo.ToString();

                        HiddenField hfApplicantNumOfLeg = e.Item.FindControl("hfApplicantNumOfLeg") as HiddenField;
                        if (hfApplicantNumOfLeg != null) hfApplicantNumOfLeg.Value = currItem.ApplicantNumOfLeg.ToString();

                        // Bind Applicant Roster Leg Collection
                        Repeater repApplicantRosterLegs = e.Item.FindControl("repApplicantRosterLegs") as Repeater;
                        if (repApplicantRosterLegs != null)
                        {
                            repApplicantRosterLegs.DataSource = currItem.ApplicantRosterLegs;
                            repApplicantRosterLegs.DataBind();
                        }
                    }
                    #endregion

                    #region Bind Respondent Roster Data
                    HtmlGenericControl divRespondentRoster = e.Item.FindControl("divRespondentRoster") as HtmlGenericControl;
                    if (divRespondentRoster != null)
                    {
                        CheckBox chkRespondentDate = e.Item.FindControl("chkRespondentDate") as CheckBox;
                        if (chkRespondentDate != null)
                        {
                            if (currItem.RespondentRosterLegs.Count > 0 && currItem.RespondentRosterLegs[0].CL_Order == 0)
                            {
                                // 為 Crew Route 飛航任務之第一個航段，若是後續隸屬同一 Crew Route 之航段，僅保留第一個航段之 CheckBox 確認方塊，其餘航段之 CheckBox 確認方塊皆加入 CSS Class invisible 隱藏

                                RespondentRosterLegIndex = 1; // 全新的 Crew Route 則須恢復初始值

                                //// 1. 判斷 Crew Route 第一個班表日期之月份和[互換月份]欄位值之月份是否相同且班表日期是否大於等於該機隊職級設定(FcdsConfig)之前置工作天數(LeadWorkdays)
                                //// 2. 或班表日期之航段集合物件(RespondentRosterLegs)內是否有可換班之勤務(Is_Swappable)
                                //// 3. 是則啟用可勾選，否則不啟用不可勾選
                                currItem.IsRespondentEnable =
                                    (currItem.RosterDate.Month.ToString() == this.FcdsApply.SwapScheduleMonth.Substring(4, 2) && currItem.RosterDate >= FcdsApply.SwappableBeginDate)
                                    || (currItem.RespondentRosterLegs.Where(o => o.Is_Swappable == true).Count() > 0);

                                RespondentRosterGroupDutyDay = currItem.RespondentDutyDay;
                                divRespondentRoster.Attributes.Remove("class");
                                divRespondentRoster.Attributes.Add("class", "ss-1pdata   ss-selectable ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString());//group01
                                divRespondentRoster.Attributes.Add("data-selectablegroup", "ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString());
                                //divRespondentRoster.Attributes.Add("OnClick", "javascript:onClickRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                                //divRespondentRoster.Attributes.Add("OnMouseOver", "javascript:onMouseOverRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                                //divRespondentRoster.Attributes.Add("OnMouseOut", "javascript:onMouseOutRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                            }
                            else
                            {
                                // 為獨立空白班或非 Crew Route 飛航任務第一個航段

                                //// 1. 判斷班表日期之月份和[互換月份]欄位值之月份是否相同，且班表日期是否大於等於該機隊職級設定(FcdsConfig)之前置工作天數(LeadWorkdays)
                                //// 2. 是則啟用可勾選，否則不啟用不可勾選
                                currItem.IsRespondentEnable =
                                    currItem.RosterDate.Month.ToString() == this.FcdsApply.SwapScheduleMonth.Substring(4, 2) &&
                                    currItem.RosterDate >= FcdsApply.SwappableBeginDate;

                                if (prevItem != null)// && currItem.IsRespondentEnable
                                {
                                    if (
                                            // 判斷是否為同一 Crew Route 之連續航段
                                            (currItem.RespondentRosterLegs.Count > 0 && currItem.RespondentRosterLegs[0].CL_Order != 0) && // 若不是 Crew Route 第一個 Leg，進而判斷[DutyDay]和[DutyNo]是否相同或總航段數是否大於累加航段數
                                            (
                                                (currItem.RespondentDutyDay == prevItem.RespondentDutyDay && currItem.RespondentDutyNo == prevItem.RespondentDutyNo) ||
                                                (currItem.RespondentRosterLegs.Count != 0 && currItem.RespondentNumOfLeg > RespondentRosterLegIndex && (RespondentRosterLegIndex != 1 || prevItem.RespondentRosterLegs.Count == 0))
                                            ) ||
                                            (currItem.RespondentRosterLegs.Count == 0 && prevItem.RespondentRosterLegs.Count != prevItem.RespondentNumOfLeg && prevItem.RespondentNumOfLeg > RespondentRosterLegIndex) // 同 Crew Route 但為 Layover 故為空白班
                                       )
                                    {
                                        // 同一 Crew Route 之連續航段
                                        currItem.IsRespondentEnable = prevItem.IsRespondentEnable; // 同 Crew Route 則是否啟用可選與前一個航段相同

                                        divRespondentRoster.Attributes.Remove("class");
                                        divRespondentRoster.Attributes.Add("class", "ss-1pdata   ss-selectable ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString());//group01
                                        divRespondentRoster.Attributes.Add("data-selectablegroup", "ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString());
                                        //divRespondentRoster.Attributes.Add("OnClick", "javascript:onClickRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                                        //divRespondentRoster.Attributes.Add("OnMouseOver", "javascript:onMouseOverRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                                        //divRespondentRoster.Attributes.Add("OnMouseOut", "javascript:onMouseOutRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");

                                        chkRespondentDate.CssClass += @"   invisible"; // 加入隱藏 CheckBox 之 CSS Class
                                        if (currItem.RespondentRosterLegs.Count != 0) RespondentRosterLegIndex++;
                                    }
                                    else
                                    {
                                        // 判斷班表日期之航段集合物件(RespondentRosterLegs)數量是否大於 0 個，且是否有可換班之勤務(Is_Swappable)
                                        if (currItem.RespondentRosterLegs.Count > 0)
                                        {
                                            currItem.IsRespondentEnable = currItem.IsRespondentEnable && currItem.RespondentRosterLegs.Where(o => o.Is_Swappable == true).Count() > 0;
                                        }

                                        RespondentRosterLegIndex = 1;
                                        RespondentRosterGroupDutyDay = 0;
                                    }
                                }
                            }

                            chkRespondentDate.Checked = currItem.IsRespondentTick;
                        }

                        Label lblRespondentDate = e.Item.FindControl("lblRespondentDate") as Label;
                        if (lblRespondentDate != null)
                        {
                            lblRespondentDate.Text = currItem.RosterDate.ToString("MMM dd", CultureInfo.CreateSpecificCulture("en-US"));
                        }

                        divRespondentRoster.Attributes.Remove("class");
                        if (currItem.IsRespondentEnable)// && !chkRespondentDate.CssClass.Contains(@"invisible")
                        {
                            divRespondentRoster.Attributes.Add("class", @"ss-1pdata   ss-selectable");
                        }
                        else
                        {
                            divRespondentRoster.Attributes.Add("class", @"ss-1pdata   ss-unselectable");
                        }

                        // Bind Respondent Roster Leg Collection
                        Repeater repRespondentRosterLegs = e.Item.FindControl("repRespondentRosterLegs") as Repeater;
                        if (repRespondentRosterLegs != null)
                        {
                            repRespondentRosterLegs.DataSource = currItem.RespondentRosterLegs;
                            repRespondentRosterLegs.DataBind();
                        }
                    }
                    #endregion

                    prevItem = currItem;
                }
            }
        }

        //protected void repSchedule_PreRender(object sender, EventArgs e)
        //{
        //    for (int i = 1; i < this.repSchedule.Items.Count; i++)
        //    {
        //        RepeaterItem currRepeaterItem = this.repSchedule.Items[i];
        //        RepeaterItem prevRepeaterItem = this.repSchedule.Items[i - 1];

        //        CheckBox chkRespondentDate = prevRepeaterItem.FindControl("chkRespondentDate") as CheckBox;

        //        HiddenField currApplicantDutyDay = currRepeaterItem.FindControl("hfApplicantDutyDay") as HiddenField;
        //        HiddenField currApplicantDutyNo = currRepeaterItem.FindControl("hfApplicantDutyNo") as HiddenField;
        //        HiddenField currApplicantNumOfLeg = currRepeaterItem.FindControl("hfApplicantNumOfLeg") as HiddenField;
        //        HiddenField prevApplicantDutyDay = prevRepeaterItem.FindControl("hfApplicantDutyDay") as HiddenField;
        //        HiddenField prevApplicantDutyNo = prevRepeaterItem.FindControl("hfApplicantDutyNo") as HiddenField;
        //        HiddenField prevApplicantNumOfLeg = prevRepeaterItem.FindControl("hfApplicantNumOfLeg") as HiddenField;

        //        Repeater repApplicantRosterLegs = currRepeaterItem.FindControl("repApplicantRosterLegs") as Repeater;
        //        if (repApplicantRosterLegs != null && currApplicantDutyDay != null && prevApplicantDutyDay != null && currApplicantDutyNo != null && prevApplicantDutyNo != null &&
        //            currApplicantNumOfLeg != null && prevApplicantNumOfLeg != null)
        //        {
        //            //if (repApplicantRosterLegs.Items.Count == 0) continue; // 表示該日期無任何航班，無須任何判斷

        //            if (
        //                (currApplicantDutyDay.Value == prevApplicantDutyDay.Value && currApplicantDutyNo.Value == prevApplicantDutyNo.Value) ||
        //                (repApplicantRosterLegs.Items.Count == 0 && int.Parse(prevApplicantNumOfLeg.Value) > ApplicantRosterLegIndex) ||
        //                (repApplicantRosterLegs.Items.Count != 0 && int.Parse(currApplicantNumOfLeg.Value) > ApplicantRosterLegIndex && ApplicantRosterLegIndex != 1)
        //               )
        //            {
        //                CheckBox chkApplicantDate = currRepeaterItem.FindControl("chkApplicantDate") as CheckBox;
        //                if (chkApplicantDate != null) chkApplicantDate.Visible = false;
        //                if (repApplicantRosterLegs.Items.Count != 0) ApplicantRosterLegIndex++;

        //                if (currApplicantNumOfLeg.Value == repApplicantRosterLegs.Items.Count.ToString() || currApplicantNumOfLeg.Value == ApplicantRosterLegIndex.ToString())
        //                {
        //                }
        //                else
        //                {
        //                }

        //            }
        //            else
        //            {
        //                ApplicantRosterLegIndex = 1; // Set to Zero
        //            }
        //        }
        //    }
        //}

        protected void repApplicantRosterLegs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FcdsApplyRosterLeg item = e.Item.DataItem as FcdsApplyRosterLeg;
                if (item != null)
                {
                    Label lblApplicantFlightNo = e.Item.FindControl("lblApplicantFlightNo") as Label;
                    if (lblApplicantFlightNo != null)
                    {
                        lblApplicantFlightNo.Text = item.Carrier_Code + item.Flight_Num;
                    }

                    if (item.Leg_Flt > 0)
                    {
                        Label lblApplicantDep = e.Item.FindControl("lblApplicantDep") as Label;
                        if (lblApplicantDep != null)
                        {
                            lblApplicantDep.Text = item.LM_Dep;
                        }

                        Label lblApplicantTo = e.Item.FindControl("lblApplicantTo") as Label;
                        if (lblApplicantTo != null)
                        {
                            lblApplicantTo.Visible = true;
                        }

                        Label lblApplicantArr = e.Item.FindControl("lblApplicantArr") as Label;
                        if (lblApplicantArr != null)
                        {
                            lblApplicantArr.Text = item.LM_Arr;
                        }
                    }

                    Label lblApplicantRpt = e.Item.FindControl("lblApplicantRpt") as Label;
                    Label lblApplicantRptTime = e.Item.FindControl("lblApplicantRptTime") as Label;
                    if (lblApplicantRpt != null && lblApplicantRptTime != null)
                    {
                        if ((item.Leg_Flt > 0 || item.VCC_Report_Time > 0) && item.Duty_Str_Dt_Tpe.HasValue)
                        {
                            lblApplicantRpt.Visible = true;

                            lblApplicantRptTime.Text = item.Duty_Str_Dt_Tpe.Value.ToString("HH:mm");
                        }
                    }

                    Label lblApplicantDbf = e.Item.FindControl("lblApplicantDbf") as Label;
                    Label lblApplicantDbfTime = e.Item.FindControl("lblApplicantDbfTime") as Label;
                    if (lblApplicantDbf != null && lblApplicantDbfTime != null)
                    {
                        if ((item.Leg_Flt > 0 || item.VCC_Debrief_Time > 0) && item.Duty_End_Dt_Tpe.HasValue)
                        {
                            lblApplicantDbf.Visible = true;

                            lblApplicantDbfTime.Text = item.Duty_End_Dt_Tpe.Value.ToString("HH:mm");
                            
                            if (item.Duty_Str_Dt_Tpe.HasValue)
                            {
                                TimeSpan ts = new TimeSpan(DateTime.Parse(item.Duty_End_Dt_Tpe.Value.ToString("yyyy/MM/dd")).Ticks - DateTime.Parse(item.Duty_Str_Dt_Tpe.Value.ToString("yyyy/MM/dd")).Ticks);
                                if (ts.TotalDays > 0) lblApplicantDbfTime.Text += @"+" + ts.TotalDays.ToString();
                            }
                        }
                    }
                }
            }
        }

        protected void repRespondentRosterLegs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FcdsApplyRosterLeg item = e.Item.DataItem as FcdsApplyRosterLeg;
                if (item != null)
                {
                    Label lblRespondentFlightNo = e.Item.FindControl("lblRespondentFlightNo") as Label;
                    if (lblRespondentFlightNo != null)
                    {
                        lblRespondentFlightNo.Text = item.Carrier_Code + item.Flight_Num;
                    }

                    if (item.Leg_Flt > 0)
                    {
                        Label lblRespondentDep = e.Item.FindControl("lblRespondentDep") as Label;
                        if (lblRespondentDep != null)
                        {
                            lblRespondentDep.Text = item.LM_Dep;
                        }

                        Label lblRespondentTo = e.Item.FindControl("lblRespondentTo") as Label;
                        if (lblRespondentTo != null)
                        {
                            lblRespondentTo.Visible = true;
                        }

                        Label lblRespondentArr = e.Item.FindControl("lblRespondentArr") as Label;
                        if (lblRespondentArr != null)
                        {
                            lblRespondentArr.Text = item.LM_Arr;
                        }
                    }

                    Label lblRespondentRpt = e.Item.FindControl("lblRespondentRpt") as Label;
                    Label lblRespondentRptTime = e.Item.FindControl("lblRespondentRptTime") as Label;
                    if (lblRespondentRpt != null && lblRespondentRptTime != null)
                    {
                        if ((item.Leg_Flt > 0 || item.VCC_Report_Time > 0) && item.Duty_Str_Dt_Tpe.HasValue)
                        {
                            lblRespondentRpt.Visible = true;

                            lblRespondentRptTime.Text = item.Duty_Str_Dt_Tpe.Value.ToString("HH:mm");
                        }
                    }

                    Label lblRespondentDbf = e.Item.FindControl("lblRespondentDbf") as Label;
                    Label lblRespondentDbfTime = e.Item.FindControl("lblRespondentDbfTime") as Label;
                    if (lblRespondentDbf != null && lblRespondentDbfTime != null)
                    {
                        if ((item.Leg_Flt > 0 || item.VCC_Debrief_Time > 0) && item.Duty_End_Dt_Tpe.HasValue)
                        {
                            lblRespondentDbf.Visible = true;

                            lblRespondentDbfTime.Text = item.Duty_End_Dt_Tpe.Value.ToString("HH:mm");

                            if (item.Duty_Str_Dt_Tpe.HasValue)
                            {
                                TimeSpan ts = new TimeSpan(DateTime.Parse(item.Duty_End_Dt_Tpe.Value.ToString("yyyy/MM/dd")).Ticks - DateTime.Parse(item.Duty_Str_Dt_Tpe.Value.ToString("yyyy/MM/dd")).Ticks);
                                if (ts.TotalDays > 0) lblRespondentDbfTime.Text += @"+" + ts.TotalDays.ToString();
                            }
                        }
                    }
                }
            }
        }

    }
}