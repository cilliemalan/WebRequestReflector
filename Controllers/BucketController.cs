using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebRequestReflector.Controllers
{
    public class BucketController : ApiController
    {
        [Route("{bucket:length(16)}")]
        [HttpDelete]
        [HttpGet]
        [HttpHead]
        [HttpOptions]
        [HttpPatch]
        [HttpPost]
        [HttpPut]
        public new HttpResponseMessage Request(string bucket, [FromBody]dynamic @object)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("{bucket:length(16)}/get")]
        [HttpGet]
        public object GetAll(string bucket)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("{bucket:length(16)}/get/{index:int}")]
        [HttpGet]
        public object Get(string bucket, int index)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("{bucket:length(16)}/delete")]
        [HttpPost]
        public HttpResponseMessage Delete(string bucket)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("create")]
        [HttpPost]
        public HttpResponseMessage Create()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
