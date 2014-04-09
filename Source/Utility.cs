using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Torian.Common.Extensions;
using System.Web;
using System.Xml.Linq;

namespace Torian.Common
{
    public static class Utility
    {

        private static char[] validKeyChars = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 
                                                 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
                                                 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 
                                                 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
                                                 '2', '3', '4', '5', '6', '7', '8', '9' };

        private static char[] validKeyCharsLcase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 
                                                     'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
                                                     '2', '3', '4', '5', '6', '7', '8', '9' };

        private static Random _r = new Random();

        public static string GenerateRandomString(int length)
        {
            return GenerateRandomString(length, false);
        }

        public static string ApplyMultiplierToNumbersInString(string theString, decimal theMultiplier)
        {
            StringBuilder newDisplay = new StringBuilder(theString);
            foreach (Match match in Regex.Matches(theString, "\\d+\\.?\\d*").Cast<Match>().OrderByDescending(a => a.Index))
            {
                string newString = String.Format("{0:0.00}", match.Value.To<decimal>() * (theMultiplier / 100M));
                newDisplay.Remove(match.Index, match.Length);
                newDisplay.Insert(match.Index, newString);
            }
            return newDisplay.ToString();
        }

        public static decimal? GetFirstNumberInString(string theString)
        {
            var m = Regex.Match(theString, "\\d+\\.?\\d*");
            if (m.Success)
            {
                return m.Value.To<decimal>();
            }
            else
            {
                return null;
            }
        }

        public static string GenerateRandomString(int length, bool caseSensitive)
        {
            char[] k = caseSensitive ? validKeyChars : validKeyCharsLcase;
            char[] keyInChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                keyInChars[i] = k[_r.Next(0, k.Length)];
            }
            return new string(keyInChars);
        }

        public static Dictionary<string, string> DictionaryFromUrlParamString(string paramString)
        {
            return (from keyValue in paramString.Split('&')
                    let tmpArray = keyValue.Split('=')
                    select new KeyValuePair<string, string>(HttpUtility.UrlDecode(tmpArray[0]), HttpUtility.UrlDecode(tmpArray[1])))
                        .ToDictionary(a => a.Key, a => a.Value);
        }

        private static readonly Regex _makeUrlRegex = new Regex("[^\\w\\d]+");

        public static string MakeUrl(string beforeString)
        {
            return _makeUrlRegex.Replace(beforeString.Replace("'", "").Replace(".", ""), "-");
        }

    }

}
