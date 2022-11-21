using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

using log4net;

namespace Library.Component.Utility
{
    /// <summary>
    /// HTTP Web 輔助密封類別。此類別無法獲得繼承。
    /// </summary>
    public sealed class HttpWebUtility
    {
        /// <summary>
        /// Log4Net 物件。
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(HttpWebUtility));

        /// <summary>
        /// 將建構子之存取修飾詞改為 private 防止建立本類別物件。
        /// </summary>
        private HttpWebUtility()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri">URI</param>
        /// <param name="isKeepAlive">是否要與網際網路資源建立持續連線，如果對網際網路資源的要求應該包含具有值 Keep-alive 的 Connection HTTP 標頭，則為 true，否則為 false。</param>
        /// <param name="postParams">欲傳送資料之 Key-Value 集合物件</param>
        /// <param name="contentType">Content-type HTTP 標頭值</param>
        /// 
        /// 
        /// <returns></returns>
        /// TODO: 可調整為接收多組 Key 與 Data
        public static string PostData(string uri, WebHeaderCollection headers, bool isKeepAlive, NameValueCollection postParams, string contentType)
        {
            string logDetail = @"uri=" + uri + ", headers=" + headers.ToString() + ", isKeepAlive=" + isKeepAlive.ToString() + ", postParams=" + postParams.ToString() + ", contentType=" + contentType;
            logger.Info(@"HttpWebUtility.PostData(), " + logDetail);

            string responseData = null;
            byte[] byteDataArray = Encoding.UTF8.GetBytes(postParams.ToString()); // 將欲傳送的資料字串轉換為 Byte 陣列
            //new UTF8Encoding().GetBytes(postParams.ToString());
            //Encoding.ASCII.GetBytes(postData);

            // 1. 呼叫接收資料之網路資源，例如：指令碼或 ASP.NET 網頁資源之 URI，為指定的 URI 配置，初始化新的 WebRequest 執行個體
            HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create(uri);
            if (headers != null) httpWebRequest.Headers = headers;
            httpWebRequest.KeepAlive = isKeepAlive;

            // 2. 設定 HttpWebRequest 的驗證資訊
            //httpWebRequest.Credentials = new NetworkCredential(Config.ImpersonateUsername, Config.ImpersonatePassword, Config.ImpersonateDomain);
            
            // 3. 設定所傳送資料使用的通訊協定方法
            httpWebRequest.Method = "POST";
            
            // 4. 設定 ContentLength 屬性(Content-length HTTP 標頭)
            httpWebRequest.ContentLength = byteDataArray.Length;
            
            // 5. 設定 ContentType 屬性(Content-type HTTP 標頭值)
            httpWebRequest.ContentType = contentType;

            try
            {
                // 6. 藉由呼叫所取得 GetRequestStream 方法保留要求資料的資料流
                using (Stream dataStream = httpWebRequest.GetRequestStream())
                {
                    // 7. 將傳回的 Stream 物件寫入資料
                    dataStream.Write(byteDataArray, 0, byteDataArray.Length);

                    // 8. 將要求傳送至伺服器藉由呼叫 GetResponse 方法，傳回包含伺服器之回應的物件。傳回的 WebResponse 目標型別視所要求 URI 的配置
                    using (HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse())
                    {
                        // 9. 取得 WebResponse 回應傳回的狀態描述，輸出至 Log
                        logger.Info("HttpWebResponse Status=" + ((HttpWebResponse) httpWebResponse).StatusDescription);

                        // 10. 取得傳送後包含 WebResponse 回應資料的資料流
                        using (Stream stream = httpWebResponse.GetResponseStream())
                        {
                            // 11. 使用 StreamReader 初始化 WebResponse 回應的資料流
                            using (StreamReader sr = new StreamReader(stream))
                            {
                                // 12. 讀取資料流內容字串
                                responseData = sr.ReadToEnd();
                            }
                        }

                        if (((HttpWebResponse) httpWebResponse).StatusCode == HttpStatusCode.OK)
                        {
                            return responseData;
                        }
                        else
                        {
                            logger.Error("HttpWebResponse Status IS NOT OK, Status Code=" + ((HttpWebResponse) httpWebResponse).StatusCode.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("HttpWebUtility.PostData(), Exception=" + ex.ToString());

                throw ex;
            }

            return responseData;
        }
    }
}