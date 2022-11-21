<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeHistory.ascx.cs" Inherits="CockpitTripTradeAdmin.UserControl.UserControl_ChangeHistory" %>

<div class="table-responsive">
    <table style="width: 100%">
        <tr>
            <td>
                <asp:GridView ID="gvChangeHistory" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" DataKeyNames="BranchID" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" ShowHeaderWhenEmpty="true" Width="85%" runat="server" OnRowDataBound="gvChangeHistory_RowDataBound" OnPreRender="gvChangeHistory_PreRender">
                    <Columns>
                        <asp:TemplateField HeaderText="No." ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:HyperLinkField DataTextField="Version" DataNavigateUrlFields="Version" HeaderText="版本" ItemStyle-HorizontalAlign="Center" />--%>
                        <asp:BoundField DataField="Version" HeaderText="Version" ItemStyle-HorizontalAlign="Center" />
                        <asp:TemplateField HeaderText="Creator" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="UpdateBy" CssClass="" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UpdateStamp" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" HeaderText="Date" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</div>