using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Library.Component.BLL
{
    // 新增加密、解密程式，保護Config的連線字串
    public class Encryption
    {       
        /// <summary>
        /// 加密程式。
        /// </summary>
        /// <param name="PlainText"></param>
        /// <returns></returns>
        public string SetEncryption(string PlainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                //加密金鑰(32 Byte)
                aesAlg.Key = Encoding.Unicode.GetBytes("中華航空加密金鑰中華航空加密金鑰");
                //初始向量(Initial Vector, iv) 類似雜湊演算法中的加密鹽(16 Byte)
                aesAlg.IV = Encoding.Unicode.GetBytes("中華航空加密向量");
                //加密器
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                //執行加密
                byte[] encrypted = encryptor.TransformFinalBlock(Encoding.Unicode.GetBytes(PlainText), 0,
        Encoding.Unicode.GetBytes(PlainText).Length);

                return Convert.ToBase64String(encrypted);
            }
        }

        /// <summary>
        /// 解密程式。
        /// </summary>
        /// <param name="CipherText"></param>
        /// <returns></returns>
        public string GetDecryption(string CipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                //加密金鑰(32 Byte)
                aesAlg.Key = Encoding.Unicode.GetBytes("中華航空加密金鑰中華航空加密金鑰");
                //初始向量(Initial Vector, iv) 類似雜湊演算法中的加密鹽(16 Byte)
                aesAlg.IV = Encoding.Unicode.GetBytes("中華航空加密向量");
                //加密器
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                //執行加密
                byte[] decrypted = decryptor.TransformFinalBlock(Convert.FromBase64String(CipherText), 0, Convert.FromBase64String(CipherText).Length);
                return Encoding.Unicode.GetString(decrypted);
            }
        }


    }
}
