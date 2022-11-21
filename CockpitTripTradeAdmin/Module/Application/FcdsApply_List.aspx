<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="FcdsApply_List.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Application.CockpitTripTradeAdmin_Module_Application_FcdsApply_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            <%--
            $("[id*=txtApplicationBeginDate]").datepicker({
                changeMonth: true,
                changeYear: true,
                dateFormat: "dMyy",//yy/mm/dd
                gotoCurrent: true,
                monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../../asset/jquery/datepicker/calendar.png'
            });
            --%>

            $('input[id*="txtApplicationDatePeriod"]').daterangepicker({
                "showDropdowns": true,
                "autoApply": true,
                "locale": {
                    "format": "DDMMMYYYY",
                    "separator": " - ",
                    "applyLabel": "Apply",
                    "cancelLabel": "Cancel",
                    "fromLabel": "From",
                    "toLabel": "To",
                    "customRangeLabel": "Custom",
                    "weekLabel": "W",
                    "daysOfWeek": [
                        "Sun",
                        "Mon",
                        "Tue",
                        "Wed",
                        "Thu",
                        "Fri",
                        "Sat"
                    ],
                    "monthNames": [
                        "Jan",
                        "Feb",
                        "Mar",
                        "Apr",
                        "May",
                        "Jun",
                        "Jul",
                        "Aug",
                        "Sep",
                        "Oct",
                        "Nov",
                        "Dec"
                    ],
                    "firstDay": 1
                },
                "linkedCalendars": false,
                "autoUpdateInput": true,
                "showCustomRangeLabel": false,
                //"startDate": "01/01/2021",
                //"endDate": "01/19/2021",
                //"opens": "left"
            }, function (start, end, label) {
                console.log('New date range selected: ' + start.format('DDMMMYYYY') + ' to ' + end.format('DDMMMYYYY') + ' (predefined range: ' + label + ')');
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <BusyBoxDotNet:BusyBox ID="BusyBox1" BorderWidth="0" ShowBusyBox="Custom" IncludeScriptsInPage="true" Image="Custom" ImageUrl="../../asset/v3.0.0/images/loading/ci-plane-loading.gif" class="loading-text" DockPosition="Right" Height="120px" Layout="ImageOnly" Text="Loading..." TextBold="true" TextColor="#d81159" TextItalic="true" TextSize="18px" TextFontName="" Position="Center"  Title="" Width="120px" runat="server" /> 
    <h2>Search Criteria</h2>
    <section class="vdform-has-mustfill  mb-3">
        <div class="row  form-3col6group">
            <div class="col-xl-4  col-lg-6  vdform-group">
                <div class="vdform-prop  vdform-prop-md">
                    <asp:Label ID="lblApplicationDatePeriod" AssociatedControlID="txtApplicationDatePeriod" CssClass="col-form-label  label-sm" Text="申請區間" runat="server" />
                </div>
                <div class="vdform-value">
                    <div class="col-sm-12">
                        <asp:TextBox ID="txtApplicationDatePeriod" CssClass="form-control  form-control-sm" Enabled="true" runat="server" />
                        <%--<asp:TextBox ID="txtApplicationBeginDate" CssClass="form-control  form-control-sm" Enabled="false" name="from" runat="server" />
                        <asp:TextBox ID="txtApplicationEndDate" CssClass="form-control  form-control-sm" Enabled="false" name="to" runat="server" />--%>
                    </div>
                </div>
            </div>
            <!-- vdform-group end-->
            <div class="col-xl-4  col-lg-6  vdform-group">
                <div class="vdform-prop  vdform-prop-md">
                    <asp:Label ID="lblAcType" AssociatedControlID="ddlAcType" CssClass="col-form-label  label-sm" Text="機隊" runat="server" />
                </div>
                <div class="vdform-value">
                    <div class="col-sm-12">
                        <asp:DropDownList ID="ddlAcType" AppendDataBoundItems="true" CssClass="form-control  form-control-sm" DataTextField="IcaoCode" DataValueField="AcType" runat="server">
                            <asp:ListItem Text="---Please Select---" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <!-- vdform-group end-->
            <div class="col-xl-4  col-lg-6  vdform-group">
                <div class="vdform-prop  vdform-prop-md">
                    <asp:Label ID="lblCrewPos" AssociatedControlID="ddlCrewPos" CssClass="col-form-label  label-sm" Text="職級" runat="server" />
                </div>
                <div class="vdform-value">
                    <div class="col-sm-12">
                        <asp:DropDownList ID="ddlCrewPos" AppendDataBoundItems="true" CssClass="form-control  form-control-sm" DataTextField="Code" DataValueField="ID" runat="server">
                            <asp:ListItem Text="---Please Select---" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <!-- vdform-group end-->
            <div class="col-xl-4  col-lg-6  vdform-group">
                <div class="vdform-prop  vdform-prop-md">
                    <asp:Label ID="lblApplyStatus" AssociatedControlID="ddlStatusCode" CssClass="col-form-label  label-sm" Text="申請單狀態" runat="server" />
                </div>
                <div class="vdform-value">
                    <div class="col-sm-12">
                        <asp:DropDownList ID="ddlStatusCode" AppendDataBoundItems="true" CssClass="form-control  form-control-sm" DataTextField="StatusEName" DataValueField="StatusCode" runat="server">
                            <asp:ListItem Text="---Please Select---" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <!-- vdform-group end-->
            <div class="col-xl-4  col-lg-6  vdform-group">
                <div class="vdform-prop  vdform-prop-md">
                    <asp:Label ID="lblIsApproval" AssociatedControlID="ddlIsApproval" CssClass="col-form-label  label-sm" Text="申請結果" runat="server" />
                </div>
                <div class="vdform-value">
                    <div class="col-sm-12">
                        <asp:DropDownList ID="ddlIsApproval" AppendDataBoundItems="true" CssClass="form-control  form-control-sm" runat="server">
                            <asp:ListItem Text="---Please Select---" Value="" />
                            <asp:ListItem Text="Agree" Value="true" />
                            <asp:ListItem Text="Disagree" Value="false" />
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <!-- vdform-group end-->
            <div class="col-xl-4  col-lg-6  vdform-group">
                <div class="vdform-prop  vdform-prop-md" id="divLblIsOneReqATime" runat="server">
                    <asp:Label ID="lblKeyword" AssociatedControlID="txtKeyword" CssClass="col-form-label  label-sm" Text="關鍵字" runat="server" />
                </div>
                <div class="vdform-value">
                    <div class="col-sm-12">
                        <asp:TextBox ID="txtKeyword" CssClass="form-control  form-control-sm" placeholder="e.g. Respondent Employee ID" runat="server" />
                    </div>
                </div>
            </div>
            <!-- vdform-group end-->
        </div>
        <!-- row end-->
    </section>
    <p class="horizonbuttons-md   mb-5">
        <asp:Button ID="btnSearch" CausesValidation="false" CssClass="btn btn-primary-sp" Text="Search" runat="server" OnClick="btnSearch_Click" />
    </p>
    <section>
        <h2>Search Result</h2>
        <div class="table-responsive">
            <asp:GridView ID="gvList" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" DataKeyNames="IDFcdsApply" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" ShowHeaderWhenEmpty="true" runat="server" Width="100%" OnPreRender="gvList_PreRender" OnRowDataBound="gvList_RowDataBound">
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
                    <asp:TemplateField HeaderText="No." ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Form No." ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:HyperLink ID="Control" NavigateUrl='<%# "FcdsApply.aspx?IDFcdsApply=" + Eval("IDFcdsApply") %>' Text='<%# Eval("IDFcdsApply") %>' runat="server" />
                            <%--<a class="btn btn-primary btn-sm" href="FcdsConfig.aspx?IDAcType=<%# DataBinder.Eval(Container.DataItem, "AcType") %>&IDCrewPos=<%# DataBinder.Eval(Container.DataItem, "CrewPos") %>">Edit</a>--%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="IDAcTypeApplicant" HeaderText="Fleet" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="IDCrewPosApplicant" HeaderText="Pos" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="SwapScheduleMonth" HeaderText="Swap Month" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="ApplicationDate" DataFormatString="{0:ddMMMyyyy}" HeaderText="Application Date" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="ApplicationDeadline" DataFormatString="{0:ddMMMyyyy}" HeaderText="First Duty Date" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="ApplicantID" HeaderText="Applicant" ItemStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="RespondentID" HeaderText="Respondent" ItemStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="StatusEName" HeaderText="Status" ItemStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="Result" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="IsApproval" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:BoundField DataField="IsApproval" HeaderText="Approval Status" ItemStyle-HorizontalAlign="Center" />--%>
                    <%--<asp:BoundField DataField="CreateBy" HeaderText="建立者" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="CreateStamp" HeaderText="建立時間" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="UpdateBy" HeaderText="更新者" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="UpdateStamp" HeaderText="更新時間" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />--%>
                </Columns>
            </asp:GridView>
        </div>
    </section>
</asp:Content>
