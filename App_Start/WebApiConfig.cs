using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebRequestReflector.Models;

namespace WebRequestReflector
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

			config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
			config.Formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}
