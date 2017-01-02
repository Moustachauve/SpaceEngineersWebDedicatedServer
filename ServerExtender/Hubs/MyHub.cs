using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Extender.Hubs
{
	public class MyHub : Hub
	{
		public void Send(string name, string message)
		{
			Clients.All.addMessage(name, message);
		}
	}
}
