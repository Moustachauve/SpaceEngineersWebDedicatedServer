using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerPlugin.Web
{
	public class AngularControllerResource : WebResource
	{
		/// <summary>
		/// Get the route path used by AngularJS
		/// </summary>
		public string Route { get; protected set; }

		/// <summary>
		/// Get the path to the html view associated with this controller, relative to the current plugin www directory
		/// </summary>
		public string ViewPath { get; protected set; }

		public string ControllerName { get { return System.IO.Path.GetFileNameWithoutExtension(Path); } }

		/// <summary>
		/// Adds an AngularJS controller and build it's routing properties
		/// </summary>
		/// <param name="route">Route used by the AngularJS</param>
		/// <param name="controllerPath"></param>
		/// <param name="viewPath"></param>
		public AngularControllerResource(string route, string controllerPath, string viewPath) : base(controllerPath)
		{
			Route = route;
			ViewPath = viewPath;
		}
	}
}
