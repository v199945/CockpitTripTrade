<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FcdsApplyApprove.ascx.cs" Inherits="CockpitTripTrade.Module.Application.UserControl.CockpitTripTrade_Module_Application_UserControl_FcdsApplyApprove" %>

<h2>組員派遣部審核意見</h2>
<div class="row">
    <div class="col-lg-12   col-xl-8   col-xxl-6">
        <section class="vdrowcols-xs1  mb-3  vdform-has-mustfill">
            <div class="vdform-group">
                <div class="vdform-prop  vdform-prop-md" id="divLblIsApproval" runat="server">
                    <asp:Label ID="lblIsApproval" AssociatedControlID="IsApproval" CssClass="col-form-label  label-sm" Text="審核意見" runat="server" />
                </div>
                <div class="vdform-value">
                    <div class="col">
                        <asp:UpdatePanel ID="upIsApproval" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:RadioButtonList ID="IsApproval" AutoPostBack="true" CssClass="form-control  form-control-sm" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" OnSelectedIndexChanged="IsApproval_SelectedIndexChanged">
                                    <asp:ListItem Text="任務不受影響，同意互換" Value="True" />
                                    <asp:ListItem Text="任務受影響，擬不予調整" Value="False" />
                                </asp:RadioButtonList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div><!-- vdform-group end-->
        
            <div class="vdform-group">
                <div class="vdform-prop  vdform-prop-md" id="divLblRejectReason" runat="server">
                    <asp:UpdatePanel ID="upLblRejectReason" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <asp:Label ID="lblMustFillRejectReason" CssClass="vdform-mustfill" Text="*" Visible="false" runat="server" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="IsApproval" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:Label ID="lblRejectReason" AssociatedControlID="RejectReason" CssClass="col-form-label  label-sm" Text="不同意原因" runat="server" />
                </div>
                <div class="vdform-value">
                    <div class="col  vdform-2inputnmid-wrapper">
                        <asp:UpdatePanel ID="upRejectReason1" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="RejectReason" AppendDataBoundItems="true" CssClass="form-control  form-control-sm" DataTextField="RejectReason" DataValueField="IDFcdsRejectReason" Enabled="false" Width="100%" runat="server">
                                    <asp:ListItem Text="---Please Select---" Value="" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvRejectReason" ControlToValidate="RejectReason" CssClass="invalid-feedback" Display="Dynamic" Enabled="false" ErrorMessage="[ Reject Reason ] is required!" SetFocusOnError="true" runat="server" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="IsApproval" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div><!-- vdform-group end-->
        
        
            <div class="vdform-group">
                <div class="vdform-prop  vdform-prop-md" id="divLblComments" runat="server">
                    <asp:Label ID="lblCommnets" AssociatedControlID="Comments" CssClass="col-form-label  label-sm" Text="備註" runat="server" />
                    <%--<label for="give-input-id" class="col-form-label  label-sm">備註</label>--%>
                </div>
                <div class="vdform-value">
                    <div class="col">
                        <asp:TextBox ID="Comments" CssClass="form-control  form-control-sm" TextMode="MultiLine" runat="server" />
                    </div>
                </div>
            </div><!-- vdform-group end-->
        </section><!--  -->
    </div>
</div>