using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Torian.Common.Extensions
{

    public static class IO
    {

        public static IEnumerable<string> SplitLines(this string s)
        {
            using (StringReader read = new StringReader(s))
            {
                string line = null;
                while ((line = read.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static Stream ToStream(this string s)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter write = new StreamWriter(ms);
            write.Write(s);
            write.Flush();
            ms.Position = 0;
            return ms;
        }

        public static string ReadToEnd(this Stream s)
        {
            using (StreamReader read = new StreamReader(s))
            {
                return read.ReadToEnd();
            }
        }

    }

}
