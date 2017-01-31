using ServerPlugin;
using ServerPlugin.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerExtender.Controllers
{
	public class PluginsController : ApiController
	{
		public IHttpActionResult GetRequiredWebResources()
		{
			var requiredWebResources = PluginManager.Instance.GetRequiredWebResources();

			return Json(requiredWebResources);
		}

		public IHttpActionResult GetAngularRoutes()
		{
			var requiredWebResources = PluginManager.Instance.GetAngularRoutes();

			return Json(requiredWebResources);
		}
	}
}
