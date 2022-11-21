<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="FcdsConfig_Log.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Configuration.CockpitTripTradeAdmin_Module_Configuration_FcdsConfig_Log" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <BusyBoxDotNet:BusyBox ID="BusyBox1" BorderWidth="0" ShowBusyBox="Custom" IncludeScriptsInPage="true" Image="Custom" ImageUrl="../../asset/v3.0.0/images/loading/ci-plane-loading.gif" class="loading-text" DockPosition="Right" Height="120px" Layout="ImageOnly" Text="Loading..." TextBold="true" TextColor="#d81159" TextItalic="true" TextSize="18px" TextFontName="" Position="Center"  Title="" Width="120px" runat="server" />
    <div class="row  mb-3">
        <div class="col-lg-12  col-xl-10">
            <div class="row  form-2col">
                <div class="col-lg-6  vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                        <asp:Label ID="lblBaseVersion" AssociatedControlID="BaseVersion" CssClass="col-form-label  label-sm" Text="Base Version" runat="server" />
                    </div>
                    <div class="vdform-value">
                        <div class="col-sm-12">
                            <asp:DropDownList ID="BaseVersion" AppendDataBoundItems="true" CssClass="form-control  form-control-sm" DataTextField="VersionValue" DataValueField="BranchID" Width="35%" runat="server">
                                <asp:ListItem Text="---請選擇---" Value="" />
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvBaseVersion" ControlToValidate="BaseVersion" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ BaseVersion ] is required!" runat="server" />
                        </div>  
                    </div>                                  
                </div><!-- vdform-group end-->
                <div class="col-lg-6  vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                        <asp:Label ID="lblTargetVersion" AssociatedControlID="TargetVersion" CssClass="col-form-label  label-sm" Text="Target Version" runat="server" />
                    </div>
                    <div class="vdform-value">
                        <div class="col-sm-12">
                            <asp:DropDownList ID="TargetVersion" AppendDataBoundItems="true" CssClass="form-control  form-control-sm" DataTextField="VersionValue" DataValueField="BranchID" Width="35%" runat="server">
                                <asp:ListItem Text="---請選擇---" Value="" />
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvTargetVersion" ControlToValidate="TargetVersion" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ TargetVersion ] is required!" runat="server" />
                        </div>  
                    </div>                                  
                </div><!-- vdform-group end-->
            </div><!-- row end-->
            <div class="row  form-2col">
                <div class="col-lg-6  vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                        <asp:Label ID="lblBaseVersionUpdateBy" CssClass="col-form-label  label-sm" Text="Update By" runat="server" />
                    </div>
                    <div class="vdform-value">
                        <div class="col-sm-12">
                            <asp:Label ID="BaseVersionUpdateBy" CssClass="form-control  form-control-sm" runat="server" />
                        </div>  
                    </div>                                  
                </div><!-- vdform-group end-->
                <div class="col-lg-6  vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                        <asp:Label ID="lblTargetVersionUpdateBy" CssClass="col-form-label  label-sm" Text="Update By" runat="server" />
                    </div>
                    <div class="vdform-value">
                        <div class="col-sm-12">
                            <asp:Label ID="TargetVersionUpdateBy" CssClass="form-control  form-control-sm" runat="server" />
                        </div>  
                    </div>                                  
                </div><!-- vdform-group end-->
            </div><!-- row end-->
            <div class="row  form-2col">
                <div class="col-lg-6  vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                        <asp:Label ID="lblBaseVersionUpdateStamp" CssClass="col-form-label  label-sm" Text="Update Date" runat="server" />
                    </div>
                    <div class="vdform-value">
                        <div class="col-sm-12">
                            <asp:Label ID="BaseVersionUpdateStamp" CssClass="form-control  form-control-sm" runat="server" />
                        </div>  
                    </div>                                  
                </div><!-- vdform-group end-->
                <div class="col-lg-6  vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                        <asp:Label ID="lblTargetVersionUpdateStamp" CssClass="col-form-label  label-sm" Text="Update Date" runat="server" />
                    </div>
                    <div class="vdform-value">
                        <div class="col-sm-12">
                            <asp:Label ID="TargetVersionUpdateStamp" CssClass="form-control  form-control-sm" runat="server" />
                        </div>  
                    </div>                                  
                </div><!-- vdform-group end-->
            </div><!-- row end-->
            <%--
            <div class="row  form-2col">
                <div class="col-lg-6  vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                    </div>
                    <div class="vdform-value">
                        <div class="col-sm-12">
                        </div>  
                    </div>                                  
                </div><!-- vdform-group end-->
                <div class="col-lg-6  vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                    </div>
                    <div class="vdform-value">
                        <div class="col-sm-12">
                        </div>  
                    </div>                                  
                </div><!-- vdform-group end-->
            </div><!-- row end-->
            --%>
            <p class="mb-5">
                <asp:Button ID="btnCompare" CssClass="btn btn-primary-sp" Text="Compare" runat="server" OnClick="btnCompare_Click" />
            </p>
        </div><!-- col end -->
    </div><!-- row end -->
    <div class="table-responsive">
        <asp:GridView ID="gvFormLogList" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" DataKeyNames="IDComponent" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" ShowHeaderWhenEmpty="true" runat="server" OnPreRender="gvFormLogList_PreRender" OnRowDataBound="gvFormLogList_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="No." HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ComponentDisplayName" HeaderStyle-Width="30%" HeaderText="Field Name" />
                <asp:BoundField DataField="BaseVersion" HeaderStyle-Width="30%" HeaderText="Base Version" />
                <asp:BoundField DataField="TargetVersion" HeaderStyle-Width="30%" HeaderText="Target Version" />
            </Columns>
        </asp:GridView>
    </div>
    <div class="table-responsive">
        <asp:GridView ID="gvTeamLogList" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" DataKeyNames="IDRole" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" ShowHeaderWhenEmpty="true" runat="server" OnPreRender="gvTeamLogList_PreRender" OnRowDataBound="gvTeamLogList_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="No." HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="RoleName" HeaderStyle-Width="30%" HeaderText="Role Name" />
                <asp:BoundField DataField="BaseVersion" HeaderStyle-Width="30%" HeaderText="Base Version" />
                <asp:BoundField DataField="TargetVersion" HeaderStyle-Width="30%" HeaderText="Target Version" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
