using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRequestReflector.Models
{
    [Serializable]
    public class BucketSummary
    {
        private string _id;
        private IList<BucketEntrySummary> _entries;
		private DateTime _created;
        private DateTime _expires;

        public BucketSummary(Bucket bucket)
        {
            _id = bucket.Id;
            _entries = bucket.Entries.Select((x, i) => new BucketEntrySummary(i, x)).ToList().AsReadOnly();
            _created = bucket.Created;
            _expires = bucket.Expires;
        }

        public string Id { get { return _id; } protected set { _id = value; } }
        public IList<BucketEntrySummary> Entries { get { return _entries; } protected set { _entries = value; } }
		public DateTime Created { get { return _created; } protected set { _created = value; } }
		public DateTime Expires { get { return _expires; } protected set { _expires = value; } }
    }
}