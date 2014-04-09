using System;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;

namespace Torian.Common.Web
{
    public class TwitterReader : IDisposable
    {
        private const string url = @"http://stream.twitter.com/1/statuses/sample.json";
        private const string urlFilter = @"http://stream.twitter.com/1/statuses/filter.json";
        private readonly string _password;
        private readonly string _username;
        private bool _running;

        private TwitterReader()
        {
            _running = false;
        }

        public TwitterReader(string username, string password)
            : this()
        {
            _username = username;
            _password = password;
        }

        private string[] SearchTerms { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event OnTwitterLineReceived OnLineReceived;
        public event OnTwitterError OnError;

        public void Start()
        {
            if (_running)
                throw new InvalidOperationException(
                    "<strong class=\"highlight\">Twitter</strong> streamer is already running");

            _running = true;

            string realUrl = url;
            if (SearchTerms != null && SearchTerms.Length > 0)
            {
                realUrl = urlFilter + "?track=" + string.Join(",", SearchTerms);
            }

            var request = (HttpWebRequest) WebRequest.Create(realUrl);
            request.Method = "GET";
            request.Credentials = new NetworkCredential(_username, _password);
            request.Timeout = 86400000;
            var response = (HttpWebResponse) request.GetResponse();
            new Action<Stream>(BeginRead).BeginInvoke(
                response.GetResponseStream(),
                EndRead,
                null);
        }

        private void BeginRead(Stream stream)
        {
            try
            {
                using (stream)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        while (!sr.EndOfStream && _running)
                        {
                            string s = sr.ReadLine();
                            OnTwitterLineReceived del = OnLineReceived;
                            if (del != null)
                            {
                                del(this, new TwitterLineReceivedArgs(s));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnTwitterError del = OnError;
                if (del != null)
                {
                    del(this, new TwitterErrorArgs(ex));
                }
                throw;
            }
        }

        private void EndRead(IAsyncResult ar)
        {
            var result = (AsyncResult) ar;
            var action = (Action<Stream>) result.AsyncDelegate;
            try
            {
                action.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                OnTwitterError del = OnError;
                if (del != null)
                {
                    del(this, new TwitterErrorArgs(ex));
                }
            }
            finally
            {
                _running = false;
            }
        }

        public void Stop()
        {
            _running = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_running)
                {
                    Stop();
                }
            }
        }
    }

    public delegate void OnTwitterLineReceived(object sender, TwitterLineReceivedArgs e);

    public class TwitterLineReceivedArgs : EventArgs
    {
        private readonly string line;

        private TwitterLineReceivedArgs()
        {
        }

        public TwitterLineReceivedArgs(string Line)
            : this()
        {
            line = Line;
        }

        public string Line
        {
            get { return line; }
        }
    }

    public delegate void OnTwitterError(object sender, TwitterErrorArgs e);

    public class TwitterErrorArgs : EventArgs
    {
        private readonly Exception exception;

        private TwitterErrorArgs()
        {
        }

        public TwitterErrorArgs(Exception Exception)
            : this()
        {
            exception = Exception;
        }

        public Exception Exception
        {
            get { return exception; }
        }
    }
}