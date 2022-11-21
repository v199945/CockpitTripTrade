<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FcdsApply_Auto.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Schedule.CockpitTripTradeAdmin_Module_Schedule_FcdsApply_Auto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>FCDS Application System Schedule Auto</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="table-responsive  mb-5">
            <table class="table table-bordered table-striped table-hover  table-sm  table-style01" style="width: 50%;">
                <colgroup>
                    <col style="width: 15%;" />
                    <col style="width: 35%;" />
                </colgroup>
                <thead>
                    <tr>
                        <th scope="col" colspan="2" style="text-align: left;">系統自動完成逾期之申請表單</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>FcdsApplyID(申請單單號)</td>
                        <td>
                            <asp:TextBox ID="IDFcdsApply" MaxLength="18" runat="server" />
                            <asp:Button ID="btnCheckStatus" Text="Check Status" OnClick="btnCheckStatus_Click" runat="server" />                            
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"><asp:Label ID="lblFcdsApplyInfo" runat="server" /></td>
                    </tr>
                    <tr id="IsTestEmail" runat="server" visible="false">
                        <td>IsTestEMail(是否為測試郵件)</td>
                        <td>
                            <asp:CheckBox ID="ckbIsTestEmail" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btnVoidFcdsApply" Text="VoidByFcdsApply" OnClick="btnVoidFcdsApply_Click" runat="server" />
                        </td>
                    </tr>
                </tbody>
            </table>
            <p></p>
            <table class="table table-bordered table-striped table-hover  table-sm  table-style01" style="width: 50%;">
                <colgroup>
                    <col style="width: 15%;" />
                    <col style="width: 35%;" />
                </colgroup>
                <thead>
                    <tr>
                        <th scope="col" colspan="2" style="text-align: left;">測試寄送電子郵件</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Email Address</td>
                        <td>
                            <asp:TextBox ID="Email" runat="server" />
                            <asp:Button ID="btnTestSendMail" CausesValidation="false" Text="Test Send Mail" runat="server" OnClick="btnTestSendMail_Click" />                        
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
