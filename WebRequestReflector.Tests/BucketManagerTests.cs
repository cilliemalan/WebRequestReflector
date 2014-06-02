using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebRequestReflector.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
		public void SerializeTest1()
		{
			var bucket1 = bucketManager.Create();
			Assert.IsNotNull(bucket1);

			var bucket2 = SerializeAndDeserialize(bucket1);
			Assert.AreEqual(bucket1.Created, bucket2.Created);
			Assert.AreEqual(bucket1.Expires, bucket2.Expires);
			Assert.AreEqual(bucket1.Id, bucket2.Id);
		}


		private T SerializeAndDeserialize<T>(T thing)
		{
			BinaryFormatter bf = new BinaryFormatter();

			using(MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, thing);
				ms.Seek(0, SeekOrigin.Begin);
				return (T)bf.Deserialize(ms);
			}
		}
	}
}
