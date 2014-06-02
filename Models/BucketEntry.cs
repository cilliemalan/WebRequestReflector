using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WebRequestReflector.Models
{
	[Serializable]
	public class BucketEntry
	{
		public List<Header> RequestHeaders { get; set; }
		public List<Header> ContentHeaders { get; set; }
		public string Method { get; set; }
		public string Contents { get; set; }
		public DateTime DateAdded { get; set; }


	}
}