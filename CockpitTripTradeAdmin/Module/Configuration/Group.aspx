<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.Master" AutoEventWireup="true" CodeBehind="Group.aspx.cs" Inherits="CockpitTripTradeAdmin.Module.Configuration.CockpitTripTradeAdmin_Module_Configuration_Group" %>

<%@ Register Src="~/UserControl/ValidationSummary.ascx" TagName="ValidationSummary" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/ChangeHistory.ascx" TagName="ChangeHistory" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:ValidationSummary ID="ValidationSummary1" runat="server" />
    <BusyBoxDotNet:BusyBox ID="BusyBox1" BorderWidth="0" ShowBusyBox="Custom" IncludeScriptsInPage="true" Image="Custom" ImageUrl="../../asset/v3.0.0/images/loading/ci-plane-loading.gif" class="loading-text" DockPosition="Right" Height="120px" Layout="ImageOnly" Text="Loading..." TextBold="true" TextColor="#d81159" TextItalic="true" TextSize="18px" TextFontName="" Position="Center"  Title="" Width="120px" runat="server" />
    <section class="mb-5">
        <ul class="nav nav-tabs" id="myTab" role="tablist">
            <li class="nav-item" role="presentation">
                <a class="nav-link active" id="basic-tab" data-toggle="tab" href="#basic" role="tab" aria-controls="basic" aria-selected="true">系統群組基本設定</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="groupunit-tab" data-toggle="tab" href="#groupunit" role="tab" aria-controls="groupunit" aria-selected="false">群組與組織部門設定</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="groupuser-tab" data-toggle="tab" href="#groupuser" role="tab" aria-controls="groupuser" aria-selected="false">群組與使用者設定</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="groupmoduleformfunction-tab" data-toggle="tab" href="#groupmoduleformfunction" role="tab" aria-controls="groupmoduleformfunction" aria-selected="false">群組與權限設定</a>
            </li>
            <li class="nav-item" role="presentation">
                <a class="nav-link" id="version-tab" data-toggle="tab" href="#version" role="tab" aria-controls="version" aria-selected="false">版本紀錄</a>
            </li>
        </ul>
        <!-- nav-tabs End -->
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane fade py-2 show active" id="basic" role="tabpanel" aria-labelledby="basic-tab">
                <div ID="divBootstrapAlert" Visible="false" runat="server" />
                <section class="vdform-has-mustfill  mb-3">
                    <!-- form col3 - vdform-group x6 -->
                    <div class="row  form-3col6group">
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md">
                                <asp:Label ID="lblIDBllGroup" AssociatedControlID="IDBllGroup" CssClass="col-form-label  label-sm" Text="編號" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="IDBllGroup" CssClass="form-control  form-control-sm" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblGroupCode" runat="server">
                                <asp:Label ID="lblGroupCode" AssociatedControlID="GroupCode" CssClass="col-form-label  label-sm" Text="群組代碼" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12" id="divGroupCode" runat="server">
                                    <asp:TextBox ID="GroupCode" CssClass="form-control  form-control-sm" MaxLength="30" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group">
                            <div class="vdform-prop  vdform-prop-md" id="divLblGroupName" runat="server">
                                <asp:Label ID="lblGroupName" AssociatedControlID="GroupName" CssClass="col-form-label  label-sm" Text="群組名稱" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12" id="divGroupName" runat="server">
                                    <asp:TextBox ID="GroupName" CssClass="form-control  form-control-sm" MaxLength="50" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                        <div class="col-xl-4  col-lg-6  vdform-group vdcolspan-3">
                            <div class="vdform-prop  vdform-prop-md" id="divLblComments" runat="server">
                                <asp:Label ID="lblComments" AssociatedControlID="Comments" CssClass="col-form-label  label-sm" Text="備註" runat="server" />
                            </div>
                            <div class="vdform-value">
                                <div class="col-sm-12" id="divComments" runat="server">
                                    <asp:TextBox ID="Comments" CssClass="form-control  form-control-sm" MaxLength="200" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!-- vdform-group end-->
                    </div>
                    <!-- row end-->
                </section>
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="groupunit" role="tabpanel" aria-labelledby="groupunit-tab">
                <asp:TreeView ID="tvGroupUnits" CssClass="" EnableClientScript="true" ShowCheckBoxes="All" ShowExpandCollapse="true" ShowLines="true" SkipLinkText="" Width="" runat="server" />
                <%--<asp:CheckBoxList ID="GroupUnit" CssClass="" CausesValidation="false" DataTextField="CDesc" DataValueField="UnitCd" RenderWhenDataEmpty="true" RepeatColumns="5" RepeatDirection="Vertical" RepeatLayout="Flow" runat="server" />--%>
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="groupuser" role="tabpanel" aria-labelledby="groupuser-tab">
                <%--<div class="vdform-group">
                    <div class="vdform-prop  vdform-prop-md">
                        <label for="give-input-id" class="col-form-label  label-sm">Property</label>
                    </div>
                    <div class="vdform-value">
                        <div class="col  form-inline">
                            <input type="text" class="form-control  form-control-sm col-7 mr-1" id="give-input-id"> 
                            <input type="button" class="btn btn-primary btn-sm  col-4 " id="give-input-id" value="Verify"> 
                        </div> 
                    </div>    
                </div>--%>
                <asp:Panel ID="pnlGroupUserForm" HorizontalAlign="Center" runat="server">
                    <section class="vdform-has-mustfill">
                        <div class="row  form-2col">
                            <div class="col-lg-6  vdform-group ">
                                <div class="vdform-prop  vdform-prop-md">
                                    <span class="vdform-mustfill">*</span>
                                    <asp:Label ID="lblUserID" AssociatedControlID="UserID" CssClass="col-form-label  label-sm" Text="Employee ID" runat="server" />
                                </div>
                                <div class="vdform-value">
                                    <div class="col form-inline">
                                        <asp:UpdatePanel ID="upUserID" UpdateMode="Conditional" runat="server">
                                            <ContentTemplate>
                                                <asp:TextBox ID="UserID" CssClass="form-control  form-control-sm" MaxLength="6" ValidationGroup="AddGroupUser" placeholder="Please input Employee ID" runat="server" />
                                                <%--<asp:RegularExpressionValidator ID="revUserID" ControlToValidate="UserID" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Employee ID ] must be 6 digits!" SetFocusOnError="true" ValidationExpression="^\d{6}$" ValidationGroup="AddGroupUser" runat="server" />
                                                <asp:RequiredFieldValidator ID="rfvUserID" ControlToValidate="UserID" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Employee ID ] is required!" SetFocusOnError="true" ValidationGroup="AddGroupUser" runat="server" />--%>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftbeUserID" FilterType="Custom" TargetControlID="UserID" ValidChars="0123456789" runat="server" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnAddGroupUser" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                        &nbsp;
                                        <asp:Button ID="btnAddGroupUser" CssClass="btn btn-primary" Text="Add User" ValidationGroup="AddGroupUser" runat="server" OnClick="btnAddGroupUser_Click" />
                                    </div>  
                                </div>    
                            </div>
                            <!-- vdform-group end-->
                        </div>
                    </section>
                </asp:Panel>
                <div class="table-responsive">
                    <asp:UpdatePanel ID="upGroupUserList" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <asp:GridView ID="gvGroupUserList" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" DataKeyNames="IDBllGroup, IDUser" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" ShowHeaderWhenEmpty="true" runat="server" Width="100%" OnPreRender="gvGroupUserList_PreRender" OnRowCancelingEdit="gvGroupUnitList_RowCancelingEdit" OnRowDeleting="gvGroupUnitList_RowDeleting" OnRowEditing="gvGroupUnitList_RowEditing" OnRowUpdating="gvGroupUnitList_RowUpdating" OnRowDataBound="gvGroupUnitList_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbtnEdit" CausesValidation="false" CommandName="Edit" CssClass="btn btn-primary btn-sm" Text="Edit" runat="server" />
                                            <asp:LinkButton ID="lbtnDelete" CausesValidation="false" CommandName="Delete" CssClass="btn btn-outline-danger btn-sm" OnClientClick="" Text="Delete" runat="server" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:LinkButton ID="lbtnUpdate" CausesValidation="false" CommandName="Update" CssClass="btn btn-primary btn-sm" Text="Update" runat="server" />
                                            <asp:LinkButton ID="lbtnCancel" CausesValidation="false" CommandName="Cancel" CssClass="btn btn-outline-primary btn-sm" Text="Cancel" runat="server" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="No." ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="10%">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User" HeaderStyle-Width="70%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIDUser" CssClass="vdform-plaintext" Text="" runat="server" />
                                            <asp:Label ID="lblUserName" CssClass="vdform-plaintext" Text="" runat="server" />
                                            <asp:Label ID="lblUserDisplayDep" CssClass="vdform-plaintext" Text="" runat="server" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtIDUser" CssClass="form-control  form-control-sm" MaxLength="6" Text='<%# Bind("IDUser") %>' placeholder="Please input Employee ID" runat="server" />
                                            <%--<asp:RegularExpressionValidator ID="revIDUser" ControlToValidate="txtIDUser" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Employee ID ] must be 6 digits!" SetFocusOnError="true" ValidationExpression="^\d{6}$" runat="server" />
                                            <asp:RequiredFieldValidator ID="rfvIDUser" ControlToValidate="txtUserID" CssClass="invalid-feedback" Display="Dynamic" ErrorMessage="[ Employee ID ] is required!" SetFocusOnError="true" runat="server" />
                                            <ajaxToolkit:FilteredTextBoxExtender ID="ftbeIDUser" FilterType="Custom" TargetControlID="txtUserID" ValidChars="0123456789" runat="server" />--%>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnAddGroupUser" EventName="Click" />
                            <%--<asp:AsyncPostBackTrigger ControlID="lbtnEdit" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="lbtnDelete" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="lbtnUpdate" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="lbtnCancel" EventName="Click" />--%>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="groupmoduleformfunction" role="tabpanel" aria-labelledby="groupmoduleformfunction-tab">
                <asp:GridView ID="gvGroupModuleFormFunctionList" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover  table-style01" DataKeyNames="IdBllModuleFormFunction" EmptyDataRowStyle-HorizontalAlign="Left" EmptyDataText="No Data!" HeaderStyle-HorizontalAlign="Center" HorizontalAlign="Center" ShowHeaderWhenEmpty="true" runat="server" Width="100%" OnPreRender="gvGroupModuleFormFunctionList_PreRender" OnRowDataBound="gvGroupModuleFormFunctionList_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5%">
                            <ItemTemplate>
                                <%--<div class="form-check  form-check-inline">
                                </div>--%>
                                <asp:CheckBox ID="cbGroupModuleFormFunction" CausesValidation="false" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="No." ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="10%">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ModuleTitle" HeaderText="模組名稱" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="28%" />
                        <asp:BoundField DataField="FormTitle" HeaderText="表單名稱" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="29%" />
                        <asp:BoundField DataField="Description_" HeaderText="功能" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="28%" />
                    </Columns>
                </asp:GridView>
            </div><!-- tab-pane End-->
            <div class="tab-pane fade py-2" id="version" role="tabpanel" aria-labelledby="version-tab">
                <uc1:ChangeHistory ID="ChangeHistory1" runat="server" />
            </div><!-- tab-pane End-->
        </div>
        <!-- tab-content End-->
        <asp:Button ID="btnCheckSave" CausesValidation="false" CssClass="btn btn-primary-sp" OnClientClick="return openBootstrapModal('checkSaveDialogModal');" UseSubmitBehavior="false" Text="Confirm to Save" runat="server" data-target="" data-toggle="modal" />
        <%--<button type="button" class="btn btn-primary-sp" data-target="" data-toggle="modal" onclick="javascript:openBootstrapModal('checkSaveDialogModal');">Confirm to Save</button>--%>
        <input type="button" class="btn btn-outline-primary" value="Back" onclick="javascript:if (confirm('Are you sure to exit this page?')) {window.onbeforeunload=null; window.location='Group_List.aspx'; return true;} else{return false;}" />
        <%--<asp:Button ID="btnTestSendMail" CausesValidation="false" CssClass="btn btn-primary  m-r-xs   m-b-s--screen-xs" Text="Test Send Mail" runat="server" OnClick="btnTestSendMail_Click" />--%>
    </section>

    <div id="checkSaveDialogModal" aria-labelledby="checkSaveDialogLabel" class="modal fade" data-backdrop="static" role="dialog" tabindex="-1" style="display: none;">
		<div class="modal-dialog modal-dialog-centered modal-sm">
			<div class="modal-content">
				<div class="modal-header">
					<h4 id="checkSaveDialogLabel" class="modal-title text-center w-100">Confirm to Save?</h4>
					<button aria-label="Close" class="close" data-dismiss="modal" type="button">
						<span aria-hidden="true">×</span>
					</button>
				</div>
				<div class="modal-body">
					<p>Are you sure to save?</p>
				</div>
				<div class="modal-footer justify-content-center">
                    <asp:Button ID="btnSave" CausesValidation="false" CssClass="btn btn-primary-sp" Text="Save" runat="server" OnClick="btnSave_Click" />
					<button class="btn btn-outline-primary" data-dismiss="modal" type="button">Cancel</button>
				</div>
			</div>
		</div>
    </div>
    
    <div id="checkSaveErrorDialogModal" aria-labelledby="checkSaveErrorDialogLabel" class="modal fade" data-backdrop="static" data-keyboard="false" role="dialog" tabindex="-1" style="display: none;">
		<div class="modal-dialog modal-dialog-centered modal-sm">
			<div class="modal-content">
				<div class="modal-header justify-content-center">
					<h4 id="checkSaveErrorDialogLabel" class="modal-title text-center w-100">Error</h4>
				</div>
				<div class="modal-body text-center">
                    <%--<div style="margin-bottom: 10px;"><img src="../../asset/v3.0.0/images/icons/icon-.png"></div>--%>
					<div id="divSaveError" />
				</div>
				<div class="modal-footer justify-content-center">
					<button class="btn btn-primary" data-dismiss="modal" data-toggle="modal" tabindex="0" type="button">OK</button>
				</div>
			</div>
		</div>
    </div>
    
    <script type="text/javascript">
        function checkUserID() {
            if ($('#<%= this.UserID.ClientID %>').val() == "") {
                $('#<%= this.UserID.ClientID %>').setfocus();
                return false;
            }
            else {
                return true;
            }
        }

        function disableTreeViewCheckBox() {
            var iGroupDepCount = '<%= this.GroupDepDataSet.Tables[0].Rows.Count %>';
            for (var i = 0; i < iGroupDepCount; i++) {
                var sNodeCheckBoxIDTemplate = "ContentPlaceHolder1_tvGroupUnitsn" + i + "CheckBox";
                var oNodeCheckBox = document.getElementById(sNodeCheckBoxIDTemplate);
                if (oNodeCheckBox != undefined) {
                    oNodeCheckBox.disabled = true;
                }
            }
        }
    </script>
</asp:Content>
