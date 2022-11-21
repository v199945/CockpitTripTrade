using System.IO;
using System.Text;

namespace Library.Component.BLL
{
    /// <summary>
    /// 實作可自定字元編碼格式、寫入資訊至字串的類別，繼承自 StringWriter 類別。
    /// </summary>
    public class EncodingStringWriter : StringWriter
    {
        #region "Property"

        /// <summary>
        /// 覆寫 StringWriter 類別字元編碼格式屬性。
        /// </summary>
        public override Encoding Encoding { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// 預設建構子。
        /// </summary>
        public EncodingStringWriter() : base()
        {
        }

        public EncodingStringWriter(StringBuilder builder, Encoding encoding) : base(builder)
        {
            this.Encoding = encoding;
        }

        #endregion

    }
}