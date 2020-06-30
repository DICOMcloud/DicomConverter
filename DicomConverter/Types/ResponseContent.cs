using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace DicomConverter.Types
{
    public class ResponseContent
    {
        public HttpStatusCode Status { get; set; }
        public HttpContent Content { get; set; }
        public string FileName { get; set; }

    }
}