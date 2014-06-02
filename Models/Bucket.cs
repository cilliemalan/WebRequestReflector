using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace WebRequestReflector.Models
{
    [Serializable]
    public class Bucket
    {
        private string _id;
        private List<BucketEntry> _entries;
        private DateTimeOffset _created;
        private DateTimeOffset _expires;


		public Bucket()
		{
            _id = null;
            _entries = new List<BucketEntry>();
            _created = DateTimeOffset.Now;
            _expires = _created.AddMinutes(5);
		}

        public Bucket(string id)
        {
            _id = id;
            _entries = new List<BucketEntry>();
            _created = DateTimeOffset.Now;
            _expires = _created.AddMinutes(5);
        }

        public string Id { get { return _id; } protected set { _id = value; } }
        public List<BucketEntry> Entries { get { return _entries; } protected set { _entries = value; } }
        public DateTimeOffset Created { get { return _created; } protected set { _created = value; } }
        public DateTimeOffset Expires { get { return _expires; } protected set { _expires = value; } }
    }
}