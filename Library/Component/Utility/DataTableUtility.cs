using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Library.Component.Utility
{
    /// <summary>
    /// DataTable 輔助密封類別。此類別無法被繼承。
    /// </summary>
    public sealed class DataTableUtility
    {
        /// <summary>
        /// Join 列舉型態。
        /// </summary>
        public enum JoinTypeEnum
        {
            Inner,
            Outer,
            FullOuter
        }

        private DataTableUtility()
        {

        }

        /// <summary>
        /// 建立 DataTable 欄位。
        /// </summary>
        /// <param name="src">來源 DataTable</param>
        /// <param name="columnName">欄位名稱</param>
        /// <param name="dataType">欄位資料型態</param>
        /// <param name="isAllowNull">欄位是否允許 NULL 值</param>
        /// <param name="defaultValue">欄位預設值</param>
        public static void CreateDataTableColumn(DataTable src, string columnName, string dataType, bool isAllowNull, string defaultValue, string expression)
        {
            if (!src.Columns.Contains(columnName))
            {
                DataColumn dc = new DataColumn(columnName);
                dc.DataType = System.Type.GetType(dataType);
                dc.AllowDBNull = isAllowNull;

                if (!string.IsNullOrEmpty(defaultValue))
                {
                    dc.DefaultValue = defaultValue;
                }

                if (!string.IsNullOrEmpty(expression))
                {
                    dc.Expression = expression;
                }

                src.Columns.Add(dc);
            }
        }

        /// <summary>
        /// TODO: 轉置(行列互換) DataTable。
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable Transpose(DataTable dt)
        {
            DataTable dst = new DataTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
            }

            return dst;
        }

        /// <summary>
        /// Join DataTable
        /// </summary>
        /// <param name="firstTable">第一個 DataTable</param>
        /// <param name="secondTable">第二個 DataTable</param>
        /// <param name="firstJoinColumns">第一個 DataTable Join 的 DataColumn 字串，以 ',' 分隔</param>
        /// <param name="secondJoinColumns">第二個 DataTable Join 的 DataColumn 字串，以 ',' 分隔</param>
        /// <param name="jt">Join 的類型</param>
        /// <returns></returns>
        public static DataTable JoinTable(DataTable firstTable, DataTable secondTable, string firstJoinColumn, string secondJoinColumn, JoinTypeEnum jt)
        {
            string[] aryFirstTableCol = firstJoinColumn.Split(',');
            string[] arySecondTableCol = secondJoinColumn.Split(',');
            if (aryFirstTableCol.Length != arySecondTableCol.Length)
                throw new ArgumentException("DataTable Join 的欄位數量不一致！");

            List<DataColumn> FirstColumns = new List<DataColumn>();
            List<DataColumn> SecondColumns = new List<DataColumn>();

            foreach (string s in aryFirstTableCol)
                FirstColumns.Add(new DataColumn(s));

            foreach (string s in arySecondTableCol)
                SecondColumns.Add(new DataColumn(s));

            return JoinTable(firstTable, secondTable, FirstColumns.ToArray(), SecondColumns.ToArray(), jt);
        }

        /// <summary>
        /// Join DataTable
        /// </summary>
        /// <param name="firstTable">第一個 DataTable</param>
        /// <param name="secondTable">第二個 DataTable</param>
        /// <param name="firstJoinColumns">第一個 DataTable Join 的 DataColumn Array</param>
        /// <param name="secondJoinColumns">第二個 DataTable Join 的 DataColumn Array</param>
        /// <param name="jt">Join 的類型</param>
        /// <returns></returns>
        public static DataTable JoinTable(DataTable firstTable, DataTable secondTable, DataColumn[] firstJoinColumns, DataColumn[] secondJoinColumns, JoinTypeEnum jt)
        {
            DataTable dtJoined = new DataTable("Join"); // 完成Join之後的DataTable物件
            List<DataColumn> DuplicationColumns = new List<DataColumn>(); // 存放欄位名稱重複

            DataSet ds = new DataSet();
            firstTable.TableName = "FirstTable";
            secondTable.TableName = "SecondTable";
            ds.Tables.AddRange(new DataTable[] { firstTable.Copy(), secondTable.Copy() }); // 將兩個DataTable的複本加入DataSet

            // FirstDataTable Join 的欄位
            DataColumn[] ParentColumns = new DataColumn[firstJoinColumns.Length];
            for (int i = 0; i < ParentColumns.Length; i++)
            {
                if (ds.Tables[0].Columns.Contains(firstJoinColumns[i].ColumnName))
                    ParentColumns[i] = ds.Tables[0].Columns[firstJoinColumns[i].ColumnName];
                else
                    throw new ArgumentException("FirstDataTable Join Column can not be null!");
            }

            // SecondDataTable Join 的欄位
            DataColumn[] ChildColumns = new DataColumn[secondJoinColumns.Length];
            for (int i = 0; i < ChildColumns.Length; i++)
            {
                if (ds.Tables[1].Columns.Contains(secondJoinColumns[i].ColumnName))
                    ChildColumns[i] = ds.Tables[1].Columns[secondJoinColumns[i].ColumnName];
                else
                    throw new ArgumentException("SecondDataTable Join Column can not be null!");
            }

            // 建立 FirstDataTable 與 SecondDataTable 的關聯性物件：關聯名稱，父 DataColumn 陣列，子 DataColumn 陣列，是否建立約束條件
            DataRelation dr = new DataRelation(string.Empty, ParentColumns, ChildColumns, false);
            ds.Relations.Add(dr);

            // 建立完成Join後的DataTable欄位
            for (int i = 0; i < firstTable.Columns.Count; i++)
            {
                dtJoined.Columns.Add(firstTable.Columns[i].ColumnName, firstTable.Columns[i].DataType);
            }

            for (int i = 0; i < secondTable.Columns.Count; i++)
            {
                // 檢查欄位名稱是否重複，
                if (dtJoined.Columns.Contains(secondTable.Columns[i].ColumnName))
                {
                    // 重複則取別名(_Second)，加入完成 Join 後的 DataTable 與 DuplicationColumns 強型別物件清單
                    DuplicationColumns.Add(dtJoined.Columns.Add(secondTable.Columns[i].ColumnName + "_Second", secondTable.Columns[i].DataType));
                }
                else
                {
                    // 不重複直接加入完成Join後的DataTable
                    dtJoined.Columns.Add(secondTable.Columns[i].ColumnName, secondTable.Columns[i].DataType);
                }
            }

            // 判斷Join的類別
            switch (jt)
            {
                case JoinTypeEnum.Inner:
                    dtJoined.BeginLoadData(); // 載入資料時關閉告知、索引維護和條件約束。Turns off notifications, index maintenance, and constraints while loading data.

                    // 取 FirstTable 的資料列
                    foreach (DataRow FirstTableDataRow in ds.Tables[0].Rows)
                    {
                        // 取得 Join 資料列
                        DataRow[] childRows = FirstTableDataRow.GetChildRows(dr); // 依據關聯性(DataRelation)取得這個父資料列的子資料列

                        // 判斷有無子資料列，有才需要將 FirstTable 與 SecondTable 同樣 Key 值的資料列複製至 aryJoinObj，
                        // 再將 aryJoinObj 附加至 dtJoined
                        if (childRows != null && childRows.Length > 0)
                        {
                            object[] aryParentObj = FirstTableDataRow.ItemArray;//父資料列物件陣列：將FirstDataTable的每個DataRow轉為物件陣列

                            foreach (DataRow SecondTableDataRow in childRows)
                            {
                                object[] aryChildObj = SecondTableDataRow.ItemArray; // 子資料列物件陣列
                                object[] aryJoinObj = new object[aryParentObj.Length + aryChildObj.Length]; // 依據父與子資料列物件陣列個數，建立 Join 後物件陣列個數
                                Array.Copy(aryParentObj, 0, aryJoinObj, 0, aryParentObj.Length); // 將 aryParentObj 陣列複製至 aryJoinObj 陣列
                                Array.Copy(aryChildObj, 0, aryJoinObj, aryParentObj.Length, aryChildObj.Length); // 將 aryChildObj 陣列複製至 aryJoinObj 陣列

                                dtJoined.LoadDataRow(aryJoinObj, true); // 更新該筆 DataRow，找不到符合的則建立新的 DataRow
                            }
                        }
                    }

                    dtJoined.EndLoadData(); // 載入資料後開啟告知、索引維護和條件約束。Turns on notifications, index maintenance, and constraints after loading data.
                    break;

                case JoinTypeEnum.Outer:
                    dtJoined.BeginLoadData(); // 載入資料時關閉告知、索引維護和條件約束。Turns off notifications, index maintenance, and constraints while loading data.

                    foreach (DataRow FirstTableDataRow in ds.Tables[0].Rows)
                    {
                        // 取得 Join 資料列
                        DataRow[] childRows = FirstTableDataRow.GetChildRows(dr); // 依據關聯性(DataRelation)取得這個父資料列的子資料列

                        // 判斷有無子資料列
                        if (childRows != null && childRows.Length > 0)
                        {
                            // 有子資料列，可在 SecondTable 取得資料，將 FirstTable 與 SecondTable 同樣 Key 值的資料列複製至 aryJoinObj，
                            // 再將 aryJoinObj 附加至 dtJoined

                            object[] aryParentObj = FirstTableDataRow.ItemArray; // 父資料列物件陣列：將 FirstDataTable 的每個 DataRow 轉為物件陣列

                            foreach (DataRow SecondTableDataRow in childRows)
                            {
                                object[] aryChildObj = SecondTableDataRow.ItemArray; // 子資料列物件陣列
                                object[] aryJoinObj = new object[aryParentObj.Length + aryChildObj.Length]; // 依據父與子資料列物件陣列個數，建立 Join 後物件陣列個數
                                Array.Copy(aryParentObj, 0, aryJoinObj, 0, aryParentObj.Length); // 將 aryParentObj 陣列複製至 aryJoinObj 陣列
                                Array.Copy(aryChildObj, 0, aryJoinObj, aryParentObj.Length, aryChildObj.Length); // 將 aryChildObj 陣列複製至 aryJoinObj 陣列

                                dtJoined.LoadDataRow(aryJoinObj, true); // 更新該筆 DataRow，找不到符合的則建立新的 DataRow
                            }
                        }
                        else
                        {
                            // 無子資料列，SecondTable 新增一列空白資料列，將 FirstTable 與 SecondTable 資料列複製至 aryJoinObj，
                            // 再將 aryJoinObj 附加至 dtJoined

                            object[] aryParentObj = FirstTableDataRow.ItemArray; // 父資料列物件陣列
                            DataRow SecondEmptyRow = secondTable.NewRow();
                            object[] aryChildObj = SecondEmptyRow.ItemArray; // 子資料列物件陣列
                            object[] aryJoinObj = new object[aryParentObj.Length + aryChildObj.Length]; // 依據父與子資料列物件陣列個數，建立Join後物件陣列個數
                            Array.Copy(aryParentObj, 0, aryJoinObj, 0, aryParentObj.Length); // 將 aryParentObj 陣列複製至 aryJoinObj 陣列

                            dtJoined.LoadDataRow(aryJoinObj, true); // 更新該筆 DataRow，找不到符合的則建立新的 DataRow
                        }
                    }

                    dtJoined.EndLoadData(); // 載入資料後開啟告知、索引維護和條件約束。Turns on notifications, index maintenance, and constraints after loading data.
                    break;

                case JoinTypeEnum.FullOuter:
                    dtJoined.BeginLoadData(); // 載入資料時關閉告知、索引維護和條件約束。Turns off notifications, index maintenance, and constraints while loading data.

                    // 以 FirstTable 為基準
                    foreach (DataRow FirstTableDataRow in ds.Tables[0].Rows)
                    {
                        // 取得 Join 資料列
                        DataRow[] childRows = FirstTableDataRow.GetChildRows(dr); // 依據關聯性(DataRelation)取得這個父資料列的子資料列

                        // 判斷有無子資料列
                        if (childRows != null && childRows.Length > 0)
                        {
                            // 有子資料列，可在 SecondTable 取得資料，將 FirstTable 與 SecondTable 同樣 Key 值的資料列複製至 aryJoinObj，
                            // 再將 aryJoinObj 附加至 dtJoined

                            object[] aryParentObj = FirstTableDataRow.ItemArray;

                            foreach (DataRow SecondTableDataRow in childRows)
                            {
                                object[] aryChildObj = SecondTableDataRow.ItemArray; // 子資料列物件陣列
                                object[] aryJoinObj = new object[aryParentObj.Length + aryChildObj.Length]; // 依據父與子資料列物件陣列個數，建立Join後物件陣列個數
                                Array.Copy(aryParentObj, 0, aryJoinObj, 0, aryParentObj.Length); // 將 aryParentObj 陣列複製至 aryJoinObj 陣列
                                Array.Copy(aryChildObj, 0, aryJoinObj, aryParentObj.Length, aryChildObj.Length); // 將 aryChildObj 陣列複製至 aryJoinObj 陣列

                                dtJoined.LoadDataRow(aryJoinObj, true); // 更新該筆 DataRow，找不到符合的則建立新的 DataRow
                            }
                        }
                        else
                        {
                            // 無子資料列，SecondTable 新增一列空白資料列，將 FirstTable 與 SecondTable 資料列複製至 aryJoinObj，
                            // 再將 aryJoinObj 附加至 dtJoined

                            object[] aryParentObj = FirstTableDataRow.ItemArray; // 父資料列物件陣列
                            DataRow SecondEmptyRow = secondTable.NewRow();
                            object[] aryChildObj = SecondEmptyRow.ItemArray; // 子資料列物件陣列
                            object[] aryJoinObj = new object[aryParentObj.Length + aryChildObj.Length]; // 依據父與子資料列物件陣列個數，建立 Join 後物件陣列個數
                            Array.Copy(aryParentObj, 0, aryJoinObj, 0, aryParentObj.Length); // 因為無子資料列，只需將 aryParentObj 陣列複製至 aryJoinObj 陣列

                            dtJoined.LoadDataRow(aryJoinObj, true); // 更新該筆 DataRow，找不到符合的則建立新的 DataRow
                        }
                    }

                    // 以 SecondTable 為基準
                    foreach (DataRow SecondTableDatarow in ds.Tables[1].Rows)
                    {
                        DataRow[] parentRows = SecondTableDatarow.GetParentRows(dr);

                        // 找出 SecondTable 有但 FirstTable 沒有的資料列，FirstTable 新增一列空白資料列，
                        // 將 FirstTable 與 SecondTable 資料列複製至 aryJoinObj，再將 aryJoinObj 附加至dtJoined
                        if (parentRows == null || parentRows.Length == 0)
                        {
                            DataRow FirstEmptyRow = firstTable.NewRow();
                            object[] aryFirstObj = FirstEmptyRow.ItemArray;
                            object[] arySecondObj = SecondTableDatarow.ItemArray;
                            object[] aryJoinObj = new object[aryFirstObj.Length + arySecondObj.Length]; // 依據 FirstTable 與 SecondTable 資料列物件陣列個數，建立 Join 後物件陣列個數
                            Array.Copy(arySecondObj, 0, aryJoinObj, aryFirstObj.Length, arySecondObj.Length); // 因為 FirstTable 無資料列，只需將 arySecondObj 陣列複製至 aryJoinObj 陣列

                            dtJoined.LoadDataRow(aryJoinObj, true);
                        }
                    }

                    dtJoined.EndLoadData(); // 載入資料後開啟告知、索引維護和條件約束。Turns on notifications, index maintenance, and constraints after loading data.
                    break;
            }

            // 移除重複的欄位名稱，只保留 FirstDataTable 的欄位名稱
            foreach (DataColumn col in DuplicationColumns)
                dtJoined.Columns.Remove(col);

            return dtJoined;
        }

        /// <summary>
        /// 將 DataTable 作排序
        /// </summary>
        /// <param name="dtSrc">來源 DataTable</param>
        /// <param name="sort">排序字串</param>
        public static DataTable SortDataTable(DataTable dtSrc, string sort)
        {
            DataView dv = dtSrc.DefaultView;
            dv.Sort = sort;
            return dv.ToTable();
        }
    }
}