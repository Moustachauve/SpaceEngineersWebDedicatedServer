using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerExtender.Hubs
{
	public class ConsoleHub : Hub
	{
		public void GetConsoleText()
		{
			Clients.Caller.consoleReplace(ConsoleHandler.Instance.Log);
		}

		public void ExecuteCommand(string command)
		{
			ConsoleHandler.Instance.ExecuteCommand("WEB\\" +Context.ConnectionId, command);
		}
	}
}
