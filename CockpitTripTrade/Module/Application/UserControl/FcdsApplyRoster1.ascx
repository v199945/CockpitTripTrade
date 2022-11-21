<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FcdsApplyRoster1.ascx.cs" Inherits="UserControl_FcdsApplyRoster" %>

<div class="schedule-2switch">
    <asp:Repeater ID="repSchedule" runat="server">
        <HeaderTemplate>
            <header class="ss-header">
                <div class="ss-header-item">
                    <span class="badge badge-pill badge-primary">Applicant</span>
                    <asp:Label ID="Applicant" CssClass="h4" runat="server" />
                    <%--<span class="h4">612345 李大仁 </span>--%>
                </div>
                <div class="ss-header-item">
                    <span class="badge badge-pill badge-primary">Respondent</span>
                    <asp:Label ID="Respondent" CssClass="h4" runat="server" />
                    <%--<span class="h4">623456 王大明 </span>--%>
                </div>
            </header>
            <%--<div class="slimScrollDiv" style="position: relative; overflow: hidden; width: auto; height: 650px;">--%>
            <div class="ss-main" style="height: 650px;">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="ss-row">
                <div class="ss-1pdata"><%--   ss-unselectable--%>
                    <div class="ss-pickdate">
                        <div class="form-check  form-check-inline">
                            <asp:CheckBox ID="chkApplicantDate" CssClass="form-check-input" runat="server" />
                            <asp:Label ID="lblApplicantDate" AssociatedControlID="chkApplicantDate" CssClass="form-check-label" runat="server" />
                            <%--
                            <input class="form-check-input" type="checkbox" value="" id="person1-0901">
                            <label class="form-check-label" for="person1-0901">
                                Sep 01
                            </label>
                            --%>
                        </div>

                    </div>
                    <div class="ss-duites">
                                            
                        <div class="ss-flight">
                            <div class="ssf-row">
                                <asp:Label ID="lblApplicantFlightNo" CssClass="ss-flightno" runat="server" />
                                <asp:Label ID="lblApplicantRoute" CssClass="ssflight-route" runat="server">
                                    <asp:Label ID="lblApplicantDep" CssClass="ss-fr-station" runat="server" />
                                    <span> - </span>
                                    <asp:Label ID="lblApplicantArr" CssClass="ss-fr-station" runat="server" />
                                </asp:Label>
                                <%--
                                <span class="ss-flightno">CI0120</span>
                                <span class="ssflight-route">
                                    <b class="ss-fr-station">TPE</b>
                                    <span> - </span>
                                    <b class="ss-fr-station">OKA</b>
                                </span>
                                --%>
                            </div>
                            <span class="ss-rptime-block">
                                RPT : <asp:Label ID="lblApplicantRptTime" CssClass="ss-rpttime" runat="server" /><%--<span class="ss-rptime">07:00</span>--%>
                            </span>
                        </div>

<%--                                <div class="ss-flight">
                            <div class="ssf-row">
                                <span class="ss-flightno">CI0121</span>
                                <span class="ssflight-route">
                                    <b class="ss-fr-station">OKA</b>
                                    <span> - </span>
                                    <b class="ss-fr-station">TPE</b>
                                </span>
                            </div>
                            <span class="ss-rptime-block">
                                RPT : <span class="ss-rptime">14:00</span>
                            </span>
                        </div>--%>

                    </div>
                </div><!-- ss-1pdata End-->
                <div class="ss-1pdata"><%--   ss-unselectable--%>
                    <div class="ss-pickdate">
                        <div class="form-check  form-check-inline">
                            <asp:CheckBox ID="chkRespondentDate" CssClass="form-check-input" runat="server" />
                            <asp:Label ID="lblRespondentDate" AssociatedControlID="chkRespondentDate" CssClass="form-check-label" runat="server" />
                            <%--
                            <input class="form-check-input" type="checkbox" value="" id="person1-0901">
                            <label class="form-check-label" for="person1-0901">
                                Sep 01
                            </label>
                            --%>
                        </div>

                    </div>
                    <div class="ss-duites">
                                            
                        <div class="ss-flight">
                            <div class="ssf-row">
                                <asp:Label ID="lblRespondentFlightNo" CssClass="ss-flightno" runat="server" />
                                <asp:Label ID="lblRespondentRoute" CssClass="ssflight-route" runat="server">
                                    <asp:Label ID="lblRespondentDep" CssClass="ss-fr-station" runat="server" />
                                    <span> - </span>
                                    <asp:Label ID="lblRespondentArr" CssClass="ss-fr-station" runat="server" />
                                </asp:Label>
                                <%--
                                <span class="ss-flightno">CI0120</span>
                                <span class="ssflight-route">
                                    <b class="ss-fr-station">TPE</b>
                                    <span> - </span>
                                    <b class="ss-fr-station">OKA</b>
                                </span>
                                --%>
                            </div>
                            <span class="ss-rptime-block">
                                RPT : <asp:Label ID="lblRespondentRptTime" CssClass="ss-rpttime" runat="server" /><%--<span class="ss-rptime">07:00</span>--%>
                            </span>
                        </div>

                    </div>
                </div><!-- ss-1pdata End-->
            </div><!-- ss-row End-->
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</div>