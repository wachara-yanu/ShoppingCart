using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace ProsoftQueryTools.Service
{
    /// <summary>
    /// Check and Create Plural Word
    /// </summary>
    public class PluralManager
    {
        /// <summary>
        /// Get plural word from singular word
        /// </summary>
        /// <param name="str">Singular word</param>
        /// <returns>Plural word</returns>
        public static string GetPlural(string str)
        {
            string strTemp = str;//= str.ToLower();
            var s = new List<string>();
            for (var i = 0; i < strTemp.Length; i++)
            {
                s.Add(strTemp.Substring(i, 1));
            }

            if (strTemp.Length > 0)
            {
                #region Process Check Y
                if (s[strTemp.Length - 1] == "y")
                {
                    var sr = CheckVowel(s[s.Count - 2]);
                    if (sr == true)
                        strTemp = strTemp + "s";
                    else
                        strTemp = strTemp.Substring(0, strTemp.Length - 1) + "ies";
                }
                else if (s[strTemp.Length - 1] == "s" || s[strTemp.Length - 1] == "x" || s[strTemp.Length - 1] == "z" || s[strTemp.Length - 1] == "o")
                {
                    strTemp = strTemp.Substring(0, strTemp.Length) + "es";
                }
                else if (strTemp.Substring(strTemp.Length - 2, 2) == "ss" || strTemp.Substring(strTemp.Length - 2, 2) == "sh" || strTemp.Substring(strTemp.Length - 2, 2) == "ch")
                {
                    strTemp = strTemp.Substring(0, strTemp.Length) + "es";
                }
                else
                {
                    strTemp = strTemp + "s";
                }
                #endregion
            }

            strTemp = UpperFirstWord(strTemp);
            return strTemp;
        }

        /// <summary>
        /// Get Upper First Word
        /// </summary>
        /// <param name="str">word</param>
        /// <returns>Upper First Word</returns>
        private static string UpperFirstWord(string str)
        {
            str = str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1);
            return str;
        }

        /// <summary>
        /// Checks the vowel.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>True if specified character is vowel, otherwise return False</returns>
        private static bool CheckVowel(string character)
        {
            var b = false;
            if (character == "a" || character == "e" || character == "o" || character == "u" || character == "u")
            {
                b = true;
            }
            return b;
        }
    }
}
