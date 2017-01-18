using ServerPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerPlugin.Web;

namespace EntityListPlugin
{
	public class EntityListPlugin : IPlugin
	{
		public string Name { get { return "Server Entity List"; } }
		public string Version { get { return "1.0"; } }

		private List<WebResource> requiredWebResources;
		public IList<WebResource> RequiredWebResources { get { return requiredWebResources.AsReadOnly(); } }

		public EntityListPlugin()
		{
			SetupWebResources();
		}

		private void SetupWebResources()
		{
			requiredWebResources = new List<WebResource>()
			{
				new AngularControllerResource("entities", "entitiesController.js", "entitiesList.html")
			};
		}
	}
}
