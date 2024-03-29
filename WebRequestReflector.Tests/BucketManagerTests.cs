﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRequestReflector.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Text;

namespace WebRequestReflector.Tests
{
	[TestClass]
	public class BucketManagerTests
	{
		BucketManager bucketManager = new BucketManager();

		[TestMethod]
		public void CreateTest()
		{
			DateTime now = DateTime.UtcNow;
			var bucket = bucketManager.Create();

			Assert.IsNotNull(bucket);
			Assert.IsNotNull(bucket.Id);
			Assert.AreNotEqual("", bucket.Id);
			Assert.IsTrue(Math.Abs((bucket.Created - now).TotalSeconds) < 1);
			Assert.IsTrue(Math.Abs((bucket.Expires - now).TotalMinutes - 5) < 1);
		}

		[TestMethod]
		public void GetTest()
		{
			var bucket1 = bucketManager.Create();
			Assert.IsNotNull(bucket1);
			Assert.IsNotNull(bucket1.Id);

			var bucket2 = bucketManager.Get(bucket1.Id);
			Assert.IsNotNull(bucket2);
			Assert.AreEqual(bucket1.Created, bucket2.Created);
			Assert.AreEqual(bucket1.Expires, bucket2.Expires);
			Assert.AreEqual(bucket1.Id, bucket2.Id);
		}

		[TestMethod]
		public void GetNonExistingTest()
		{
			var bucket = bucketManager.Get("zzzz");
			Assert.IsNull(bucket);
		}

		[TestMethod]
		public void DeleteTest()
		{
			var bucket1 = bucketManager.Create();
			Assert.IsNotNull(bucket1);
			Assert.IsNotNull(bucket1.Id);

			var bucket2 = bucketManager.Get(bucket1.Id);
			Assert.IsNotNull(bucket2);

			bucketManager.Delete(bucket1.Id);

			var bucket3 = bucketManager.Get(bucket1.Id);
			Assert.IsNull(bucket3);
		}

		[TestMethod]
		public async Task SerializeTest1Xml()
		{
			var bucket1 = bucketManager.Create();
			Assert.IsNotNull(bucket1);

			var bucket2 = await SerializeAndDeserializeXml(bucket1);
			Assert.IsNotNull(bucket2);
			Assert.AreEqual(bucket1.Created, bucket2.Created);
			Assert.AreEqual(bucket1.Expires, bucket2.Expires);
			Assert.AreEqual(bucket1.Id, bucket2.Id);
		}

