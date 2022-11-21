using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

using Library.Component.BLL;

namespace Library.Component.Utility
{
    /// <summary>
    /// ASP.NET TreeView 控制項輔助密封類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class TreeViewUtility
    {
        private TreeViewUtility()
        {

        }

        /// <summary>
        /// 建立子節點
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="parentNode"></param>
        public static void PopulateTree(string nodeValueName, string nodeTextName, DataRow dataRow, TreeNode parentNode, GroupUnitCollection groupUnits)
        {
            foreach (DataRow row in dataRow.GetChildRows("OrgRelation"))
            {
                TreeNode treeNode = CreateNode(row[nodeValueName].ToString(), row[nodeTextName].ToString(), true, groupUnits != null && groupUnits.Exists(o => o.UnitCd == row[nodeValueName].ToString()));
                parentNode.ChildNodes.Add(treeNode);
                PopulateTree(nodeValueName, nodeTextName, row, treeNode, groupUnits);  //Recursively build the tree
            }
        }

        /// <summary>
        /// 節點屬性設定
        /// </summary>
        /// <param name="nodeValue">節點的值</param>
        /// <param name="nodeText">節點顯示的文字</param>
        /// <param name="expanded">是否展開節點</param>
        /// <returns></returns>
        public static TreeNode CreateNode(string nodeValue, string nodeText, bool expanded, bool isChecked)
        {
            TreeNode node = new TreeNode();
            node.Expanded = expanded;
            node.Checked = isChecked;
            node.SelectAction = TreeNodeSelectAction.None;
            node.Text = nodeText;
            node.Value = nodeValue;

            return node;
        }
    }
}
