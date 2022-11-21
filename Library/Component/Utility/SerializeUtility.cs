using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Library.Component.BLL;

namespace Library.Component.Utility
{
    /// <summary>
    /// 序列化(Serialize)與反序列化(Deserialize)輔助密封類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class SerializeUtility
    {
        /// <summary>
        /// 建構子之存取修飾詞改為 private 防止建立本類別物件。
        /// </summary>
        private SerializeUtility()
        {
        }

        /// <summary>
        /// 序列化(Serialize)
        /// </summary>
        /// <param name="o">需要被序列化的物件</param>
        /// <returns>序列化字串</returns>
        /// <remarks>
        /// Sample Code
        /// ViewState["xxx"] = SerializeUtility.Serialize(obj)
        /// </remarks>
        public static string Serialize(object o)
        {
            XmlSerializer xs = new XmlSerializer(o.GetType());
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            EncodingStringWriter esw = new EncodingStringWriter(sb, Encoding.UTF8);
            //StringWriter sw = new StringWriter(sb);

            xs.Serialize(esw, o, xsn);

            return sb.ToString();
        }

        /// <summary>
        /// 反序列化(Deserialize)
        /// </summary>
        /// <typeparam name="T">反序列化類別型態</typeparam>
        /// <param name="s">序列化字串</param>
        /// <returns>反序列化類別型態</returns>
        /// <remarks>
        /// Sample Code
        /// Class myClass = SerializeUtility.Deserialize(of myClass)(ViewState["xxx"].ToString())
        /// </remarks>
        public static T Deserialize<T>(string s)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.XmlResolver = null;
            xdoc.LoadXml(s);
            XmlNodeReader xnr = new XmlNodeReader(xdoc.DocumentElement);
            
            XmlSerializer xs = new XmlSerializer(typeof(T));
            object o = xs.Deserialize(xnr);

            return (T)o;
        }
    }

}