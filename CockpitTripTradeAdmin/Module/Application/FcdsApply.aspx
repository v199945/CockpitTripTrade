<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="FcdsApply.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Application.CockpitTripTradeAdmin_Module_Application_FcdsApply" %>

<%@ Register Src="~/UserControl/ApprovalHistory.ascx" TagName="ApprovalHistory" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/ChangeHistory.ascx" TagName="ChangeHistory" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/ValidationSummary.ascx" TagName="ValidationSummary" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Application/UserControl/FcdsApplyRoster.ascx" TagName="FcdsApplyRoster" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Application/UserControl/FcdsApplyApprove.ascx" TagName="FcdsApplyApprove" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../asset/v3.0.0/js/page-fcrewSwitch-audit.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:ValidationSummary ID="ValidationSummary1" runat="server" />
    <BusyBoxDotNet:BusyBox ID="BusyBox1" BorderWidth="0" ShowBusyBox="Custom" IncludeScriptsInPage="true" Image="Custom" ImageUrl="../../asset/v3.0.0/images/loading/ci-plane-loading.gif" class="loading-text" DockPosition="Right" Height="120px" Layout="ImageOnly" Text="Loading..." TextBold="true" TextColor="#d81159" TextItalic="true" TextSize="18px" TextFontName="" Position="Center"  Title="" Width="120px" runat="server" />
    <section class="mb-5">
        <ul class="nav nav-tabs" id="myTab" role="tablist">
            <li class="nav-item" role="presentation">
                <a class="nav-link active" id="apply-tab" data-toggle="tab" href="#apply" role="tab" aria-controls="apply" aria-selected="true">主表單</a>
            </li>
            <%--<li class="nav-item" role="presentation">
                <a class="nav-link" id="flowteam-tab" data-toggle="tab" href="#flowteam" role="tab" aria-controls="flowteam" aria-selected="false">流程團隊</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="notify-tab" data-toggle="tab" href="#notify" role="tab" aria-controls="notify" aria-selected="false">結案通知</a>
            </li>--%>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="approvalhistory-tab" data-toggle="tab" href="#approvalhistory" role="tab" aria-controls="approvalhistory" aria-selected="false">流程紀錄</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="version-tab" data-toggle="tab" href="#version" role="tab" aria-controls="version" aria-selected="false">版本紀錄</a>
            </li>
        </ul>
        <!-- nav-tabs End -->
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane fade py-2 show active" id="apply" role="tabpanel" aria-labelledby="apply-tab">
                <section class="vdform-has-mustfill mb-3">
                    <div class="row  form-2col">
                        <div class="col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblIDFcdsApply" AssociatedControlID="IDFcdsApply" CssClass="col-form-label  label-sm" Text="表單編號" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="IDFcdsApply" CssClass="form-control  form-control-sm" runat="server" />
                                </div>  
                            </div>
                        </div><!-- vdform-group end-->
                        <div class="col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblStatusCode" AssociatedControlID="StatusCode" CssClass="col-form-label  label-sm" Text="表單狀態" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="StatusCode" CssClass="form-control  form-control-sm" runat="server" />
                                </div>  
                            </div>
                        </div><!-- vdform-group end-->
                    </div><!-- row end-->
                    <div class="row  form-2col">
                        <div class="col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblApplicationDate" AssociatedControlID="ApplicationDate" CssClass="col-form-label  label-sm" Text="申請日期" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="ApplicationDate" CssClass="form-control  form-control-sm" runat="server" />
                                </div>  
                            </div>
                        </div><!-- vdform-group end-->
                        <div class="col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblApplicationDeadline" AssociatedControlID="ApplicationDeadline" CssClass="col-form-label  label-sm" Text="任務日期" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="ApplicationDeadline" CssClass="form-control  form-control-sm" runat="server" />
                                </div>  
                            </div>
                        </div><!-- vdform-group end-->
                    </div><!-- row end-->
                    <div class="row  form-2col">
                        <div class="col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblApplicantID" AssociatedControlID="ApplicantID" CssClass="col-form-label  label-sm" Text="申請人" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="ApplicantID" CssClass="form-control  form-control-sm" runat="server" />
                                </div>  
                            </div>
                        </div><!-- vdform-group end-->
                        <div class="col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblRespondentID" runat="server">
                                <%--<span class="vdform-mustfill">*</span>--%>
                                <asp:Label ID="lblRespondentID" AssociatedControlID="RespondentID" CssClass="col-form-label  label-sm" Text="受申請人" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12 row no-gutters" id="divRespondentID" runat="server">
                                    <asp:TextBox ID="RespondentID" CssClass="form-control  form-control-sm" placeholder="Please input Cockpit Crew ID" runat="server" />                                    
                                </div>  
                            </div>
                        </div><!-- vdform-group end-->
                    </div><!-- row end-->
                    <div class="row  form-2col">
                        <div class="col-lg-12  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblSwapScheduleMonth" runat="server">
                                <asp:Label ID="lblSwapScheduleMonth" AssociatedControlID="SwapScheduleMonth" CssClass="col-form-label  label-sm" Text="互換月份" runat="server" />
                            </div>
                            <div class="vdform-value"><%--  form-inline  form-inline-checkradio--%>
                                <div class="col-sm-12" id="divSwapScheduleMonth" runat="server">
                                    <asp:RadioButtonList ID="SwapScheduleMonth" CssClass="form-control  form-control-sm" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvSwapScheduleMonth" ControlToValidate="SwapScheduleMonth" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Swap Month ] is required!" SetFocusOnError="true" ValidationGroup="Respondent" runat="server" />
                                </div>  
                            </div>
                        </div><!-- vdform-group end-->
                        <%--
                        <div class="col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblIsApplicantRevoke" AssociatedControlID="IsApplicantRevoke" CssClass="col-form-label  label-sm" Text="申請人收回否" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="IsApplicantRevoke" CssClass="form-control  form-control-sm" runat="server" />
                                </div>
                            </div>
                        </div><!-- vdform-group end-->
                        --%>
                    </div><!-- row end-->

                </section>
                <uc1:FcdsApplyRoster ID="FcdsApplyRoster1" runat="server" />
                <uc1:FcdsApplyApprove ID="FcdsApplyApprove1" runat="server" />
                <div class="button-fixed-bottom-undermd">
                    <%--<button type="button" class="btn btn-outline-info" data-target="#checkSaveCompleteDialogModal" data-toggle="modal">Test Modal</button>--%>
                    <button id="checkApprove" type="button" class="btn btn-primary" data-target="" data-toggle="modal" runat="server" onclick="javascript:openBootstrapModal('checkApproveDialogModal');">確認批核</button>
                    <button id="checkSave" type="button" class="btn btn-primary" data-target="" data-toggle="modal" runat="server" onclick="javascript:openBootstrapModal('checkSaveDialogModal');">確認呈核</button>
                    <button id="checkReturn" type="button" class="btn btn-outline-danger" data-target="" data-toggle="modal" runat="server" onclick="javascript:openBootstrapModal('checkReturnDialogModal');">確認退回</button>
                    <button id="checkRevoke" type="button" class="btn btn-outline-danger" data-target="" data-toggle="modal" runat="server" onclick="javascript:openBootstrapModal('checkRevokeDialogModal');">確認Revoke</button>
                    
                    <input type="button" class="btn btn-outline-primary" value="Back" onclick="javascript:if (confirm('Are you sure to exit this page?')) {window.onbeforeunload = null; history.go(-1); return true;} else{return false;}" />
                </div>
            </div><!-- tab-pane End-->
            <%--<div class="tab-pane fade py-2" id="flowteam" role="tabpanel" aria-labelledby="flowteam-tab">
                <uc1:TeamGroup ID="FlowTeam1" runat="server" />
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="notify" role="tabpanel" aria-labelledby="notify-tab">
                <uc1:TeamGroup ID="NotifyTeam1" runat="server" />
            </div><!-- tab-pane End-->--%>
            <div class="tab-pane fade py-2" id="approvalhistory" role="tabpanel" aria-labelledby="approvalhistory-tab">
                <uc1:ApprovalHistory ID="ApprovalHistory1" runat="server" />
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="version" role="tabpanel" aria-labelledby="version-tab">
                <uc1:ChangeHistory ID="ChangeHistory1" runat="server" />
            </div><!-- tab-pane End-->
        </div>
        <!-- tab-content End-->
    </section>
    
    <div id="checkReturnDialogModal" aria-labelledby="checkReturnDialogLabel" class="modal fade" data-backdrop="static" role="dialog" tabindex="-1" style="display: none;">
		<div class="modal-dialog modal-dialog-centered modal-sm">
			<div class="modal-content">
				<div class="modal-header">
					<h4 id="checkReturnDialogLabel" class="modal-title text-center w-100">Confirm to Return?</h4>
					<button aria-label="Close" class="close" data-dismiss="modal" type="button">
						<span aria-hidden="true">×</span>
					</button>
				</div>
				<div class="modal-body">
					<p>Are you sure to retrun to <asp:Label ID="lblReturnTo" CssClass="text-info" runat="server" /> ?</p>
				</div>
				<div class="modal-footer justify-content-center">
                    <asp:Button ID="btnReturn" CausesValidation="false" CssClass="btn btn-outline-danger" Text="Return" runat="server" OnClick="btnReturn_Click" />
					<button class="btn btn-outline-primary" data-dismiss="modal" type="button">Cancel</button>
				</div>
			</div>
		</div>
    </div>
    
    <div id="checkRevokeDialogModal" aria-labelledby="checkRevokeDialogLabel" class="modal fade" data-backdrop="static" role="dialog" tabindex="-1" style="display: none;">
		<div class="modal-dialog modal-dialog-centered modal-sm">
			<div class="modal-content">
				<div class="modal-header">
					<h4 id="checkRevokeDialogLabel" class="modal-title text-center w-100">Confirm to Revoke?</h4>
					<button aria-label="Close" class="close" data-dismiss="modal" type="button">
						<span aria-hidden="true">×</span>
					</button>
				</div>
				<div class="modal-body">
					<p>Are you sure to revoke?</p>
				</div>
				<div class="modal-footer justify-content-center">
                    <asp:Button ID="btnRevoke" CausesValidation="false" CssClass="btn btn-outline-danger" Text="Revoke" runat="server" OnClick="btnRevoke_Click" />
					<button class="btn btn-outline-primary" data-dismiss="modal" type="button">Cancel</button>
				</div>
			</div>
		</div>
    </div>

    <div id="checkApproveDialogModal" aria-labelledby="checkApproveDialogLabel" class="modal fade" data-backdrop="static" role="dialog" tabindex="-1" style="display: none;">
		<div class="modal-dialog modal-dialog-centered modal-sm">
			<div class="modal-content">
				<div class="modal-header">
					<h4 id="checkApproveDialogLabel" class="modal-title text-center w-100">確認批核？</h4>
					<button aria-label="Close" class="close" data-dismiss="modal" type="button">
						<span aria-hidden="true">×</span>
					</button>
				</div>
				<div class="modal-body">
                    <asp:UpdatePanel ID="upApprove" UpdateMode="Always" runat="server">
                        <ContentTemplate>
						    <span id="spanApprove" runat="server"> 互換任務？</span>
                        </ContentTemplate>
                    </asp:UpdatePanel>
				</div>
				<div class="modal-footer justify-content-center">
                    <asp:Button ID="btnApprove" CausesValidation="false" CssClass="btn btn-primary" Text="批核" runat="server" OnClick="btnApprove_Click" />
					<button class="btn btn-outline-primary" data-dismiss="modal" type="button">Cancel</button>
				</div>
			</div>
		</div>
    </div>
    
    <div id="checkSaveDialogModal" aria-labelledby="checkSaveDialogLabel" class="modal fade" data-backdrop="static" role="dialog" tabindex="-1" style="display: none;"><%-- aria-hidden="true" padding-right: 17px;--%>
		<div class="modal-dialog modal-dialog-centered modal-sm">
			<div class="modal-content">
				<div class="modal-header">
					<h4 id="checkSaveDialogLabel" class="modal-title text-center w-100">確認呈核？</h4>
					<button aria-label="Close" class="close" data-dismiss="modal" type="button">
						<span aria-hidden="true">×</span>
					</button>
				</div>
				<div class="modal-body">
					<span id="spanSubmitTo" runat="server">您確定要呈核給</span>
				</div>
				<div class="modal-footer justify-content-center">
                    <asp:Button ID="btnSave" CausesValidation="false" CssClass="btn btn-primary" Text="Save" runat="server" OnClick="btnSave_Click" />
					<button class="btn btn-outline-primary" data-dismiss="modal" type="button">Cancel</button>
				</div>
			</div>
		</div>
    </div>

    <div id="checkSaveCompleteDialogModal" aria-labelledby="checkSaveCompleteDialogLabel" class="modal fade" data-backdrop="static" data-keyboard="false" role="dialog" tabindex="-1" style="display: none;"><%-- aria-hidden="true" padding-right: 17px;--%>
		<div class="modal-dialog modal-dialog-centered modal-sm">
			<div class="modal-content">
				<div class="modal-header justify-content-center">
					<h4 id="checkSaveCompleteDialogLabel" class="modal-title text-center w-100" />
				</div>
				<div class="modal-body text-center">
                    <div style="margin-bottom: 10px;"><img src="../../asset/v3.0.0/images/icons/icon-success.png"></div>
					<div id="divSaveComplete" />
				</div>
				<div class="modal-footer justify-content-center">
					<button class="btn btn-primary" data-dismiss="modal" data-toggle="modal" tabindex="0" type="button" onclick="window.location='ReviewFcdsApply_List.aspx';">Confirm</button>
				</div>
			</div>
		</div>
    </div>

    <script type="text/javascript">
        <%--window.onbeforeunload = function () {
            return "★★ 您尚未將編輯過的表單資料儲存，請問您確定要離開此頁面嗎？ ★★";
        };--%>
    </script>
</asp:Content>
