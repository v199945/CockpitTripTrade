<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectEmployee.ascx.cs" Inherits="CockpitTripTradeAdmin.UserControl.UserControl_SelectEmployee" %>
<%-- form-control  form-control-sm  col-4  mr-3  is-invalid--%>
<div class="row no-gutters">
    <asp:DropDownList ID="Employee" AppendDataBoundItems="true" CssClass="form-control  form-control-sm  col-4  mr-3" DataTextField="DisplayName" DataValueField="EmployID" runat="server">
        <asp:ListItem Text="---請選擇---" Value="" />
    </asp:DropDownList>
    <%-- AutoPostBack="false"--%>
    <%----%>
    <div class="col-6 invalid-feedback">
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Enabled="false" ControlToValidate="Employee" Display="Dynamic" runat="server" />
    <%-- CssClass="invalid-feedback"--%>
    </div>
</div>