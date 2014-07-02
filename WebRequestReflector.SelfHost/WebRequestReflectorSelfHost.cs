using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRequestReflector
{
	public class WebRequestReflectorSelfHost : IDisposable
	{
		private IDisposable _application;
		private StartOptions _startupOptions;

		public WebRequestReflectorSelfHost()
			:this("http://localhost:8080/")
		{
		}

		public WebRequestReflectorSelfHost(string baseUrl)
			:this(new StartOptions(baseUrl))
		{
		}

		private WebRequestReflectorSelfHost(StartOptions startupOptions)
		{
			_startupOptions = startupOptions;
		}

		public bool Started { get { return _application != null; } }
		public StartOptions StartOptions { get { return _startupOptions; } set { _startupOptions = value; } }

		public void Start()
		{
			if (_application != null) throw new InvalidOperationException("The web application is already started");


			var app = WebApp.Start<Startup>(_startupOptions);
			_application = app;
		}

		public void Stop()
		{
			if (_application == null) throw new InvalidOperationException("The web application is not started");

			var app = _application;
			_application = null;
			app.Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{

			}
		}

		~WebRequestReflectorSelfHost()
		{
			Dispose(false);
		}
	}
}
