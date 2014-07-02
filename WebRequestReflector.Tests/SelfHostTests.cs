using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;

namespace WebRequestReflector.Tests
{
	[TestClass]
	public class SelfHostTests
	{
		[TestMethod]
		public async Task SelfHostTest()
		{
			HttpSelfHostConfiguration config = new HttpSelfHostConfiguration("http://localhost:9967/");
			config.HostNameComparisonMode = HostNameComparisonMode.Exact;
			WebRequestReflectorSelfHost wrrsh = new WebRequestReflectorSelfHost(config);

			Assert.IsNotNull(wrrsh);

			await wrrsh.StartAsync();
			await wrrsh.StopAsync();
		}
	}
}
