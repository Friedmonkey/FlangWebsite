using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace FlangWebsite
{
    public class WebsiteContext
    {
        public string Page { get; set; }
        public string IpAddress { get; set; }
        public WebsiteRequest Request { get; set; }
    }
    public class WebsiteRequest
    {
        public string HttpMethod { get; set; }
        public Uri Url { get; set; }
        public string RawUrl { get; set; }
        public string ContentType { get; set; }
        public CookieCollection Cookies { get; set; }
        public Stream InputStream { get; set; }
        public NameValueCollection Headers { get; set; }
        public NameValueCollection QueryString { get; set; }
        public bool KeepAlive { get; set; }

    }
}
