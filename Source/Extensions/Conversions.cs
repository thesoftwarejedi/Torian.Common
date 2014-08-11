using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Torian.Common.Extensions
{

public static class Conversions
    {

        public static DateTime? DateTimeTryParseExact(this string input, string format, DateTime? defaultReturn)
        {
            DateTime res;
            if (DateTime.TryParseExact(input, format, null, DateTimeStyles.AssumeLocal, out res))
            {
                return res;
            }
            else
            {
                return defaultReturn;
            }
        }

        public static string ToStringNullSafe(this object o)
        {
            return o == null ? null : o.ToString();
        }

        public static T To<T>(this IConvertible obj)
        {
            Type t = typeof(T);

            if (t.IsGenericType
                && (t.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                if (obj == null || obj.ToString() == "")
                {
                    return (T)(object)null;
                }
                else
                {
                    return (T)Convert.ChangeType(obj, Nullable.GetUnderlyingType(t));
                }
            }
            else
            {
                return (T)Convert.ChangeType(obj, t);
            }
        }

        public static T To<T> (this IConvertible obj, T defaultVal = default(T))
        {
            try
            {
                return To<T>(obj);
            }
            catch
            {
                return defaultVal;
            }
        }

        public static bool To<T> (this IConvertible obj, out T newObj, T defaultVal = default(T))
        {
            try
            {
                newObj = To<T>(obj);
                return true;
            }
            catch
            {
                newObj = defaultVal;
                return false;
            }
        }

    }

}
