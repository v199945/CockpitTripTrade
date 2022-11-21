using System;
using System.Collections.Generic;

namespace Library.Component.Enums
{
    /// <summary>
    /// 回傳物件類型列舉型態。
    /// </summary>
    public enum ReturnObjectTypeEnum
    {
        /// <summary>
        /// 回傳物件類型為集合。
        /// </summary>
        Collection,

        /// <summary>
        /// 回傳物件類型為資料表。
        /// </summary>
        DataTable,

        /// <summary>
        /// 回傳物件類型為陣列。
        /// </summary>
        Array,

        /// <summary>
        /// 回傳物件類型為資料集。
        /// </summary>
        DataSet,

        /// <summary>
        /// 回傳物件類型為資料列。
        /// </summary>
        DataRow,

        /// <summary>
        /// 回傳物件類型為 XML。
        /// </summary>
        Xml,

        /// <summary>
        /// 回傳物件類型為 Json。
        /// </summary>
        Json
    }
}