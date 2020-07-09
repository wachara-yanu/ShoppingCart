using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Prosoft.Service
{
    /// <summary>
    /// การเข้ารหัสและการถอดรหัส
    /// </summary>
    public class EncryptManager
    {
        #region กำหนดค่าของ Key
        //Default PrivateKey :: ห้ามน้อยกว่า 6 มากกว่า 9
        private const String defaultpublicKey = "Big8kHan#";
        //สามารถทำการเปลี่ยนแปลง PrivateKey ได้ :: ห้ามน้อยกว่า 4 มากกว่า 13
        //หมายเหตุ *** ห้ามทำการแก้ไข Private โดยเด็กขาด
        private const String privateKey = "@2c0pY0RighT5";
        //สามารถกำหนดค่า public Key เพื่อเข้ารหัส
        private String publicKey = defaultpublicKey;
        #endregion

        /// <summary>
        ///กำหนดค่า Key เพื่อทำการ Encode Data
        /// </summary>
        public String PublicKey
        {
            set
            {
                publicKey = value;
            }
        }
        /// <summary>
        /// จะทำการแปลงข้อมูลให้อยู่ในรูปแบบที่สามาถร เข้ารหัสได้
        /// </summary>
        private bool VaridateRange(String UserName, String pass)
        {
            if (!Varidation.VaridateRangeUserName(UserName)) return true;
            if (!Varidation.VaridateRangePassword(pass)) return true;//ไม่สามารถใช้งานได้
            return false;//ใช้งานได้
        }
        /// <summary>
        /// ทำการ Encode ข้อมูล เช่น Password ไม่สามารถถอดหรัสได้
        /// </summary>
        public string GetEncoding(string UserName, string pass)
        {
            try
            {
                UserName = UserName.ToLower();
                if (VaridateRange(UserName, pass)) return "";
                //เอา ตัวแรกของ  UserName คือ  U มาแทรกในตำแหน่งที่  3 โดยเริ่มตำแหน่งแรกเป็น 1  paUsswo
                pass = pass.Insert(2, UserName.Substring(0, 1));
                //เอา ตัวที่ 3 ของ UserName คือ e มาแทรกในตำแหน่งที่  6 โดยเริ่มตำแหน่งแรกเป็น 1  paUssewo
                pass = pass.Insert(5, UserName.Substring(2, 1));
                //เอา ตัวสุดท้าย ของ UserName คือ r มาแทรกในตำแหน่งที่  2 โดยเริ่มตำแหน่งแรกเป็น 1  praUssewo
                pass = pass.Insert(1, UserName.Substring(UserName.Length - 1, 1));
                //นำตัวอักษรทั้งหมดมาบวกกัน ไปต่อข้อความกับ ข้อความเดิม แล้วนำตัวแรกไปต่อท้าย เอาตัวท้ายมาไว้ตัวหน้า
                char[] charpass = pass.ToCharArray();
                string temp;
                int sum = 0;
                foreach (char ch in charpass)
                    sum += (byte)ch;
                temp = sum.ToString();
                pass += temp.Substring(0, 1);
                pass = temp.Substring(temp.Length - 1, 1) + pass;

                #region <KeyConvert>
                string[,] KeyFlag = new string[2, 5];
                if ((sum % 2) == 0)
                {
                    KeyFlag[0, 0] = "A"; KeyFlag[1, 0] = "!";
                    KeyFlag[0, 1] = "e"; KeyFlag[1, 1] = "#";
                    KeyFlag[0, 2] = "I"; KeyFlag[1, 2] = "฿";
                    KeyFlag[0, 3] = "o"; KeyFlag[1, 3] = "$";
                    KeyFlag[0, 4] = "U"; KeyFlag[1, 4] = "@";
                }
                else
                {
                    KeyFlag[0, 0] = "a"; KeyFlag[1, 0] = "%";
                    KeyFlag[0, 1] = "E"; KeyFlag[1, 1] = "&";
                    KeyFlag[0, 2] = "i"; KeyFlag[1, 2] = ":";
                    KeyFlag[0, 3] = "O"; KeyFlag[1, 3] = "^";
                    KeyFlag[0, 4] = "u"; KeyFlag[1, 4] = "*";
                }
                #endregion

                for (int i = 0; i < pass.Length; i++)
                    if ((i % 2) == 0)
                        for (int j = 0; j < 5; j++)
                            if (KeyFlag[0, j] == pass[i].ToString())
                                pass = pass.Replace(pass[i].ToString(), KeyFlag[1, j]);
                temp = "";
                charpass = pass.ToCharArray();
                foreach (char ch in charpass)
                    temp += ((byte)ch).ToString("000");
                pass = temp;
                sum = 0;
                foreach (char ch in pass)
                    sum += (int)ch;
                pass += sum.ToString("0000").Substring(0, 4);
                string rtn = Authenticate(pass).ToString();
                return rtn;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// ส่งข้อมูลที่ทำการแปลงข้อมูลให้อยู่ใน รูปของการ Encode แล้ว
        /// </summary>
        string Authenticate(string userPwd)
        {
            if (Varidation.IsEmpty(userPwd)) return "";
            // Should we create a hash for the password?
            if (userPwd.Length > 0)
                userPwd = HashData(userPwd);
            return userPwd;
        }
        /// <summary>
        /// ทำการแปลงข้อมูลให้อยู่ใน รูปของการ Encode
        /// </summary>
        string HashData(string data)
        {
            try
            {
                byte[] msgBytes = System.Text.Encoding.Unicode.GetBytes(data);
                SHA1Managed sha1 = new SHA1Managed();
                byte[] hashBytes = sha1.ComputeHash(msgBytes);
                StringBuilder hashRet = new StringBuilder("");
                foreach (byte b in hashBytes)
                    hashRet.AppendFormat("{0:X}", b);
                return hashRet.ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// A method to Encrypt/decrypt a specified Triple DES encrypted file using the 
        /// key and iv provided.
        /// สำหรับ Encrypt ข้อมูล อื่นๆ ที่ไม่ใช้ Password สามารถถอดรหัสได้
        /// </summary>
        public string EncryptData(string plainMessage)
        {
            return EncryptData(plainMessage, publicKey);
        }
        /// <summary>
        /// A method to Encrypt/decrypt a specified Triple DES encrypted file using the 
        /// key and iv provided.
        /// สำหรับ Encrypt ข้อมูล อื่นๆ ที่ไม่ใช้ Password สามารถถอดรหัสได้
        /// </summary>
        private string EncryptData(string plainMessage, string password)
        {
            if (Varidation.IsEmpty(plainMessage)) return "";
            try
            {
                password = GetEncoding(privateKey, password);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.IV = new byte[8];
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[0]);
                des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
                MemoryStream ms = new MemoryStream(plainMessage.Length * 2);
                CryptoStream encStream = new CryptoStream(ms, des.CreateEncryptor(),
                    CryptoStreamMode.Write);
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainMessage);
                encStream.Write(plainBytes, 0, plainBytes.Length);
                encStream.FlushFinalBlock();
                byte[] encryptedBytes = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(encryptedBytes, 0, (int)ms.Length);
                encStream.Close();
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// A method to Encrypt/decrypt a specified Triple DES encrypted file using the 
        /// key and iv provided.
        /// สำหรับ Decrypt ข้อมูล 
        /// </summary>
        public string DecryptData(string encryptedBase64)
        {
            return DecryptData(encryptedBase64, publicKey);
        }
        /// <summary>
        /// A method to Encrypt/decrypt a specified Triple DES encrypted file using the 
        /// key and iv provided.
        /// สำหรับ Decrypt ข้อมูล 
        /// </summary>
        private string DecryptData(string encryptedBase64, string password)
        {
            if (Varidation.IsEmpty(encryptedBase64)) return "";
            try
            {
                password = GetEncoding(privateKey, password);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.IV = new byte[8];
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[0]);
                des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
                byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
                MemoryStream ms = new MemoryStream(encryptedBase64.Length);
                CryptoStream decStream = new CryptoStream(ms, des.CreateDecryptor(),
                    CryptoStreamMode.Write);
                decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                decStream.FlushFinalBlock();
                byte[] plainBytes = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(plainBytes, 0, (int)ms.Length);
                decStream.Close();
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception e)
            {
                //throw new NotImplementedException();
                //return Redirect(res.Pageviews.PvNotFound);
                //throw new Exception("Can't convert because data type of column " + columnDestination + " is not String!");
                throw new Exception(e.Message);
            }
        }

        

        /// <summary>
        /// ตรวจสอบข้อมูลต่างของ EncryptAlgorithm
        /// </summary>
        public class Varidation
        {
            /// <summary>
            /// ตรวจสอบค่าว่าง
            /// Return True:ว่าง  , False:Password:ไม่ว่าง
            /// </summary>
            public static bool IsEmpty(String Data)
            {
                return (String.Equals(Data, String.Empty));
            }
            /// <summary>
            /// สำหรับตรวจสอบช่วงหรือรูปแบบของ UserName ว่าสามารถใช้เข้ารหัสได้
            /// Return True:User นี้สามารถใช้งานได้  , False:User นี้ไม่สามารถใช้งานได้
            /// </summary>
            public static bool VaridateRangeUserName(String UserName)
            {
                if (IsEmpty(UserName) || String.Equals(UserName, null)) return false;
                if ((UserName.Length < 4) || (UserName.Length > 13)) return false;
                //Call Function Option
                return true;
            }
            /// <summary>
            /// สำหรับตรวจสอบช่วงหรือรูปแบบของรหัสผ่านว่าสามารถใช้เข้ารหัสได้
            /// Return True:Password นี้สามารถใช้งานได้  , False:Password นี้ไม่สามารถใช้งานได้
            /// </summary>
            public static bool VaridateRangePassword(String Password)
            {
                if ((Password.Length < 6) || (Password.Length > 9)) return false;
                //Call Function Option  เพื่อกำหนดการทำงาน
                return true;
            }
            /// <summary>
            /// สำหรับตรวจสอบรหัสผ่านสามารถว่างได้หรือไม่
            /// Return True:ว่างได้  , False:Password:ว่างไม่ได้
            /// </summary>
            public static bool VaridateNullPassword(String Password)
            {
                return ((IsEmpty(Password) || String.Equals(Password, null)));
            }
        }
    }
}
