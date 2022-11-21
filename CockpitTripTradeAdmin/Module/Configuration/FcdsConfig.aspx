<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="FcdsConfig.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Configuration.CockpitTripTradeAdmin_Module_Configuration_FcdsConfig" %>

<%@ Register Src="~/UserControl/TeamGroup.ascx" TagName="TeamGroup" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/ValidationSummary.ascx" TagName="ValidationSummary" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/ChangeHistory.ascx" TagName="ChangeHistory" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:ValidationSummary ID="ValidationSummary1" runat="server" />
    <%--ld02/ld_ci_plane.png--%>
    <BusyBoxDotNet:BusyBox ID="BusyBox1" BorderWidth="0" ShowBusyBox="Custom" IncludeScriptsInPage="true" Image="Custom" ImageUrl="../../asset/v3.0.0/images/loading/ci-plane-loading.gif" class="loading-text" DockPosition="Right" Height="120px" Layout="ImageOnly" Text="Loading..." TextBold="true" TextColor="#d81159" TextItalic="true" TextSize="18px" TextFontName="" Position="Center"  Title="" Width="120px" runat="server" />
    <section class="mb-5">
        <ul class="nav nav-tabs" id="myTab" role="tablist">
            <li class="nav-item" role="presentation">
                <a class="nav-link active" id="basic-tab" data-toggle="tab" href="#basic" role="tab" aria-controls="basic" aria-selected="true">機隊職級基本設定</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="flowteam-tab" data-toggle="tab" href="#flowteam" role="tab" aria-controls="flowteam" aria-selected="false">流程團隊</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="notify-tab" data-toggle="tab" href="#notify" role="tab" aria-controls="notify" aria-selected="false">結案通知</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="version-tab" data-toggle="tab" href="#version" role="tab" aria-controls="version" aria-selected="false">版本紀錄</a>
            </li>
        </ul>
        <!-- nav-tabs End -->
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane fade py-2 show active" id="basic" role="tabpanel" aria-labelledby="basic-tab">
                <div ID="divBootstrapAlert" Visible="false" runat="server" />
                <%--<div class="alert alert-success alert-dismissible fade show  col-xl-8" role="alert">
                    <strong>Save Complete!</strong> 您所做的修改已於 2020/10/26 15:00 儲存完畢!
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>--%>
                <section class="vdform-has-mustfill  mb-3">
                    <div class="row  form-3col6group">
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblIDFcdsConfig" AssociatedControlID="IDFcdsConfig" CssClass="col-form-label  label-sm" Text="編號" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="IDFcdsConfig" CssClass="form-control  form-control-sm" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblIDAcType" AssociatedControlID="IDAcType" CssClass="col-form-label  label-sm" Text="機隊" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="IDAcType" CssClass="form-control  form-control-sm" runat="server" />
                                    <asp:HiddenField ID="hfIDAcType" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblIDCrewPos" AssociatedControlID="IDCrewPos" CssClass="col-form-label  label-sm" Text="職級" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="IDCrewPos" CssClass="form-control  form-control-sm" runat="server" />
                                    <asp:HiddenField ID="hfIDCrewPos" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblFleetDepCode" runat="server">
                                <asp:Label ID="lblFleetDepCode" AssociatedControlID="FleetDepCode" CssClass="col-form-label  label-sm" Text="機隊對應部門" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12" id="divFleetDepCode" runat="server">
                                    <asp:DropDownList ID="FleetDepCode" AppendDataBoundItems="true" AutoPostBack="true" CssClass="form-control  form-control-sm" DataTextField="CDesc" DataValueField="DepValue" runat="server" OnSelectedIndexChanged="FleetDepCode_SelectedIndexChanged">
                                        <asp:ListItem Text="---Please select---" Value="" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblNumOfMonth" runat="server">
                                <asp:Label ID="lblNumOfMonth" AssociatedControlID="NumOfMonth" CssClass="col-form-label  label-sm" Text="每月申請次數" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12" id="divNumOfMonth" runat="server">
                                    <asp:TextBox ID="NumOfMonth" CssClass="form-control  form-control-sm" MaxLength="1" placeholder="Please input a number" runat="server" />
                                    <asp:RangeValidator ID="rvNumOfMonth" ControlToValidate="NumOfMonth" CssClass="invalid-feedback" Display="Dynamic" MaximumValue="9" MinimumValue="1" ErrorMessage="[ 每月申請次數 ]須落於1至9之間" Type="Integer" runat="server" />
                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbeNumOfMonth" FilterType="Custom" TargetControlID="NumOfMonth" ValidChars="123456789" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblIsOneReqATime" runat="server">
                                <asp:Label ID="lblIsOneReqATime" AssociatedControlID="IsOneReqATime" CssClass="col-form-label  label-sm" Text="一次一單否" runat="server" />
                            </div>
                            <div class="" id="divIsOneReqATime" runat="server"><%-- class="vdform-value"--%>
                                <%-- CssClass="form-control  form-control-sm"--%>
                                <%-- form-check-input form-control  form-control-sm--%>
                                <asp:RadioButtonList ID="IsOneReqATime" CssClass="" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
                                    <asp:ListItem Text="是" Value="True" />
                                    <asp:ListItem Text="否" Value="False" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                    </div>
                    <!-- row end-->
                    <div class="row  form-3col6group">
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblIsApplicantRevoke" runat="server">
                                <asp:Label ID="lblIsApplicantRevoke" AssociatedControlID="IsApplicantRevoke" CssClass="col-form-label  label-sm" Text="可撤回否" runat="server" />
                            </div>
                            <div class="vdform-value  form-inline   form-inline-checkradio" id="divIsApplicantRevoke" runat="server"><%-- class="vdform-value  form-inline"--%>
                                <asp:RadioButtonList ID="IsApplicantRevoke" CssClass="vdaspnet-radiobuttonlist" RepeatColumns="2" runat="server">
                                    <asp:ListItem Text="是" Value="True" />
                                    <asp:ListItem Text="否" Value="False" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblLeadWorkdays" runat="server">
                                <asp:Label ID="lblLeadWorkdays" AssociatedControlID="LeadWorkdays" CssClass="col-form-label  label-sm" Text="前置工作天" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12" id="divLeadWorkdays" runat="server">
                                    <asp:TextBox ID="LeadWorkdays" CssClass="form-control  form-control-sm" MaxLength="1" placeholder="Please input a number" runat="server" />
                                    <asp:RangeValidator ID="rvLeadWorkdays" ControlToValidate="LeadWorkdays" CssClass="invalid-feedback" Display="Dynamic" MaximumValue="9" MinimumValue="1" ErrorMessage="[ 前置工作天 ]須落於1至9之間" Type="Integer" runat="server" />
                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbeLeadWorkdays" FilterType="Custom" TargetControlID="LeadWorkdays" ValidChars="123456789" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblDeadlineOfAcrossMonth" runat="server">
                                <asp:Label ID="lblDeadlineOfAcrossMonth" AssociatedControlID="DeadlineOfAcrossMonth" CssClass="col-form-label  label-sm" Text="跨月班期限" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12" id="divDeadlineOfAcrossMonth" runat="server">
                                    <asp:DropDownList ID="DeadlineOfAcrossMonth" AppendDataBoundItems="true" AutoPostBack="false" CssClass="form-control  form-control-sm" DataTextField="CDesc" DataValueField="DepValue" runat="server">
                                        <asp:ListItem Text="---Please select---" Value="" />
                                    </asp:DropDownList>
                                    <%--<asp:TextBox ID="DeadlineOfAcrossMonth" CssClass="form-control  form-control-sm" MaxLength="2" runat="server" />--%>
                                    <%--<asp:RangeValidator ID="rvDeadlineOfAcrossMonth" ControlToValidate="DeadlineOfAcrossMonth" CssClass="invalid-feedback" Display="Dynamic" MaximumValue="30" MinimumValue="1" ErrorMessage="[ 跨月班期限 ]須落於1至30之間" Type="Integer" runat="server" />--%>
                                    <%--<ajaxToolkit:FilteredTextBoxExtender ID="ftbeDeadlineOfAcrossMonth" FilterType="Custom, Numbers" TargetControlID="DeadlineOfAcrossMonth" InvalidChars="0" runat="server" />--%>
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                    </div>
                    <!-- row end-->
                </section>
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="flowteam" role="tabpanel" aria-labelledby="flowteam-tab">
                <uc1:TeamGroup ID="FlowTeam1" runat="server" />
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="notify" role="tabpanel" aria-labelledby="notify-tab">
                <asp:UpdatePanel ID="upNotifyTeam1" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <uc1:TeamGroup ID="NotifyTeam1" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="FleetDepCode" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="version" role="tabpanel" aria-labelledby="version-tab">
                <uc1:ChangeHistory ID="ChangeHistory1" runat="server" />
            </div><!-- tab-pane End-->
        </div>
        <!-- tab-content End-->
        <%--因須檢查是否有編輯權限，故使用 ASP.NET Button 控制項--%>
        <asp:Button ID="btnCheckSave" CausesValidation="false" CssClass="btn btn-primary-sp" OnClientClick="return openBootstrapModal('checkSaveDialogModal');" UseSubmitBehavior="false" Text="Confirm to Save" runat="server" data-target="" data-toggle="modal" />
        <%--<button id="checkSave" type="button" class="btn btn-primary-sp" data-target="" data-toggle="modal" runat="server" onclick="javascript:openBootstrapModal('checkSaveDialogModal');">Confirm to Save</button>--%>
        <input type="button" class="btn btn-outline-primary" value="Back" onclick="javascript:if (confirm('Are you sure to exit this page?')) {window.onbeforeunload=null; window.location='FcdsConfig_List.aspx'; return true;} else{return false;}" />
        <%--<asp:Button ID="btnTestSendMail" CausesValidation="false" CssClass="btn btn-primary  m-r-xs   m-b-s--screen-xs" Text="Test Send Mail" runat="server" OnClick="btnTestSendMail_Click" />--%>
    </section>

    <div id="checkSaveDialogModal" aria-labelledby="checkSaveDialogLabel" class="modal fade" data-backdrop="static" role="dialog" tabindex="-1" style="display: none;">
		<div class="modal-dialog modal-dialog-centered modal-sm">
			<div class="modal-content">
				<div class="modal-header">
					<h4 id="checkSaveDialogLabel" class="modal-title text-center w-100">Confirm to Save?</h4>
					<button aria-label="Close" class="close" data-dismiss="modal" type="button">
						<span aria-hidden="true">×</span>
					</button>
				</div>
				<div class="modal-body">
					<p>Are you sure to save?</p>
				</div>
				<div class="modal-footer justify-content-center">
                    <asp:Button ID="btnSave" CausesValidation="false" CssClass="btn btn-primary-sp" Text="Save" runat="server" OnClick="btnSave_Click" />
					<button class="btn btn-outline-primary" data-dismiss="modal" type="button">Cancel</button>
				</div>
			</div>
		</div>
    </div>

<%--    <script type="text/javascript">
        window.onbeforeunload = function () {
            return "★★ 您尚未將編輯過的表單資料儲存，請問您確定要離開此頁面嗎？ ★★";
        };
    </script>--%>
</asp:Content>