		[TestMethod]
		public async Task SerializeTest2Xml()
		{
			var bucketEntry1 = new BucketEntry
			{
				DateAdded = DateTime.UtcNow.AddSeconds(-2),
				Method = HttpMethod.Get.ToString(),
				ContentHeaders = CreateSomeContentHeaders(),
				RequestHeaders = CreateSomeRequestHeaders(),
				Contents = "blahblahblah"
			};

			Assert.IsNotNull(bucketEntry1);
			Assert.IsNotNull(bucketEntry1.ContentHeaders);
			Assert.IsNotNull(bucketEntry1.RequestHeaders);

			var bucketEntry2 = await SerializeAndDeserializeXml(bucketEntry1);

			Assert.IsNotNull(bucketEntry2);
			Assert.IsNotNull(bucketEntry2.ContentHeaders);
			Assert.IsNotNull(bucketEntry2.RequestHeaders);
			Assert.AreEqual(bucketEntry1.DateAdded, bucketEntry2.DateAdded);
			Assert.AreEqual(bucketEntry1.Method, bucketEntry2.Method);
			Assert.AreEqual(bucketEntry1.Contents, bucketEntry2.Contents);
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.ContentHeaders, bucketEntry2.ContentHeaders));
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.RequestHeaders, bucketEntry2.RequestHeaders));
		}

		[TestMethod]
		public async Task SerializeTest3Xml()
		{
			var bucket1 = bucketManager.Create();
			Assert.IsNotNull(bucket1);

			var bucketEntry1 = new BucketEntry
			{
				DateAdded = DateTime.UtcNow.AddSeconds(-2),
				Method = HttpMethod.Get.ToString(),
				ContentHeaders = CreateSomeContentHeaders(),
				RequestHeaders = CreateSomeRequestHeaders(),
				Contents = "blahblahblah"
			};

			bucket1.Entries.Add(bucketEntry1);

			var bucket2 = await SerializeAndDeserializeXml(bucket1);
			Assert.IsNotNull(bucket2);
			Assert.AreEqual(bucket1.Created, bucket2.Created);
			Assert.AreEqual(bucket1.Expires, bucket2.Expires);
			Assert.AreEqual(bucket1.Entries.Count, bucket2.Entries.Count);
			Assert.AreEqual(bucket1.Id, bucket2.Id);

			var bucketEntry2 = bucket2.Entries[0];
			Assert.IsNotNull(bucketEntry2);
			Assert.IsNotNull(bucketEntry2.ContentHeaders);
			Assert.IsNotNull(bucketEntry2.RequestHeaders);
			Assert.AreEqual(bucketEntry1.DateAdded, bucketEntry2.DateAdded);
			Assert.AreEqual(bucketEntry1.Method, bucketEntry2.Method);
			Assert.AreEqual(bucketEntry1.Contents, bucketEntry2.Contents);
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.ContentHeaders, bucketEntry2.ContentHeaders));
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.RequestHeaders, bucketEntry2.RequestHeaders));
		}

		[TestMethod]
		public async Task SerializeTest1Json()
		{
			var bucket1 = bucketManager.Create();
			Assert.IsNotNull(bucket1);

			var bucket2 = await SerializeAndDeserializeJson(bucket1);
			Assert.IsNotNull(bucket2);
			Assert.IsTrue(Math.Abs((bucket1.Created - bucket2.Created).TotalSeconds) < 1);
			Assert.IsTrue(Math.Abs((bucket1.Expires - bucket2.Expires).TotalSeconds) < 1);
			Assert.AreEqual(bucket1.Id, bucket2.Id);
		}

		[TestMethod]
		public async Task SerializeTest2Json()
		{
			var bucketEntry1 = new BucketEntry
			{
				DateAdded = DateTime.UtcNow.AddSeconds(-2),
				Method = HttpMethod.Get.ToString(),
				ContentHeaders = CreateSomeContentHeaders(),
				RequestHeaders = CreateSomeRequestHeaders(),
				Contents = "blahblahblah"
			};

			Assert.IsNotNull(bucketEntry1);
			Assert.IsNotNull(bucketEntry1.ContentHeaders);
			Assert.IsNotNull(bucketEntry1.RequestHeaders);

			var bucketEntry2 = await SerializeAndDeserializeJson(bucketEntry1);

			Assert.IsNotNull(bucketEntry2);
			Assert.IsNotNull(bucketEntry2.ContentHeaders);
			Assert.IsNotNull(bucketEntry2.RequestHeaders);
			Assert.IsTrue(Math.Abs((bucketEntry1.DateAdded - bucketEntry2.DateAdded).TotalSeconds) < 1);
			Assert.AreEqual(bucketEntry1.DateAdded, bucketEntry2.DateAdded);
			Assert.AreEqual(bucketEntry1.Method, bucketEntry2.Method);
			Assert.AreEqual(bucketEntry1.Contents, bucketEntry2.Contents);
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.ContentHeaders, bucketEntry2.ContentHeaders));
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.RequestHeaders, bucketEntry2.RequestHeaders));
		}

		[TestMethod]
		public async Task SerializeTest3Json()
		{
			var bucket1 = bucketManager.Create();
			Assert.IsNotNull(bucket1);

			var bucketEntry1 = new BucketEntry
			{
				DateAdded = DateTime.UtcNow.AddSeconds(-2),
				Method = HttpMethod.Get.ToString(),
				ContentHeaders = CreateSomeContentHeaders(),
				RequestHeaders = CreateSomeRequestHeaders(),
				Contents = "blahblahblah"
			};

			bucket1.Entries.Add(bucketEntry1);

			var bucket2 = await SerializeAndDeserializeJson(bucket1);
			Assert.IsNotNull(bucket2);
			Assert.IsTrue(Math.Abs((bucket1.Created - bucket2.Created).TotalSeconds) < 1);
			Assert.IsTrue(Math.Abs((bucket1.Expires - bucket2.Expires).TotalSeconds) < 1);
			Assert.AreEqual(bucket1.Entries.Count, bucket2.Entries.Count);
			Assert.AreEqual(bucket1.Id, bucket2.Id);

			var bucketEntry2 = bucket2.Entries[0];
			Assert.IsNotNull(bucketEntry2);
			Assert.IsNotNull(bucketEntry2.ContentHeaders);
			Assert.IsNotNull(bucketEntry2.RequestHeaders);
			Assert.IsTrue(Math.Abs((bucketEntry1.DateAdded - bucketEntry2.DateAdded).TotalSeconds) < 1);
			Assert.AreEqual(bucketEntry1.Method, bucketEntry2.Method);
			Assert.AreEqual(bucketEntry1.Contents, bucketEntry2.Contents);
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.ContentHeaders, bucketEntry2.ContentHeaders));
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.RequestHeaders, bucketEntry2.RequestHeaders));
		}

		[TestMethod]
		public async Task EntriesTest()
		{
			var bucket1 = bucketManager.Create();
			Assert.IsNotNull(bucket1);

			var bucket2 = await SerializeAndDeserializeXml(bucket1);
			Assert.IsNotNull(bucket2);

			BucketEntry bucketEntry1 = new BucketEntry
			{
				DateAdded = DateTime.UtcNow.AddSeconds(-2),
				Method = HttpMethod.Get.ToString(),
				ContentHeaders = CreateSomeContentHeaders(),
				RequestHeaders = CreateSomeRequestHeaders(),
				Contents = "blahblahblah"
			};

			BucketEntry bucketEntry2 = new BucketEntry
			{
				DateAdded = DateTime.UtcNow.AddSeconds(-1),
				Method = HttpMethod.Get.ToString(),
				ContentHeaders = CreateSomeContentHeaders(),
				RequestHeaders = CreateSomeRequestHeaders(),
				Contents = "blahblahblah2"
			};

			bucket2.Entries.Add(bucketEntry1);
			bucket2.Entries.Add(bucketEntry2);

			bucketManager.Update(bucket2);

			var bucket3 = bucketManager.Get(bucket1.Id);
			Assert.IsNotNull(bucket3);
			Assert.AreEqual(bucket2.Created, bucket3.Created);
			Assert.AreEqual(bucket2.Expires, bucket3.Expires);
			Assert.AreEqual(bucket2.Entries.Count, bucket3.Entries.Count);
			Assert.AreEqual(bucket2.Id, bucket3.Id);


			var bucketEntry3 = bucket3.Entries[0];
			Assert.IsNotNull(bucketEntry3);
			Assert.IsNotNull(bucketEntry3.ContentHeaders);
			Assert.IsNotNull(bucketEntry3.RequestHeaders);
			Assert.AreEqual(bucketEntry1.DateAdded, bucketEntry3.DateAdded);
			Assert.AreEqual(bucketEntry1.Method, bucketEntry3.Method);
			Assert.AreEqual(bucketEntry1.Contents, bucketEntry3.Contents);
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.ContentHeaders, bucketEntry3.ContentHeaders));
			Assert.IsTrue(HeadersAreEqual(bucketEntry1.RequestHeaders, bucketEntry3.RequestHeaders));
		}

		private class BucketEntryEqualityComparer : EqualityComparer<BucketEntry>
		{

			public override bool Equals(BucketEntry x, BucketEntry y)
			{
				return x.Contents == y.Contents &&
					x.DateAdded == y.DateAdded &&
					x.Method == y.Method &&
					HeadersAreEqual(x.RequestHeaders, y.RequestHeaders) &&
					HeadersAreEqual(x.ContentHeaders, y.ContentHeaders);
			}

			public override int GetHashCode(BucketEntry obj)
			{
				return string.Join("::",
					obj.Contents,
					obj.DateAdded,
					obj.Method,
					obj.RequestHeaders,
					obj.ContentHeaders).GetHashCode();
			}
		}

		public bool EntriesEqual(IEnumerable<BucketEntry> entries1, IEnumerable<BucketEntry> entries2)
		{
			int i1 = 0;
			int i2 = 0;
			int r = 0;

			var joined = entries1.Join(entries2,
				x => { i1++; return x; },
				x => { i2++; return x; },
				(a, b) => r++,
				new BucketEntryEqualityComparer());

			return i1 == i2 && i1 == r;
		}


		public static bool HeadersAreEqual(List<Header> expected, List<Header> actual)
		{
			if (expected == null) return expected == null && actual == null;
			else
			{
				HashSet<Header> h_expected = new HashSet<Header>(expected);
				HashSet<Header> h_actual = new HashSet<Header>(actual);

				return h_expected.SetEquals(h_actual);
			}
		}

		public static Headers CreateSomeContentHeaders()
		{
			HttpContentHeaders headers = CreateHttpContentHeaders();
			headers.ContentType = new MediaTypeHeaderValue("text/plain");
			headers.Expires = DateTime.UtcNow.AddDays(1);
			headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "whatever.txt" };
			headers.LastModified = DateTime.UtcNow;

			return FixHeaders(headers);
		}

		public static Headers CreateSomeRequestHeaders()
		{
			HttpRequestHeaders headers = CreateHttpRequestHeaders();
			headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml") { CharSet = "utf-8" });
			headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
			headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
			headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain", 0.8));
			headers.ConnectionClose = true;
			headers.Host = "chills.co.za";
			headers.CacheControl = new CacheControlHeaderValue() { Public = true };
			headers.Referrer = new Uri("http://www.google.co.za");

			return FixHeaders(headers);
		}

		public static async Task<T> SerializeAndDeserializeXml<T>(T thing)
		{
			return await SerializeAndDeserialize(thing, new XmlMediaTypeFormatter());
		}

		public static async Task<T> SerializeAndDeserializeJson<T>(T thing)
		{
			return await SerializeAndDeserialize(thing, new JsonMediaTypeFormatter());
		}

		public static async Task<T> SerializeAndDeserialize<T>(T thing, MediaTypeFormatter formatter)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				await formatter.WriteToStreamAsync(typeof(T), thing, ms, null, null);
				ms.Seek(0, SeekOrigin.Begin);
				Console.Write(Encoding.UTF8.GetString(ms.ToArray()));
				return (T)await formatter.ReadFromStreamAsync(typeof(T), ms, null, null);
			}
		}

		public static HttpRequestHeaders CreateHttpRequestHeaders()
		{
			HttpRequestMessage request = new HttpRequestMessage();
			return request.Headers;
		}

		public static HttpContentHeaders CreateHttpContentHeaders()
		{
			var content = new ByteArrayContent(new byte[0]);
			return content.Headers;
		}


		public static Headers FixHeaders(HttpHeaders headers)
		{
			if (headers == null) return null;
			return new Headers(headers);
		}
	}
}
