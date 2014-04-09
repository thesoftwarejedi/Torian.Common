using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace Torian.Common.Web
{

    public static class UrlHelper
    {

        public static Regex _makeUrlRegex = new Regex("[^\\w\\d]+");

        public static string MakeUrl(string beforeString)
        {
            return _makeUrlRegex.Replace(beforeString.Replace("'", "").Replace(".", ""), "-");
        }

        public static string MakeUrlDashDots(string beforeString)
        {
            return _makeUrlRegex.Replace(beforeString.Replace("'", ""), "-");
        }

        private static Regex _extractDomainRegex = new Regex(@"https?:/+[?\w\d.-]*?(?'domain'[\w\d-]+\.[\w\d]{2,4}(\.ag|\.bz|\.es|\.in|\.mx|\.nz|\.tw|\.uk|\.ar|\.br|\.in|\.il|\.sg|\.hk|\.au|\.za|\.jp)?)([^.\w\d-]+|$)");

        public static string ExtractHttpDomain(string url)
        {
            if (url == null) return null;

            if (!url.Contains("://"))
            {
                url = "http://" + url;
            }

            string domain = null;

            var match = _extractDomainRegex.Match(url);
            if (match != null && match.Success)
            {
                domain = match.Groups["domain"].Value;
            }

            return domain == null ? null : domain.ToLower();
        }

        private static Regex _extractEntireDomainRegexMinusWww = new Regex(@"https?:/+(www\.)?(?'domain'[\w\d\.-]+).*");

        public static string ExtractEntireHttpDomainMinusWww(string url)
        {
            if (url == null) return null;

            if (!url.Contains("://"))
            {
                url = "http://" + url;
            }

            string domain = null;

            var match = _extractEntireDomainRegexMinusWww.Match(url);
            if (match != null && match.Success)
            {
                domain = match.Groups["domain"].Value;
            }

            return domain == null ? null : domain.ToLower();
        }

        public static bool IsRootDomain(string domain)
        {
            return ExtractHttpDomain(domain) == ExtractEntireHttpDomainMinusWww(domain);
        }

        private static Regex _findShareASaleLinkRegex = new Regex("(?<=['\"])http://.*?(?=['\"])");
        private static Regex _findAffiliateTechnologyRegex = new Regex("content=\"0;url=(.*)\"");

        public static string FollowUrl(string url, bool isShareASale)
        {
            WebClientEx wc = new WebClientEx();
            wc.CookieContainer = new CookieContainer();
            wc.UserAgent = WebClientEx.USER_AGENT_CHROME; //some sites error on the .NET useragent, so we mimic chrome
            var contents = wc.DownloadString(new Uri(url));
            if ((int)wc.ResponseCode >= 300)
            {
                throw new Exception("ResponseCode >= 300: " + (int)wc.ResponseCode);
            }
            if (isShareASale) //share-a-sale doesn't 301 or 302, they use javascript
            {
                var sasLink = _findShareASaleLinkRegex.Match(contents).Value;
                url = UrlHelper.ExtractEntireHttpDomainMinusWww(sasLink);
            }
            else
            {
                url = UrlHelper.ExtractEntireHttpDomainMinusWww(wc.ResponseUri.ToString());
            }
            if (url.EndsWith(".affiliatetechnology.com"))
            {
                var affTechLink = _findAffiliateTechnologyRegex.Match(contents).Value;
                url = UrlHelper.ExtractEntireHttpDomainMinusWww(affTechLink);
            }
            if (url != null)
            {
                url = url.Trim().ToLower();
            }
            return url;
        }

    }

}
