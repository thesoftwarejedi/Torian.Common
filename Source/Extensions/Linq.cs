using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace Torian.Common.Extensions
{

    public static class Linq
    {

        public static int? IndexOfFirst<T>(this IEnumerable<T> e, Func<T, bool> exp) 
        {
            int i = 0;
            foreach (var item in e)
            {
                if (exp(item)) return i;
                i++;
            }
            return null;
        }

        public static T FindOrCreate<T, TOrderKey>(this Table<T> table, Expression<Func<T, bool>> find, Expression<Func<T, TOrderKey>> order, bool desc, Action<T> create) where T : class, new()
        {
            T val = null;
            if (desc)
            {
                val = table.OrderByDescending(order).FirstOrDefault(find);
            }
            else
            {
                val = table.OrderBy(order).FirstOrDefault(find);
            }
            if (val == null)
            {
                val = new T();
                create(val);
                table.InsertOnSubmit(val);
            }
            return val;
        }

        public static T FindOrCreate<T>(this Table<T> table, Expression<Func<T, bool>> find, Action<T> create) where T : class, new()
        {
            T val = table.FirstOrDefault(find);
            if (val == null)
            {
                val = new T();
                create(val);
                table.InsertOnSubmit(val);
            }
            return val;
        }


        public static T FindOrCreate<T>(this Table<T> table, Expression<Func<T, bool>> find) where T : class, new()
        {
            return FindOrCreate(table, find, a => { });
        }

        public static XElement ElementDeep(this XElement container, params XName[] names)
        {
            foreach (var name in names)
            {
                container = container.Element(name);
                if (container == null) return null;
            }
            return container;
        }

        public static string ValueOrNull(this XElement element)
        {
            if (element == null)
            {
                return null;
            }
            return element.Value;
        }

    }

}
