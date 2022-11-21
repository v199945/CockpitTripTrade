using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Library.Component.BLL;
using Library.Component.Enums;
using Library.Component.Utility;
using Library.Module.FCDS;
using Library.Module.FCDS.Configuration;
using Library.Module.HRDB;

namespace CockpitTripTradeAdmin.Module.Configuration
{
    public partial class CockpitTripTradeAdmin_Module_Configuration_Group : ConfigurationPage
    {
        private ModuleForm moduleForm;
        private Group obj;
        private string proID;

        /// <summary>
        /// 序列化之群組與使用者集合物件。
        /// </summary>
        private GroupUserCollection GroupUsers
        {
            get
            {
                return SerializeUtility.Deserialize<GroupUserCollection>(ViewState["GroupUsers"].ToString());
            }
            
            set
            {
                ViewState["GroupUsers"] = SerializeUtility.Serialize(value);
            }
        }

        public readonly DataSet GroupDepDataSet = FcdsHelper.FetchGroupDep();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.BaseFormName = @"BllGroup";
            moduleForm = new ModuleForm(this.BaseModuleName, this.BaseFormName);
            this.BaseFormTitle = moduleForm.FormTitle;

            string idBllGroup = Page.Request.QueryString["IDBllGroup"];
            obj = new Group(idBllGroup);

            if (!string.IsNullOrEmpty(idBllGroup) && string.IsNullOrEmpty(obj.IDBllGroup))
            {
                string script = @"<script type=""text/javascript"">alert('Invalid Link!'); window.location='../ErrorHandler/NotFoundPage.aspx';</script>";
                Response.Write(script);
                Response.End();
            }

            if (LoginSession.HasEditAuthority(this.Page, moduleForm.IDBllModuleForm))
            {
                if (!string.IsNullOrEmpty(idBllGroup))
                {
                    this.BasePageModeEnum = PageMode.PageModeEnum.Edit;
                }
                else
                {
                    this.BasePageModeEnum = PageMode.PageModeEnum.Create;
                }
            }
            else
            {
                if (LoginSession.HasViewAuthority(this.Page, moduleForm.IDBllModuleForm))
                {
                    this.BasePageModeEnum = PageMode.PageModeEnum.View;
                }
                else
                {
                    // Redirect to Unauthorized Page.
                    this.Page.Response.Redirect(@"~/Module/ErrorHandler/UnauthorizedPage.aspx");
                }
            }

            switch (this.BasePageModeEnum)
            {
                case PageMode.PageModeEnum.Create:
                    proID = moduleForm.InitialProID;
                    break;

                case PageMode.PageModeEnum.Edit:
                    proID = moduleForm.InitialProID;
                    break;

                default:
                    proID = moduleForm.InitialProID;
                    break;
            }

            if (!Page.IsPostBack)
            {
                this.GroupUsers = obj.GroupUsers == null ? new GroupUserCollection() : obj.GroupUsers;
                InitForm();
            }

            BindUserControls();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ModuleFormRight.SetModuleFormRight(this.Page, this.BaseModuleName, this.BaseFormName, this.proID, this.BasePageModeEnum);
        }

        private void InitForm()
        {
            BindForm();
            SetForm();
        }

        private void BindForm()
        {
            this.IDBllGroup.Text = obj.IDBllGroup;
            this.GroupCode.Text = obj.GroupCode;
            this.GroupName.Text = obj.GroupName;
            this.Comments.Text = obj.Comments;

            BindGroupUnitTreeView();
            BindGroupUserList();
            BindGroupModuleFormFunctionList();
        }

        /// <summary>
        /// 繫結群組與組織部門 TreeView 控制項。
        /// </summary>
        private void BindGroupUnitTreeView()
        {
            this.tvGroupUnits.Nodes.Clear(); // 待修正：PostBack 後造成無法正常縮合或展開節點
            DataSet ds = FcdsHelper.FetchGroupDep();
            foreach (DataRow row in this.GroupDepDataSet.Tables[0].Rows)
            {
                if (row["UperUt"].ToString() == @"001")
                {
                    // 建立根節點
                    TreeNode treeNode = TreeViewUtility.CreateNode(row["UnitCd"].ToString(), row["CDesc"].ToString(), true, obj.GroupUnits != null && obj.GroupUnits.Exists(o => o.UnitCd == row["UnitCd"].ToString()));

                    // 將每個根節點加入 TreeView 控制項
                    this.tvGroupUnits.Nodes.Add(treeNode);

                    // 將每個根節點填入子節點
                    TreeViewUtility.PopulateTree(@"UnitCd", @"CDesc", row, treeNode, obj.GroupUnits);
                }
            }
        }

