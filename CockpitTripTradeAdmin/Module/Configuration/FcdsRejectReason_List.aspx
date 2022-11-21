﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="FcdsRejectReason_List.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Configuration.CockpitTripTradeAdmin_Module_Configuration_FcdsRejectReason_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>
        <asp:Button ID="btnNew" CssClass="btn btn-primary-sp" Text="New" runat="server" OnClick="btnNew_Click" />
    </p>
    <div class="table-responsive">
        <asp:GridView ID="gvList" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" ShowHeaderWhenEmpty="true" runat="server" Width="100%" OnPreRender="gvList_PreRender" OnRowDataBound="gvList_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:HyperLink ID="Control" CssClass="btn btn-primary btn-sm" NavigateUrl='<%# "FcdsRejectReason.aspx?IDFcdsRejectReason=" + Eval("IDFcdsRejectReason") %>' runat="server" />
                        <%--<a class="btn btn-primary btn-sm" href="FcdsRejectReason.aspx?IDFcdsRejectReason=<%# DataBinder.Eval(Container, "DataItem.IDFcdsRejectReason") %>">Edit</a>--%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="No." ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="RejectReason" HeaderText="不同意原因" />
                <asp:BoundField DataField="IsValidFlag" HeaderText="有效否" ItemStyle-HorizontalAlign="Center" />
                <%--<asp:BoundField DataField="CreateBy" HeaderText="建立者" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="CreateStamp" HeaderText="建立時間" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />--%>
                <asp:BoundField DataField="UpdateBy" HeaderText="更新者" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="UpdateStamp" HeaderText="更新時間" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
