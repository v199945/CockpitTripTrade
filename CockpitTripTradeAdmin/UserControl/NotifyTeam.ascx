<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NotifyTeam.ascx.cs" Inherits="CockpitTripTrade.UserControl.UserControl_NotifyTeam" %>
<%@ Register Src="~/UserControl/SelectEmployee.ascx" TagName="SelectEmployee" TagPrefix="uc1" %>


<asp:GridView ID="gvNotifyTeam" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" DataKeyNames="IDFlowTeam, IDRole, RoleCode" EmptyDataRowStyle-HorizontalAlign="Center" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" ShowHeaderWhenEmpty="true" Width="60%" runat="server" OnRowDataBound="gvFlowTeam_RowDataBound">
    <Columns>
        <asp:TemplateField HeaderStyle-Width="5%" HeaderText="No." ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
            <ItemTemplate>
                <asp:Label ID="No" CssClass="col-sm-4 col-form-label" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-Width="15%" HeaderText="Role" ItemStyle-VerticalAlign="Middle">
            <ItemTemplate>
                <asp:Label ID="RoleName" CssClass="col-sm-4 col-form-label" Text='<%# DataBinder.Eval(Container.DataItem, "RoleName") %>' runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <%--<asp:BoundField DataField="RoleName" HeaderStyle-Width="15%" HeaderText="Role" />--%>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="40%" HeaderText="Select an Employee">
            <ItemTemplate>
                <uc1:SelectEmployee ID="SelectEmployee1" runat="server" />
                <%--Enable='<%# Bind("Enable") %>'--%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>