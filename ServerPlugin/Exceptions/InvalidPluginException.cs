using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerPlugin.Exceptions
{
	public class InvalidPluginException : Exception
	{
		public InvalidPluginException() : base()
		{
		}

		public InvalidPluginException(string message) : base(message)
		{
		}

		public InvalidPluginException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
