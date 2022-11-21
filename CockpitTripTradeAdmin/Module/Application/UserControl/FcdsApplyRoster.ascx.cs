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
using Library.Component.Utility;
using Library.Module.FCDS.Application;
using Library.Module.FZDB;

namespace CockpitTripTradeAdmin.Module.Application.UserControl
{
    public partial class CockpitTripTradeAdmin_Module_Application_UserControl_FcdsApplyRoster : ApplicationUserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CockpitTripTradeAdmin_Module_Application_UserControl_FcdsApplyRoster));
        private Stopwatch sw = new Stopwatch();

        /// <summary>
        /// 
        /// </summary>
        public FcdsApplyRosterCollection FcdsApplyRosters
        {
            get
            {
                return SerializeUtility.Deserialize<FcdsApplyRosterCollection>(ViewState["FcdsApplyRosters"].ToString());
            }
            
            set
            {
                ViewState["FcdsApplyRosters"] = SerializeUtility.Serialize(value);
            }
        }

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

        /// <summary>
        /// 申請人勾選之最小班表日期，作為推算申請單之逾期日期(任務日期)。
        /// </summary>
        public DateTime? ApplicantMinTickRosterDate { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }

            
        }

        /// <summary>
        /// 繫結飛航組員任務換班申請系統申請主表單之申請人與受申請人之 Repeater 物件資料。
        /// </summary>
        public void BindRepeaterList()
        {
            this.repSchedule.DataSource = this.FcdsApplyRosters;

            sw.Start();
            this.repSchedule.DataBind();
            sw.Stop();
            logger.Info(@"FcdsApplyRoster.repSchedule_ItemDataBound() Bind Repeater cost=" + sw.Elapsed.TotalSeconds + @" seconds");
            Debug.WriteLine(@"FcdsApplyRoster.repSchedule_ItemDataBound() Bind Repeater cost=" + sw.Elapsed.TotalSeconds + @" seconds");
            sw.Reset();

            this.FcdsApplyRosters = this.tempFcdsApplyRosters;
        }

        /// <summary>
        /// 前一個 FcdsApplyRoster 物件。
        /// </summary>
        private FcdsApplyRoster prevItem;

        /// <summary>
        /// 申請人班表日期之航段數索引值。
        /// </summary>
        private int ApplicantRosterLegIndex = 1;

        /// <summary>
        /// 申請人班表日期同一 Crew Route 之[DutyDay]，以便於 divApplicantRoster 增加 [data-selectablegroup]屬性帶入相同屬性值，群組化同一 Crew Route。
        /// </summary>
        private int ApplicantRosterGroupDutyDay = 0;

        /// <summary>
        /// 受申請人班表日期之航段數索引值。
        /// </summary>
        private int RespondentRosterLegIndex = 1;

        /// <summary>
        /// 受申請人班表日期同一 Crew Route 之[DutyDay]，以便於 divRespondentRoster 增加 [data-selectablegroup]屬性帶入相同屬性值，群組化同一 Crew Route。
        /// </summary>
        private int RespondentRosterGroupDutyDay = 0;

        /// <summary>
        /// 申請人 Crew Route 總航段數。
        /// </summary>
        private int ApplicantCRTtlLegNum = 0;

        /// <summary>
        /// 受申請人 Crew Route 總航段數。
        /// </summary>
        private int RespondentCRTtlLegNum = 0;

        private FcdsApplyRosterCollection tempFcdsApplyRosters = new FcdsApplyRosterCollection();
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
                            if (currItem.ApplicantRosterLegs.Count > 0 && currItem.ApplicantRosterLegs[0].CL_Order.HasValue && currItem.ApplicantRosterLegs[0].CL_Order == 0)
                            {
                                // 為 Crew Route 飛航任務之第一個航段，若是後續隸屬同一 Crew Route 之航段，僅保留第一個航段之 CheckBox 確認方塊，其餘航段之 CheckBox 確認方塊皆加入 CSS Class invisible 隱藏

                                ApplicantRosterLegIndex = currItem.ApplicantRosterLegs.Count; // 全新的 Crew Route 則指派為班表日期之航段數量
                                ApplicantCRTtlLegNum = currItem.ApplicantNumOfLeg; // 全新的 Crew Route 則指派為總航段數

                                //// 1. 判斷 Crew Route 第一個班表日期之月份和[互換月份]欄位值之月份相同且班表日期大於等於可申請換班起始日期(FcdsApply.SwappableBeginDate.Value)
                                //// 2. 且班表日期之航段集合物件(ApplicantRosterLegs)內有可換班之勤務(Is_Swappable)
                                //// 3. 且班表日期之航段集合物件(ApplicantRosterLegs)內無有跨月班之 Crew Route(Is_AcrossMonth)或跨月班申請期限日期大於當下日期
                                //// 4. 是則啟用可勾選，否則不啟用不可勾選
                                currItem.IsApplicantEnable =
                                    currItem.RosterDate.Month.ToString("D2") == this.FcdsApply.SwapScheduleMonth.Substring(4, 2) && currItem.RosterDate >= FcdsApply.SwappableBeginDate.Value
                                    && currItem.ApplicantRosterLegs.Where(o => o.Is_Swappable == true).Count() > 0
                                    && (currItem.ApplicantRosterLegs.Where(o => o.Is_AcrossMonth == true).Count() == 0 || this.FcdsApply.DeadlineOfAcrossMonthDate.Value > DateTime.Now)
                                    ;

                                ApplicantRosterGroupDutyDay = currItem.ApplicantDutyDay;
                                //divApplicantRoster.Attributes.Remove("class");
                                //divApplicantRoster.Attributes.Add("class", "ss-1pdata   ss-selectable ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString());//group01
                                divApplicantRoster.Attributes.Add("data-selectablegroup", "ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString());
                                //divApplicantRoster.Attributes.Add("OnClick", "javascript:onClickRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                                //divApplicantRoster.Attributes.Add("OnMouseOver", "javascript:onMouseOverRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                                //divApplicantRoster.Attributes.Add("OnMouseOut", "javascript:onMouseOutRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                            }
                            else
                            {
                                // 為獨立空白班、地面任務或非 Crew Route 飛航任務第一個航段(即同一 Crew Route 之連續航段)

                                //// 1. 判斷
                                ////  1.1 班表日期之月份和[互換月份]欄位值之月份是否相同
                                ////  1.2 且班表日期是否大於等於可申請換班起始日期(FcdsApply.SwappableBeginDate.Value)
                                //// 2. 是則啟用可勾選，否則不啟用不可勾選
                                currItem.IsApplicantEnable =
                                    currItem.RosterDate.Month.ToString("D2") == this.FcdsApply.SwapScheduleMonth.Substring(4, 2) &&
                                    currItem.RosterDate >= FcdsApply.SwappableBeginDate.Value
                                    ;

                                if (prevItem != null)
                                {
                                    if (
                                            // 判斷此日期是否為同一 Crew Route 之連續航段飛航任務
                                            (
                                             (currItem.ApplicantRosterLegs.Count > 0 && currItem.ApplicantRosterLegs[0].CL_Order.HasValue && currItem.ApplicantRosterLegs[0].CL_Order != 0) // 若不是 Crew Route 第一個 Leg
                                             && ( // 進而判斷
                                                 (currItem.ApplicantDutyDay == prevItem.ApplicantDutyDay && currItem.ApplicantDutyNo == prevItem.ApplicantDutyNo)
                                                 || // 或
                                                 (currItem.ApplicantRosterLegs.Count != 0 && currItem.ApplicantNumOfLeg > ApplicantRosterLegIndex && (ApplicantRosterLegIndex != 1 || prevItem.ApplicantRosterLegs.Count == 0)) // 此日期之航段數不為零、且此日期之總航段數是否大於累加航段數變數(ApplicantRosterLegIndex)、且累加航段數變數不為一或前一日期之航段數為零
                                                )
                                            )
                                            || (currItem.ApplicantRosterLegs.Count == 0 && prevItem.ApplicantRosterLegs.Count != prevItem.ApplicantNumOfLeg && prevItem.ApplicantNumOfLeg > ApplicantRosterLegIndex) // 同 Crew Route 但為 Layover，故為空白班
                                            || (ApplicantCRTtlLegNum > ApplicantRosterLegIndex) // 同 Crew Route 之總航段數仍大於累加航段數變數(ApplicantRosterLegIndex)為連續 Layover，故為連續空白班
                                       )
                                    {
                                        // 同一 Crew Route 之連續航段
                                        currItem.IsApplicantEnable = prevItem.IsApplicantEnable; // 同 Crew Route 則是否啟用可選與前一個航段相同

                                        //divApplicantRoster.Attributes.Remove("class");
                                        //divApplicantRoster.Attributes.Add("class", "ss-1pdata   ss-selectable ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString());//group01
                                        divApplicantRoster.Attributes.Add("data-selectablegroup", "ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString());
                                        //divApplicantRoster.Attributes.Add("OnClick", "javascript:onClickRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                                        //divApplicantRoster.Attributes.Add("OnMouseOver", "javascript:onMouseOverRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");
                                        //divApplicantRoster.Attributes.Add("OnMouseOut", "javascript:onMouseOutRosterGroup('ss-selectable-group-applicant-" + ApplicantRosterGroupDutyDay.ToString() + "')");

                                        chkApplicantDate.CssClass += @"   invisible"; // 加入隱藏 CheckBox 之 CSS Class

                                        // 若申請人班表日期之航段集合物件(ApplicantRosterLegs)數量大於 0 個，則累計航段集合物件項目數量
                                        if (currItem.ApplicantRosterLegs.Count != 0) ApplicantRosterLegIndex += currItem.ApplicantRosterLegs.Count;
                                    }
                                    else
                                    {
                                        // 為獨立空白班或地面任務
                                        // 判斷班表日期之航段集合物件(ApplicantRosterLegs)數量是否大於 0 個，且是否有可換班之勤務(Is_Swappable)，則開放申請人勾選
                                        if (currItem.ApplicantRosterLegs.Count > 0)
                                        {
                                            currItem.IsApplicantEnable = currItem.IsApplicantEnable && currItem.ApplicantRosterLegs.Where(o => o.Is_Swappable == true).Count() > 0;
                                        }

                                        ApplicantRosterLegIndex = 1; // 回復預設值
                                        ApplicantRosterGroupDutyDay = 0; // 回復預設值
                                        ApplicantCRTtlLegNum = 0; // 回復預設值
                                    }
                                }
                            }

                            chkApplicantDate.Checked = currItem.IsApplicantTick;
                            chkApplicantDate.Enabled = this.BasePageModeEnum == PageMode.PageModeEnum.Create;
                        }

                        Label lblApplicantDate = e.Item.FindControl("lblApplicantDate") as Label;
                        if (lblApplicantDate != null)
                        {
                            lblApplicantDate.Text = currItem.RosterDate.ToString("MMM dd", CultureInfo.CreateSpecificCulture("en-US"));
                        }

                        HiddenField hfApplicantRosterDate = e.Item.FindControl("hfApplicantRosterDate") as HiddenField;
                        if (hfApplicantRosterDate != null) hfApplicantRosterDate.Value = currItem.RosterDate.ToString("yyyy/MM/dd");

                        divApplicantRoster.Attributes.Remove("class");
                        if (currItem.IsApplicantEnable)
                        {
                            divApplicantRoster.Attributes.Add("class", @"ss-1pdata   ss-selectable");
                        }
                        else
                        {
                            divApplicantRoster.Attributes.Add("class", @"ss-1pdata   ss-unselectable");
                        }

                        if (currItem.IsApplicantTick)
                        {
                            divApplicantRoster.Attributes.Add("class", divApplicantRoster.Attributes["class"] + @"   ss-selected");
                        }

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
                            if (currItem.RespondentRosterLegs.Count > 0 && currItem.RespondentRosterLegs[0].CL_Order.HasValue && currItem.RespondentRosterLegs[0].CL_Order == 0)
                            {
                                // 為 Crew Route 飛航任務之第一個航段，若是後續隸屬同一 Crew Route 之航段，僅保留第一個航段之 CheckBox 確認方塊，其餘航段之 CheckBox 確認方塊皆加入 CSS Class invisible 隱藏

                                RespondentRosterLegIndex = currItem.RespondentRosterLegs.Count; // 全新的 Crew Route 則指派為班表日期之航段數量
                                RespondentCRTtlLegNum = currItem.RespondentNumOfLeg; // 全新的 Crew Route 則指派為總航段數

                                //// 1. 判斷 Crew Route 第一個班表日期之月份和[互換月份]欄位值之月份相同且班表日期大於等於可申請換班起始日期(FcdsApply.SwappableBeginDate.Value)
                                //// 2. 且班表日期之航段集合物件(ApplicantRosterLegs)內有可換班之勤務(Is_Swappable)
                                //// 3. 且班表日期之航段集合物件(ApplicantRosterLegs)內無有跨月班之 Crew Route(Is_AcrossMonth)或跨月班申請期限日期大於當下日期
                                //// 4. 是則啟用可勾選，否則不啟用不可勾選
                                currItem.IsRespondentEnable =
                                    currItem.RosterDate.Month.ToString("D2") == this.FcdsApply.SwapScheduleMonth.Substring(4, 2) && currItem.RosterDate >= FcdsApply.SwappableBeginDate.Value
                                    && currItem.RespondentRosterLegs.Where(o => o.Is_Swappable == true).Count() > 0
                                    && (currItem.RespondentRosterLegs.Where(o => o.Is_AcrossMonth == true).Count() <= 0 || this.FcdsApply.DeadlineOfAcrossMonthDate.Value > DateTime.Now)
                                    ;

                                RespondentRosterGroupDutyDay = currItem.RespondentDutyDay;
                                //divRespondentRoster.Attributes.Remove("class");
                                //divRespondentRoster.Attributes.Add("class", "ss-1pdata   ss-selectable ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString());//group01
                                divRespondentRoster.Attributes.Add("data-selectablegroup", "ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString());
                                //divRespondentRoster.Attributes.Add("OnClick", "javascript:onClickRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                                //divRespondentRoster.Attributes.Add("OnMouseOver", "javascript:onMouseOverRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                                //divRespondentRoster.Attributes.Add("OnMouseOut", "javascript:onMouseOutRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                            }
                            else
                            {
                                // 為獨立空白班、地面任務或非 Crew Route 飛航任務第一個航段(即同一 Crew Route 之連續航段)

                                //// 1. 判斷
                                ////  1.1 班表日期之月份和[互換月份]欄位值之月份是否相同
                                ////  1.2 且班表日期是否大於等於可申請換班起始日期(FcdsApply.SwappableBeginDate.Value)
                                //// 2. 是則啟用可勾選，否則不啟用不可勾選
                                currItem.IsRespondentEnable =
                                    currItem.RosterDate.Month.ToString("D2") == this.FcdsApply.SwapScheduleMonth.Substring(4, 2) &&
                                    currItem.RosterDate >= FcdsApply.SwappableBeginDate.Value
                                    ;

                                if (prevItem != null)
                                {
                                    if (
                                            // 判斷此日期是否為同一 Crew Route 之連續航段飛航任務
                                            (
                                             (currItem.RespondentRosterLegs.Count > 0 && currItem.RespondentRosterLegs[0].CL_Order.HasValue && currItem.RespondentRosterLegs[0].CL_Order != 0) // 若不是 Crew Route 第一個 Leg
                                             && ( // 進而判斷
                                                 (currItem.RespondentDutyDay == prevItem.RespondentDutyDay && currItem.RespondentDutyNo == prevItem.RespondentDutyNo) // 此日期之[DutyDay]和[DutyNo]是否相同
                                                 || // 或
                                                 (currItem.RespondentRosterLegs.Count != 0 && currItem.RespondentNumOfLeg > RespondentRosterLegIndex && (RespondentRosterLegIndex != 1 || prevItem.RespondentRosterLegs.Count == 0)) // 此日期之航段數不為零、且此日期之總航段數是否大於累加航段數變數(RespondentRosterLegIndex)、且累加航段數變數不為一或前一日期之航段數為零
                                                )
                                            )
                                            || (currItem.RespondentRosterLegs.Count == 0 && prevItem.RespondentRosterLegs.Count != prevItem.RespondentNumOfLeg && prevItem.RespondentNumOfLeg > RespondentRosterLegIndex) // 同 Crew Route 但為 Layover，故為空白班
                                            || (RespondentCRTtlLegNum > RespondentRosterLegIndex) // 同 Crew Route 之總航段數仍大於累加航段數變數(RespondentRosterLegIndex)為連續 Layover，故為連續空白班
                                       )
                                    {
                                        // 同一 Crew Route 之連續航段
                                        currItem.IsRespondentEnable = prevItem.IsRespondentEnable; // 同 Crew Route 則是否啟用可選與前一個航段相同

                                        //divRespondentRoster.Attributes.Remove("class");
                                        //divRespondentRoster.Attributes.Add("class", "ss-1pdata   ss-selectable ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString());//group01
                                        divRespondentRoster.Attributes.Add("data-selectablegroup", "ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString());
                                        //divRespondentRoster.Attributes.Add("OnClick", "javascript:onClickRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                                        //divRespondentRoster.Attributes.Add("OnMouseOver", "javascript:onMouseOverRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");
                                        //divRespondentRoster.Attributes.Add("OnMouseOut", "javascript:onMouseOutRosterGroup('ss-selectable-group-respondent-" + RespondentRosterGroupDutyDay.ToString() + "')");

                                        chkRespondentDate.CssClass += @"   invisible"; // 加入隱藏 CheckBox 之 CSS Class

                                        // 若受申请人班表日期之航段集合物件(RespondentRosterLegs)數量大於 0 個，則累計航段集合物件項目數量
                                        if (currItem.RespondentRosterLegs.Count != 0) RespondentRosterLegIndex += currItem.RespondentRosterLegs.Count;
                                    }
                                    else
                                    {
                                        // 為獨立空白班或地面任務，表示 Crew Route 已結束
                                        // 判斷班表日期之航段集合物件(RespondentRosterLegs)數量是否大於 0 個，且是否有可換班之勤務(Is_Swappable)，則開放受申請人勾選
                                        if (currItem.RespondentRosterLegs.Count > 0)
                                        {
                                            currItem.IsRespondentEnable = currItem.IsRespondentEnable && currItem.RespondentRosterLegs.Where(o => o.Is_Swappable == true).Count() > 0;
                                        }

                                        RespondentRosterLegIndex = 1; // 回復預設值
                                        RespondentRosterGroupDutyDay = 0; // 回復預設值
                                        RespondentCRTtlLegNum = 0; // 回復預設值
                                    }
                                }
                            }

                            chkRespondentDate.Checked = currItem.IsRespondentTick;
                            chkRespondentDate.Enabled = this.BasePageModeEnum == PageMode.PageModeEnum.Create;
                        }

                        Label lblRespondentDate = e.Item.FindControl("lblRespondentDate") as Label;
                        if (lblRespondentDate != null)
                        {
                            lblRespondentDate.Text = currItem.RosterDate.ToString("MMM dd", CultureInfo.CreateSpecificCulture("en-US"));
                        }

                        HiddenField hfRespondentRosterDate = e.Item.FindControl("hfRespondentRosterDate") as HiddenField;
                        if (hfRespondentRosterDate != null) hfRespondentRosterDate.Value = currItem.RosterDate.ToString("yyyy/MM/dd");

                        divRespondentRoster.Attributes.Remove("class");
                        if (currItem.IsRespondentEnable)
                        {
                            divRespondentRoster.Attributes.Add("class", @"ss-1pdata   ss-selectable");
                        }
                        else
                        {
                            divRespondentRoster.Attributes.Add("class", @"ss-1pdata   ss-unselectable");
                        }

                        if (currItem.IsRespondentTick)
                        {
                            divRespondentRoster.Attributes.Add("class", divRespondentRoster.Attributes["class"] + @"   ss-selected");
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

                    this.tempFcdsApplyRosters.Add(currItem);
                    prevItem = currItem;
                }
            }
        }

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

                            HiddenField hfApplicantRosterDate = e.Item.Parent.Parent.FindControl("hfApplicantRosterDate") as HiddenField;
                            if (hfApplicantRosterDate != null && DateTime.TryParse(hfApplicantRosterDate.Value, out _))
                            {
                                DateTime rosterDate = DateTime.Parse(hfApplicantRosterDate.Value);
                                TimeSpan ts = new TimeSpan(DateTime.Parse(item.Duty_End_Dt_Tpe.Value.ToString("yyyy/MM/dd")).Ticks - DateTime.Parse(rosterDate.ToString("yyyy/MM/dd")).Ticks);
                                if (ts.TotalDays != 0) lblApplicantDbfTime.Text += @"+" + ts.TotalDays.ToString();
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

                            HiddenField hfRespondentRosterDate = e.Item.Parent.Parent.FindControl("hfRespondentRosterDate") as HiddenField;
                            if (hfRespondentRosterDate != null && DateTime.TryParse(hfRespondentRosterDate.Value, out _))
                            {
                                DateTime rosterDate = DateTime.Parse(hfRespondentRosterDate.Value);
                                TimeSpan ts = new TimeSpan(DateTime.Parse(item.Duty_End_Dt_Tpe.Value.ToString("yyyy/MM/dd")).Ticks - DateTime.Parse(rosterDate.ToString("yyyy/MM/dd")).Ticks);
                                if (ts.TotalDays != 0) lblRespondentDbfTime.Text += @"+" + ts.TotalDays.ToString();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///新增 FcdsApplyRoster 集合物件。
        /// </summary>
        /// <param name="isSaveLog">是否儲存日誌</param>
        /// <returns></returns>
        /// <remarks>前台飛航組員建立申請單時專用。</remarks>
        public bool SaveApplyRoster(bool isSaveLog)
        {
            UpdateCollectionObjValue();

            foreach (FcdsApplyRoster obj in this.FcdsApplyRosters)
            {
                obj.IDFcdsApply = this.FcdsApply.IDFcdsApply;
                obj.BranchID = this.FcdsApply.BranchID;
                obj.Version = this.FcdsApply.Version;
                obj.ApplicantID = this.FcdsApply.ApplicantID;
                obj.RespondentID = this.FcdsApply.RespondentID;
                obj.CreateBy = this.FcdsApply.CreateBy;
                obj.CreateStamp = this.FcdsApply.CreateStamp;
                obj.UpdateBy = this.FcdsApply.UpdateBy;
                obj.UpdateStamp = this.FcdsApply.UpdateStamp;

                if (!obj.Save(this.BasePageModeEnum, isSaveLog))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 更新 FcdsApplyRoster 集合物件。
        /// </summary>
        /// <param name="isSaveLog">是否儲存日誌</param>
        /// <returns></returns>
        public bool UpdateApplyRoster( bool isSaveLog)
        {
            foreach (FcdsApplyRoster obj in this.FcdsApplyRosters)
            {
                obj.BranchID = this.FcdsApply.BranchID;
                obj.Version = this.FcdsApply.Version;
                obj.UpdateBy = this.FcdsApply.UpdateBy;
                obj.UpdateStamp = this.FcdsApply.UpdateStamp;

                if (!obj.Save(this.BasePageModeEnum, isSaveLog))
                    return false;
            }

            return true;

        }

        /// <summary>
        /// 檢核班表 Repeater 控制項，飛航組員任務換班申請系統新建申請單時檢核。
        /// 1. 申請人與受申請人勾選天數是否相同
        /// 2. 申請人與受申請人皆至少須勾選一天
        /// </summary>
        /// <returns></returns>
        public string ValidateUserControl()
        {
            string alert = null;
            bool hasApplicantTick = false;
            bool hasRespondentTick = false;

            if (this.BasePageModeEnum == PageMode.PageModeEnum.Create)
            {
                foreach (RepeaterItem ri in this.repSchedule.Items)
                {
                    HtmlGenericControl divApplicantRoster = ri.FindControl("divApplicantRoster") as HtmlGenericControl;
                    HtmlGenericControl divRespondentRoster = ri.FindControl("divRespondentRoster") as HtmlGenericControl;
                    Label lblApplicantDate = ri.FindControl("lblApplicantDate") as Label;
                    Label lblRespondentDate = ri.FindControl("lblRespondentDate") as Label;

                    if (divApplicantRoster != null && divRespondentRoster != null
                        && lblApplicantDate != null && !string.IsNullOrEmpty(lblApplicantDate.Text) && DateTime.TryParse(lblApplicantDate.Text, out _)
                        && lblRespondentDate != null && !string.IsNullOrEmpty(lblRespondentDate.Text) && DateTime.TryParse(lblRespondentDate.Text, out _))
                    {
                        CheckBox chkApplicantDate = ri.FindControl("chkApplicantDate") as CheckBox;
                        CheckBox chkRespondentDate = ri.FindControl("chkRespondentDate") as CheckBox;

                        if (chkApplicantDate != null && chkRespondentDate != null)
                        {
                            if (chkApplicantDate.Checked != chkRespondentDate.Checked)
                            {
                                alert += @"Please check the rosters of " + lblApplicantDate.Text + @"!\n";
                            }
                            else
                            {
                            }

                            if (chkApplicantDate.Checked)
                            {
                                divApplicantRoster.Attributes[@"class"] += @" ss-selected";
                            }
                            else
                            {
                                divApplicantRoster.Attributes[@"class"] = divApplicantRoster.Attributes[@"class"].Replace(@" ss-selected", string.Empty);
                            }

                            if (chkRespondentDate.Checked)
                            {
                                divRespondentRoster.Attributes[@"class"] += @" ss-selected";
                            }
                            else
                            {
                                divRespondentRoster.Attributes[@"class"] = divRespondentRoster.Attributes[@"class"].Replace(@" ss-selected", string.Empty);
                            }

                            if (!hasApplicantTick && chkApplicantDate.Checked)
                            {
                                this.ApplicantMinTickRosterDate = DateTime.Parse(lblApplicantDate.Text, CultureInfo.CreateSpecificCulture("en-US"));
                                hasApplicantTick = true;
                            }

                            if (!hasRespondentTick && chkRespondentDate.Checked)
                            {
                                hasRespondentTick = true;
                            }
                        }
                    }
                }

                if (!hasApplicantTick)
                    alert += @"Please Select one Roster of Applicant date at least!\n";

                if (!hasRespondentTick)
                    alert += @"Please Select one Roster of Respondent date at least!\n";
            }

            return alert;
        }

        private void UpdateCollectionObjValue()
        {
            FcdsApplyRosterCollection col = this.FcdsApplyRosters;
            FcdsApplyRoster obj = null;

            foreach (RepeaterItem ri in this.repSchedule.Items)
            {
                DateTime? rosterDate = null;

                HtmlGenericControl divApplicantRoster = ri.FindControl("divApplicantRoster") as HtmlGenericControl;
                HtmlGenericControl divRespondentRoster = ri.FindControl("divRespondentRoster") as HtmlGenericControl;
                Label lblApplicantDate = ri.FindControl("lblApplicantDate") as Label;
                if (divApplicantRoster != null && divRespondentRoster != null &&
                    lblApplicantDate != null && !string.IsNullOrEmpty(lblApplicantDate.Text) && DateTime.TryParse(lblApplicantDate.Text, out _))
                {
                    rosterDate = DateTime.Parse(lblApplicantDate.Text, CultureInfo.CreateSpecificCulture("en-US"));
                    if (rosterDate.HasValue)
                    {
                        obj = col.Find(o => o.RosterDate == rosterDate);
                        if (obj == null)
                        {
                            logger.Error("RosterDate=" + rosterDate.Value.ToString("yyyy/MM/dd") + ", There is no object in collection!");
                        }
                        else
                        {
                            CheckBox chkApplicantDate = ri.FindControl("chkApplicantDate") as CheckBox;
                            if (chkApplicantDate != null)
                            {
                                obj.IsApplicantTick = chkApplicantDate.Checked;
                            }

                            CheckBox chkRespondentDate = ri.FindControl("chkRespondentDate") as CheckBox;
                            if (chkRespondentDate != null)
                            {
                                obj.IsRespondentTick = chkRespondentDate.Checked;
                            }

                        }
                    }
                }
            }

            this.FcdsApplyRosters = col;
        }

        private FcdsApplyRoster SetRepeaterItemValue(RepeaterItem ri)
        {
            FcdsApplyRoster obj = new FcdsApplyRoster(); ;
            DateTime? rosterDate = null;

            Label lblApplicantDate = ri.FindControl("lblApplicantDate") as Label;
            if (lblApplicantDate != null && !string.IsNullOrEmpty(lblApplicantDate.Text) && DateTime.TryParse(lblApplicantDate.Text, out _))
            {
                rosterDate = DateTime.Parse(lblApplicantDate.Text, CultureInfo.CreateSpecificCulture("en-US"));
                if (rosterDate.HasValue)
                {
                    obj.RosterDate = rosterDate.Value;
                }
            }

            obj.ApplicantID = this.ApplicantCrewDb.ID;

            CheckBox chkApplicantDate = ri.FindControl("chkApplicantDate") as CheckBox;
            if (chkApplicantDate != null)
            {
                obj.IsApplicantTick = chkApplicantDate.Checked;
            }

            obj.RespondentID = this.RespondentCrewDb.ID;
            CheckBox chkRespondentDate = ri.FindControl("chkRespondentDate") as CheckBox;
            if (chkRespondentDate != null)
            {
                obj.IsRespondentTick = chkRespondentDate.Checked;
            }

            return obj;
        }

    }
}