using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace Torian.Common.Extensions
{

    public static class Web
    {

        public static string UserHostAddressSafe(this HttpRequest req)
        {
            string ip = null;
            try
            {
                ip = req.UserHostAddress;
            }
            catch { }
            return ip;
        }

        public static string DownloadUrl(this string s, params string[] p)
        {
            using (WebClient wc = new WebClient())
            {
                return s.DownloadUrl(wc, string.Format(s, p));
            }
        }

        public static string DownloadUrl(this string s, WebClient wc, params string[] p)
        {
            return wc.DownloadString(string.Format(s, p));
        }

    }

}
