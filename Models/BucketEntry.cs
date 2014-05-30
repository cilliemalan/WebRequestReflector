using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace WebRequestReflector.Models
{
    public class BucketEntry
    {
        public string Key { get; set; }
        public HttpRequestHeaders Headers { get; set; }
        public HttpMethod Method { get; set; }
        public string Contents { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset Expires { get; set; }
    }
}