using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using Torian.Common.Extensions;
using Torian.Common.Tracing;
using Torian.Common;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Torian.Common.ExternalApiHelpers
{

    public class FacebookLookup
    {

        private static string applicationKey = ConfigurationSettings.AppSettings.Get("ApiKey");
        private static string secretKey = ConfigurationSettings.AppSettings.Get("Secret");
        private static string appId = ConfigurationSettings.AppSettings.Get("AppId");

        public static string GetAccessToken()
        {
            var info = GetInfoFromCookie();
            if (info == null) return null;
            return info["access_token"];
        }

        public static string GetSessionKey()
        {
            var info = GetInfoFromCookie();
            if (info == null) return null;
            return info["session_key"];
        }

        private static Dictionary<string, string> GetInfoFromCookie()
        {
            string cookie = HttpContext.Current.Request.Cookies["fbs_" + appId].Value;
            if (cookie == null) return null;
            //cookie looks like this:
            //"access_token=10150093249885195%7C2.INZDbiKgrXnR4QsVGa0Vpw__.3600.1275433200-515778541%7CNLxw26cHVaowSj_X4BMWmlZHNyk.&base_domain=edeems.com&expires=1275433200&secret=SNDG62JQ2oOa_BS_U1yqEQ__&session_key=2.INZDbiKgrXnR4QsVGa0Vpw__.3600.1275433200-515778541&sig=76adea5af484d39df624495f69cdd9e9&uid=515778541"
            Dictionary<string, string> info = Utility.DictionaryFromUrlParamString(cookie.Trim('"'));
            //check the expiry
            var expiry = info["expires"].To<int>();
            if (expiry > 0 && DateTime.Now.GetEpochTime() >= expiry)
            {
                ClearFacebookCookies();
                return null;
            }
            //TODO validate sig too!
            return info;
        }

        public static long? GetFacebookUserId()
        {
            var info = GetInfoFromCookie();
            if (info == null) return null;
            string uid = info["uid"];
            if (uid == null) return null;
            return uid.ToOrDefault<long?>();
        }

        public static User GetFacebookUser()
        {
            long? uid = GetFacebookUserId();
            if (uid == null) return null;
            string url = "https://graph.facebook.com/me?access_token=" + GetInfoFromCookie()["access_token"];
            string results = url.DownloadUrl();
            JavaScriptSerializer ds = new JavaScriptSerializer();
            var u = ds.Deserialize<User>(results);
            u.FirstName = results;
            return u;
        }

        public static void ClearFacebookCookies()
        {
            string[] cookies = new[] { "fbs" };
            foreach (var c in cookies)
            {
                string fullCookie = c + "_" + appId;

                if (HttpContext.Current != null &&
                    HttpContext.Current.Response.Cookies[fullCookie] != null)
                {
                    HttpContext.Current.Response.Cookies[fullCookie].Expires = DateTime.Now.AddMonths(-1);
                }
            }
        }

        public static Post GetPost(string postId, string accessToken)
        {
            string url = "https://graph.facebook.com/postId?access_token=" + accessToken;
            string results = url.DownloadUrl();
            if (results.Contains("(#2500)")) return null;
            JavaScriptSerializer ds = new JavaScriptSerializer();
            var u = ds.Deserialize<Post>(results);
            return u;
        }

        [DataContract]
        public class User
        {

            [DataMember(Name = "id")]
            public string Id;

            [DataMember(Name = "name")]
            public string Name;

            [DataMember(Name = "first_name")]
            public string FirstName;

            [DataMember(Name = "last_name")]
            public string LastName;

            [DataMember(Name = "email", IsRequired = false)]
            public string Email;

        }

        [DataContract]
        public class Post
        {

            [DataMember(Name = "message")]
            public string Message;

        }

    }

}
