using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRequestReflector.Controllers;
using WebRequestReflector.Models;

namespace WebRequestReflector.Tests
{
	[TestClass]
	public class BucketControllerTests
	{
		private BucketController controller = new BucketController(new BucketManager());

		[TestMethod]
		public void CreateTest()
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
	}
}
