using System;
using System.Security.Cryptography;
using System.Text;

using log4net;

namespace Library.Component.Utility
{
    /// <summary>
    /// 加解密輔助密封類別。此類別無法被繼承。
    /// </summary>
    public sealed class EncryptUtility
    {
        /// <summary>
        /// Log4Net 物件。
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(EncryptUtility));

        private EncryptUtility()
        {
            //
            // TODO: 在這裡新增建構函式邏輯
            //
        }

        /// <summary>
        /// 加密字串。
        /// </summary>
        /// <param name="source">欲加密字串</param>
        /// <param name="key">加解密金鑰</param>
        /// <returns></returns>
        public static string Encrypt(string source, string key)
        {
            byte[] bSource = Encoding.UTF8.GetBytes(source);
            byte[] bKey = Encoding.UTF8.GetBytes(key);

            MD5CryptoServiceProvider md5csp = new MD5CryptoServiceProvider();
            byte[] bMD5 = md5csp.ComputeHash(bKey);

            RijndaelManaged rijndael = new RijndaelManaged();
            ICryptoTransform ct = rijndael.CreateEncryptor(bMD5, bMD5);

            byte[] bContent = ct.TransformFinalBlock(bSource, 0, bSource.Length);
            string encrypt = Encoding.UTF8.GetString(bContent);

            return encrypt;
        }

        /// <summary>
        /// 解密字串。
        /// </summary>
        /// <param name="cipher">欲解密字串</param>
        /// <param name="key">加解密金鑰</param>
        /// <returns></returns>
        public static string Decrypt(string cipher, string key)
        {
            byte[] bCipher = Convert.FromBase64String(cipher);
            byte[] bKey = Encoding.UTF8.GetBytes(key);

            MD5CryptoServiceProvider md5csp = new MD5CryptoServiceProvider();
            byte[] bMD5 = md5csp.ComputeHash(bKey);

            RijndaelManaged rijndael = new RijndaelManaged();
            ICryptoTransform ct = rijndael.CreateDecryptor(bMD5, bMD5);

            byte[] bContent = ct.TransformFinalBlock(bCipher, 0, bCipher.Length);
            string decrypt = Encoding.UTF8.GetString(bContent);

            return decrypt;
        }
    }
}