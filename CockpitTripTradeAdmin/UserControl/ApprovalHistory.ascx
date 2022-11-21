<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalHistory.ascx.cs" Inherits="CockpitTripTradeAdmin.UserControl.UserControl_ApprovalHistory" %>

<div class="table-responsive">
    <asp:GridView ID="gvApprovalHistory" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" DataKeyNames="" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" ShowHeaderWhenEmpty="true" Width="85%" runat="server" OnPreRender="gvApprovalHistory_PreRender" OnRowDataBound="gvApprovalHistory_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderStyle-Width="5%" HeaderText="No." ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <asp:Label ID="No" CssClass="col-form-label  label-sm" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Task" HeaderStyle-Width="10%" HeaderText="Task" />
            <asp:BoundField DataField="Signee" HeaderStyle-Width="25%" HeaderText="Signee" />
            <asp:BoundField DataField="Action" HeaderStyle-Width="10%" HeaderText="Action" />
            <%--<asp:BoundField DataField="Comments" HeaderStyle-Width="10%" HeaderText="Comments" />--%>
            <asp:BoundField DataField="StartTime" HeaderStyle-Width="15%" HeaderText="Start Time" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField DataField="EndTime" HeaderStyle-Width="15%" HeaderText="End Time" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField DataField="Duration" DataFormatString="{0:F1}" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Right" HeaderText="Duration(h)" />
        </Columns>
    </asp:GridView>
</div>