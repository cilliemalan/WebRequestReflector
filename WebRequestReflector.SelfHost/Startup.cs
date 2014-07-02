using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace WebRequestReflector
{
	class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			HttpConfiguration configuration = new HttpConfiguration();
			WebApiConfig.Register(configuration);
			app.UseWebApi(configuration);
			app.UseCors(CorsOptions.AllowAll);
		}
	}
}
