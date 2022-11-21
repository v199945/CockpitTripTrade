<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="ImpersonateUser.aspx.cs" Inherits="CockpitTripTrade.Module.Admin.CockpitTripTrade_Module_Admin_ImpersonateUser" %>

<%@ Register Src="~/UserControl/ValidationSummary.ascx" TagName="ValidationSummary" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <BusyBoxDotNet:BusyBox ID="BusyBox1" BorderWidth="0" ShowBusyBox="Custom" IncludeScriptsInPage="true" Image="Custom" ImageUrl="../../asset/v3.0.0/images/loading/ci-plane-loading.gif" class="loading-text" DockPosition="Right" Height="120px" Layout="ImageOnly" Text="Loading..." TextBold="true" TextColor="#d81159" TextItalic="true" TextSize="18px" TextFontName="" Position="Center"  Title="" Width="120px" runat="server" />
        <section class="vdform-has-mustfill">
            <div class="row  form-2col">
                <div class="col-lg-6  vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                        <span class="vdform-mustfill">*</span>
                        <asp:Label ID="lblImpersonate" AssociatedControlID="Impersonate" CssClass="col-form-label  label-sm" Text="Crew ID" runat="server" />
                    </div>
                    <div class="vdform-value">
                        <div class="col-sm-12">
                            <asp:TextBox ID="Impersonate" CssClass="form-control  form-control-sm" MaxLength="6" placeholder="Please input Crew ID" runat="server" />
                            <asp:RegularExpressionValidator ID="revImpersonate" ControlToValidate="Impersonate" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Crew ID ] must be 6 digits!" SetFocusOnError="true" ValidationExpression="^\d{6}$" runat="server" />
                            <asp:RequiredFieldValidator ID="rfvImpersonate" ControlToValidate="Impersonate" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Crew ID ] is required!" SetFocusOnError="true" runat="server" />
                            <ajaxToolkit:FilteredTextBoxExtender ID="ftbeImpersonate" FilterType="Custom" TargetControlID="Impersonate" ValidChars="0123456789" runat="server" />
                        </div>  
                    </div>                           
                </div><!-- vdform-group end-->
            </div><!-- row end-->

            <button id="checkImpersonate" type="button" class="btn btn-primary-sp" data-target="" data-toggle="modal" onclick="javascript:openBootstrapModal('checkImpersonateDialogModal');">Confirm to Impersonate</button>
            <asp:Button ID="btnCancel" CausesValidation="false" CssClass="btn btn-outline-primary" Text="Cancel" runat="server" OnClick="btnCancel_Click" />
        </section>
        <div id="checkImpersonateDialogModal" aria-labelledby="checkImpersonateDialogLabel" class="modal fade" role="dialog" tabindex="-1" style="display: none;">
		    <div class="modal-dialog modal-dialog-centered">
			    <div class="modal-content">
				    <div class="modal-header">
					    <h4 id="checkImpersonateDialogLabel" class="modal-title text-center w-100">Confirm to Impersonate?</h4>
					    <button aria-label="Close" class="close" data-dismiss="modal" type="button">
						    <span aria-hidden="true">×</span>
					    </button>
				    </div>
				    <div class="modal-body">
					    <p>Are you sure to impersonate <span id="spanImpersonate" class="text-info"></span> ?</p>
				    </div>
				    <div class="modal-footer justify-content-center">
                        <asp:Button ID="btnImpersonate" CausesValidation="false" CssClass="btn btn-primary-sp" Text="Impersonate" runat="server" OnClick="btnImpersonate_Click" />
					    <button class="btn btn-outline-primary" data-dismiss="modal" type="button">Cancel</button>
				    </div>
			    </div>
		    </div>
        </div>
        
        <script type="text/javascript">
            $(document).ready(init);

            function init() {
                $('#checkImpersonate').click(function (e) {
                    $('#spanImpersonate').text($('#<%= this.Impersonate.ClientID %>').val());
                });
            }
        </script>
</asp:Content>