        /// <summary>
        /// 繫結群組與使用者 GridView 控制項。
        /// </summary>
        private void BindGroupUserList()
        {
            this.gvGroupUserList.DataSource = this.GroupUsers;
            this.gvGroupUserList.DataBind();
            this.gvGroupUserList.Columns[0].Visible = LoginSession.HasEditAuthority(this.Page, moduleForm.IDBllModuleForm);
        }

        private void BindGroupModuleFormFunctionList()
        {
            this.gvGroupModuleFormFunctionList.DataSource = ModuleFormFunction.FetchAllModuleFormFunction(ReturnObjectTypeEnum.Collection);
            this.gvGroupModuleFormFunctionList.DataBind();
            this.gvGroupModuleFormFunctionList.Columns[0].Visible = LoginSession.HasEditAuthority(this.Page, moduleForm.IDBllModuleForm);
        }

        protected void gvGroupUnitList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            GridViewRow gvr = e.Row;
            GroupUser row = e.Row.DataItem as GroupUser;

            LinkButton lbtnEdit = gvr.FindControl("lbtnEdit") as LinkButton;
            if (lbtnEdit != null)
            {
                LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, lbtnEdit);
            }

            LinkButton lbtnDelete = gvr.FindControl("lbtnDelete") as LinkButton;
            if (lbtnDelete != null)
            {
                lbtnDelete.Attributes.Add(@"OnClick", this.BusyBox1.ShowFunctionCall);
                LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, lbtnDelete);
            }

            LinkButton lbtnUpdate = gvr.FindControl("lbtnUpdate") as LinkButton;
            if (lbtnUpdate != null)
            {
                lbtnUpdate.Attributes.Add(@"OnClick", this.BusyBox1.ShowFunctionCall);
            }

            Label lblIDUser = gvr.FindControl("lblIDUser") as Label;
            Label lblUserName = gvr.FindControl("lblUserName") as Label;
            Label lblUserDisplayDep = gvr.FindControl("lblUserDisplayDep") as Label;
            if (lblIDUser != null && lblUserName != null && lblUserDisplayDep != null)
            {
                HrVEgEmploy employ = new HrVEgEmploy(row.IDUser);
                lblIDUser.Text = employ.EmployID;
                lblUserName.Text = employ.CName;
                lblUserDisplayDep.Text = employ.DisplayDepName;
            }
        }

        protected void gvGroupModuleFormFunctionList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            if (obj != null && obj.GroupModuleFormFunctions != null && obj.GroupModuleFormFunctions.Count > 0)
            {
                GridViewRow gvr = e.Row;
                ModuleFormFunction row = e.Row.DataItem as ModuleFormFunction;

                CheckBox cbGroupModuleFormFunction = e.Row.FindControl(@"cbGroupModuleFormFunction") as CheckBox;
                if (cbGroupModuleFormFunction != null)
                {
                    //cbGroupModuleFormFunction.CssClass = @"form-check-input";
                    cbGroupModuleFormFunction.Checked = obj.GroupModuleFormFunctions.Where(o => o.IDBllModuleFormFunction == row.IDBllModuleFormFunction).Count() > 0;
                }
            }

        }

        protected void gvGroupUserList_PreRender(object sender, EventArgs e)
        {
            this.gvGroupUserList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void gvGroupModuleFormFunctionList_PreRender(object sender, EventArgs e)
        {
            this.gvGroupModuleFormFunctionList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        private void SetForm()
        {
            // RequiredFieldValidator_CheckValidControl('" + this.rfvUserID.ClientID + @"', '" + this.UserID.ClientID + @"');
            this.UserID.Attributes.Add("OnInput", @"javascript:ValidatorOnChange(event);");

            this.btnSave.Attributes.Add(@"OnClick", this.BusyBox1.ShowFunctionCall);
            //this.btnSave.Attributes.Add("OnClick", "if (Page_ClientValidate()) {if (confirm('Are you sure to save?')) {window.onbeforeunload=null; " + this.BusyBox1.ShowFunctionCall + @" return true;} else {return false;}} else {RequiredFieldValidator_CheckAllValidControl(Page_Validators);}");
            this.btnAddGroupUser.Attributes.Add(@"OnClick", @"if (checkUserID()) {" + this.BusyBox1.ShowFunctionCall + @"} else {return false;}");

            LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, this.btnCheckSave);
            LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, this.btnSave);
            LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, this.pnlGroupUserForm);
            LoginSession.VerifyAuthorization(this.Page, moduleForm.IDBllModuleForm, this.btnAddGroupUser);

            if (!LoginSession.HasEditAuthority(this.Page, moduleForm.IDBllModuleForm))
            {
                this.RunJavascript(@"disableTreeViewCheckBox();");
            }
        }

        private void BindUserControls()
        {
            this.ChangeHistory1.BasePageModeEnum = this.BasePageModeEnum;
            this.ChangeHistory1.ModuleName = this.BaseModuleName;
            this.ChangeHistory1.FormName = this.BaseFormName;
            this.ChangeHistory1.IDObject = obj.IDBllGroup;
            this.ChangeHistory1.Object = obj;
        }

        protected void btnAddGroupUser_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.UserID.Text))
            {
                HrVEgEmploy employee = new HrVEgEmploy(this.UserID.Text.Trim());
                if (string.IsNullOrEmpty(employee.EmployID))
                {
                    this.RunJavascript(@"$('#divSaveError').text('User " + this.UserID.Text + @" does not exist!'); $('#checkSaveErrorDialogModal').modal('show');");
                }
                else
                {
                    UpdateGroupUserGridView();

                    if (this.GroupUsers.Where(o => o.IDUser == employee.EmployID).Count() > 0)
                    {
                        this.RunJavascript(@"$('#divSaveError').text('User " + this.UserID.Text + @" already exists!'); $('#checkSaveErrorDialogModal').modal('show');");
                    }
                    else
                    {
                        GroupUserCollection col = this.GroupUsers;
                        GroupUser gu = new GroupUser();
                        gu.IDBllGroup = obj.IDBllGroup;
                        gu.IDUser = this.UserID.Text.Trim();
                        col.Add(gu);
                        this.GroupUsers = col;

                        BindGroupUserList();
                        this.UserID.Text = string.Empty;
                    }
                }
            }

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        protected void gvGroupUnitList_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            this.gvGroupUserList.EditIndex = -1;

            BindGroupUserList();
        }

        protected void gvGroupUnitList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            UpdateGroupUserGridView();

            GroupUserCollection col = this.GroupUsers;
            col.RemoveAt(e.RowIndex);
            this.GroupUsers = col;

            e.Cancel = true;

            BindGroupUserList();

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        protected void gvGroupUnitList_RowEditing(object sender, GridViewEditEventArgs e)
        {
            this.gvGroupUserList.EditIndex = e.NewEditIndex;

            BindGroupUserList();

            TextBox txtIDUser = this.gvGroupUserList.Rows[e.NewEditIndex].FindControl("txtIDUser") as TextBox;
            if (txtIDUser != null)
            {
                txtIDUser.Focus();
            }
        }

        protected void gvGroupUnitList_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            TextBox txtIDUser = this.gvGroupUserList.Rows[e.RowIndex].FindControl("txtIDUser") as TextBox;
            if (txtIDUser != null)
            {
                HrVEgEmploy employee = new HrVEgEmploy(txtIDUser.Text);
                if (string.IsNullOrEmpty(employee.EmployID))
                {
                    this.RunJavascript(@"$('#divSaveError').text('User " + txtIDUser.Text + @" is not exist!'); $('#checkSaveErrorDialogModal').modal('show');");
                }
                else
                {
                    GroupUserCollection col = this.GroupUsers;
                    col[e.RowIndex].IDUser = txtIDUser.Text;
                    this.GroupUsers = col;
                }
            }

            // 重設要編輯的資料列索引
            this.gvGroupUserList.EditIndex = -1;

            BindGroupUserList();

            BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsPassValidation() && SaveObject(true))
            {
                //BindForm();
                this.ChangeHistory1.IDObject = obj.IDBllGroup;
                this.ChangeHistory1.BindList();
                BusyBoxUtility.HideBusyBox(this.Page, e, this.BusyBox1.HideFunctionCall);

                this.BootstrapAlerts(this.divBootstrapAlert, @"<strong>Save Complete!</strong> 您所做的修改已於 " + obj.UpdateStamp.Value.ToString("yyyy/MM/dd HH:mm:ss") + @" 儲存成功！", true, BootstrapAlertsTypeEnum.Success);
                //this.Redirect(@"Group_List.aspx");
            }
            else
            {
                this.BootstrapAlerts(this.divBootstrapAlert, @"<strong>Save Failed!</strong> 您所做的修改儲存失敗，請再試一次！", true, BootstrapAlertsTypeEnum.Danger);
                return;
            }
        }

        private bool IsPassValidation()
        {
            string alert = null;

            if (this.BasePageModeEnum != PageMode.PageModeEnum.Create && string.IsNullOrEmpty(this.IDBllGroup.Text))
            {
                alert += @"[" + this.lblIDBllGroup.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.GroupCode.Text))
            {
                alert += @"[" + this.lblGroupCode.Text + @"] is required!\n";
            }

            if (string.IsNullOrEmpty(this.GroupName.Text))
            {
                alert += @"[" + this.lblGroupName.Text + @"] is required!\n";
            }

            for (int i = 0; i < this.gvGroupUserList.Rows.Count; i++)
            {
                string iduser = (this.gvGroupUserList.Rows[i].FindControl("lblIDUser") as Label).Text;
                for (int j = i + 1; j < this.gvGroupUserList.Rows.Count; j++)
                {
                    if (iduser == (this.gvGroupUserList.Rows[j].FindControl("lblIDUser") as Label).Text)
                    {
                        alert += @"There are duplicate rows in Group and Users!\n";
                    }
                }
            }

            if (string.IsNullOrEmpty(alert))
            {
                return true;
            }
            else
            {
                this.Alert(alert);
                return false;
            }
        }

        private bool SaveObject(bool isSaveLog)
        {
            SetObjValue();
            if (obj.Save(FcdsHelper.ROOT_NAME_SPACE, this.BasePageModeEnum, isSaveLog))
            {
                UpdateGroupUnitTreeView();
                UpdateGroupUserGridView();
                UpdateGroupModuleFormFunctionGridView();

                if (GroupUnit.SaveGroupUnitCollection(obj.IDBllGroup, obj.GroupUnits, isSaveLog)
                        && GroupUser.SaveGroupUserCollection(obj.IDBllGroup, this.GroupUsers, isSaveLog)
                            && GroupModuleFormFunction.SaveGroupModuleFormFunctionCollection(obj.IDBllGroup, obj.GroupModuleFormFunctions, isSaveLog))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 設定表單物件屬性值。
        /// </summary>
        private void SetObjValue()
        {
            obj.GroupCode = this.GroupCode.Text;
            obj.GroupName = this.GroupName.Text;
            obj.Comments = this.Comments.Text.Trim();

            obj.CreateBy = this.LoginSession.UserID;
            obj.CreateStamp = DateTime.Now;
            obj.UpdateBy = this.LoginSession.UserID;
            obj.UpdateStamp = DateTime.Now;
        }

        /// <summary>
        /// 依據部門 TreeView 控制項中勾選的節點，更新使用者群組與組織部門集合物件(GroupUnits)。
        /// </summary>
        /// <returns></returns>
        private void UpdateGroupUnitTreeView()
        {
            obj.GroupUnits.Clear();
            foreach (TreeNode node in this.tvGroupUnits.CheckedNodes)
            {
                obj.GroupUnits.Add(SetGroupUnitTreeNode(node.Value));
            }

            //foreach (TreeNode treeNodes in this.tvGroupUnits.Nodes)
            //{
            //    IterateTreeNodes(treeNodes);
            //}
        }

        /// <summary>
        /// 遍歷 tvGroupUnits TreeView 控制項中各節點。
        /// </summary>
        /// <param name="nodes">tvGroupUnits TreeView 控制項中的節點</param>
        private void IterateTreeNodes(TreeNode nodes)
        {
            if (nodes.Checked)
            {
                obj.GroupUnits.Add(SetGroupUnitTreeNode(nodes.Value));
            }

            foreach (TreeNode tn in nodes.ChildNodes)
            {
                IterateTreeNodes(tn);
            }
        }

        /// <summary>
        /// 依據群組與組織部門 TreeView 控制項中已勾選核取方塊的節點，設定群組與組織部門物件值。
        /// </summary>
        /// <param name="nodeValue">tvGroupUnits TreeView 控制項中節點的值</param>
        /// <returns></returns>
        private GroupUnit SetGroupUnitTreeNode(string nodeValue)
        {
            GroupUnit gu = new GroupUnit();
            gu.BranchID = obj.BranchID;
            gu.Version = obj.Version;
            gu.IDBllGroup = obj.IDBllGroup;
            gu.UnitCd = nodeValue;
            gu.CreateBy = obj.CreateBy;
            gu.CreateStamp = obj.CreateStamp;
            gu.UpdateBy = obj.UpdateBy;
            gu.UpdateStamp = obj.UpdateStamp;

            return gu;
        }

        /// <summary>
        /// 依據群組與使用者設定 gvGroupUserList GridView 控制項，更新群組與使用者集合物件(GroupUsers)。
        /// </summary>
        private void UpdateGroupUserGridView()
        {
            GroupUserCollection col = new GroupUserCollection();
            foreach (GridViewRow gvr in this.gvGroupUserList.Rows)
            {
                col.Add(SetGroupUserGridViewRow(gvr));
            }

            this.GroupUsers = col;
        }

        /// <summary>
        /// 依據群組與使用者 GridViewRow 設定群組與使用者物件值。
        /// </summary>
        /// <param name="gvr">群組與使用者 GridView 控制項中的個別資料列</param>
        /// <returns></returns>
        private GroupUser SetGroupUserGridViewRow(GridViewRow gvr)
        {
            GroupUser gu = null;
            Label lblIDUser = gvr.FindControl("lblIDUser") as Label;
            if (lblIDUser != null)
            {
                gu = new GroupUser();
                gu.BranchID = obj.BranchID;
                gu.Version = obj.Version;
                gu.IDBllGroup = obj.IDBllGroup;
                gu.IDUser = lblIDUser.Text;
                gu.CreateBy = obj.UpdateBy;
                gu.CreateStamp = obj.UpdateStamp;
                gu.UpdateBy = obj.UpdateBy;
                gu.UpdateStamp = obj.UpdateStamp;
            }

            return gu;
        }

        /// <summary>
        /// 依據群組與權限設定 gvGroupModuleFormFunctionList GridView 控制項，更新群組與權限集合物件(GroupModuleFunctions)。
        /// </summary>
        private void UpdateGroupModuleFormFunctionGridView()
        {
            GroupModuleFormFunctionCollection col = new GroupModuleFormFunctionCollection();
            foreach (GridViewRow gvr in this.gvGroupModuleFormFunctionList.Rows)
            {
                CheckBox cbGroupModuleFormFunction = gvr.FindControl("cbGroupModuleFormFunction") as CheckBox;
                if (cbGroupModuleFormFunction != null && cbGroupModuleFormFunction.Checked)
                {
                    col.Add(SetGroupModuleFormFunction(gvr));
                }
            }

            obj.GroupModuleFormFunctions = col;
        }

        private GroupModuleFormFunction SetGroupModuleFormFunction(GridViewRow gvr)
        {
            GroupModuleFormFunction gmff = null;
            if (!string.IsNullOrEmpty(this.gvGroupModuleFormFunctionList.DataKeys[gvr.RowIndex].Value.ToString()))
            {
                ModuleFormFunction mff = new ModuleFormFunction(this.gvGroupModuleFormFunctionList.DataKeys[gvr.RowIndex].Value.ToString());

                gmff = new GroupModuleFormFunction();
                gmff.BranchID = obj.BranchID;
                gmff.Version = obj.Version;
                gmff.IDBllGroup = obj.IDBllGroup;
                gmff.IDBllModuleFormFunction = mff.IDBllModuleFormFunction;
                gmff.CreateBy = obj.UpdateBy;
                gmff.CreateStamp = obj.UpdateStamp;
                gmff.UpdateBy = obj.UpdateBy;
                gmff.UpdateStamp = obj.UpdateStamp;

            }

            return gmff;
        }
    }
}