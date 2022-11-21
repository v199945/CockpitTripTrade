<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="FcdsRejectReason.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Configuration.CockpitTripTradeAdmin_Module_Configuration_FcdsRejectReason" %>

<%@ Register Src="~/UserControl/ValidationSummary.ascx" TagName="ValidationSummary" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/ChangeHistory.ascx" TagName="ChangeHistory" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:ValidationSummary ID="ValidationSummary1" runat="server" />
    <BusyBoxDotNet:BusyBox ID="BusyBox1" BorderWidth="0" ShowBusyBox="Custom" IncludeScriptsInPage="true" Image="Custom" ImageUrl="../../asset/v3.0.0/images/loading/ci-plane-loading.gif" class="loading-text" DockPosition="Right" Height="120px" Layout="ImageOnly" Text="Loading..." TextBold="true" TextColor="#d81159" TextItalic="true" TextSize="18px" TextFontName="" Position="Center"  Title="" Width="120px" runat="server" />
    <section class="mb-5">
        <ul class="nav nav-tabs" id="myTab" role="tablist">
            <li class="nav-item" role="presentation">
                <a class="nav-link active" id="basic-tab" data-toggle="tab" href="#basic" role="tab" aria-controls="basic" aria-selected="true">不同意原因基本設定</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="version-tab" data-toggle="tab" href="#version" role="tab" aria-controls="version" aria-selected="false">版本紀錄</a>
            </li>
        </ul>
        <!-- nav-tabs End -->
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane fade py-2 show active" id="basic" role="tabpanel" aria-labelledby="basic-tab">
                <div ID="divBootstrapAlert" Visible="false" runat="server" />
                <section class="vdform-has-mustfill  mb-3">
                    <!-- form col3 - vdform-group x6 -->
                    <div class="row  form-3col6group">
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblIDFcdsRejectReason" AssociatedControlID="IDFcdsRejectReason" CssClass="col-form-label  label-sm" Text="編號" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="IDFcdsRejectReason" CssClass="form-control  form-control-sm" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblRejectReason" runat="server">
                                <asp:Label ID="lblRejectReason" AssociatedControlID="RejectReason" CssClass="col-form-label  label-sm" Text="不同意原因" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12" id="divRejectReason" runat="server">
                                    <asp:TextBox ID="RejectReason" CssClass="form-control  form-control-sm" MaxLength="500" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblIsValidFlag" runat="server">
                                <asp:Label ID="lblIsValidFlag" AssociatedControlID="IsValidFlag" CssClass="col-form-label  label-sm" Text="有效否" runat="server" />
                            </div>
                            <div><%-- class="vdform-value"--%>
                                <div class="col-sm-12" id="divIsValidFlag" runat="server">
                                    <asp:RadioButtonList ID="IsValidFlag" CssClass="form-control  form-control-sm" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
                                        <asp:ListItem Text="是" Value="True" />
                                        <asp:ListItem Text="否" Value="False" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                    </div>
                    <!-- row end-->
                </section>
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="version" role="tabpanel" aria-labelledby="version-tab">
                <uc1:ChangeHistory ID="ChangeHistory1" runat="server" />
            </div><!-- tab-pane End-->
        </div>
        <!-- tab-content End-->
        <asp:Button ID="btnCheckSave" CausesValidation="false" CssClass="btn btn-primary-sp" OnClientClick="return openBootstrapModal('checkSaveDialogModal');" UseSubmitBehavior="false" Text="Confirm to Save" runat="server" data-target="" data-toggle="modal" />
        <%--<button type="button" class="btn btn-primary-sp" data-target="" data-toggle="modal" onclick="javascript:openBootstrapModal('checkSaveDialogModal');">Confirm to Save</button>--%>
        <input type="button" class="btn btn-outline-primary" value="Back" onclick="javascript:if (confirm('Are you sure to exit this page?')) {window.onbeforeunload=null; window.location='FcdsRejectReason_List.aspx'; return true;} else{return false;}" />
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
