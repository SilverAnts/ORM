using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;
using System.Net;

namespace SilverAnts.Core.Utilities
{
    /// <summary>
    /// 数据安全
    /// </summary>
    public class Security
    {
        #region MD5
        /// <summary>
        /// 对某个字符串进行加密
        /// </summary>
        /// <param name="inputPassword"></param>
        /// <returns></returns>
        public static string EncryptMd5(string inputPassword)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(inputPassword);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(b[i].ToString("x").PadLeft(2, '0'));

            }
            return sb.ToString();
        }
        /// <summary>
        /// 对某个字符串进行加密-配置全局Key AppSettrings -- Key
        /// </summary>
        /// <param name="inputPassword"></param>
        /// <returns></returns>
        public static string EncryptMd5UKey(string inputPassword)
        {
            var key_des = EncryptDES(inputPassword, ConfigurationManager.AppSettings["Key"]);
            return EncryptMd5(key_des);
        }
        #endregion

        #region DES
        public static string DecryptDES(string encryptedString, string encryptKey)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] btIV = Encoding.UTF8.GetBytes(encryptKey.Substring(encryptKey.Length - 8, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(encryptedString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch
                {
                    return encryptedString;
                }
            }
        }
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] btIV = Encoding.UTF8.GetBytes(encryptKey.Substring(encryptKey.Length - 8, 8));
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
                catch
                {
                    return encryptString;
                }
            }
        }
        #endregion

        #region SHA256
        public static string EncryptSHA256(string regNum)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一个SHA256
            byte[] source = Encoding.Default.GetBytes(regNum);//将字符串转为Byte[]
            byte[] crypto = sha256.ComputeHash(source);//进行SHA256加密
            return Convert.ToBase64String(crypto);//把加密后的字符串从Byte[]转为字符串
        }

        #endregion

        #region Base64fdsa

        public static string EncodeBase64(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(source);
                return Convert.ToBase64String(bytes);
            }
        }
        public static string DecodeBase64(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                return "";
            }
            else
            {
                byte[] bytes = Convert.FromBase64String(result);
                return Encoding.UTF8.GetString(bytes);
            }
        }
        #endregion
    }
}
