using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Web;

namespace WebRequestReflector.Models
{
	[Serializable]
	public class BucketEntry : ISerializable
	{
		[NonSerialized]
		private HttpRequestHeaders _requestHeaders;
		[NonSerialized]
		private HttpContentHeaders _contentHeaders;
		[NonSerialized]
		private HttpMethod _method;
		[NonSerialized]
		private string _contents;
		[NonSerialized]
		private DateTimeOffset _dateAdded;

		public HttpRequestHeaders RequestHeaders { get { return _requestHeaders; } set { _requestHeaders = value; } }
		public HttpContentHeaders ContentHeaders { get { return _contentHeaders; } set { _contentHeaders = value; } }
		public HttpMethod Method { get { return _method; } set { _method = value; } }
		public string Contents { get { return _contents; } set { _contents = value; } }
		public DateTimeOffset DateAdded { get { return _dateAdded; } set { _dateAdded = value; } }

		public BucketEntry()
		{

		}

		protected BucketEntry(SerializationInfo info, StreamingContext context)
		{
			var content = new ByteArrayContent(new byte[0]);
			HttpRequestMessage request = new HttpRequestMessage();

			RequestHeaders = request.Headers;
			ContentHeaders = content.Headers;

			var ieRequestHeaders = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>)info.GetValue("RequestHeaders", typeof(IEnumerable<KeyValuePair<string, IEnumerable<string>>>));
			var ieContentHeaders = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>)info.GetValue("ContentHeaders", typeof(IEnumerable<KeyValuePair<string, IEnumerable<string>>>));

			foreach (var item in ieRequestHeaders) RequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
			foreach (var item in ieContentHeaders) ContentHeaders.TryAddWithoutValidation(item.Key, item.Value);

			var sMethod = (string)info.GetValue("Method", typeof(string));
			Method = new HttpMethod(sMethod);
			Contents = (string)info.GetValue("Contents", typeof(string));
			DateAdded = (DateTimeOffset)info.GetValue("DateAdded", typeof(DateTimeOffset));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// Use the AddValue method to specify serialized values.
			info.AddValue("RequestHeaders", RequestHeaders.ToArray(), typeof(IEnumerable<KeyValuePair<string, IEnumerable<string>>>));
			info.AddValue("ContentHeaders", ContentHeaders.ToArray(), typeof(IEnumerable<KeyValuePair<string, IEnumerable<string>>>));
			info.AddValue("Method", Method.ToString(), typeof(string));
			info.AddValue("Contents", Contents, typeof(string));
			info.AddValue("DateAdded", DateAdded, typeof(DateTimeOffset));
		}
	}
}