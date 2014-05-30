using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRequestReflector.Models;

namespace WebRequestReflector.Tests
{
	[TestClass]
	public class BucketManagerTests
	{
		BucketManager bucketManager = new BucketManager();

		[TestMethod]
		public void CreateTest()
		{
			DateTimeOffset now = DateTimeOffset.Now;
			var bucket = bucketManager.Create();

			Assert.IsNotNull(bucket);
			Assert.IsNotNull(bucket.Id);
			Assert.AreNotEqual("", bucket.Id);
			Assert.IsTrue(Math.Abs((bucket.Created - now).TotalSeconds) < 0.01);
			Assert.IsTrue(Math.Abs((bucket.Expires - now).TotalMinutes - 5) < 0.01);
		}
	}
}
