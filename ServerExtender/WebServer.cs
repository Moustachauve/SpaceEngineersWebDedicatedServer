using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerExtender
{
	public class WebServer
	{
		string baseAddress = "http://localhost:9000/";
		IDisposable serverInstance;

		public void Start()
		{
			serverInstance = WebApp.Start<WebServer>(baseAddress);
		}

		public void Stop()
		{
			serverInstance.Dispose();
		}

		public void Configuration(IAppBuilder appBuilder)
		{
			// Configure Web API for self-host. 
			HttpConfiguration config = new HttpConfiguration();
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			config.Routes.MapHttpRoute(
				name: "Fallback",
				routeTemplate: "{name}/{*other}",
				defaults: new { controller = "Default", action = "Index" },
				constraints: new { name = "^(?!(api|angular|css|fonts|img|js|favicon.ico)$).*$" }
			);

			appBuilder.UseCors(CorsOptions.AllowAll);
			appBuilder.MapSignalR();
			appBuilder.UseWebApi(config);
			

			var physicalFileSystem = new PhysicalFileSystem(@"./www");
			var options = new FileServerOptions
			{
				EnableDefaultFiles = true,
				FileSystem = physicalFileSystem
			};
			options.StaticFileOptions.FileSystem = physicalFileSystem;
			options.StaticFileOptions.ServeUnknownFileTypes = true;
			options.DefaultFilesOptions.DefaultFileNames = new[]
			{
				"index.html"
            };

			config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
			config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;

			appBuilder.UseFileServer(options);
		}
	}
}
