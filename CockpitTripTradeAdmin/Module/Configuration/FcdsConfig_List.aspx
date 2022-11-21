<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="FcdsConfig_List.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Configuration.CockpitTripTradeAdmin_Module_Configuration_FcdsConfig_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="table-responsive">
        <asp:GridView ID="gvList" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" ShowHeaderWhenEmpty="true" runat="server" Width="100%" OnPreRender="gvList_PreRender" OnRowDataBound="gvList_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:HyperLink ID="Control" CssClass="btn btn-primary btn-sm" NavigateUrl='<%# "FcdsConfig.aspx?IDAcType=" + Eval("AcType") + "&IDCrewPos=" + Eval("CrewPos") %>' runat="server" />
                        <%--<a class="btn btn-primary btn-sm" href="FcdsConfig.aspx?IDAcType=<%# DataBinder.Eval(Container.DataItem, "AcType") %>&IDCrewPos=<%# DataBinder.Eval(Container.DataItem, "CrewPos") %>">Edit</a>--%>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:HyperLinkField DataNavigateUrlFields="AcType, CrewPos" DataNavigateUrlFormatString="FcdsConfig.aspx?IDAcType={0}&IDCrewPos={1}" HeaderText="Control" ItemStyle-CssClass="btn btn-primary btn-sm" ItemStyle-HorizontalAlign="Center" />--%>
                <asp:TemplateField HeaderText="No." ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FleetCode" HeaderText="機隊" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="CrewPosCode" HeaderText="職級" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="NumOfMonth" HeaderText="每月申請次數" ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="IsOneReqATime" HeaderText="一次一單否" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="IsApplicantRevoke" HeaderText="可撤回否" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="LeadWorkdays" HeaderText="前置工作天" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="DeadlineOfAcrossMonth" HeaderText="跨月班期限" ItemStyle-HorizontalAlign="Center" />
                <%--<asp:BoundField DataField="CreateBy" HeaderText="建立者" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="CreateStamp" HeaderText="建立時間" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />--%>
                <asp:BoundField DataField="UpdateBy" HeaderText="更新者" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="UpdateStamp" HeaderText="更新時間" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
