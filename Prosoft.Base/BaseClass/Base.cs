using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using Prosoft.Service;
using System.Data;

using System.Web.Mvc;

namespace Prosoft.Base
{

    /// <summary>
    /// OrderBy ListNo
    /// </summary>
    public enum OrderByListNo
    {
        None,
        ASC,
        DESC,
    }

    public class Base
    {
        #region Member
        static EncryptManager encrypt;
        static string id = string.Empty;
        #endregion

        #region Encode
        /// <summary>
        /// convert ค่ากลับคืนปกติ 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DeCode(string text)
        {
            if (MemoryCache.Default["EncryptManager"] != null)
            {
                encrypt = (EncryptManager)MemoryCache.Default["EncryptManager"];
            }
            else
            {
                encrypt = new EncryptManager();
                MemoryCache.Default.Add("EncryptManager", encrypt, DateTime.Now.AddMinutes(5));
            }
            return DataManager.ConvertToString(encrypt.DecryptData(DeCodeForUrl(text)), "");
        }
        /// <summary>
        /// convert ค่ากลับคืนปกติ ใช้กรณี retrun ค่าเป็น int
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int DeCodeID(string ID)
        {

            if (MemoryCache.Default["EncryptManager"] != null)
            {
                encrypt = (EncryptManager)MemoryCache.Default["EncryptManager"];
            }
            else
            {
                encrypt = new EncryptManager();
                MemoryCache.Default.Add("EncryptManager", encrypt, DateTime.Now.AddMinutes(5));
            }

            return DataManager.ConvertToInteger(encrypt.DecryptData(DeCodeForUrl(ID)), 0);
        }
        /// <summary>
        /// convert ค่ากลับคืนปกติ ใช้กรณี retrun ค่าเป็น string 1,2,3... ใช้กับคำสั่ง where column in (DeCodeID(ID))
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string DeCodeID(List<string> ID)
        {

            foreach (var item in ID)
            {
                id += "," + DeCodeID(item).ToString();
            }
            id = id.Remove(0, 1);
            return id;
        }
        /// <summary>
        /// convert string เป็นสายอักขระพิเศษ
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EnCode(string text)
        {
            if (MemoryCache.Default["EncryptManager"] != null)
            {
                encrypt = (EncryptManager)MemoryCache.Default["EncryptManager"];
            }
            else
            {
                encrypt = new EncryptManager();
                MemoryCache.Default.Add("EncryptManager", encrypt, DateTime.Now.AddMinutes(5));
            }
            return EnCodeForUrl(DataManager.ConvertToString(encrypt.EncryptData(text), ""));
        }
        /// <summary>
        /// convert int เป็นสายอักขระพิเศษ
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EnCodeID(int ID)
        {

            return EnCode(ID.ToString());
        }

        /// <summary>
        /// กรณีส่งค่าผ่าน Url
        /// </summary>
        /// <param name="TextEnCode"></param>
        /// <returns></returns>
        public static string EnCodeForUrl(string Text)
        {
            Text = Text.Replace("/", "4869dba4869");
            return Text;
        }

        /// <summary>
        /// กรณีส่งค่าผ่าน Url
        /// </summary>
        /// <param name="TextEnCode"></param>
        /// <returns></returns>
        public static string DeCodeForUrl(string TextEnCode)
        {
            TextEnCode = TextEnCode.Replace("4869dba4869", "/");
            return TextEnCode;
        }
        #endregion

        #region MemoryCache
        public bool MemoryCacheContains(string name)
        {
            return MemoryCache.Default[name] == null ? false : true;
        }

        public void MemoryCacheAdd(string name, object Object)
        {
            MemoryCache.Default.Add(name, Object, DateTime.Now.AddMinutes(5));
        }

        public object MemoryCacheGet(string name)
        {
            return MemoryCache.Default[name];
        }
        #endregion

        public static string AppLang { get; set; }
    }
}