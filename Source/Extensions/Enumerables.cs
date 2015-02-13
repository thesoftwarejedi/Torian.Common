using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Torian.Common.Extensions
{

    public static class Enumerables
    {

        public static IEnumerable<T> Interleave<T>(this IEnumerable<T> l1, IEnumerable<T> l2, int l1Count, int l2Count)
        {
            return Interleave(l1.GetEnumerator(), l2.GetEnumerator(), l1Count, l2Count);
        }

        public static IEnumerable<T> Interleave<T>(this IEnumerator<T> l1, IEnumerator<T> l2, int l1Count, int l2Count)
        {
            if (l1 != null)
            {
                for (int i = 0; i < l1Count; i++)
                {
                    if (l1.MoveNext())
                    {
                        yield return l1.Current;
                    }
                    else
                    {
                        l1 = null;
                        break;
                    }
                }
            }
            if (l2 != null)
            {
                for (int i = 0; i < l2Count; i++)
                {
                    if (l2.MoveNext())
                    {
                        yield return l2.Current;
                    }
                    else
                    {
                        l2 = null;
                        break;
                    }
                }
            }
            //recurse if either not ended
            if (l1 != null || l2 != null)
            {
                foreach (var item in Interleave(l1, l2, l1Count, l2Count))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> EveryOther<T>(this IEnumerable<T> e, T t)
        {
            foreach (var item in e)
            {
                yield return item;
                yield return t;
            }
        }

        public static string ToStringEach<T>(this IEnumerable<T> e)
        {
            StringBuilder sb = new StringBuilder();
            e.Select(a => a.ToStringNullSafe()).EveryOther(Environment.NewLine).ForEachImmediate(a => sb.Append(a));
            return sb.ToString();
        }

        private static Random _s_Random = new Random();

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return Shuffle(source, _s_Random);
        }

        //attribution: http://stackoverflow.com/questions/1287567/is-using-random-and-orderby-a-good-shuffle-algorithm
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random r)
        {
            T[] items = source.ToArray();
            for (int i = items.Length - 1; i >= 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                // ... except we don't really need to swap it fully, as we can
                // return it immediately, and afterwards it's irrelevant.
                int swapIndex = r.Next(i + 1);
                yield return items[swapIndex];
                items[swapIndex] = items[i];
            }
        }

        public static IEnumerable<IEnumerable<T>> InBatches<T>(this IEnumerable<T> source, int batchSize)
        {
            for (IEnumerable<T> s = source; s.Any(); s = s.Skip(batchSize))
                yield return s.Take(batchSize);
        }

        public static IEnumerable<IEnumerable<T>> SplitEnumerable<T>(this IEnumerable<T> source, int items)
        {
            for (int i = 0; i < items; ++i)
                yield return source.Where((x, index) => (index % items) == i);
        }

        public static IEnumerable<T> AtLeast<T>(this IEnumerable<T> source, int howMany)
        {
            int i = howMany;
            foreach (var item in source)
            {
                i--;
                yield return item;
            }
            while (i-- > 0)
            {
                yield return default(T);
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static int SumOrDefault<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            if (source == null || source.Count() <= 0)
                return 0;
            return source.Sum(selector);
        }

        public static int SumOrDefault<T>(this IEnumerable<T> source, Func<T, int?> selector)
        {
            if (source == null || source.Count() <= 0)
                return 0;
            return source.Sum(selector) ?? 0;
        }

        public static decimal SumOrDefault<T>(this IEnumerable<T> source, Func<T, decimal> selector)
        {
            if (source == null || source.Count() <= 0)
                return 0M;
            return source.Sum(selector);
        }

        public static decimal SumOrDefault<T>(this IEnumerable<T> source, Func<T, decimal?> selector)
        {
            if (source == null || source.Count() <= 0)
                return 0M;
            return source.Sum(selector) ?? 0M;
        }

        public static IEnumerable<T> ForEachChained<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        public static IEnumerable<T> ForEachImmediate<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
            return source;
        }

        public static IEnumerable<T> RandomRange<T>(this IEnumerable<T> source, int numOfItems)
        {
            return source.RandomRange(new Random(), numOfItems);
        }

        public static IEnumerable<T> RandomRange<T>(this IEnumerable<T> source, Random random, int numOfItems)
        {
            //is this the most efficient, or is it better to get the whole 
            //random index list, order, then iterate forward only?
            //
            //for arrays it shouldn't matter, but it does have a major 
            //issue with non indexable data structures. however, ordering them 
            //after picking them isn't exactly inferred behavior...
            int cnt = source.Count();
            if (cnt <= numOfItems)
            {
                //we're being asked for more than we have, just quietly return what we have
                foreach (var item in source)
                {
                    yield return item;
                }
                yield break;
            }
            else
            {
                HashSet<int> chosenItems = new HashSet<int>();
                for (int i = 0; i < numOfItems; i++)
                {
                    int newNum = -1;
                    do
                    {
                        newNum = random.Next(0, cnt);
                    } while (chosenItems.Contains(newNum));
                    chosenItems.Add(newNum);
                    yield return source.ElementAt(newNum);
                }
                yield break;
            }
        }

        public static TValue GetValueOrDefault<TKey, TValue> (this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue ret;
            // Ignore return value
            dictionary.TryGetValue(key, out ret);
            return ret;
        }

    }

}
