using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebRequestReflector.Controllers;
using WebRequestReflector.Models;

namespace WebRequestReflector.Tests
{
	[TestClass]
	public class BucketControllerTests
	{
		private BucketController controller = new BucketController(new BucketManager());

		[TestMethod]
		public void ControllerCreateTest()
		{
			DateTimeOffset now = DateTimeOffset.Now;
			BucketSummary summary = controller.Create();
			Assert.IsNotNull(summary);
			Assert.IsNotNull(summary.Id);
			Assert.AreNotEqual("", summary.Id);
			Assert.IsNotNull(summary.Entries);
			Assert.AreEqual(0, summary.Entries.Count);
			Assert.IsTrue(Math.Abs((summary.Created - now).TotalSeconds) < 0.01);
			Assert.IsTrue(Math.Abs((summary.Expires - now).TotalMinutes - 5) < 0.01);
		}

		[TestMethod]
		public void ControllerGetTest()
		{
			BucketSummary bucket1 = controller.Create();
			Assert.IsNotNull(bucket1);
			Assert.IsNotNull(bucket1.Id);

			BucketSummary bucket2 = controller.GetAll(bucket1.Id);
			Assert.IsNotNull(bucket2);
			Assert.AreEqual(bucket1.Created, bucket2.Created);
			Assert.AreEqual(bucket1.Expires, bucket2.Expires);
			Assert.AreEqual(bucket1.Id, bucket2.Id);
		}

		[TestMethod]
		public void ControllerGetNonExistingTest()
		{
			try
			{
				var bucket = controller.GetAll("zzzz");
				Assert.Fail();
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
			}
		}

		[TestMethod]
		public void DeleteTest()
		{
			BucketSummary bucket1 = controller.Create();
			Assert.IsNotNull(bucket1);
			Assert.IsNotNull(bucket1.Id);

			BucketSummary bucket2 = controller.GetAll(bucket1.Id);
			Assert.IsNotNull(bucket2);

			controller.Delete(bucket1.Id);

			try
			{
				var bucket = controller.GetAll(bucket1.Id);
				Assert.Fail();
			}
			catch (HttpResponseException ex)
			{
				Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
			}
		}

		[TestMethod]
		public async Task RequestTest()
		{
			BucketSummary bucket = controller.Create();
			Assert.IsNotNull(bucket);
			Assert.IsNotNull(bucket.Id);

			controller.RequestBase = CreateRequest();
			string requestData = await controller.RequestBase.Content.ReadAsStringAsync();

			BucketEntrySummary summary = await controller.Request(bucket.Id);
			Assert.IsNotNull(summary);
			Assert.AreEqual(controller.RequestBase.Method.ToString(), summary.Method);
			Assert.AreEqual(requestData.Length, summary.Length);
			Assert.AreEqual(0, summary.Index);
		}

		[TestMethod]
		public async Task MultipleRequestTest()
		{
			BucketSummary bucket = controller.Create();
			Assert.IsNotNull(bucket);
			Assert.IsNotNull(bucket.Id);

			controller.RequestBase = CreateRequest();
			string requestData1 = await controller.RequestBase.Content.ReadAsStringAsync();

			BucketEntrySummary summary1 = await controller.Request(bucket.Id);
			Assert.IsNotNull(summary1);
			Assert.AreEqual(controller.RequestBase.Method.ToString(), summary1.Method);
			Assert.AreEqual(requestData1.Length, summary1.Length);
			Assert.AreEqual(0, summary1.Index);


			controller.RequestBase = CreateRequest();
			controller.RequestBase.Method = HttpMethod.Put;
			controller.RequestBase.Content = new StringContent("Hello, again, World");
			string requestData2 = await controller.RequestBase.Content.ReadAsStringAsync();

			BucketEntrySummary summary2 = await controller.Request(bucket.Id);
			Assert.IsNotNull(summary2);
			Assert.AreEqual(controller.RequestBase.Method.ToString(), summary2.Method);
			Assert.AreEqual(requestData2.Length, summary2.Length);
			Assert.AreEqual(1, summary2.Index);
		}

