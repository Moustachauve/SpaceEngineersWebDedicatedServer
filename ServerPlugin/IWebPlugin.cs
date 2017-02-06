using ServerPlugin.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerPlugin
{
	public interface IWebPlugin
	{
		IList<WebResource> RequiredWebResources { get; }
		WebResource DefaultRoute { get; }
	}
}
