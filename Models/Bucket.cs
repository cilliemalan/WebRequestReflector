using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WebRequestReflector.Models
{
    [Serializable]
	public class Bucket
    {

		public Bucket()
			:this(null)
		{
		}

		public Bucket(string id)
		{
			Id = id;
			Entries = new List<BucketEntry>();
			Created = DateTime.Now;
			Expires = Created.AddMinutes(5);
		}

		public string Id { get; set; }
		public List<BucketEntry> Entries { get; set; }
		public DateTime Created { get; set; }
		public DateTime Expires { get; set; }
	}
}