		[TestMethod]
		public async Task GetEntryTest()
		{
			BucketSummary bucket = controller.Create();
			Assert.IsNotNull(bucket);
			Assert.IsNotNull(bucket.Id);

			controller.RequestBase = CreateRequest();
			string requestData = await controller.RequestBase.Content.ReadAsStringAsync();

			BucketEntrySummary summary = await controller.Request(bucket.Id);
			Assert.IsNotNull(summary);
			Assert.AreEqual(controller.RequestBase.Method.ToString(), summary.Method);
			Assert.AreEqual(requestData.Length, summary.Length);
			Assert.AreEqual(0, summary.Index);

			var entry = controller.Get(bucket.Id, summary.Index);
			Assert.IsNotNull(entry);
			Assert.AreEqual(summary.DateAdded, entry.DateAdded);
			Assert.AreEqual(summary.Method, entry.Method);
			Assert.AreEqual(requestData, entry.Contents);
			Assert.AreEqual(summary.Length, entry.Contents.Length);
			Assert.IsTrue(BucketManagerTests.HeadersAreEqual(BucketManagerTests.FixHeaders(controller.RequestBase.Headers), entry.RequestHeaders));
			Assert.IsTrue(BucketManagerTests.HeadersAreEqual(BucketManagerTests.FixHeaders(controller.RequestBase.Content.Headers), entry.ContentHeaders));
		}

		[TestMethod]
		public async Task GetMultipleEntryTest()
		{
			BucketSummary bucket = controller.Create();
			Assert.IsNotNull(bucket);
			Assert.IsNotNull(bucket.Id);

			var request1 = controller.RequestBase = CreateRequest();
			string requestData1 = await controller.RequestBase.Content.ReadAsStringAsync();

			BucketEntrySummary summary1 = await controller.Request(bucket.Id);
			Assert.IsNotNull(summary1);
			Assert.AreEqual(controller.RequestBase.Method.ToString(), summary1.Method);
			Assert.AreEqual(requestData1.Length, summary1.Length);
			Assert.AreEqual(0, summary1.Index);


			var request2 = controller.RequestBase = CreateRequest();
			controller.RequestBase.Method = HttpMethod.Put;
			controller.RequestBase.Content = new StringContent("Hello, again, World");
			string requestData2 = await controller.RequestBase.Content.ReadAsStringAsync();

			BucketEntrySummary summary2 = await controller.Request(bucket.Id);
			Assert.IsNotNull(summary2);
			Assert.AreEqual(controller.RequestBase.Method.ToString(), summary2.Method);
			Assert.AreEqual(requestData2.Length, summary2.Length);
			Assert.AreEqual(1, summary2.Index);


			var entry1 = controller.Get(bucket.Id, summary1.Index);
			Assert.IsNotNull(entry1);
			Assert.AreEqual(summary1.DateAdded, entry1.DateAdded);
			Assert.AreEqual(summary1.Method, entry1.Method);
			Assert.AreEqual(requestData1, entry1.Contents);
			Assert.AreEqual(summary1.Length, entry1.Contents.Length);
			Assert.IsTrue(BucketManagerTests.HeadersAreEqual(BucketManagerTests.FixHeaders(request1.Headers), entry1.RequestHeaders));
			Assert.IsTrue(BucketManagerTests.HeadersAreEqual(BucketManagerTests.FixHeaders(request1.Content.Headers), entry1.ContentHeaders));

			var entry2 = controller.Get(bucket.Id, summary2.Index);
			Assert.IsNotNull(entry2);
			Assert.AreEqual(summary2.DateAdded, entry2.DateAdded);
			Assert.AreEqual(summary2.Method, entry2.Method);
			Assert.AreEqual(requestData2, entry2.Contents);
			Assert.AreEqual(summary2.Length, entry2.Contents.Length);
			Assert.IsTrue(BucketManagerTests.HeadersAreEqual(BucketManagerTests.FixHeaders(request2.Headers), entry2.RequestHeaders));
			Assert.IsTrue(BucketManagerTests.HeadersAreEqual(BucketManagerTests.FixHeaders(request2.Content.Headers), entry2.ContentHeaders));
		}

		private HttpRequestMessage CreateRequest()
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/whatever")
			{
				Content = new StringContent("Hello World")
			};

			var requestHeaders = BucketManagerTests.CreateSomeRequestHeaders();
			var contentHeaders = BucketManagerTests.CreateSomeContentHeaders();

			foreach (var header in requestHeaders.GroupBy(x => x.Key)) request.Headers.TryAddWithoutValidation(header.Key, header.Select(x=>x.Value));
			foreach (var header in contentHeaders.GroupBy(x => x.Key)) request.Content.Headers.TryAddWithoutValidation(header.Key, header.Select(x => x.Value));

			return request;
		}
	}
}
