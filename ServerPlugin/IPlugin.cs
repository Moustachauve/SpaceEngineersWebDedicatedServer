using ServerPlugin.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerPlugin
{
	public interface IPlugin
	{
		string Name { get; }
		string Version { get; }

		IList<WebResource> RequiredWebResources { get; }
	}
}
