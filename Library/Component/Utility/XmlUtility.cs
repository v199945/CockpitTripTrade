using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

using log4net;

namespace Library.Component.Utility
{
    /// <summary>
    /// XML 檔案操作輔助密封類別。此類別無法被繼承。
    /// </summary>
    public sealed class XmlUtility
    {
        /// <summary>
        /// Log4Net 物件。
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(XmlUtility));

        private XmlUtility()
        {
            //
            // TODO: 在這裡新增建構函式邏輯
            //
        }

        /// <summary>
        /// 搜尋XML節點。
        /// </summary>
        /// <param name="xml">XML 字串</param>
        /// <param name="xpath">XPath</param>
        public static string SearchXmlNodes(string xml, string xpath)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return string.Empty;
            }

            // TODO: 檢查xpath變數

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null; // 不解析外部的DTD(Document Type Definition)、實體及結構描述
            doc.LoadXml(xml);

            XmlNodeList list = doc.SelectNodes(xpath);

            return list[0].InnerText;
            //foreach (XmlNode node in list)
            //{
            //}
        }
    }
}