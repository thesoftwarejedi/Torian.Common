using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Torian.Common.Web
{

    public class WebClientEx : WebClient
    {

        public const string USER_AGENT_CHROME = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.70 Safari/533.4";

        public CookieContainer CookieContainer { get; set; }

        public string ContentType { get; set; }

        Uri _responseUri;

        public Uri ResponseUri
        {
            get { return _responseUri; }
        }

        string _userAgent;

        public string UserAgent
        {
            set { _userAgent = value; }
            get { return _userAgent; }
        }

        HttpStatusCode _responseCode;

        public HttpStatusCode ResponseCode
        {
            get { return _responseCode; }
        }

        public int? RequestTimeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest && CookieContainer != null)
            {
                (request as HttpWebRequest).CookieContainer = CookieContainer;
            }
            if (ContentType != null)
            {
                request.ContentType = ContentType;
            }
            if (UserAgent != null)
            {
                ((HttpWebRequest)request).UserAgent = UserAgent;
            }

            if (RequestTimeout != null)
            {
                request.Timeout = RequestTimeout.Value;
            }
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)base.GetWebResponse(request);
            _responseUri = response.ResponseUri;
            _responseCode = response.StatusCode;
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            return GetWebResponse(request);
        }

    }

}
