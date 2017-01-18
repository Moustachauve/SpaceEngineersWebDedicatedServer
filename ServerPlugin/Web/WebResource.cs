using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerPlugin.Web
{
	public class WebResource
	{
		/// <summary>
		/// Path of the resource relative to the current plugin www folder
		/// </summary>
		public string Path { get; protected set; }

		public WebResource(string path)
		{
			Path = path;
		}
	}
}
