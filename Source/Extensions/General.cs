using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Torian.Common.Extensions
{

    public static class General
    {

        public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default) => dict.TryGetValue(key, out TV value) ? value : defaultValue;
        
        public static T TryOrDefault<U, T>(this U u, Func<U, T> f)
        {
            try
            {
                return f(u);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static int ContainsCount(this string a, IEnumerable<string> b)
        {
            return b.Count(c => a.Contains(c));
        }

        public static string SubstringSafe(this string s, int startIndex, int length)
        {
            if (s == null) return s;
            if (startIndex + length > s.Length)
            {
                return s.Substring(startIndex);
            }
            else
            {
                return s.Substring(startIndex, length);
            }
        }

        public static long GetEpochTime(this DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static string Format(this string a, params object[] p) 
        {
            return string.Format(a, p);
        }

    }

}
