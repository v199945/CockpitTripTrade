<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="NewFcdsApply.aspx.cs" Inherits="CockpitTripTrade.Module.Application.CockpitTripTrade_Module_Application_NewFcdsApply" %>

<%@ Register Src="~/Module/Application/UserControl/FcdsApplyRoster.ascx" TagName="FcdsApplyRoster" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/ValidationSummary.ascx" TagName="ValidationSummary" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/ChangeHistory.ascx" TagName="ChangeHistory" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../../asset/plug-in/bs-stepper/bs-stepper.min.css">
    <script src="../../asset/v3.0.0/js/page-fcrewSwitch.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:ValidationSummary ID="ValidationSummary1" runat="server" />
    <BusyBoxDotNet:BusyBox ID="BusyBox1" BorderWidth="0" ShowBusyBox="Custom" IncludeScriptsInPage="true" Image="Custom" ImageUrl="../../asset/v3.0.0/images/loading/ci-plane-loading.gif" class="loading-text" DockPosition="Right" Height="120px" Layout="ImageOnly" Text="Loading..." TextBold="true" TextColor="#d81159" TextItalic="true" TextSize="18px" TextFontName="" Position="Center"  Title="" Width="120px" runat="server" />

    <div class="bs-stepper">
        <%--20221108 648267:修正PC跑版/--%>
        <div class="bs-stepper-header d-none d-md-flex" role="tablist">
            <!-- your steps here -->
            <div class="step" data-target="#select-respondent-month">
                <button type="button" class="step-trigger" role="tab">
                <span class="bs-stepper-circle">1</span>
                <span class="bs-stepper-label">Select Crew and Month</span>
                </button>
            </div>
            <div class="line"></div>
            <div class="step" data-target="#tick-duties">
                <button type="button" class="step-trigger" role="tab">
                <span class="bs-stepper-circle">2</span>
                <span class="bs-stepper-label">Tick Duties</span>
                </button>
            </div>
            <div class="line"></div>
            <div class="step" data-target="#finished">
                <button type="button" class="step-trigger" role="tab">
                <span class="bs-stepper-circle">3</span>
                <span class="bs-stepper-label">Finished</span>
                </button>
            </div>
        </div>

        <div class="bs-stepper-content">
            <!-- your steps content here -->
            <div id="select-respondent-month" class="content" role="tabpanel">
                <div class="form-group">
                    <section class="vdform-has-mustfill mb-3">
                        <div class="row  form-2col">
                            <div class="col-lg-6  vdform-group">
                                <div class="vdform-prop  vdform-prop-md">
                                    <asp:Label ID="lblApplicationDate" AssociatedControlID="ApplicationDate" CssClass="col-form-label  label-sm" Text="Application Date" runat="server" />
                                </div>
                                <div class="vdform-value">
                                    <div class="col-sm-12">
                                        <asp:TextBox ID="ApplicationDate" CssClass="form-control  form-control-sm" runat="server" ReadOnly="true" />
                                    </div>  
                                </div>
                            </div><!-- vdform-group end-->
                            <div class="col-lg-6  vdform-group">
                                <div class="vdform-prop  vdform-prop-md" id="divLblSwapScheduleMonth" runat="server">
                                    <span class="vdform-mustfill">*</span>
                                    <asp:Label ID="lblSwapScheduleMonth" AssociatedControlID="SwapScheduleMonth" CssClass="col-form-label  label-sm" Text="Swapping Month" runat="server" />
                                </div>
                                <div class="vdform-value">
                                    <div class="col-sm-12" id="divSwapScheduleMonth" runat="server">
                                        <asp:RadioButtonList ID="SwapScheduleMonth" CssClass="form-control  form-control-sm" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" />
                                        <asp:RequiredFieldValidator ID="rfvSwapScheduleMonth" ControlToValidate="SwapScheduleMonth" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Swap Month ] is required!" SetFocusOnError="true" ValidationGroup="Respondent" runat="server" />
                                    </div>  
                                </div>
                            </div><!-- vdform-group end-->
                        </div><!-- row end-->
                        <div class="row  form-2col">
                            <div class="col-lg-6  vdform-group">
                                <div class="vdform-prop  vdform-prop-md">
                                    <asp:Label ID="lblApplicantID" AssociatedControlID="ApplicantID" CssClass="col-form-label  label-sm" Text="Applicant" runat="server" />
                                </div>
                                <div class="vdform-value">
                                    <div class="col-sm-12">
                                        <asp:TextBox ID="ApplicantID" CssClass="form-control  form-control-sm" runat="server" readonly="true"/>
                                    </div>  
                                </div>
                            </div><!-- vdform-group end-->
                            <div class="col-lg-6  vdform-group">
                                <div class="vdform-prop  vdform-prop-md" id="divLblRespondentID" runat="server">
                                    <span class="vdform-mustfill">*</span>
                                    <asp:Label ID="lblRespondentID" AssociatedControlID="RespondentID" CssClass="col-form-label  label-sm" Text="Appointee" runat="server" />
                                </div>
                                <div class="vdform-value  form-inline no-gutters">
                                    <div class="col-sm-12 row no-gutters" id="divRespondentID" runat="server">
                                        <asp:TextBox ID="RespondentID" CssClass="form-control  form-control-sm col-9 col-md-6 mr-2" MaxLength="6" placeholder="Input Flight Crew ID and click the Verify button" runat="server" />
                                        <asp:Button ID="btnQryRespondentID" CausesValidation="false" CssClass="btn btn-primary btn-sm" Text="Verify" ValidationGroup="Respondent" runat="server" OnClick="btnQryRespondentID_Click" />

                                        <asp:UpdatePanel ID="upRespondentInfo" UpdateMode="Conditional" runat="server">
                                            <ContentTemplate>
                                                <asp:Label ID="RespondentInfo" runat="server" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnQryRespondentID" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                        <asp:CompareValidator ID="cvRespondentID" ControlToValidate="RespondentID" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Respondent Crew ID ] cannot be equal to yourself!" Operator="NotEqual" SetFocusOnError="true" Type="String" ValidationGroup="Respondent" runat="server" />
                                        <%--<asp:CustomValidator ID="cstvRespondentID" ClientValidationFunction="ClientValidateRespondentInfo" ControlToValidate="RespondentID" CssClass="invalid-feedback" Display="Dynamic" OnServerValidate="cstvRespondentID_ServerValidate" SetFocusOnError="true" runat="server" />--%>
                                        <%--ClientValidationFunction="CheckRespondentID" EnableClientScript="false"--%>
                                        <asp:RegularExpressionValidator ID="revRespondentID" ControlToValidate="RespondentID" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Respondent Crew ID ] must be 6 digits!" SetFocusOnError="true" ValidationExpression="^\d{6}$" ValidationGroup="Respondent" runat="server" />
                                        <asp:RequiredFieldValidator ID="rfvRespondentID" ControlToValidate="RespondentID" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Respondent Crew ID ] is required!" SetFocusOnError="true" ValidationGroup="Respondent" runat="server" />
                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbeRespondentID" FilterType="Custom" TargetControlID="RespondentID" ValidChars="0123456789" runat="server" />
                                    </div>  
                                </div>
                            </div><!-- vdform-group end-->
                        </div><!-- row end-->

                    </section>
                </div>
                <div>
                    <asp:Button ID="btnSelectRrespondentMonthNext" CausesValidation="false" CssClass="btn btn-primary-sp" ValidationGroup="Respondent" Text="Next" runat="server" OnClick="btnSelectRrespondentMonthNext_Click" />
                    <%--<button type="button" class="btn btn-primary" onclick="stepper.next()">Next</button>--%>
                </div>
            </div>
            <div id="tick-duties" class="content" role="tabpanel">
                <div class="form-group">
                    <asp:UpdatePanel ID="upNext" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <uc1:FcdsApplyRoster ID="FcdsApplyRoster1" runat="server" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSelectRrespondentMonthNext" EventName="Click" /><%--btnQryRespondentID--%>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div>
                    <%--<button type="button" class="btn btn-primary" onclick="stepper.previous()">Previous</button>
                    <button type="button" class="btn btn-primary-sp" onclick="simulateSubmit()">Client Submit</button>--%>
                    <asp:Button ID="btnTickDutiesPrevious" CausesValidation="false" CssClass="btn btn-primary" OnClientClick="stepper.previous()" Text="Previous" runat="server" />
                    <button id="checkSave" type="button" class="btn btn-primary-sp" data-target="#checkSaveDialogModal" data-toggle="modal"">Confirm to Submit</button>
                </div>                
            </div>
            <div id="finished" class="content" role="tabpanel">
                <div class="alert alert-success" role="alert">
                    <strong>Success!</strong> Your duty swap request application of Form No. <asp:HyperLink ID="hlIDFcdsApply" runat="server" /> has been submitted to <asp:Label ID="lblRespondent" CssClass="vdform-plaintext" runat="server" /> successfully!
                </div>
            </div>
        </div>
    </div>
    
    <div id="checkSaveDialogModal" aria-labelledby="checkSaveDialogLabel" class="modal fade" data-backdrop="static" role="dialog" tabindex="-1" style="display: none;"><%-- aria-hidden="true" padding-right: 17px;--%>
		<div class="modal-dialog modal-dialog-centered">
			<div class="modal-content">
				<div class="modal-header">
					<h4 id="checkSaveDialogLabel" class="modal-title text-center w-100">Confirm to Submit?</h4>
					<button aria-label="Close" class="close" data-dismiss="modal" type="button">
						<span aria-hidden="true">×</span>
					</button>
				</div>
				<div class="modal-body">
                    <asp:UpdatePanel ID="upSubmitTo" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <p>Are you sure to Submit to <asp:Label ID="lblSubmitTo" CssClass="text-info" runat="server" /></p>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSelectRrespondentMonthNext" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
				</div>
				<div class="modal-footer justify-content-center">
                    <asp:Button ID="btnSave" CausesValidation="false" CssClass="btn btn-primary-sp" Text="Submit" runat="server" OnClick="btnSave_Click" />
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
					<button class="btn btn-primary" data-dismiss="modal" data-toggle="modal" tabindex="0" type="button">Confirm</button>
				</div>
			</div>
		</div>
    </div>

    <script src="../../asset/plug-in/bs-stepper/bs-stepper.min.js"></script>
    <script type="text/javascript">
        var stepperElem = document.querySelector('.bs-stepper'); // 回傳 document 選到的第一個 "bs-stepper" class
        var stepper = new Stepper(stepperElem);
        var done = false;
        var currStep = 1;
        history.pushState(currStep, '');

        // 切換到步驟前觸發，呼叫e.preventDefault()可阻止切換
        stepperElem.addEventListener("show.bs-stepper", function (e) {
            if (done) { //若程序完成，不再切換
                e.preventDefault();
                return;
            }
        });

        // 切換到步驟後觸發，e.detail.indexStep 為目前步驟序號(從0開始)
        stepperElem.addEventListener("shown.bs-stepper", function (e) {
            var idx = e.detail.indexStep + 1;
            currStep = idx;
            // pushState() 記下歷程以支援瀏覽器回上頁功能
            history.pushState(idx, '');
        })

        // 瀏覽器上一頁下一頁觸發
        window.onpopstate = function (e) {
            if (e.state && e.state != currStep)
                stepper.to(e.state);
        };

        // 模擬送出表單，註記已完成，不再允許切換步驟
        function simulateSubmit() {
            stepper.next();
            done = true;
        }
    </script>

    <script type="text/javascript">
        function CheckRespondentID() {
            var oRespondentInfo = document.getElementById('<%= this.RespondentInfo.ClientID %>');
            if (oRespondentInfo != null && oRespondentInfo != undefined) {
                oRespondentInfo.innerText = "";
            }
            
            var aryRespondentValidators = new Array(document.getElementById('<%= this.cvRespondentID.ClientID %>'), document.getElementById('<%= this.revRespondentID.ClientID %>'), document.getElementById('<%= this.rfvRespondentID.ClientID %>'), document.getElementById('<%= this.rfvSwapScheduleMonth.ClientID %>'));
            for (i = 0; i < aryRespondentValidators.length; i++) {
                RequiredFieldValidator_CheckValidControl(aryRespondentValidators[i].id, aryRespondentValidators[i].controltovalidate);
                if (!aryRespondentValidators[i].isvalid) {
                    break;
                }
            }
        }

        <%--
        CheckRespondentInfo
        function ClientValidateRespondentInfo(source, args) {
            var oRespondentInfo = document.getElementById('<%= this.RespondentInfo.ClientID %>');
            if (oRespondentInfo != null && oRespondentInfo != undefined && oRespondentInfo.innerText == "") {
                args.isvalid = false;
                //alert('Please press the [Verify] button after inputing the Respondent ID!');
                //return false;
            }
            else {
                args.isvalid = true;
            }

            //return true;
        }

        function CheckRespondentID1(sender, args) {
            args.IsValid = true;

            var oApplicantID = document.getElementById('<%= this.ApplicantID.ClientID %>');
            if (oApplicantID != null && oApplicantID != undefined) {
                if (args.Value == oApplicantID.value.substring(0, 6)) {
                    args.IsValid = false;
                }

                const cRegExpPattern = /^\d{6}$/;
                if (args.Value.match(cRegExpPattern) == null) {
                    args.IsValid = false;
                }
            }

            var sRespondentID = '<%= this.RespondentID.ClientID %>';
            if (args.IsValid) {
                $("#" + sRespondentID + "").removeClass("is-invalid");
                $("#" + sRespondentID + "").addClass("is-valid");
            }
            else {
                $("#" + sRespondentID + "").removeClass("is-valid");
                $("#" + sRespondentID + "").addClass("is-invalid");
            }
        }--%>

        <%--window.onbeforeunload = function () {
            return "★★ 您尚未將編輯過的表單資料儲存，請問您確定要離開此頁面嗎？ ★★";
        };--%>
    </script>
</asp:Content>
