using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Hubs
{
	public class ServerConfigHub : Hub
	{
		public void GetConfig()
		{
			Clients.Caller.replaceConfig(DedicatedGameServer.ServerConfig);
		}

		public void ReloadConfig()
		{
			Clients.Caller.replaceConfig(DedicatedGameServer.ReloadServerConfig());
		}
	}
}
