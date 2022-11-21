<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FcdsApplyRoster.ascx.cs" Inherits="CockpitTripTrade.Module.Application.UserControl.UserControl_FcdsApplyRoster" %>

<div class="schedule-2switch">
    <asp:Repeater ID="repSchedule" runat="server" OnItemDataBound="repSchedule_ItemDataBound"><%-- OnPreRender="repSchedule_PreRender"--%>
        <HeaderTemplate>
            <header class="ss-header"><%-- row no-gutters--%>
                <div class="ss-header-item">
                    <span class="badge badge-pill badge-primary">Applicant</span>
                    <asp:Label ID="lblApplicant" CssClass="h4" runat="server" />
                    <%--<span class="h4">612345 李大仁 </span>--%>
                </div>
                <div class="ss-header-item">
                    <span class="badge badge-pill badge-primary">Respondent</span>
                    <asp:Label ID="lblRespondent" CssClass="h4" runat="server" />
                    <%--<span class="h4">623456 王大明 </span>--%>
                </div>
            </header>
            <%--<div class="slimScrollDiv" style="position: relative; overflow: hidden; width: auto; height: 650px;">--%>
            <div class="ss-main" style="height: 650px;">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="ss-row">
                <div id="divApplicantRoster" class="ss-1pdata" runat="server"><%--   ss-unselectable--%>
                    <div class="ss-pickdate">
                        <div class="form-check  form-check-inline">
                            <asp:CheckBox ID="chkApplicantDate" CssClass="form-check-input" runat="server" />
                            <asp:Label ID="lblApplicantDate" AssociatedControlID="chkApplicantDate" CssClass="form-check-label" runat="server" />
                            <asp:HiddenField ID="hfApplicantRosterDate" runat="server" />
                            <%--
                            <input class="form-check-input" type="checkbox" value="" id="person1-0901">
                            <label class="form-check-label" for="person1-0901">
                                Sep 01
                            </label>
                            --%>
                        </div>

                    </div>
                    <div class="ss-duties">

                        <asp:Repeater ID="repApplicantRosterLegs" runat="server" OnItemDataBound="repApplicantRosterLegs_ItemDataBound">
                            <HeaderTemplate></HeaderTemplate>
                            <ItemTemplate>
                                <div class="ss-flight">
                                    <div class="ssf-row">
                                        <asp:Label ID="lblApplicantFlightNo" CssClass="ss-flightno" runat="server" />
                                        <span class="ssflight-route">
                                            <asp:Label ID="lblApplicantDep" CssClass="ss-fr-station" runat="server" />
                                            <asp:Label ID="lblApplicantTo" Text="-" Visible="false" runat="server" /><%--<span> - </span>--%>
                                            <asp:Label ID="lblApplicantArr" CssClass="ss-fr-station" runat="server" />
                                        </span>
                                        <%--
                                        <span class="ss-flightno">CI0120</span>
                                        <span class="ssflight-route">
                                            <b class="ss-fr-station">TPE</b>
                                            <span> - </span>
                                            <b class="ss-fr-station">OKA</b>
                                        </span>
                                        --%>
                                    </div>
                                    <span class="ss-rptime-block" Visible="false">
                                        <asp:Label ID="lblApplicantRpt" Text="RPT: " Visible="false" runat="server" /><%--RPT : --%>
                                        <asp:Label ID="lblApplicantRptTime" CssClass="ss-rptime" runat="server" /><%--<span class="ss-rptime">07:00</span>--%>
                                        <asp:Label ID="lblApplicantDbf" Text="RLS: " Visible="false" runat="server" />
                                        <asp:Label ID="lblApplicantDbfTime" CssClass="ss-rptime" runat="server" />
                                    </span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    <%--<div class="ss-flight">
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
                <div id="divRespondentRoster" class="ss-1pdata" runat="server"><%--   ss-unselectable--%>
                    <div class="ss-pickdate">
                        <div class="form-check  form-check-inline">
                            <asp:CheckBox ID="chkRespondentDate" CssClass="form-check-input" runat="server" />
                            <asp:Label ID="lblRespondentDate" AssociatedControlID="chkRespondentDate" CssClass="form-check-label" runat="server" />
                            <asp:HiddenField ID="hfRespondentRosterDate" runat="server" />
                            <%--
                            <input class="form-check-input" type="checkbox" value="" id="person1-0901">
                            <label class="form-check-label" for="person1-0901">
                                Sep 01
                            </label>
                            --%>
                        </div>

                    </div>
                    <div class="ss-duties">

                        <asp:Repeater ID="repRespondentRosterLegs" runat="server" OnItemDataBound="repRespondentRosterLegs_ItemDataBound">
                            <HeaderTemplate></HeaderTemplate>
                            <ItemTemplate>
                                <div class="ss-flight">
                                    <div class="ssf-row">
                                        <asp:Label ID="lblRespondentFlightNo" CssClass="ss-flightno" runat="server" />
                                        <span class="ssflight-route">
                                            <asp:Label ID="lblRespondentDep" CssClass="ss-fr-station" runat="server" />
                                            <asp:Label ID="lblApplicantTo" Text="-" Visible="false" runat="server" /><%--<span> - </span>--%>
                                            <asp:Label ID="lblRespondentArr" CssClass="ss-fr-station" runat="server" />
                                        </span>
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
                                        <asp:Label ID="lblRespondentRpt" Text="RPT: " Visible="false" runat="server" /><%--RPT : --%>
                                        <asp:Label ID="lblRespondentRptTime" CssClass="ss-rptime" runat="server" /><%--<span class="ss-rptime">07:00</span>--%>
                                        <asp:Label ID="lblRespondentDbf" Text="RLS: " Visible="false" runat="server" />
                                        <asp:Label ID="lblRespondentDbfTime" CssClass="ss-rptime" runat="server" />
                                    </span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div><!-- ss-1pdata End-->
            </div><!-- ss-row End-->
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</div>
<!-- schedule-2switch End-->

<%--
<script type="text/javascript">
    $(function () {
    });

    function onMouseOverRosterGroup(className) {
        if (typeof (className) == "string") {
            var elements = document.getElementsByClassName(className);
            for (var i = 0; i < elements.length; i++) {
                if (!$("#" + elements[i].id).hasClass("ss-selected")) {
                    $("#" + elements[i].id).addClass("ss-js-group-hover");
                }
            }
        }
    }

    function onMouseOutRosterGroup(className) {
        if (typeof (className) == "string") {
            var elements = document.getElementsByClassName(className);
            for (var i = 0; i < elements.length; i++) {
                if (!$("#" + elements[i].id).hasClass("ss-selected")) {
                    $("#" + elements[i].id).removeClass("ss-js-group-hover");
                }
            }
        }
    }
    function onClickRosterGroup(className) {
        if (typeof (className) == "string") {
            var elements = document.getElementsByClassName(className);
            for (var i = 0; i < elements.length; i++) {
                if ($("#" + elements[i].id).hasClass("ss-selected")) {
                    $("#" + elements[i].id).removeClass("ss-selected").find(".ss-pickdate  :checkbox").prop("checked", false);
                }
                else {
                    $("#" + elements[i].id).addClass("ss-selected").find(".ss-pickdate  :checkbox").prop("checked", true);
                }
            }
        }
    }
</script>
--%>