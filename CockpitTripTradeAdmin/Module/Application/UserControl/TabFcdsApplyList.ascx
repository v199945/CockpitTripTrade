<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabFcdsApplyList.ascx.cs" Inherits="CockpitTripTradeAdmin.Module.Application.UserControl.CockpitTripTradeAdmin_Module_Application_UserControl_TabFcdsApplyList" %>

<section>
    <div class="table-responsive">
        <asp:GridView ID="gvList" AutoGenerateColumns="false" CssClass="table table-striped table-hover  table-style01  fcs-table-reviewlists" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" RowStyle-VerticalAlign="Middle" ShowHeaderWhenEmpty="true" runat="server" OnPreRender="gvList_PreRender" OnRowDataBound="gvList_RowDataBound">
            <EmptyDataTemplate>
                <div class="showresult">
                    <img class="showresult-icon" src="../../asset/v3.0.0/images/icons/img-status-no-result.svg" alt="">
                    <div class="showresult-info">
                        <h3>No search results</h3>
                        <p>We could not find any search results. We suggest you try a new search.</p>
                    </div>
                </div>
            </EmptyDataTemplate>
            <Columns>
                <%--<asp:TemplateField HeaderText="No." ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="檢視" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:HyperLink ID="Control" NavigateUrl='<%# "../FcdsApply.aspx?IDFcdsApply=" + Eval("IDFcdsApply") + "&ProID=" + Eval("ProID") %>' Text="檢視" runat="server" />
                        <%--<a class="btn btn-primary btn-sm" href="FcdsConfig.aspx?IDAcType=<%# DataBinder.Eval(Container.DataItem, "AcType") %>&IDCrewPos=<%# DataBinder.Eval(Container.DataItem, "CrewPos") %>">Edit</a>--%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="IDFcdsApply" HeaderText="單號" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="StatusCName" HeaderText="申請單狀態" ItemStyle-HorizontalAlign="Left" />
                <%--<asp:BoundField DataField="IsApproval" HeaderText="批核狀態" ItemStyle-HorizontalAlign="Left" />--%>
                <%--<asp:BoundField DataField="SwapScheduleMonth" HeaderText="Swap Month" ItemStyle-HorizontalAlign="Center" />--%>
                <asp:BoundField DataField="ApplicationDate" DataFormatString="{0:ddMMMyyyy}" HeaderText="申請日期" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="ApplicationDeadline" DataFormatString="{0:ddMMMyyyy}" HeaderText="任務日期" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="ApplicantID" HeaderText="申請人" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="RespondentID" HeaderText="受申請人" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="IDAcTypeApplicant" HeaderText="機隊" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="IDCrewPosApplicant" HeaderText="職級" ItemStyle-HorizontalAlign="Center" />
            </Columns>
        </asp:GridView>
    </div>
</section>