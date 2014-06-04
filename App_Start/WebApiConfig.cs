using Microsoft.Practices.Unity;
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
            var container = new UnityContainer();
            container.RegisterType<BucketManager>(new ContainerControlledLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

			config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
			config.Formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}
