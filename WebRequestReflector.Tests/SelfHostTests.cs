using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;

namespace WebRequestReflector.Tests
{
	[TestClass]
	public class SelfHostTests
	{
		public const string HostUrl = "http://localhost:9967/";

		[TestMethod]
		public async Task SelfHostTest()
		{
			HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(HostUrl);
			config.HostNameComparisonMode = HostNameComparisonMode.Exact;
			WebRequestReflectorSelfHost wrrsh = new WebRequestReflectorSelfHost(config);

			Assert.IsNotNull(wrrsh);

			await wrrsh.StartAsync();
			await wrrsh.StopAsync();
		}
		
		[TestMethod]
		public async Task SelfHostPingTest()
		{
			HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(HostUrl);
			config.HostNameComparisonMode = HostNameComparisonMode.Exact;
			WebRequestReflectorSelfHost wrrsh = new WebRequestReflectorSelfHost(config);

			Assert.IsNotNull(wrrsh);

			await wrrsh.StartAsync();

			using(HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
				var response = await client.PostAsync(HostUrl + "create", new StringContent(""));
				Assert.IsNotNull(response);
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.IsNotNull(response.Content);
				Assert.AreEqual("application/xml", response.Content.Headers.ContentType.MediaType);
			}

			await wrrsh.StopAsync();
		}
	}
}
