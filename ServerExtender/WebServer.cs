using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerExtender
{
	public class WebServer
	{
		string baseAddress = "http://localhost:9000/";
		IDisposable serverInstance;

		public void Start()
		{
			serverInstance = WebApp.Start<WebServerConfig>(url: baseAddress);
		}

		public void Stop()
		{
			serverInstance.Dispose();
		}
	}
}
