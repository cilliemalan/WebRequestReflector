using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebRequestReflector.Models;

namespace WebRequestReflector.Controllers
{
    public class BucketController : ApiController
    {
        BucketManager _bucketManager;

		/// <summary>
		/// Gets or Sets what would have been <see cref="ApiController.Request"/> on <see cref="ApiController"/>.
		/// </summary>
		public HttpRequestMessage RequestBase { get { return base.Request; } set { base.Request = value; } }

        public BucketController(BucketManager bucketManager)
        {
            if (bucketManager == null) throw new ArgumentNullException("bucketManager");
            _bucketManager = bucketManager;
        }

        [Route("{bucket:length(16)}")]
        [HttpDelete]
        [HttpGet]
        [HttpHead]
        [HttpOptions]
        [HttpPatch]
        [HttpPost]
        [HttpPut]
        public async new Task<BucketEntrySummary> Request(string bucket)
        {
            var bkt = GetBucket(bucket);

            BucketEntry newEntry = new BucketEntry
            {
                Contents = base.Request.Content != null ?await base.Request.Content.ReadAsStringAsync() : null,
                RequestHeaders = base.Request.Headers,
				ContentHeaders = base.Request.Content != null ? base.Request.Content.Headers : null,
                DateAdded = DateTimeOffset.Now,
                Method = base.Request.Method
            };

            bkt.Entries.Add(newEntry);

            _bucketManager.Update(bkt);

            return new BucketEntrySummary(bkt.Entries.Count - 1, newEntry);
        }

        [Route("{bucket:length(16)}/get")]
        [HttpGet]
        public BucketSummary GetAll(string bucket)
        {
            var bkt = GetBucket(bucket);

            return new BucketSummary(bkt);
        }

        [Route("{bucket:length(16)}/get/{index:int}")]
        [HttpGet]
        public BucketEntry Get(string bucket, int index)
        {
            var bkt = GetBucket(bucket);
            if (index < 0 || index >= bkt.Entries.Count) throw new ArgumentOutOfRangeException("index");
            return bkt.Entries[index];
        }

        [Route("{bucket:length(16)}/delete")]
        [HttpPost]
        public void Delete(string bucket)
        {
            var bkt = GetBucket(bucket);
            _bucketManager.Delete(bucket);
        }

        [Route("create")]
        [HttpPost]
        public BucketSummary Create()
        {
            return new BucketSummary(_bucketManager.Create());
        }

        private Bucket GetBucket(string bucket)
        {
            if (bucket == null) throw new ArgumentNullException("bucket");

            var bkt = _bucketManager.Get(bucket);
            if (bkt == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return bkt;
        }
    }
}
