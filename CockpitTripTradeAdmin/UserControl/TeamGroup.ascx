<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeamGroup.ascx.cs" Inherits="CockpitTripTradeAdmin.UserControl.UserControl_TeamGroup" %>
<%@ Register Src="~/UserControl/SelectEmployee.ascx" TagName="SelectEmployee" TagPrefix="uc1" %>

<div class="table-responsive">
    <asp:GridView ID="gvTeamGroup" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" DataKeyNames="IDFlowTeam, IDRole, RoleCode" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" ShowHeaderWhenEmpty="true" Width="80%" runat="server" OnPreRender="gvTeamGroup_PreRender" OnRowDataBound="gvFlowTeam_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderStyle-Width="5%" HeaderText="No." ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <asp:Label ID="No" CssClass="col-form-label  label-sm" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-Width="20%" HeaderText="Role" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <asp:Label ID="RoleName" CssClass="col-form-label  label-sm" Text='<%# DataBinder.Eval(Container.DataItem, "RoleName") %>' runat="server" />
                    <asp:HiddenField ID="RequireCode" Value='<%# DataBinder.Eval(Container.DataItem, "RequireCode") %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:BoundField DataField="RoleName" HeaderStyle-Width="15%" HeaderText="Role" />--%>
            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="55%" HeaderText="Select an Employee">
                <ItemTemplate>
                    <uc1:SelectEmployee ID="SelectEmployee1" runat="server" />
                    <%--Enable='<%# Bind("Enable") %>'--%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>