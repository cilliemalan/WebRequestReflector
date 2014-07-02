using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRequestReflector.Models
{
    public class BucketSummary
    {
        private string _id;
        private List<BucketEntrySummary> _entries;
		private DateTime _created;
		private DateTime _expires;

		public BucketSummary(Bucket bucket)
		{
			_id = bucket.Id;
			_entries = bucket.Entries.Select((x, i) => new BucketEntrySummary(i, x)).ToList();
			_created = bucket.Created;
			_expires = bucket.Expires;
		}

		public BucketSummary()
		{
		}

        public string Id { get { return _id; } set { _id = value; } }
        public List<BucketEntrySummary> Entries { get { return _entries; } set { _entries = value; } }
		public DateTime Created { get { return _created; } set { _created = value; } }
		public DateTime Expires { get { return _expires; } set { _expires = value; } }
    }
}